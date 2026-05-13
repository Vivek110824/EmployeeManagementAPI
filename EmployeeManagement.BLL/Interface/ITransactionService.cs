using EmployeeManagement.Model;

namespace EmployeeManagement.BLL.Interface;

public interface ITransactionService
{

    Task<Transtatus>CreateTransaction(TransactionCreateRequest request);
    Task<List<EmployeeTransaction>>GetTransactionsByUser(Guid userId,UserTransactionQueryParameters query);
    Task<List<NetBalanceResponse>>GetNetBalance();
}