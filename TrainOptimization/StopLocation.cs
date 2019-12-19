using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Xml;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:58
/// <summary>

[Serializable]
public class StopLocation : IEquatable<StopLocation>, IComparable<StopLocation>
{
    protected static List<StopLocation> mListStopLoc = null;
    protected static int mMinLen = 0;
    protected int lvlocation = -1;
	protected int lvstart_coordinate;
	protected int lvend_coordinate;
	protected Int16 lvcapacity;
    protected Segment mPrevSwitch = null;
    protected Segment mNextSwitch = null;
    protected StopLocation mPrevStopLocation = null;
    protected StopLocation mNextStopLocation = null;
    protected Segment[] mLeftSegment = null;
    protected Segment[] mRightSegment = null;
    protected int mDwellTimeOnEndStopLocation = 0; //seconds
    protected Dictionary<int, ISet<int>> mLeftDependency = new Dictionary<int, ISet<int>>();
    protected Dictionary<int, ISet<int>> mRightDependency = new Dictionary<int, ISet<int>>();
    protected ISet<string> mNoStopSet;
    private const int CSIDES = 2;

    public StopLocation()
	{
		Clear();

        mLeftDependency = new Dictionary<int, ISet<int>>();
        mRightDependency = new Dictionary<int, ISet<int>>();
        mNoStopSet = new HashSet<string>();
    }

	public StopLocation(int location) : this()
	{
        this.lvlocation = location;
		Load();
    }

	public StopLocation(int location, int start_coordinate, int end_coordinate, Int16 capacity) : this()
	{
		this.lvlocation = location;
		this.lvstart_coordinate = start_coordinate;
		this.lvend_coordinate = end_coordinate;
		this.lvcapacity = capacity;

        mLeftSegment = new Segment[this.lvcapacity];
        mRightSegment = new Segment[this.lvcapacity];
    }

    public int CompareTo(StopLocation pOther)
    {
        int lvRes = 0;

        if (pOther == null) return 1;

        if (pOther.Start_coordinate >= this.Start_coordinate && pOther.End_coordinate <= this.End_coordinate)
        {
            lvRes = 0;
        }
        else if (this.Start_coordinate >= pOther.Start_coordinate)
        {
            lvRes = 1;
        }
        else if (this.Start_coordinate <= pOther.Start_coordinate)
        {
            lvRes = -1;
        }

        return lvRes;
    }

    public static bool operator ==(StopLocation obj1, StopLocation obj2)
    {
        bool lvRes = false;

        bool lvNoRef1 = ReferenceEquals(obj1, null);
        bool lvNoRef2 = ReferenceEquals(obj2, null);

        if (lvNoRef1 && lvNoRef2)
        {
            return true;
        }

        if (lvNoRef1)
        {
            return false;
        }

        if (lvNoRef2)
        {
            return false;
        }

        if (obj1.lvlocation == obj2.lvlocation)
        {
            lvRes = true;
        }

        /*
        if ((obj1.lvstart_coordinate >= obj2.lvstart_coordinate) && (obj1.lvend_coordinate <= obj2.lvend_coordinate))
        {
            lvRes = true;
        }
        */

        return lvRes;
    }

    public static bool operator !=(StopLocation obj1, StopLocation obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(StopLocation other)
    {
        bool lvRes = false;

        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.lvlocation == other.lvlocation)
        {
            lvRes = true;
        }

        /*
        if ((this.lvlocation == other.lvlocation) && (this.lvstart_coordinate == other.lvstart_coordinate) && (this.lvend_coordinate == other.lvend_coordinate))
        {
            lvRes = true;
        }
        */

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;
        StopLocation lvStopLocation = obj as StopLocation;

        lvRes = Equals(lvStopLocation);

        return lvRes;
    }

    public override int GetHashCode()
    {
        return lvlocation;
    }

    public bool HasBetweenSwitch(StopLocation pStopLocation)
    {
        bool lvRes = false;

        if (pStopLocation != null)
        {
            if (lvlocation < pStopLocation.Location)
            {
                if ((NextSwitch != null) && (NextSwitch.Start_coordinate < pStopLocation.Start_coordinate))
                {
                    lvRes = true;
                }
            }
            else if (lvlocation > pStopLocation.Location)
            {
                if ((PrevSwitch != null) && (PrevSwitch.End_coordinate > pStopLocation.End_coordinate))
                {
                    lvRes = true;
                }
            }
        }

        return lvRes;
    }
    
