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

    internal static class MenuManager
    {
        public static Menu mainMenu;
        public static Menu manageServerMenu;

        public static void GenerateMenus()
        {
            mainMenu = new Menu("\n     Main Menu     \n", null, new List<Option>() { 

                new Option("List Server(s)", "1", ConsoleKey.D1, () => { OptionsMap.Option_ListServers(false); }),
                new Option("Select Server", "2", ConsoleKey.D2, () => { OptionsMap.Option_ListServers(ServerManager.ServersList.Count != 0); }),
                new Option("Manage Selected Server", "3", ConsoleKey.D3, OptionsMap.Option_ManageServer),
                new Option("Add Existing Server", "4", ConsoleKey.D4, OptionsMap.Option_AddExistingServer),
                new Option("Create New Server\n", "5", ConsoleKey.D5, OptionsMap.Option_CreateNewServer),
                
                new Option("Close MC Manager", "Esc", ConsoleKey.Escape, ()=>{ })

            });

            manageServerMenu = new Menu("\n     Manage Server     \n", null, new List<Option>() {

                new Option("Return to Main Menu\n", "Esc", ConsoleKey.Escape, ()=>{  }),

                new Option("View Stats", "1", ConsoleKey.D1, ()=>{ ServerManager.currentServerSelected.Value.ViewStats(); }),
                new Option("View Logs", "2", ConsoleKey.D2, ()=>{ ServerManager.currentServerSelected.Value.ViewLogs(); }),
                new Option("Open Console\n", "3", ConsoleKey.D3, ()=>{ ServerManager.currentServerSelected.Value.OpenConsole(); }),
                
                new Option("Remove Server", "del", ConsoleKey.Delete, () => { 
                    
                    //WRAP INTO CONFIRM ACTION
                    ServerManager.currentServerSelected.Value.StopServer();
                
                }),
                new Option("Stop Server", "x", ConsoleKey.X, ()=>{ 
                    
                    //WRAP INTO CONFIRM ACTION
                    ServerManager.currentServerSelected.Value.StopServer();
                
                }),

            });
        }
    }

    struct Option
    {
        public string optionName;
        public string selectKeyString;
        public ConsoleKey selectKey;

        public Action callback;

        public Option(string optionName, string selectKeyString, ConsoleKey selectKey, Action callback) 
        { 
            this.optionName = optionName;
            this.selectKeyString = selectKeyString;
            this.selectKey = selectKey;
            this.callback = callback;
        }
    }

    struct Menu
    {
        public string title;
        public string description;
        public List<Option> options;

        public Menu(string title, string description, List<Option> options)
        {
            this.title = title;
            this.description = description;
            this.options = options;
        }

        public void PrintMenu()
        {
            string padding = " ";

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
                Console.Write($" {padding}{options.Find(searchOption => searchOption.Equals(option)).selectKeyString}) ");
                Console.ResetColor();
                Console.WriteLine(option.optionName);
            }

            Console.WriteLine();
            if(ServerManager.currentServerSelected != null)
            {
                Console.WriteLine($" {padding}Selected: [{ServerManager.currentServerSelected.Value.IP + ':' + ServerManager.currentServerSelected.Value.Port}] {ServerManager.currentServerSelected.Value.Name}");
            }

        }

        public void AccessMenu()
        {
            ConsoleKeyInfo selectedKey;
            int pressKeyDelay = 200;

            PrintMenu();

            do
            {
                // Clear the input buffer
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }

                Thread.Sleep(pressKeyDelay);

                selectedKey = Console.ReadKey(intercept: true);
                Option? selectedOption = options.FirstOrDefault(option => option.selectKey == selectedKey.Key);

                if(!selectedOption.Equals(default(Option)))
                {
                    Console.Clear();

                    selectedOption.Value.callback();

                    PrintMenu();
                }

            } while (selectedKey.Key != ConsoleKey.Escape);

            Console.Clear();
        }
    }
}
