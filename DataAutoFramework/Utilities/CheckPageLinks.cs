using DataAutoFramework.Helper;
using Microsoft.Playwright;

namespace DataAutoFramework.Utilities
{
    public class CheckPageLinks
    {
        public async Task<(int failCount, string failMsg)> CheckCrossLinks(string testLink, Dictionary<string, string> SpecialLinks)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);

            var hrefs = page.Locator("#main-column").Locator("a");
            var failCount = 0;
            var failMsg = "";

            for (var index = 0; index < await hrefs.CountAsync(); index++)
            {
                var href = hrefs.Nth(index);
                var attri = href.GetAttributeAsync("href").Result;
                var text = href.InnerTextAsync().Result;

                if (String.IsNullOrEmpty(text.Trim()) || text.Trim() == "English (United States)")
                {
                    continue;
                }

                if (SpecialLinks.ContainsKey(text.Trim()) && SpecialLinks[text.Trim()] == attri)
                {
                    continue;
                }

                var subContent = text.ToLower().Replace("-", " ").Replace("@", " ").Split(" ");
                var flag = false;

                foreach (string s in subContent)
                {
                    if (attri?.ToLower().Replace(".", "").Contains(s) ?? false)
                    {
                        flag = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!flag)
                {
                    failCount++;
                    failMsg = failMsg + text.Trim() + ": " + attri + "\n";
                }
                
            }
            await browser.CloseAsync();
            return (failCount, failMsg);
        }

        public async Task CheckBrokenLinks(string testLink, List<string> errorList)
        {
            string baseUri = "https://learn.microsoft.com/";

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(testLink);

            var links = await page.Locator("a").AllAsync();

            foreach (var link in links)
            {
                var href = await link.GetAttributeAsync("href");
                if (!string.IsNullOrEmpty(href) && !href.StartsWith("mailto"))
                {
                    if (href.StartsWith("#"))
                    {
                        href = testLink + href;
                    }
                    else if (!href.StartsWith("#") && !href.StartsWith("http") && !href.StartsWith("https"))
                    {
                        href = baseUri + href;
                    }
                    if (!await ValidationHelper.CheckIfPageExist(href))
                    {
                        errorList.Add(href);
                    }
                }
            }

            await browser.CloseAsync();
        }
    }
}
