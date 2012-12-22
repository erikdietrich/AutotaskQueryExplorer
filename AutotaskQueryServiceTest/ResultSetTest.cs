using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskQueryServiceTest
{
    [TestClass]
    public class ResultSetTest
    {
        private ResultSet Target { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            Target = new ResultSet(new List<string>() { "1", "2" });
        }

        [TestClass]
        public class Constructor : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_HeaderRow_To_Passed_In_Row()
            {
                var header = new List<string>() { "1", "2" };
                Target = new ResultSet(header);

                Assert.AreEqual<int>(header.Count, Target.HeaderRow.Count);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_ArgumentNullException_On_Null_HeaderRow()
            {
                ExtendedAssert.Throws<ArgumentNullException>(() => new ResultSet(null));
            }
        }

        [TestClass]
        public class Add : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_InvalidFieldCountException_When_Added_Row_Size_Doesnt_Match_Header()
            {
                ExtendedAssert.Throws<ResultSet.InvalidFieldCountException>(() => Target.Add(new List<string>()));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Increments_Count_By_One()
            {
                var firstRow = new List<string>() { "2", "4" };
                int originalCount = Target.Count;

                Target.Add(firstRow);

                Assert.AreEqual<int>(originalCount + 1, Target.Count);
            }
        }

        [TestClass]
        public class Clear : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Sets_Count_To_Zero()
            {
                Target.Add(new List<string>() { "1", "2" });

                Target.Clear();

                Assert.AreEqual<int>(0, Target.Count);
            }
        }

        [TestClass]
        public class Contains : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_True_When_Row_Exists()
            {
                var row = new List<string>() { "a", "b" };
                Target.Add(row);

                Assert.IsTrue(Target.Contains(row));
            }
        }

        [TestClass]
        public class CopyTo : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Copies_ResultRows_To_Target_Array()
            {
                var row = new List<string>() { "a", "b" };
                Target.Add(row);
                var rows = new IList<string>[3];
                Target.CopyTo(rows, 0);

                Assert.AreEqual<string>("a", rows[0].ElementAt(0));
            }
        }

        [TestClass]
        public class IsReadOnly : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_False()
            {
                Assert.IsFalse(Target.IsReadOnly);
            }
        }

        [TestClass]
        public class Remove : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Removes_Item_In_Question()
            {
                var row = new List<string>() { "a", "b" };
                Target.Add(row);
                Target.Remove(row);

                Assert.AreEqual<int>(0, Target.Count);
            }
        }

        [TestClass]
        public class Enumeration : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Supports_For_Each()
            {
                Target.Add(new List<string>() { "a", "b" });
                int count = 0;
                foreach (var row in Target)
                    count++;

                Assert.AreEqual<int>(1, count);
            }
        }

        [TestClass]
        public class Empty : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Result_Set_With_Zero_Rows()
            {
                Assert.AreEqual<int>(0, ResultSet.Empty.Count);
            }
        }

        [TestClass]
        public class IndexOf : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_IndexOf_Row_In_Question()
            {
                var row = new List<string>() { "a", "b" };
                Target.Add(row);

                Assert.AreEqual<int>(0, Target.IndexOf(row));
            }
        }

        [TestClass]
        public class Insert : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Inserts_Row_Into_Position_In_Question()
            {
                var firstRow = new List<string>() { "a", "b" };
                var secondRow = new List<string>() { "b", "c" };

                Target.Add(firstRow);
                Target.Insert(0, secondRow);

                Assert.AreEqual<int>(0, Target.IndexOf(secondRow));
            }
        }

        [TestClass]
        public class RemoveAt : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Removes_Element_In_Question()
            {
                var firstRow = new List<string>() { "a", "b" };
                var secondRow = new List<string>() { "b", "c" };

                Target.Add(firstRow);
                Target.Add(secondRow);

                Target.RemoveAt(0);

                Assert.IsTrue(Target.IndexOf(secondRow) >= 0);
            }
        }

        [TestClass]
        public class Indexer : ResultSetTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Can_Access_Elements()
            {
                var firstRow = new List<string>() { "a", "b" };

                Target.Add(firstRow);

                Assert.AreEqual<string>(firstRow[0], Target[0][0]);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Can_Set_Elements()
            {
                var firstRow = new List<string>() { "a", "b" };

                Target.Add(new List<string>() { null, null });
                Target[0] = firstRow;

                Assert.AreEqual<string>(firstRow[0], Target[0][0]);
            }
        }
    }
}
