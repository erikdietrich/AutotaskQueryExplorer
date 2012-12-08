using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskShell.AutotaskService;

namespace AutotaskShell
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = GetUserName();
            string pass = GetPassword();
            var service = LoginToAutotask(userName, pass);
            Console.Write(">");

            for (string command = Console.ReadLine(); command.ToLower() != "exit"; command = Console.ReadLine())
            {
                string entity = command.Split('.')[0];
                string field = command.Split('.')[1];

                string query = string.Format("<queryxml><entity>{0}</entity><query><field>id<expression op=\"greaterthan\">0</expression></field></query></queryxml>", entity);
                var results = service.query(query);
                results.EntityResults.Cast<Account>().ToList().ForEach(acc => Console.WriteLine("Account name is " + acc.AccountName));

                foreach (var result in results.EntityResults)
                {
                    Console.WriteLine(result.GetType().GetProperty(field).GetValue(result, null));
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
        private static ATWS LoginToAutotask(string userName, string pass)
        {
            var service = new ATWS();
            var zoneinfo = service.getZoneInfo(userName);
            var cred = new NetworkCredential(userName, pass);
            var credCache = new CredentialCache();
            credCache.Add(new Uri(zoneinfo.URL), "Basic", cred);
            service.Credentials = credCache;
            return service;
        }
    }
}
