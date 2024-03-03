using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCServerManager
{
    internal static class ServerManager
    {
        public static List<Server> ServersList;
        public static Server currentServerSelected;

        public static void InitializeServerManager()
        {
            ServersList = new List<Server>();
        }

        public static async Task InitializeServerConnections()
        {

            // Create an array of tasks
            Task[] tasks = new Task[] { };
            foreach (Server server in ServersList)
            {
                tasks.Append(Task.Run(() => server.TryConnectMPORT()));
            }

            try
            {
                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void AddExistingServer(string name, string description, string ip, int port, int rcon_port) 
        {  
            ServersList.Add(new Server(name, description, ip, port, rcon_port)); 
        }

        public static void OpenServerConsole(ref Server server)
        {
            bool Speaking = true;

            try
            {
                while (Speaking)
                {

                    // Receive the response from the server.
                    string response = server.MConnection.Client_reader.ReadLine();

                    if (response.Equals("/i"))
                    {

                        string sendMessage = Console.ReadLine();
                        server.MConnection.Client_writer.WriteLine(sendMessage);

                    }
                    else if (response.Equals("/ik"))
                    {

                        ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                        object serializedKeyInfo = new
                        {
                            keyInfo.KeyChar,
                            keyInfo.Key,
                            keyInfo.Modifiers
                        };

                        string serializedString = JsonConvert.SerializeObject(serializedKeyInfo);
                        server.MConnection.Client_writer.WriteLine(serializedString);

                    }
                    else if (response.Equals("/c"))
                    {
                        Console.Clear();
                    }
                    else if (response.Equals("/up"))
                    {
                        Console.WriteLine("ping!");
                        // Receive serialized string from the network stream
                        string receivedString = response.Substring(3);

                        // Deserialize the received string into SerializedConsoleKeyInfo
                        SerializedUpdateServerInfo receivedServerInfo = JsonConvert.DeserializeObject<SerializedUpdateServerInfo>(receivedString);
                        currentServerSelected.UpdateServer(receivedServerInfo);
                    }
                    else if (response.Equals("/dc"))
                    {
                        break;
                    }
                    else
                    {
                        // Display the server's response.
                        Console.WriteLine(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                server.MConnection.Client_writer.Flush();
                Console.WriteLine("Exiting server connection...");
                Console.WriteLine("note: server connection is still active");

            }
        }
    }

    internal struct SerializedUpdateServerInfo
    {
        public string Name;
        public string Description;
        public string Version;
        public ServerStatus Status;
        public int CurrentPlayers;
        public int MaxPlayers;
        public DateTime LastUpdate;
        public DateTime StartTime;
    }

    internal enum ServerStatus
    {
        Offline,
        MPort_FAIL,
        Outdated,
        Online
    }

    internal class MConnection
    {
        public TcpClient Client_to_server { get; set; }
        public NetworkStream Client_stream { get; set; }
        public StreamWriter Client_writer { get; set; }
        public StreamReader Client_reader { get; set; }
    }

    internal class Server
    {
        //Identifiers
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public ServerStatus Status { get; set; }

        //Network
        public string IP {  get; set; }
        public int Port { get; set; }
        public int MPort { get; set; }
        public MConnection MConnection { get; set; }

        //Info
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime StartTime { get; set; }

        // Constructor for Existing Server
        public Server(string name, string description, string ip, int port, int mport, bool dontAdd = false)
        {

            this.Name = name;
            this.Description = description;
            this.IP = ip;
            this.Port = port;

            //MCONNECTION INFO
            this.MPort = mport;
            this.MConnection = new MConnection();

            this.Version = "xx.xx.xx";
            this.Status = ServerStatus.Offline;
            this.CurrentPlayers = 0;
            this.MaxPlayers = 0;

            this.LastUpdate = DateTime.MinValue;
            this.StartTime = DateTime.MinValue;

            // Only for testing purposes
            if (!dontAdd)
            {
                ServerManager.ServersList.Add(this);
            }
        }

        public void PrintServer(int id = 0)
        {

            if(id > 0)
            {

                Console.WriteLine("________________________________________________________________________________________");

                Console.WriteLine($"\n(ID: {id})");

                switch (Status)
                {
                    case ServerStatus.Offline:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"Status: Offline");
                        Console.ResetColor();
                        break;

                    case ServerStatus.Online:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"Status: Online");
                        Console.ResetColor();

                        break;

                    case ServerStatus.Outdated:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"Status: Outdated");
                        Console.ResetColor();
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        break;

                    default:
                        Console.Write($"Status: Unknown");
                        break;
                }


                Console.Write($" | Name: {Name} | Desc: {Description}\n");
                Console.WriteLine($"Socket: {IP}:{Port} | Version: {Version} | Players: {CurrentPlayers}/{MaxPlayers}");
                Console.WriteLine($"Last Version Update: {LastUpdate} | Last Restart: {StartTime}");

                Console.ResetColor();

                Console.WriteLine("________________________________________________________________________________________");

            }
            else
            {

                Console.WriteLine("________________________________________________________________________________________");
                Console.WriteLine();

                switch (Status)
                {
                    case ServerStatus.Offline:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"Status: Offline");
                        Console.ResetColor();
                        break;

                    case ServerStatus.Online:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"Status: Online");
                        Console.ResetColor();

                        break;

                    case ServerStatus.Outdated:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"Status: Outdated");
                        Console.ResetColor();
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        break;

                    default:
                        Console.Write($"Status: Unknown");
                        break;
                }

                Console.WriteLine($" | Name: {Name} | Desc: {Description}");
                Console.WriteLine($"Socket: {IP}:{Port} | Version: {Version} | Players: {CurrentPlayers}/{MaxPlayers}");
                Console.WriteLine($"Last Version Update: {LastUpdate} | Last Restart: {StartTime}");

                Console.ResetColor();

                Console.WriteLine("________________________________________________________________________________________");

            }

        }

        //On fail, MPORT unresponsive status
        //On success, MPORT responsive status
        public void TryConnectMPORT()
        {
            try
            {

                MConnection.Client_to_server = new TcpClient(IP, MPort);
                MConnection.Client_stream = MConnection.Client_to_server.GetStream();
                MConnection.Client_writer = new StreamWriter(MConnection.Client_stream, Encoding.UTF8);
                MConnection.Client_reader = new StreamReader(MConnection.Client_stream);

                MConnection.Client_writer.AutoFlush = true;

            }
            catch
            {

                CurrentPlayers = 0;
                Status = ServerStatus.MPort_FAIL;

            }

        }

        public void UpdateServer(SerializedUpdateServerInfo NewServerInfo)
        {
            this.Name = NewServerInfo.Name;
            this.Description = NewServerInfo.Description;
            this.Version = NewServerInfo.Version;   
            this.Status = NewServerInfo.Status;
            this.CurrentPlayers = NewServerInfo.CurrentPlayers;
            this.MaxPlayers = NewServerInfo.MaxPlayers;
            this.LastUpdate = NewServerInfo.LastUpdate;
            this.StartTime = NewServerInfo.StartTime;
        }

    }
}
