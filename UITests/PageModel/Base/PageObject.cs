using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using UITests.Utilities;
using System.Configuration;

namespace UITests.SiteModel.Base
{
    public abstract class PageObject
    {
        protected readonly IWebDriver driver;
        public PageObject(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Helpers      

       

        // Scrolls the view port of the driver window to the top of the page
        protected void ScrollWindowToTopOfThePage()
        {
            Thread.Sleep(2000);
            ((IJavaScriptExecutor) driver).ExecuteScript("scroll(0, 0)");
        }

        // Scrolls the view port down the specified number of pixels
        protected void ScrollWindow(int pixelHeight)
        {
            string scroller = string.Format("scroll(0, {0})", pixelHeight);
            ((IJavaScriptExecutor) driver).ExecuteScript(scroller);
        }

        // Scrolls the view port down to the specified element
        protected void ScrollWindowToElement(IWebElement element)
        {          
            int x = element.Location.X;
            int y = element.Location.Y;
            string scroller = string.Format("scroll({0}, {1})", x, y);
            ((IJavaScriptExecutor) driver).ExecuteScript(scroller);
        }

          protected void UploadFile(IWebElement uploadButton, string fileName)
        {
            var fullFilePath = AppDomain.CurrentDomain.BaseDirectory + ConfigurationSettings.AppSettings["ImagePath"] + fileName;
            ScrollWindowToElement(uploadButton);
            Thread.Sleep(2000);
            uploadButton.Click();

            var succeeded = AutoItX.AU3_WinWait("Open", "", 30);

            //Attempt to active window
            AutoItX.AU3_WinActivate("Open", "");
            
            ////Wait until window is opened
            //while (AutoItX.AU3_WinActive("Open", null) != 1)
            //{
            //    Thread.Sleep(100);
            //}

            Thread.Sleep(2000);
            AutoItX.AU3_ControlSetText("Open", null, "Edit1", fullFilePath);
            Thread.Sleep(2000);
            AutoItX.AU3_ControlClick("Open", null, "Button1", null, 1, 10, 10);

            ////Wait until window is closed
            //while (AutoItX.AU3_WinActive("Open", null) == 1)
            //{
            //    Thread.Sleep(100);
            //}
        }
 

        #endregion
    }
}