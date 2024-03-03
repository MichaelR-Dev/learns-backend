using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace MCServerManager
{
    internal static class PromptHelpers
    {
        #region misc methods

        public static string ReadClientLine() {
            Server.streamReader.DiscardBufferedData();
            // /i input
            Console.WriteLine("/i");
            return Server.streamReader.ReadLine() ?? "";
        }

        public static void ReadClientClear()
        {
            Server.streamReader.DiscardBufferedData();
            // /c clear
            Console.WriteLine("/c");
        }

        struct SerializedConsoleKeyInfo
        {
            public char KeyChar;
            public ConsoleKey Key;
            public ConsoleModifiers Modifiers;
        }

        public static ConsoleKeyInfo ReadClientKey()
        {
            Server.streamReader.DiscardBufferedData();
            // /ik inputkey
            Console.WriteLine("/ik");

            // Receiver
            // Receive serialized string from the network stream
            string receivedString = Server.streamReader.ReadLine() ?? "";

            // Deserialize the received string into SerializedConsoleKeyInfo
            var receivedKeyInfo = JsonConvert.DeserializeObject<SerializedConsoleKeyInfo>(receivedString);

            // Create a new ConsoleKeyInfo instance
            ConsoleKeyInfo reconstructedKeyInfo = new(
                receivedKeyInfo.KeyChar,
                receivedKeyInfo.Key,
                false,
                false,
                false
            );

            return reconstructedKeyInfo;
        }

        public static string PromptStringConfirmable(string prompt)
        {
            string confirm;
            string prompt_answer;

            do
            {

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                prompt_answer = Console.ReadLine() ?? "";

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nConfirm Answer: [y] or [n]");
                Console.ResetColor();

                confirm = Console.ReadLine() ?? "";

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

                if (ValidateIP(Console.ReadLine() ?? "", out var result))
                {
                    prompt_answer = result ?? "";

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nConfirm Answer: [y] or [n]");
                    Console.ResetColor();

                    confirm = Console.ReadLine() ?? "";

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

        public static int PromptIntConfirmable(string prompt, Action? prompt_action = null)
        {

            string confirm = "n";
            int prompt_answer;

            do
            {
                prompt_action?.Invoke();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(prompt);
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out prompt_answer)) {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nConfirm Answer: [y] or [n]");
                    Console.ResetColor();

                    confirm = Console.ReadLine() ?? "";

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

                    confirm = Console.ReadLine() ?? "";

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

        public static void PromptEnterKeyContinue(bool leadingNewLine = false)
        {
            if(leadingNewLine)
            {
                Console.WriteLine();
            }

            Console.WriteLine("Press Enter to continue...");
            ReadClientLine();
        }

        public static bool ValidateIP(string ip, out string? result)
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
    }
}
