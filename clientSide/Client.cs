using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;


public class SocketClient {
    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }
    private static void StartClient()
    {
        byte[] bytes = new byte[2048];

        try
        {
            IPHostEntry host = Dns.GetHostEntry("192.168.1.101");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11001);

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                
                int adsasd = sender.Receive(bytes);
                Console.WriteLine( Encoding.ASCII.GetString(bytes, 0, adsasd));

                Console.WriteLine("Type admin for admin privilages or type guest to continue as guest") ;
                
                Console.Write("Your role is : ");
                String role = Console.ReadLine();
                byte[] msg = Encoding.ASCII.GetBytes(role);
                sender.Send(msg);
                
                
                if (role == "admin")
                {
                    int bytes_role = sender.Receive(bytes);
                    Console.Write( Encoding.ASCII.GetString(bytes, 0, bytes_role)); // write your pw
                    
                    string pw = Console.ReadLine();
                    
                    byte[] msg2 = Encoding.ASCII.GetBytes(pw);
                    sender.Send(msg2);
                    
                    int bytes_respnose = sender.Receive(bytes);
                    Console.WriteLine( Encoding.ASCII.GetString(bytes, 0, bytes_respnose));
                    
                    

                }
                else
                {
                    int bytes_response = sender.Receive(bytes);
                    Console.WriteLine( Encoding.ASCII.GetString(bytes, 0, bytes_response));
                    
                }
                
                int bytes_options = sender.Receive(bytes);
                Console.WriteLine( Encoding.ASCII.GetString(bytes, 0, bytes_options));

                
                Console.Write("Choose option : ");
                string option = Console.ReadLine();
                
                switch (option)
                {
                    case "1":option_send_message(sender);
                        break;
                    case "2": option_read_file(sender);
                        break;
                    case "3": option_write_file(sender);
                        break;
                    case "4": option_exceute_del(sender);
                        break;
                    case "quit":request_type("quit",sender); 
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
                
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}