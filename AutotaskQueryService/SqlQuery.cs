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
        private const string SELECT = "select";
        private const string FROM = "from";
        private const string WHERE = "where";

        public class InvalidSyntaxException : Exception { }

        private readonly string _sql;

        public IEnumerable<string> Columns { get { return GetColumnsFromSqlStatement(_sql); } }

        public string Entity 
        { 
            get 
            { 
                return _sql.Split(' ').SkipWhile(token => token != FROM).First(token => token != FROM); 
            } 
        }

        public string WhereClause
        {
            get
            {
                int index = GetIndexOfLastCharacter(WHERE);
                bool didWeFindTheString = index >= 0;
                return didWeFindTheString ? _sql.Substring(index).Trim() : string.Empty;
            }
        }

        public SqlQuery(string sql)
        {
            if (ContainsSyntaxErrors(sql))
                throw new InvalidSyntaxException();

            _sql = sql.ToLower();
        }

        private static bool ContainsSyntaxErrors(string sql)
        {
            var parser = new TSql100Parser(true);
            IList<ParseError> errors;
            parser.Parse(new StringReader(sql), out errors);
            return errors.Count > 0;
        }

        private IEnumerable<string> GetColumnsFromSqlStatement(string sqlStatement)
        {
            int startIndex = GetIndexOfLastCharacter(SELECT);
            int endIndex = sqlStatement.IndexOf(FROM);
            var columns = GetNonEmptyTokensBetween(startIndex, endIndex).Select(str => str.Replace(",", string.Empty));

            if (DoColumnsRepresentSelectAll(columns))
                return Enumerable.Empty<string>();

            return columns;
        }

        private int GetIndexOfLastCharacter(string substring)
        {
            int startIndex = _sql.IndexOf(substring);
            return startIndex >= 0 ? startIndex + substring.Length : -1;
        }

        private IEnumerable<string> GetNonEmptyTokensBetween(int startIndex, int endIndex)
        {
            var columnText = _sql.Substring(startIndex, endIndex - startIndex);
            var nonEmptyTokens = columnText.Split(' ').Where(token => !string.IsNullOrEmpty(token));
            return nonEmptyTokens.Select(token => token.Trim());
        }

        private static bool DoColumnsRepresentSelectAll(IEnumerable<string> columns)
        {
            return columns.Count() == 1 && columns.ElementAt(0) == "*";
        }
    }
}
