using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 23/04/2015 20:31:35
/// <summary>

public class TrainPerformanceControl
{
    protected static Dictionary<int, TrainPerformanceControl> mDicTrainPerformance = new Dictionary<int, TrainPerformanceControl>();
    protected string lvtraintype;
    protected Int16 lvdirection;
    protected int lvlocation;
    protected string lvud;
    protected int lvstop_location;
    protected double lvtimemov;
    protected double lvtimemovstop;
    protected double lvtimestop;
    protected double lvtimestopstop;
    protected double lvtimeheadwaymov;
    protected double lvtimeheadwaymovstop;
    protected double lvtimeheadwaystop;
    protected double lvtimeheadwaystopstop;
    protected Int16 lvDestinationTrack;

    private static bool mUseDBPerformanceData = false;

    public static bool UseDBPerformanceData
    {
        set
        {
            mUseDBPerformanceData = value;
        }
        get
        {
            return mUseDBPerformanceData;
        }
    }

    public TrainPerformanceControl()
    {
        Clear();
    }

    public TrainPerformanceControl(string traintype, Int16 direction, int location, string ud, Int16 destinationtrack, bool pUseDBPerformanceData)
        : this()
    {
        this.lvtraintype = traintype;
        this.lvdirection = direction;
        this.lvlocation = location;
        this.lvud = ud;
        this.DestinationTrack = destinationtrack;

        Load();
    }

    public TrainPerformanceControl(string traintype, Int16 direction, int location, string ud, int stop_location, double timemov, double timestop, double timeheadwaymov, double timeheadwaystop, Int16 destinationtrack)
    {
        this.lvtraintype = traintype;
        this.lvdirection = direction;
        this.lvlocation = location;
        this.lvud = ud;
        this.lvstop_location = stop_location;
        this.lvtimemov = timemov;
        this.lvtimestop = timestop;
        this.lvtimeheadwaymov = timeheadwaymov;
        this.lvtimeheadwaystop = timeheadwaystop;
        this.lvDestinationTrack = destinationtrack;
    }

