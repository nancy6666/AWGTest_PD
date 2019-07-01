using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions.Class_Test_Data_Spec
{
    public class CProductCategory
    {
        public string Name { get; set; }
        public int Max_Channel { get; set; }
        public string Comment { get; set; }
        public float Itu_Start { get; set; }
        public float Itu_Step { get; set; }

        //public CProductCategory(SqlCommand cmd,int id)
        //{
        //    ReadTable(cmd, id);
        //}
        public void ReadTable(SqlCommand cmd, int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Category_ID",
                    DbType = DbType.Int32,
                    Value = id
                });
                cmd.CommandText = "dbo.select_product_category";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds != null & ds.Tables.Count == 1)
                {
                    DataTable dt = new DataTable();
                    dt = ds.Tables[0];
                    if (dt.Rows.Count == 1)
                    {
                        this.Max_Channel = (int)dt.Rows[0]["max_channel"];
                        this.Name = (string)dt.Rows[0]["name"];
                        this.Itu_Start = (float)Convert.ToDouble(dt.Rows[0]["itu_start"]);
                        this.Itu_Step = (float)Convert.ToDouble(dt.Rows[0]["itu_step"]);
                    }
                    else
                    {
                        throw new Exception("从数据表plc_production_product_category查询到的数据不止一行！");
                    }
                }
                else
                {
                    throw new Exception("从数据表plc_production_product_category查询的数据为空或者不止一张数据表!");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            }
        
    }
   
}
