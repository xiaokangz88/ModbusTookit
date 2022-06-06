using System;

namespace ModbusToolkit {

    public interface IModbus : IDisposable {

        byte[] SendRequest(byte slaveId, byte[] requestPDU);
    }
}
