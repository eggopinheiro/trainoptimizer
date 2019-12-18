using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 20:59:43
/// <summary>

public class Plan
{
	protected double lvplan_id;
	protected string lvtrain_name;
	protected int lvorigem;
	protected int lvdestino;
	protected DateTime lvdeparture_time;
	protected DateTime lvhist;
	protected string lvoid;

	public Plan()
	{
		Clear();
	}

	public Plan(double plan_id)
	{
		Clear();

		this.lvplan_id = plan_id;
		Load();
	}

	public Plan(double plan_id, string train_name, int origem, int destino, DateTime departure_time, DateTime hist, string pmt_id, string oid)
	{
		this.lvplan_id = plan_id;
		this.lvtrain_name = train_name;
		this.lvorigem = origem;
		this.lvdestino = destino;
		this.lvdeparture_time = departure_time;
		this.lvhist = hist;
		this.lvoid = oid;
	}

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = PlanDataAccess.GetDataByKey(this.lvplan_id, false, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvplan_id = ((row["plan_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["plan_id"]);
			this.lvtrain_name = ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString());
			this.lvorigem = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
			this.lvdestino = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
			this.lvdeparture_time = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
			this.lvhist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));
			this.lvoid = ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString());

			lvResult = true;
		}

		return lvResult;
	}

	public string GetFlotSerie(string pStrColor, string pStrLabel, string pStrXAxisName, string pStrYAxisName, string pStrIdent, Boolean isDashed, string pStrSymbol)
	{
		StringBuilder lvResult = new StringBuilder();
		DataSet ds = null;
		string lvXValues = "";
		string lvYValues = "";
		Boolean lvHasElement = false;

		ds = PlanDataAccess.GetData(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, "", this.lvoid, false, "");

		if (String.IsNullOrEmpty(pStrSymbol))
		{
			if(isDashed)
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");
			}
			else
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": false, \"radius\": 2}, \"lines\": {\"show\": true, \"lineWidth\": 3}, \"data\": [");
			}
		}
		else
		{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 2, \"symbol\": \"" + pStrSymbol + "\"}, \"data\": [");
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
				lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
			}
			else
			{
				lvResult.Append("[" + lvXValues + ", " + lvYValues + "]");
				lvHasElement = true;
			}
		}

		lvResult.Append("]}");


		return lvResult.ToString();
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

		ds = PlanDataAccess.GetData(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, "", this.lvoid, false, "");

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

			lvStrFlotClass += "Plan_id: " + ((row["plan_id"] == DBNull.Value) ? "\"\"" : row["plan_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Train_name: \"" + ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString()) + "\", ";
			lvStrFlotClass += "Origem: " + ((row["origem"] == DBNull.Value) ? "\"\"" : row["origem"].ToString()) + ", ";
			lvStrFlotClass += "Destino: " + ((row["destino"] == DBNull.Value) ? "\"\"" : row["destino"].ToString()) + ", ";
			lvStrFlotClass += "Departure_time: \"" + ((row["departure_time"] == DBNull.Value) ? "" : DateTime.Parse(row["departure_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Hist: \"" + ((row["hist"] == DBNull.Value) ? "" : DateTime.Parse(row["hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Pmt_id: \"" + ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString()) + "\", ";
			lvStrFlotClass += "Oid: \"" + ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString()) + "\", ";
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

	public List<Plan> GetList()
	{
		List<Plan> lvResult = new List<Plan>();
		DataSet ds = null;
		Plan lvElement = null;

        ds = PlanDataAccess.GetData(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, this.lvoid, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Plan();

			lvElement.Plan_id = ((row["plan_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["plan_id"]);
			lvElement.Train_name = ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString());
			lvElement.Origem = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
			lvElement.Destino = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
			lvElement.Departure_time = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
			lvElement.Hist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));
			lvElement.Oid = ((row["oid"] == DBNull.Value) ? "" : row["oid"].ToString());

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvplan_id = ConnectionManager.DOUBLE_REF_VALUE;
		this.lvtrain_name = "";
		this.lvorigem = Int32.MinValue;
		this.lvdestino = Int32.MinValue;
		this.lvdeparture_time = DateTime.MinValue;
		this.lvhist = DateTime.MinValue;
		this.lvoid = "";
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = PlanDataAccess.Insert(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, "", this.lvoid);

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
			lvRowsAffect = PlanDataAccess.Update(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, "", this.lvoid);

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

	public bool UpdateKey(double plan_id)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = PlanDataAccess.UpdateKey(this.lvplan_id, plan_id);

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
			lvRowsAffect = PlanDataAccess.Delete(this.lvplan_id);

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

		ds = PlanDataAccess.GetData(this.lvplan_id, this.lvtrain_name, this.lvorigem, this.lvdestino, this.lvdeparture_time, this.lvhist, "", this.lvoid, false, "");

		dt = ds.Tables[0];

		return dt;
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

	public string Train_name
	{
		get
		{
			return this.lvtrain_name;
		}
		set
		{
			this.lvtrain_name = value;
		}
	}

	public int Origem
	{
		get
		{
			return this.lvorigem;
		}
		set
		{
			this.lvorigem = value;
		}
	}

	public int Destino
	{
		get
		{
			return this.lvdestino;
		}
		set
		{
			this.lvdestino = value;
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
}

