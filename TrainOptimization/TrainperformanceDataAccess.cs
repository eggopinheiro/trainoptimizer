using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 23/04/2015 20:31:35
/// <summary>

[DataObject(true)]
public class TrainperformanceDataAccess
{
	public TrainperformanceDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(string ptraintype, Int16 pdirection, int plocation, string pud, int pstop_location, double ptimemov, double ptimestop, double ptimeheadwaymov, double ptimeheadwaystop, Int16 pDestinationTrack)
	{
		string lvSql = "";
		int lvRowAffects = 0;

        lvSql = "insert into tbtrainperformance(traintype, direction, location, ud, stop_location, timemov, timestop, timeheadwaymov, timeheadwaystop, destination_track) ";
        lvSql += "values(@traintype, @direction, @location, @ud, @stop_location, @timemov, @timestop, @timeheadwaymov, @timeheadwaystop, @destinationtrack)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = ptraintype;

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		if(pDestinationTrack == Int16.MinValue)
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pDestinationTrack;
		}

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;

		if(pstop_location == Int32.MinValue)
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = pstop_location;
		}

        if (ptimemov == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = ptimemov;
		}

        if (ptimestop == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = ptimestop;
		}

        if (ptimeheadwaymov == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = ptimeheadwaymov;
        }

        if (ptimeheadwaystop == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = ptimeheadwaystop;
        }

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

        ConnectionManager.DebugMySqlQuery(cmd, "TrainperformanceDataAccess.Insert.lvSql");

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
    public static int Update(string ptraintype, Int16 pdirection, int plocation, string pud, int pstop_location, double ptimemov, double ptimestop, double ptimeheadwaymov, double ptimeheadwaystop, Int16 pDestinationTrack)
	{
		string lvSql = "";
		int lvRowAffects = 0;

        lvSql = "update tbtrainperformance set stop_location=@stop_location, timemov=@timemov, timestop=@timestop, timeheadwaymov=@timeheadwaymov, timeheadwaystop=@timeheadwaystop Where traintype=@traintype And direction=@direction And location=@location And ud=@ud And destination_track=@destinationtrack";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = ptraintype;
		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		if(pDestinationTrack == Int16.MinValue)
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pDestinationTrack;
		}
        
		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;
		if(pstop_location == Int32.MinValue)
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = pstop_location;
		}

        if (ptimemov == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = ptimemov;
		}

		if(ptimestop == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = ptimestop;
		}

        if (ptimeheadwaymov == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = ptimeheadwaymov;
        }

        if (ptimeheadwaystop == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = ptimeheadwaystop;
        }

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

        ConnectionManager.DebugMySqlQuery(cmd, "TrainperformanceDataAccess.Update.lvSql");

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
    public static int UpdateKey(string pOrigtraintype, Int16 pOrigdirection, int pOriglocation, string pOrigud, Int16 pOrigdestinationtrack, string ptraintype, Int16 pdirection, int plocation, string pud, Int16 pdestinationtrack)
	{
		string lvSql = "";
		int lvRowAffects = 0;

        lvSql = "update tbtrainperformance set traintype=@traintype, direction=@direction, location=@location, ud=@ud, destination_track=@destinationtrack Where traintype=@origtraintype And direction=@origdirection And location=@origlocation And ud=@origud And destination_track=@origdestinationtrack";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = ptraintype;
		cmd.Parameters.Add("@origtraintype", MySqlDbType.String).Value = pOrigtraintype;
		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		if(pdestinationtrack == Int16.MinValue)
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pdestinationtrack;
		}

        if (pOrigdestinationtrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@origdestinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@origdestinationtrack", MySqlDbType.Int16).Value = pOrigdestinationtrack;
        }
        
        if (pOrigdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@origdirection", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origdirection", MySqlDbType.Int16).Value = pOrigdirection;
		}

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

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;
		cmd.Parameters.Add("@origud", MySqlDbType.String).Value = pOrigud;
		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(string ptraintype, Int16 pdirection, int plocation, string pud, Int16 pdestinationtrack)
	{
		string lvSql = "";
		int lvRowAffects = 0;

        lvSql = "delete from tbtrainperformance Where traintype=@traintype And direction=@direction And location=@location And ud=@ud And destination_track=@destinationtrack";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = ptraintype;

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

        if (pdestinationtrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pdestinationtrack;
        }

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainperformanceDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetAll()
	{
		string lvSql = "";
		DataSet ds = new DataSet();

		lvSql = "select * from tbtrainperformance";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainperformance");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetData(string ptraintype = "", Int16 pdirection = -32768, int plocation = -2147483648, string pud = "", int pstop_location = -2147483648, double ptimemov = -999999999999, double ptimestop = -999999999999, double ptimeheadwaymov = -999999999999, double ptimeheadwaystop = -999999999999, Int16 pDestrinationTrack = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

        lvSql = "select tbtrainperformance.traintype, tbtrainperformance.direction, tbtrainperformance.location, tbtrainperformance.ud, tbtrainperformance.stop_location, tbtrainperformance.timemov, tbtrainperformance.timemovstop, tbtrainperformance.timestop, tbtrainperformance.timestopstop, tbtrainperformance.timeheadwaymov, tbtrainperformance.timeheadwaymovstop, tbtrainperformance.timeheadwaystop, tbtrainperformance.timeheadwaystopstop, tbtrainperformance.destination_track From tbtrainperformance";

		if(!string.IsNullOrEmpty(ptraintype))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.traintype like @traintype";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.traintype like @traintype";
			}
		}

		if(pdirection > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.direction=@direction";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.direction=@direction";
			}
		}

		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.location=@location";
			}
		}

		if(!string.IsNullOrEmpty(pud))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.ud like @ud";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.ud like @ud";
			}
		}

		if(pstop_location > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.stop_location=@stop_location";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.stop_location=@stop_location";
			}
		}

        if (ptimemov > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timemov=@timemov";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timemov=@timemov";
			}
		}

        if (ptimestop > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timestop=@timestop";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timestop=@timestop";
			}
		}

        if (ptimeheadwaymov > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaymov=@timeheadwaymov";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaymov=@timeheadwaymov";
            }
        }

        if (ptimeheadwaystop > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaystop=@timeheadwaystop";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaystop=@timeheadwaystop";
            }
        }

        if (pDestrinationTrack > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.destination_track=@destinationtrack";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.destination_track=@destinationtrack";
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

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = "%" + ptraintype + "%";

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

        if (pDestrinationTrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pDestrinationTrack;
        }

		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = "%" + pud + "%";

		if(pstop_location == Int32.MinValue)
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@stop_location", MySqlDbType.Int32).Value = pstop_location;
		}

        if (ptimemov == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timemov", MySqlDbType.Double).Value = ptimemov;
		}

        if (ptimestop == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timestop", MySqlDbType.Double).Value = ptimestop;
		}

        if (ptimeheadwaymov == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaymov", MySqlDbType.Double).Value = ptimeheadwaymov;
        }

        if (ptimeheadwaystop == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaystop", MySqlDbType.Double).Value = ptimeheadwaystop;
        }

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainperformance");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetDataByRange(string ptraintype = "", Int16 pdirectionInicio = -32768, Int16 pdirectionFim = -32768, int plocationInicio = -2147483648, int plocationFim = -2147483648, string pud = "", int pstop_locationInicio = -2147483648, int pstop_locationFim = -2147483648, double ptimemovInicio = -999999999999, double ptimemovFim = -999999999999, double ptimestopInicio = -999999999999, double ptimestopFim = -999999999999, double ptimeheadwaymovInicio = -999999999999, double ptimeheadwaymovFim = -999999999999, double ptimeheadwaystopInicio = -999999999999, double ptimeheadwaystopFim = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

        lvSql = "select tbtrainperformance.traintype, tbtrainperformance.direction, tbtrainperformance.location, tbtrainperformance.ud, tbtrainperformance.stop_location, tbtrainperformance.timemov, tbtrainperformance.timestop, tbtrainperformance.timeheadwaymov, tbtrainperformance.timeheadwaystop From tbtrainperformance";

		if(!string.IsNullOrEmpty(ptraintype))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.traintype like @traintype";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.traintype like @traintype";
			}
		}

		if(pdirectionInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.direction >= @directionInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.direction >= @directionInicio";
			}
		}

		if(pdirectionFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.direction <= @directionFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.direction <= @directionFim";
			}
		}

		if(plocationInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.location >= @locationInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.location >= @locationInicio";
			}
		}

		if(plocationFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.location <= @locationFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.location <= @locationFim";
			}
		}

		if(!string.IsNullOrEmpty(pud))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.ud like @ud";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.ud like @ud";
			}
		}

		if(pstop_locationInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.stop_location >= @stop_locationInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.stop_location >= @stop_locationInicio";
			}
		}

		if(pstop_locationFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.stop_location <= @stop_locationFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.stop_location <= @stop_locationFim";
			}
		}

        if (ptimemovInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timemov >= @timemovInicio";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timemov >= @timemovInicio";
			}
		}

        if (ptimemovFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timemov <= @timemovFim";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timemov <= @timemovFim";
			}
		}

        if (ptimestopInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timestop >= @timestopInicio";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timestop >= @timestopInicio";
			}
		}

        if (ptimestopFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
                lvSqlWhere += "tbtrainperformance.timestop <= @timestopFim";
			}
			else
			{
                lvSqlWhere += " And tbtrainperformance.timestop <= @timestopFim";
			}
		}

        if (ptimeheadwaymovInicio > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaymov >= @timeheadwaymovInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaymov >= @timeheadwaymovInicio";
            }
        }

        if (ptimeheadwaymovFim > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaymov <= @timeheadwaymovFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaymov <= @timeheadwaymovFim";
            }
        }

        if (ptimeheadwaystopInicio > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaystop >= @timeheadwaystopInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaystop >= @timeheadwaystopInicio";
            }
        }

        if (ptimeheadwaystopFim > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.timeheadwaystop <= @timeheadwaystopFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.timeheadwaystop <= @timeheadwaystopFim";
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

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = "%" + ptraintype + "%";

		if(pdirectionInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@directionInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@directionInicio", MySqlDbType.Int16).Value = pdirectionInicio;
		}

		if(pdirectionFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@directionFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@directionFim", MySqlDbType.Int16).Value = pdirectionFim;
		}

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

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = "%" + pud + "%";

		if(pstop_locationInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@stop_locationInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@stop_locationInicio", MySqlDbType.Int32).Value = pstop_locationInicio;
		}

		if(pstop_locationFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@stop_locationFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@stop_locationFim", MySqlDbType.Int32).Value = pstop_locationFim;
		}

        if (ptimemovInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timemovInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timemovInicio", MySqlDbType.Double).Value = ptimemovInicio;
		}

        if (ptimemovFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timemovFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timemovFim", MySqlDbType.Double).Value = ptimemovFim;
		}

        if (ptimestopInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timestopInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timestopInicio", MySqlDbType.Double).Value = ptimestopInicio;
		}

        if (ptimestopFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
            cmd.Parameters.Add("@timestopFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@timestopFim", MySqlDbType.Double).Value = ptimestopFim;
		}

        if (ptimeheadwaymovInicio == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaymovInicio", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaymovInicio", MySqlDbType.Double).Value = ptimeheadwaymovInicio;
        }

        if (ptimeheadwaymovFim == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaymovFim", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaymovFim", MySqlDbType.Double).Value = ptimeheadwaymovFim;
        }

        if (ptimeheadwaystopInicio == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaystopInicio", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaystopInicio", MySqlDbType.Double).Value = ptimeheadwaystopInicio;
        }

        if (ptimeheadwaystopFim == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@timeheadwaystopFim", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timeheadwaystopFim", MySqlDbType.Double).Value = ptimeheadwaystopFim;
        }

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainperformance");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetDataByKey(string ptraintype = "", Int16 pdirection = -32768, int plocation = -2147483648, string pud = "", Int16 pdestinationtrack = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

        lvSql = "select tbtrainperformance.traintype, tbtrainperformance.direction, tbtrainperformance.location, tbtrainperformance.ud, tbtrainperformance.stop_location, tbtrainperformance.timemov, tbtrainperformance.timemovstop, tbtrainperformance.timestop, tbtrainperformance.timestopstop, tbtrainperformance.timeheadwaymov, tbtrainperformance.timeheadwaymovstop, tbtrainperformance.timeheadwaystop, tbtrainperformance.timeheadwaystopstop, tbtrainperformance.destination_track From tbtrainperformance";
		if(!string.IsNullOrEmpty(ptraintype))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.traintype like @traintype";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.traintype like @traintype";
			}
		}

		if(pdirection > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.direction=@direction";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.direction=@direction";
			}
		}

        if (pdestinationtrack > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainperformance.destination_track=@destinationtrack";
            }
            else
            {
                lvSqlWhere += " And tbtrainperformance.destination_track=@destinationtrack";
            }
        }

		if(plocation > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.location=@location";
			}
		}

		if(!string.IsNullOrEmpty(pud))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainperformance.ud like @ud";
			}
			else
			{
				lvSqlWhere += " And tbtrainperformance.ud like @ud";
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

		cmd.Parameters.Add("@traintype", MySqlDbType.String).Value = "%" + ptraintype + "%";

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		if(pdestinationtrack == Int16.MinValue)
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
            cmd.Parameters.Add("@destinationtrack", MySqlDbType.Int16).Value = pdestinationtrack;
		}
        
		if(plocation == Int32.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int32).Value = plocation;
		}

		cmd.Parameters.Add("@ud", MySqlDbType.String).Value = "%" + pud + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainperformance");
		return ds;
	}

}

