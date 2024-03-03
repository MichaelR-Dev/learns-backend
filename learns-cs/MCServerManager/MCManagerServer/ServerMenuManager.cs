using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//Depends on CoreRCON https://www.nuget.org/packages/CoreRCON#readme-body-tab thanks to them!

namespace MCServerManager
{
    /**
     *  Main Menu
     * 
     *  List Server(s)
     *  Manage Server
     *  Add Existing Server
     *  Create New Server
     *  
     *  Close MC Manager
     *  
     */

    /**
     *  Server List
     *  
     *  [Expanding List of Servers and Stats]
     *  
     *  Select Server
     *  
     *  or
     *  
     *  Backspace to Return to Main Menu
     *  
     */

    /**
     *  Manage Server (Selected Server)
     *  
     *  Return to Main Menu
     *  
     *  View Stats
     *  View Logs
     *  Open Console
     *  
     *  Stop Server
     *  
     */

    internal static class ServerMenuManager
    {
        public static Menu manageServerMenu;

        public static void GenerateMenus()
        {

            manageServerMenu = new Menu("\n     Manage Server     \n", "", [

                new Option("Return to Main Menu\n", "Esc", ConsoleKey.Escape, ()=>{ Server.CloseConsole(); }),

                new Option("View Stats", "1", ConsoleKey.D1, ()=>{ Server.ViewStats(); }),
                new Option("View Logs", "2", ConsoleKey.D2, ()=>{ Server.ViewLogs(); }),
                new Option("Open Console\n", "3", ConsoleKey.D3, ()=>{ Server.OpenConsole(); }),
                
                new Option("Remove Server", "del", ConsoleKey.Delete, () => { 
                    
                    //WRAP INTO CONFIRM ACTION
                    Server.StopServer();
                
                }),

                new Option("Stop Server", "x", ConsoleKey.X, ()=>{ 
                    
                    //WRAP INTO CONFIRM ACTION
                    Server.StopServer();
                
                }),

            ]);
        }
    }

    struct Option(string optionName, string selectKeyString, ConsoleKey selectKey, Action callback)
    {
        public string optionName = optionName;
        public string selectKeyString = selectKeyString;
        public ConsoleKey selectKey = selectKey;

        public Action callback = callback;
    }

    struct Menu(string title, string description, List<Option> options)
    {
        public string title = title;
        public string? description = description ?? "";
        public List<Option> options = options;

        public readonly void PrintMenu()
        {
            string padding = "  ";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(padding + title);
            Console.ResetColor();

            if (description != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(padding + description);
                Console.ResetColor();
            }

            foreach (Option option in options)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{padding}{options.Find(searchOption => searchOption.Equals(option)).selectKeyString}) ");
                Console.ResetColor();
                Console.WriteLine(option.optionName);
            }

            Console.WriteLine("");
            Console.WriteLine($"{padding}Viewing: [{Server.IP + ':' + Server.Port}] {Server.Name}");

        }

        public readonly void AccessMenu()
        {
            ConsoleKeyInfo selectedKey;
            int pressKeyDelay = 200;

            PrintMenu();

            do
            {
                Thread.Sleep(pressKeyDelay);

                selectedKey = PromptHelpers.ReadClientKey();
                Option? selectedOption = options.FirstOrDefault(option => option.selectKey == selectedKey.Key);

                if(!selectedOption.Equals(default(Option)))
                {
                    PromptHelpers.ReadClientClear();
                    selectedOption.Value.callback();
                    PrintMenu();
                }

            } while (selectedKey.Key != ConsoleKey.Escape);

            PromptHelpers.ReadClientClear();
        }
    }
}
