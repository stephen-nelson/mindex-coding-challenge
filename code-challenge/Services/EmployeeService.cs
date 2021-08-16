using challenge.Models;
using challenge.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee CreateEmployee(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.AddEmployee(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetEmployeeById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetEmployeeById(id);
            }

            return null;
        }

        public Employee ReplaceEmployee(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.RemoveEmployee(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.AddEmployee(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(Employee employee)
        {
            ReportingStructure structure;

            if (employee == null)
            {
                structure = null;
            }
            else
            {
                structure = new ReportingStructure()
                {
                    Employee = employee,
                    NumberOfReports = GetNumberOfReports(employee)
                };
            }

            return structure;
        }

        public Compensation GetCompensationByEmployeeId(String id)
        {
            Compensation compensation = null;

            if (!String.IsNullOrEmpty(id))
            {
                compensation = _employeeRepository.GetCompensationByEmployeeId(id);
            }
            else
            {
                compensation = null;
            }

            return compensation;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {
            if (compensation != null)
            {
                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        /// <summary>
        /// Get the number of total reports for the given <see cref="Employee"/>.  The number of reports is determined
        /// to be the number of <see cref="Employee.DirectReports"/> for an employee and all of their direct reports.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> to query</param>
        /// <returns>The total number of reports for the given <see cref="Employee"/></returns>
        private static int GetNumberOfReports(Employee employee)
        {
            int totalReports;

            if (employee.DirectReports == null)
            {
                totalReports = 0;
            }
            else
            {
                // Get the number of DIRECT reports
                totalReports = employee.DirectReports.Count;

                // The number of reports includes the DIRECT reporters of this employee's reporters, etc.
                foreach (Employee reporter in employee.DirectReports)
                {
                    totalReports += GetNumberOfReports(reporter);
                }
            }

            return totalReports;
        }
    }
}
