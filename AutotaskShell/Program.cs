using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService;
using AutotaskShell.AutotaskService;

namespace AutotaskShell
{
    class Program
    {
        private static BasicQueryService _service;

        static void Main(string[] args)
        {
            string userName = GetUserName();
            string pass = GetPassword();
            _service = new BasicQueryService();
            _service.Login(userName, pass);

            Console.Write(">");

            for (string command = Console.ReadLine(); command.ToLower() != "exit"; command = Console.ReadLine())
            {
                foreach(var row in _service.ExecuteQuery(command))
                {
                    foreach (var column in row)
                        Console.Write(string.Format(" {0} ", column));
                    Console.Write('\n');
                }
                Console.Write(">");
            }
        }
        
        private static string GetUserName()
        {
            Console.Write("UserName: ");
            var userName = Console.ReadLine();
            return userName;
        }

        private static string GetPassword()
        {
            Console.Write("Password: ");
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            var pass = Console.ReadLine();
            Console.ForegroundColor = originalColor;
            return pass;
        }
        
    }
}
