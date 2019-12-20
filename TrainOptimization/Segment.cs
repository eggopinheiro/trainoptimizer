using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Xml;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:55
/// <summary>

[Serializable]
public class Segment : IEquatable<Segment>, IComparable<Segment>
{
    protected static List<Segment> mListSegment = null;
    protected static List<Segment> mListSwitch = null;
    protected const string mSwitchParameterFile = "switches_rules.xml";
    protected int lvlocation;
	protected string lvsegment;
	protected int lvstart_coordinate;
	protected int lvend_coordinate;
    protected StopLocation mPrevStopLocation = null;
    protected StopLocation mNextStopLocation = null;
    protected StopLocation mCurrentStopLocation = null;
    protected Segment mPrevSwitch = null;
    protected Segment mNextSwitch = null;
    protected Int16 mTrack = 0;
    protected bool mAllowSameLineMov = false;
    protected bool mIsSwitch = false;
    protected Dictionary<int, ISet<int>> mLeftNoEntrance = null;
    protected Dictionary<int, ISet<int>> mRightNoEntrance = null;

    public Segment()
	{
		Clear();
//        mSegmentComparer = new ComparerSegmentDataType();
    }

	public Segment(int location, string segment) : this()
	{
		this.lvlocation = location;
		this.lvsegment = segment;
		Load();
    }

	public Segment(int location, string segment, int start_coordinate, int end_coordinate) : this()
	{
		this.lvlocation = location;
		this.lvsegment = segment;
		this.lvstart_coordinate = start_coordinate;
		this.lvend_coordinate = end_coordinate;
	}

