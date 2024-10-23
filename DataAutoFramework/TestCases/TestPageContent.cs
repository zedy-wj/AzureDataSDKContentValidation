using DataAutoFramework.Helper;
using HtmlAgilityPack;
using NUnit.Framework.Legacy;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace DataAutoFramework.TestCases
{
    public class TestPageText
    {
        public static List<string> TestLinks { get; set; }

        static TestPageText()
        {
            TestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/overview/azure/appconfiguration-readme?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.azureappconfigurationclient?view=azure-python"
            };
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestExtraChar(string testLink)
        {
            var errorList = new List<string>();
            var web = new HtmlWeb();
            var doc = web.Load(testLink);
            foreach (var item in doc.DocumentNode.SelectNodes("//p"))
            {
                var text = item.InnerText.Trim();
                if (text.StartsWith("â\u0080\u0099") || text.EndsWith("â\u0080\u0099") || text.StartsWith('~') || text.EndsWith('~'))
                {
                    errorList.Add(text);
                }
            }

            ClassicAssert.Zero(errorList.Count, testLink + " has extra charactor of '-' and `~` at " + string.Join(",", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestCodeBlock(string testLink)
        {
            var errorList = new List<string>();
            var web = new HtmlWeb();
            var doc = web.Load(testLink);
            foreach (var item in doc.DocumentNode.SelectNodes("//div[contains(@class, 'notranslate')]"))
            {
                var text = item.InnerText;
                text = text.TrimEnd('\n');
                var newCode = await ValidationHelper.ParsePythonCode(text);
                Console.WriteLine(text);
            }

            ClassicAssert.Zero(errorList.Count, testLink + " has wrong format" + string.Join(",", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public void TestBlankNode(string testLink)
        {
            var blankNodeCount = 0;
            var web = new HtmlWeb();
            var doc = web.Load(testLink);
            HtmlNodeCollection items = doc.DocumentNode.SelectNodes("//div[contains(@class, 'admonition seealso')]/ul[contains(@class, 'simple')]/li");
            if(items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    blankNodeCount += String.IsNullOrEmpty(item.InnerText) ? 1 : 0;
                }
            }
            
            ClassicAssert.Zero(blankNodeCount);
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public void TestGarbledText(string testLink)
        {
            var errorList = new List<string>();
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);
            var pLocators = await page.Locator("p").AllAsync();
            
            string pattern_1 = @":[\w]+\s+[\w]+:";
            string pattern_2 = @":[\w]+:";
            bool containsSpecificText = false;

            foreach (var pLocator in pLocators)
            {
                var text = await pLocator.TextContentAsync();

                if (Regex.IsMatch(text, pattern_1) || Regex.IsMatch(text, pattern_2))
                {
                    containsSpecificText = true;
                    break;
                }
            }
            if (containsSpecificText)
            {
                errorList.Add(testLink);
            }
            
            await browser.CloseAsync();
            ClassicAssert.Zero(errorList.Count, testLink + " has garbled text" + string.Join(",", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestIsTableEmpty(string testLink)
        {
            var errorList = new List<string>();
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);

            var tableLocator = page.Locator("table");
            //var tableLocator = page.Locator("table:not([aria-label*='Package'])");
            var rows = await tableLocator.Locator("tr").AllAsync();
            bool IsTableEmpty = false;

            foreach (var row in rows)
            {
                var cells = await row.Locator("td, th").AllAsync();
                foreach (var cell in cells)
                {
                    var textContent = await cell.TextContentAsync();
                    if (string.IsNullOrWhiteSpace(textContent))
                    {
                        IsTableEmpty = true;
                        break;
                    } 
                }
            }
            if(IsTableEmpty)
            {
                errorList.Add(testLink);
            }
            
            await browser.CloseAsync();
            ClassicAssert.Zero(errorList.Count, testLink + " has table is empty" + string.Join(",", errorList));
        }
    }
}
