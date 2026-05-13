using EmployeeManagement.BLL.Interface;
using EmployeeManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        this.employeeService = employeeService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateRequest request)
    {
        try
        {

            var result = await employeeService.CreateEmployee(request);

            if (result.Code == 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch
        {
            return StatusCode(500, new Transtatus
            {
                Code = 2,
                Message = "Something went wrong"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update
    (
        Guid id,
        [FromBody] EmployeeUpdateRequest request
    )
    {
        try
        {
            var result = await employeeService.UpdateEmployee(id,request);

            if (result.Code == 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new Transtatus
            {
                Code = 2,
                Message = ex.Message
            });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await employeeService.DeleteEmployee(id);

            if (result.Code == 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch
        {
            return StatusCode(500, new Transtatus
            {
                Code = 2,
                Message = "Something went wrong"
            });
        }
    }



    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var employee = await employeeService.GetEmployeeById(id);

            if (employee == null)
            {
                return NotFound(new Transtatus
                {
                    Code = 1,
                    Message = "Employee not found"
                });
            }

            return Ok(employee);
        }
        catch
        {
            return StatusCode(500, new Transtatus
            {
                Code = 2,
                Message = "Something went wrong"
            });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll
    (
        [FromQuery] EmployeeQueryParameters query
    )
    {
        try
        {
            var employees = await employeeService.GetEmployees(query);

            return Ok(employees);
        }
        catch
        {
            return StatusCode(500, new Transtatus
            {
                Code = 2,
                Message = "Something went wrong"
            });
        }
    }
}