using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public class BasicQueryService
    {
        private readonly ATWS _webService = new ATWS();

        public IEnumerable<IEnumerable<string>> ExecuteQuery(string command)
        {
            var sqlQuery = new SqlQuery(command);
            var autotaskQuery = new AutotaskQuery(sqlQuery.Entity);
            if(!string.IsNullOrEmpty(sqlQuery.WhereClause))
                autotaskQuery.SetWhereClause(sqlQuery.WhereClause);

            var results = _webService.query(autotaskQuery.ToString());

            foreach (var result in results.EntityResults)
                yield return GetRow(sqlQuery, result);
        }

        public void Login(string userName, string password)
        {
            var zoneinfo = _webService.getZoneInfo(userName);
            var cred = new NetworkCredential(userName, password);
            var credCache = new CredentialCache();
            credCache.Add(new Uri(zoneinfo.URL), "Basic", cred);
            _webService.Credentials = credCache;
        }

        private static IEnumerable<string> GetRow(SqlQuery sqlQuery, Entity result)
        {
            foreach (string field in sqlQuery.Columns)
                yield return string.Format(" {0} ", result.GetType().GetProperty(field).GetValue(result, null));
        }
    }
}
