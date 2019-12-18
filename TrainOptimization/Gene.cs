using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Gene
/// </summary>

[Serializable]
public class Gene : IEquatable<Gene>
{
    private int mId = 0;
    private Int64 mTrainId = 0L;
    private DateTime mTime = DateTime.MinValue;
    private Segment mSegment = null;
    private Int16 mTrack = 0;
    private Int16 mDirection = 0;
    private int mCoordinate;
    private int mStart = Int32.MinValue;
    private int mEnd = Int32.MinValue;
    private string mTrainName = "";
    private StopLocation mStopLocation = null;
    private StopLocation mStartStopLocation = null;
    private StopLocation mEndStopLocation = null;
    private DateTime mDepartureTime = DateTime.MinValue;
    private DateTime mHeadWayTime = DateTime.MinValue;
    private double mSpeed = 0.0;
    private double mValueWeight = 1.0;
    private STATE mState = STATE.UNDEF;

    public enum STATE
    {
        IN,
        OUT,
        UNDEF
    }

    public static bool operator ==(Gene obj1, Gene obj2)
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

        if ((obj1.mTrainId == obj2.mTrainId) && (obj1.mStopLocation == obj2.mStopLocation))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public static bool operator !=(Gene obj1, Gene obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(Gene other)
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

        if ((this.mStopLocation != null) || (other.StopLocation != null))
        {
            if ((this.mTrainId == other.mTrainId) && (this.mStopLocation == other.mStopLocation))
            {
                lvRes = true;
            }
        }
        else if ((this.mTrainId == other.mTrainId) && (this.mSegment.Location == other.mSegment.Location) && (this.mSegment.SegmentValue.Equals(other.mSegment.SegmentValue)))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;
        Gene lvGene = obj as Gene;

        if (lvGene != null)
        {
            lvRes = Equals(lvGene);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        return mId;
    }

    public void LoadId()
    {
        if (mStopLocation != null)
        {
            mId = new { mTrainId, mStopLocation.Location, mState }.GetHashCode();
        }
        else
        {
            mId = new { mTrainId, mState }.GetHashCode();
        }
    }

    public int GetID()
    {
        return mId;
    }

    public DateTime HeadWayTime
    {
        get { return mHeadWayTime; }
        set { mHeadWayTime = value; }
    }

    public double Speed
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }

    public double ValueWeight
    {
        get { return mValueWeight; }
        set { mValueWeight = value; }
    }

    public string TrainName
    {
        get { return mTrainName; }
        set { mTrainName = value; }
    }

    public DateTime Time
    {
        get { return mTime; }
        set { mTime = value; }
    }

    public Int16 Direction
    {
        get { return mDirection; }
        set { mDirection = value; }
    }

    public Int16 Track
    {
        get { return mTrack; }
        set { mTrack = value; }
    }

    public int Coordinate
    {
        get { return mCoordinate; }
        set { mCoordinate = value; }
    }

    public Int64 TrainId
    {
        get { return mTrainId; }
        set { mTrainId = value; }
    }

    public int Start
    {
        get { return mStart; }
        set { mStart = value; }
    }

    public int End
    {
        get { return mEnd; }
        set { mEnd = value; }
    }

    public DateTime DepartureTime
    {
        get
        {
            return mDepartureTime;
        }
        set
        {
            mDepartureTime = value;
        }
    }

    public StopLocation StopLocation
    {
        get { return mStopLocation; }
        set { mStopLocation = value; }
    }

    public Segment SegmentInstance
    {
        get { return mSegment; }
        set { mSegment = value; }
    }

    public StopLocation EndStopLocation
    {
        get
        {
            return mEndStopLocation;
        }

        set
        {
            mEndStopLocation = value;
        }
    }

    public StopLocation StartStopLocation
    {
        get
        {
            return mStartStopLocation;
        }

        set
        {
            mStartStopLocation = value;
        }
    }

    public STATE State
    {
        get
        {
            return mState;
        }

        set
        {
            mState = value;
        }
    }

    public Gene Clone()
    {
        Gene lvRes = new Gene();

        lvRes.mId = mId;
        lvRes.mTrainId = mTrainId;
        lvRes.mTime = mTime;
        lvRes.mSegment = mSegment;
        lvRes.mTrack = mTrack;
        lvRes.mDirection = mDirection;
        lvRes.mCoordinate = mCoordinate;
        lvRes.mStart = mStart;
        lvRes.mEnd = mEnd;
        lvRes.mTrainName = mTrainName;
        lvRes.mStopLocation = mStopLocation;
        lvRes.mStartStopLocation = mStartStopLocation;
        lvRes.mEndStopLocation = mEndStopLocation;
        lvRes.mDepartureTime = mDepartureTime;
        lvRes.mHeadWayTime = mHeadWayTime;
        lvRes.mSpeed = mSpeed;
        lvRes.mValueWeight = mValueWeight;
        lvRes.mState = mState;

        //return this.MemberwiseClone();
        return lvRes;
    }

    public string GetStateString()
    {
        string lvRes = "";

        switch (mState)
        {
            case STATE.IN:
                lvRes = "IN";
                break;
            case STATE.OUT:
                lvRes = "OUT";
                break;
            case STATE.UNDEF:
                lvRes = "";
                break;
            default:
                lvRes = "";
                break;
        }

        return lvRes;
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        lvRes.Append("Gene: ");
        lvRes.Append(mTrainId);
        lvRes.Append(" - ");
        lvRes.Append(mTrainName);
        lvRes.Append(", Location: ");
        lvRes.Append(mSegment.Location);
        lvRes.Append(", UD: ");
        lvRes.Append(mSegment.SegmentValue);
        lvRes.Append(", Direction: ");
        lvRes.Append(mDirection);
        lvRes.Append(", Track: ");
        lvRes.Append(mTrack);
        lvRes.Append(", Time: ");
        lvRes.Append(mTime);
        lvRes.Append(", HeadWayTime: ");
        lvRes.Append(mHeadWayTime);
        lvRes.Append(", Tipo: ");
        lvRes.Append(GetStateString());
        lvRes.Append(", Speed: ");
        lvRes.Append(mSpeed);

        if (mStopLocation != null)
        {
            lvRes.Append(", Stop Location: ");
            lvRes.Append(mStopLocation);
        }

        return lvRes.ToString();
    }
}