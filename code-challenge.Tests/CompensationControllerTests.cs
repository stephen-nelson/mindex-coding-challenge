using challenge.Models;
using code_challenge.Tests.Integration.Extensions;
using code_challenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            Compensation compensation = new Compensation()
            {
                Employee = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 123456,
                EffectiveDate = new DateTime(2014, 10, 6)
            };

            string requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            Task<HttpResponseMessage> postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            HttpResponseMessage response = postRequestTask.Result;
            Compensation newCompensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(compensation.Employee, newCompensation.Employee);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Returns_NotFound()
        {
            // Arrange
            Compensation compensation = new Compensation()
            {
                Employee = "Invalid_Id",
                Salary = 424242,
                EffectiveDate = new DateTime(1990, 1, 1)
            };

            string requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            Task<HttpResponseMessage> postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            HttpResponseMessage response = postRequestTask.Result;
            Compensation newCompensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsNull(newCompensation);
        }

        /// <summary>
        /// This test assumes that CreateCompensation_Returns_Created already ran and passed.
        /// It is verifying that the the Compensation entry created therein was saved correctly.
        /// </summary>
        [TestMethod]
        public void ReadCompensation_Returns_Ok()
        {
            // Arrange
            const string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            const int expectedSalary = 123456;
            DateTime expectedStartDate = new DateTime(2014, 10, 6);

            // Execute
            Task<HttpResponseMessage> getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            HttpResponseMessage response = getRequestTask.Result;
            Compensation compensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(employeeId, compensation.Employee);
            Assert.AreEqual(expectedSalary, compensation.Salary);
            Assert.AreEqual(expectedStartDate, compensation.EffectiveDate);
        }

        [TestMethod]
        public void ReadCompensation_Returns_NotFound()
        {
            // Arrange
            const string employeeId = "Invalid_Id";

            // Execute
            Task<HttpResponseMessage> getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            HttpResponseMessage response = getRequestTask.Result;
            Compensation compensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsNull(compensation);
        }
    }
}
