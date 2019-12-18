using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;

/// <summary>
/// Criado por Eggo Pinheiro em 13/02/2015 14:04:00
/// <summary>

public class Trainmovsegment
{
	protected Train lvtrain_id;
	protected DateTime lvdata_ocup;
	protected DateTime lvdata_desocup;
	protected Int16 lvlocation;
	protected string lvud;
	protected Int16 lvdirection;
	protected Int16 lvtrack;
	protected DateTime lvdate_hist;
	protected int lvcoordinate;

	public Trainmovsegment()
	{
		this.lvtrain_id = new Train();

		Clear();
        StopLocation.LoadList();
    }

	public Trainmovsegment(Int64 train_id, DateTime data_ocup)
	{
		this.lvtrain_id = new Train();
        Clear();

		this.lvtrain_id.Train_id = train_id;
		this.lvdata_ocup = data_ocup;
		Load();
    }

	public Trainmovsegment(Int64 train_id, DateTime data_ocup, DateTime data_desocup, Int16 location, string ud, Int16 direction, Int16 track, DateTime date_hist, int coordinate)
	{
		this.lvtrain_id = new Train();

		this.lvtrain_id.Train_id = train_id;
		this.lvdata_ocup = data_ocup;
		this.lvdata_desocup = data_desocup;
		this.lvlocation = location;
		this.lvud = ud;
		this.lvdirection = direction;
		this.lvtrack = track;
		this.lvdate_hist = date_hist;
		this.lvcoordinate = coordinate;
    }

