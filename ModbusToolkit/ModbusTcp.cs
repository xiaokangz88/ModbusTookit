using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ModbusToolkit {
    internal sealed class ModbusTcp : IModbus {

        private readonly ILoggerAdapter Logger;

        private readonly TcpClient tcpClient = new TcpClient();

        private ushort LastTxID = 0;

        public ModbusTcp(string host, int port, ILoggerAdapter logger = null) {
            Logger = logger ?? NullLoggerAdapter.Instance;

            try {
                tcpClient.Connect(host, port);

                Logger.Info($"Connect to {host} port {port}: Connection established");
                tcpClient.Client.SendTimeout = 3000;
                tcpClient.Client.ReceiveTimeout = 3000;
            }
            catch (SocketException ex) {
                Logger.Error($"Connect to {host} port {port}: Connection failed", ex);
                throw;
            }
        }

        public byte[] SendRequest(byte slaveId, byte[] requestPDU) {

            ushort TxID = ++LastTxID;

            try {

                byte[] requestADU = MakMBAP(TxID, slaveId, (ushort)requestPDU.Length).Concat(requestPDU).ToArray();
                if (Logger.IsInfoEnabled) Logger.Info($"Tx: {TxID:D5} - {ToHexString(requestADU)}");

                tcpClient.Client.Send(requestADU);

                byte[] MBAP = new byte[7];
                tcpClient.Client.Receive(MBAP, 0, 7, SocketFlags.None);
                ushort RxID = (ushort)((MBAP[0] << 8) + MBAP[1]);
                if (Logger.IsDebugEnabled) Logger.Debug($"Rx: {RxID:D5} - Response MBAP Header: {ToHexString(MBAP)}");

                ushort length = MBAP[4];
                length <<= 8;
                length += MBAP[5];

                byte[] responsePDU = new byte[length - 1];
                tcpClient.Client.Receive(responsePDU, 0, responsePDU.Length, SocketFlags.None);
                if (Logger.IsInfoEnabled) Logger.Info($"Rx: {RxID:D5} - {ToHexString(MBAP.Concat(responsePDU))}");
                if (responsePDU[0] > 0x80) {
                    Logger.Error($"Modbus Exception: {ModbusException.GetExceptionName(responsePDU[1])} ({responsePDU[1]})");
                    throw new ModbusException(responsePDU[1]);
                }

                return responsePDU;
            }
            catch (SocketException ex) {
                Logger.Error($"Tx: {TxID:D5} - {slaveId}, Communication failure", ex);
                throw;
            }
        }

        private static string ToHexString(IEnumerable<byte> requestPDU) {
            return string.Join(" ", requestPDU.Select(x => x.ToString("X2")));
        }

        private byte[] MakMBAP(ushort TxID, byte slaveId, ushort lenOfPDU) {
            ushort length = (byte)(lenOfPDU + 1);
            return new byte[7] {
                (byte)(TxID >> 8),
                (byte)TxID,
                0,
                0,
                (byte)(length >> 8),
                (byte)length,
                slaveId
            };
        }

        public void Dispose() {
            if (tcpClient.Connected) {
                tcpClient.Close();
                Logger.Info("Connection closed");
            }
        }
    }
}
