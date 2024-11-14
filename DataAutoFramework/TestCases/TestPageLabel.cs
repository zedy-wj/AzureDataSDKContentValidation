using NUnit.Framework.Legacy;
using NUnit.Framework;
using DataAutoFramework.Utilities;

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
            var checkPageLabel = new CheckPageLabel();
            await checkPageLabel.CheckExtraLabel(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has extra label of  " + string.Join(",", errorList));
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestUnnecessarySymbols(string testLink)
        {
            var errorList = new List<string>();
            var checkPageLabel = new CheckPageLabel();
            await checkPageLabel.CheckUnnecessarySymbols(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has unnecessary symbols:\n" + string.Join("\n", errorList));
        }
    }
}