	public virtual bool Load()
	{
		bool lvResult = false;

		DataSet ds = TrainmovsegmentDataAccess.GetDataByKey(this.lvtrain_id.Train_id, this.lvdata_ocup, false, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvtrain_id.Train_id = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
			this.lvdata_ocup = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
			this.lvdata_desocup = ((row["data_desocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_desocup"].ToString()));
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
			this.lvud = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
			this.lvdirection = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
			this.lvtrack = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
			this.lvdate_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));
			this.lvcoordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);

			lvResult = true;
		}

		return lvResult;
	}

    public static string GetFlotSerieTimeLine(string pStrColor, DateTime pDateRef)
    {
        long lvNow = -1;

        if (pDateRef.Date == DateTime.Now.Date)
        {
            lvNow = ConnectionManager.GetUTCDateTime(DateTime.Now);
        }
        else
        {
            lvNow = ConnectionManager.GetUTCDateTime(pDateRef);
        }

        string lvResult = "{\"color\": \"" + pStrColor + "\", \"dashes\": {\"show\": true, \"lineWidth\": 2, \"dashLength\": 3}, \"data\": [";

        lvResult += "[" + lvNow + ", 0]";
        lvResult += ", [" + lvNow + ", 89200000]";

        lvResult += "]}";

        return lvResult;
    }

	public string GetFlotSerie(string pStrColor, string pStrLabel, string pStrXAxisName, string pStrYAxisName, string pStrIdent, Boolean isDashed, string pStrSymbol, DateTime pDepTime, DateTime pInitialDate, DateTime pFinalDate, string pStrTrainType, Int16 pDirection)
	{
        StringBuilder lvResult = new StringBuilder();
        DateTime lvCurrentDate = DateTime.MinValue;
        double lvCurrentCoordinate = Int32.MinValue;
		DataSet ds = null;
		string lvXValues = "";
		string lvYValues = "";
		Boolean lvHasElement = false;

        string[] lvStrPrefixes = { "M", "C", "J", "P" };

        ds = TrainmovsegmentDataAccess.GetDataByRange(ptrain_idInicio: Double.Parse(pStrIdent), ptrain_idFim: Double.Parse(pStrIdent), pdata_ocupInicio: pInitialDate, pdata_ocupFim: pFinalDate, pStrSortField: "tbtrainmovsegment.data_ocup asc");

        //DebugLog.Logar("ds.Tables[0].Rows.Count = " + ds.Tables[0].Rows.Count);

		if (String.IsNullOrEmpty(pStrSymbol))
		{
			if(isDashed)
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");
			}
			else
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": false, \"radius\": 2}, \"lines\": {\"show\": true, \"lineWidth\": 3}, \"data\": [");
			}
		}
		else
		{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 2, \"symbol\": \"" + pStrSymbol + "\"}, \"data\": [");
		}

		foreach (DataRow row in ds.Tables[0].Rows)
		{
            if(row[pStrXAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
                lvCurrentDate = DateTime.Parse(row[pStrXAxisName.Trim()].ToString());
				lvXValues = ConnectionManager.GetUTCDateTime((row[pStrXAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : lvCurrentDate).ToString();
//                DebugLog.Logar("lvXValues = " + DateTime.Parse(row[pStrXAxisName.Trim()].ToString()) + " => " + lvXValues);
            }
			else
			{
                lvXValues = ((row[pStrXAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrXAxisName.Trim()].ToString());
//                DebugLog.Logar("lvXValues = " + row[pStrXAxisName.Trim()].ToString() + " => " + lvXValues);
            }

			if(row[pStrYAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
				lvYValues = ConnectionManager.GetUTCDateTime((row[pStrYAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrYAxisName.Trim()].ToString())).ToString();
//                DebugLog.Logar("lvYValues = " + DateTime.Parse(row[pStrXAxisName.Trim()].ToString()) + " => " + lvYValues);
            }
			else
			{
                lvCurrentCoordinate = ((row[pStrYAxisName.Trim()] == DBNull.Value) ? Double.MinValue : Double.Parse(row[pStrYAxisName.Trim()].ToString()));
				lvYValues = lvCurrentCoordinate.ToString();
//                DebugLog.Logar("lvYValues = " + row[pStrXAxisName.Trim()].ToString() + " => " + lvYValues);

                /*
                lvCurrentSegment = mStopLocations.GetCurrentStopSegment((int)(Double.Parse(lvYValues) * 100000), pDirection, out lvStopSegIndex);

                if (lvCurrentSegment != null)
                {
                    lvYValues = (lvCurrentSegment.Location / 100000).ToString();
                }
                 */ 
            }

//            DebugLog.Logar("");

			lvXValues = lvXValues.Replace(",", ".");
			lvYValues = lvYValues.Replace(",", ".");

			if(!lvHasElement)
			{
                //if ((ds.Tables[0].Rows.Count > 1) && (pDepTime.Date < pFinalDate.Date) && (lvStrPrefixes.Contains(pStrTrainType)))
                if ((pDepTime.Date < pFinalDate.Date) && (lvStrPrefixes.Contains(pStrTrainType)))
                {
                    lvResult.Append("[" + ConnectionManager.GetUTCDateTime(pInitialDate) + ", " + lvYValues + "]");
                    lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
                }
                else
                {
                    lvResult.Append("[" + lvXValues + ", " + lvYValues + "]");
                }
                lvHasElement = true;
            }
			else
			{
				lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
			}
		}

        if ((lvCurrentDate < DateTime.Now) && (lvStrPrefixes.Contains(pStrTrainType)) && (ds.Tables[0].Rows.Count >= 1) && (lvCurrentCoordinate > Double.MinValue) && (lvCurrentCoordinate > StopLocation.FirstLocation) && (lvCurrentCoordinate < StopLocation.LastLocation))
        {
            if (pFinalDate.Date == DateTime.Now.Date)
            {
                lvResult.Append(", [" + ConnectionManager.GetUTCDateTime(DateTime.Now) + ", " + lvYValues + "]");
            }
            else
            {
                lvResult.Append(", [" + ConnectionManager.GetUTCDateTime(pFinalDate) + ", " + lvYValues + "]");
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

		ds = TrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvdata_ocup, this.lvdata_desocup, this.lvlocation, this.lvud, this.lvdirection, this.lvtrack, this.lvdate_hist, this.lvcoordinate, false, "");

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
			lvStrFlotClass += "Data_ocup: \"" + ((row["data_ocup"] == DBNull.Value) ? "" : DateTime.Parse(row["data_ocup"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Data_desocup: \"" + ((row["data_desocup"] == DBNull.Value) ? "" : DateTime.Parse(row["data_desocup"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Location: " + ((row["location"] == DBNull.Value) ? "\"\"" : row["location"].ToString()) + ", ";
			lvStrFlotClass += "Ud: \"" + ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString()) + "\", ";
			lvStrFlotClass += "Direction: " + ((row["direction"] == DBNull.Value) ? "\"\"" : row["direction"].ToString()) + ", ";
			lvStrFlotClass += "Track: " + ((row["track"] == DBNull.Value) ? "\"\"" : row["track"].ToString()) + ", ";
			lvStrFlotClass += "Date_hist: \"" + ((row["date_hist"] == DBNull.Value) ? "" : DateTime.Parse(row["date_hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Coordinate: " + ((row["coordinate"] == DBNull.Value) ? "\"\"" : row["coordinate"].ToString()) + ", ";
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

	public List<Trainmovsegment> GetList()
	{
		List<Trainmovsegment> lvResult = new List<Trainmovsegment>();
		DataSet ds = null;
		Trainmovsegment lvElement = null;

		ds = TrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvdata_ocup, this.lvdata_desocup, this.lvlocation, this.lvud, this.lvdirection, this.lvtrack, this.lvdate_hist, this.lvcoordinate, false, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Trainmovsegment();

			lvElement.Train_id.Train_id = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
			lvElement.Data_ocup = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
			lvElement.Data_desocup = ((row["data_desocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_desocup"].ToString()));
			lvElement.Location = ((row["location"] == DBNull.Value) ? Int16.MinValue : (Int16)row["location"]);
			lvElement.Ud = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
			lvElement.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : (Int16)row["direction"]);
			lvElement.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : (Int16)row["track"]);
			lvElement.Date_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));
			lvElement.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvtrain_id.Clear();
		this.lvdata_ocup = DateTime.MinValue;
		this.lvdata_desocup = DateTime.MinValue;
		this.lvlocation = Int16.MinValue;
		this.lvud = "";
		this.lvdirection = Int16.MinValue;
		this.lvtrack = Int16.MinValue;
		this.lvdate_hist = DateTime.MinValue;
		this.lvcoordinate = Int32.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainmovsegmentDataAccess.Insert(this.lvtrain_id.Train_id, this.lvdata_ocup, this.lvdata_desocup, this.lvlocation, this.lvud, this.lvdirection, this.lvtrack, this.lvdate_hist, this.lvcoordinate);

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
			lvRowsAffect = TrainmovsegmentDataAccess.Update(this.lvtrain_id.Train_id, this.lvdata_ocup, this.lvdata_desocup, this.lvlocation, this.lvud, this.lvdirection, this.lvtrack, this.lvdate_hist, this.lvcoordinate);

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

	public bool UpdateKey(double train_id, DateTime data_ocup)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = TrainmovsegmentDataAccess.UpdateKey(this.lvtrain_id.Train_id, this.lvdata_ocup, train_id, data_ocup);

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
			lvRowsAffect = TrainmovsegmentDataAccess.Delete(this.lvtrain_id.Train_id, this.lvdata_ocup);

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

		ds = TrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvdata_ocup, this.lvdata_desocup, this.lvlocation, this.lvud, this.lvdirection, this.lvtrack, this.lvdate_hist, this.lvcoordinate, false, "");

		dt = ds.Tables[0];

		return dt;
	}

	public Train Train_id
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

	public DateTime Data_ocup
	{
		get
		{
			return this.lvdata_ocup;
		}
		set
		{
			this.lvdata_ocup = value;
		}
	}

	public DateTime Data_desocup
	{
		get
		{
			return this.lvdata_desocup;
		}
		set
		{
			this.lvdata_desocup = value;
		}
	}

	public Int16 Location
	{
		get
		{
			return this.lvlocation;
		}
		set
		{
			this.lvlocation = value;
		}
	}

	public string Ud
	{
		get
		{
			return this.lvud;
		}
		set
		{
			this.lvud = value;
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

	public Int16 Track
	{
		get
		{
			return this.lvtrack;
		}
		set
		{
			this.lvtrack = value;
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

	public int Coordinate
	{
		get
		{
			return this.lvcoordinate;
		}
		set
		{
			this.lvcoordinate = value;
		}
	}


}

