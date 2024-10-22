﻿using NUnit.Framework.Legacy;
using NUnit.Framework;
using Microsoft.Playwright;
using System.Text.Json;

namespace DataAutoFramework.TestCases
{
    public class TestPageContent
    {
        public static List<string> TestLinks { get; set; }
        public static Dictionary<string, string> SpecialLinks { get; set; }
        public static List<string> ContentTestLinks { get; set; }

        static TestPageContent()
        {
            TestLinks = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("appsettings.json")) ?? new List<string>();

            SpecialLinks = new Dictionary<string, string>();

            SpecialLinks.Add("Read in English", "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python");
            SpecialLinks.Add("our contributor guide", "https://github.com/Azure/azure-sdk-for-python/blob/main/CONTRIBUTING.md");
            // SpecialLinks.Add("English (United States)", "/en-us/locale?target=https%3A%2F%2Flearn.microsoft.com%2Fen-us%2Fpython%2Fapi%2Foverview%2Fazure%2Fapp-configuration%3Fview%3Dazure-python");
            SpecialLinks.Add("Privacy", "https://go.microsoft.com/fwlink/?LinkId=521839");

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
                if (!set.Add(textContent)) //存在重复元素
                {
                    duplicateTexts.Add(textContent);
                }
            }

            await browser.CloseAsync();

            ClassicAssert.Zero(duplicateTexts.Count, testLink + " has duplicate link at " + string.Join(",", duplicateTexts));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
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

            ClassicAssert.Zero(duplicateList.Count, testLink + " has duplicate link at " + string.Join(",", duplicateList));
        }
    }
}