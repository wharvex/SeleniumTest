using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using InvalidOperationException = System.InvalidOperationException;

namespace SeleniumTest;

public class Helper
{
    public static string RememberMeId = "rememberMeOptIn-checkbox";
    public static string DocPath = Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData
    );

    public static IWebElement? GetElementOrNull(IWebDriver driver, By by)
    {
        IWebElement e;
        try
        {
            e = driver.FindElement(by);
        }
        catch (NoSuchElementException)
        {
            return null;
        }

        return e;
    }

    public static IWebElement? GetVisibleElementOrNull(IWebDriver driver, By by)
    {
        var e = GetElementOrNull(driver, by);
        if (e == null)
            return null;

        return IsElementVisible(e) ? e : null;
    }

    public static IWebElement GetElementWait(IWebDriver driver, WebDriverWait wait, By by)
    {
        return wait.Until(d => GetElementOrNull(d, by))
            ?? throw new InvalidOperationException(
                "Waited for element, but it didn't load in time."
            );
    }

    public static IWebElement GetVisibleElementWait(IWebDriver driver, WebDriverWait wait, By by)
    {
        return wait.Until(d => GetVisibleElementOrNull(d, by))
            ?? throw new InvalidOperationException(
                "Waited for element, but it didn't load in time."
            );
    }

    public static bool IsElementVisible(IWebElement element)
    {
        return element is { Displayed: true, Enabled: true };
    }

    public static void DebugPrintTxt(string output, string suffix, bool append = false)
    {
        using var outputFile = new StreamWriter(
            Path.Combine(DocPath, "sel_test_output_" + suffix + ".txt"),
            append
        );

        outputFile.WriteLine(output);
    }

    public static void SubmitFilter(IWebDriver driver, Actions actions)
    {
        var watch = Stopwatch.StartNew();
        var worked = false;
        while (watch.ElapsedMilliseconds < 10000)
        {
            try
            {
                actions
                    .Click(
                        driver
                            .FindElements(
                                By.CssSelector("button[aria-label*='Apply current filter']")
                            )
                            .FirstOrDefault(e => e.Displayed)
                    )
                    .Perform();
                worked = true;
            }
            catch (Exception) { }
        }

        if (!worked)
            throw new InvalidOperationException("blah");
    }
}
