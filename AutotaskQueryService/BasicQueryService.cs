using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;
using System.Reflection;
using System.Linq.Expressions;

namespace AutotaskQueryService
{
    public class BasicQueryService : IQueryService
    {
        private readonly ATWS _webService = new ATWS();

        private readonly IBuildResultSets _builder;

        /// <summary>Dependency injected constructor</summary>
        /// <param name="builder">The result set builder scheme to use</param>
        public BasicQueryService(IBuildResultSets builder = null)
        {
            _builder = builder ?? new ResultSetBuilder();
        }

        public ResultSet ExecuteQuery(string command)
        {
            var sqlQuery = new SqlQuery(command);
            var entities = GetEntitiesFromAutotask(sqlQuery.Entity, sqlQuery.WhereClause);
            return _builder.BuildResultSet(sqlQuery, entities);
        }

        public void Login(string userName, string password)
        {
            var zoneinfo = _webService.getZoneInfo(userName);
            var cred = new NetworkCredential(userName, password);
            var credCache = new CredentialCache();
            credCache.Add(new Uri(zoneinfo.URL), "Basic", cred);
            _webService.Credentials = credCache;

            if (!_webService.getEntityInfo().Any())
                throw new InvalidOperationException();
        }

        private IEnumerable<Entity> GetEntitiesFromAutotask(string entityName, string whereClause)
        {
            var autotaskQuery = new AutotaskQuery(entityName);

            if (!string.IsNullOrEmpty(whereClause))
                autotaskQuery.SetWhereClause(whereClause);

            return _webService.query(autotaskQuery.ToString()).EntityResults;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _webService != null)
                _webService.Dispose();
        }

        ~BasicQueryService()
        {
            Dispose(false);
        }
    }
}
