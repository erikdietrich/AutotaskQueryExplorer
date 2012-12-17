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

                Console.Write(">");

                for (string command = Console.ReadLine(); !IsExitCommand(command); command = Console.ReadLine())
                    ExecuteCommand(command);
            }
        }

        private static bool IsExitCommand(string query)
        {
            return query.Equals("exit", StringComparison.CurrentCultureIgnoreCase);
        }

        private static void ExecuteCommand(string command)
        {
            if (IsNonQueryCommand(command))
                PerformNonQueryCommand(command);
            else
                PerformQueryCommand(command);

            Console.Write(">");
        }

        private static bool IsNonQueryCommand(string command)
        {
            return command.Equals("clear", StringComparison.InvariantCultureIgnoreCase);
        }

        private static void PerformNonQueryCommand(string command)
        {
            if(command.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                Console.Clear();
        }

        private static void PerformQueryCommand(string query)
        {
            var resultSet = TryExecuteQuery(query);
            WriteHeader(resultSet);
            WriteResults(resultSet);
        }

        private static void WriteHeader(ResultSet resultSet)
        {
            foreach (var headerItem in resultSet.HeaderRow)
                Console.Write(string.Format(" {0} ", headerItem));

            Console.Write("\n\n");
        }
        private static void WriteResults(ResultSet resultSet)
        {
            foreach (var row in resultSet)
                WriteRow(row);
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
