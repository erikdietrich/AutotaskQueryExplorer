using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AutotaskQueryExplorer.Infrastructure;
using AutotaskQueryService;

namespace AutotaskQueryExplorer.Results
{
    public class QueryToolViewModel : ViewModel
    {

        private ResultSet _Results;
        private readonly IQueryService _service;

        private string _query;
        public string Query
        {
            get { return _query; }
            set
            {
                if (_query != value)
                    RaisePropertyChanged();
                _query = value;
            }
        }

        public ResultSet Results
        {
            get { return _Results; }
            private set
            {
                _Results = value;
                RaisePropertyChanged();
            }
        }

        public ICommand RunQuery { get; private set; }

        #region Constructor

        /// <summary>Initializes a new instance of the QueryToolViewModel class.</summary>
        public QueryToolViewModel(IQueryService service)
        {
            _service = service;
            RunQuery = new SimpleCommand(ExecuteSelectQuery, IsQueryAvailable);
        }

        #endregion

        private void ExecuteSelectQuery()
        {
            Results = _service.ExecuteQuery(_query);
        }

        private bool IsQueryAvailable()
        {
            return !string.IsNullOrEmpty(_query);
        }

    }
}
