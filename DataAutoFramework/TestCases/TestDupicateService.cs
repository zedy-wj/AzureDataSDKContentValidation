using NUnit.Framework;
using DataAutoFramework.Helper;
using NUnit.Framework.Legacy;
using System.Text.Json;
using Microsoft.Playwright;
using HtmlAgilityPack;

namespace DataAutoFramework.TestCases
{
    public class TestDupicateService
    {
        public static List<string> ContentTestLinks { get; set; }
        public static List<string> SiderTestLinks { get; set; }

        static TestDupicateService()
        {
            ContentTestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/?view=azure-python"
            };
            SiderTestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/advisor?view=azure-python"
            };
        }

        [Test]
        [TestCaseSource(nameof(ContentTestLinks))]
        public async Task TestDuplicateLinksByContent(string testLink)
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
                if (!set.Add(textContent)) //存在重复元素
                {
                    duplicateTexts.Add(textContent);
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(duplicateTexts.Count, testLink + " has duplicate link at " + string.Join(",", duplicateTexts));

        }

        [Test]
        [TestCaseSource(nameof(SiderTestLinks))]
        public async Task TestDuplicateLinksBySider(string testLink)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            string url = "https://learn.microsoft.com/en-us/python/api/overview/azure/advisor?view=azure-python";

            await page.GotoAsync(url);

            var spanElements = await page.Locator("span.tree-expander").AllAsync();

            HashSet<string> set = new HashSet<string>();
            List<string> duplicateList = new List<string>();

            foreach (var element in spanElements)
            {
                var text = await element.InnerTextAsync();
                if (!set.Add(text))
                {
                    duplicateList.Add(text);
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(duplicateList.Count, testLink + " has duplicate link at " + string.Join(",", duplicateList));
        }
    }
}
