using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// ������������ ������ ��� Socks4a ������-�������.
    /// </summary>
    public class Socks4aProxyClient : Socks4ProxyClient 
    {
        #region ������������ (��������)

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4aProxyClient"/>.
        /// </summary>
        public Socks4aProxyClient()
            : this(null) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4aProxyClient"/> �������� ������ ������-�������, � ������������� ���� ������ - 1080.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        public Socks4aProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4aProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        public Socks4aProxyClient(string host, int port)
            : this(host, port, string.Empty) { }

        /// <summary>
        /// �������������� ����� ��������� ������ <see cref="Socks4aProxyClient"/> ��������� ������� � ������-�������.
        /// </summary>
        /// <param name="host">���� ������-�������.</param>
        /// <param name="port">���� ������-�������.</param>
        /// <param name="username">��� ������������ ��� ����������� �� ������-�������.</param>
        public Socks4aProxyClient(string host, int port, string username)
            : base(host, port, username)
        {
            _type = ProxyType.Socks4a;
        }

        #endregion


        #region ������ (��������)

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4aProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <returns>��������� ������ <see cref="Socks4aProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">�������� ��������� <paramref name="proxyAddress"/> ����� <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">�������� ��������� <paramref name="proxyAddress"/> �������� ������ �������.</exception>
        /// <exception cref="System.FormatException">������ ����� �������� ������������.</exception>
        public static Socks4aProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.Socks4a, proxyAddress) as Socks4aProxyClient;
        }

        /// <summary>
        /// ����������� ������ � ��������� ������ <see cref="Socks4aProxyClient"/>. ���������� ��������, �����������, ������� �� ��������� ��������������.
        /// </summary>
        /// <param name="proxyAddress">������ ���� - ����:����:���_������������:������. ��� ��������� ��������� �������� ���������������.</param>
        /// <param name="result">���� �������������� ��������� �������, �� �������� ��������� ������ <see cref="Socks4aProxyClient"/>, ����� <see langword="null"/>.</param>
        /// <returns>�������� <see langword="true"/>, ���� �������� <paramref name="proxyAddress"/> ������������ �������, ����� <see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks4aProxyClient result)
        {
            ProxyClient proxy;

            if (TryParse(ProxyType.Socks4a, proxyAddress, out proxy))
            {
                result = proxy as Socks4aProxyClient;
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        protected override void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte[] dstPort = GetPortBytes(destinationPort);
            byte[] dstIp = { 0, 0, 0, 1 };

            byte[] userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            byte[] dstAddr = ASCIIEncoding.ASCII.GetBytes(destinationHost);

            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL| DSTADDR      |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            //    1    1      2              4           variable       1    variable        1 
            byte[] request = new byte[10 + userId.Length + dstAddr.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstPort.CopyTo(request, 2);
            dstIp.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;
            dstAddr.CopyTo(request, 9 + userId.Length);
            request[9 + userId.Length + dstAddr.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //    1    1      2              4
            byte[] response = new byte[8];

            nStream.Read(response, 0, 8);

            byte reply = response[1];

            // ���� ������ �� ��������.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }
    }
}