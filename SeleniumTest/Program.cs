using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Set up some resources.
        var driver = new EdgeDriver();
        var actions = new Actions(driver);
        var wait = new WebDriverWait(driver, new TimeSpan(0, 1, 0));
        IJavaScriptExecutor js = driver;

        // Get username and password data from local file.
        // TODO: Add validations with helpful error messages.
        var upDatPath = Path.Combine(Helper.DocPath, "sel_test_input_up.txt");
        var upDat = File.ReadAllLines(upDatPath);
        var usernameStr = upDat[0];
        var passwordStr = upDat[1];

        // Open the browser and navigate to the LinkedIn login page.
        driver.Manage().Window.Maximize();
        driver
            .Navigate()
            .GoToUrl(
                "https://www.linkedin.com/login?fromSignIn=true&amp;trk=guest_homepage-basic_nav-header-signin"
            );

        // Enter username.
        var usernameInput = Helper.GetElementWait(driver, wait, By.Id("username"));
        usernameInput.SendKeys(usernameStr);

        // Enter password.
        var passwordInput = Helper.GetElementWait(driver, wait, By.Id("password"));
        passwordInput.SendKeys(passwordStr);

        // Get the "Keep me logged in" checkbox.
        try
        {
            var rememberCheckbox = Helper.GetElementWait(driver, wait, By.Id(Helper.RememberMeId));
            // Click the checkbox.
            // TODO: Read the top answer in the link below, maybe it explains why the WebDriver click action doesn't work here.
            // https://stackoverflow.com/questions/34562061/webdriver-click-vs-javascript-click
            // Maybe related: the rememberCheckbox never becomes "enabled" or "displayed".
            js.ExecuteScript("arguments[0].click();", rememberCheckbox);
        }
        catch (InvalidOperationException) { }

        // Submit form.
        passwordInput.SendKeys(OpenQA.Selenium.Keys.Return);

        // Go to Jobs.
        var jobsButton = Helper.GetElementWait(driver, wait, By.LinkText("Jobs"));
        actions.Click(jobsButton).Perform();

        // Search for software developer jobs.
        var searchBox = Helper.GetElementWait(
            driver,
            wait,
            By.ClassName("jobs-search-box__text-input")
        );
        searchBox.SendKeys("software developer");
        searchBox.SendKeys(OpenQA.Selenium.Keys.Return);

        // Get/click the experience filter button.
        var experienceBtn = Helper.GetElementWait(driver, wait, By.Id("searchFilter_experience"));
        actions.Click(experienceBtn).Perform();

        // Get/click the "entry level" checkbox.
        var entryLevelCheckBox = Helper.GetElementWait(driver, wait, By.Id("experience-2"));
        actions.Click(entryLevelCheckBox).Perform();

        // Click "show results".
        Helper.SubmitFilter(driver, actions);

        var showAllFiltersBtn = Helper.GetElementWait(
            driver,
            wait,
            By.CssSelector("button[aria-label*='Show all filters.']")
        );
        actions.Click(showAllFiltersBtn).Perform();

        // TODO: This works, but see if you can achieve the same result via a better route.
        Thread.Sleep(2000);
        var sortByDateBtn = Helper.GetElementWait(driver, wait, By.Id("advanced-filter-sortBy-DD"));
        //actions.Click(sortByDateBtn).Perform();
        js.ExecuteScript("arguments[0].click();", sortByDateBtn);

        //Thread.Sleep(2000);

        Helper.SubmitFilter(driver, actions);

        List<string> urls = [];

        var i = 0;

        while (i < 10)
        {
            var shareBtn = Helper.GetVisibleElementWait(
                driver,
                wait,
                By.ClassName("social-share__dropdown-trigger")
            );
            js.ExecuteScript("arguments[0].click();", shareBtn);

            //var shareDiv = Helper.GetVisibleElementWait(
            //    driver,
            //    wait,
            //    By.ClassName("social-share__content")
            //);

            Win32.SetCursorPos(1059, 485);
            var watch = Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < 1000)
            {
                Debug.WriteLine("waiting BEFORE copy link click");
            }
            watch.Stop();
            Win32.DoMouseClick(1059, 485);

            while (!Clipboard.ContainsText())
            {
                Debug.WriteLine("Waiting AFTER copy link click");
                Thread.Sleep(10);
            }

            var url = Clipboard.GetText();
            Helper.DebugPrintTxt(url, "2", i > 0);
            urls.Add(url);
            Thread.Sleep(2000);

            Win32.SetCursorPos(608, 704);
            Win32.DoMouseClick(608, 704);
            Win32.SetCursorPos(608, 704);
            Win32.DoMouseClick(608, 704);

            Thread.Sleep(2000);
            Win32.SetCursorPos(484, 368);
            Win32.DoMouseClick(484, 368);
            Thread.Sleep(3000);
            //var query = driver.FindElement(By.)
            // driver.Quit();
            i++;
        }

        var uniqueUrls = urls.Distinct().ToList();
        Helper.DebugPrintTxt("yes", "1");
        uniqueUrls.ForEach(u => Helper.DebugPrintTxt(u, "1", true));
    }
}
