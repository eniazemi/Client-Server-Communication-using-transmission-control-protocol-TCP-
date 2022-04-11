namespace clientSide
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            private static void StartClient()
            {
                byte[] bytes = new byte[2048];

                try
                {
                    IPHostEntry host = Dns.GetHostEntry("192.168.1.101");
                    IPAddress ipAddress = host.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11001);

                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                

            }
    }
}