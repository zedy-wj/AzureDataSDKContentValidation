using NUnit.Framework.Legacy;
using NUnit.Framework;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace DataAutoFramework.TestCases
{
    public class TestPageLabel
    {
        public static List<string> TestLinks { get; set; }

        static TestPageLabel()
        {
            TestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/overview/azure/appconfiguration-readme?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-mgmt-batch/azure.mgmt.batch.models.azureresource"
            };
        }


        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestExtraLabel(string testLink)
        {
            var errorList = new List<string>();
            var labelList = new List<string> {
                "<br",
                "<h1",
                "<h2",
                "<h3",
                "<h4",
                "<h5",
                "<h6",
                "<em",
                "<a",
                "<span",
                "<div",
                "<ul",
                "<ol",
                "<li",
                "<table",
                "<tr",
                "<td",
                "<th",
                "<img",
                "<code",
                "<xref",
                "&amp;",
                "&lt",
                "&gt",
                "&quot",
                "&apos"
            };

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);
            var text = await page.Locator("html").InnerTextAsync();

            foreach (var label in labelList)
            {

                if (text.Contains(label))
                {
                    errorList.Add(label);
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(errorList.Count, testLink + " has extra label : \n" + string.Join("\n", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestUnnecessarySymbols(string testLink)
        {
            var errorList = new List<string>();
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);
            var paragraphs = await page.Locator("p").AllInnerTextsAsync();
            var tableContents = new List<string>();
            var tableCount = await page.Locator("table").CountAsync();
            var codeBlocks = await page.Locator("code").AllInnerTextsAsync();

            if (paragraphs != null)
            {
                foreach (var paragraph in paragraphs)
                {
                    var paragraphMatches = Regex.Matches(paragraph, @"[\[\]<>]|/{3}");

                    foreach (Match match in paragraphMatches)
                    {
                        errorList.Add($"The paragraph contains unnecessary symbol:{ paragraph}");
                    }
                }
            }

            for (int i = 0; i < tableCount; i++)
            {
                var tableContent = await page.Locator("table").Nth(i).InnerHTMLAsync();
                tableContents.Add(tableContent);
            }

            foreach (var tableContent in tableContents)
            {
                var tagMatches = Regex.Matches(tableContent, @"<\/\w+>\s*&gt;\s*<\/\w+>|~");
                foreach (Match match in tagMatches)
                {
                    errorList.Add($"table contains unnecessary symbol: {match.Value}");
                }
            }

            if (codeBlocks != null)
            {
                foreach (var codeBlock in codeBlocks)
                {
                    var tildeMatches = Regex.Matches(codeBlock, @"~");
                    foreach (Match match in tildeMatches)
                    {
                        errorList.Add($"Code block contains unnecessary symbol: {match.Value}");
                    }
                }
            }

            ClassicAssert.Zero(errorList.Count, testLink + " has unnecessary symbols:\n" + string.Join("\n", errorList));
        }
    }
}