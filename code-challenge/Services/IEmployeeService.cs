using challenge.Models;
using System;

namespace challenge.Services
{
    public interface IEmployeeService
    {
        Employee GetEmployeeById(String id);
        Employee CreateEmployee(Employee employee);
        Employee ReplaceEmployee(Employee originalEmployee, Employee newEmployee);
        ReportingStructure GetReportingStructure(Employee employee);
        Compensation GetCompensationByEmployeeId(String id);
        Compensation CreateCompensation(Compensation compensation);
    }
}
