using System;
using System.Collections.Generic;
using CoreRCON;
using CoreRCON.Parsers.Standard;
using System.Net;
using System.Threading.Tasks;
using CoreRCON.PacketFormats;
using System.Data;

namespace MCServerManager
{
    internal static class ServerManager
    {
        public static List<Server> ServersList;
        public static Server? currentServerSelected;
        public static string rconPassword = "password";

        public static void InitializeServerManager()
        {
            ServersList = new List<Server>();
        }

        public static void AddExistingServer(string name, string description, string ip, int port, int rcon_port) 
        {  
            ServersList.Add(new Server(name, description, ip, port, rcon_port)); 
        }
    }

    enum ServerStatus
    {
        Offline,
        Outdated,
        Online
    }

    struct Server
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public ServerStatus Status { get; set; }
        public string IP {  get; set; }
        public int Port { get; set; }
        public int RCONPort { get; set; }
        public RCON RCONConnection { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime StartTime { get; set; }

        // Constructor for Existing Server
        public Server(string name, string description, string ip, int port, int rcon_port, bool dontAdd = false)
        {

            this.Name = name;
            this.Description = description;
            this.IP = ip;
            this.Port = port;
            this.RCONPort = rcon_port;
            this.RCONConnection = new RCON(IPAddress.Parse(ip), (ushort)rcon_port, ServerManager.rconPassword);

            this.Version = "xx.xx.xx";
            this.Status = ServerStatus.Offline;
            this.CurrentPlayers = 0;
            this.MaxPlayers = 0;

            this.LastUpdate = DateTime.MinValue;
            this.StartTime = DateTime.MinValue;

            //WHEN CREATING SERVER THE FOLLOWING MUST HAPPEN
            //grab rcon port
            //enable rcon in server.properties
            //set rcon password

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

                switch (this.Status)
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

                switch (this.Status)
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

        public void ViewStats()
        {
            ServerManager.currentServerSelected.Value.PrintServer();
            OptionsMap.PromptEnterKeyContinue();
            Console.Clear();
        }

        public void ViewLogs()
        {
            ServerManager.currentServerSelected.Value.PrintServer();
            OptionsMap.PromptEnterKeyContinue();
            Console.Clear();
        }

        public async Task OpenConsole()
        {
            if(RCONConnection == null)
            {
                RCONConnection = new RCON(new IPEndPoint(IPAddress.Parse(IP), RCONPort), ServerManager.rconPassword);
            }

            Console.Clear();
            Console.WriteLine($"Attempting RCON Connection to {IP + ":" + RCONPort}...");

            try
            {
                // Connect to server via rcon
                await RCONConnection.ConnectAsync();

                if (RCONConnection.Connected)
                {
                    Console.Clear();
                    Console.WriteLine($"Connection to {IP}:{RCONPort} Established...");

                    string command;
                    string response = "";
                    List<string> messageLog = new List<string>() { $"Connection to {IP}:{RCONPort} Established..." };

                    do
                    {
                        command = Console.ReadLine();

                        try
                        {
                            response = await RCONConnection.SendCommandAsync(command);
                            messageLog.Add(command);
                            messageLog.Add(response);
                        }
                        catch(Exception ex) 
                        { 
                            Console.WriteLine(ex.ToString());
                        }

                        Console.Clear();

                        // Display the message log
                        foreach (string message in messageLog)
                        {
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                Console.WriteLine(message);
                            }
                        }

                        // Display the "type exit" message at the end
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Type exit to close connection...");
                        Console.ResetColor();

                    } while (!command.Equals("exit"));

                    Console.WriteLine($"Closing console...");
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
            }

            OptionsMap.PromptEnterKeyContinue();
            Console.Clear();

            MenuManager.manageServerMenu.PrintMenu();
            
        }

        public void StopServer()
        {
            ServerManager.currentServerSelected.Value.PrintServer();
            OptionsMap.PromptEnterKeyContinue();
            Console.Clear();
        }

    }
}
