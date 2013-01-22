using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutotaskQueryExplorer.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutotaskQueryExplorerTest.Login
{
    [TestClass]
    public class ViewModelTest
    {
        private class ChildViewModel : ViewModel
        {
            public void RaiseParentPropertyChanged(string propertyName)
            {
                RaisePropertyChanged();
            }
        }

        [TestClass]
        public class RaisePropertyChanged
        {
            [TestMethod, Owner("ebd"), TestCategory("Proven"), TestCategory("Unit")]
            public void Triggers_PropertyChanged()
            {
                string raisedPropertyName = null;
                var target = new ChildViewModel();
                target.PropertyChanged += (o, e) => raisedPropertyName = e.PropertyName;
                target.RaiseParentPropertyChanged("blah");

                Assert.AreEqual<string>("RaiseParentPropertyChanged", raisedPropertyName);
                
            }
        }
    }
}