    public virtual bool Load()
    {
        bool lvResult = false;


        DataSet ds = TrainperformanceDataAccess.GetDataByKey(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvDestinationTrack, "");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            this.lvtraintype = ((row["traintype"] == DBNull.Value) ? "" : row["traintype"].ToString());
            this.lvdirection = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
            this.lvlocation = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            this.lvud = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
            this.lvstop_location = ((row["stop_location"] == DBNull.Value) ? Int32.MinValue : (int)row["stop_location"]);
            this.lvtimemov = ((row["timemov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timemov"]);
            this.lvtimemovstop = ((row["timemovstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timemovstop"]);
            this.lvtimestop = ((row["timestop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timestop"]);
            this.lvtimestopstop = ((row["timestopstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timestopstop"]);
            this.lvtimeheadwaymov = ((row["timeheadwaymov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaymov"]);
            this.lvtimeheadwaymovstop = ((row["timeheadwaymovstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaymovstop"]);
            this.lvtimeheadwaystop = ((row["timeheadwaystop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaystop"]);
            this.lvtimeheadwaystopstop = ((row["timeheadwaystopstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaystopstop"]);
            this.lvDestinationTrack = ((row["destination_track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["destination_track"]));

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

        ds = TrainperformanceDataAccess.GetData(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.lvtimemov, this.TimeStop, this.lvtimeheadwaymov, this.lvtimeheadwaystop, this.lvDestinationTrack, "");

        if (String.IsNullOrEmpty(pStrSymbol))
        {
            if (isDashed)
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

            if (row[pStrXAxisName.Trim()].GetType().Name.Equals("DateTime"))
            {
                lvXValues = ConnectionManager.GetUTCDateTime((row[pStrXAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrXAxisName.Trim()].ToString())).ToString();
            }
            else
            {
                lvXValues = ((row[pStrXAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrXAxisName.Trim()].ToString());
            }

            if (row[pStrYAxisName.Trim()].GetType().Name.Equals("DateTime"))
            {
                lvYValues = ConnectionManager.GetUTCDateTime((row[pStrYAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrYAxisName.Trim()].ToString())).ToString();
            }
            else
            {
                lvYValues = ((row[pStrYAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrYAxisName.Trim()].ToString());
            }

            lvXValues = lvXValues.Replace(",", ".");
            lvYValues = lvYValues.Replace(",", ".");

            if (!lvHasElement)
            {
                lvResult.Append("[" + lvXValues + ", " + lvYValues + "]");
                lvHasElement = true;
            }
            else
            {
                lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
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
        if (lvLabelLen > -1)
        {
            lvStrVector = pStrLabel.Substring(0, lvLabelLen).Trim();
        }
        else
        {
            lvStrVector = pStrLabel.Trim();
        }

        if (lvStrVector.Length == 0)
        {
            return lvResult;
        }

        ds = TrainperformanceDataAccess.GetData(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.TimeMov, this.TimeStop, this.TimeHeadWayMov, this.TimeHeadWayStop, this.lvDestinationTrack, "");

        lvResult = "var " + lvStrVector + " = [";

        foreach (DataRow row in ds.Tables[0].Rows)
        {

            if (lvHasElement)
            {
                lvStrFlotClass = ", {";
            }
            else
            {
                lvStrFlotClass = "{";
                lvHasElement = true;
            }

            lvStrFlotClass += "Traintype: \"" + ((row["traintype"] == DBNull.Value) ? "" : row["traintype"].ToString()) + "\", ";
            lvStrFlotClass += "Direction: " + ((row["direction"] == DBNull.Value) ? "\"\"" : row["direction"].ToString()) + ", ";
            lvStrFlotClass += "Location: " + ((row["location"] == DBNull.Value) ? "\"\"" : row["location"].ToString()) + ", ";
            lvStrFlotClass += "Ud: \"" + ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString()) + "\", ";
            lvStrFlotClass += "Stop_location: " + ((row["stop_location"] == DBNull.Value) ? "\"\"" : row["stop_location"].ToString()) + ", ";
            lvStrFlotClass += "Timemov: " + ((row["timemov"] == DBNull.Value) ? "\"\"" : row["timemov"].ToString().Replace(",", ".")) + ", ";
            lvStrFlotClass += "Timestop: " + ((row["timestop"] == DBNull.Value) ? "\"\"" : row["timestop"].ToString().Replace(",", ".")) + ", ";
            lvStrFlotClass += "Timeheadwaymov: " + ((row["timeheadwaymov"] == DBNull.Value) ? "\"\"" : row["timeheadwaymov"].ToString().Replace(",", ".")) + ", ";
            lvStrFlotClass += "Timeheadwaystop: " + ((row["timeheadwaystop"] == DBNull.Value) ? "\"\"" : row["timeheadwaystop"].ToString().Replace(",", ".")) + ", ";

            if (lvStrFlotClass.LastIndexOf(",") == lvStrFlotClass.Length - 2)
            {
                lvStrFlotClass = lvStrFlotClass.Substring(0, lvStrFlotClass.Length - 2);
            }

            lvStrFlotClass += "}";

            lvResult += lvStrFlotClass + " \n ";
        }

        lvResult += "]; \n\n";

        return lvResult;
    }

    public static void LoadDic()
    {
        mDicTrainPerformance = new Dictionary<int, TrainPerformanceControl>();

        DataSet ds = null;
        TrainPerformanceControl lvElement = null;
        int lvKey = 0;

        ds = TrainperformanceDataAccess.GetAll();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new TrainPerformanceControl();

            lvElement.Traintype = ((row["traintype"] == DBNull.Value) ? "" : row["traintype"].ToString());
            lvElement.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.Ud = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
            lvElement.Stop_location = ((row["stop_location"] == DBNull.Value) ? Int32.MinValue : (int)row["stop_location"]);
            lvElement.TimeMov = ((row["timemov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timemov"]);
            lvElement.TimeMovStop = ((row["timemovstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timemovstop"]);
            lvElement.TimeStop = ((row["timestop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timestop"]);
            lvElement.TimeStopStop = ((row["timestopstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timestopstop"]);
            lvElement.TimeHeadWayMov = ((row["timeheadwaymov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaymov"]);
            lvElement.TimeHeadwayMovStop = ((row["timeheadwaymovstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaymovstop"]);
            lvElement.TimeHeadWayStop = ((row["timeheadwaystop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaystop"]);
            lvElement.TimeHeadwayStopStop = ((row["timeheadwaystopstop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaystopstop"]);
            lvElement.DestinationTrack = ((row["destination_track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["destination_track"]));

            //lvStrKey = lvElement.Traintype + ":" + lvElement.Direction + ":" + lvElement.Location + ":" + lvElement.Ud + ":" + lvElement.DestinationTrack;
            lvKey = new { lvElement.Traintype, lvElement.Direction, lvElement.Location, lvElement.Ud, lvElement.DestinationTrack }.GetHashCode();

            mDicTrainPerformance.Add(lvKey, lvElement);

            lvElement = null;
        }

        //DebugLog.Logar("LoadDic.mDicTrainPerformance.Count = " + mDicTrainPerformance.Count);
    }

    public static Dictionary<int, TrainPerformanceControl> GetDic()
    {
        return mDicTrainPerformance;
    }

    public static TrainPerformanceControl GetElementByKey(string pStrTrainType, int pDirection, int pLocation, string pStrUD, int pDestinationTrack)
    {
        int lvKey = 0;
        TrainPerformanceControl lvRes = null;

        if (mDicTrainPerformance != null)
        {
            if (mDicTrainPerformance.Count > 0)
            {
                lvKey = new { pStrTrainType, pDirection, pLocation, pStrUD, pDestinationTrack }.GetHashCode();

                //lvStrKey = String.Join(":", new String[] { pStrTrainType, pDirection.ToString(), pLocation.ToString(), pStrUD, pDestinationTrack.ToString() });

                if (mDicTrainPerformance.ContainsKey(lvKey))
                {
                    lvRes = mDicTrainPerformance[lvKey];
                }
            }
        }

        return lvRes;
    }

    public List<TrainPerformanceControl> GetList()
    {
        List<TrainPerformanceControl> lvResult = new List<TrainPerformanceControl>();
        DataSet ds = null;
        TrainPerformanceControl lvElement = null;

        if (mUseDBPerformanceData)
        {
            ds = TrainperformanceDataAccess.GetData(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.lvtimemov, this.lvtimestop, this.lvtimeheadwaymov, this.lvtimeheadwaystop, this.DestinationTrack, "");

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lvElement = new TrainPerformanceControl();

                lvElement.Traintype = ((row["traintype"] == DBNull.Value) ? "" : row["traintype"].ToString());
                lvElement.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
                lvElement.Ud = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                lvElement.Stop_location = ((row["stop_location"] == DBNull.Value) ? Int32.MinValue : (int)row["stop_location"]);
                lvElement.TimeMov = ((row["timemov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timemov"]);
                lvElement.TimeStop = ((row["timestop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timestop"]);
                lvElement.TimeHeadWayMov = ((row["timeheadwaymov"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaymov"]);
                lvElement.TimeHeadWayStop = ((row["timeheadwaystop"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["timeheadwaystop"]);

                lvResult.Add(lvElement);
                lvElement = null;
            }
        }

        return lvResult;
    }

    public virtual void Clear()
    {
        this.lvtraintype = "";
        this.lvdirection = Int16.MinValue;
        this.lvlocation = Int32.MinValue;
        this.lvud = "";
        this.lvstop_location = Int32.MinValue;
        this.lvtimemov = ConnectionManager.DOUBLE_REF_VALUE;
        this.lvtimestop = ConnectionManager.DOUBLE_REF_VALUE;
        this.lvtimeheadwaymov = ConnectionManager.DOUBLE_REF_VALUE;
        this.lvtimeheadwaystop = ConnectionManager.DOUBLE_REF_VALUE;
    }

    public virtual bool Insert()
    {
        bool lvResult = false;
        int lvRowsAffect = 0;

        try
        {
            lvRowsAffect = TrainperformanceDataAccess.Insert(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.lvtimemov, this.lvtimestop, this.lvtimeheadwaymov, this.lvtimeheadwaystop, this.DestinationTrack);

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
            lvRowsAffect = TrainperformanceDataAccess.Update(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.lvtimemov, this.lvtimestop, this.lvtimeheadwaymov, this.lvtimeheadwaystop, this.lvDestinationTrack);

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

    public bool UpdateKey(string traintype, Int16 direction, int location, string ud, Int16 destinationtrack)
    {
        bool lvResult = false;
        int lvRowsAffect = 0;

        try
        {
            lvRowsAffect = TrainperformanceDataAccess.UpdateKey(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvDestinationTrack, traintype, direction, location, ud, destinationtrack);

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
            lvRowsAffect = TrainperformanceDataAccess.Delete(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvDestinationTrack);

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

        ds = TrainperformanceDataAccess.GetData(this.lvtraintype, this.lvdirection, this.lvlocation, this.lvud, this.lvstop_location, this.lvtimemov, this.lvtimestop, this.lvtimeheadwaymov, this.lvtimeheadwaystop, this.lvDestinationTrack, "");

        dt = ds.Tables[0];

        return dt;
    }

    public string Traintype
    {
        get
        {
            return this.lvtraintype;
        }
        set
        {
            this.lvtraintype = value;
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

    public int Location
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

    public int Stop_location
    {
        get
        {
            return this.lvstop_location;
        }
        set
        {
            this.lvstop_location = value;
        }
    }

    public double TimeMov
    {
        get
        {
            return this.lvtimemov;
        }
        set
        {
            this.lvtimemov = value;
        }
    }

    public double TimeStop
    {
        get
        {
            return this.lvtimestop;
        }
        set
        {
            this.lvtimestop = value;
        }
    }

    public double TimeHeadWayMov
    {
        get
        {
            return this.lvtimeheadwaymov;
        }
        set
        {
            this.lvtimeheadwaymov = value;
        }
    }

    public double TimeHeadWayStop
    {
        get
        {
            return this.lvtimeheadwaystop;
        }
        set
        {
            this.lvtimeheadwaystop = value;
        }
    }

    public Int16 DestinationTrack
    {
        get
        {
            return this.lvDestinationTrack;
        }
        set
        {
            this.lvDestinationTrack = value;
        }
    }

    public double TimeMovStop
    {
        get
        {
            return lvtimemovstop;
        }

        set
        {
            lvtimemovstop = value;
        }
    }

    public double TimeStopStop
    {
        get
        {
            return lvtimestopstop;
        }

        set
        {
            lvtimestopstop = value;
        }
    }

    public double TimeHeadwayMovStop
    {
        get
        {
            return lvtimeheadwaymovstop;
        }

        set
        {
            lvtimeheadwaymovstop = value;
        }
    }

    public double TimeHeadwayStopStop
    {
        get
        {
            return lvtimeheadwaystopstop;
        }

        set
        {
            lvtimeheadwaystopstop = value;
        }
    }
}
