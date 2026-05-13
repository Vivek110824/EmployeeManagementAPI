using EmployeeManagement.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.DAL;

public class TransactionRepository
{
    public async Task<Transtatus> AddTransaction(EmployeeTransaction model)
    {
        Transtatus transtatus = new Transtatus();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddTransaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId",model.UserId);
                    cmd.Parameters.AddWithValue("@Amount",model.Amount);
                    cmd.Parameters.AddWithValue("@Type",model.Type.ToString());

                    cmd.Parameters.Add("@Message",SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@Code",SqlDbType.Int).Direction = ParameterDirection.Output;

                    await con.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();

                    transtatus.Message =Convert.ToString(cmd.Parameters["@Message"].Value);

                    transtatus.Code = Convert.ToInt32(cmd.Parameters["@Code"].Value);

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            transtatus.Code = 2;
            transtatus.Message = "Something went wrong";
        }

        return transtatus;
    }

    public async Task<List<EmployeeTransaction>>GetTransactionsByUser(Guid userId,int pageNumber,int pageSize)
    {
        List<EmployeeTransaction> list = new List<EmployeeTransaction>();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetTransactionsByUser",con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId",userId);
                    cmd.Parameters.AddWithValue("@PageNumber",pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize",pageSize);

                    await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new EmployeeTransaction
                            {
                                Id = Guid.Parse
                                (
                                    reader["Id"].ToString()
                                ),

                                UserId = Guid.Parse
                                (
                                    reader["UserId"].ToString()
                                ),

                                Amount = Convert.ToDecimal
                                (
                                    reader["Amount"]
                                ),

                                Type = Enum.Parse<TransactionType>
                                (
                                    reader["Type"].ToString()
                                ),

                                CreatedDate = Convert.ToDateTime
                                (
                                    reader["CreatedDate"]
                                ),

                                RunningBalance = Convert.ToDecimal
                                (
                                    reader["RunningBalance"]
                                )
                            });
                        }
                    }

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            throw;
        }

        return list;
    }

    public async Task<List<NetBalanceResponse>>GetNetBalance()
    {
        List<NetBalanceResponse> list = new List<NetBalanceResponse>();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                string query = @"
                SELECT
                    E.Id,
                    E.Name,

                    ISNULL
                    (
                        SUM
                        (
                            CASE
                                WHEN T.Type = 'Credit'
                                THEN T.Amount

                                WHEN T.Type = 'Debit'
                                THEN -T.Amount
                            END
                        ),
                        0
                    ) AS NetBalance

                FROM Employees E
                LEFT JOIN Transactions T
                    ON E.Id = T.UserId

                GROUP BY
                    E.Id,
                    E.Name

                ORDER BY NetBalance DESC";

                using (var cmd =
                    new SqlCommand(query, con))
                {
                    await con.OpenAsync();

                    using (var reader =
                        await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new NetBalanceResponse
                            {
                                UserId = Guid.Parse
                                (
                                    reader["Id"].ToString()
                                ),

                                Name = Convert.ToString
                                (
                                    reader["Name"]
                                ),

                                NetBalance = Convert.ToDecimal
                                (
                                    reader["NetBalance"]
                                )
                            });
                        }
                    }

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            throw;
        }

        return list;
    }
}