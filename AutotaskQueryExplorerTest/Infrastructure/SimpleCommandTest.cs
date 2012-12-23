using AutotaskQueryExplorer.Infrastructure;
using AutotaskQueryServiceTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutotaskQueryExplorerTest.Infrastructure
{
    [TestClass]
    public class SimpleCommandTest
    {
        private readonly Action DoNothing = () => { };

        private SimpleCommand Target { get; set; }

        [TestInitialize]
        public void BeforeEachTest()
        {
            Target = new SimpleCommand(DoNothing);
        }

        [TestClass]
        public class Constructor
        {
            /// <remarks>Having an instance of this class is terminally stupid if the command is null</remarks>
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_ArgumentNullExcpetion_On_Null_Command()
            {
                ExtendedAssert.Throws<ArgumentNullException>(() => new SimpleCommand(null));
            }
        }

        [TestClass]
        public class CanExecute : SimpleCommandTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_True_When_No_CanExecute_Is_Specified()
            {
                Assert.IsTrue(Target.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_False_When_Passed_In_Func_Returns_False()
            {
               Target = new SimpleCommand(DoNothing, () => { return false; });

                Assert.IsFalse(Target.CanExecute(null));
            }
        }

        [TestClass]
        public class Execute : SimpleCommandTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Executes_Passed_In_Action()
            {
                bool wasCalled = false;
                Target = new SimpleCommand(() => { wasCalled = true; });
                Target.Execute(null);

                Assert.IsTrue(wasCalled);
            }
        }
    } 
}
