using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

class Cluster : IComparable<Cluster>
{
    private IIndividual<Gene> mCenter = null;
    private int mRadius = 0;
    private int mMinRadius = 0;
    private int mUniqueId = -1;
    private double mValue = 0.0;
    private double mCoolingFactor = 0.0;
    private const double MIN_RADIUS_FACTOR = 0.01;

    public Cluster(double pCoolingFactor)
    {
        mCoolingFactor = pCoolingFactor;
        mUniqueId = RuntimeHelpers.GetHashCode(this);
    }

    public Cluster(int pRadius, double pCoolingFactor) : this(pCoolingFactor)
    {
        if (pRadius > mMinRadius)
        {
            mRadius = pRadius;
        }
    }

    public int CompareTo(Cluster pOther)
    {
        int lvRes = 0;

        if (pOther == null) return 1;

        if (pOther.Value == mValue)
        {
            lvRes = 0;
        }
        else if (mValue >= pOther.Value)
        {
            lvRes = -1;
        }
        else if (mValue <= pOther.Value)
        {
            lvRes = 1;
        }

        return lvRes;
    }

    public bool hasMinRadius()
    {
        return (mRadius <= (int)(mCenter.Count * MIN_RADIUS_FACTOR));
    }

    public void AddElement()
    {
        mValue++;
    }

    public void Reset()
    {
        mValue = 0;
    }

    public void Chill()
    {
        mValue *= mCoolingFactor;
    }

    public bool InsideCluster(IIndividual<Gene> pIndividual)
    {
        bool lvRes = false;

        if(pIndividual.GetDistanceFrom(mCenter) <= mRadius)
        {
            lvRes = true;
        }

        return lvRes;
    }

    public int Radius
    {
        get
        {
            return mRadius;
        }

        set
        {
            if (value > mMinRadius)
            {
                mRadius = value;
            }
            else
            {
                mRadius = mMinRadius;
            }
        }
    }

    public IIndividual<Gene> Center
    {
        get
        {
            return mCenter;
        }

        set
        {
            mCenter = value.Clone();

            if(mRadius == 0)
            {
                mMinRadius = (int)(mCenter.Count * MIN_RADIUS_FACTOR);
                mRadius = mMinRadius;
            }
        }
    }

    public int UniqueId
    {
        get
        {
            return mUniqueId;
        }
    }

    public double Value
    {
        get
        {
            return mValue;
        }
    }
}
