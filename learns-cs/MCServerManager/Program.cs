using System;
using System.Threading.Tasks;

namespace MCServerManager
{
    internal class Program
    {

        static async Task Main()
        {
            try
            {
                MCMenuManager.GenerateMenus();

                ServerManager.InitializeServerManager();
                ServerManager.ServersList.Add(new Server("test", "testdesc", "127.0.0.1", 25565, 25567, true));

                Console.WriteLine("Initializing Server Connections...");
                await ServerManager.InitializeServerConnections();
                Console.Clear();

                MCMenuManager.mainMenu.AccessMenu();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Environment.Exit(0);

        }

    }
}
