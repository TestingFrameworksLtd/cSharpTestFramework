using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using UITests.SiteModel.Session;
using UITests.SiteModel.Base;
using System.Configuration;
using UITests.Utilities;

namespace UITests.TestHooks
{
    public class TestRunSettings
    {
        public bool EnableReport = true;
    }

     public static class ReportOnHelpers
            {
               
                public static void Report(string text)
                {
                    var testRunSettings = new TestRunSettings();

                    if (testRunSettings.EnableReport)
                    {
                        Console.WriteLine("[" + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + ":" +
                                          DateTime.UtcNow.Second + "] " + text);
                    }
                }
            }

    public class BrowserHelpers
    {
        public static string ScreenshotDirectory { get; set; }

        public static string TakeScreenshot(Navigator browser, string screenshotDirectory, string screenshotName)
        {
            string screenshotFilepath;

            try
            {
                screenshotFilepath = string.Format("{0}/{1}.png", screenshotDirectory, screenshotName);

                 ((ITakesScreenshot)browser).GetScreenshot().SaveAsFile(screenshotFilepath, ImageFormat.Png);
            }
            catch (Exception e)
            {
                screenshotFilepath = "[FAILED] Could not take screenshot. Error:" + e.Message;
            }

            return screenshotFilepath;
        }
    }


            [Binding]
            public class Hooks : Steps
            {
                protected static Navigator Navigator;               

                #region Test hooks

                [BeforeFeature]
                public static void BeforeFeatureRun()
                {
                    DeleteScenarioArtefacts();                   
                    ReportOnHelpers.Report("Initializing Browser");
                    Navigator = new Navigator(ConfigurationManager.AppSettings.Get("Browser"));                   
                    Navigator.Maximize();
                    string screenshotDirectory = GetScenarioArtefactPath();
                    BrowserHelpers.ScreenshotDirectory = screenshotDirectory;
                    CreateDirectory(screenshotDirectory);
                }

                [BeforeScenario]
                public static void BeforeScenario()
                {
                    ReportOnHelpers.Report("Recording scenario artefacts to " + BrowserHelpers.ScreenshotDirectory);
                    //PrepareDb();
                    CloseAlerts();
                }

                [BeforeStep]
                public static void BeforeStep()
                {
                    Exception testException = ScenarioContext.Current.TestError;
                    //If we have a scenario error
                    if (testException != null)
                    {
                        Assert.Fail(testException.Message); //Skip the remaining steps
                    }
                }

                [AfterStep]
                public static void AfterStep()
                {
                    //TakeScreenshot();
                }

                [AfterScenario]
                public static void AfterScenario()
                {
                    TakeScreenshot();
                    CloseAlerts();
                    Exception testException = ScenarioContext.Current.TestError;
                    if (testException != null)
                    {                       
                        ReportOnHelpers.Report("Error Artifacts in " + BrowserHelpers.ScreenshotDirectory);
                    }
                }

                [AfterFeature]
                public static void AfterFeatureRun()
                {
                    try
                    {
                        Navigator.CloseBrowser();
                    }
                    finally
                    {
                        Navigator.KillBrowserSession();
                    }
                }

                #endregion

                #region Helper methods

            
                private static void PrepareDb()
                {
                    Database db = new Database();
                    string query = null;
                    if (ConfigurationManager.AppSettings.Get("Env").Equals("beta"))
                    {
                          query = "";
                    }
                    else if (ConfigurationManager.AppSettings.Get("Env").Equals("live"))
                    {
                         query = "";
                    }
                  
                  //  db.ExcecuteNonQuery(query);
                }

                private static string TakeScreenshot()
                {
                    string screenshotName = string.Format("{0} {1:HH-mm-ss-fff}", ScenarioContext.Current.ScenarioInfo.Title,
                        DateTime.Now);

                    return BrowserHelpers.TakeScreenshot(Navigator, BrowserHelpers.ScreenshotDirectory, screenshotName);
                }

              

                protected static string GetScenarioArtefactPath()
                {
                    return string.Format(@"C:\SeleniumArtefacts\{0}\", FeatureContext.Current.FeatureInfo.Title);
                }


                private static void DeleteScenarioArtefacts()
                {
                    string targetDirectory = GetScenarioArtefactPath();
                    if (Directory.Exists(targetDirectory))
                        Directory.Delete(targetDirectory, true);
                }

                public static void CloseAlerts()
                {
                    try
                    {
                        IAlert alert = Navigator.getDriver().SwitchTo().Alert();

                        if (alert != null)
                        {
                            ReportOnHelpers.Report(
                                string.Format("[INFO] Attempting to close alert window with text: {0}", alert.Text));
                            alert.Accept();
                            ReportOnHelpers.Report(" > Closed open alert window");
                        }
                    }
                    catch (NoAlertPresentException)
                    {
                    }
                    catch (WebDriverException)
                    {
                    }
                }

             public static string CreateDirectory(string targetDirectory)
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                if (!targetDirectory.EndsWith(@"\"))
                {
                    targetDirectory += @"\";
                }

                return targetDirectory;
            }

                #endregion
            }

           
        }
  