    public virtual bool Load()
	{
		bool lvResult = false;

		DataSet ds = StopLocationDataAccess.GetDataByKey(this.lvlocation, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
			this.lvstart_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
			this.lvend_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
			this.lvcapacity = ((row["capacity"] == DBNull.Value) ? Int16.MinValue : (Int16)row["capacity"]);

			lvResult = true;
		}

		return lvResult;
	}

    public static string GetFlotLocations()
    {
        StringBuilder lvResult = new StringBuilder();
        string lvStrLabel = "";

        foreach (StopLocation lvStopLocation in mListStopLoc)
        {
            lvStrLabel = ((double)lvStopLocation.Location / 100000.0).ToString().Replace(",", ".");

            if (lvResult.Length > 0)
            {
                lvResult.Append(", ");
            }

            lvResult.Append("[");
            lvResult.Append(lvStrLabel);
            lvResult.Append(", \"KM ");
            lvResult.Append(lvStrLabel);
            lvResult.Append("\"]");
        }
        lvResult.Insert(0, "[");
        lvResult.Append("]");

        return lvResult.ToString();
    }

    public string GetFlotSerie(string pStrColor, string pStrLabel, string pStrXAxisName, string pStrYAxisName, string pStrIdent, Boolean isDashed, string pStrSymbol)
	{
		StringBuilder lvResult = new StringBuilder();
		DataSet ds = null;
		string lvXValues = "";
		string lvYValues = "";
		Boolean lvHasElement = false;

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

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

			if(!lvHasElement)
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

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

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

			lvStrFlotClass += "Location: " + ((row["location"] == DBNull.Value) ? "\"\"" : row["location"].ToString()) + ", ";
			lvStrFlotClass += "Start_coordinate: " + ((row["start_coordinate"] == DBNull.Value) ? "\"\"" : row["start_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "End_coordinate: " + ((row["end_coordinate"] == DBNull.Value) ? "\"\"" : row["end_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Capacity: " + ((row["capacity"] == DBNull.Value) ? "\"\"" : row["capacity"].ToString()) + ", ";
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

    public Segment GetNextSwitchSegment(int pDirection)
    {
        Segment lvRes = null;

        if (pDirection > 0)
        {
            lvRes = mNextSwitch;
        }
        else
        {
            lvRes = mPrevSwitch;
        }

        return lvRes;
    }

    public Segment GetSegment(int pDirection, int pTrack)
    {
        Segment lvRes = null;

        if (pTrack > 0)
        {
            if (pDirection > 0)
            {
                if (pTrack - 1 < mRightSegment.Length)
                {
                    lvRes = mRightSegment[pTrack - 1];

                    if(lvRes == null)
                    {
                        foreach(Segment lvSeg in mRightSegment)
                        {
                            if(lvSeg != null)
                            {
                                lvRes = lvSeg;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    lvRes = mRightSegment[mRightSegment.Length - 1];
                }
            }
            else
            {
                if (pTrack - 1 < mLeftSegment.Length)
                {
                    lvRes = mLeftSegment[pTrack - 1];

                    if (lvRes == null)
                    {
                        foreach (Segment lvSeg in mLeftSegment)
                        {
                            if (lvSeg != null)
                            {
                                lvRes = lvSeg;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    lvRes = mLeftSegment[mLeftSegment.Length - 1];
                }
            }
        }

        return lvRes;
    }

    public StopLocation GetNextStopSegment(int pDirection)
    {
        StopLocation lvRes = null;

        if (pDirection > 0)
        {
            lvRes = mNextStopLocation;
        }
        else
        {
            lvRes = mPrevStopLocation;
        }

        return lvRes;
    }

    public static StopLocation GetNextStopSegment(int pCoordinate, int pDirection)
    {
        int lvStopSegIndex = -1;
        StopLocation lvCurrentSegment = null;
        StopLocation lvResSegment = null;

        if (mListStopLoc == null)
        {
            return null;
        }

        if (mListStopLoc.Count == 0)
        {
            return null;
        }

        lvCurrentSegment = StopLocation.GetCurrentStopSegment(pCoordinate, pDirection, out lvStopSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvStopSegIndex < (mListStopLoc.Count - 1))
                {
                    lvResSegment = mListStopLoc[lvStopSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvStopSegIndex > 0)
                {
                    lvResSegment = mListStopLoc[lvStopSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvStopSegIndex >= mListStopLoc.Count)
                {
                    lvResSegment = mListStopLoc[mListStopLoc.Count - 1];
                }
                else if (~lvStopSegIndex <= 0)
                {
                    lvResSegment = mListStopLoc[0];
                }
                else
                {
                    lvResSegment = mListStopLoc[~lvStopSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvStopSegIndex >= mListStopLoc.Count)
                {
                    lvResSegment = mListStopLoc[mListStopLoc.Count - 1];
                }
                else if (~lvStopSegIndex <= 1)
                {
                    lvResSegment = mListStopLoc[0];
                }
                else
                {
                    lvResSegment = mListStopLoc[~lvStopSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static StopLocation GetCurrentStopSegment(int pCoordinate, int pDirection, out int pIdxSegment)
    {
        int lvStopSegIndex = -1;
        StopLocation lvSubSegment = new StopLocation();
        StopLocation lvResSegment = null;

        if (mListStopLoc == null)
        {
            pIdxSegment = -1;
            return null;
        }

        if (mListStopLoc.Count == 0)
        {
            pIdxSegment = -1;
            return null;
        }

        if (pDirection > 0)
        {
            lvSubSegment.Start_coordinate = pCoordinate;
            lvSubSegment.End_coordinate = pCoordinate + 100;
        }
        else if (pDirection < 0)
        {
            lvSubSegment.Start_coordinate = pCoordinate - 100;
            lvSubSegment.End_coordinate = pCoordinate;
        }

        lvStopSegIndex = mListStopLoc.BinarySearch(lvSubSegment);

        pIdxSegment = lvStopSegIndex;
        if (lvStopSegIndex >= 0)
        {
            lvResSegment = mListStopLoc[lvStopSegIndex];
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    public static void ImportList(List<StopLocation> pStopLocations = null)
    {
        Segment lvSegment = null;
        StopLocation lvStopLocation;
        List<Segment> lvSegments = Segment.GetList();
        int lvIndex = 0;

        if(pStopLocations != null)
        {
            mListStopLoc = pStopLocations;
        }

        for(int ind = 0; ind < mListStopLoc.Count; ind++)
        {
            lvStopLocation = mListStopLoc[ind];

            if(ind > 0)
            {
                lvStopLocation.PrevStopLocation = mListStopLoc[ind - 1];
            }

            if(ind < (mListStopLoc.Count - 1))
            {
                lvStopLocation.NextStopLocation = mListStopLoc[ind + 1];
            }

            for (int i = lvIndex; i < lvSegments.Count; i++)
            {
                lvSegment = lvSegments[i];

                if ((lvSegment.Start_coordinate < lvStopLocation.End_coordinate) && (lvSegment.End_coordinate > lvStopLocation.Start_coordinate) && (lvSegment.End_coordinate <= lvStopLocation.End_coordinate))
                {
                    if (lvSegment.Track <= lvStopLocation.Capacity)
                    {
                        if (lvSegment.IsSwitch)
                        {
                            lvStopLocation.Start_coordinate = lvSegment.End_coordinate;
                        }
                        else
                        {
                            lvStopLocation.LeftSegment[lvSegment.Track - 1] = lvSegment;
                        }
                    }
                }

                if((lvSegment.End_coordinate >= lvStopLocation.Start_coordinate) && (lvSegment.Start_coordinate >= lvStopLocation.Start_coordinate) && (lvSegment.Start_coordinate < lvStopLocation.End_coordinate))
                {
                    if (lvSegment.Track <= lvStopLocation.Capacity)
                    {
                        if (lvSegment.IsSwitch)
                        {
                            lvStopLocation.End_coordinate = lvSegment.Start_coordinate;

                            if(i >= lvStopLocation.Capacity)
                            {
                                for (int idx=i-lvStopLocation.Capacity; idx < i; idx++)
                                {
                                    lvSegment = lvSegments[idx];
                                    if (lvSegment.Track <= lvStopLocation.Capacity)
                                    {
                                        lvStopLocation.RightSegment[lvSegment.Track - 1] = lvSegment;
                                    }
                                }
                            }
                        }
                        else
                        {
                            lvStopLocation.RightSegment[lvSegment.Track - 1] = lvSegment;
                        }
                    }
                }

                if (lvStopLocation.PrevSwitch == null)
                {
                    for (int indsw = i; indsw >= 0; indsw--)
                    {
                        lvSegment = lvSegments[indsw];
                        if ((lvSegment.IsSwitch) && ((lvSegment.End_coordinate <= lvStopLocation.Start_coordinate) || (lvSegment.Start_coordinate == lvStopLocation.Start_coordinate)))
                        {
                            lvStopLocation.PrevSwitch = lvSegment;
                            break;
                        }
                    }
                }

                if (lvStopLocation.NextSwitch == null)
                {
                    for (int indsw = i; indsw < lvSegments.Count; indsw++)
                    {
                        lvSegment = lvSegments[indsw];
                        if ((lvSegment.IsSwitch) && ((lvSegment.Start_coordinate >= lvStopLocation.End_coordinate) || (lvSegment.End_coordinate == lvStopLocation.End_coordinate)))
                        {
                            lvStopLocation.NextSwitch = lvSegment;
                            break;
                        }
                    }
                }

                if(lvSegments[i].Start_coordinate >= lvStopLocation.End_coordinate)
                {
                    lvIndex = i;
                    break;
                }
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("lvStopLocation => " + lvStopLocation + "\n");
                lvStrInfo.Append("lvStopLocation.PrevSwitch => " + (lvStopLocation.PrevSwitch == null ? "null" : lvStopLocation.PrevSwitch.ToString()) + "\n");
                lvStrInfo.Append("lvStopLocation.NextSwitch => " + (lvStopLocation.NextSwitch == null ? "null" : lvStopLocation.NextSwitch.ToString()) + "\n");
                lvStrInfo.Append("lvStopLocation.PrevStopLocation => " + (lvStopLocation.PrevStopLocation == null ? "null" : lvStopLocation.PrevStopLocation.ToString()) + "\n");
                lvStrInfo.Append("lvStopLocation.NextStopLocation => " + (lvStopLocation.NextStopLocation == null ? "null" : lvStopLocation.NextStopLocation.ToString()) + "\n");
                for (int index=0; index < lvStopLocation.LeftSegment.Length; index++)
                {
                    lvSegment = lvStopLocation.LeftSegment[index];
                    lvStrInfo.Append("lvStopLocation.LeftSegment => " + (lvSegment == null ? "null" : lvSegment.ToString()) + "\n");
                }
                for (int index = 0; index < lvStopLocation.RightSegment.Length; index++)
                {
                    lvSegment = lvStopLocation.RightSegment[index];
                    lvStrInfo.Append("lvStopLocation.RightSegment => " + (lvSegment == null ? "null" : lvSegment.ToString()) + "\n");
                }

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }
#endif
        }
    }

    public static void LoadList(string pStrFile)
    {
        StopLocation lvStopLocation;
        string lvStrDependency;
        string[] lvVarDependency;
        string[] lvVarElements;
        int lvKey;
        int lvValue;

        mListStopLoc = new List<StopLocation>();

        try
        {
            XmlReader lvXmlReader = XmlReader.Create(pStrFile);

            while (lvXmlReader.Read())
            {
                if (lvXmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (lvXmlReader.Name)
                    {
                        case "StopLocation":
                            lvStopLocation = new StopLocation();
                            if (lvXmlReader["location"] != null)
                            {
                                lvStopLocation.Location = Convert.ToInt32(lvXmlReader["location"]);
                            }

                            if (lvXmlReader["start_coordinate"] != null)
                            {
                                lvStopLocation.Start_coordinate = Convert.ToInt32(lvXmlReader["start_coordinate"]);
                            }

                            if (lvXmlReader["end_coordinate"] != null)
                            {
                                lvStopLocation.End_coordinate = Convert.ToInt32(lvXmlReader["end_coordinate"]);
                            }

                            if (lvXmlReader["capacity"] != null)
                            {
                                lvStopLocation.Capacity = Convert.ToInt16(lvXmlReader["capacity"]);
                            }

                            if (lvXmlReader["end_dwell_time"] != null)
                            {
                                lvStopLocation.DwellTimeOnEndStopLocation = Convert.ToInt32(lvXmlReader["end_dwell_time"]);
                            }

                            if (lvXmlReader["left_dependency"] != null)
                            {
                                lvStrDependency = lvXmlReader["left_dependency"];
                                lvVarDependency = lvStrDependency.Split(';');

                                foreach(string lvElem in lvVarDependency)
                                {
                                    if(lvElem.IndexOf('-') > 0)
                                    {
                                        lvVarElements = lvElem.Split('-');

                                        if(lvVarElements.Length == 2)
                                        {
                                            if(!Int32.TryParse(lvVarElements[0], out lvKey))
                                            {
                                                lvKey = 0;
                                            }

                                            if (!Int32.TryParse(lvVarElements[1], out lvValue))
                                            {
                                                lvValue = 0;
                                            }
                                            
                                            if((lvKey > 0) && (lvValue > 0))
                                            {
                                                lvStopLocation.AddDependency(lvKey, lvValue, 1);
                                            }
                                        }
                                    }
                                }
                            }

                            if (lvXmlReader["right_dependency"] != null)
                            {
                                lvStrDependency = lvXmlReader["right_dependency"];
                                lvVarDependency = lvStrDependency.Split(';');

                                foreach (string lvElem in lvVarDependency)
                                {
                                    if (lvElem.IndexOf('-') > 0)
                                    {
                                        lvVarElements = lvElem.Split('-');

                                        if (lvVarElements.Length == 2)
                                        {
                                            if (!Int32.TryParse(lvVarElements[0], out lvKey))
                                            {
                                                lvKey = 0;
                                            }

                                            if (!Int32.TryParse(lvVarElements[1], out lvValue))
                                            {
                                                lvValue = 0;
                                            }

                                            if ((lvKey > 0) && (lvValue > 0))
                                            {
                                                lvStopLocation.AddDependency(lvKey, lvValue, -1);
                                            }
                                        }
                                    }
                                }
                            }

                            if (lvXmlReader["no_stop"] != null)
                            {
                                lvStrDependency = lvXmlReader["no_stop"];
                                lvVarDependency = lvStrDependency.Split(';');

                                foreach (string lvElem in lvVarDependency)
                                {
                                    if (lvElem.Length > 0)
                                    {
                                        lvStopLocation.NoStopSet.Add(lvElem);
                                    }
                                }
                            }

                            mListStopLoc.Add(lvStopLocation);

                            break;
                    }
                }
            }

            ImportList();
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public static void LoadList()
    {
        DataSet ds = null;
        StopLocation lvElement = null;

        ds = StopLocationDataAccess.GetAll("location asc");

        mListStopLoc = new List<StopLocation>();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new StopLocation();

            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvElement.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
            lvElement.Capacity = ((row["capacity"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["capacity"]));

            mListStopLoc.Add(lvElement);
        }

        ImportList();
    }

    /*
    public static void LoadList(List<Segment> pSwitchSegments)
    {
        mListStopLoc = new List<StopLocation>();
        DataSet ds = null;
        int lvLastLocation = Int32.MinValue;
        StopLocation lvCurrentElement = null;
        StopLocation lvElement = null;
        StopLocation lvPrevElement = null;
        Segment lvCurrentSegment = null;
        int lvIndLeft = 0;
        int lvIndRight = 0;
        int lvIndex = 0;

        ds = StopLocationDataAccess.GetAll("location asc");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new StopLocation();

            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvElement.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
            lvElement.Capacity = ((row["capacity"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["capacity"]));

            if(lvCurrentElement != null)
            {
                lvCurrentElement.mNextStopLocation = lvElement;

                if(pSwitchSegments != null)
                {
                    while (lvIndRight < pSwitchSegments.Count)
                    {
                        if (lvCurrentElement.lvend_coordinate > pSwitchSegments[lvIndRight].End_coordinate)
                        {
                            lvIndRight++;
                            lvIndLeft = lvIndRight - 1;
                        }

                        if (lvIndRight >= pSwitchSegments.Count)
                        {
                            break;
                        }

                        if (lvCurrentElement.lvstart_coordinate < pSwitchSegments[lvIndLeft].Start_coordinate)
                        {
                            lvIndLeft--;
                        }

                        if (lvIndLeft >= lvIndRight)
                        {
                            lvIndRight = lvIndLeft - 1;
                        }

                        if ((lvCurrentElement.lvstart_coordinate >= pSwitchSegments[lvIndLeft].Start_coordinate) && (lvCurrentElement.lvend_coordinate <= pSwitchSegments[lvIndRight].End_coordinate))
                        {
                            lvCurrentElement.mPrevSwitch = pSwitchSegments[lvIndLeft];
                            lvCurrentElement.mNextSwitch = pSwitchSegments[lvIndRight];
                            break;
                        }
                    }
                }

                for (int i = 0; i < lvCurrentElement.Capacity; i++)
                {
                    lvCurrentSegment = Segment.GetCurrentSegment(lvCurrentElement.Start_coordinate, 1, (i + 1), out lvIndex);
                    lvCurrentElement.LeftSegment.Add(lvCurrentSegment);

                    lvCurrentSegment = Segment.GetCurrentSegment(lvCurrentElement.End_coordinate, -1, (i + 1), out lvIndex);
                    lvCurrentElement.RightSegment.Add(lvCurrentSegment);
                }

                mListStopLoc.Add(lvCurrentElement);
                lvCurrentElement = lvElement;
            }
            else if (lvElement.mNextStopLocation == null)
            {
                lvCurrentElement = lvElement;
            }
            lvElement.mPrevStopLocation = lvPrevElement;

            if (FIRST_STOP_SEGMENT == Int32.MinValue)
            {
                FIRST_STOP_SEGMENT = lvElement.Location;
            }

            lvLastLocation = lvElement.Location;

            lvPrevElement = lvElement;
            lvElement = null;
        }

        if(lvCurrentElement != null)
        {
            if (pSwitchSegments != null)
            {
                while (lvIndRight < pSwitchSegments.Count)
                {
                    if (lvCurrentElement.lvend_coordinate > pSwitchSegments[lvIndRight].End_coordinate)
                    {
                        lvIndRight++;
                        lvIndLeft = lvIndRight - 1;
                    }

                    if(lvIndRight >= pSwitchSegments.Count)
                    {
                        break;
                    }

                    if (lvCurrentElement.lvstart_coordinate < pSwitchSegments[lvIndLeft].Start_coordinate)
                    {
                        lvIndLeft--;
                    }

                    if (lvIndLeft >= lvIndRight)
                    {
                        lvIndRight = lvIndLeft - 1;
                    }

                    if ((lvCurrentElement.lvstart_coordinate >= pSwitchSegments[lvIndLeft].Start_coordinate) && (lvCurrentElement.lvend_coordinate <= pSwitchSegments[lvIndRight].End_coordinate))
                    {
                        lvCurrentElement.mPrevSwitch = pSwitchSegments[lvIndLeft];
                        lvCurrentElement.mNextSwitch = pSwitchSegments[lvIndRight];
                        break;
                    }
                }
            }

            for (int i = 0; i < lvCurrentElement.Capacity; i++)
            {
                lvCurrentElement.LeftSegment.Add(Segment.GetCurrentSegment(lvCurrentElement.Start_coordinate, 1, i, out lvIndex));
                lvCurrentElement.RightSegment.Add(Segment.GetCurrentSegment(lvCurrentElement.End_coordinate, -1, i, out lvIndex));
            }

            mListStopLoc.Add(lvCurrentElement);
        }

        DebugLog.Logar("LoadList.mListStopLoc.Count = " + mListStopLoc.Count);

        LAST_STOP_SEGMENT = lvLastLocation;
    }
*/

    public static List<StopLocation> GetList()
    {
        return mListStopLoc;
    }

    public static string GetFlotTicks()
    {
        StringBuilder lvStrRes = new StringBuilder();
        int lvValue;
        int lvCount = 0; ;
        string lvStrValue = "";
        DataSet ds = null;

        ds = StopLocationDataAccess.GetAll();

        lvStrRes.Append("[");
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvValue = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]) / 100000;
            lvStrValue = lvValue.ToString();
            if (lvStrValue.Length == 1)
            {
                lvStrValue = "00" + lvStrValue;
            }
            else if (lvStrValue.Length == 2)
            {
                lvStrValue = "0" + lvStrValue;
            }

            lvStrValue = "[" + lvValue + ", \"KM " + lvStrValue + "\"]";

            if (lvCount > 0)
            {
                lvStrRes.Append(", ");
            }
            lvStrRes.Append(lvStrValue);
            lvCount++;
        }
        lvStrRes.Append("]");

//        DebugLog.Logar("Ticks = " + lvStrRes.ToString());

        return lvStrRes.ToString();
    }

	public virtual void Clear()
	{

		this.lvlocation = Int32.MinValue;
		this.lvstart_coordinate = Int32.MinValue;
		this.lvend_coordinate = Int32.MinValue;
		this.lvcapacity = Int16.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = StopLocationDataAccess.Insert(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity);

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
			lvRowsAffect = StopLocationDataAccess.Update(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity);

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

	public bool UpdateKey(int location)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = StopLocationDataAccess.UpdateKey(this.lvlocation, location);

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
			lvRowsAffect = StopLocationDataAccess.Delete(this.lvlocation);

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

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

		dt = ds.Tables[0];

		return dt;
	}

    public void AddDependency(int pKey, int pValue, int pDirection)
    {
        ISet<int> lvDependenceTracks;
        Dictionary<int, ISet<int>> lvDependency = null;

        if(pDirection > 0)
        {
            lvDependency = mLeftDependency;
        }
        else if(pDirection < 0)
        {
            lvDependency = mRightDependency;
        }
        else
        {
            return;
        }

        if (!lvDependency.ContainsKey(pKey))
        {
            lvDependenceTracks = new HashSet<int>();
            lvDependenceTracks.Add(pValue);

            lvDependency.Add(pKey, lvDependenceTracks);
        }
        else
        {
            lvDependenceTracks = lvDependency[pKey];
            lvDependenceTracks.Add(pValue);
        }
    }

    public ISet<int> HasDependency(int pKey, int pDirection)
    {
        ISet<int> lvRes = null;
        Dictionary<int, ISet<int>> lvDependency = null;

        if (pDirection > 0)
        {
            lvDependency = mLeftDependency;
        }
        else if (pDirection < 0)
        {
            lvDependency = mRightDependency;
        }
        else
        {
            return lvRes;
        }

        if ((lvDependency != null) && (lvDependency.Count > 0))
        {
            if (lvDependency.ContainsKey(pKey))
            {
                lvRes = lvDependency[pKey];
            }
        }

        return lvRes;
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

	public int Start_coordinate
	{
		get
		{
			return this.lvstart_coordinate;
		}
		set
		{
			this.lvstart_coordinate = value;
		}
	}

	public int End_coordinate
	{
		get
		{
			return this.lvend_coordinate;
		}
		set
		{
			this.lvend_coordinate = value;
		}
	}

	public Int16 Capacity
	{
		get
		{
			return this.lvcapacity;
		}
		set
		{
			this.lvcapacity = value;
            mLeftSegment = new Segment[this.lvcapacity];
            mRightSegment = new Segment[this.lvcapacity];
        }
    }

    public static int FirstLocation
    {
        get
        {
            if ((mListStopLoc != null) && (mListStopLoc.Count > 0))
            {
                return (mListStopLoc[0].Start_coordinate / 100000);
            }
            else
            {
                return -1;
            }
        }
    }

    public static int LastLocation
    {
        get
        {
            if ((mListStopLoc != null) && (mListStopLoc.Count > 0))
            {
                return (mListStopLoc[mListStopLoc.Count-1].Start_coordinate / 100000);
            }
            else
            {
                return -1;
            }
        }
    }

    protected Segment[] LeftSegment
    {
        get
        {
            return mLeftSegment;
        }
    }

    protected Segment[] RightSegment
    {
        get
        {
            return mRightSegment;
        }
    }

    public static int MinLen
    {
        get
        {
            return mMinLen;
        }

        set
        {
            mMinLen = value;
        }
    }

    public Segment PrevSwitch
    {
        get
        {
            return mPrevSwitch;
        }

        set
        {
            mPrevSwitch = value;
        }
    }

    public Segment NextSwitch
    {
        get
        {
            return mNextSwitch;
        }

        set
        {
            mNextSwitch = value;
        }
    }

    protected StopLocation PrevStopLocation
    {
        get
        {
            return mPrevStopLocation;
        }

        set
        {
            mPrevStopLocation = value;
        }
    }

    protected StopLocation NextStopLocation
    {
        get
        {
            return mNextStopLocation;
        }

        set
        {
            mNextStopLocation = value;
        }
    }

    public static int SIDES
    {
        get
        {
            return CSIDES;
        }
    }

    public int DwellTimeOnEndStopLocation
    {
        get
        {
            return mDwellTimeOnEndStopLocation;
        }

        set
        {
            mDwellTimeOnEndStopLocation = value;
        }
    }

    public ISet<string> NoStopSet
    {
        get
        {
            return mNoStopSet;
        }
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        lvRes.Append("Stop Location: ");
        lvRes.Append(lvlocation);
        lvRes.Append(", Start: ");
        lvRes.Append(lvstart_coordinate);
        lvRes.Append(", End: ");
        lvRes.Append(lvend_coordinate);
        lvRes.Append(", Capacity: ");
        lvRes.Append(lvcapacity);

        return lvRes.ToString();
    }
}

