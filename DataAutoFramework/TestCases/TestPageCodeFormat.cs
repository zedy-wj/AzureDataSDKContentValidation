using NUnit.Framework.Legacy;
using NUnit.Framework;
using DataAutoFramework.Utilities;

namespace DataAutoFramework.TestCases
{
    public class TestPageCodeFormat
    {
        public static List<string> TestLinks { get; set; }

        static TestPageCodeFormat()
        {
            TestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/overview/azure/appconfiguration-readme?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-confidentialledger/azure.confidentialledger.aio.confidentialledgerclient"
            };
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestIncorrectFormatJson(string testLink)
        {
            // Error list
            List<string> errorList = new List<string>();
            var checkPageCodeFormat = new CheckPageCodeFormat();
            await checkPageCodeFormat.CheckIncorrectFormatJson(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has testLink has improperly displayed code comments in JSON snippet :  " + string.Join(",", errorList));
        }
        
    }
}
