using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmployeeManagement.Model;

namespace EmployeeManagement.BLL.Interface;

public interface IEmployeeService
{
    Task<Transtatus> CreateEmployee(EmployeeCreateRequest request);
    Task<Employee?> GetEmployeeById(Guid id);
    Task<List<Employee>> GetEmployees(EmployeeQueryParameters query);
    Task<Transtatus?> UpdateEmployee(Guid id,EmployeeUpdateRequest request);
    Task<Transtatus> DeleteEmployee(Guid id);
}
