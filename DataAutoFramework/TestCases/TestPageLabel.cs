using NUnit.Framework.Legacy;
using NUnit.Framework;
using Microsoft.Playwright;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DataAutoFramework.TestCases
{
    public class TestPageLabel
    {
        public static List<string> TestLinks { get; set; }
        public static Dictionary<string, string> SpecialLinks { get; set; }

        static TestPageLabel()
        {
            TestLinks = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("appsettings.json")) ?? new List<string>();

            SpecialLinks = new Dictionary<string, string>();

            SpecialLinks.Add("Read in English", "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python");
            SpecialLinks.Add("our contributor guide", "https://github.com/Azure/azure-sdk-for-python/blob/main/CONTRIBUTING.md");
            // SpecialLinks.Add("English (United States)", "/en-us/locale?target=https%3A%2F%2Flearn.microsoft.com%2Fen-us%2Fpython%2Fapi%2Foverview%2Fazure%2Fapp-configuration%3Fview%3Dazure-python");
            SpecialLinks.Add("Privacy", "https://go.microsoft.com/fwlink/?LinkId=521839");
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

            ClassicAssert.Zero(errorList.Count, testLink + " has extra label of  " + string.Join(",", errorList));
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
            
            if (paragraphs != null)
            {
                foreach (var paragraph in paragraphs)
                {
                    var paragraphMatches = Regex.Matches(paragraph, @"[\[\]<>]|/{3}");

                    foreach (Match match in paragraphMatches)
                    {
                        errorList.Add(paragraph);
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
                    errorList.Add(match.Value);
                }
            }            

            ClassicAssert.Zero(errorList.Count, testLink + " has unnecessary symbols:\n" + string.Join("\n", errorList));
        }
    }
}
