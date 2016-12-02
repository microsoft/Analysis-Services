using System;

namespace AsPartitionProcessing
{
    public class LogMessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public LogMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
