/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Diagnostics;

namespace Bam
{
	/// <summary>
	/// Contains identifying information about the process that holds a <see cref="Bam.Data.Dynamic.Objects.JsonMultiProcessData"/> lock.
	/// </summary>
	[Serializable]
    public class JsonMultiProcessDataLockInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMultiProcessDataLockInfo"/> class, capturing the current process ID and machine name.
        /// </summary>
        public JsonMultiProcessDataLockInfo()
        {
            this.ProcessId = Process.GetCurrentProcess().Id;
            this.MachineName = Environment.MachineName;
        }

        /// <summary>
        /// Gets or sets the process ID of the lock holder.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the machine name of the lock holder.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Determines whether the specified object represents the same process on the same machine.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the object is a <see cref="JsonMultiProcessDataLockInfo"/> with the same ProcessId and MachineName.</returns>
        public override bool Equals(object obj)
        {
            if (obj is JsonMultiProcessDataLockInfo lockInfo)
            {
                return lockInfo.ProcessId == ProcessId && MachineName.Equals(MachineName);
            }
            return false;
        }
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return $"{MachineName}:{ProcessId}".GetHashCode();
        }
    }
}
