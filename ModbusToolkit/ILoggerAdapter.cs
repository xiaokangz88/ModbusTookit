using System;

namespace ModbusToolkit {
    public interface ILoggerAdapter {
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsErrorEnabled { get; }

        void Debug(string message, Exception exception = null);
        void Info(string message, Exception exception = null);
        void Error(string message, Exception exception = null);
    }
}
