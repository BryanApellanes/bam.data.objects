using System.Reflection;
using Bam.Data.Repositories;
using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectWriter : IObjectWriter
{
    public ObjectWriter(IObjectStorageManager objectStorageManager, IObjectHashCalculator objectHashCalculator, IObjectPropertyWriter objectPropertyWriter)
    {
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectHashCalculator = objectHashCalculator;
        this.ObjectPropertyWriter = objectPropertyWriter;
        this.Process = (owr) => this.WriteAsync(owr);
    }
    
    protected BackgroundThreadQueue<ObjectWriteRequest> BackgroundThreadQueue { get; set; }
    protected IObjectStorageManager ObjectStorageManager { get; set; }
    protected IObjectHashCalculator ObjectHashCalculator { get; set; }
    protected IObjectPropertyWriter ObjectPropertyWriter { get; set; }
    
    private Action<ObjectWriteRequest> _process;
    protected Action<ObjectWriteRequest> Process
    {
        get
        {
            if (_process == null)
            {
                throw new InvalidOperationException("Process Action not set");
            }

            return _process;
        }
        set
        {
            _process = value;
            BackgroundThreadQueue = new BackgroundThreadQueue<ObjectWriteRequest>(_process);
            BackgroundThreadQueue.QueueEmptied += (sender, args) => QueueEmptied?.Invoke(this, args);
        } 
    }

    public void Enqueue(object data)
    {
        if (data == null)
        {
            return;
        }
        Enqueue(data.GetType(), data);
    }

    public void Enqueue(Type type, object data)
    {
        if (type == null || data == null)
        {
            return;
        }
        BackgroundThreadQueue.Enqueue(new ObjectWriteRequest
        {
            Type = type,
            Data = data
        });
    }

    public Task WriteAsync(object data)
    {
        if (data == null)
        {
            return Task.CompletedTask;
        }
        return WriteAsync(data.GetType(), data);
    }

    protected async Task WriteAsync(ObjectWriteRequest objectWriteRequest)
    {
        WriteObjectStarted?.Invoke(this, EventArgs.Empty); // TODO: send event args
        object data = objectWriteRequest.Data;
        List<Task> writeTasks = new List<Task>();
        await Task.Run(() => Parallel.ForEach(objectWriteRequest.Type.GetProperties(), async prop =>
            {
                PropertyInfo property = prop;
                object value = prop.GetValue(data);
                writeTasks.Add(WritePropertyAsync(property, value, data));
            })
        );
        Task.WaitAll(writeTasks.ToArray());
        WriteObjectComplete?.Invoke(this, EventArgs.Empty); // TODO: send event args
    }
    
    public async Task WriteAsync(Type type, object data)
    {
        await WriteAsync(new ObjectWriteRequest
        {
            Type = type,
            Data = data
        });
    }

    public async Task WritePropertyAsync(PropertyInfo prop, object propertyValue, object parentData)
    {
        await ObjectPropertyWriter.WritePropertyAsync(prop, propertyValue, parentData);
    }

    public bool Delete(object data, Type type = null)
    {
        throw new NotImplementedException();
    }
    
    public event EventHandler QueueEmptied;
    
    public event EventHandler WriteObjectStarted;
    public event EventHandler WriteObjectComplete;
    
    public event EventHandler? WriteObjectFailed;
    public event EventHandler? WriteObjectPropertiesFailed;
    public event EventHandler? DeleteFailed;
}