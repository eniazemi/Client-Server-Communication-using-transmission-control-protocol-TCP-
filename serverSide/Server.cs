using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;


namespace server_client_TCP
{
    internal class Program
    {
        public static int Main(String[] args)
            {
                StartServer();
                return 0;
            }
        
        public static void revice_message(int x, Socket handler, byte[] bytes) 
        {
        string message = null;
        
        int bytes_for_message = handler.Receive(bytes);
        message += Encoding.ASCII.GetString(bytes, 0, bytes_for_message);
        Console.WriteLine("USER {0} send text : {1}",x, message);
        
        byte[] bytes_for_second_message_ack = Encoding.ASCII.GetBytes("ACK . Just a reminder your text was : "+ message);
        handler.Send(bytes_for_second_message_ack);
    }
    
        private static void show_files(Socket handler)
        {
            string[] files = Directory.GetFiles(@"C:\Users\User\Desktop\testC#\server\server", "*.txt");
            string avaliable_files = "";
            foreach (string line in files)
            {
                avaliable_files += line + "\n";

            }
            byte[] random = Encoding.ASCII.GetBytes(avaliable_files);
            handler.Send(random);
            Console.WriteLine("Showing avalible files to user...");
        }
         private static void let_read_file(Socket handler, byte[] bytes)
    {
        show_files(handler);
        
        int bytes_for_file = handler.Receive(bytes);
        string file = Encoding.ASCII.GetString(bytes, 0, bytes_for_file);
        string full_path = "C:\\Users\\User\\Desktop\\testC#\\server\\server\\" + file;
        if (File.Exists(full_path))
        {
            Console.WriteLine("User has chosen to write file ---> " + full_path);
            string[] lines = System.IO.File.ReadAllLines(full_path);
            string text = "";
            foreach (string line in lines)
            {
                text += line + "\n";
            }

            byte[] bytes_for_second_message_ack = Encoding.ASCII.GetBytes(text);
            handler.Send(bytes_for_second_message_ack);
        }
        else
        {
            Console.WriteLine("Trying to read a file that do now exist");
            Console.WriteLine("File "+full_path+" do not exist");
            byte[] bytes_for_second_message_ack = Encoding.ASCII.GetBytes("File do not exist!");
            handler.Send(bytes_for_second_message_ack);
        }
    }
    
    private static void let_write_file(Socket handler, byte[] bytes)
    {
        show_files(handler);
        byte[] msg = Encoding.ASCII.GetBytes("Name of file: C:\\Users\\User\\Desktop\\testC#\\server\\server\\");
        handler.Send(msg);
        
        int bytes_for_file = handler.Receive(bytes);
        string file_name = Encoding.ASCII.GetString(bytes, 0, bytes_for_file);
        string full_path = "C:\\Users\\User\\Desktop\\testC#\\server\\server\\"+file_name;
        Console.WriteLine("User has chosen to write file ---> "+full_path);

        byte[] msg2 = Encoding.ASCII.GetBytes("What do you want to write: ");
        handler.Send(msg2);
        
        int bytes_for_text = handler.Receive(bytes);
        string text = Encoding.ASCII.GetString(bytes, 0, bytes_for_text);
        Console.WriteLine("Adding text |:" + text + "| to file with path :" + full_path);
        
        using (StreamWriter sw = File.AppendText(full_path))
        {
            sw.WriteLine("\n");
            sw.WriteLine(text);
        }	

        Console.WriteLine("Text:| "+ text+" | was written on : "+full_path);
        byte[] confirmation = Encoding.ASCII.GetBytes("File was written successfully");
        handler.Send(confirmation);    }
    
    public static void let_delete_file(Socket handler, byte[] bytes)
    {
        show_files(handler);
        int bytes_for_file = handler.Receive(bytes);
        string file = Encoding.ASCII.GetString(bytes, 0, bytes_for_file);
        
        string full_path = "C:\\Users\\User\\Desktop\\testC#\\server\\server\\" + file;
        
        Console.WriteLine("User has chosen to DELETE file ---> "+full_path);

        string ALERT = "--ALERT-- This opreation is not reversible --ALERT--\n" +
                       "Type YES to continue / Type any other key to cancel";
        byte[] msg = Encoding.ASCII.GetBytes(ALERT);
        handler.Send(msg);
        
        int alert_response_byte = handler.Receive(bytes);
        string response = Encoding.ASCII.GetString(bytes, 0, alert_response_byte);
        
        if (response=="YES")
        {
            File.Delete(full_path);

            byte[] confirmation = Encoding.ASCII.GetBytes("File was succesfully deleted");
            handler.Send(confirmation);
            Console.WriteLine("File "+full_path+" was deleted");
        }
        else
        {
            byte[] confirmation = Encoding.ASCII.GetBytes("Operation was CANCEL. File was not DELETED! ");
            handler.Send(confirmation);
            Console.WriteLine("Operation was CANCEL. File was not DELETED!" );
        }
    }
    
