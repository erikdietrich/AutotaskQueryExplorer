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
        private readonly IAutotaskWebService _webService;

        private readonly IBuildResultSets _builder;

        /// <summary>Dependency injected constructor</summary>
        /// <param name="builder">The result set builder scheme to use</param>
        public BasicQueryService(IBuildResultSets builder = null, IAutotaskWebService webService = null)
        {
            _builder = builder ?? new ResultSetBuilder();
            _webService = webService ?? new AutotaskWebService();
        }

        public ResultSet ExecuteQuery(string command)
        {
            var sqlQuery = new SqlQuery(command);
            var entities = GetEntitiesFromAutotask(sqlQuery.Entity, sqlQuery.WhereClause);
            var orderedEntities = OrderBy(entities, sqlQuery.OrderByClause);
            return _builder.BuildResultSet(sqlQuery, orderedEntities);
        }

        public void Login(string userName, string password)
        {
            SetWebServiceCredentials(userName, password);
            if (!AreWeConnectedToWebService())
                throw new InvalidOperationException();
        }

        void SetWebServiceCredentials(string userName, string password)
        {
            var zoneinfo = _webService.GetZoneInfo(userName);
            var credential = new NetworkCredential(userName, password);
            var credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(zoneinfo.URL), "Basic", credential);
            _webService.Credentials = credentialCache;
        }

        private bool AreWeConnectedToWebService()
        {
            return _webService.GetEntityInfo().Any();
        }

        private IEnumerable<Entity> GetEntitiesFromAutotask(string entityName, string whereClause)
        {
            var autotaskQuery = new AutotaskQuery(entityName);

            if (!string.IsNullOrEmpty(whereClause))
                autotaskQuery.SetWhereClause(whereClause);

            return _webService.Query(autotaskQuery.ToString()).EntityResults;
        }

        private static IEnumerable<T> OrderBy<T>(IEnumerable<T> entities, string propertyName)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            var propertyInfo = entities.First().GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return entities.OrderBy(e => propertyInfo.GetValue(e, null));
        }
    }
}
