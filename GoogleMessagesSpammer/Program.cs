using System;
using System.Collections.Generic;

namespace GoogleMessagesSpammer
{
    internal class Program
    {
        private static Dictionary<string, bool> isRcs;

        public static void Main(string[] args)
        {
            GoogleMessagesClient.Setup();
            GoogleMessagesClient.CheckLogin();
            while (true)
            {
                Console.Write("Enter phone number to spam: ");
                GoogleMessagesClient.ChangeUser(Console.ReadLine());
                Console.WriteLine("How many times to send? (or -1)");
                var time = int.Parse(Console.ReadLine());
                Console.WriteLine("Write a message, end with !@! to finish.");
                var msg = "";
                while (true)
                {
                    var lastMsg = Console.ReadLine();
                    if (lastMsg == "!@!")
                        break;
                    msg += lastMsg;
                    msg += '\n';
                }

                for (var i = 0; i < time || time == -1; i++) GoogleMessagesClient.SendMessage(msg);
            }
        }
    }
}