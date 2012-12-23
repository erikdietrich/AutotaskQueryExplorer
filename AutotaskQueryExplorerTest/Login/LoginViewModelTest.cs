using AutotaskQueryExplorer.Infrastructure;
using AutotaskQueryExplorer.Login;
using AutotaskQueryService;
using AutotaskQueryServiceTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.JustMock;

namespace AutotaskQueryExplorerTest.Login
{
    [TestClass]
    public class LoginViewModelTest
    {
        private LoginViewModel Target { get; set; }

        private IQueryService Service;

        [TestInitialize]
        public void BeforeEachTest()
        {
            Service = Mock.Create<IQueryService>();
            Target = new LoginViewModel(Service);
        }

        [TestClass]
        public class UserName : LoginViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_Null()
            {
                Assert.IsNull(Target.Username);
            }
        }

        [TestClass]
        public class Password : LoginViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_Null()
            {
                Assert.IsNull(Target.Password);
            }
        }

        [TestClass]
        public class Login : LoginViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_To_InstanceOf_SimpleCommand()
            {
                Assert.IsInstanceOfType(Target.Login, typeof(SimpleCommand));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void CanExecute_Returns_False_When_UserName_Is_Null()
            {
                Target.Username = null;

                Assert.IsFalse(Target.Login.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void CanExecute_Returns_True_When_Username_And_Pass_Are_Non_Empty()
            {
                Target.Username = "someuser";
                Target.Password = "weakpass";

                Assert.IsTrue(Target.Login.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void CanExecute_Returns_False_When_Pass_Is_Empty()
            {
                Target.Username = "someuser";
                Target.Password = string.Empty;

                Assert.IsFalse(Target.Login.CanExecute(null));
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Execute_Invokes_Service_Login()
            {
                Target = new LoginViewModel(Service) { Username = "asdf", Password = "fdsa" };
                Mock.Arrange(() => Service.Login(Arg.IsAny<string>(), Arg.IsAny<string>())).MustBeCalled();
                Target.Login.Execute(null);
                
                Mock.Assert(Service);
            }

            /// <remarks>I originally inlined the logic in the command getter, so TDD to cache the first instance</remarks>
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Returns_Same_Object_Instance_On_Successive_Accesses()
            {
                var command = Target.Login;
                var secondCommand = Target.Login;

                Assert.AreEqual<ICommand>(command, secondCommand);
            }
        }

        [TestClass]
        public class IsUserLoggedIn : LoginViewModelTest
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Initializes_False()
            {
                Assert.IsFalse(Target.IsUserLoggedIn);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Is_True_After_Successful_Login()
            {
                Target.Username = "asdf";
                Target.Password = "fda";
                Target.Login.Execute(null);

                Assert.IsTrue(Target.IsUserLoggedIn);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Is_False_After_Failed_Login()
            {
                Target.Username = "asdf";
                Target.Password = "fasdF";
                Mock.Arrange(() => Service.Login(Arg.IsAny<string>(), Arg.IsAny<string>())).Throws(new Exception());
                Target.Login.Execute(null);

                Assert.IsFalse(Target.IsUserLoggedIn);
            }

            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Raises_PropertyChanged_When_State_Changes()
            {
                bool wasCalled = false;
                Target.Username = "asdf";
                Target.Password = "fda";
                Target.PropertyChanged += (o, e) => wasCalled = true;
                Target.Login.Execute(null);

                Assert.IsTrue(wasCalled);
            }
        }
    }
}
