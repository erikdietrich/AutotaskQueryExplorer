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
        private const string Where = "where";
        private const string OrderBy = "order by";

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

        public string WhereClause { get { return GetAfterToken(Where, OrderBy); } }

        public string OrderByClause { get { return GetAfterToken(OrderBy); } }

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

        private string GetAfterToken(string token, string nextToken = null)
        {
            int index = GetIndexOfLastCharacter(token);
            int length = GetLengthUntilNextTokenOrEnd(nextToken, index);

            bool didWeFindTheString = index >= 0;
            return didWeFindTheString ? _sql.Substring(index, length).Trim() : string.Empty;
        }

        private int GetIndexOfLastCharacter(string substring)
        {
            int startIndex = _sql.IndexOf(substring);
            return startIndex >= 0 ? startIndex + substring.Length : -1;
        }

        private int GetLengthUntilNextTokenOrEnd(string nextToken, int index)
        {
            int endindex = DoesNextTokenExist(nextToken, index) ? _sql.IndexOf(nextToken) : _sql.Length;
            return endindex > index ? endindex - index : _sql.Length - index;
        }

        private bool DoesNextTokenExist(string nextToken, int index)
        {
            return nextToken != null && _sql.IndexOf(nextToken, Math.Max(index, 0), StringComparison.InvariantCultureIgnoreCase) > 0;
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
