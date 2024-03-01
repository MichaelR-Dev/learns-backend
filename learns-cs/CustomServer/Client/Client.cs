using System.Net.Sockets;
using System.Drawing;
using System.Text;

namespace CustomClient
{
    internal class Client
    {
        // Keep a log of messages
        readonly static List<string> messageLog = [];

        static void Main()
        {

            try
            {
                Console.WriteLine("Please specify ip:");
                string? ip = Console.ReadLine();
                byte[] data;

                if (string.IsNullOrWhiteSpace(ip)) {
                    ip = "127.0.0.1";
                }

                TcpClient client = new(ip, 50000);
                NetworkStream stream = client.GetStream();

                _ = HandleServerAsync(client, stream);
                Console.WriteLine("Type exit to close connection...");

                while (true)
                {

                    string message = Console.ReadLine() ?? "";

                    if (message.ToLower().Equals( "exit" ))
                    {
                        break;
                    }

                    if(!string.IsNullOrWhiteSpace(message))
                    {
                        data = Encoding.ASCII.GetBytes(message);
                        stream.Write(data);
                    }

                }

                stream.Close();
                client.Close();

            }catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
            }
            
        }

        static async Task HandleServerAsync(TcpClient connection, NetworkStream stream)
        {

            // Handle communication with the server using the stream
            while (true)
            {
                byte[] data = new byte[1024];
                int bytesRead = await stream.ReadAsync(data);

                if (bytesRead == 0)
                {
                    // No more data, the server has disconnected the client
                    Console.WriteLine($"Disconnected from server...");
                    break;

                }

                string message = Encoding.ASCII.GetString(data, 0, bytesRead).TrimEnd('\0');
                if(!string.IsNullOrWhiteSpace(message))
                {
                    messageLog.Add(message);
                    DisplayMessageLog();
                }

            }

            // Close the connection when done
            stream.Close();
            connection.Close();

        }

        static void DisplayMessageLog()
        {
            // Clear the console
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
            Console.WriteLine("Type exit to close connection...");
        }
    }
}
