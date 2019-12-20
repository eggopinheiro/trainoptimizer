using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 12/03/2015 15:53:23
/// <summary>

public class Interdicao : IComparable<Interdicao>, IEquatable<Interdicao>
{
    private static List<Interdicao> mListInterdicao = new List<Interdicao>();
    protected Int64 lvti_id = 0L;
    protected Int64 lvbranch_id;
    protected int lvstart_pos;
	protected int lvend_pos;
	protected DateTime lvstart_time;
	protected DateTime lvend_time;
	protected Int16 lvfield_interdicted;
    protected Int16 mTrack;
    protected string lvss_name;
	protected string lvstatus;
	protected string lvreason;
	protected DateTime lvplan_time;
	protected DateTime lvhist;

	public Interdicao()
	{
	}

	public Interdicao(Int64 ti_id) : this()
	{
		this.lvti_id = ti_id;
		Load();
	}

	public Interdicao(Int64 ti_id, int start_pos, int end_pos, DateTime start_time, DateTime end_time, Int16 field_interdicted, string ss_name, string status, string reason, DateTime plan_time, DateTime hist)
	{
		this.lvti_id = ti_id;
		this.lvstart_pos = start_pos;
		this.lvend_pos = end_pos;
		this.lvstart_time = start_time;
		this.lvend_time = end_time;
		this.lvfield_interdicted = field_interdicted;
		this.lvss_name = ss_name;
        this.mTrack = Segment.GetLineByUD(lvss_name);
        this.lvstatus = status;
		this.lvreason = reason;
		this.lvplan_time = plan_time;
		this.lvhist = hist;
	}

    public int CompareTo(Interdicao pOther)
    {
        int lvRes = -1;

        if (pOther == null) return 1;

        if (pOther.lvstart_pos >= this.lvstart_pos && pOther.lvend_pos <= this.lvend_pos)
        {
            lvRes = 0;
        }
        else if (this.lvstart_pos >= pOther.lvstart_pos)
        {
            lvRes = 1;
        }
        else if (this.lvstart_pos <= pOther.lvstart_pos)
        {
            lvRes = -1;
        }

        return lvRes;
    }

    public static bool operator ==(Interdicao obj1, Interdicao obj2)
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

        if (obj1.lvti_id == obj2.lvti_id)
        {
            lvRes = true;
        }

