using EmployeeManagement.BLL.Interface;
using EmployeeManagement.DAL;
using EmployeeManagement.Model;

namespace EmployeeManagement.BLL;

public class TransactionService : ITransactionService
{
    private readonly TransactionRepository transactionRepository = new TransactionRepository();


    public async Task<Transtatus>CreateTransaction(TransactionCreateRequest request)
    {
        Transtatus transtatus = new Transtatus();

        try
        {
            if (request.Amount <= 0)
            {
                transtatus.Code = 1;
                transtatus.Message =
                    "Amount must be greater than 0";

                return transtatus;
            }

            if
            (
                request.Type != TransactionType.Credit
                &&
                request.Type != TransactionType.Debit
            )
            {
                transtatus.Code = 1;
                transtatus.Message =
                    "Invalid transaction type";

                return transtatus;
            }

            EmployeeTransaction model = new EmployeeTransaction{
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Type = request.Type
                };

            transtatus = await transactionRepository.AddTransaction(model);
        }
        catch (Exception ex)
        {
            transtatus.Code = 2;
            transtatus.Message = ex.Message;
        }

        return transtatus;
    }


    public async Task<List<EmployeeTransaction>>GetTransactionsByUser(Guid userId,UserTransactionQueryParameters query)
    {
        try
        {
            query.PageNumber =query.PageNumber <= 0? 1: query.PageNumber;
            query.PageSize =query.PageSize <= 0? 10: query.PageSize;

            return await transactionRepository.GetTransactionsByUser(userId,query.PageNumber,query.PageSize);
        }
        catch
        {
            throw;
        }
    }


    public async Task<List<NetBalanceResponse>> GetNetBalance()
    {
        try
        {
            return await transactionRepository.GetNetBalance();
        }
        catch
        {
            throw;
        }
    }
}