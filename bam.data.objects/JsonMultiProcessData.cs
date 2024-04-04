/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Bam.Net.Logging;
using Bam.Net.Configuration;
using System.Configuration;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects
{
    public class JsonMultiProcessData : RawData
    {
        public JsonMultiProcessData(object data, Encoding encoding = null) : base(JsonObjectEncoder.Default.Encode(data).Value, encoding)
        {         
            Args.ThrowIfNull(data, nameof(data));
            LockTimeout = 150;
            AcquireLockRetryInterval = 50;
            ObjectEncoder = JsonObjectEncoder.Default;
            DataType = data.GetType();
        }

        private JsonObjectEncoder ObjectEncoder
        {
            get;
            set;
        }
        
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

                IObjectEncoding encoding = ObjectEncoder.Encode(data);
                
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

        public Type DataType
        {
            get;
            set;
        }

        string _rootDirectory;
        readonly object _rootDirectoryLock = new object();
        public string RootDirectory
        {
            get
            {
                return _rootDirectoryLock.DoubleCheckLock(ref _rootDirectory, () => Path.Combine(RuntimeSettings.ProcessDataFolder, DataType.Name));
            }
            set
            {
                _rootDirectory = Path.Combine(value, DataType.Name);
            } 
        }

        public event EventHandler AcquireLockException;
      
        protected void OnAcquireLockException(Exception ex)
        {
            if (AcquireLockException != null)
            {
                LastExceptionMessage = "PID={0}:{1}".Format(Process.GetCurrentProcess().Id, ex.Message);
                AcquireLockException(this, new EventArgs());
            }
        }

        public event EventHandler WaitingForLock;

        protected void OnWaitingForLock()
        {
            WaitingForLock?.Invoke(this, new EventArgs());
        }
                
        public string LastExceptionMessage { get; set; }

        /// <summary>
        /// Gets the process id of the process who has 
        /// the lock
        /// </summary>
        public string CurrentLockerId { get; set; }

        public string CurrentLockerMachineName { get; set; }

        protected string LockFile => Path.Combine(RootDirectory, "{0}.lock".Format(HashString));

        protected string TempLockFile => $"{LockFile}.tmp";

        protected internal string WriteFile => Path.Combine(RootDirectory, "{0}.write".Format(HashString));

        protected internal string ReadFile => Path.Combine(RootDirectory, "{0}.read".Format(HashString));

        protected internal string DataFile => Path.Combine(RootDirectory, HashString);

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
                                    ObjectEncoder.Decode<JsonMultiProcessDataLockInfo>(File.ReadAllBytes(LockFile));
                                CurrentLockerId = currentLockInfo?.ProcessId.ToString();
                                CurrentLockerMachineName = currentLockInfo?.MachineName;
                                OnWaitingForLock();
                            }

                            Thread.Sleep(AcquireLockRetryInterval);
                        }
                        return LockFile;
                    }, (lockFile) =>
                    {
                        IObjectEncoding encoding = ObjectEncoder.Encode(lockInfo);
                        
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
