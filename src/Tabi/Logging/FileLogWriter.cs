﻿using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tabi.Helpers;

namespace Tabi.Logging
{
    public class FileLogWriter : LogWriter
    {
        public static EventHandler<EventArgs> LogWriteEvent;
        LogFileAccess logAccess;

        public FileLogWriter() : base()
        {
            logAccess = new LogFileAccess();
        }

        private async Task WriteToFile(string text)
        {
            await logAccess.AppendAsync(text);
            LogWriteEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override async Task ErrorConsumerAsync(ISourceBlock<Exception> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                await WriteToFile($"{DateTime.Now} {Source.Receive()}\n");
                LogWriteEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override async Task LogConsumerAsync(ISourceBlock<string> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                await WriteToFile($"{DateTime.Now} {Source.Receive()}\n");
                LogWriteEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
