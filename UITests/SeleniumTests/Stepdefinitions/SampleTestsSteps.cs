using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using UITests.PageModel.Pages;
using UITests.TestHooks;

namespace UITests.SeleniumTests.Stepdefinitions
{
    [Binding]
    public sealed class SampleTestsSteps : Hooks
    {
        // For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef
        [Given(@"I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            Navigator.Visit<LoginPage>(); j          
        }
    }
}
