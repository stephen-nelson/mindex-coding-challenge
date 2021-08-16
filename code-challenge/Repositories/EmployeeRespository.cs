using System;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetEmployeeById(string id)
        {
            // The DirectReports must be eager loaded via Include().  It will be a null list otherwise.
            Employee employee = _employeeContext.Employees.Include(e => e.DirectReports).Where(e => e.EmployeeId == id).SingleOrDefault();

            // The DirectReports of the requested employee must also be eager loaded to get their DirectReports.  Eager
            // loading in this way is slightly inefficient since it's a N+1 query.  One way to improve performance
            // would be to restructure the data of DirectReports into its own table so that it can be retrieved with 1 query.
            if (employee != null && employee.DirectReports != null)
            {
                foreach (Employee sub in employee.DirectReports)
                {
                    GetEmployeeById(sub.EmployeeId);
                }
            }

            return employee;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee RemoveEmployee(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }

        public Compensation GetCompensationByEmployeeId(String id)
        {
            Compensation compensation = _employeeContext.Compensation.SingleOrDefault(c => c.Employee == id);
            return compensation;
        }
    }
}
