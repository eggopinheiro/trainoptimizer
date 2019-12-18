using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:58
/// <summary>

[DataObject(true)]
public class StopLocationDataAccess
{
	public StopLocationDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(int plocation, int pstart_coordinate, int pend_coordinate, Int16 pcapacity)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbtgsegment(location, start_coordinate, end_coordinate, capacity) ";
		lvSql += "values(@location, @start_coordinate, @end_coordinate, @capacity)";

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

		if(pcapacity == Int16.MinValue)
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = pcapacity;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(int plocation, int pstart_coordinate, int pend_coordinate, Int16 pcapacity)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtgsegment set start_coordinate=@start_coordinate, end_coordinate=@end_coordinate, capacity=@capacity Where location=@location";

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

		if(pcapacity == Int16.MinValue)
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = pcapacity;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(int pOriglocation, int plocation)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtgsegment set location=@location Where location=@origlocation";

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

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(int plocation)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbtgsegment Where location=@location";

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

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TgsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetAll(string pSortField = "")
	{
		string lvSql = "";
		DataSet ds = new DataSet();

        lvSql = "select * from tbtgsegment";

        if (pSortField.Length > 0)
        {
            lvSql += " order by " + pSortField;
        }

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtgsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(int plocation = -2147483648, int pstart_coordinate = -2147483648, int pend_coordinate = -2147483648, Int16 pcapacity = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtgsegment.location, tbtgsegment.start_coordinate, tbtgsegment.end_coordinate, tbtgsegment.capacity From tbtgsegment";

		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.location=@location";
			}
		}

		if(pstart_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.start_coordinate=@start_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.start_coordinate=@start_coordinate";
			}
		}

		if(pend_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.end_coordinate=@end_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.end_coordinate=@end_coordinate";
			}
		}

		if(pcapacity > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.capacity=@capacity";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.capacity=@capacity";
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

		if(pcapacity == Int16.MinValue)
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@capacity", MySqlDbType.Int16).Value = pcapacity;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtgsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(int plocationInicio = -2147483648, int plocationFim = -2147483648, int pstart_coordinateInicio = -2147483648, int pstart_coordinateFim = -2147483648, int pend_coordinateInicio = -2147483648, int pend_coordinateFim = -2147483648, Int16 pcapacityInicio = -32768, Int16 pcapacityFim = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtgsegment.location, tbtgsegment.start_coordinate, tbtgsegment.end_coordinate, tbtgsegment.capacity From tbtgsegment";

		if(plocationInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.location >= @locationInicio";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.location >= @locationInicio";
			}
		}

		if(plocationFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.location <= @locationFim";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.location <= @locationFim";
			}
		}

		if(pstart_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.start_coordinate >= @start_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.start_coordinate >= @start_coordinateInicio";
			}
		}

		if(pstart_coordinateFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.start_coordinate <= @start_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.start_coordinate <= @start_coordinateFim";
			}
		}

		if(pend_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.end_coordinate >= @end_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.end_coordinate >= @end_coordinateInicio";
			}
		}

		if(pend_coordinateFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.end_coordinate <= @end_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.end_coordinate <= @end_coordinateFim";
			}
		}

		if(pcapacityInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.capacity >= @capacityInicio";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.capacity >= @capacityInicio";
			}
		}

		if(pcapacityFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.capacity <= @capacityFim";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.capacity <= @capacityFim";
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

		if(pcapacityInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@capacityInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@capacityInicio", MySqlDbType.Int16).Value = pcapacityInicio;
		}

		if(pcapacityFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@capacityFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@capacityFim", MySqlDbType.Int16).Value = pcapacityFim;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtgsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(int plocation = -2147483648, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtgsegment.location, tbtgsegment.start_coordinate, tbtgsegment.end_coordinate, tbtgsegment.capacity From tbtgsegment";
		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtgsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbtgsegment.location=@location";
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

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtgsegment");
		return ds;
	}

}

