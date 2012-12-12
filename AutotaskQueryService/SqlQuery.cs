using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;


namespace AutotaskQueryService
{
    public class SqlQuery
    {
        private const string SELECT = "SELECT";
        private const string FROM = "FROM";
        private const string WHERE = "WHERE";

        public class InvalidSyntaxException : Exception { }

        private readonly string _sql;

        public IEnumerable<string> Columns { get { return GetColumnsFromSqlStatement(_sql); } }

        public string Entity { get { return GetEntityFromSql(_sql); } }

        public string WhereClause 
        {
            get 
            {
                int index = GetIndexOfLastCharacter(_sql, WHERE);
                bool didWeFindTheString = index >= 0;
                return didWeFindTheString ? _sql.Substring(index).Trim() : string.Empty;
            } 
        }

        public SqlQuery(string sql)
        {
            if (ContainsSyntaxErrors(sql))
                throw new InvalidSyntaxException();

            _sql = sql;
        }

        private static bool ContainsSyntaxErrors(string sql)
        {
            var parser = new TSql100Parser(true);
            IList<ParseError> errors;
            parser.Parse(new StringReader(sql), out errors);
            return errors.Count > 0;
        }

        private static IEnumerable<string> GetColumnsFromSqlStatement(string sqlStatement)
        {
            int startIndex = GetIndexOfLastCharacter(sqlStatement, SELECT);
            int endIndex = sqlStatement.IndexOf(FROM, StringComparison.InvariantCultureIgnoreCase);
            var columns = GetColumnNamesFromStatement(sqlStatement, startIndex, endIndex);

            if (DoColumnsRepresentSelectAll(columns))
                return Enumerable.Empty<string>();

            return columns;
        }

        private static string GetEntityFromSql(string sqlStatement)
        {
            return sqlStatement.Split(' ').SkipWhile(token => token != FROM).First(token => token != FROM);
        }

        private static int GetIndexOfLastCharacter(string sqlStatement, string substring)
        {
            int startIndex = sqlStatement.IndexOf(substring, StringComparison.InvariantCultureIgnoreCase);
            return startIndex >= 0 ? startIndex + substring.Length : -1;
        }

        private static IEnumerable<string> GetColumnNamesFromStatement(string sqlStatement, int startIndex, int endIndex)
        {
            var columnText = sqlStatement.Substring(startIndex, endIndex - startIndex);
            var nonEmptyTokens = columnText.Split(' ').Where(token => !string.IsNullOrEmpty(token));
            return nonEmptyTokens.Select(token => token.Trim().Replace(",", string.Empty));
        }
        private static bool DoColumnsRepresentSelectAll(IEnumerable<string> columns)
        {
            return columns.Count() == 1 && columns.ElementAt(0) == "*";
        }
    }
}
