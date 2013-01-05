using System;
using System.Collections.Generic;
using System.IO;
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
        private static IQueryService _service;

        private static ResultSet _mostRecent;

        static void Main(string[] args)
        {
            using(var webService = new AutotaskWebService())
            {
                _service = new BasicQueryService(webService: webService);
                var gatekeeper = new Gatekeeper(_service);
                gatekeeper.TryLoginUntilSuccess();

                LoopInCommandModeUntilExitRequest();
            }
        }

        private static void LoopInCommandModeUntilExitRequest()
        {
            Console.Write(">");
            for (string command = Console.ReadLine(); !IsExitCommand(command); command = Console.ReadLine())
                ExecuteCommand(command);
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
            return command.Equals("clear", StringComparison.InvariantCultureIgnoreCase) || command.StartsWith("export", StringComparison.InvariantCultureIgnoreCase);
        }

        private static void PerformNonQueryCommand(string command)
        {
            if (command.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                Console.Clear();
            else if (command.StartsWith("export", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!TryDumpToFile(command.Split(' ').FirstOrDefault(str => !str.Equals("export", StringComparison.InvariantCultureIgnoreCase))))
                    Console.WriteLine("Invalid file specified.");
            }
        }

        private static void PerformQueryCommand(string query)
        {
            _mostRecent = TryExecuteQuery(query);
            WriteHeader(_mostRecent);
            WriteResults(_mostRecent);
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

        #region This contains the mechanics of should probably become its own class

        private static bool TryDumpToFile(string filename)
        {
            try
            {
                DumpToFile(filename);
            }
            catch { return false; }
            return true;
        }

        private static void DumpToFile(string filename)
        {
            using (var stream = File.CreateText(filename))
            {
                WriteHeader(_mostRecent, stream);
                WriteResults(_mostRecent, stream);
            }
        }

        private static void WriteHeader(ResultSet resultSet, TextWriter writer = null)
        {
            var theWriter = writer ?? Console.Out;
            theWriter.WriteLine(String.Join("|", resultSet.HeaderRow));
            theWriter.Write("\n");
        }

        private static void WriteResults(ResultSet resultSet, TextWriter writer = null)
        {
            foreach (var row in resultSet)
                WriteRowToConsole(row, writer);
        }

        private static void WriteRowToConsole(IList<string> row, TextWriter writer = null)
        {
            var theWriter = writer ?? Console.Out;
            theWriter.WriteLine(string.Join("|", row));
        }

        #endregion
    }
}
