using challenge.Models;
using code_challenge.Tests.Integration.Extensions;
using code_challenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [TestInitialize]
        public void InitializeTest()
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetEmployeeReportingStructure1_Returns_Ok()
        {
            // Arrange
            const string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            const string expectedFirstName = "John";
            const string expectedLastName = "Lennon";
            const int expectedReports = 4;

            // Execute
            Task<HttpResponseMessage> getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reportingStructure");
            HttpResponseMessage response = getRequestTask.Result;
            ReportingStructure structure = response.DeserializeContent<ReportingStructure>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedReports, structure.NumberOfReports);
        }

        [TestMethod]
        public void GetEmployeeReportingStructure2_Returns_Ok()
        {
            // Arrange
            const string employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
            const string expectedFirstName = "Paul";
            const string expectedLastName = "McCartney";
            const int expectedReports = 0;

            // Execute
            Task<HttpResponseMessage> getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reportingStructure");
            HttpResponseMessage response = getRequestTask.Result;
            ReportingStructure structure = response.DeserializeContent<ReportingStructure>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedReports, structure.NumberOfReports);
        }

        [TestMethod]
        public void GetEmployeeReportingStructure3_Returns_Ok()
        {
            // Arrange
            const string employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";
            const string expectedFirstName = "Ringo";
            const string expectedLastName = "Starr";
            const int expectedReports = 2;

            // Execute
            Task<HttpResponseMessage> getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reportingStructure");
            HttpResponseMessage response = getRequestTask.Result;
            ReportingStructure structure = response.DeserializeContent<ReportingStructure>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedReports, structure.NumberOfReports);
        }
    }
}