        return lvRes;
    }

    public static bool operator !=(Interdicao obj1, Interdicao obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(Interdicao other)
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

        if (other.Start_pos == this.Start_pos && other.End_pos == this.End_pos && other.Start_time == this.Start_time && other.End_time == this.End_time && other.Ss_name.Equals(this.Ss_name))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;

        if (obj is Interdicao)
        {
            lvRes = Equals(obj as Interdicao);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int lvHashCode = 0;

            if (lvti_id != 0.0)
            {
                lvHashCode = (int)lvti_id;
            }

            return lvHashCode;
        }
    }

    public static void LoadList(DateTime pInitDate, DateTime pEndDate)
    {
        mListInterdicao = new List<Interdicao>();
        DataSet ds = null;
        Interdicao lvElement = null;

        ds = InterdicaoDataAccess.GetCurrentData(pInitDate, pEndDate, "start_pos, start_time, ss_name");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new Interdicao();

            lvElement.Ti_id = ((row["ti_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["ti_id"]));
            lvElement.Start_pos = ((row["start_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["start_pos"]);
            lvElement.End_pos = ((row["end_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["end_pos"]);
            lvElement.Start_time = ((row["start_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["start_time"].ToString()));
            lvElement.End_time = ((row["end_time"] == DBNull.Value) ? DateTime.MaxValue : DateTime.Parse(row["end_time"].ToString()));
            lvElement.Field_interdicted = ((row["field_interdicted"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["field_interdicted"]));
            lvElement.Ss_name = ((row["ss_name"] == DBNull.Value) ? "" : row["ss_name"].ToString());
            lvElement.Status = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
            lvElement.Reason = ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString());
            lvElement.Plan_time = ((row["plan_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["plan_time"].ToString()));
            lvElement.Hist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));

            lvElement.Track = Segment.GetLineByUD(lvElement.Ss_name);

            mListInterdicao.Add(lvElement);
            lvElement = null;
        }

#if DEBUG
        DebugLog.Logar("LoadList.mListInterdicao.Count = " + mListInterdicao.Count);
#endif
    }

    public static void Add(Interdicao pInterd)
    {
        if (pInterd != null)
        {
            mListInterdicao.Add(pInterd);
        }
    }

    public static void Add(Int64 pti_id, int pstart_pos, int pend_pos, DateTime pstart_time, DateTime pend_time, Int16 pfield_interdicted, string pss_name, string pstatus, string preason, DateTime pplan_time)
    {
        Interdicao lvElement = new Interdicao();

        lvElement.Ti_id = pti_id;
        lvElement.Start_pos = pstart_pos;
        lvElement.End_pos = pend_pos;
        lvElement.Start_time = pstart_time;
        lvElement.End_time = pend_time;
        lvElement.Field_interdicted = pfield_interdicted;
        lvElement.Ss_name = pss_name;
        lvElement.Status = pstatus;
        lvElement.Reason = preason;
        lvElement.Plan_time = pplan_time;
        lvElement.Hist = DateTime.Now;

        lvElement.Track = Segment.GetLineByUD(lvElement.Ss_name);

        mListInterdicao.Add(lvElement);
        lvElement = null;
    }

    public static List<Interdicao> GetList()
    {
        return mListInterdicao;
    }

    public static Interdicao GetNextInterdiction(int pCoordinate, int pDirection, out int pSegIndex)
    {
        Interdicao lvCurrentInterdiction = null;
        Interdicao lvResInterdiction = null;

        pSegIndex = -1;

        if (mListInterdicao == null)
        {
            return null;
        }

        lvCurrentInterdiction = Interdicao.GetCurrentInterdiction(pCoordinate, out pSegIndex);

        if (lvCurrentInterdiction != null)
        {
            if (pDirection > 0)
            {
                if (pSegIndex == (mListInterdicao.Count - 1))
                {
                    lvResInterdiction = mListInterdicao[pSegIndex];
                }
                else
                {
                    lvResInterdiction = mListInterdicao[pSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (pSegIndex <= 1)
                {
                    lvResInterdiction = mListInterdicao[0];
                }
                else
                {
                    lvResInterdiction = mListInterdicao[pSegIndex - 1];
                }
            }
        }
        else
        {
            pSegIndex = ~pSegIndex;
            if (pDirection > 0)
            {
                if (pSegIndex == mListInterdicao.Count)
                {
                    lvResInterdiction = mListInterdicao[mListInterdicao.Count - 1];
                }
                else if (pSegIndex == 0)
                {
                    lvResInterdiction = mListInterdicao[0];
                }
                else
                {
                    lvResInterdiction = mListInterdicao[pSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (pSegIndex == mListInterdicao.Count)
                {
                    lvResInterdiction = mListInterdicao[mListInterdicao.Count - 1];
                }
                else if (pSegIndex <= 1)
                {
                    lvResInterdiction = mListInterdicao[0];
                }
                else
                {
                    lvResInterdiction = mListInterdicao[pSegIndex - 1];
                }
            }
        }

        if (lvResInterdiction == null)
        {
            pSegIndex = -1;
        }
        return lvResInterdiction;
    }

    public static Interdicao GetCurrentInterdiction(int pCoordinate, out int pSegIndex)
    {
        Interdicao lvSubSegment = new Interdicao();
        Interdicao lvResInterdiction = null;

        pSegIndex = -1;

        if (mListInterdicao == null)
        {
            return null;
        }

        if (mListInterdicao.Count == 0)
        {
            return null;
        }

        lvSubSegment.Start_pos = pCoordinate;
        lvSubSegment.End_pos = pCoordinate + 100;

        pSegIndex = mListInterdicao.BinarySearch(lvSubSegment);

        if (pSegIndex >= 0)
        {
            lvResInterdiction = mListInterdicao[pSegIndex];
        }
        else
        {
            lvResInterdiction = null;
        }

        return lvResInterdiction;
    }

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = InterdicaoDataAccess.GetDataByKey(this.lvti_id, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvti_id = ((row["ti_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["ti_id"]));
			this.lvstart_pos = ((row["start_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["start_pos"]);
			this.lvend_pos = ((row["end_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["end_pos"]);
			this.lvstart_time = ((row["start_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["start_time"].ToString()));
			this.lvend_time = ((row["end_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["end_time"].ToString()));
			this.lvfield_interdicted = ((row["field_interdicted"] == DBNull.Value) ? Int16.MinValue : (Int16)row["field_interdicted"]);
			this.lvss_name = ((row["ss_name"] == DBNull.Value) ? "" : row["ss_name"].ToString());
			this.lvstatus = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
			this.lvreason = ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString());
			this.lvplan_time = ((row["plan_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["plan_time"].ToString()));
			this.lvhist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));

            this.mTrack = Segment.GetLineByUD(this.lvss_name);

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

		ds = InterdicaoDataAccess.GetData(this.lvti_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvfield_interdicted, this.lvss_name, this.lvstatus, this.lvreason, this.lvplan_time, this.lvhist, "");

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

		ds = InterdicaoDataAccess.GetData(this.lvti_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvfield_interdicted, this.lvss_name, this.lvstatus, this.lvreason, this.lvplan_time, this.lvhist, "");

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

			lvStrFlotClass += "Ti_id: " + ((row["ti_id"] == DBNull.Value) ? "\"\"" : row["ti_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Start_pos: " + ((row["start_pos"] == DBNull.Value) ? "\"\"" : row["start_pos"].ToString()) + ", ";
			lvStrFlotClass += "End_pos: " + ((row["end_pos"] == DBNull.Value) ? "\"\"" : row["end_pos"].ToString()) + ", ";
			lvStrFlotClass += "Start_time: \"" + ((row["start_time"] == DBNull.Value) ? "" : DateTime.Parse(row["start_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "End_time: \"" + ((row["end_time"] == DBNull.Value) ? "" : DateTime.Parse(row["end_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Field_interdicted: " + ((row["field_interdicted"] == DBNull.Value) ? "\"\"" : row["field_interdicted"].ToString()) + ", ";
			lvStrFlotClass += "Ss_name: \"" + ((row["ss_name"] == DBNull.Value) ? "" : row["ss_name"].ToString()) + "\", ";
			lvStrFlotClass += "Status: \"" + ((row["status"] == DBNull.Value) ? "" : row["status"].ToString()) + "\", ";
			lvStrFlotClass += "Reason: \"" + ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString()) + "\", ";
			lvStrFlotClass += "Plan_time: \"" + ((row["plan_time"] == DBNull.Value) ? "" : DateTime.Parse(row["plan_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Hist: \"" + ((row["hist"] == DBNull.Value) ? "" : DateTime.Parse(row["hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
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

    public static void ClearAll()
    {
        if(mListInterdicao != null)
        {
            mListInterdicao.Clear();
        }
        else
        {
            mListInterdicao = new List<Interdicao>();
        }
        
    }

	public virtual void Clear()
	{

		this.lvti_id = 0L;
		this.lvstart_pos = Int32.MinValue;
		this.lvend_pos = Int32.MinValue;
		this.lvstart_time = DateTime.MinValue;
		this.lvend_time = DateTime.MinValue;
		this.lvfield_interdicted = Int16.MinValue;
		this.lvss_name = "";
		this.lvstatus = "";
		this.lvreason = "";
		this.lvplan_time = DateTime.MinValue;
		this.lvhist = DateTime.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = InterdicaoDataAccess.Insert(this.lvti_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvfield_interdicted, this.lvss_name, this.lvstatus, this.lvreason, this.lvplan_time, this.lvhist);

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
			lvRowsAffect = InterdicaoDataAccess.Update(this.lvti_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvfield_interdicted, this.lvss_name, this.lvstatus, this.lvreason, this.lvplan_time, this.lvhist);

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

	public bool UpdateKey(double ti_id)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = InterdicaoDataAccess.UpdateKey(this.lvti_id, ti_id);

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
			lvRowsAffect = InterdicaoDataAccess.Delete(this.lvti_id);

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

		ds = InterdicaoDataAccess.GetData(this.lvti_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvfield_interdicted, this.lvss_name, this.lvstatus, this.lvreason, this.lvplan_time, this.lvhist, "");

		dt = ds.Tables[0];

		return dt;
	}

	public Int64 Ti_id
	{
		get
		{
			return this.lvti_id;
		}
		set
		{
			this.lvti_id = value;
		}
	}

	public int Start_pos
	{
		get
		{
			return this.lvstart_pos;
		}
		set
		{
			this.lvstart_pos = value;
		}
	}

	public int End_pos
	{
		get
		{
			return this.lvend_pos;
		}
		set
		{
			this.lvend_pos = value;
		}
	}

	public DateTime Start_time
	{
		get
		{
			return this.lvstart_time;
		}
		set
		{
			this.lvstart_time = value;
		}
	}

	public DateTime End_time
	{
		get
		{
			return this.lvend_time;
		}
		set
		{
			this.lvend_time = value;
		}
	}

	public Int16 Field_interdicted
	{
		get
		{
			return this.lvfield_interdicted;
		}
		set
		{
			this.lvfield_interdicted = value;
		}
	}

	public string Ss_name
	{
		get
		{
			return this.lvss_name;
		}
		set
		{
			this.lvss_name = value;
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

	public string Reason
	{
		get
		{
			return this.lvreason;
		}
		set
		{
			this.lvreason = value;
		}
	}

	public DateTime Plan_time
	{
		get
		{
			return this.lvplan_time;
		}
		set
		{
			this.lvplan_time = value;
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

    protected static List<Interdicao> Interdicoes
    {
        get
        {
            return mListInterdicao;
        }
    }

    protected Int64 Lvbranch_id
    {
        get
        {
            return lvbranch_id;
        }

        set
        {
            lvbranch_id = value;
        }
    }

    public short Track
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

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        lvRes.Append("Id: ");
        lvRes.Append(lvti_id);
        lvRes.Append(", Start_Pos: ");
        lvRes.Append(lvstart_pos);
        lvRes.Append(", End_Pos: ");
        lvRes.Append(lvend_pos);
        lvRes.Append(", Start_Time: ");
        lvRes.Append(lvstart_time);
        lvRes.Append(", End_Time: ");
        lvRes.Append(lvend_time);
        lvRes.Append(", UD: ");
        lvRes.Append(lvss_name);
        lvRes.Append(", Track: ");
        lvRes.Append(mTrack);

        return lvRes.ToString();
    }
}

