using MCServerManager;
using System.Net.Sockets;

namespace MCManagerServer
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                //Generate menus
                ServerMenuManager.GenerateMenus();

                //Start Server and Get Server Info
                //Simulating Server Start
                Server.InitializeServer();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
