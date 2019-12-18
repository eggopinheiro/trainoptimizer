using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 13/02/2015 14:03:58
/// <summary>

[DataObject(true)]
public class TrainDataAccess
{
	public TrainDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(double ptrain_id, string pname, string ptype, DateTime pcreation_tm, DateTime pdeparture_time, DateTime parrival_time, Int16 pdirection, string ppriority, string pstatus, int pdeparture_coordinate, int parrival_coordinate, double plotes, Int16 pisvalid, int plast_coordinate, DateTime plast_info_updated, string ppmt_id, string pOS, double pplan_id, string pOSSGF, string plast_track, DateTime phist, Int16 pcmd_loco_id, Int16 pusr_cmd_loco_id, Int16 pplan_id_lock, string poid, int punilogcurrcoord, DateTime punilogcurinfodate, string punilogcurseg, double ploco_code)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbtrain(train_id, name, type, creation_tm, departure_time, arrival_time, direction, priority, status, departure_coordinate, arrival_coordinate, lotes, isvalid, last_coordinate, last_info_updated, pmt_id, OS, plan_id, OSSGF, last_track, hist, cmd_loco_id, usr_cmd_loco_id, plan_id_lock, oid, unilogcurrcoord, unilogcurinfodate, unilogcurseg, loco_code) ";
		lvSql += "values(@train_id, @name, @type, @creation_tm, @departure_time, @arrival_time, @direction, @priority, @status, @departure_coordinate, @arrival_coordinate, @lotes, @isvalid, @last_coordinate, @last_info_updated, @pmt_id, @OS, @plan_id, @OSSGF, @last_track, @hist, @cmd_loco_id, @usr_cmd_loco_id, @plan_id_lock, @oid, @unilogcurrcoord, @unilogcurinfodate, @unilogcurseg, @loco_code)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.Parameters.Add("@name", MySqlDbType.String).Value = pname;

		cmd.Parameters.Add("@type", MySqlDbType.String).Value = ptype;

		if(pcreation_tm == DateTime.MinValue)
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = pcreation_tm.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(parrival_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = parrival_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		cmd.Parameters.Add("@priority", MySqlDbType.String).Value = ppriority;

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;

		if(pdeparture_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = pdeparture_coordinate;
		}

		if(parrival_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = parrival_coordinate;
		}

		if(plotes == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = plotes;
		}

		if(pisvalid == Int16.MinValue)
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = pisvalid;
		}

		if(plast_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = plast_coordinate;
		}

		if(plast_info_updated == DateTime.MinValue)
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = plast_info_updated.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = pOS;

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@OSSGF", MySqlDbType.String).Value = pOSSGF;

