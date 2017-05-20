using OpenQA.Selenium;
using System;
using System.Threading;


namespace UITests.Utilities
{
    public class WaitHelpers
    {
        public void WaitForElement(IWebElement elementToWait, int timesToWait = 6,
            int millisecondsToWaitEachTime = 1000)
        {
            bool elementFound = false;
            int timesAttempted = 1;

            while (!elementFound && timesAttempted < timesToWait)
            {
                try
                {
                    if (elementToWait.Enabled)
                    {
                        elementFound = true;
                    }
                    else
                    {
                        Thread.Sleep(millisecondsToWaitEachTime);
                    }
                }
                catch (Exception)
                {
                }
                timesAttempted++;
            }

            if (!elementFound)
            {
                throw new Exception(string.Format("[FAILED] Element '{0}' Not found", elementToWait));
            }
        }



        public void WaitForSeconds(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

    }
}