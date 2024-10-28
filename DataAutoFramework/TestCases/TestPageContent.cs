using NUnit.Framework.Legacy;
using NUnit.Framework;
using Microsoft.Playwright;
using System.Text.Json;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace DataAutoFramework.TestCases
{
    public class TestPageContent
    {
        public static List<string> TestLinks { get; set; }
        public static List<string> SiderTestLinks { get; set; }
        public static List<string> ContentTestLinks { get; set; }

        static TestPageContent()
        {
            TestLinks = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("appsettings.json")) ?? new List<string>();

            SiderTestLinks = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("appsettings.json")) ?? new List<string>();

            ContentTestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/?view=azure-python"
            };
        }

        [Test]
        [TestCaseSource(nameof(ContentTestLinks))]
        public async Task TestDuplicateServiceByContent(string testLink)
        {

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(testLink);

            HashSet<string> set = new HashSet<string>();
            List<string> duplicateTexts = new List<string>();

            var aElements = await page.Locator("li.has-three-text-columns-list-items.is-unstyled a[data-linktype='relative-path']").AllAsync();

            foreach (var aElement in aElements)
            {
                var textContent = await aElement.InnerTextAsync();
                if (!set.Add(textContent)) 
                {
                    duplicateTexts.Add(textContent);
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(duplicateTexts.Count, testLink + " has duplicate service at " + string.Join(",", duplicateTexts));
        }

        [Test]
        [TestCaseSource(nameof(SiderTestLinks))]
        public async Task TestDuplicateServiceBySider(string testLink)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(testLink);
            await page.WaitForSelectorAsync("li.border-top.tree-item.is-expanded");

            var parentLi = await page.QuerySelectorAsync("li.border-top.tree-item.is-expanded");

            var liElements = await parentLi.QuerySelectorAllAsync("ul.tree-group > li[aria-level='2']");

            HashSet<string> set = new HashSet<string>();
            List<string> duplicateList = new List<string>();

            foreach (var element in liElements)
            {
                var text = await element.InnerTextAsync();
                if (text != "Overview")
                {
                    if (!set.Add(text))
                    {
                        duplicateList.Add(text);
                    }
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(duplicateList.Count, testLink + " has duplicate service at " + string.Join(",", duplicateList));
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
        public async Task TestGarbledText(string testLink)
        {
            var errorList = new List<string>();
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);
            var pLocators = await page.Locator("p").AllAsync();

            foreach (var pLocator in pLocators)
            {
                var text = await pLocator.TextContentAsync();

                if (Regex.IsMatch(text, @":[\w]+\s+[\w]+:") || Regex.IsMatch(text, @":[\w]+:"))
                {
                    errorList.Add(text);
                }
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

            foreach (var row in rows)
            {
                var cells = await row.Locator("td, th").AllAsync();
                foreach (var cell in cells)
                {
                    var textContent = await cell.TextContentAsync();
                    if (string.IsNullOrWhiteSpace(textContent))
                    {
                        errorList.Add(textContent);
                    } 
                }
            }
            
            await browser.CloseAsync();
            ClassicAssert.Zero(errorList.Count, testLink + " has table is empty" + string.Join(",", errorList));
        }
    }
}
