using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SqlFunctions
{
   public interface ITestDetail
    {
        int Channel { get;  set; }
        /// <summary>
        /// 一条测试数据在数据库表中的ID号
        /// </summary>
        int RawID { get; set; }
        /// <summary>
        /// 判断一组测试数据是否在客户要的spec内
        /// </summary>
        /// <param name="testDetailPf">测试数据的判断结果</param>
        void GetPfRet(out ITestDetailPf testDetailPf);
        /// <summary>
        /// 保存一条测试数据到数据库
        /// </summary>
        /// <param name="cmd">数据库实例化对象</param>
        /// <param name="testId"></param>
        void SaveTestDetail(SqlCommand cmd,int testId);
       
    }
}
