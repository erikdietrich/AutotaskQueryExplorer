using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public class ResultSetBuilder : IBuildResultSets
    {
        public ResultSet BuildResultSet(SqlQuery query, IEnumerable<Entity> entities)
        {
            var entityType = Type.GetType(String.Format("{0}.{1}", typeof(Entity).Namespace, query.Entity), true, true);
            var columnHeaders = GetColumnHeaders(query, entityType);

            var set = new ResultSet(columnHeaders);
            foreach (var entity in entities)
                set.Add(BuildResultSetRow(columnHeaders, entity).ToList());

            return set;
        }

        private static IEnumerable<string> GetColumnHeaders(SqlQuery query, Type entityType)
        {
            return query.Columns.Any() ? query.Columns : GetQualifyingPropertiesOfType(entityType);
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

        private static IEnumerable<string> GetQualifyingPropertiesOfType(Type entityType)
        {
            var allProperties = entityType.GetProperties();
            var qualifyingProperties = allProperties.Where(pr => pr.Name == "id").Union(
                allProperties.Where(pr => pr.PropertyType == typeof(Object)));
            return qualifyingProperties.Select(pr => pr.Name.ToLower());
        }
    }
}
