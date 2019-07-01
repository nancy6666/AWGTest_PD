using System;
using System.Collections.Generic;
using System.Text;

namespace SqlFunctions //修改名字空间
{
	public class MES_TEST_DATA
	{
		/// <summary>
		/// 无参构造方法
		/// </summary>
		public MES_TEST_DATA(){ }
	
		/// <summary>
		/// 指定字段的构造方法
		/// </summary>
		public MES_TEST_DATA(string serial_no,string current_station,string next_station,DateTime test_start,DateTime test_time,string result,string test_failures,string operator1,string test_no,string vender_id,string date_code,string test_stage,string line_no,decimal mES_SECOND_data,long id,string iN_OUT,string workshop_id,string mo_no,string wafer_no,string part_no,string remark,string create_by,DateTime create_date)
		{
			this.serial_no = serial_no;
			this.current_station = current_station;
			this.next_station = next_station;
			this.test_start = test_start;
			this.test_time = test_time;
			this.result = result;
			this.test_failures = test_failures;
			this.operator1 = operator1;
			this.test_no = test_no;
			this.vender_id = vender_id;
			this.date_code = date_code;
			this.test_stage = test_stage;
			this.line_no = line_no;
			this.mES_SECOND_data = mES_SECOND_data;
			this.id = id;
			this.iN_OUT = iN_OUT;
			this.workshop_id = workshop_id;
			this.mo_no = mo_no;
			this.wafer_no = wafer_no;
			this.part_no = part_no;
			this.remark = remark;
			this.create_by = create_by;
			this.create_date = create_date;
		}
	
		private string serial_no;
		public string Serial_no
		{
			get { return serial_no; }
			set { serial_no = value; }
		}
	
		private string current_station;
		public string Current_station
		{
			get { return current_station; }
			set { current_station = value; }
		}
	
		private string next_station;
		public string Next_station
		{
			get { return next_station; }
			set { next_station = value; }
		}
	
		private DateTime test_start;
		public DateTime Test_start
		{
			get { return test_start; }
			set { test_start = value; }
		}
	
		private DateTime test_time;
		public DateTime Test_time
		{
			get { return test_time; }
			set { test_time = value; }
		}
	
		private string result;
		public string Result
		{
			get { return result; }
			set { result = value; }
		}
	
		private string test_failures;
		public string Test_failures
		{
			get { return test_failures; }
			set { test_failures = value; }
		}
	
		private string operator1;
		public string Operator
		{
			get { return operator1; }
			set { operator1 = value; }
		}
	
		private string test_no;
		public string Test_no
		{
			get { return test_no; }
			set { test_no = value; }
		}
	
		private string vender_id;
		public string Vender_id
		{
			get { return vender_id; }
			set { vender_id = value; }
		}
	
		private string date_code;
		public string Date_code
		{
			get { return date_code; }
			set { date_code = value; }
		}
	
		private string test_stage;
		public string Test_stage
		{
			get { return test_stage; }
			set { test_stage = value; }
		}
	
		private string line_no;
		public string Line_no
		{
			get { return line_no; }
			set { line_no = value; }
		}
	
		private decimal mES_SECOND_data;
		public decimal MES_SECOND_data
		{
			get { return mES_SECOND_data; }
			set { mES_SECOND_data = value; }
		}
	
		private long id;
		public long Id
		{
			get { return id; }
			set { id = value; }
		}
	
		private string iN_OUT;
		public string IN_OUT
		{
			get { return iN_OUT; }
			set { iN_OUT = value; }
		}
	
		private string workshop_id;
		public string Workshop_id
		{
			get { return workshop_id; }
			set { workshop_id = value; }
		}
	
		private string mo_no;
		public string Mo_no
		{
			get { return mo_no; }
			set { mo_no = value; }
		}
	
		private string wafer_no;
		public string Wafer_no
		{
			get { return wafer_no; }
			set { wafer_no = value; }
		}
	
		private string part_no;
		public string Part_no
		{
			get { return part_no; }
			set { part_no = value; }
		}
	
		private string remark;
		public string Remark
		{
			get { return remark; }
			set { remark = value; }
		}
	
		private string create_by;
		public string Create_by
		{
			get { return create_by; }
			set { create_by = value; }
		}
	
		private DateTime create_date;
		public DateTime Create_date
		{
			get { return create_date; }
			set { create_date = value; }
		}
	}
}