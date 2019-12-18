using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
/// <summary>
/// Criado por Eggo Pinheiro em 13/02/2015 14:03:59
/// <summary>

public class Train
{

	protected Int64 lvtrain_id;
	protected string lvname;
	protected string lvtype;
	protected DateTime lvcreation_tm;
	protected DateTime lvdeparture_time;
	protected DateTime lvarrival_time;
	protected Int16 lvdirection;
	protected string lvpriority;
	protected string lvstatus;
	protected int lvdeparture_coordinate;
	protected int lvarrival_coordinate;
	protected double lvlotes;
	protected Int16 lvisvalid;
	protected int lvlast_coordinate;
	protected DateTime lvlast_info_updated;
	protected string lvpmt_id;
	protected string lvOS;
	protected double lvplan_id;
	protected string lvOSSGF;
	protected string lvlast_track;
	protected DateTime lvhist;
	protected Int16 lvcmd_loco_id;
	protected Int16 lvusr_cmd_loco_id;
	protected Int16 lvplan_id_lock;
	protected string lvoid;
	protected int lvunilogcurrcoord;
	protected DateTime lvunilogcurinfodate;
	protected string lvunilogcurseg;
	protected double lvloco_code;

    public static string GetColorByTrainType(string type)
    {
        string lvRes = "green";

        if (ConfigurationManager.AppSettings[type + "_TYPE"] != null)
        {
            lvRes = ConfigurationManager.AppSettings[type + "_TYPE"];
        }

        return lvRes;
    }

	public Train()
	{

		Clear();
	}

	public Train(Int64 train_id)
	{

		Clear();

		this.lvtrain_id = train_id;
		Load();
	}

