namespace ModbusToolkit {
    public sealed class ModbusFactory {
        public static IModbus Create(string host, int port, ILoggerAdapter logger = null) {
            return new ModbusTcp(host, port, logger);
        }
    }
}