    public int CompareTo(Segment pOther)
    {
        int lvRes = 0;

        if (pOther == null) return -1;

        if ((pOther.Start_coordinate >= this.Start_coordinate) && (pOther.End_coordinate <= this.End_coordinate))
        {
            if (this.Track > pOther.Track)
            {
                lvRes = 1;
            }
            else if (this.Track < pOther.Track)
            {
                lvRes = -1;
            }
            else
            {
                lvRes = 0;
            }
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

    public static bool operator ==(Segment obj1, Segment obj2)
    {
        bool lvRes = false;

        if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
        {
            return true;
        }
        else if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        else if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        if ((obj1.lvstart_coordinate >= obj2.lvstart_coordinate) && (obj1.lvend_coordinate <= obj2.lvend_coordinate))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public static bool operator !=(Segment obj1, Segment obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(Segment other)
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

        if ((this.lvlocation == other.lvlocation) && (this.lvsegment.Equals(other.lvsegment)))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;

        if (obj is Segment)
        {
            lvRes = Equals(obj as Segment);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int lvHashCode = 0;

            if (lvsegment != null)
            {
                lvHashCode = lvsegment.GetHashCode();
                lvHashCode = (lvHashCode * 397) ^ lvlocation.GetHashCode();
                lvHashCode = (lvHashCode * 397) ^ mTrack.GetHashCode();
            }

            return lvHashCode;
        }
    }

	public virtual bool Load()
	{
		bool lvResult = false;

		DataSet ds = SegmentDataAccess.GetDataByKey(this.lvlocation, this.lvsegment, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
			this.lvsegment = ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString());
			this.lvstart_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
			this.lvend_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
            this.mAllowSameLineMov = ((row["allowSameLineMov"] == DBNull.Value) ? false : (bool)row["allowSameLineMov"]);

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, this.mAllowSameLineMov, "");

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, this.mAllowSameLineMov, "");

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
			lvStrFlotClass += "Segment: \"" + ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString()) + "\", ";
			lvStrFlotClass += "Start_coordinate: " + ((row["start_coordinate"] == DBNull.Value) ? "\"\"" : row["start_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "End_coordinate: " + ((row["end_coordinate"] == DBNull.Value) ? "\"\"" : row["end_coordinate"].ToString()) + ", ";
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

    public static Int16 GetLineByUD(string pStrUD)
    {
        Int16 lvRes = 1;

        if (pStrUD.StartsWith("CDV_1"))
        {
            lvRes = 1;
        }
        else if (pStrUD.StartsWith("CDV_2"))
        {
            lvRes = 2;
        }
        else if (pStrUD.StartsWith("CDV_3"))
        {
            lvRes = 3;
        }
        else if (pStrUD.Equals("SBA") || pStrUD.Equals("CAR02") || pStrUD.Equals("SLZ02") || pStrUD.Equals("PARKI") || pStrUD.Equals("1"))
        {
            lvRes = 1;
        }
        else if (pStrUD.Equals("PARKII") || pStrUD.Equals("2"))
        {
            lvRes = 2;
        }
        else if (pStrUD.Equals("PARKIII") || pStrUD.Equals("3"))
        {
            lvRes = 3;
        }

        return lvRes;
    }

    public static Segment GetNextSegment(int pCoordinate, int pDirection)
    {
        int lvSegIndex = -1;
        Segment lvCurrentSegment = null;
        Segment lvResSegment = null;

        if (mListSegment == null)
        {
            return null;
        }

        lvCurrentSegment = Segment.GetCurrentSegment(pCoordinate, pDirection, 0, out lvSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvSegIndex == (mListSegment.Count - 1))
                {
                    lvResSegment = mListSegment[lvSegIndex];
                }
                else
                {
                    lvResSegment = mListSegment[lvSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvSegIndex <= 1)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[lvSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvSegIndex == mListSegment.Count)
                {
                    lvResSegment = mListSegment[mListSegment.Count - 1];
                }
                else if (~lvSegIndex == 0)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[~lvSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvSegIndex == mListSegment.Count)
                {
                    lvResSegment = mListSegment[mListSegment.Count - 1];
                }
                else if (~lvSegIndex <= 1)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[~lvSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static Segment GetSegmentAt(int pCoordinate, int pLine, int pStart=0, int pEnd=Int32.MaxValue)
    {
        Segment lvRes = null;
        Segment lvSegment = null;
        Segment lvAuxSegment = null;
        int lvMiddle;
        int lvIndex = -1;

        if (pStart > pEnd) return lvRes;

        if(pEnd == Int32.MaxValue)
        {
            pEnd = mListSegment.Count - 1;
        }

        lvMiddle = (pStart + pEnd) / 2;
        lvSegment = mListSegment[lvMiddle];

        if((pCoordinate >= lvSegment.Start_coordinate) && (pCoordinate <= lvSegment.End_coordinate))
        {
            if(pLine == lvSegment.Track)
            {
                lvRes = lvSegment;
            }
            else 
            {
                lvAuxSegment = lvSegment;

                if (lvSegment.Track < pLine)
                {
                    lvIndex = lvMiddle + 1;
                }
                else if(lvSegment.Track > pLine)
                {
                    lvIndex = lvMiddle - 1;
                }

                if (lvIndex <= pEnd)
                {
                    lvSegment = mListSegment[lvIndex];

                    while (lvSegment.Track != pLine)
                    {
                        if (lvSegment.Track < pLine)
                        {
                            lvIndex++;
                        }
                        else if (lvSegment.Track > pLine)
                        {
                            lvIndex--;
                        }

                        if (lvIndex > pEnd)
                        {
                            break;
                        }
                        else
                        {
                            lvSegment = mListSegment[lvIndex];

                            if((lvSegment.Start_coordinate != lvAuxSegment.Start_coordinate) && (lvSegment.Start_coordinate != lvAuxSegment.End_coordinate) && (lvSegment.End_coordinate != lvAuxSegment.Start_coordinate) && (lvSegment.End_coordinate != lvAuxSegment.End_coordinate))
                            {
                                break;
                            }
                        }
                    }

                    if((lvSegment.Track == pLine) && ((lvSegment.Start_coordinate == lvAuxSegment.Start_coordinate) || (lvSegment.Start_coordinate == lvAuxSegment.End_coordinate) || (lvSegment.End_coordinate == lvAuxSegment.Start_coordinate) || (lvSegment.End_coordinate == lvAuxSegment.End_coordinate)))
                    {
                        lvRes = lvSegment;
                    }
                }
            }
        }
        else if(pCoordinate > lvSegment.End_coordinate)
        {
            lvRes = GetSegmentAt(pCoordinate, pLine, lvMiddle + 1, pEnd);
        }
        else if(pCoordinate < lvSegment.Start_coordinate)
        {
            lvRes = GetSegmentAt(pCoordinate, pLine, pStart, lvMiddle - 1);
        }

        return lvRes;
    }

    public static Segment GetCurrentSegment(int pCoordinate, int pDirection, int pLine, out int pSegIndex)
    {
        Segment lvSubSegment = new Segment();
        Segment lvResSegment = null;
        Segment lvOrigSegment = null;
        int lvCurrentIndex = -1;
        bool lvFound = false;

        if (mListSegment == null)
        {
            pSegIndex = -1;
            return null;
        }

        if (pLine == 0)
        {
            pLine = 1;
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
        lvSubSegment.Track = Convert.ToInt16(pLine);

        pSegIndex = mListSegment.BinarySearch(lvSubSegment);
        lvCurrentIndex = pSegIndex;

        if (pSegIndex >= 0)
        {
            lvResSegment = mListSegment[pSegIndex];
            if (pLine != 0)
            {
                lvOrigSegment = lvResSegment;

                while (lvSubSegment.Start_coordinate >= lvResSegment.Start_coordinate && lvSubSegment.End_coordinate <= lvResSegment.End_coordinate)
                {
                    if (lvResSegment.mTrack == pLine)
                    {
                        lvFound = true;
                        break;
                    }
                    else
                    {
                        pSegIndex--;
                        if (pSegIndex < 0)
                        {
                            break;
                        }
                        lvResSegment = mListSegment[pSegIndex];
                    }
                }

                if (!lvFound)
                {
                    pSegIndex = lvCurrentIndex + 1;
                    lvResSegment = mListSegment[pSegIndex];
                    while (lvSubSegment.Start_coordinate >= lvResSegment.Start_coordinate && lvSubSegment.End_coordinate <= lvResSegment.End_coordinate)
                    {
                        if (lvResSegment.mTrack == pLine)
                        {
                            lvFound = true;
                            break;
                        }
                        else
                        {
                            pSegIndex++;

                            if (pSegIndex >= mListSegment.Count)
                            {
                                break;
                            }
                            lvResSegment = mListSegment[pSegIndex];
                        }
                    }
                }

                if (!lvFound)
                {
                    lvResSegment = lvOrigSegment;
                }
            }
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    /*
        public static Segment GetCurrentSegment(int pCoordinate, int pDirection, int pLine, out int pSegIndex)
        {
            Segment lvSubSegment = new Segment();
            Segment lvResSegment = null;
            Segment lvOrigSegment = null;
            int lvCurrentIndex = -1;

            if (mListSegment == null)
            {
                pSegIndex = -1;
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

            pSegIndex = mListSegment.BinarySearch(lvSubSegment);
            lvCurrentIndex = pSegIndex;

            if (pSegIndex >= 0)
            {
                lvResSegment = mListSegment[pSegIndex];
                if (pLine != 0)
                {
                    lvOrigSegment = lvResSegment;

                    if (lvResSegment.SegmentValue.StartsWith("CDV_1") && pLine != 1)
                    {
                        lvResSegment = mListSegment[++pSegIndex];

                        if (!lvResSegment.SegmentValue.StartsWith("CDV_2") && !lvResSegment.SegmentValue.Equals("PARKII"))
                        {
                            lvResSegment = mListSegment[(lvCurrentIndex - 1)];
                        }
                    }

                    if (lvResSegment.SegmentValue.StartsWith("CDV_2") && pLine != 2)
                    {
                        lvResSegment = mListSegment[--pSegIndex];

                        if (!lvResSegment.SegmentValue.StartsWith("CDV_1") && !lvResSegment.SegmentValue.Equals("PARKI"))
                        {
                            lvResSegment = mListSegment[(lvCurrentIndex + 1)];
                        }
                    }

                    if (lvResSegment.SegmentValue.Equals("PARKI") && pLine != 1)
                    {
                        lvResSegment = mListSegment[++pSegIndex];

                        if (!lvResSegment.SegmentValue.StartsWith("CDV_2") && !lvResSegment.SegmentValue.Equals("PARKII"))
                        {
                            lvResSegment = mListSegment[(lvCurrentIndex - 1)];
                        }
                    }

                    if (lvResSegment.SegmentValue.Equals("PARKII") && pLine != 2)
                    {
                        lvResSegment = mListSegment[--pSegIndex];

                        if (!lvResSegment.SegmentValue.StartsWith("CDV_1") && !lvResSegment.SegmentValue.Equals("PARKI"))
                        {
                            lvResSegment = mListSegment[(lvCurrentIndex + 1)];
                        }
                    }

                    if (lvSubSegment.Start_coordinate < lvResSegment.Start_coordinate || lvSubSegment.End_coordinate > lvResSegment.End_coordinate)
                    {
                        lvResSegment = lvOrigSegment;
                    }
                }
            }
            else
            {
                lvResSegment = null;
            }

            return lvResSegment;
        }
    */

    public StopLocation GetNextStopLocation(int pDirection)
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

    public static Segment GetNextSwitchSegment(int pCoordinate, int pDirection)
    {
        int lvSegIndex = -1;
        Segment lvCurrentSegment = null;
        Segment lvResSegment = null;

        if (mListSwitch == null)
        {
            return null;
        }

        lvCurrentSegment = Segment.GetCurrentSwitchSegment(pCoordinate, pDirection, out lvSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvSegIndex == (mListSwitch.Count - 1))
                {
                    lvResSegment = mListSwitch[lvSegIndex];
                }
                else
                {
                    lvResSegment = mListSwitch[lvSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvSegIndex <= 1)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[lvSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvSegIndex == mListSwitch.Count)
                {
                    lvResSegment = mListSwitch[mListSwitch.Count - 1];
                }
                else if (~lvSegIndex == 0)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[~lvSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvSegIndex == mListSwitch.Count)
                {
                    lvResSegment = mListSwitch[mListSwitch.Count - 1];
                }
                else if (~lvSegIndex <= 1)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[~lvSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static Segment GetCurrentSwitchSegment(int pCoordinate, int pDirection, out int pSegIndex)
    {
        Segment lvSubSegment = new Segment();
        Segment lvResSegment = null;

        if (mListSegment == null)
        {
            pSegIndex = -1;
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

        pSegIndex = mListSwitch.BinarySearch(lvSubSegment);

        if (pSegIndex >= 0)
        {
            lvResSegment = mListSwitch[pSegIndex];
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    public static void LoadStopLocations(List<StopLocation> pStopLocations)
    {
        int lvIndNext = 0;
        int lvIndPrev = 0;

        if((mListSegment != null) && (pStopLocations != null))
        {
            foreach(Segment lvSegment in mListSegment)
            {
                if(lvSegment.End_coordinate <= pStopLocations[lvIndNext].Start_coordinate)
                {
                    lvSegment.NextStopLocation = pStopLocations[lvIndNext];
                }
                else if(lvIndNext < (pStopLocations.Count - 1))
                {
                    lvIndNext++;
                    if (lvSegment.End_coordinate <= pStopLocations[lvIndNext].Start_coordinate)
                    {
                        lvSegment.NextStopLocation = pStopLocations[lvIndNext];
                    }
                }

                while (lvIndPrev < (pStopLocations.Count - 1))
                {
                    if (lvSegment.Start_coordinate >= pStopLocations[lvIndPrev].End_coordinate)
                    {
                        if (lvSegment.Start_coordinate < pStopLocations[lvIndPrev + 1].End_coordinate)
                        {
                            lvSegment.PrevStopLocation = pStopLocations[lvIndPrev];
                            break;
                        }
                        else
                        {
                            lvIndPrev++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                foreach (Segment lvSegment in mListSegment)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvSegment => " + lvSegment + "\n");

                    if (lvSegment.OwnerStopLocation == null)
                    {
                        lvStrInfo.Append("lvSegment.OwnerStopLocation => null \n");
                    }
                    else
                    {
                        lvStrInfo.Append("lvSegment.OwnerStopLocation => " + lvSegment.OwnerStopLocation + "\n");
                    }

                    if (lvSegment.PrevStopLocation == null)
                    {
                        lvStrInfo.Append("lvSegment.PrevStopLocation => null \n");
                    }
                    else
                    {
                        lvStrInfo.Append("lvSegment.PrevStopLocation => " + lvSegment.PrevStopLocation + "\n");
                    }

                    if (lvSegment.NextStopLocation == null)
                    {
                        lvStrInfo.Append("lvSegment.NextStopLocation => null \n");
                    }
                    else
                    {
                        lvStrInfo.Append("lvSegment.NextStopLocation => " + lvSegment.NextStopLocation + "\n");
                    }

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
            }
#endif
        }
    }

    public static void LoadNeighborSwitch()
    {
        int lvIndPrev = 0;
        int lvIndNext = 0;
        Segment lvSegm;

        if ((mListSegment != null) && (mListSwitch != null))
        {
            for(int i = mListSwitch.Count-1; i>=0; i--)
            {
                lvSegm = mListSwitch[i];

                if (!lvSegm.IsSwitch)
                {
                    mListSwitch.RemoveAt(i);
                }
            }

            foreach (Segment lvSegment in mListSegment)
            {
                if (lvSegment.Start_coordinate < mListSwitch[lvIndNext].Start_coordinate)
                {
                    lvSegment.mNextSwitch = mListSwitch[lvIndNext];
                }
                else if (lvIndNext < (mListSwitch.Count - 1))
                {
                    lvIndNext++;
                    if (lvSegment.Start_coordinate < mListSwitch[lvIndNext].Start_coordinate)
                    {
                        lvSegment.mNextSwitch = mListSwitch[lvIndNext];
                    }
                }

                if (lvSegment.Start_coordinate >= mListSwitch[lvIndPrev].End_coordinate)
                {
                    /*
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvSegment = " + lvSegment);
                        lvStrInfo.Append("mListSwitch[" + lvIndPrev + "] = " + mListSwitch[lvIndPrev]);

                        DebugLog.Logar(lvStrInfo.ToString());
                    }
                    */

                    lvSegment.mPrevSwitch = mListSwitch[lvIndPrev];

                    if (lvIndPrev < (mListSwitch.Count - 1))
                    {
                        /*
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvSegment.End_coordinate = " + lvSegment.End_coordinate);
                            lvStrInfo.Append("mListSwitch[" + (lvIndPrev + 1) + "].End_coordinate = " + mListSwitch[lvIndPrev + 1].End_coordinate);
                            lvStrInfo.Append("lvSegment.Start_coordinate = " + lvSegment.Start_coordinate);
                            lvStrInfo.Append("mListSwitch[" + (lvIndPrev + 1) + "].Start_coordinate = " + mListSwitch[lvIndPrev + 1].Start_coordinate);

                            DebugLog.Logar(lvStrInfo.ToString());
                        }
                        */

                        if ((lvSegment.End_coordinate >= mListSwitch[lvIndPrev + 1].End_coordinate) && (lvSegment.Start_coordinate >= mListSwitch[lvIndPrev + 1].Start_coordinate))
                        {
                            lvIndPrev++;
                        }
                    }
                }
                /*
                else
                {
                    DebugLog.Logar("lvSegment sem prev Switch = " + lvSegment);
                }
                */

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvSegment => " + lvSegment + "\n");
                    lvStrInfo.Append("lvSegment.PrevSwitch => " + (lvSegment.PrevSwitch == null ? "null" : lvSegment.PrevSwitch.ToString()) + "\n");
                    lvStrInfo.Append("lvSegment.NextSwitch => " + (lvSegment.NextSwitch == null ? "null" : lvSegment.NextSwitch.ToString()) + "\n");

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
            }
        }
    }

    private static void LoadSwitchesParameters(string pCrossoverSide, Segment pSegment)
    {
        if (!pSegment.IsSwitch) return;

        try
        {
            XmlReader lvXmlReader = XmlReader.Create(ConfigurationManager.AppSettings["LOG_PATH"] + mSwitchParameterFile);

            while (lvXmlReader.Read())
            {
                if (lvXmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (lvXmlReader.Name)
                    {
                        case "Switch":
                            if (lvXmlReader["name"] != null)
                            {
                                if(pCrossoverSide.Equals(lvXmlReader["name"]))
                                {
                                    if (lvXmlReader["left_no_entrance"] != null)
                                    {
                                        pSegment.AddNoEntrances(lvXmlReader["left_no_entrance"], 1);
                                    }

                                    if (lvXmlReader["right_no_entrance"] != null)
                                    {
                                        pSegment.AddNoEntrances(lvXmlReader["right_no_entrance"], -1);
                                    }
                                }
                            }

                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public static void LoadList()
    {
        string lvCrossoverSide;
        mListSegment = new List<Segment>();
        mListSwitch = new List<Segment>();

        DataSet ds = null;
        Segment lvElement = null;

        ds = SegmentDataAccess.GetAll("location, start_coordinate, end_coordinate");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new Segment();

            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.SegmentValue = ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString());
            lvElement.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvElement.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
            lvElement.AllowSameLineMov = ((row["allow_same_line_mov"] == DBNull.Value) ? false : (bool)row["allow_same_line_mov"]);
            lvElement.IsSwitch = ((row["is_switch"] == DBNull.Value) ? false : (bool)row["is_switch"]);
            lvElement.Track = ((row["track"] == DBNull.Value) ? (short)0 : (short)row["track"]);

            //lvElement.Track = GetLineByUD(lvElement.SegmentValue);
            lvElement.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));

            mListSegment.Add(lvElement);

            //if (lvElement.SegmentValue.StartsWith("CV03") || lvElement.SegmentValue.StartsWith("SW") || lvElement.SegmentValue.Equals("WT"))
            if (lvElement.SegmentValue.StartsWith("CV03") || lvElement.SegmentValue.Equals("WT") || (lvElement.IsSwitch && (lvElement.Track == 1) && !lvElement.SegmentValue.StartsWith("TE")))
            {
                lvElement.IsSwitch = true;

                lvCrossoverSide = ((row["crossover_side"] == DBNull.Value) ? "" : row["crossover_side"].ToString());

                if(lvCrossoverSide.Length > 0)
                {
                    LoadSwitchesParameters(lvCrossoverSide, lvElement);
                }

                mListSwitch.Add(lvElement);
            }
            else
            {
                lvElement.IsSwitch = false;
            }
            lvElement = null;
        }

        if(mListSegment.Count == 0)
        {
            DebugLog.Save("Erro => Nao foi possível carregar os segmentos da ferrovia !!!");
        }
        else if(mListSegment.Count > 1)
        {
            mListSegment.Sort();
        }

#if DEBUG
        DebugLog.Logar("LoadList.mListSegment.Count = " + mListSegment.Count, false);
        DebugLog.Logar("LoadList.mListSwitch.Count = " + mListSwitch.Count, false);
#endif
    }

	public static List<Segment> GetList()
	{
        return mListSegment;
	}

    public static void SetList(List<Segment> pList)
    {
        mListSegment = pList;
    }

    public static List<Segment> GetListSwitch()
    {
        return mListSwitch;
    }

    public static void SetListSwitch(List<Segment> pList)
    {
        mListSwitch = pList;
    }

    public void AddNoEntrances(string pStrInput, int pDirection)
    {
        Dictionary<int, ISet<int>> lvEntrance = null;
        string[] lvVarEntrance;
        string[] lvVarElements;
        int lvKey;
        int lvValue;

        if (mIsSwitch)
        {
            if (pDirection > 0)
            {
                lvEntrance = mLeftNoEntrance;
            }
            else if (pDirection < 0)
            {
                lvEntrance = mRightNoEntrance;
            }
            else
            {
                return;
            }

            lvVarEntrance = pStrInput.Split(';');

            foreach (string lvElem in lvVarEntrance)
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
                            AddNoEntrance(lvKey, lvValue, pDirection);
                        }
                    }
                }
            }
        }
    }

    public void AddNoEntrance(int pKey, int pValue, int pDirection)
    {
        ISet<int> lvDependenceTracks;
        Dictionary<int, ISet<int>> lvEntrance = null;

        if (pDirection > 0)
        {
            lvEntrance = mLeftNoEntrance;
        }
        else if (pDirection < 0)
        {
            lvEntrance = mRightNoEntrance;
        }
        else
        {
            return;
        }

        if (lvEntrance != null)
        {
            lock(lvEntrance)
            {
                if (!lvEntrance.ContainsKey(pKey))
                {
                    lvDependenceTracks = new HashSet<int>();
                    lvDependenceTracks.Add(pValue);

                    lvEntrance.Add(pKey, lvDependenceTracks);
                }
                else
                {
                    lvDependenceTracks = lvEntrance[pKey];
                    lvDependenceTracks.Add(pValue);
                }
            }
        }
    }

    public ISet<int> GetNoEntrance(int pKey, int pDirection)
    {
        ISet<int> lvRes = null;
        Dictionary<int, ISet<int>> lvEntrance = null;

        if (pDirection > 0)
        {
            lvEntrance = mLeftNoEntrance;
        }
        else if (pDirection < 0)
        {
            lvEntrance = mRightNoEntrance;
        }
        else
        {
            return lvRes;
        }

        if (lvEntrance != null)
        {
            if (lvEntrance.Count > 0)
            {
                if (lvEntrance.ContainsKey(pKey))
                {
                    lvRes = lvEntrance[pKey];
                }
            }
        }

        return lvRes;
    }

    public virtual void Clear()
	{

		this.lvlocation = Int32.MinValue;
		this.lvsegment = "";
		this.lvstart_coordinate = Int32.MinValue;
		this.lvend_coordinate = Int32.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = SegmentDataAccess.Insert(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate);

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
			lvRowsAffect = SegmentDataAccess.Update(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, this.mAllowSameLineMov);

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

	public bool UpdateKey(int location, string segment)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = SegmentDataAccess.UpdateKey(this.lvlocation, this.lvsegment, location, segment);

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
			lvRowsAffect = SegmentDataAccess.Delete(this.lvlocation, this.lvsegment);

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, this.mAllowSameLineMov, "");

		dt = ds.Tables[0];

		return dt;
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

	public string SegmentValue
	{
		get
		{
			return this.lvsegment;
		}
		set
		{
			this.lvsegment = value;
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

    public Int16 Track
    {
        get
        {
            return mTrack;
        }

        set
        {
            mTrack = value;
        }
    }

    public Segment PrevSwitch
    {
        get
        {
            return mPrevSwitch;
        }
    }

    public Segment NextSwitch
    {
        get
        {
            return mNextSwitch;
        }
    }

    public bool AllowSameLineMov
    {
        get
        {
            return mAllowSameLineMov;
        }

        set
        {
            mAllowSameLineMov = value;
        }
    }

    public bool IsSwitch
    {
        get
        {
            return mIsSwitch;
        }

        set
        {
            mIsSwitch = value;

            if(mIsSwitch)
            {
                mLeftNoEntrance = new Dictionary<int, ISet<int>>();
                mRightNoEntrance = new Dictionary<int, ISet<int>>();
            }
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

    public StopLocation OwnerStopLocation
    {
        get
        {
            return mCurrentStopLocation;
        }

        set
        {
            mCurrentStopLocation = value;
        }
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        lvRes.Append("Segment: ");
        lvRes.Append(lvlocation);
        lvRes.Append(", Start: ");
        lvRes.Append(lvstart_coordinate);
        lvRes.Append(", End: ");
        lvRes.Append(lvend_coordinate);
        lvRes.Append(", UD: ");
        lvRes.Append(lvsegment);
        lvRes.Append(", Track: ");
        lvRes.Append(mTrack);

        return lvRes.ToString();
    }
}