	public Train(Int64 train_id, string name, string type, DateTime creation_tm, DateTime departure_time, DateTime arrival_time, Int16 direction, string priority, string status, int departure_coordinate, int arrival_coordinate, double lotes, Int16 isvalid, int last_coordinate, DateTime last_info_updated, string pmt_id, string OS, double plan_id, string OSSGF, string last_track, DateTime hist, Int16 cmd_loco_id, Int16 usr_cmd_loco_id, Int16 plan_id_lock, string oid, int unilogcurrcoord, DateTime unilogcurinfodate, string unilogcurseg, double loco_code)
	{

		this.lvtrain_id = train_id;
		this.lvname = name;
		this.lvtype = type;
		this.lvcreation_tm = creation_tm;
		this.lvdeparture_time = departure_time;
		this.lvarrival_time = arrival_time;
		this.lvdirection = direction;
		this.lvpriority = priority;
		this.lvstatus = status;
		this.lvdeparture_coordinate = departure_coordinate;
		this.lvarrival_coordinate = arrival_coordinate;
		this.lvlotes = lotes;
		this.lvisvalid = isvalid;
		this.lvlast_coordinate = last_coordinate;
		this.lvlast_info_updated = last_info_updated;
		this.lvpmt_id = pmt_id;
		this.lvOS = OS;
		this.lvplan_id = plan_id;
		this.lvOSSGF = OSSGF;
		this.lvlast_track = last_track;
		this.lvhist = hist;
		this.lvcmd_loco_id = cmd_loco_id;
		this.lvusr_cmd_loco_id = usr_cmd_loco_id;
		this.lvplan_id_lock = plan_id_lock;
		this.lvoid = oid;
		this.lvunilogcurrcoord = unilogcurrcoord;
		this.lvunilogcurinfodate = unilogcurinfodate;
		this.lvunilogcurseg = unilogcurseg;
		this.lvloco_code = loco_code;
	}

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = TrainDataAccess.GetDataByKey(this.lvtrain_id, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvtrain_id = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
			this.lvname = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());
			this.lvtype = ((row["type"] == DBNull.Value) ? "" : row["type"].ToString());
			this.lvcreation_tm = ((row["creation_tm"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["creation_tm"].ToString()));
			this.lvdeparture_time = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
			this.lvarrival_time = ((row["arrival_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["arrival_time"].ToString()));
			this.lvdirection = ((row["direction"] == DBNull.Value) ? Int16.MinValue : (Int16)row["direction"]);
			this.lvpriority = ((row["priority"] == DBNull.Value) ? "" : row["priority"].ToString());
			this.lvstatus = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
			this.lvdeparture_coordinate = ((row["departure_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["departure_coordinate"]);
			this.lvarrival_coordinate = ((row["arrival_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["arrival_coordinate"]);
			this.lvlotes = ((row["lotes"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["lotes"]);
			this.lvisvalid = ((row["isvalid"] == DBNull.Value) ? Int16.MinValue : (Int16)row["isvalid"]);
			this.lvlast_coordinate = ((row["last_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["last_coordinate"]);
			this.lvlast_info_updated = ((row["last_info_updated"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["last_info_updated"].ToString()));
			this.lvpmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			this.lvOS = ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString());
			this.lvplan_id = ((row["plan_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["plan_id"]);
			this.lvOSSGF = ((row["OSSGF"] == DBNull.Value) ? "" : row["OSSGF"].ToString());
			this.lvlast_track = ((row["last_track"] == DBNull.Value) ? "" : row["last_track"].ToString());
			this.lvhist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));
			this.lvcmd_loco_id = ((row["cmd_loco_id"] == DBNull.Value) ? Int16.MinValue : (Int16)row["cmd_loco_id"]);
			this.lvusr_cmd_loco_id = ((row["usr_cmd_loco_id"] == DBNull.Value) ? Int16.MinValue : (Int16)row["usr_cmd_loco_id"]);
			this.lvplan_id_lock = ((row["plan_id_lock"] == DBNull.Value) ? Int16.MinValue : (Int16)row["plan_id_lock"]);
			this.lvoid = ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString());
			this.lvunilogcurrcoord = ((row["unilogcurrcoord"] == DBNull.Value) ? Int32.MinValue : (int)row["unilogcurrcoord"]);
			this.lvunilogcurinfodate = ((row["unilogcurinfodate"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["unilogcurinfodate"].ToString()));
			this.lvunilogcurseg = ((row["unilogcurseg"] == DBNull.Value) ? "" : row["unilogcurseg"].ToString());
			this.lvloco_code = ((row["loco_code"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["loco_code"]);

			lvResult = true;
		}

		return lvResult;
	}

	public string GetFlotSerie(string pStrColor, string pStrLabel, string pStrXAxisName, string pStrYAxisName, string pStrIdent, Boolean isDashed, string pStrSymbol)
	{
		string lvResult = "";
		DataSet ds = null;
		string lvXValues = "";
		string lvYValues = "";
		Boolean lvHasElement = false;

		ds = TrainDataAccess.GetData(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code, "");

		if (String.IsNullOrEmpty(pStrSymbol))
		{
			if(isDashed)
			{
				lvResult = "{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [";
			}
			else
			{
				lvResult = "{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": false, \"radius\": 2}, \"lines\": {\"show\": true, \"lineWidth\": 3}, \"data\": [";
			}
		}
		else
		{
				lvResult = "{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 2, \"symbol\": \"" + pStrSymbol + "\"}, \"data\": [";
		}

		foreach (DataRow row in ds.Tables[0].Rows)
		{

			if(row[pStrXAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
				lvXValues = ConnectionManager.GetUTCDateTime((row[pStrXAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrXAxisName.Trim()].ToString())).ToString();
			}
			else
			{
				lvXValues = ((row[pStrXAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrXAxisName.Trim()].ToString());
			}

			if(row[pStrYAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
				lvYValues = ConnectionManager.GetUTCDateTime((row[pStrYAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrYAxisName.Trim()].ToString())).ToString();
			}
			else
			{
				lvYValues = ((row[pStrYAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrYAxisName.Trim()].ToString());
			}

			lvXValues = lvXValues.Replace(",", ".");
			lvYValues = lvYValues.Replace(",", ".");

			if(lvHasElement)
			{
				lvResult += ", [" + lvXValues + ", " + lvYValues + "]";
			}
			else
			{
				lvResult += "[" + lvXValues + ", " + lvYValues + "]";
				lvHasElement = true;
			}
		}

		lvResult += "]}";


		return lvResult;
	}

	public string GetFlotClass(string pStrLabel)
	{
		string lvResult = "";
		DataSet ds = null;
		string lvStrFlotClass = "";
		string lvStrVector = "";
		int lvLabelLen = -1;
		Boolean lvHasElement = false;

		lvLabelLen = pStrLabel.IndexOf(".");
		if(lvLabelLen > -1)
		{
			lvStrVector = pStrLabel.Substring(0, lvLabelLen).Trim();
		}
		else
		{
			lvStrVector = pStrLabel.Trim();
		}

		if(lvStrVector.Length == 0)
		{
			return lvResult;
		}

		ds = TrainDataAccess.GetData(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code, "");

		lvResult = "var " + lvStrVector + " = [";

		foreach (DataRow row in ds.Tables[0].Rows)
		{

			if(lvHasElement)
			{
				lvStrFlotClass = ", {";
			}
			else
			{
				lvStrFlotClass = "{";
				lvHasElement = true;
			}

			lvStrFlotClass += "Train_id: " + ((row["train_id"] == DBNull.Value) ? "\"\"" : row["train_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Name: \"" + ((row["name"] == DBNull.Value) ? "" : row["name"].ToString()) + "\", ";
			lvStrFlotClass += "Type: \"" + ((row["type"] == DBNull.Value) ? "" : row["type"].ToString()) + "\", ";
			lvStrFlotClass += "Creation_tm: \"" + ((row["creation_tm"] == DBNull.Value) ? "" : DateTime.Parse(row["creation_tm"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Departure_time: \"" + ((row["departure_time"] == DBNull.Value) ? "" : DateTime.Parse(row["departure_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Arrival_time: \"" + ((row["arrival_time"] == DBNull.Value) ? "" : DateTime.Parse(row["arrival_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Direction: " + ((row["direction"] == DBNull.Value) ? "\"\"" : row["direction"].ToString()) + ", ";
			lvStrFlotClass += "Priority: \"" + ((row["priority"] == DBNull.Value) ? "" : row["priority"].ToString()) + "\", ";
			lvStrFlotClass += "Status: \"" + ((row["status"] == DBNull.Value) ? "" : row["status"].ToString()) + "\", ";
			lvStrFlotClass += "Departure_coordinate: " + ((row["departure_coordinate"] == DBNull.Value) ? "\"\"" : row["departure_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Arrival_coordinate: " + ((row["arrival_coordinate"] == DBNull.Value) ? "\"\"" : row["arrival_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Lotes: " + ((row["lotes"] == DBNull.Value) ? "\"\"" : row["lotes"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Isvalid: " + ((row["isvalid"] == DBNull.Value) ? "\"\"" : row["isvalid"].ToString()) + ", ";
			lvStrFlotClass += "Last_coordinate: " + ((row["last_coordinate"] == DBNull.Value) ? "\"\"" : row["last_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Last_info_updated: \"" + ((row["last_info_updated"] == DBNull.Value) ? "" : DateTime.Parse(row["last_info_updated"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Pmt_id: \"" + ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString()) + "\", ";
			lvStrFlotClass += "OS: \"" + ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString()) + "\", ";
			lvStrFlotClass += "Plan_id: " + ((row["plan_id"] == DBNull.Value) ? "\"\"" : row["plan_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "OSSGF: \"" + ((row["OSSGF"] == DBNull.Value) ? "" : row["OSSGF"].ToString()) + "\", ";
			lvStrFlotClass += "Last_track: \"" + ((row["last_track"] == DBNull.Value) ? "" : row["last_track"].ToString()) + "\", ";
			lvStrFlotClass += "Hist: \"" + ((row["hist"] == DBNull.Value) ? "" : DateTime.Parse(row["hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Cmd_loco_id: " + ((row["cmd_loco_id"] == DBNull.Value) ? "\"\"" : row["cmd_loco_id"].ToString()) + ", ";
			lvStrFlotClass += "Usr_cmd_loco_id: " + ((row["usr_cmd_loco_id"] == DBNull.Value) ? "\"\"" : row["usr_cmd_loco_id"].ToString()) + ", ";
			lvStrFlotClass += "Plan_id_lock: " + ((row["plan_id_lock"] == DBNull.Value) ? "\"\"" : row["plan_id_lock"].ToString()) + ", ";
			lvStrFlotClass += "Oid: \"" + ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString()) + "\", ";
			lvStrFlotClass += "Unilogcurrcoord: " + ((row["unilogcurrcoord"] == DBNull.Value) ? "\"\"" : row["unilogcurrcoord"].ToString()) + ", ";
			lvStrFlotClass += "Unilogcurinfodate: \"" + ((row["unilogcurinfodate"] == DBNull.Value) ? "" : DateTime.Parse(row["unilogcurinfodate"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Unilogcurseg: \"" + ((row["unilogcurseg"] == DBNull.Value) ? "" : row["unilogcurseg"].ToString()) + "\", ";
			lvStrFlotClass += "Loco_code: " + ((row["loco_code"] == DBNull.Value) ? "\"\"" : row["loco_code"].ToString().Replace(",", ".")) + ", ";
			if(lvStrFlotClass.LastIndexOf(",") == lvStrFlotClass.Length - 2)
			{
				lvStrFlotClass = lvStrFlotClass.Substring(0, lvStrFlotClass.Length - 2);
			}

			lvStrFlotClass += "}";

			lvResult += lvStrFlotClass + " \n ";
		}

		lvResult += "]; \n\n";

		return lvResult;
	}

	public List<Train> GetList()
	{
		List<Train> lvResult = new List<Train>();
		DataSet ds = null;
		Train lvElement = null;

		ds = TrainDataAccess.GetData(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Train();

			lvElement.Train_id = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
			lvElement.Name = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());
			lvElement.Type = ((row["type"] == DBNull.Value) ? "" : row["type"].ToString());
			lvElement.Creation_tm = ((row["creation_tm"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["creation_tm"].ToString()));
			lvElement.Departure_time = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
			lvElement.Arrival_time = ((row["arrival_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["arrival_time"].ToString()));
			lvElement.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : (Int16)row["direction"]);
			lvElement.Priority = ((row["priority"] == DBNull.Value) ? "" : row["priority"].ToString());
			lvElement.Status = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
			lvElement.Departure_coordinate = ((row["departure_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["departure_coordinate"]);
			lvElement.Arrival_coordinate = ((row["arrival_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["arrival_coordinate"]);
			lvElement.Lotes = ((row["lotes"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["lotes"]);
			lvElement.Isvalid = ((row["isvalid"] == DBNull.Value) ? Int16.MinValue : (Int16)row["isvalid"]);
			lvElement.Last_coordinate = ((row["last_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["last_coordinate"]);
			lvElement.Last_info_updated = ((row["last_info_updated"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["last_info_updated"].ToString()));
			lvElement.Pmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			lvElement.OS = ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString());
			lvElement.Plan_id = ((row["plan_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["plan_id"]);
			lvElement.OSSGF = ((row["OSSGF"] == DBNull.Value) ? "" : row["OSSGF"].ToString());
			lvElement.Last_track = ((row["last_track"] == DBNull.Value) ? "" : row["last_track"].ToString());
			lvElement.Hist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));
			lvElement.Cmd_loco_id = ((row["cmd_loco_id"] == DBNull.Value) ? Int16.MinValue : (Int16)row["cmd_loco_id"]);
			lvElement.Usr_cmd_loco_id = ((row["usr_cmd_loco_id"] == DBNull.Value) ? Int16.MinValue : (Int16)row["usr_cmd_loco_id"]);
			lvElement.Plan_id_lock = ((row["plan_id_lock"] == DBNull.Value) ? Int16.MinValue : (Int16)row["plan_id_lock"]);
			lvElement.Oid = ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString());
			lvElement.Unilogcurrcoord = ((row["unilogcurrcoord"] == DBNull.Value) ? Int32.MinValue : (int)row["unilogcurrcoord"]);
			lvElement.Unilogcurinfodate = ((row["unilogcurinfodate"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["unilogcurinfodate"].ToString()));
			lvElement.Unilogcurseg = ((row["unilogcurseg"] == DBNull.Value) ? "" : row["unilogcurseg"].ToString());
			lvElement.Loco_code = ((row["loco_code"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["loco_code"]);

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

        this.lvtrain_id = Int64.MinValue;
		this.lvname = "";
		this.lvtype = "";
		this.lvcreation_tm = DateTime.MinValue;
		this.lvdeparture_time = DateTime.MinValue;
		this.lvarrival_time = DateTime.MinValue;
		this.lvdirection = Int16.MinValue;
		this.lvpriority = "";
		this.lvstatus = "";
		this.lvdeparture_coordinate = Int32.MinValue;
		this.lvarrival_coordinate = Int32.MinValue;
		this.lvlotes = ConnectionManager.DOUBLE_REF_VALUE;
		this.lvisvalid = Int16.MinValue;
		this.lvlast_coordinate = Int32.MinValue;
		this.lvlast_info_updated = DateTime.MinValue;
		this.lvpmt_id = "";
		this.lvOS = "";
		this.lvplan_id = ConnectionManager.DOUBLE_REF_VALUE;
		this.lvOSSGF = "";
		this.lvlast_track = "";
		this.lvhist = DateTime.MinValue;
		this.lvcmd_loco_id = Int16.MinValue;
		this.lvusr_cmd_loco_id = Int16.MinValue;
		this.lvplan_id_lock = Int16.MinValue;
		this.lvoid = "";
		this.lvunilogcurrcoord = Int32.MinValue;
		this.lvunilogcurinfodate = DateTime.MinValue;
		this.lvunilogcurseg = "";
		this.lvloco_code = ConnectionManager.DOUBLE_REF_VALUE;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainDataAccess.Insert(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public virtual bool Update()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainDataAccess.Update(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public bool UpdateKey(double train_id)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = TrainDataAccess.UpdateKey(this.lvtrain_id, train_id);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public virtual bool Delete()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainDataAccess.Delete(this.lvtrain_id);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public DataTable GetData()
	{
		DataTable dt = null;
		DataSet ds = null;

		ds = TrainDataAccess.GetData(this.lvtrain_id, this.lvname, this.lvtype, this.lvcreation_tm, this.lvdeparture_time, this.lvarrival_time, this.lvdirection, this.lvpriority, this.lvstatus, this.lvdeparture_coordinate, this.lvarrival_coordinate, this.lvlotes, this.lvisvalid, this.lvlast_coordinate, this.lvlast_info_updated, this.lvpmt_id, this.lvOS, this.lvplan_id, this.lvOSSGF, this.lvlast_track, this.lvhist, this.lvcmd_loco_id, this.lvusr_cmd_loco_id, this.lvplan_id_lock, this.lvoid, this.lvunilogcurrcoord, this.lvunilogcurinfodate, this.lvunilogcurseg, this.lvloco_code, "");

		dt = ds.Tables[0];

		return dt;
	}

	public Int64 Train_id
	{
		get
		{
			return this.lvtrain_id;
		}
		set
		{
			this.lvtrain_id = value;
		}
	}

	public string Name
	{
		get
		{
			return this.lvname;
		}
		set
		{
			this.lvname = value;
		}
	}

	public string Type
	{
		get
		{
			return this.lvtype;
		}
		set
		{
			this.lvtype = value;
		}
	}

	public DateTime Creation_tm
	{
		get
		{
			return this.lvcreation_tm;
		}
		set
		{
			this.lvcreation_tm = value;
		}
	}

	public DateTime Departure_time
	{
		get
		{
			return this.lvdeparture_time;
		}
		set
		{
			this.lvdeparture_time = value;
		}
	}

	public DateTime Arrival_time
	{
		get
		{
			return this.lvarrival_time;
		}
		set
		{
			this.lvarrival_time = value;
		}
	}

	public Int16 Direction
	{
		get
		{
			return this.lvdirection;
		}
		set
		{
			this.lvdirection = value;
		}
	}

	public string Priority
	{
		get
		{
			return this.lvpriority;
		}
		set
		{
			this.lvpriority = value;
		}
	}

	public string Status
	{
		get
		{
			return this.lvstatus;
		}
		set
		{
			this.lvstatus = value;
		}
	}

	public int Departure_coordinate
	{
		get
		{
			return this.lvdeparture_coordinate;
		}
		set
		{
			this.lvdeparture_coordinate = value;
		}
	}

	public int Arrival_coordinate
	{
		get
		{
			return this.lvarrival_coordinate;
		}
		set
		{
			this.lvarrival_coordinate = value;
		}
	}

	public double Lotes
	{
		get
		{
			return this.lvlotes;
		}
		set
		{
			this.lvlotes = value;
		}
	}

	public Int16 Isvalid
	{
		get
		{
			return this.lvisvalid;
		}
		set
		{
			this.lvisvalid = value;
		}
	}

	public int Last_coordinate
	{
		get
		{
			return this.lvlast_coordinate;
		}
		set
		{
			this.lvlast_coordinate = value;
		}
	}

	public DateTime Last_info_updated
	{
		get
		{
			return this.lvlast_info_updated;
		}
		set
		{
			this.lvlast_info_updated = value;
		}
	}

	public string Pmt_id
	{
		get
		{
			return this.lvpmt_id;
		}
		set
		{
			this.lvpmt_id = value;
		}
	}

	public string OS
	{
		get
		{
			return this.lvOS;
		}
		set
		{
			this.lvOS = value;
		}
	}

	public double Plan_id
	{
		get
		{
			return this.lvplan_id;
		}
		set
		{
			this.lvplan_id = value;
		}
	}

	public string OSSGF
	{
		get
		{
			return this.lvOSSGF;
		}
		set
		{
			this.lvOSSGF = value;
		}
	}

	public string Last_track
	{
		get
		{
			return this.lvlast_track;
		}
		set
		{
			this.lvlast_track = value;
		}
	}

	public DateTime Hist
	{
		get
		{
			return this.lvhist;
		}
		set
		{
			this.lvhist = value;
		}
	}

	public Int16 Cmd_loco_id
	{
		get
		{
			return this.lvcmd_loco_id;
		}
		set
		{
			this.lvcmd_loco_id = value;
		}
	}

	public Int16 Usr_cmd_loco_id
	{
		get
		{
			return this.lvusr_cmd_loco_id;
		}
		set
		{
			this.lvusr_cmd_loco_id = value;
		}
	}

	public Int16 Plan_id_lock
	{
		get
		{
			return this.lvplan_id_lock;
		}
		set
		{
			this.lvplan_id_lock = value;
		}
	}

	public string Oid
	{
		get
		{
			return this.lvoid;
		}
		set
		{
			this.lvoid = value;
		}
	}

	public int Unilogcurrcoord
	{
		get
		{
			return this.lvunilogcurrcoord;
		}
		set
		{
			this.lvunilogcurrcoord = value;
		}
	}

	public DateTime Unilogcurinfodate
	{
		get
		{
			return this.lvunilogcurinfodate;
		}
		set
		{
			this.lvunilogcurinfodate = value;
		}
	}

	public string Unilogcurseg
	{
		get
		{
			return this.lvunilogcurseg;
		}
		set
		{
			this.lvunilogcurseg = value;
		}
	}

	public double Loco_code
	{
		get
		{
			return this.lvloco_code;
		}
		set
		{
			this.lvloco_code = value;
		}
	}


	public DataTable GetDataOfTrainmovsegment(double train_id, Boolean pUseForeignKey = false, string pStrOrderBy = "")
	{
		DataTable dt = null;
		DataSet ds = null;

		ds = TrainmovsegmentDataAccess.GetDataByTrain(train_id, pUseForeignKey, pStrOrderBy);

		dt = ds.Tables[0];

		return dt;
	}


}

