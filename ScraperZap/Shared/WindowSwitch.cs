using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq.Expressions;

namespace ScraperZap.Shared
{
    internal class WindowSwitch
    {
        public void SwitchToWindow(Expression<Func<IWebDriver, bool>> predicateExp, ChromeDriver driver)
        {
            var predicate = predicateExp.Compile();
            foreach (var handle in driver.WindowHandles)
            {
                driver.SwitchTo().Window(handle);
                if (predicate(driver))
                {
                    return;
                }
            }

            throw new ArgumentException(string.Format("Unable to find window with condition: '{0}'", predicateExp.Body));
        }
    }
}
