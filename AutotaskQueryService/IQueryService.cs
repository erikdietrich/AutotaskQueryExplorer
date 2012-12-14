using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public interface IQueryService : IDisposable
    {
        ResultSet ExecuteQuery(string command);

        void Login(string userName, string password);
    }
}
