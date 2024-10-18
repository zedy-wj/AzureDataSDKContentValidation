using NUnit.Framework;
using Microsoft.Playwright;

namespace DataAutoFramework.TestCases
{
    public class TestPageTable
    {
        public static List<string> TestLinks { get; set; }
        public static List<string> ErrorLinks { get; } = new List<string>();

        static TestPageTable()
        {
            TestLinks = new List<string>
            {
                "https://learn.microsoft.com/en-us/python/api/azure-digitaltwins-core/azure.digitaltwins.core.digitaltwinsmodeldata?view=azure-python",
	            "https://learn.microsoft.com/en-us/python/api/azure-digitaltwins-core/azure.digitaltwins.core.incomingrelationship?view=azure-python",
	            "https://learn.microsoft.com/en-us/python/api/azure-iot-device/azure.iot.device.message?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-keyvault-administration/azure.keyvault.administration.aio.keyvaultbackupclient?view=azure-python",
	            "https://learn.microsoft.com/en-us/python/api/azure-iot-hub/azure.iot.hub?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-digitaltwins-core/azure.digitaltwins.core.aio.digitaltwinsclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-iot-hub/azure.iot.hub.protocol.models?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-iot-hub/azure.iot.hub.models?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-keyvault-administration/azure.keyvault.administration.apiversion?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-keyvault-administration/azure.keyvault.administration.keyvaultbackupclient?view=azure-python",
                "https://learn.microsoft.com/en-us/python/api/azure-iot-hub/azure.iot.hub.sastoken.sastoken?view=azure-python"
            };
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        public async Task TestExtraLabel(string testLink)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(testLink);

            var tableLocator = page.Locator("table");
            var isTableEmpty = await IsTableEmptyAsync(tableLocator);

            if (isTableEmpty)
            {
                ErrorLinks.Add(testLink);
            }
            
            await browser.CloseAsync();
        }

        [Test]
        [TestCaseSource(nameof(TestLinks))]
        private async Task<bool> IsTableEmptyAsync(ILocator table)
        {
            var rows = await table.Locator("tr").AllAsync();

            foreach (var row in rows)
            {
                var cells = await row.Locator("td, th").AllAsync();
                foreach (var cell in cells)
                {
                    var textContent = await cell.TextContentAsync();
                    if (string.IsNullOrWhiteSpace(textContent)) return true; 
                }
            }
            return false;
        }
    }

}
