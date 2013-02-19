using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService;
using AutotaskQueryService.net.autotask.webservices5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskQueryServiceTest
{
    [TestClass]
    public class ResultSetBuilderTest
    {
        private ResultSetBuilder Target { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            Target = new ResultSetBuilder();
        }

        [TestClass]
        public class BuildResultSet : ResultSetBuilderTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Empty_On_Empty_Entity_Collection()
            {
                var entities = Enumerable.Empty<Entity>();

                var resultSet = Target.BuildResultSet(new SqlQuery("SELECT * FROM Invoice"), entities);

                Assert.AreEqual<int>(entities.Count(), resultSet.Count);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Single_Row_For_Single_Entity()
            {
                var entities = new List<Invoice>() { new Invoice() };

                var resultSet = Target.BuildResultSet(new SqlQuery("SELECT * FROM Invoice"), entities);

                Assert.AreEqual<int>(entities.Count, resultSet.Count);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Result_Set_With_Column_Count_Matching_Query()
            {
                var entities = Enumerable.Empty<Invoice>();

                var resultSet = Target.BuildResultSet(new SqlQuery("SELECT id FROM Invoice"), entities);

                Assert.AreEqual<int>(1, resultSet.HeaderRow.Count);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Set_With_id_Column()
            {
                var entities = Enumerable.Empty<Invoice>();

                var resultSet = Target.BuildResultSet(new SqlQuery("SELECT * FROM Invoice"), entities);

                Assert.IsTrue(resultSet.HeaderRow.Contains("id"));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Set_With_id_Column_First()
            {
                var entities = Enumerable.Empty<Invoice>();

                var resultSet = Target.BuildResultSet(new SqlQuery("SELECT * FROM Invoice"), entities);

                Assert.IsTrue(resultSet.HeaderRow.First() == "id");
            }
        }
    }
}
