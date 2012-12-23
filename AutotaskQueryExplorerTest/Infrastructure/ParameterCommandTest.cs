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
    public class ParameterCommandTest
    {
        [TestClass]
        public class Constructor
        {
            /// <remarks>Not a lot of point in having a command without this</remarks>
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_ArgumentNullException_If_Execute_Is_Null()
            {
                ExtendedAssert.Throws<ArgumentNullException>(() => new ParameterCommand<string>(null));
            }
        }

        [TestClass]
        public class CanExecute
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_True_When_No_Func_Is_Specified()
            {
                var command = new ParameterCommand<string>((string s) => { });

                Assert.IsTrue(command.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_False_When_Func_Returns_False()
            {
                var command = new ParameterCommand<string>((string s) => { }, (string s) => { return false; });

                Assert.IsFalse(command.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_InvalidOperationException_When_Wrong_Type_Is_Passed_In()
            {
                var command = new ParameterCommand<string>((string s) => { }, (string s) => { return false; });

                ExtendedAssert.Throws<InvalidOperationException>(() => command.CanExecute(12));
            }
        }

        [TestClass]
        public class Execute
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Executes_Passed_In_Action()
            {
                bool wasCalled = false;
                var command = new ParameterCommand<int>((i) => { wasCalled = true; });
                command.Execute(12);

                Assert.IsTrue(wasCalled);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_InvalidOperationException_On_Wrong_ArgumentType()
            {
                var command = new ParameterCommand<int>((i) => { });
                ExtendedAssert.Throws<InvalidOperationException>(() => command.Execute("fdsa"));
            }
        }
    }
}
