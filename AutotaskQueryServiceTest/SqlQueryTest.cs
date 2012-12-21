using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutotaskQueryService;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskQueryServiceTest
{
    [TestClass]
    public class SqlQueryTest
    {
        private SqlQuery Target { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            Target = new SqlQuery("SELECT id FROM Account");
        }

        [TestClass]
        public class Constructor
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_Exception_On_Badly_Formed_Sql()
            {
                ExtendedAssert.Throws<SqlQuery.InvalidSyntaxException>(() => new SqlQuery("asdf SELECT * FROM Account WHERE 1="));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Does_Not_Throw_Exception_On_Well_Formed_Sql()
            {
                ExtendedAssert.DoesNotThrow(() => new SqlQuery("SELECT id FROM Account"));
            }
        }

        [TestClass]
        public class Columns : SqlQueryTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_One_Element_For_One_Selected_Column()
            {
                Assert.AreEqual<int>(1, Target.Columns.Count());
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Text_Of_Selected_Column()
            {
                Assert.AreEqual<string>("id", Target.Columns.ElementAt(0));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Text_Of_Two_Selected_Columns()
            {
                const string column2 = "AccountName";
                Target = new SqlQuery(String.Format("SELECT id, {0} FROM Account", column2));

                Assert.AreEqual<string>(column2.ToLower(), Target.Columns.ElementAt(1));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Empty_List_When_Select_Contains_Star()
            {
                Target = new SqlQuery("SELECT * FROM Account");

                Assert.AreEqual<int>(0, Target.Columns.Count());
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Does_Not_Include_Commas_In_Field_Names()
            {
                var query = new SqlQuery("SELECT AccountName, id FROM Account");

                Assert.IsFalse(query.Columns.ElementAt(0).Contains(","));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Populates_Correctly_With_Lowercase_Select()
            {
                const string column2 = "AccountName";
                Target = new SqlQuery(String.Format("select id, {0} FROM Account", column2));

                Assert.AreEqual<string>(column2.ToLower(), Target.Columns.ElementAt(1));
            }
        }

        [TestClass]
        public class Entity : SqlQueryTest  
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Word_After_From_In_Sql_Statement()
            {
                const string entity = "account";
                Target = new SqlQuery(String.Format("SELECT * FROM {0}", entity));

                Assert.AreEqual<string>(entity, Target.Entity);
            }

            /// <summary>This is in response to a bug I discovered in live action while using the tool</summary>
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Correct_Value_When_From_Is_Not_Capitlized()
            {
                const string entity = "account";
                Target = new SqlQuery(String.Format("select * from {0}", entity));

                Assert.AreEqual<string>(entity, Target.Entity);
            }
        }

        [TestClass]
        public class WhereClause : SqlQueryTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Stuff_After_Where()
            {
                Target = new SqlQuery("SELECT * FROM Account WHERE id = 12");

                Assert.AreEqual<string>("id = 12", Target.WhereClause);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Empty_String_When_No_WhereClause_Exists()
            {
                Assert.AreEqual<string>(string.Empty, Target.WhereClause);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Does_Not_Include_OrderBy_When_There_Is_An_OrderBy_Clause()
            {
                string clause = "id = 12";
                Target = new SqlQuery(String.Format("SELECT * From Account WHERE {0} ORDER BY id", clause));

                Assert.AreEqual<string>(clause, Target.WhereClause);
            }
        }

        [TestClass]
        public class OrderByClause : SqlQueryTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Stuff_After_Order_By()
            {
                Target = new SqlQuery("SELECT * FROM Account ORDER BY id");

                Assert.AreEqual<string>("id", Target.OrderByClause);
            }
        }
    }
}