
using OpenQA.Selenium.Firefox;

namespace UITests.SiteModel.Session
{
    public static class CustomFirefoxProfile 
    {
        public static FirefoxProfile Profile() {
            const string FolderName = "temp";
            var profile = new FirefoxProfile {EnableNativeEvents = true};
            profile.SetPreference("browser.download.folderList", 2);
            // profile.SetPreference("browser.download.manager.showWhenStarting", false);
            profile.SetPreference("browser.download.dir", FolderName);
            profile.SetPreference("browser.download.downloadDir", FolderName);
            profile.SetPreference("browser.download.defaultFolder", FolderName);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/csv");
            return profile;
        }
    }
}