    private static bool is_admin(Socket handler)
    {
        byte[] bytes = new byte[2048];
            
        int bytes_for_role = handler.Receive(bytes);
        string role = Encoding.ASCII.GetString(bytes, 0, bytes_for_role);

        if (role == "admin")
        {
            byte[] msg2 = Encoding.ASCII.GetBytes("Write your password: ");
            handler.Send(msg2);

            int bytes_for_pw = handler.Receive(bytes);
            string pw_recived = Encoding.ASCII.GetString(bytes, 0, bytes_for_pw); // pw from user
            
            if (pw_recived == "admin")
            {
                byte[] msg3 = Encoding.ASCII.GetBytes("AUTH COMPLETED SUCCESSFULLY");
                handler.Send(msg3);
                return true;
            }
            else
            {
                byte[] msg3 = Encoding.ASCII.GetBytes("WRONG PASSWORD. YOU DONT HAVE ACCESS AS ADMIN");
                handler.Send(msg3);
                return false;
            }
        }
        
        else
        {
            byte[] msg3 = Encoding.ASCII.GetBytes("CONTINUING AS GUEST");
            handler.Send(msg3);
            return false;

        }
    }
    
        
        private static void Secure_canal_of_communication(Socket listener)
    {
        int x = 1;
        
        while (x < 20)
        {
            byte[] bytes = new byte[2048];
            Socket handler = listener.Accept();
            Console.WriteLine("Listening user: " + x);

            byte[] msg = Encoding.ASCII.GetBytes("SERVER IS SAYING :\nSYN/ACK. Welcome user. Your id is: " + x);
            handler.Send(msg);
            bool admin = is_admin(handler);
            
            if (admin)
            {
                byte[] msg2 =
                    Encoding.ASCII.GetBytes(
                        "\n---------You have administrator privileges----------" +
                        "\nPress 1 to send message " +
                        "\nPress 2 to read file" +
                        "\nPress 3 to write file" +
                        "\nPress 4 to delete file" +
                        "\nType 'quit' to disconnect immediately\n ");
                handler.Send(msg2);
            }
            else
            {
                byte[] msg2 = Encoding.ASCII.GetBytes(
                    "\nYou are a guest user you only have premission to read and send a message " +
                    "\nPress 1 for sending messages " +
                    "\nPress 2 for reading files" +
                    "\nType quit to disconnect immediately");
                handler.Send(msg2);
            }

            // request type
            int bytes_for_option = handler.Receive(bytes);
            string chosen_option = Encoding.ASCII.GetString(bytes, 0, bytes_for_option);

            Console.WriteLine("USER {0} chose operation : {1}", x, chosen_option);

            if (admin)
            {
                switch (chosen_option)
                {
                    case "SEND MESSAGE":
                        byte[] msg3 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg3);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);

                        revice_message(x, handler, bytes);
                        break;

                    case "READ FILE":
                        byte[] msg4 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg4);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);
                        let_read_file(handler, bytes);
                        break;
                    
                    case "WRITE FILE":
                        byte[] msg5 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg5);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);
                        let_write_file(handler, bytes);
                        break;
                    
                    case "DELETE FILE":
                        byte[] msg6 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg6);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);
                        let_delete_file(handler, bytes);
                        break;
                    
                    default:
                        byte[] msg7 = Encoding.ASCII.GetBytes("BAD INPUT");
                        handler.Send(msg7);
                        Console.WriteLine("UNKNOWN COMMAND. SERVER IS BLOCKING THAT REQUEST");
                        Console.WriteLine("USER {0} AUTH DENIED", x);
                        break;
                }
            }
            else
            {
                switch (chosen_option)
                {
                    case "SEND MESSAGE":
                        byte[] msg3 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg3);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);

                        revice_message(x, handler, bytes);
                        break;

                    case "READ FILE":
                        byte[] msg4 = Encoding.ASCII.GetBytes("AUTH GRANTED");
                        handler.Send(msg4);
                        Console.WriteLine("USER {0} AUTH GRANTED", x);
                        let_read_file(handler, bytes);
                        break;
                    
                    default:
                        byte[] msg7 = Encoding.ASCII.GetBytes("AUTH DENIED ---- NO ACCESS OR BAD INPUT");
                        handler.Send(msg7);
                        Console.WriteLine("USER {0} DO NOT HAVE PRIVILEGES -- AUTH DENIED", x);
                        break;
                }
            }
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            Console.WriteLine("User {0} disconnected succesfully", x);
            x += 1;
        }

        Console.WriteLine("MAXIMUM ");
    }


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

