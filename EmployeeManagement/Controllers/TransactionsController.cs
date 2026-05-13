using EmployeeManagement.BLL.Interface;
using EmployeeManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService transactionService;

    public TransactionsController
    (
        ITransactionService transactionService
    )
    {
        this.transactionService = transactionService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create
    (
        [FromBody] TransactionCreateRequest request
    )
    {
        try
        {
            var result = await transactionService.CreateTransaction(request);

            if (result.Code == 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch
        {
            return StatusCode(500,
                new Transtatus
                {
                    Code = 2,
                    Message = "Something went wrong"
                });
        }
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetByUser
    (
        Guid userId,
        [FromQuery]
        UserTransactionQueryParameters query
    )
    {
        try
        {
            var result = await transactionService.GetTransactionsByUser(userId,query);

            return Ok(result);
        }
        catch
        {
            return StatusCode(500,
                new Transtatus
                {
                    Code = 2,
                    Message = "Something went wrong"
                });
        }
    }

    [HttpGet("net-balance")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> NetBalance()
    {
        try
        {
            var result = await transactionService.GetNetBalance();

            return Ok(result);
        }
        catch
        {
            return StatusCode(500,
                new Transtatus
                {
                    Code = 2,
                    Message = "Something went wrong"
                });
        }
    }
}