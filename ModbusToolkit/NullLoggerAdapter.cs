using System;

namespace ModbusToolkit {
    internal class NullLoggerAdapter : ILoggerAdapter {

        public static NullLoggerAdapter Instance = new NullLoggerAdapter();

        public bool IsDebugEnabled => false;

        public bool IsInfoEnabled => false;

        public bool IsErrorEnabled => false;

        public void Debug(string message, Exception exception = null) {
        }

        public void Error(string message, Exception exception = null) {
        }

        public void Info(string message, Exception exception = null) {
        }
    }
}
