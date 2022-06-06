namespace ModbusToolkit {
    public sealed class ModbusFactory {
        public static IModbus Create(string host, int port) {
            return new ModbusTcp(host, port);
        }
    }
}
