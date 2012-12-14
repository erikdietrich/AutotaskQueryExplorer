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
            using (_service = new BasicQueryService())
            {
                var gatekeeper = new Gatekeeper(_service);
                gatekeeper.TryLoginUntilSuccess();
                int x = 6; int y = 7;
                if((x & y) > 7)
                    Console.WriteLine("asdF");

                Console.Write(">");

                for (string query = Console.ReadLine(); !IsExitRequest(query); query = Console.ReadLine())
                    ExecuteQuery(query);
            }
        }

        private static bool IsExitRequest(string query)
        {
            return query.Equals("exit", StringComparison.CurrentCultureIgnoreCase);
        }

        private static void ExecuteQuery(string query)
        {
            var resultSet = TryExecuteQuery(query);

            foreach (var headerItem in resultSet.HeaderRow)
                Console.Write(string.Format(" {0} ", headerItem));

            Console.Write("\n\n");

            foreach (var row in resultSet)
                WriteRow(row);

            Console.Write(">");
        }

        private static void WriteRow(IEnumerable<string> row)
        {
            foreach (var column in row)
                Console.Write(string.Format(" {0} ", column));
            Console.Write('\n');
        }

        private static ResultSet TryExecuteQuery(string command)
        {
            try
            {
                return _service.ExecuteQuery(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid query caused error: " + ex.Message);
                return ResultSet.Empty;
            }
        }        
    }
}
