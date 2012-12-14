using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;
using System.Reflection;

namespace AutotaskQueryService
{
    public class BasicQueryService : IQueryService
    {
        private readonly ATWS _webService = new ATWS();

        public ResultSet ExecuteQuery(string command)
        {
            var sqlQuery = new SqlQuery(command);
            var entities = GetEntitiesFromAutotask(sqlQuery.Entity, sqlQuery.WhereClause);
            if (!entities.Any())
                return ResultSet.Empty;

            return BuildResultSet(sqlQuery, entities);
        }

        public void Login(string userName, string password)
        {
            var zoneinfo = _webService.getZoneInfo(userName);
            var cred = new NetworkCredential(userName, password);
            var credCache = new CredentialCache();
            credCache.Add(new Uri(zoneinfo.URL), "Basic", cred);
            _webService.Credentials = credCache;
        }

        private IEnumerable<Entity> GetEntitiesFromAutotask(string entityName, string whereClause)
        {
            var autotaskQuery = new AutotaskQuery(entityName);

            if (!string.IsNullOrEmpty(whereClause))
                autotaskQuery.SetWhereClause(whereClause);

            return _webService.query(autotaskQuery.ToString()).EntityResults;
        }

        private static ResultSet BuildResultSet(SqlQuery sqlQuery, IEnumerable<Entity> entities)
        {
            var columnHeaders = GetColumnHeaders(sqlQuery, entities.First().GetType());
            var resultSet = new ResultSet(columnHeaders);

            foreach (var result in entities)
                resultSet.Add(BuildResultSetRow(columnHeaders, result).ToList());

            return resultSet;
        }

        private static IEnumerable<string> GetColumnHeaders(SqlQuery query, Type entityType)
        {
            var columns = new List<string>();
            if (!query.Columns.Any())
            {
                return entityType.GetProperties().Where(pr => pr.PropertyType == typeof(Object)).Select(pr => pr.Name.ToLower());
            }
            return query.Columns;
        }

        private static IEnumerable<string> BuildResultSetRow(IEnumerable<string> columns, Entity result)
        {
            foreach (string field in columns)
            {
                var type = result.GetType();
                var property = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                object propertyValue = property.GetValue(result, null);
                yield return string.Format("{0}", propertyValue);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_webService != null)
                    _webService.Dispose();
        }

        ~BasicQueryService()
        {
            Dispose(false);
        }
    }
}
