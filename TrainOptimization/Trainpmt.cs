using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 20:59:40
/// <summary>

public class Trainpmt
{

	protected string lvpmt_id;
	protected string lvprefix;
	protected DateTime lvdate_hist;
	protected DateTime lvprev_part;
	protected Int16 lvKMOrigem;
	protected Int16 lvKMDestino;
	protected Int16 lvsentflag;
	protected string lvOS;

	public Trainpmt()
	{

		Clear();
	}

	public Trainpmt(string pmt_id, string prefix)
	{

		Clear();

		this.lvpmt_id = pmt_id;
		this.lvprefix = prefix;
		Load();
	}

	public Trainpmt(string pmt_id, string prefix, DateTime date_hist, DateTime prev_part, Int16 KMOrigem, Int16 KMDestino, Int16 sentflag, string OS)
	{

		this.lvpmt_id = pmt_id;
		this.lvprefix = prefix;
		this.lvdate_hist = date_hist;
		this.lvprev_part = prev_part;
		this.lvKMOrigem = KMOrigem;
		this.lvKMDestino = KMDestino;
		this.lvsentflag = sentflag;
		this.lvOS = OS;
	}

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = TrainpmtDataAccess.GetDataByKey(this.lvpmt_id, this.lvprefix, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvpmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			this.lvprefix = ((row["prefix"] == DBNull.Value) ? "" : row["prefix"].ToString());
			this.lvdate_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));
			this.lvprev_part = ((row["prev_part"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["prev_part"].ToString()));
			this.lvKMOrigem = ((row["KMOrigem"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMOrigem"]);
			this.lvKMDestino = ((row["KMDestino"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMDestino"]);
			this.lvsentflag = ((row["sentflag"] == DBNull.Value) ? Int16.MinValue : (Int16)row["sentflag"]);
			this.lvOS = ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString());

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

		ds = TrainpmtDataAccess.GetData(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS, "");

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

		lvResult.Append("]}; \n\n");


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

		ds = TrainpmtDataAccess.GetData(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS, "");

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

			lvStrFlotClass += "Pmt_id: \"" + ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString()) + "\", ";
			lvStrFlotClass += "Prefix: \"" + ((row["prefix"] == DBNull.Value) ? "" : row["prefix"].ToString()) + "\", ";
			lvStrFlotClass += "Date_hist: \"" + ((row["date_hist"] == DBNull.Value) ? "" : DateTime.Parse(row["date_hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Prev_part: \"" + ((row["prev_part"] == DBNull.Value) ? "" : DateTime.Parse(row["prev_part"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "KMOrigem: " + ((row["KMOrigem"] == DBNull.Value) ? "\"\"" : row["KMOrigem"].ToString()) + ", ";
			lvStrFlotClass += "KMDestino: " + ((row["KMDestino"] == DBNull.Value) ? "\"\"" : row["KMDestino"].ToString()) + ", ";
			lvStrFlotClass += "Sentflag: " + ((row["sentflag"] == DBNull.Value) ? "\"\"" : row["sentflag"].ToString()) + ", ";
			lvStrFlotClass += "OS: \"" + ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString()) + "\", ";
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

	public List<Trainpmt> GetList()
	{
		List<Trainpmt> lvResult = new List<Trainpmt>();
		DataSet ds = null;
		Trainpmt lvElement = null;

		ds = TrainpmtDataAccess.GetData(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Trainpmt();

			lvElement.Pmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			lvElement.Prefix = ((row["prefix"] == DBNull.Value) ? "" : row["prefix"].ToString());
			lvElement.Date_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));
			lvElement.Prev_part = ((row["prev_part"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["prev_part"].ToString()));
			lvElement.KMOrigem = ((row["KMOrigem"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMOrigem"]);
			lvElement.KMDestino = ((row["KMDestino"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMDestino"]);
			lvElement.Sentflag = ((row["sentflag"] == DBNull.Value) ? Int16.MinValue : (Int16)row["sentflag"]);
			lvElement.OS = ((row["OS"] == DBNull.Value) ? "" : row["OS"].ToString());

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvpmt_id = "";
		this.lvprefix = "";
		this.lvdate_hist = DateTime.MinValue;
		this.lvprev_part = DateTime.MinValue;
		this.lvKMOrigem = Int16.MinValue;
		this.lvKMDestino = Int16.MinValue;
		this.lvsentflag = Int16.MinValue;
		this.lvOS = "";
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainpmtDataAccess.Insert(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS);

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
			lvRowsAffect = TrainpmtDataAccess.Update(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS);

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

	public bool UpdateKey(string pmt_id, string prefix)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = TrainpmtDataAccess.UpdateKey(this.lvpmt_id, this.lvprefix, pmt_id, prefix);

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
			lvRowsAffect = TrainpmtDataAccess.Delete(this.lvpmt_id, this.lvprefix);

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

		ds = TrainpmtDataAccess.GetData(this.lvpmt_id, this.lvprefix, this.lvdate_hist, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvsentflag, this.lvOS, "");

		dt = ds.Tables[0];

		return dt;
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

	public string Prefix
	{
		get
		{
			return this.lvprefix;
		}
		set
		{
			this.lvprefix = value;
		}
	}

	public DateTime Date_hist
	{
		get
		{
			return this.lvdate_hist;
		}
		set
		{
			this.lvdate_hist = value;
		}
	}

	public DateTime Prev_part
	{
		get
		{
			return this.lvprev_part;
		}
		set
		{
			this.lvprev_part = value;
		}
	}

	public Int16 KMOrigem
	{
		get
		{
			return this.lvKMOrigem;
		}
		set
		{
			this.lvKMOrigem = value;
		}
	}

	public Int16 KMDestino
	{
		get
		{
			return this.lvKMDestino;
		}
		set
		{
			this.lvKMDestino = value;
		}
	}

	public Int16 Sentflag
	{
		get
		{
			return this.lvsentflag;
		}
		set
		{
			this.lvsentflag = value;
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


	public DataTable GetDataOfPlan(string pmt_id, Boolean pUseForeignKey = false, string pStrOrderBy = "")
	{
		DataTable dt = null;
		DataSet ds = null;

		ds = PlanDataAccess.GetDataByTrainpmt(pmt_id, pUseForeignKey, pStrOrderBy);

		dt = ds.Tables[0];

		return dt;
	}


}

