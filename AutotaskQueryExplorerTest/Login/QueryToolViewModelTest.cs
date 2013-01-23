using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AutotaskQueryExplorer.Infrastructure;
using AutotaskQueryExplorer.Results;
using AutotaskQueryService;
using AutotaskQueryServiceTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AutotaskQueryExplorerTest.Login
{
    [TestClass]
    public class QueryToolViewModelTest
    {
        private QueryToolViewModel Target { get; set; }

        private IQueryService MockService { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            MockService = Mock.Create<IQueryService>();
            MockService.Arrange(serv => serv.ExecuteQuery(Arg.AnyString)).Returns(new ResultSet(Enumerable.Empty<string>()));

            Target = new QueryToolViewModel(MockService);
        }

        [TestClass]
        public class Query : QueryToolViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_Null()
            {
                Assert.IsNull(Target.Query);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Raises_PropertyChanged_On_Change()
            {
                bool wasCalled = false;
                Target.PropertyChanged += (o, e) => wasCalled = true;
                Target.Query = "asdf";

                Assert.IsTrue(wasCalled);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Does_Not_Raise_Property_Changed_When_Set_To_The_Same_Thing()
            {
                bool wasCalled = false;
                Target.PropertyChanged += (o, e) => wasCalled = true;
                Target.Query = Target.Query;

                Assert.IsFalse(wasCalled);
            }
        }

        [TestClass]
        public class RunQuery : QueryToolViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void CanExecute_Initializes_False()
            {
                Assert.IsFalse(Target.RunQuery.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void CanExecute_Returns_True_When_Query_Has_Text()
            {
                Target.Query = "Asdf";
                Assert.IsTrue(Target.RunQuery.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Execute_Results_In_Non_Null_Result_Set()
            {
                Target.Query = "fdas";
                Target.RunQuery.Execute(null);

                Assert.IsNotNull(Target.Results);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Execute_Invokes_QueryService()
            {
                Target.Query = "asdf";
                MockService.Arrange(qs => qs.ExecuteQuery(Target.Query)).MustBeCalled();

                Target.RunQuery.Execute(null);

                MockService.Assert();
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Execute_Results_In_PropertyChanged_For_Results()
            {
                bool wasCalled = false;
                Target.PropertyChanged += (o, e) => { if (e.PropertyName == "Results") wasCalled = true; };
                Target.Query = "asdf";
                Target.RunQuery.Execute(null);

                Assert.IsTrue(wasCalled);
            }
        }

        [TestClass]
        public class Results : QueryToolViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_Null()
            {
                Assert.IsNull(Target.Results);
            }
        }
    }
}
