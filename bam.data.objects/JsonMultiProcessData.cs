/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Text;
using System.Diagnostics;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects
{
    /// <summary>
    /// Provides multi-process-safe read/write access to JSON-serialized data files using file-based locking.
    /// </summary>
    public class JsonMultiProcessData : RawData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMultiProcessData"/> class, encoding the data as JSON.
        /// </summary>
        /// <param name="data">The data object to serialize and manage.</param>
        /// <param name="encoding">The text encoding to use, or null for the default.</param>
        public JsonMultiProcessData(object data, Encoding encoding = null) : base(JsonObjectDataEncoder.Default.Encode(data).Value, encoding)
        {         
            Args.ThrowIfNull(data, nameof(data));
            LockTimeout = 150;
            AcquireLockRetryInterval = 50;
            ObjectDataEncoder = JsonObjectDataEncoder.Default;
            DataType = data.GetType();
        }

        private JsonObjectDataEncoder ObjectDataEncoder
        {
            get;
            set;
        }
        
        /// <summary>
        /// Writes the specified data to the file system using a file-based lock for multi-process safety.
        /// </summary>
        /// <param name="data">The data object to write.</param>
        /// <returns>True if the lock was acquired and data was written; false if the lock timed out.</returns>
        public virtual bool Write(object data)
        {
            if(AcquireLock(LockTimeout))
            {
                // if the message file doesn't exist write to it
                string writeTo = DataFile;
                if (File.Exists(DataFile))
                {
                    //  else write to the WriteFile
                    writeTo = WriteFile;
                }

                IObjectEncoding encoding = ObjectDataEncoder.Encode(data);
                
                File.WriteAllBytes(writeTo, encoding.Value);

                // if WriteFile exists move it on top of MessageFile
                if (File.Exists(WriteFile))
                {
                    File.Delete(DataFile);
                    File.Move(WriteFile, DataFile);
                }

                // copy MessageFile to ReadFile
                File.Copy(DataFile, ReadFile, true);
                File.Move(LockFile, TempLockFile);
                File.Delete(TempLockFile);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Reads and deserializes the data from the read-copy file.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the data as.</typeparam>
        /// <returns>The deserialized data, or default if the read file does not exist.</returns>
        public T Read<T>()
        {            
            if (File.Exists(ReadFile))
            {
                return ReadFile.DecodeFromFile<T>();
            }

            return default(T);
        }

        /// <summary>
        /// The number of milliseconds to wait to 
        /// try and acquire a lock
        /// </summary>
        public int LockTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the acquire lock retry interval, the amount of time in milliseconds to sleep
        /// between attempts to acquire a lock.
        /// </summary>
        /// <value>
        /// The acquire lock retry interval.
        /// </value>
        public int AcquireLockRetryInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CLR type of the managed data object.
        /// </summary>
        public Type DataType
        {
            get;
            set;
        }

        string _rootDirectory;
        readonly object _rootDirectoryLock = new object();
        /// <summary>
        /// Gets or sets the root directory for this multi-process data instance, combining the process data folder with the data type name.
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return _rootDirectoryLock.DoubleCheckLock(ref _rootDirectory, () => Path.Combine(RuntimeSettings.ProcessDataFolder, DataType.Name));
            }
            set => _rootDirectory = Path.Combine(value, DataType.Name);
        }

        /// <summary>
        /// Occurs when an exception is thrown while attempting to acquire a lock.
        /// </summary>
        public event EventHandler AcquireLockException;
      
        protected void OnAcquireLockException(Exception ex)
        {
            if (AcquireLockException != null)
            {
                LastExceptionMessage = "PID={0}:{1}".Format(Process.GetCurrentProcess().Id, ex.Message);
                AcquireLockException(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when the instance is waiting for another process to release the lock.
        /// </summary>
        public event EventHandler WaitingForLock;

        protected void OnWaitingForLock()
        {
            WaitingForLock?.Invoke(this, new EventArgs());
        }
                
        /// <summary>
        /// Gets or sets the message from the last exception encountered during lock acquisition.
        /// </summary>
        public string LastExceptionMessage { get; set; }

        /// <summary>
        /// Gets the process id of the process who has 
        /// the lock
        /// </summary>
        public string CurrentLockerId { get; set; }

        /// <summary>
        /// Gets or sets the machine name of the process that currently holds the lock.
        /// </summary>
        public string CurrentLockerMachineName { get; set; }

        protected string LockFile => Path.Combine(RootDirectory, "{0}.lock".Format(HashHexString));

        protected string TempLockFile => $"{LockFile}.tmp";

        protected internal string WriteFile => Path.Combine(RootDirectory, "{0}.write".Format(HashHexString));

        protected internal string ReadFile => Path.Combine(RootDirectory, "{0}.read".Format(HashHexString));

        protected internal string DataFile => Path.Combine(RootDirectory, HashHexString);

        private void EnsureRoot()
        {
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }
        }

        static readonly object _lock = new object();
        private bool AcquireLock(int timeoutInMilliseconds)
        {
            try
            {
                lock (_lock)
                {
                    EnsureRoot();
                    JsonMultiProcessDataLockInfo lockInfo = new JsonMultiProcessDataLockInfo();
                    bool timeoutExpired = Exec.TakesTooLong(() =>
                    {
                        bool logged = false;
                        while (File.Exists(LockFile))
                        {
                            if (!logged)
                            {
                                logged = true;
                                JsonMultiProcessDataLockInfo currentLockInfo =
                                    ObjectDataEncoder.Decode<JsonMultiProcessDataLockInfo>(File.ReadAllBytes(LockFile));
                                CurrentLockerId = currentLockInfo?.ProcessId.ToString();
                                CurrentLockerMachineName = currentLockInfo?.MachineName;
                                OnWaitingForLock();
                            }

                            Thread.Sleep(AcquireLockRetryInterval);
                        }
                        return LockFile;
                    }, (lockFile) =>
                    {
                        IObjectEncoding encoding = ObjectDataEncoder.Encode(lockInfo);
                        
                        File.WriteAllBytes(lockFile, encoding.Value);
                        
                        return lockFile;
                    }, TimeSpan.FromMilliseconds(timeoutInMilliseconds));

                    return !timeoutExpired;
                }
            }
            catch (Exception ex)
            {
                OnAcquireLockException(ex);
                return false;
            }
        }

    }
}
