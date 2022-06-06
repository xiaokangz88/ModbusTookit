using System;
using System.Collections.Generic;

namespace ModbusToolkit {
    public class ModbusException : Exception {

        static readonly Dictionary<int, string> ModbusExceptions = new Dictionary<int, string> {
            { 1,    "Illegal Function" },
            { 2,    "Illegal Data Address" },
            { 3,    "Illegal Data Value" },
            { 4,    "Slave Device Failure" },
            { 5,    "Acknowledge" },
            { 6,    "Slave Device Busy" },
            { 7,    "Negative Acknowledge" },
            { 8,    "Memory Parity Error" },
            { 10,   "Gateway Path Unavailable" },
            { 11,   "Gateway Target Device Failed to Respond" },
        };

        public byte ExceptionCode { get; set; }

        public ModbusException(byte exceptionCode)
            : base(GetExceptionName(exceptionCode)) {
            ExceptionCode = exceptionCode;
        }

        public ModbusException(byte exceptionCode, string message)
            : base(message) {
            ExceptionCode = exceptionCode;
        }

        /// <summary>
        /// Get the exception name of the specified modbus exception code.
        /// </summary>
        /// <param name="exceptionCode">The modbus exception code.</param>
        /// <returns>Returns the modbus exception name.</returns>
        public static string GetExceptionName(byte exceptionCode) {
            return ModbusExceptions.TryGetValue(exceptionCode, out string exceptionName) ? exceptionName : "Unknown Exception";
        }
    }
}