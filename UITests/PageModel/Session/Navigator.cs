using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using UITests.SiteModel.Base;
using System.Collections.Generic;


namespace UITests.SiteModel.Session
{
    public class Navigator
    {
        
        private readonly IWebDriver _webDriver;

        public Navigator(string driver)  {
           
            switch (driver)
            {
                case "firefox":                   
                    break;
                case "chrome":                                       
                    _webDriver = new ChromeDriver();
                    break;
                case "phantomJS":                   
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(200));
        }

        public T Visit<T>() where T : PageObject
        {
            var page = (T)Activator.CreateInstance(typeof(T), _webDriver);
            return Visit(page);
        }

        public T Visit<T>(T page) where T : PageObject
        {
            var url = (string) page.GetType().GetField("URL").GetValue(null);
            if (url != null)
            {
                _webDriver.Navigate().GoToUrl(url);
            }
            else
            {
                throw new Exception("No URL is available on the page " + typeof (T).FullName);
            }
            return page;
        }

        public void Visit(string url)
        {
            if (url != null)
            {
                _webDriver.Navigate().GoToUrl(url);
            }
        }

        public T GoTo<T>() where T : PageObject
        {
            var page = (T)Activator.CreateInstance(typeof(T), _webDriver);
            return page;
        }

        public void Maximize()
        {
            _webDriver.Manage().Window.Maximize();
        }          

        public void OpenNewTab()
        {
            var newTab = new Actions(_webDriver);
            newTab.SendKeys(Keys.Control + "t").Perform();
        }

        public void SwitchToNewTab()
        {
            var tabs = new List<String>(_webDriver.WindowHandles);
            _webDriver.SwitchTo().Window(tabs[1]);           
        }
        public void SwitchToNativeWindow()
        {
            var tabs = new List<String>(_webDriver.WindowHandles);
            _webDriver.SwitchTo().Window(tabs[0]);
        }

        public IWebDriver getDriver()
        {
            return _webDriver;
        }
      
        public void CloseBrowser()
        {
            _webDriver.Close();
        }


        internal void Refresh()
        {
            
        }

        public void KillBrowserSession()
        {
            var nativeDriverQuit = Task.Factory.StartNew(() => _webDriver.Quit());
            if (!nativeDriverQuit.Wait(TimeSpan.FromSeconds(10)))
            {
                var currentProcessPid = Process.GetCurrentProcess().Id;
                foreach (var process in Process.GetProcesses())
                {
                    using (var mo = new ManagementObject("win32_process.handle='" + process.Id.ToString(CultureInfo.InvariantCulture) + "'"))
                    {
                        mo.Get();
                        var parentPid = Convert.ToInt32(mo["ParentProcessId"]);

                        if (parentPid == currentProcessPid)
                        {
                            process.Kill();
                        }
                    }
                }
            }
        }
    }
}