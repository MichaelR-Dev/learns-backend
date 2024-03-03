using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCServerManager
{
    enum ServerStatus
    {
        Offline,
        Outdated,
        Online
    }

    internal static class Server
    {
        public static string Name = string.Empty;
        public static string Description = string.Empty;
        public static string Version = string.Empty;
        public static ServerStatus Status = ServerStatus.Offline;

        public static string IP = string.Empty;
        public static int Port = 0;
        public static int MPort = 0;

        public static TcpListener? Listener;
        public static StreamReader streamReader;
        public static StreamWriter streamWriter;

        public static int CurrentPlayers = 0;
        public static int MaxPlayers = 0;

        public static DateTime LastUpdate = DateTime.MinValue;
        public static DateTime StartTime = DateTime.MinValue;

        private static void PrintServer()
        {
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

        public static void ViewStats()
        {
            PrintServer();
            PromptHelpers.PromptEnterKeyContinue();
            PromptHelpers.ReadClientClear();
        }

        public static void ViewLogs()
        {
            PrintServer();
            PromptHelpers.PromptEnterKeyContinue();
            PromptHelpers.ReadClientClear();
        }

        public static void OpenConsole()
        {
            PrintServer();
            PromptHelpers.PromptEnterKeyContinue();
            PromptHelpers.ReadClientClear();

            //Establish SSL over TCP

            //Maintain TCP Connection
        }

        public static void CloseConsole()
        {
            //Clear Console on Client
            PromptHelpers.ReadClientClear();

            //Send cancel connection to Manager Client
            Console.WriteLine("/dc");
        }

        public static void InitializeServer()
        {

            //If no server files, create server

            //If server files exist, start main server

            Name = "TEST";
            Description = "Test Desc";
            Version = "1.20.4";
            Status = ServerStatus.Online;
            IP = "127.0.0.1";
            Port = 25565;
            MPort = 25567;
            CurrentPlayers = 0;
            MaxPlayers = 10;
            LastUpdate = DateTime.Now;
            StartTime = DateTime.MinValue;

            //Start manager server
            _ = StartManagerServer();
        }

        public static void StartMainServer()
        {
            /*string start_server_command = $"@echo off\njava -Xmx4096M -Xms4096M -jar minecraft_server_1.20.4.jar nogui";

            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = "C:/Users/kmedr/OneDrive/Desktop/Minecraft Servers List/Minecraft SMP 1.20.4/",
            };

            // Create the process & sets start info
            using (Process process = new Process { StartInfo = psi })
            {
                // Enable event-driven output reading
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);

                // Start the process
                process.Start();

                // Begin asynchronous reading of the output
                process.BeginOutputReadLine();

                process.StandardInput.WriteLine(start_server_command);

                // Wait for the process to finish
                process.WaitForExit();
            }*/
        }

        public static async Task StartManagerServer()
        {
            bool shutdown = false;

            try
            {

                // TcpListener is listening on specified port.
                Listener = new TcpListener(IPAddress.Parse(IP), MPort);

                // Start listening for client requests.
                Listener.Start();

                Console.WriteLine("Server is waiting for a connection...");
                TcpClient? client = null;

                // Enter the listening loop.
                while (!shutdown)
                {
                    // Blocks until a client has connected to the server. NOTE: KEEPS ACCEPTING CLIENTS
                    client ??= Listener.AcceptTcpClient();
                    Console.WriteLine("Client connected! Writing/Reading redirecting to NetworkStream...");

                    // handle communication with the connected client.
                    await HandleClient(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Stop listening for new clients.
                Listener?.Stop();
            }

        }

        static async Task HandleClient(TcpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            TcpClient tcpClient = client;
            NetworkStream clientStream = tcpClient.GetStream();

            // Create a StreamWriter/Reader to write/read to the network stream
            streamReader = new(clientStream, Encoding.UTF8);
            streamWriter = new(clientStream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            // Set Console.Out & Console.In to the Streams
            Console.SetOut(streamWriter);
            Console.SetIn(streamReader);

            //WORKS WHEN HITTING ENTER IN MENU? MAYBE WAITING FOR INPUT?
            _ = Task.Run(() => {
                while (true)
                {
                    Thread.Sleep(2000);

                    object serializedUpdateServerInfo = new
                    {
                        //Update data
                        Name,
                        Description,
                        Version,
                        Status,
                        CurrentPlayers,
                        MaxPlayers,
                        DateTime.Now,
                        DateTime.MinValue
                    };

                    string serializedString = JsonConvert.SerializeObject(serializedUpdateServerInfo);
                    Console.WriteLine("/up" + serializedString);
                }
            });

            while (true)
            {
                try
                {
                    ServerMenuManager.manageServerMenu.AccessMenu();
                }
                catch(IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                    break;
                }
            }

            // Close the connection.
            tcpClient.Close();

            // Flush and close the StreamWriter and StreamReader when done
            await streamWriter.FlushAsync();
            streamWriter.Close();
            streamReader.Close();

        }

        public static void StopServer()
        {
            PrintServer();
            PromptHelpers.PromptEnterKeyContinue();
            PromptHelpers.ReadClientClear();
        }

    }
}
