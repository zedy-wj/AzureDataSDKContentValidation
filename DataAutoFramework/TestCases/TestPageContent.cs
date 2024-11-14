using NUnit.Framework.Legacy;
using NUnit.Framework;
using System.Text.Json;
using DataAutoFramework.Utilities;

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

            List<string> duplicateTexts = new List<string>();
            var checkPageContent = new CheckPageContent();
            await checkPageContent.CheckDuplicateServiceByContent(testLink, duplicateTexts);

            ClassicAssert.Zero(duplicateTexts.Count, testLink + " has duplicate service at " + string.Join(",", duplicateTexts));
        }

        [Test]
        [TestCaseSource(nameof(SiderTestLinks))]
        public async Task TestDuplicateServiceBySider(string testLink)
        {
            
            List<string> duplicateList = new List<string>();
            var checkPageContent = new CheckPageContent();
            await checkPageContent.CheckDuplicateServiceBySider(testLink, duplicateList);

            ClassicAssert.Zero(duplicateList.Count, testLink + " has duplicate service at " + string.Join(",", duplicateList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestGarbledText(string testLink)
        {
            var errorList = new List<string>();
            var checkPageContent = new CheckPageContent();
            await checkPageContent.CheckGarbledText(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has garbled text" + string.Join(",", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestIsTableEmpty(string testLink)
        {
            var checkPageTable = new CheckPageContent();
            var count = await checkPageTable.CheckIsTableEmpty(testLink);

            ClassicAssert.Zero(count, testLink + " has table is empty.");
        }
    }
}
