using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITests.PageModel.Pages
{
    public class LoginPage : UITests.SiteModel.Base.PageObject
    {
        public static string URL = ConfigurationManager.AppSettings.Get("SeleniumTestUrl");
         public LoginPage(IWebDriver driver)
            : base(driver)
        {
        }

    }
}
