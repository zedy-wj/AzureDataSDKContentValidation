using Microsoft.Playwright;
using System.Text.RegularExpressions;


namespace DataAutoFramework.Utilities
{
    public class CheckPageContent
    {
        public async Task CheckDuplicateServiceByContent(string testLink, List<string> duplicateTexts)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(testLink);

            HashSet<string> set = new HashSet<string>();

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
        }

        public async Task CheckDuplicateServiceBySider(string testLink, List<string> duplicateList)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(testLink);
            await page.WaitForSelectorAsync("li.border-top.tree-item.is-expanded");

            var parentLi = await page.QuerySelectorAsync("li.border-top.tree-item.is-expanded");

            var liElements = await parentLi.QuerySelectorAllAsync("ul.tree-group > li[aria-level='2']");

            HashSet<string> set = new HashSet<string>();

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
        }

        public async Task CheckGarbledText(string testLink, List<string> errorList)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);
            var pLocators = await page.Locator("p").AllAsync();

            foreach (var pLocator in pLocators)
            {
                var text = await pLocator.TextContentAsync();

                if (Regex.IsMatch(text, @":[\w]+(?:\s+[\w]+){0,2}:"))
                {
                    errorList.Add(text);
                }
            }

            await browser.CloseAsync();
        }

        public async Task<int> CheckIsTableEmpty(string testLink)
        {
            var count = 0;
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
                        count++;
                    }
                }
            }

            await browser.CloseAsync();
            return count;
        }
    }
}
