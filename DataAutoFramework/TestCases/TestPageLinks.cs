using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Text.Json;
using DataAutoFramework.Utilities;

namespace DataAutoFramework.TestCases
{
    public class TestPageLinks
    {
        public static List<string> TestLinks { get; set; }

        public static Dictionary<string, string> SpecialLinks { get; set; }

        static TestPageLinks()
        {
            //TestLinks = new List<string>
            //{
            //    "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python",
            //    "https://learn.microsoft.com/en-us/python/api/overview/azure/appconfiguration-readme?view=azure-python",
            //    "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration?view=azure-python",
            //    "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio?view=azure-python",
            //    "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio.azureappconfigurationclient?view=azure-python",
            //    "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.azureappconfigurationclient?view=azure-python"
            //};

            TestLinks = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("appsettings.json")) ?? new List<string>();

            SpecialLinks = new Dictionary<string, string>();

            SpecialLinks.Add("Read in English", "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python");
            SpecialLinks.Add("our contributor guide", "https://github.com/Azure/azure-sdk-for-python/blob/main/CONTRIBUTING.md");
            // SpecialLinks.Add("English (United States)", "/en-us/locale?target=https%3A%2F%2Flearn.microsoft.com%2Fen-us%2Fpython%2Fapi%2Foverview%2Fazure%2Fapp-configuration%3Fview%3Dazure-python");
            SpecialLinks.Add("Privacy", "https://go.microsoft.com/fwlink/?LinkId=521839");
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestCrossLinks(string testLink)
        {
            var checkPageLinks = new CheckPageLinks();
            var (failCount, failMsg) = await checkPageLinks.CheckCrossLinks(testLink, SpecialLinks);

            ClassicAssert.Zero(failCount, failMsg);
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestBrokenLinks(string testLink)
        {
            var errorList = new List<string>();
            var checkBrokenLinks = new CheckPageLinks();
            await checkBrokenLinks.CheckBrokenLinks(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has error link at " + string.Join(",", errorList));
        }
    }
}
