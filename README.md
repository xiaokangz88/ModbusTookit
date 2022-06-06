# ModbusTookit - a simple modbus library

## Example Usage

```csharp
class Program {
    public static void Main() {
        const byte SlaveID = 32;
        using (IModbus modbus = ModbusFactory.Create("localhost", 502)) {
            // read coil status
            byte[] coils = modbus.ReadCoils(SlaveID, address: 0, quantity: 10);
            modbus.WriteCoils(SlaveID, address: 0, quantity: 10, new byte[] { 0x02, 0x77 });

            // read/write holding registers
            byte[] holdingValues = modbus.ReadHoldingRegisters(SlaveID, address: 0, quantity: 10);
            modbus.WriteHoldingRegisters(SlaveID, address: 0, new byte[] { 0x08, 0x09, 0x0A, 0x0B });

            // read discrete inputs
            byte[] data = modbus.ReadDiscreteInputs(SlaveID, address: 0, quantity: 10);

            // read input registers
            byte[] inputValues = modbus.ReadInputRegisters(SlaveID, address: 0, quantity: 10);
        }
    }
}

```
