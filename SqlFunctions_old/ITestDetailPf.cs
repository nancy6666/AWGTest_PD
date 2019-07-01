using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public interface ITestDetailPf
    {
        int Ret_id
        {
            set;
            get;
        }       
        int Class_Id { get; set; }
        bool PfRet { get; set; }
        void SaveTestDetailPf(SqlCommand cmd);
    }
}
