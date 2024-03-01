using System;
using System.Collections.Generic;

namespace MCServerManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MenuManager.GenerateMenus();
                ServerManager.InitializeServerManager();
                ServerManager.ServersList.Add(new Server("test", "testdesc", "127.0.0.1", 25565, 25575, true));

                MenuManager.mainMenu.AccessMenu();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            Console.WriteLine();
            Environment.Exit(0);

        }

    }
}
