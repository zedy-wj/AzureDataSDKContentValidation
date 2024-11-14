using NUnit.Framework.Legacy;
using NUnit.Framework;
using DataAutoFramework.Utilities;

namespace DataAutoFramework.TestCases
{
    public class TestPageAnnotation
    {
        public static List<string> TestLinks { get; set; }

        static TestPageAnnotation()
        {
            TestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/overview/azure/app-configuration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/overview/azure/appconfiguration-readme?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.aio.azureappconfigurationclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-appconfiguration/azure.appconfiguration.azureappconfigurationclient?view=azure-python",
            };

        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestMissingTypeAnnotations(string testLink)
        {
            var errorList = new List<string>();
            var checkPageAnnotation = new CheckPageAnnotation();
            await checkPageAnnotation.CheckMissingTypeAnnotations(testLink, errorList);

            ClassicAssert.Zero(errorList.Count, testLink + " has  wrong type annotations of  " + string.Join(",", errorList));
        }

    }
}
