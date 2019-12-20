using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:55
/// <summary>

[DataObject(true)]
public class SegmentDataAccess
{
    protected static Int64 mBranch = 0;
    protected static string mBranchName = "";

    public SegmentDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(int plocation, string psegment, int pstart_coordinate, int pend_coordinate, bool pAllowSameLineMov = false)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbsegment(location, segment, start_coordinate, end_coordinate, allowSameLineMov) ";
		lvSql += "values(@location, @segment, @start_coordinate, @end_coordinate, @allowSameLineMov)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = psegment;

		if(pstart_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = pstart_coordinate;
		}

		if(pend_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = pend_coordinate;
		}

        cmd.Parameters.Add("@allowSameLineMov", MySqlDbType.Binary).Value = pAllowSameLineMov;

        cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(int plocation, string psegment, int pstart_coordinate, int pend_coordinate, bool pAllowSameLineMov)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbsegment set start_coordinate=@start_coordinate, end_coordinate=@end_coordinate, allowSameLineMov=@allowSameLineMov Where location=@location And segment=@segment";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = psegment;
		if(pstart_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = pstart_coordinate;
		}

		if(pend_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = pend_coordinate;
		}

        cmd.Parameters.Add("@allowSameLineMov", MySqlDbType.Binary).Value = pAllowSameLineMov;

        cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(int pOriglocation, string pOrigsegment, int plocation, string psegment)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbsegment set location=@location, segment=@segment Where location=@origlocation And segment=@origsegment";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		if(pOriglocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@origlocation", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origlocation", MySqlDbType.Int32).Value = pOriglocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = psegment;
		cmd.Parameters.Add("@origsegment", MySqlDbType.String).Value = pOrigsegment;
		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(int plocation, string psegment)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbsegment Where location=@location And segment=@segment";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = psegment;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

    public static void LoadBranch(string pStrBranch)
    {
        Int64 lvRes = 0;
        string lvSql = "";
        DataSet ds = new DataSet();

        lvSql = "select id from tbbranch where tbbranch.name=@name";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@name", MySqlDbType.String).Value = pStrBranch;

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbbranch");

        if (ds.Tables[0].Rows.Count > 0)
        {
            mBranch = Convert.ToInt64(ds.Tables[0].Rows[0][0]);
            mBranchName = pStrBranch;
        }
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetAll(string pSortField = "")
	{
		string lvSql = "";
		DataSet ds = new DataSet();

		lvSql = "select * from tbsegment where branch_id=@branch_id";

        if (pSortField.Length > 0)
        {
            lvSql += " order by " + pSortField;           
        }

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@branch_id", MySqlDbType.String).Value = mBranch;

        cmd.CommandType = CommandType.Text;

#if DEBUG
        ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
#endif

        conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(int plocation = -2147483648, string psegment = "", int pstart_coordinate = -2147483648, int pend_coordinate = -2147483648, bool pAllowSameLineMov = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbsegment.location, tbsegment.segment, tbsegment.start_coordinate, tbsegment.end_coordinate, tbsegment.allowSameLineMov From tbsegment";

		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbsegment.location=@location";
			}
		}

		if(!string.IsNullOrEmpty(psegment))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.segment like @segment";
			}
			else
			{
				lvSqlWhere += " And tbsegment.segment like @segment";
			}
		}

		if(pstart_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.start_coordinate=@start_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbsegment.start_coordinate=@start_coordinate";
			}
		}

		if(pend_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.end_coordinate=@end_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbsegment.end_coordinate=@end_coordinate";
			}
		}

        if (pAllowSameLineMov)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbsegment.allowSameLineMov=@allowSameLineMov";
            }
            else
            {
                lvSqlWhere += " And tbsegment.allowSameLineMov=@allowSameLineMov";
            }
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = "%" + psegment + "%";

		if(pstart_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_coordinate", MySqlDbType.Int32).Value = pstart_coordinate;
		}

		if(pend_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_coordinate", MySqlDbType.Int32).Value = pend_coordinate;
		}

        if (pAllowSameLineMov)
        {
            cmd.Parameters.Add("@allowSameLineMov", MySqlDbType.Binary).Value = pAllowSameLineMov;
        }

        cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(int plocationInicio = -2147483648, int plocationFim = -2147483648, string psegment = "", int pstart_coordinateInicio = -2147483648, int pstart_coordinateFim = -2147483648, int pend_coordinateInicio = -2147483648, int pend_coordinateFim = -2147483648, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbsegment.location, tbsegment.segment, tbsegment.start_coordinate, tbsegment.end_coordinate, tbsegment.allowSameLineMov From tbsegment";

		if(plocationInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.location >= @locationInicio";
			}
			else
			{
				lvSqlWhere += " And tbsegment.location >= @locationInicio";
			}
		}

		if(plocationFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.location <= @locationFim";
			}
			else
			{
				lvSqlWhere += " And tbsegment.location <= @locationFim";
			}
		}

		if(!string.IsNullOrEmpty(psegment))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.segment like @segment";
			}
			else
			{
				lvSqlWhere += " And tbsegment.segment like @segment";
			}
		}

		if(pstart_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.start_coordinate >= @start_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbsegment.start_coordinate >= @start_coordinateInicio";
			}
		}

		if(pstart_coordinateFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.start_coordinate <= @start_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbsegment.start_coordinate <= @start_coordinateFim";
			}
		}

		if(pend_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.end_coordinate >= @end_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbsegment.end_coordinate >= @end_coordinateInicio";
			}
		}

		if(pend_coordinateFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.end_coordinate <= @end_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbsegment.end_coordinate <= @end_coordinateFim";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocationInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@locationInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@locationInicio", MySqlDbType.Int32).Value = plocationInicio;
		}

		if(plocationFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@locationFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@locationFim", MySqlDbType.Int32).Value = plocationFim;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = "%" + psegment + "%";

		if(pstart_coordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_coordinateInicio", MySqlDbType.Int32).Value = pstart_coordinateInicio;
		}

		if(pstart_coordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_coordinateFim", MySqlDbType.Int32).Value = pstart_coordinateFim;
		}

		if(pend_coordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_coordinateInicio", MySqlDbType.Int32).Value = pend_coordinateInicio;
		}

		if(pend_coordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_coordinateFim", MySqlDbType.Int32).Value = pend_coordinateFim;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(int plocation = -2147483648, string psegment = "", string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbsegment.location, tbsegment.segment, tbsegment.start_coordinate, tbsegment.end_coordinate, tbsegment.allowSameLineMov From tbsegment";
		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbsegment.location=@location";
			}
		}

		if(!string.IsNullOrEmpty(psegment))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbsegment.segment like @segment";
			}
			else
			{
				lvSqlWhere += " And tbsegment.segment like @segment";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@segment", MySqlDbType.String).Value = "%" + psegment + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbsegment");
		return ds;
	}

    public static long Branch
    {
        get
        {
            return mBranch;
        }
    }

    public static string BranchName
    {
        get
        {
            return mBranchName;
        }
    }
}

