using System;
using System.Collections.Generic;
using System.Linq;
using AutotaskQueryService;
using AutotaskQueryService.net.autotask.webservices5;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AutotaskQueryServiceTest
{
    [TestClass]
    public class BasicQueryServiceTest
    {
        private static IEnumerable<T> GenerateEntities<T>(int count) where T : Entity, new()
        {
            for (int index = 0; index < count; index++)
                yield return new T() { id = index + 1 };
        }

        public BasicQueryService Target { get; set; }

        public IBuildResultSets Builder { get; set; }
        public IAutotaskWebService WebService { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            WebService = Mock.Create<IAutotaskWebService>();
            Builder = Mock.Create<IBuildResultSets>();

            Target = new BasicQueryService(webService: WebService);
        }

        [TestClass]
        public class ExecuteQuery : BasicQueryServiceTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Invokes_Web_Service_On_Valid_Sql_Query()
            {
                WebService.Arrange(s => s.Query(Arg.AnyString)).Returns(new ATWSResponse() { EntityResults = new Entity[] { } }).MustBeCalled();

                Target.ExecuteQuery("SELECT * FROM Invoice");

                WebService.Assert();
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Orders_Entities_Returned_From_Service()
            {
                WebService.Arrange(s => s.Query(Arg.AnyString)).Returns(new ATWSResponse() { EntityResults = new Entity[] { new Invoice() { AccountID = 2 }, new Invoice() { AccountID = 1 } } });

                var results = Target.ExecuteQuery("SELECT AccountID FROM Invoice WHERE AccountID > 0 ORDER BY AccountID");

                Assert.AreEqual<string>("2", results.ElementAt(1).ElementAt(0));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Queries_WebService_Twice_If_Max_Number_Of_Results_Returned()
            {
                var response = new ATWSResponse() { EntityResults = GenerateEntities<Invoice>(500).ToArray() };
                var secondResponse = new ATWSResponse() { EntityResults = GenerateEntities<Invoice>(1).ToArray() };

                WebService.Arrange(s => s.Query(Arg.AnyString)).Returns(response).InSequence();
                WebService.Arrange(s => s.Query(Arg.AnyString)).Returns(secondResponse).InSequence();

                var results = Target.ExecuteQuery("SELECT * from Invoice");

                Assert.AreEqual<int>(response.EntityResults.Count() + secondResponse.EntityResults.Count(), results.Count());
            }
        }

        [TestClass]
        public class Login : BasicQueryServiceTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_InvalidOperationException_When_WebService_Does_Not_Return_Any_EntityInfo()
            {
                WebService.Arrange(s => s.GetZoneInfo(Arg.AnyString)).Returns(new ATWSZoneInfo() { URL = "http://daedtech.com" });
                WebService.Arrange(s => s.GetEntityInfo()).Returns(new EntityInfo[] { });

                ExtendedAssert.Throws<InvalidOperationException>(() => Target.Login("Asdf", "fdas"));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Does_Not_Throw_Exception_When_Entity_Info_Is_Returned()
            {
                WebService.Arrange(s => s.GetZoneInfo(Arg.AnyString)).Returns(new ATWSZoneInfo() { URL = "http://daedtech.com" });
                WebService.Arrange(s => s.GetEntityInfo()).Returns(new EntityInfo[] { new EntityInfo() });

                ExtendedAssert.DoesNotThrow(() => Target.Login("Asdf", "fdas"));
            }
        }
    }
}
