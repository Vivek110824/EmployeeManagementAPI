using Azure.Core;
using EmployeeManagement.BLL.Interface;
using EmployeeManagement.DAL;
using EmployeeManagement.Model;

namespace EmployeeManagement.BLL;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeRepository employeeRepository= new EmployeeRepository();

    public async Task<Transtatus> CreateEmployee(EmployeeCreateRequest request)
    {
        ValidateCreateOrUpdateRequest(request.Name, request.Email);
        return await employeeRepository.AddEmployee(request);
    }

    public async Task<Employee> GetEmployeeById(Guid id) => await employeeRepository.GetEmployeeById(id);

    public async Task<List<Employee>> GetEmployees(EmployeeQueryParameters query)
    {
        query.PageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
        query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        return await employeeRepository.GetEmployees(query);
    }

    public async Task<Transtatus> UpdateEmployee(Guid id, EmployeeUpdateRequest request)
    {
        ValidateCreateOrUpdateRequest(request.Name, request.Email);
        return await employeeRepository.UpdateEmployee(id,request);
    }

    public async Task<Transtatus> DeleteEmployee(Guid id) => await employeeRepository.DeleteEmployee(id);

    private static void ValidateCreateOrUpdateRequest(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.");
        }
    }
}
