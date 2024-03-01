using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomServer
{
    internal struct Client(TcpClient tcpClient, NetworkStream stream)
    {

        public TcpClient tcpClient = tcpClient;
        public NetworkStream stream = stream;

        //Identifiers
        //public string username;

    }

    internal static class Server
    {
        private readonly static int listening_port = 50000;
        private static bool serverFull = false;

        private readonly static List<Client> connected_clients = new(10);

        static async Task Main()
        {
            TcpListener server = new(IPAddress.Any, listening_port);

            try
            {
                server.Start();

                Console.WriteLine($"Server listening on port {listening_port}...");

                while (true)
                {
                    TcpClient tcpClient = await server.AcceptTcpClientAsync();
                    NetworkStream stream = tcpClient.GetStream();

                    Client client = new(tcpClient, stream);

                    if (!serverFull && connected_clients.Count != connected_clients.Capacity)
                    {
                        connected_clients.Add(client);

                        // Handle each client asynchronously
                        _ = HandleClientAsync(client);
                    }
                    else
                    {
                        serverFull = true;

                        // Reject client or handle accordingly
                        Console.WriteLine("Server is full. Rejecting client...");

                        // Send rejection message
                        NetworkStream rejectedStream = tcpClient.GetStream();
                        byte[] rejectionMessage = Encoding.UTF8.GetBytes("Server is full. Rejected.");
                        await rejectedStream.WriteAsync(rejectionMessage);

                        tcpClient.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
            }
            finally
            {
                // Perform cleanup or wait for user input before closing
                Console.WriteLine("Press Enter to close the server...");
                Console.ReadLine();
                server.Stop();
            }
        }

        static async Task HandleClientAsync(Client client)
        {

            // Handle communication with the client using the stream
            while (true)
            {

                byte[] data = new byte[1024];
                int bytesRead = await client.stream.ReadAsync(data);

                if (bytesRead == 0)
                {
                    // No more data, the client has disconnected
                    Console.WriteLine($"Client{connected_clients.IndexOf(client)} disconnected.");
                    break;
                }

                string message = Encoding.ASCII.GetString(data);
                
                if(message.StartsWith("/youtube "))
                {
                    string[] args = message.Split(' ');
                    if(args.Length == 2)
                    {
                        string[] link = args[1].Split("?");
                        string command = $"start https://www.youtube.com/watch_popup?{link[1]}&vq=hd1080";

                        Process.Start("cmd.exe", $"/c {command}");
                        Console.WriteLine($"Opened youtube video {link[1]}");

                        SendMessageToAll($"Opened youtube video {link[1]}");
                    }
                }
                else
                {
                    string signed_message = $"[{"Client" + connected_clients.IndexOf(client)}]: {message}";
                    Console.WriteLine(signed_message);

                    SendMessageToAll(signed_message);
                }


            }

            client.stream.Close();
            client.tcpClient.Close();

            connected_clients.Remove(client);
            serverFull = false;

        }

        /*static void LogMessage(string signed_message, Client client)
        {
            byte[] data = Encoding.ASCII.GetBytes(signed_message);
            client.stream.Write(data);
        }*/

        static void SendMessageToAll(string signed_message) 
        {
            foreach (Client client in connected_clients)
            {
                byte[] data = Encoding.ASCII.GetBytes(signed_message);
                client.stream.Write(data);
            }
        }
    }
}
