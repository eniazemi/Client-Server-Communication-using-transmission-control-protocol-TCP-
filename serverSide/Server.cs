namespace server_client_TCP
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            public static void StartServer()
            {

                IPHostEntry host = Dns.GetHostEntry("192.168.1.101");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11001);

                try
                {
                    Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    Console.WriteLine("SERVER IS READY\nWAITING FOR CONNECTION...");
                    Secure_canal_of_communication(listener);
                }
        
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
    }
}