using System;
using AutotaskQueryService;
using AutotaskQueryServiceTest;
using AutotaskShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskShellTest
{
    [TestClass]
    public class GatekeeperTest
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Throws_Exception_On_Null_Service()
            {
                ExtendedAssert.Throws<ArgumentNullException>(() => new Gatekeeper(null));
            }
        }
    }
}
