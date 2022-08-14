using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest
{
    public class BasePage : Base
    {
        public BasePage(TestPackage TestPackage)
        {
            this.TestPackage = TestPackage;
        }
    }
}
