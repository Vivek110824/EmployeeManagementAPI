using EmployeeManagement.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.DAL;

public class EmployeeRepository
{
    public async Task<Transtatus> AddEmployee(EmployeeCreateRequest model)
    {
        Transtatus transtatus = new Transtatus();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_AddEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Department",
                        (object?)model.Department ?? DBNull.Value);

                    cmd.Parameters.Add("@Message", SqlDbType.VarChar, 500)
                        .Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@Code", SqlDbType.Int)
                        .Direction = ParameterDirection.Output;

                    await con.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();

                    transtatus.Message =
                        Convert.ToString(cmd.Parameters["@Message"].Value);

                    transtatus.Code =
                        Convert.ToInt32(cmd.Parameters["@Code"].Value);

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            transtatus.Message = "Something went wrong";
            transtatus.Code = 2;
        }

        return transtatus;
    }

    public async Task<Transtatus> UpdateEmployee(Guid id,EmployeeUpdateRequest model)
    {
        Transtatus transtatus = new Transtatus();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Department",
                        (object?)model.Department ?? DBNull.Value);

                    cmd.Parameters.Add("@Message", SqlDbType.VarChar, 500)
                        .Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@Code", SqlDbType.Int)
                        .Direction = ParameterDirection.Output;

                    await con.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();

                    transtatus.Message =
                        Convert.ToString(cmd.Parameters["@Message"].Value);

                    transtatus.Code =
                        Convert.ToInt32(cmd.Parameters["@Code"].Value);

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            transtatus.Message = "Something went wrong";
            transtatus.Code = 2;
        }

        return transtatus;
    }

    public async Task<Transtatus> DeleteEmployee(Guid id)
    {
        Transtatus transtatus = new Transtatus();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_DeleteEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.Parameters.Add("@Message", SqlDbType.VarChar, 500)
                        .Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@Code", SqlDbType.Int)
                        .Direction = ParameterDirection.Output;

                    await con.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();

                    transtatus.Message =
                        Convert.ToString(cmd.Parameters["@Message"].Value);

                    transtatus.Code =
                        Convert.ToInt32(cmd.Parameters["@Code"].Value);

                    await con.CloseAsync();
                }
            }
        }
        catch
        {
            transtatus.Message = "Something went wrong";
            transtatus.Code = 2;
        }

        return transtatus;
    }


    public async Task<Employee?> GetEmployeeById(Guid id)
    {
        Employee? employee = null;

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetEmployeeById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new Employee
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = Convert.ToString(reader["Name"]),
                                Email = Convert.ToString(reader["Email"]),
                                Department = Convert.ToString(reader["Department"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
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

        return employee;
    }


    public async Task<List<Employee>> GetEmployees
    (
       EmployeeQueryParameters employeeQuery
    )
    {
        List<Employee> list = new List<Employee>();

        try
        {
            using (var con = new SqlConnection(CommonHelper.ConnectionString))
            {
                using (var cmd = new SqlCommand("sp_GetEmployees", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", employeeQuery.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", employeeQuery.PageSize);
                    cmd.Parameters.AddWithValue("@Search",
                        (object?)employeeQuery.Search ?? DBNull.Value);

                    await con.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Employee
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = Convert.ToString(reader["Name"]),
                                Email = Convert.ToString(reader["Email"]),
                                Department = Convert.ToString(reader["Department"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
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