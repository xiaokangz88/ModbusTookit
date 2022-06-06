using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ModbusToolkit.Tests.Shared {
    [TestClass]
    public class ModbusTests {

#pragma warning disable CS8618
        static IModbus modbus;
#pragma warning restore CS8618
        const byte SLAVE_ID = 32;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _) {
            modbus = ModbusFactory.Create("localhost", 502);
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            modbus.Dispose();
        }

        [TestMethod]
        public void TestReadWriteHoldingRegisters() {

            // Arrange
            byte byte1 = 12;
            byte byte2 = 34;
            byte byte3 = 56;
            byte byte4 = 78;

            // Action
            modbus.WriteHoldingRegisters(SLAVE_ID, 0, new byte[] { byte1, byte2, byte3, byte4 });
            byte[] data = modbus.ReadHoldingRegisters(SLAVE_ID, 0, 2);

            // Assert
            Assert.AreEqual(data.Length, 4);
            Assert.AreEqual(data[0], byte1);
            Assert.AreEqual(data[1], byte2);
            Assert.AreEqual(data[2], byte3);
            Assert.AreEqual(data[3], byte4);
        }

        [TestMethod]
        public void TestReadWriteCoils() {

            // Arrange
            byte[] bits = new byte[2] { 0b_1001_1111, 0b_0000_0010 };

            // Action
            modbus.WriteCoils(SLAVE_ID, 0, 10, bits);
            byte[] data = modbus.ReadCoils(SLAVE_ID, 0, 10);

            // Assert
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(0, bits[0] ^ data[0]);
            Assert.AreEqual(0, bits[1] ^ data[1]);
        }

        [TestMethod]
        public void TestReadDiscreteInputs() {

            // Arrange
            byte[] expectedBits = new byte[2] { 0b_0101_1010, 0b_0000_0011 };

            // Action
            byte[] data = modbus.ReadDiscreteInputs(SLAVE_ID, 0, 10);

            // Assert
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(0, expectedBits[0] ^ data[0]);
            Assert.AreEqual(0, expectedBits[1] ^ data[1]);
        }

        [TestMethod]
        public void TestReadInputRegisters() {

            // Arrange
            byte[] expectedData = new byte[20] {
                0, 0,       // 0
                0xFF, 0xFF, // 65535
                0x04, 0x57, // 1111
                0x08, 0xAE, // 2222
                0x0D, 0x05, // 3333
                0x11, 0x5C, // 4444
                0x15, 0xB3, // 5555
                0x1A, 0x0A, // 6666
                0x1E, 0x61, // 7777
                0x22, 0xB8, // 8888
            };

            // Action
            byte[] data = modbus.ReadInputRegisters(SLAVE_ID, 0, 10);

            // Assert
            Assert.AreEqual(20, data.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedData, data));
        }
    }
}
