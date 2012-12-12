using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutotaskQueryService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskQueryServiceTest
{
    [TestClass]
    public class AutotaskQueryTest
    {
        protected AutotaskQuery Target { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            Target = new AutotaskQuery("thisCanBeAnything");
        }

        [TestClass]
        public class Constructor
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_Entity_To_Passed_In_Value()
            {
                const string entity = "Asadf";
                var query = new AutotaskQuery(entity);

                Assert.AreEqual<string>(entity, query.Entity);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_ArgumentException_If_Entity_Is_Null_Or_Empty()
            {
                ExtendedAssert.Throws<ArgumentException>(() => new AutotaskQuery(null));
                ExtendedAssert.Throws<ArgumentException>(() => new AutotaskQuery(string.Empty));
            }
        }

        [TestClass]
        public class ToStringMethod : AutotaskQueryTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Starts_With_Query_Xml_Tag()
            {
                StringAssert.StartsWith(Target.ToString(), String.Format("<{0}>", AutotaskQuery.RootNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Ends_With_queryxml_tag_end()
            {
                StringAssert.EndsWith(Target.ToString(), String.Format("</{0}>", AutotaskQuery.RootNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Entity_Tag()
            {
                StringAssert.Contains(Target.ToString(), string.Format("<{0}>", AutotaskQuery.EntityNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Entity_Name()
            {
                const string entityName = "adsf";
                Target = new AutotaskQuery(entityName);

                StringAssert.Contains(Target.ToString(), entityName);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Query_Tag()
            {
                StringAssert.Contains(Target.ToString(), string.Format("<{0}>", AutotaskQuery.QueryNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_field_Tag()
            {
                StringAssert.Contains(Target.ToString(), String.Format("<{0}>", AutotaskQuery.FieldNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_Text_id_When_No_Clauses_Are_Specified()
            {
                StringAssert.Contains(Target.ToString(), "id");
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_expression_tag()
            {
                StringAssert.Contains(Target.ToString(), String.Format("</{0}>", AutotaskQuery.ExpressionNodeName));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_op_attribute()
            {
                StringAssert.Contains(Target.ToString(), "op=");
            }

            /// <summary>This basically amounts to a SELECT * query, but since Autotask API doesn't support
            /// select all, we have to supply some trivially true condition by default</summary>
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Contains_greaterthan_With_Value_Zero_For_Query_With_No_Condition_Specified()
            {
                StringAssert.Contains(Target.ToString(), "op=\"greaterthan\">0");
            }
        }

        [TestClass]
        public class SetWhereClause : AutotaskQueryTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Results_In_ToString_Containing_New_Op()
            {
                Target.SetWhereClause("id = 12");

                StringAssert.Contains(Target.ToString(), "op=\"equals\">12");
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Works_For_NotEqual()
            {
                Target.SetWhereClause("id <> 21");

                StringAssert.Contains(Target.ToString(), "op=\"notequal\">21");
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Works_For_Less_Than()
            {
                Target.SetWhereClause("asdf < 21");

                StringAssert.Contains(Target.ToString(), "op=\"lessthan\">21");
            }
        }
    }
}