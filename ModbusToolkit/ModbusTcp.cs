using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ModbusToolkit {
    internal sealed class ModbusTcp : IModbus {

        static readonly ILog Log = LogManager.GetLogger(typeof(ModbusTcp));

        private ushort LastTxID = 0;

        private readonly TcpClient tcpClient = new TcpClient();

        public ModbusTcp(string host, int port, int sendTimeout = 3000, int receiveTimeout = 3000) {
            try {
                tcpClient.Connect(host, port);

                Log.Info($"Connect to {host} port {port}: Connection established");
                tcpClient.Client.SendTimeout = sendTimeout;
                tcpClient.Client.ReceiveTimeout = receiveTimeout;
            }
            catch (SocketException ex) {
                Log.Error($"Connect to {host} port {port}: Connection failed", ex);
                throw;
            }
        }

        public byte[] SendRequest(byte slaveId, byte[] requestPDU) {

            ushort TxID = ++LastTxID;

            try {

                byte[] requestADU = MakMBAP(TxID, slaveId, (ushort)requestPDU.Length).Concat(requestPDU).ToArray();
                if (Log.IsInfoEnabled) Log.Info($"Tx: {TxID:D5} - {ToHexString(requestADU)}");

                tcpClient.Client.Send(requestADU);

                byte[] MBAP = new byte[7];
                tcpClient.Client.Receive(MBAP, 0, 7, SocketFlags.None);
                ushort RxID = (ushort)((MBAP[0] << 8) + MBAP[1]);
                if (Log.IsDebugEnabled) Log.Debug($"Rx: {RxID:D5} - Response MBAP Header: {ToHexString(MBAP)}");

                ushort length = MBAP[4];
                length <<= 8;
                length += MBAP[5];

                byte[] responsePDU = new byte[length - 1];
                tcpClient.Client.Receive(responsePDU, 0, responsePDU.Length, SocketFlags.None);
                if (Log.IsInfoEnabled) Log.Info($"Rx: {RxID:D5} - {ToHexString(MBAP.Concat(responsePDU))}");
                if (responsePDU[0] > 0x80) {
                    Log.Error($"Modbus Exception: {ModbusException.GetExceptionName(responsePDU[1])} ({responsePDU[1]})");
                    throw new ModbusException(responsePDU[1]);
                }

                return responsePDU;
            }
            catch (SocketException ex) {
                Log.Error($"Tx: {TxID:D5} - {slaveId}, Communication failure", ex);
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
                Log.Info("Connection closed");
            }
        }
    }
}
