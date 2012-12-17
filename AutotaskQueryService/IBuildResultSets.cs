using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public interface IBuildResultSets
    {
        ResultSet BuildResultSet(SqlQuery query, IEnumerable<Entity> entities);
    }
}
