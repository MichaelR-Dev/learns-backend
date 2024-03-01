using System;
using System.Linq;

namespace MCServerManager
{
    internal static class OptionsMap
    {
        #region misc methods

        public static string PromptStringConfirmable(string prompt)
        {
            string confirm = "n";
            string prompt_answer;

            do
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                prompt_answer = Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nConfirm Answer: [y] or [n]");
                Console.ResetColor();

                confirm = Console.ReadLine();

                Console.Clear();


            } while (!confirm.ToLower().Equals("y"));

            return prompt_answer;
        }

        public static string PromptIPConfirmable(string prompt)
        {
            string confirm = "n";
            string prompt_answer = "127.0.0.1";

            do
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                if (ValidateIP(Console.ReadLine(), out string result))
                {
                    prompt_answer = result;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nConfirm Answer: [y] or [n]");
                    Console.ResetColor();

                    confirm = Console.ReadLine();

                    Console.Clear();

                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid IP Try Again");
                    Console.ResetColor();

                    PromptEnterKeyContinue(false);

                    Console.Clear();

                }

            } while (!confirm.ToLower().Equals("y"));

            return prompt_answer;
        }

        public static int PromptIntConfirmable(string prompt, Action prompt_action = null)
        {

            string confirm = "n";
            int prompt_answer;

            do
            {
                if(prompt_action != null)
                {
                    prompt_action();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out prompt_answer)) {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nConfirm Answer: [y] or [n]");
                    Console.ResetColor();

                    confirm = Console.ReadLine();

                    Console.Clear();

                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid Input Try Again");
                    Console.ResetColor();

                    PromptEnterKeyContinue(false);

                    Console.Clear();

                }

            } while (!confirm.ToLower().Equals("y"));

            return prompt_answer;
        }

        public static int PromptPortConfirmable(string prompt)
        {
            string confirm = "n";
            int prompt_answer;

            do
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out prompt_answer) && prompt_answer > 0 && prompt_answer < 65535)
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nConfirm Answer: [y] or [n]");
                    Console.ResetColor();

                    confirm = Console.ReadLine();

                    Console.Clear();

                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid Port Try Again");
                    Console.ResetColor();

                    PromptEnterKeyContinue(false);

                    Console.Clear();

                }

            } while (!confirm.ToLower().Equals("y"));

            return prompt_answer;
        }

        public static void PromptEnterKeyContinue(bool leadingNewLine = true)
        {
            if(leadingNewLine)
            {
                Console.WriteLine();
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        public static bool ValidateIP(string ip, out string result)
        {
            string[] bytes = ip.Split('.');

            if (bytes.Length == 4 
                && int.TryParse(bytes[0], out int _) 
                && int.TryParse(bytes[1], out int _) 
                && int.TryParse(bytes[2], out int _) 
                && int.TryParse(bytes[3], out int _)
                )
            {
                result = ip;
                return true;
            }

            result = null;
            return false;
        }
        
        #endregion

        #region main menu options

        public static void Option_ListServers(bool isSelectingServer)
        {

            if(isSelectingServer)
            {
                int selectedIndex = PromptIntConfirmable("Select Server by ID:", () =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("               Server(s) List               ");
                    Console.ResetColor();

                    foreach (Server server in ServerManager.ServersList)
                    {
                        int index = ServerManager.ServersList.IndexOf(server);
                        server.PrintServer(index + 1);
                    }
                });

                if(selectedIndex > 0 && selectedIndex <= ServerManager.ServersList.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Server ID: {selectedIndex} Selected");
                    Console.ResetColor();
                    ServerManager.currentServerSelected = ServerManager.ServersList.ElementAt(selectedIndex - 1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Server, Returning...");
                    Console.ResetColor();
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("               Server(s) List               ");
                Console.ResetColor();

                if (ServerManager.ServersList.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No Servers Tracked");
                    Console.ResetColor();
                }
                else
                {
                    foreach (Server server in ServerManager.ServersList)
                    {
                        server.PrintServer();
                    }
                }
            }

            PromptEnterKeyContinue();
            Console.Clear();

        }

        public static Action Option_ManageServer = () =>
        {
            if(ServerManager.currentServerSelected != null)
            {
                MenuManager.manageServerMenu.AccessMenu();
            }
            else
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Server Selected, Returning...");
                Console.ResetColor();

                PromptEnterKeyContinue(false);
                Console.Clear();
            }

        };

        public static Action Option_AddExistingServer = () =>
        {

            string server_name = PromptStringConfirmable("Enter server name:");
            string server_description = PromptStringConfirmable("Enter server description:");
            string server_ip = PromptIPConfirmable("Enter server ip:");
            int server_port = PromptPortConfirmable("Enter server port:");
            int rcon_port = PromptPortConfirmable("Enter server rcon port:");

            new Server(server_name, server_description, server_ip, server_port, rcon_port);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Server Added Successfully");
            Console.ResetColor();

            PromptEnterKeyContinue(false);
            Console.Clear();

        };

        public static Action Option_CreateNewServer = () =>
        {

            string server_name = PromptStringConfirmable("Enter server name:");
            string server_description = PromptStringConfirmable("Enter server description:");
            string server_ip = PromptIPConfirmable("Enter server ip:");
            int server_port = PromptPortConfirmable("Enter server port:");
            int rcon_port = PromptPortConfirmable("Enter server rcon port:");

            new Server(server_name, server_description, server_ip, server_port, rcon_port);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Server Created Successfully");
            Console.ResetColor();

            PromptEnterKeyContinue(false);
            Console.Clear();

        };

        #endregion
    }
}
