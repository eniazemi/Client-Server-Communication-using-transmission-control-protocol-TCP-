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
    private static void request_type(string x, Socket sender)
    {
        byte[] choice_of_user = Encoding.ASCII.GetBytes(x);
        sender.Send(choice_of_user);
    }

    private static string get_authorization(Socket sender)
    {
        byte[] bytes = new byte[2048];
        int auth = sender.Receive(bytes);
        string asd = Encoding.ASCII.GetString(bytes, 0, auth);
        return asd;
    }

    private static void option_send_message(Socket sender)
    {
        request_type("SEND MESSAGE", sender);
        string asd = get_authorization(sender);
        Console.WriteLine(asd);
        if (asd != "AUTH GRANTED")
        {
            Console.WriteLine("You dont have access to do this operation");
        }
        else
        {
            Console.Write("Write your message: ");
            string getMessageConsole = Console.ReadLine();
        
            byte[] bytes = new byte[2048];
            byte[] msg = Encoding.ASCII.GetBytes(getMessageConsole);
            sender.Send(msg);

            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("Server is saying : " +
                              Encoding.ASCII.GetString(bytes, 0, bytesRec));
            
        }
        
    }

    private static void available_files(Socket sender)
    {
        byte[] bytes = new byte[2048];
        
        int bytesFile = sender.Receive(bytes);
        String files = Encoding.ASCII.GetString(bytes, 0, bytesFile);
        
        Console.WriteLine(files);
    }

    private static void option_read_file(Socket sender)
    {
        request_type("READ FILE",sender);
        
        Console.WriteLine(get_authorization(sender));
        
        Console.WriteLine("----Files that you can read are:---- ");
        available_files(sender);
        byte[] bytes = new byte[2048];
       
        Console.Write("Choose the name of file that you want to read: ");
        string requiredFile = Console.ReadLine();
        
        byte[] msg = Encoding.ASCII.GetBytes(requiredFile);
        sender.Send(msg);
        
        int bytesRec = sender.Receive(bytes); 
        Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRec));
        
    }

    private static void option_write_file(Socket sender)
    {
        byte[] bytes = new byte[2048];
        
        request_type("WRITE FILE",sender);
        
        string auth = get_authorization(sender);
        Console.WriteLine(auth);
        
        if (auth == "AUTH GRANTED")
        {
            Console.WriteLine("----Choose in which file do you want to write:----");
            
            available_files(sender);
            
            int bytesRec = sender.Receive(bytes);
            Console.Write(Encoding.ASCII.GetString(bytes, 0, bytesRec));
            
            string name_of_file = Console.ReadLine();
            byte[] byte_name_of_file = Encoding.ASCII.GetBytes(name_of_file);
            sender.Send(byte_name_of_file);
            
            int bytesRecWhat = sender.Receive(bytes);
            Console.Write(Encoding.ASCII.GetString(bytes, 0, bytesRecWhat));
            
            string write_text = Console.ReadLine();
            byte[] bytes_write_text = Encoding.ASCII.GetBytes(write_text);
            sender.Send(bytes_write_text);
            
            int bytes_confiramtion = sender.Receive(bytes);
            Console.Write(Encoding.ASCII.GetString(bytes, 0, bytes_confiramtion));
        }
    }

    private static void option_exceute_del(Socket sender)
    {
        byte[] bytes = new byte[2048];
        
        request_type("DELETE FILE",sender);
        
        string auth = get_authorization(sender);
        Console.WriteLine(auth);
        
        if (auth =="AUTH GRANTED")
        {
            Console.WriteLine("Files that you can delete are: ");
            available_files(sender);
        
            Console.Write("Choose the name of file that you want to DELETE: ");
        
            string requiredFile = Console.ReadLine();
            byte[] msg = Encoding.ASCII.GetBytes(requiredFile);
            sender.Send(msg);
        
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRec));
            
            Console.Write("DO YOU WANT TO CONTINUE: ");
            String alert_response = Console.ReadLine();
            byte[] msg2 = Encoding.ASCII.GetBytes(alert_response);
            sender.Send(msg2);
            
            int confirmation = sender.Receive(bytes);
            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, confirmation));
        }
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