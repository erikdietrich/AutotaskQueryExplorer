using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService.net.autotask.webservices5;

namespace AutotaskQueryService
{
    public class AutotaskWebService : IAutotaskWebService
    {
        private readonly ATWS _webService = new ATWS();

        public ICredentials Credentials { get { return _webService.Credentials; } set { _webService.Credentials = value; } }

        public ATWSZoneInfo GetZoneInfo(string userName)
        {
            return _webService.getZoneInfo(userName);
        }

        public EntityInfo[] GetEntityInfo()
        {
            return _webService.getEntityInfo();
        }

        public ATWSResponse Query(string queryText)
        {
            return _webService.query(queryText);
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

        ~AutotaskWebService()
        {
            Dispose(false);
        }

    }
}
