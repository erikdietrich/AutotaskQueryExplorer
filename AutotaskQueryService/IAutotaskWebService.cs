using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public interface IAutotaskWebService : IDisposable
    {
        ICredentials Credentials { get; set; }

        ATWSZoneInfo GetZoneInfo(string userName);

        EntityInfo[] GetEntityInfo();

        ATWSResponse Query(string queryText);
    }
}
