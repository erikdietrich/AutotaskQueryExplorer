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

            const string query = "<queryxml><entity>Account</entity><query><field>id<expression op=\"greaterthan\">0</expression></field></query></queryxml>";
            var result = service.query(query);
            if (result.EntityResults.Any())
                result.EntityResults.Cast<Account>().ToList().ForEach(acc => Console.WriteLine("Account name is " + acc.AccountName));
            else
                Console.WriteLine("Error");

            Console.ReadLine();

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