		cmd.Parameters.Add("@last_track", MySqlDbType.String).Value = plast_track;

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = pcmd_loco_id;
		}

		if(pusr_cmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = pusr_cmd_loco_id;
		}

		if(pplan_id_lock == Int16.MinValue)
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = pplan_id_lock;
		}

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = poid;

		if(punilogcurrcoord == Int32.MinValue)
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = punilogcurrcoord;
		}

		if(punilogcurinfodate == DateTime.MinValue)
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = punilogcurinfodate.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@unilogcurseg", MySqlDbType.String).Value = punilogcurseg;

		if(ploco_code == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = ploco_code;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(double ptrain_id, string pname, string ptype, DateTime pcreation_tm, DateTime pdeparture_time, DateTime parrival_time, Int16 pdirection, string ppriority, string pstatus, int pdeparture_coordinate, int parrival_coordinate, double plotes, Int16 pisvalid, int plast_coordinate, DateTime plast_info_updated, string ppmt_id, string pOS, double pplan_id, string pOSSGF, string plast_track, DateTime phist, Int16 pcmd_loco_id, Int16 pusr_cmd_loco_id, Int16 pplan_id_lock, string poid, int punilogcurrcoord, DateTime punilogcurinfodate, string punilogcurseg, double ploco_code)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrain set name=@name, type=@type, creation_tm=@creation_tm, departure_time=@departure_time, arrival_time=@arrival_time, direction=@direction, priority=@priority, status=@status, departure_coordinate=@departure_coordinate, arrival_coordinate=@arrival_coordinate, lotes=@lotes, isvalid=@isvalid, last_coordinate=@last_coordinate, last_info_updated=@last_info_updated, pmt_id=@pmt_id, OS=@OS, plan_id=@plan_id, OSSGF=@OSSGF, last_track=@last_track, hist=@hist, cmd_loco_id=@cmd_loco_id, usr_cmd_loco_id=@usr_cmd_loco_id, plan_id_lock=@plan_id_lock, oid=@oid, unilogcurrcoord=@unilogcurrcoord, unilogcurinfodate=@unilogcurinfodate, unilogcurseg=@unilogcurseg, loco_code=@loco_code Where train_id=@train_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.Parameters.Add("@name", MySqlDbType.String).Value = pname;
		cmd.Parameters.Add("@type", MySqlDbType.String).Value = ptype;
		if(pcreation_tm == DateTime.MinValue)
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = pcreation_tm.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(parrival_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = parrival_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		cmd.Parameters.Add("@priority", MySqlDbType.String).Value = ppriority;
		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;
		if(pdeparture_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = pdeparture_coordinate;
		}

		if(parrival_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = parrival_coordinate;
		}

		if(plotes == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = plotes;
		}

		if(pisvalid == Int16.MinValue)
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = pisvalid;
		}

		if(plast_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = plast_coordinate;
		}

		if(plast_info_updated == DateTime.MinValue)
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = plast_info_updated.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = pOS;
		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@OSSGF", MySqlDbType.String).Value = pOSSGF;
		cmd.Parameters.Add("@last_track", MySqlDbType.String).Value = plast_track;
		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = pcmd_loco_id;
		}

		if(pusr_cmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = pusr_cmd_loco_id;
		}

		if(pplan_id_lock == Int16.MinValue)
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = pplan_id_lock;
		}

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = poid;
		if(punilogcurrcoord == Int32.MinValue)
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = punilogcurrcoord;
		}

		if(punilogcurinfodate == DateTime.MinValue)
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = punilogcurinfodate.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@unilogcurseg", MySqlDbType.String).Value = punilogcurseg;
		if(ploco_code == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = ploco_code;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(double pOrigtrain_id, double ptrain_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrain set train_id=@train_id Where train_id=@origtrain_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(pOrigtrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = pOrigtrain_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(double ptrain_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbtrain Where train_id=@train_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbtrain";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrain");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(double ptrain_id = -999999999999, string pname = "", string ptype = "", DateTime pcreation_tm = default(DateTime), DateTime pdeparture_time = default(DateTime), DateTime parrival_time = default(DateTime), Int16 pdirection = -32768, string ppriority = "", string pstatus = "", int pdeparture_coordinate = -2147483648, int parrival_coordinate = -2147483648, double plotes = -999999999999, Int16 pisvalid = -32768, int plast_coordinate = -2147483648, DateTime plast_info_updated = default(DateTime), string ppmt_id = "", string pOS = "", double pplan_id = -999999999999, string pOSSGF = "", string plast_track = "", DateTime phist = default(DateTime), Int16 pcmd_loco_id = -32768, Int16 pusr_cmd_loco_id = -32768, Int16 pplan_id_lock = -32768, string poid = "", int punilogcurrcoord = -2147483648, DateTime punilogcurinfodate = default(DateTime), string punilogcurseg = "", double ploco_code = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrain.train_id, tbtrain.name, tbtrain.type, tbtrain.creation_tm, tbtrain.departure_time, tbtrain.arrival_time, tbtrain.direction, tbtrain.priority, tbtrain.status, tbtrain.departure_coordinate, tbtrain.arrival_coordinate, tbtrain.lotes, tbtrain.isvalid, tbtrain.last_coordinate, tbtrain.last_info_updated, tbtrain.pmt_id, tbtrain.OS, tbtrain.plan_id, tbtrain.OSSGF, tbtrain.last_track, tbtrain.hist, tbtrain.cmd_loco_id, tbtrain.usr_cmd_loco_id, tbtrain.plan_id_lock, tbtrain.oid, tbtrain.unilogcurrcoord, tbtrain.unilogcurinfodate, tbtrain.unilogcurseg, tbtrain.loco_code From tbtrain";

		if(ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.train_id=@train_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.train_id=@train_id";
			}
		}

		if(!string.IsNullOrEmpty(pname))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.name like @name";
			}
			else
			{
				lvSqlWhere += " And tbtrain.name like @name";
			}
		}

		if(!string.IsNullOrEmpty(ptype))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.type like @type";
			}
			else
			{
				lvSqlWhere += " And tbtrain.type like @type";
			}
		}

		if(pcreation_tm > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.creation_tm=@creation_tm";
			}
			else
			{
				lvSqlWhere += " And tbtrain.creation_tm=@creation_tm";
			}
		}

		if(pdeparture_time > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_time=@departure_time";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_time=@departure_time";
			}
		}

		if(parrival_time > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_time=@arrival_time";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_time=@arrival_time";
			}
		}

		if(pdirection > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.direction=@direction";
			}
			else
			{
				lvSqlWhere += " And tbtrain.direction=@direction";
			}
		}

		if(!string.IsNullOrEmpty(ppriority))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.priority like @priority";
			}
			else
			{
				lvSqlWhere += " And tbtrain.priority like @priority";
			}
		}

		if(!string.IsNullOrEmpty(pstatus))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.status like @status";
			}
			else
			{
				lvSqlWhere += " And tbtrain.status like @status";
			}
		}

		if(pdeparture_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_coordinate=@departure_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_coordinate=@departure_coordinate";
			}
		}

		if(parrival_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_coordinate=@arrival_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_coordinate=@arrival_coordinate";
			}
		}

		if(plotes > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.lotes=@lotes";
			}
			else
			{
				lvSqlWhere += " And tbtrain.lotes=@lotes";
			}
		}

		if(pisvalid > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.isvalid=@isvalid";
			}
			else
			{
				lvSqlWhere += " And tbtrain.isvalid=@isvalid";
			}
		}

		if(plast_coordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_coordinate=@last_coordinate";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_coordinate=@last_coordinate";
			}
		}

		if(plast_info_updated > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_info_updated=@last_info_updated";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_info_updated=@last_info_updated";
			}
		}

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(pOS))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.OS like @OS";
			}
			else
			{
				lvSqlWhere += " And tbtrain.OS like @OS";
			}
		}

		if(pplan_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id=@plan_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id=@plan_id";
			}
		}

		if(!string.IsNullOrEmpty(pOSSGF))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.OSSGF like @OSSGF";
			}
			else
			{
				lvSqlWhere += " And tbtrain.OSSGF like @OSSGF";
			}
		}

		if(!string.IsNullOrEmpty(plast_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_track like @last_track";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_track like @last_track";
			}
		}

		if(phist > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.hist=@hist";
			}
			else
			{
				lvSqlWhere += " And tbtrain.hist=@hist";
			}
		}

		if(pcmd_loco_id > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.cmd_loco_id=@cmd_loco_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.cmd_loco_id=@cmd_loco_id";
			}
		}

		if(pusr_cmd_loco_id > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.usr_cmd_loco_id=@usr_cmd_loco_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.usr_cmd_loco_id=@usr_cmd_loco_id";
			}
		}

		if(pplan_id_lock > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id_lock=@plan_id_lock";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id_lock=@plan_id_lock";
			}
		}

		if(!string.IsNullOrEmpty(poid))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.oid like @oid";
			}
			else
			{
				lvSqlWhere += " And tbtrain.oid like @oid";
			}
		}

		if(punilogcurrcoord > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurrcoord=@unilogcurrcoord";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurrcoord=@unilogcurrcoord";
			}
		}

		if(punilogcurinfodate > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurinfodate=@unilogcurinfodate";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurinfodate=@unilogcurinfodate";
			}
		}

		if(!string.IsNullOrEmpty(punilogcurseg))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurseg like @unilogcurseg";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurseg like @unilogcurseg";
			}
		}

		if(ploco_code > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.loco_code=@loco_code";
			}
			else
			{
				lvSqlWhere += " And tbtrain.loco_code=@loco_code";
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

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.Parameters.Add("@name", MySqlDbType.String).Value = "%" + pname + "%";

		cmd.Parameters.Add("@type", MySqlDbType.String).Value = "%" + ptype + "%";

		if(pcreation_tm == DateTime.MinValue)
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@creation_tm", MySqlDbType.DateTime).Value = pcreation_tm.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(parrival_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_time", MySqlDbType.DateTime).Value = parrival_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdirection == Int16.MinValue)
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
		}

		cmd.Parameters.Add("@priority", MySqlDbType.String).Value = "%" + ppriority + "%";

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = "%" + pstatus + "%";

		if(pdeparture_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_coordinate", MySqlDbType.Int32).Value = pdeparture_coordinate;
		}

		if(parrival_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_coordinate", MySqlDbType.Int32).Value = parrival_coordinate;
		}

		if(plotes == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@lotes", MySqlDbType.Double).Value = plotes;
		}

		if(pisvalid == Int16.MinValue)
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@isvalid", MySqlDbType.Int16).Value = pisvalid;
		}

		if(plast_coordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_coordinate", MySqlDbType.Int32).Value = plast_coordinate;
		}

		if(plast_info_updated == DateTime.MinValue)
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_info_updated", MySqlDbType.DateTime).Value = plast_info_updated.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = "%" + pOS + "%";

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@OSSGF", MySqlDbType.String).Value = "%" + pOSSGF + "%";

		cmd.Parameters.Add("@last_track", MySqlDbType.String).Value = "%" + plast_track + "%";

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@cmd_loco_id", MySqlDbType.Int16).Value = pcmd_loco_id;
		}

		if(pusr_cmd_loco_id == Int16.MinValue)
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@usr_cmd_loco_id", MySqlDbType.Int16).Value = pusr_cmd_loco_id;
		}

		if(pplan_id_lock == Int16.MinValue)
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id_lock", MySqlDbType.Int16).Value = pplan_id_lock;
		}

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = "%" + poid + "%";

		if(punilogcurrcoord == Int32.MinValue)
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurrcoord", MySqlDbType.Int32).Value = punilogcurrcoord;
		}

		if(punilogcurinfodate == DateTime.MinValue)
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurinfodate", MySqlDbType.DateTime).Value = punilogcurinfodate.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@unilogcurseg", MySqlDbType.String).Value = "%" + punilogcurseg + "%";

		if(ploco_code == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@loco_code", MySqlDbType.Double).Value = ploco_code;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrain");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(double ptrain_idInicio = -999999999999, double ptrain_idFim = -999999999999, string pname = "", string ptype = "", DateTime pcreation_tmInicio = default(DateTime), DateTime pcreation_tmFim = default(DateTime), DateTime pdeparture_timeInicio = default(DateTime), DateTime pdeparture_timeFim = default(DateTime), DateTime parrival_timeInicio = default(DateTime), DateTime parrival_timeFim = default(DateTime), Int16 pdirectionInicio = -32768, Int16 pdirectionFim = -32768, string ppriority = "", string pstatus = "", int pdeparture_coordinateInicio = -2147483648, int pdeparture_coordinateFim = -2147483648, int parrival_coordinateInicio = -2147483648, int parrival_coordinateFim = -2147483648, double plotesInicio = -999999999999, double plotesFim = -999999999999, Int16 pisvalidInicio = -32768, Int16 pisvalidFim = -32768, int plast_coordinateInicio = -2147483648, int plast_coordinateFim = -2147483648, DateTime plast_info_updatedInicio = default(DateTime), DateTime plast_info_updatedFim = default(DateTime), string ppmt_id = "", string pOS = "", double pplan_idInicio = -999999999999, double pplan_idFim = -999999999999, string pOSSGF = "", string plast_track = "", DateTime phistInicio = default(DateTime), DateTime phistFim = default(DateTime), Int16 pcmd_loco_idInicio = -32768, Int16 pcmd_loco_idFim = -32768, Int16 pusr_cmd_loco_idInicio = -32768, Int16 pusr_cmd_loco_idFim = -32768, Int16 pplan_id_lockInicio = -32768, Int16 pplan_id_lockFim = -32768, string poid = "", int punilogcurrcoordInicio = -2147483648, int punilogcurrcoordFim = -2147483648, DateTime punilogcurinfodateInicio = default(DateTime), DateTime punilogcurinfodateFim = default(DateTime), string punilogcurseg = "", double ploco_codeInicio = -999999999999, double ploco_codeFim = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrain.train_id, tbtrain.name, tbtrain.type, tbtrain.creation_tm, tbtrain.departure_time, tbtrain.arrival_time, tbtrain.direction, tbtrain.priority, tbtrain.status, tbtrain.departure_coordinate, tbtrain.arrival_coordinate, tbtrain.lotes, tbtrain.isvalid, tbtrain.last_coordinate, tbtrain.last_info_updated, tbtrain.pmt_id, tbtrain.OS, tbtrain.plan_id, tbtrain.OSSGF, tbtrain.last_track, tbtrain.hist, tbtrain.cmd_loco_id, tbtrain.usr_cmd_loco_id, tbtrain.plan_id_lock, tbtrain.oid, tbtrain.unilogcurrcoord, tbtrain.unilogcurinfodate, tbtrain.unilogcurseg, tbtrain.loco_code From tbtrain";

		if(ptrain_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.train_id >= @train_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.train_id >= @train_idInicio";
			}
		}

		if(ptrain_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.train_id <= @train_idFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.train_id <= @train_idFim";
			}
		}

		if(!string.IsNullOrEmpty(pname))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.name like @name";
			}
			else
			{
				lvSqlWhere += " And tbtrain.name like @name";
			}
		}

		if(!string.IsNullOrEmpty(ptype))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.type like @type";
			}
			else
			{
				lvSqlWhere += " And tbtrain.type like @type";
			}
		}

		if(pcreation_tmInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.creation_tm >= @creation_tmInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.creation_tm >= @creation_tmInicio";
			}
		}

		if(pcreation_tmFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.creation_tm <= @creation_tmFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.creation_tm <= @creation_tmFim";
			}
		}

		if(pdeparture_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_time >= @departure_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_time >= @departure_timeInicio";
			}
		}

		if(pdeparture_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_time <= @departure_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_time <= @departure_timeFim";
			}
		}

		if(parrival_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_time >= @arrival_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_time >= @arrival_timeInicio";
			}
		}

		if(parrival_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_time <= @arrival_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_time <= @arrival_timeFim";
			}
		}

		if(pdirectionInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.direction >= @directionInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.direction >= @directionInicio";
			}
		}

		if(pdirectionFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.direction <= @directionFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.direction <= @directionFim";
			}
		}

		if(!string.IsNullOrEmpty(ppriority))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.priority like @priority";
			}
			else
			{
				lvSqlWhere += " And tbtrain.priority like @priority";
			}
		}

		if(!string.IsNullOrEmpty(pstatus))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.status like @status";
			}
			else
			{
				lvSqlWhere += " And tbtrain.status like @status";
			}
		}

		if(pdeparture_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_coordinate >= @departure_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_coordinate >= @departure_coordinateInicio";
			}
		}

		if(pdeparture_coordinateFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.departure_coordinate <= @departure_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.departure_coordinate <= @departure_coordinateFim";
			}
		}

		if(parrival_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_coordinate >= @arrival_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_coordinate >= @arrival_coordinateInicio";
			}
		}

		if(parrival_coordinateFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.arrival_coordinate <= @arrival_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.arrival_coordinate <= @arrival_coordinateFim";
			}
		}

		if(plotesInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.lotes >= @lotesInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.lotes >= @lotesInicio";
			}
		}

		if(plotesFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.lotes <= @lotesFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.lotes <= @lotesFim";
			}
		}

		if(pisvalidInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.isvalid >= @isvalidInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.isvalid >= @isvalidInicio";
			}
		}

		if(pisvalidFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.isvalid <= @isvalidFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.isvalid <= @isvalidFim";
			}
		}

		if(plast_coordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_coordinate >= @last_coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_coordinate >= @last_coordinateInicio";
			}
		}

		if(plast_coordinateFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_coordinate <= @last_coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_coordinate <= @last_coordinateFim";
			}
		}

		if(plast_info_updatedInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_info_updated >= @last_info_updatedInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_info_updated >= @last_info_updatedInicio";
			}
		}

		if(plast_info_updatedFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_info_updated <= @last_info_updatedFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_info_updated <= @last_info_updatedFim";
			}
		}

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(pOS))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.OS like @OS";
			}
			else
			{
				lvSqlWhere += " And tbtrain.OS like @OS";
			}
		}

		if(pplan_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id >= @plan_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id >= @plan_idInicio";
			}
		}

		if(pplan_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id <= @plan_idFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id <= @plan_idFim";
			}
		}

		if(!string.IsNullOrEmpty(pOSSGF))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.OSSGF like @OSSGF";
			}
			else
			{
				lvSqlWhere += " And tbtrain.OSSGF like @OSSGF";
			}
		}

		if(!string.IsNullOrEmpty(plast_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.last_track like @last_track";
			}
			else
			{
				lvSqlWhere += " And tbtrain.last_track like @last_track";
			}
		}

		if(phistInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.hist >= @histInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.hist >= @histInicio";
			}
		}

		if(phistFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.hist <= @histFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.hist <= @histFim";
			}
		}

		if(pcmd_loco_idInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.cmd_loco_id >= @cmd_loco_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.cmd_loco_id >= @cmd_loco_idInicio";
			}
		}

		if(pcmd_loco_idFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.cmd_loco_id <= @cmd_loco_idFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.cmd_loco_id <= @cmd_loco_idFim";
			}
		}

		if(pusr_cmd_loco_idInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.usr_cmd_loco_id >= @usr_cmd_loco_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.usr_cmd_loco_id >= @usr_cmd_loco_idInicio";
			}
		}

		if(pusr_cmd_loco_idFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.usr_cmd_loco_id <= @usr_cmd_loco_idFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.usr_cmd_loco_id <= @usr_cmd_loco_idFim";
			}
		}

		if(pplan_id_lockInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id_lock >= @plan_id_lockInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id_lock >= @plan_id_lockInicio";
			}
		}

		if(pplan_id_lockFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.plan_id_lock <= @plan_id_lockFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.plan_id_lock <= @plan_id_lockFim";
			}
		}

		if(!string.IsNullOrEmpty(poid))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.oid like @oid";
			}
			else
			{
				lvSqlWhere += " And tbtrain.oid like @oid";
			}
		}

		if(punilogcurrcoordInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurrcoord >= @unilogcurrcoordInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurrcoord >= @unilogcurrcoordInicio";
			}
		}

		if(punilogcurrcoordFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurrcoord <= @unilogcurrcoordFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurrcoord <= @unilogcurrcoordFim";
			}
		}

		if(punilogcurinfodateInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurinfodate >= @unilogcurinfodateInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurinfodate >= @unilogcurinfodateInicio";
			}
		}

		if(punilogcurinfodateFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurinfodate <= @unilogcurinfodateFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurinfodate <= @unilogcurinfodateFim";
			}
		}

		if(!string.IsNullOrEmpty(punilogcurseg))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.unilogcurseg like @unilogcurseg";
			}
			else
			{
				lvSqlWhere += " And tbtrain.unilogcurseg like @unilogcurseg";
			}
		}

		if(ploco_codeInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.loco_code >= @loco_codeInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrain.loco_code >= @loco_codeInicio";
			}
		}

		if(ploco_codeFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.loco_code <= @loco_codeFim";
			}
			else
			{
				lvSqlWhere += " And tbtrain.loco_code <= @loco_codeFim";
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

		if(ptrain_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = ptrain_idInicio;
		}

		if(ptrain_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = ptrain_idFim;
		}

		cmd.Parameters.Add("@name", MySqlDbType.String).Value = "%" + pname + "%";

		cmd.Parameters.Add("@type", MySqlDbType.String).Value = "%" + ptype + "%";

		if(pcreation_tmInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@creation_tmInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@creation_tmInicio", MySqlDbType.DateTime).Value = pcreation_tmInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcreation_tmFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@creation_tmFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@creation_tmFim", MySqlDbType.DateTime).Value = pcreation_tmFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdeparture_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_timeInicio", MySqlDbType.DateTime).Value = pdeparture_timeInicio;
		}

		if(pdeparture_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_timeFim", MySqlDbType.DateTime).Value = pdeparture_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(parrival_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@arrival_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_timeInicio", MySqlDbType.DateTime).Value = parrival_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(parrival_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@arrival_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_timeFim", MySqlDbType.DateTime).Value = parrival_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

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

		cmd.Parameters.Add("@priority", MySqlDbType.String).Value = "%" + ppriority + "%";

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = "%" + pstatus + "%";

		if(pdeparture_coordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@departure_coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_coordinateInicio", MySqlDbType.Int32).Value = pdeparture_coordinateInicio;
		}

		if(pdeparture_coordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@departure_coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_coordinateFim", MySqlDbType.Int32).Value = pdeparture_coordinateFim;
		}

		if(parrival_coordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@arrival_coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_coordinateInicio", MySqlDbType.Int32).Value = parrival_coordinateInicio;
		}

		if(parrival_coordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@arrival_coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@arrival_coordinateFim", MySqlDbType.Int32).Value = parrival_coordinateFim;
		}

		if(plotesInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@lotesInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@lotesInicio", MySqlDbType.Double).Value = plotesInicio;
		}

		if(plotesFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@lotesFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@lotesFim", MySqlDbType.Double).Value = plotesFim;
		}

		if(pisvalidInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@isvalidInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@isvalidInicio", MySqlDbType.Int16).Value = pisvalidInicio;
		}

		if(pisvalidFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@isvalidFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@isvalidFim", MySqlDbType.Int16).Value = pisvalidFim;
		}

		if(plast_coordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@last_coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_coordinateInicio", MySqlDbType.Int32).Value = plast_coordinateInicio;
		}

		if(plast_coordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@last_coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_coordinateFim", MySqlDbType.Int32).Value = plast_coordinateFim;
		}

		if(plast_info_updatedInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@last_info_updatedInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_info_updatedInicio", MySqlDbType.DateTime).Value = plast_info_updatedInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(plast_info_updatedFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@last_info_updatedFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@last_info_updatedFim", MySqlDbType.DateTime).Value = plast_info_updatedFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = "%" + pOS + "%";

		if(pplan_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_idInicio", MySqlDbType.Double).Value = pplan_idInicio;
		}

		if(pplan_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_idFim", MySqlDbType.Double).Value = pplan_idFim;
		}

		cmd.Parameters.Add("@OSSGF", MySqlDbType.String).Value = "%" + pOSSGF + "%";

		cmd.Parameters.Add("@last_track", MySqlDbType.String).Value = "%" + plast_track + "%";

		if(phistInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = phistInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phistFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = phistFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcmd_loco_idInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@cmd_loco_idInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@cmd_loco_idInicio", MySqlDbType.Int16).Value = pcmd_loco_idInicio;
		}

		if(pcmd_loco_idFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@cmd_loco_idFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@cmd_loco_idFim", MySqlDbType.Int16).Value = pcmd_loco_idFim;
		}

		if(pusr_cmd_loco_idInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@usr_cmd_loco_idInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@usr_cmd_loco_idInicio", MySqlDbType.Int16).Value = pusr_cmd_loco_idInicio;
		}

		if(pusr_cmd_loco_idFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@usr_cmd_loco_idFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@usr_cmd_loco_idFim", MySqlDbType.Int16).Value = pusr_cmd_loco_idFim;
		}

		if(pplan_id_lockInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@plan_id_lockInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id_lockInicio", MySqlDbType.Int16).Value = pplan_id_lockInicio;
		}

		if(pplan_id_lockFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@plan_id_lockFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id_lockFim", MySqlDbType.Int16).Value = pplan_id_lockFim;
		}

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = "%" + poid + "%";

		if(punilogcurrcoordInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@unilogcurrcoordInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurrcoordInicio", MySqlDbType.Int32).Value = punilogcurrcoordInicio;
		}

		if(punilogcurrcoordFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@unilogcurrcoordFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurrcoordFim", MySqlDbType.Int32).Value = punilogcurrcoordFim;
		}

		if(punilogcurinfodateInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@unilogcurinfodateInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurinfodateInicio", MySqlDbType.DateTime).Value = punilogcurinfodateInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(punilogcurinfodateFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@unilogcurinfodateFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@unilogcurinfodateFim", MySqlDbType.DateTime).Value = punilogcurinfodateFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@unilogcurseg", MySqlDbType.String).Value = "%" + punilogcurseg + "%";

		if(ploco_codeInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@loco_codeInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@loco_codeInicio", MySqlDbType.Double).Value = ploco_codeInicio;
		}

		if(ploco_codeFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@loco_codeFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@loco_codeFim", MySqlDbType.Double).Value = ploco_codeFim;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrain");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(double ptrain_id = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrain.train_id, tbtrain.name, tbtrain.type, tbtrain.creation_tm, tbtrain.departure_time, tbtrain.arrival_time, tbtrain.direction, tbtrain.priority, tbtrain.status, tbtrain.departure_coordinate, tbtrain.arrival_coordinate, tbtrain.lotes, tbtrain.isvalid, tbtrain.last_coordinate, tbtrain.last_info_updated, tbtrain.pmt_id, tbtrain.OS, tbtrain.plan_id, tbtrain.OSSGF, tbtrain.last_track, tbtrain.hist, tbtrain.cmd_loco_id, tbtrain.usr_cmd_loco_id, tbtrain.plan_id_lock, tbtrain.oid, tbtrain.unilogcurrcoord, tbtrain.unilogcurinfodate, tbtrain.unilogcurseg, tbtrain.loco_code From tbtrain";
		if(ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrain.train_id=@train_id";
			}
			else
			{
				lvSqlWhere += " And tbtrain.train_id=@train_id";
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

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrain");
		return ds;
	}

}

