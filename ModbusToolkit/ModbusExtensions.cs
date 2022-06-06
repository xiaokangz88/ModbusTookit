using System.Linq;

namespace ModbusToolkit {
    public static class ModbusExtensions {

        #region Core Methods
        public static byte[] ReadCoils(this IModbus modbus, byte slaveId, ushort address, ushort quantity) {
            var requestPDU = MakePDU(0x01, address, quantity);
            var responsePDU = modbus.SendRequest(slaveId, requestPDU);
            return responsePDU.Skip(2).ToArray();
        }

        public static void WriteCoils(this IModbus modbus, byte slaveId, ushort address, ushort quantity, byte[] data) {
            modbus.SendRequest(slaveId, MakePDU(0x0F, address, quantity, data));
        }

        public static byte[] ReadDiscreteInputs(this IModbus modbus, byte slaveId, ushort address, ushort quantity) {
            byte[] requestPDU = MakePDU(0x02, address, quantity);
            byte[] responsePDU = modbus.SendRequest(slaveId, requestPDU);
            return responsePDU.Skip(2).ToArray();
        }

        public static byte[] ReadHoldingRegisters(this IModbus modbus, byte slaveId, ushort address, ushort quantity) {
            byte[] requestPDU = MakePDU(0x03, address, quantity);
            byte[] responsePDU = modbus.SendRequest(slaveId, requestPDU);
            return responsePDU.Skip(2).ToArray();
        }

        public static void WriteHoldingRegisters(this IModbus modbus, byte slaveId, ushort address, params byte[] data) {
            modbus.SendRequest(slaveId, MakePDU(0x10, address, (ushort)(data.Length / 2), data));
        }

        public static byte[] ReadInputRegisters(this IModbus modbus, byte slaveId, ushort address, ushort quantity) {
            byte[] requestPDU = MakePDU(0x04, address, quantity);
            byte[] responsePDU = modbus.SendRequest(slaveId, requestPDU);
            return responsePDU.Skip(2).ToArray();
        }
        #endregion

        private static byte[] MakePDU(byte function, ushort address, ushort quantity) {
            return new byte[] {
                function,
                (byte)(address >> 8),
                (byte)address,
                (byte)(quantity >> 8),
                (byte)quantity
            };
        }

        private static byte[] MakePDU(byte function, ushort address, ushort quantity, byte[] data) {
            return new byte[] {
                function,
                (byte)(address >> 8),
                (byte)address,
                (byte)(quantity >> 8),
                (byte)quantity,
                (byte)data.Length
            }.Concat(data).ToArray();
        }
    }
}
