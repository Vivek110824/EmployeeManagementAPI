using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Model
{
    public class CommonHelper
    {
        public static string ConnectionString = string.Empty;
    }
    public class Transtatus
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
