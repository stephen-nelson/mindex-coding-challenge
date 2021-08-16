using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(String id);
        Employee AddEmployee(Employee employee);
        Employee RemoveEmployee(Employee employee);
        Compensation AddCompensation(Compensation compensation);
        Compensation GetCompensationByEmployeeId(String id);
        Task SaveAsync();
    }
}