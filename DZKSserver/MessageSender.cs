using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DZKSserver
{
    public class MessageSender
    {
        static private TcpListener server;
        static public void Open()
        {
            IPAddress serverAddress = IPAddress.Parse(StaticParametrs.serverAddress);
            server = new TcpListener(serverAddress, StaticParametrs.serverPort);
            server.Start();
            Console.WriteLine("Запуск TCP сервера.");
            StartListening();
        }
        static private async void StartListening()
        {
            if(server != null)
            {
                while (true)
                {
                    StartClientServing(await server.AcceptTcpClientAsync());
                }
            }
        }
        static private async void StartClientServing(TcpClient? client)
        {
            string messageServ = "";
            if (client != null)
            {
                Console.WriteLine($"Client {client.Client.AddressFamily.ToString()} connected");
                NetworkStream clientStream = client.GetStream();
                while (true)
                {
                    try { await clientStream.WriteAsync(new byte[0]); }
                    catch { break; }

                    if (messageServ != StaticParametrs.messageForApp)
                    { 
                        messageServ = StaticParametrs.messageForApp;

                        byte[] data = Encoding.UTF8.GetBytes(messageServ);
                        try
                        {
                            await clientStream.WriteAsync(data, 0, data.Length);
                        }
                        catch(Exception ex) { Console.WriteLine(ex.Message); break; }

                        Console.WriteLine($"Sending message to client {client.Client.AddressFamily.ToString()}.");
                    }
                    
                }
                GC.CollectionCount(2);
                clientStream.Close();
                Console.WriteLine($"Client {client.Client.AddressFamily.ToString()} disconected.");
                client.Close();
                
            }
        }
        static public void StopServer()
        {
            if (server != null)
                try
                {
                    server.Stop();
                }
                catch(Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
