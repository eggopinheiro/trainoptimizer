using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for RailRoadFitness
/// </summary>
public class RailRoadFitness : IFitness<TrainMovement>
{
    private double mVMA = 80.0;
    private int mFitnessCallNum = 0;
    private string mStrLineResult = "";
    private string mStrHeaderResult = "";
    private int mFunctionCallReg = 0;
    private string mType = "";
    private double mOptEarlyPenalty;
    private double mOptDelayedPenalty;
    private Population mPopulation = null;

    public RailRoadFitness(double pVMA, string pStrType = "", int pFunctionCallReg = 0)
	{
        mVMA = pVMA;

        if (pStrType.Trim().Length == 0)
        {
            mType = ConfigurationManager.AppSettings["OBJECTIVE_FUNCTION_TYPE"];
        }

        if (pFunctionCallReg == 0)
        {
            if (!Int32.TryParse(ConfigurationManager.AppSettings["FUNCTION_CALL_REG"], out mFunctionCallReg))
            {
                mFunctionCallReg = 0;
            }
        }
        else
        {
            mFunctionCallReg = pFunctionCallReg;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["OPT_EARLY_PENALTY"], out mOptEarlyPenalty))
        {
            mOptEarlyPenalty = 1.0;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["OPT_DELAYED_PENALTY"], out mOptDelayedPenalty))
        {
            mOptDelayedPenalty = 1.0;
        }
    }

    public double GetFitness(IIndividual<TrainMovement> pIndividual)
    {
        double lvRes = 0.0;

        if ((pIndividual == null) || (pIndividual.GetUniqueId() == -1))
        {
            return double.MaxValue;
        }

        mFitnessCallNum++;

        if ((mFitnessCallNum % mFunctionCallReg) == 0)
        {
            if (mPopulation != null)
            {
#if DEBUG
                DebugLog.Logar(DateTime.Now + " => Fitness Called Num = " + mFitnessCallNum, false);
                Population.dump(mPopulation.GetBestIndividual(), null);

                if (mStrHeaderResult.Length == 0)
                {
                    mStrHeaderResult = "Call Num. " + FitnessCallNum;
                    mStrLineResult = mPopulation.GetBestIndividual().GetFitness().ToString();
                }
                else
                {
                    mStrHeaderResult += "|Call Num. " + FitnessCallNum;
                    mStrLineResult += "|" + mPopulation.GetBestIndividual().GetFitness();
                }
#else
                lock(this)
                {
                    ElapsedTimeDataAccess.Update(mPopulation.UniqueId, DateTime.MinValue, mFitnessCallNum, mPopulation.GetBestIndividual().GetFitness(), mPopulation.CurrentGeneration, mPopulation.Count, mPopulation.HillClimbingCallReg);
                }
#endif
            }
        }

        switch (mType)
        {
            case "TT":
                lvRes = GetFitnessTT(pIndividual);
                break;
            case "THP":
                lvRes = GetFitnessTHP(pIndividual);
                break;
            default:
                lvRes = GetFitnessTT(pIndividual);
                break;
        }

        return lvRes;
    }

    public double GetFitnessTHP(IIndividual<TrainMovement> pIndividual)
    {
        Dictionary<Int64, DateTime> lvDicTrainTime = new Dictionary<Int64, DateTime>();
        Gene lvGene = null;
        double lvRes = 0.0;
        double lvElapsedTime = 0.0;

        try
        {
            foreach (TrainMovement lvTrainMov in pIndividual)
            {
                for (int i = 0; i < lvTrainMov.Count; i++)
                {
                    lvGene = lvTrainMov[i];

                    if (lvGene.State == Gene.STATE.IN)
                    {
                        if (!lvDicTrainTime.ContainsKey(lvGene.TrainId))
                        {
                            lvDicTrainTime.Add(lvGene.TrainId, lvGene.HeadWayTime);
                        }
                        else
                        {
                            lvDicTrainTime[lvGene.TrainId] = lvGene.HeadWayTime;
                        }
                    }
                    else if (lvGene.State == Gene.STATE.OUT)
                    {
                        if (lvDicTrainTime.ContainsKey(lvGene.TrainId))
                        {
                            lvElapsedTime = (lvGene.HeadWayTime - lvDicTrainTime[lvGene.TrainId]).TotalHours;
                            if (lvElapsedTime > 0)
                            {
                                lvRes += lvGene.ValueWeight * Math.Pow(2.0, lvElapsedTime);
                            }
                        }
                    }
                }
            }

            if (lvDicTrainTime.Count > 0)
            {
                lvRes = lvRes / lvDicTrainTime.Count;
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    public double GetFitnessTT(IIndividual<TrainMovement> pIndividual)
    {
        Dictionary<Int64, FitnessElement> lvDicTrainTime = new Dictionary<Int64, FitnessElement>();
        FitnessElement lvFitnessElement = null;
        double lvRes = 0.0;
        double lvOpt = double.MaxValue;
        double lvTotalOpt = 0.0;
        double lvTotalTime = 0.0;
        int lvCount = 0;

        try
        {
            foreach (TrainMovement lvTrainMov in pIndividual)
            {
                foreach(Gene lvGene in lvTrainMov)
                {
                    if (!lvDicTrainTime.ContainsKey(lvGene.TrainId))
                    {
                        lvFitnessElement = new FitnessElement();

                        lvFitnessElement.TrainId = lvGene.TrainId;
                        lvFitnessElement.InitialTime = lvGene.Time;
                        lvFitnessElement.ValueWeight = lvGene.ValueWeight;
                        lvFitnessElement.EndStopLocation = lvGene.EndStopLocation;

                        if (lvGene.OptimumTime > DateTime.MinValue)
                        {
                            if (lvGene.StopLocation != null)
                            {
                                if (lvGene.Direction > 0)
                                {
                                    lvOpt = (Math.Abs(lvGene.EndStopLocation.Start_coordinate - lvGene.StopLocation.End_coordinate) / 100000.0) / mVMA;
                                }
                                else
                                {
                                    lvOpt = (Math.Abs(lvGene.EndStopLocation.End_coordinate - lvGene.StopLocation.Start_coordinate) / 100000.0) / mVMA;
                                }
                            }
                            else
                            {
                                if (lvGene.Direction > 0)
                                {
                                    lvOpt = (Math.Abs(lvGene.EndStopLocation.Start_coordinate - lvGene.Coordinate) / 100000.0) / mVMA;
                                }
                                else
                                {
                                    lvOpt = (Math.Abs(lvGene.EndStopLocation.End_coordinate - lvGene.Coordinate) / 100000.0) / mVMA;
                                }
                            }
                            lvTotalOpt += lvOpt;
                            //lvOpt = TrainIndividual.GetOptimum(lvGene);

                            lvFitnessElement.Optimun = lvOpt;
                            lvFitnessElement.HasMinTimeTarget = true;
                        }
                        else
                        {
                            lvFitnessElement.Optimun = (lvGene.OptimumTime - lvGene.Time).TotalHours;
                            lvFitnessElement.HasMinTimeTarget = false;
                        }

                        lvDicTrainTime.Add(lvGene.TrainId, lvFitnessElement);

                        lvFitnessElement = null;
                    }
                    else
                    {
                        lvFitnessElement = lvDicTrainTime[lvGene.TrainId];
                        if (lvGene.Time > lvFitnessElement.EndTime)
                        {
                            lvFitnessElement.EndTime = lvGene.Time;
                            lvFitnessElement.CurrentStopLocation = lvGene.StopLocation;
                            lvFitnessElement.CurrentGene = lvGene;
                        }
                    }
                }
            }

            foreach (FitnessElement lvFitnessElem in lvDicTrainTime.Values)
            {
                lvTotalTime = (lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours;
                if (lvFitnessElem.Optimun > 0)
                {
                    if (lvFitnessElement.HasMinTimeTarget)
                    {
                        //lvRes += lvFitnessElem.ValueWeight * ((lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours - lvFitnessElem.Optimun) / lvFitnessElem.Optimun;
                        lvRes += lvFitnessElem.ValueWeight * (lvTotalTime - lvFitnessElem.Optimun);
                    }
                    else
                    {
                        if(lvTotalTime < lvFitnessElem.Optimun)
                        {
                            lvRes += lvFitnessElem.ValueWeight * (lvFitnessElem.Optimun - lvTotalTime) * mOptEarlyPenalty;
                        }
                        else if(lvTotalTime > lvFitnessElem.Optimun)
                        {
                            lvRes += lvFitnessElem.ValueWeight * (lvTotalTime - lvFitnessElem.Optimun) * mOptDelayedPenalty;
                        }
                    }

                    if ((lvTotalTime - lvFitnessElem.Optimun) < 0)
                    {
#if DEBUG
                        DebugLog.Logar("RailRoadFitness => Trem: " + lvFitnessElem.TrainId);
                        DebugLog.Logar("(lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours = " + (lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours);
                        DebugLog.Logar("lvFitnessElem.Optimun = " + lvFitnessElem.Optimun);
                        //((TrainIndividual)pIndividual).GenerateFlotFiles(DebugLog.LogPath);
                        //((TrainIndividual)pIndividual).Dump(lvFitnessElem.TrainId, lvFitnessElement.EndStopLocation);
#endif
                        lvRes += lvFitnessElem.ValueWeight * ((lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours + lvFitnessElem.Optimun * 10);
                        //lvRes += (lvFitnessElem.Optimun * 100);
                    }
                    lvCount++;
                }
            }

            if (lvCount > 0)
            {
                lvRes = lvRes / lvCount;

                if (lvRes < 0.0)
                {
                    lvRes = Double.MaxValue;
                }
                else
                {
                    pIndividual.GBest = lvTotalOpt / lvCount;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    public int FitnessCallNum
    {
        get
        {
            return mFitnessCallNum;
        }
    }

    public Population Population
    {
        get
        {
            return mPopulation;
        }

        set
        {
            mPopulation = value;
        }
    }

    public string LineResult
    {
        get
        {
            return mStrLineResult;
        }

        set
        {
            mStrLineResult = value;
        }
    }

    public string HeaderResult
    {
        get
        {
            return mStrHeaderResult;
        }

        set
        {
            mStrHeaderResult = value;
        }
    }

    public string Type
    {
        get
        {
            return mType;
        }

        set
        {
            mType = value;
        }
    }

    private class FitnessElement : IEquatable<FitnessElement>
    {
        private Int64 mTrainId = 0L;
        private DateTime mInitialTime = DateTime.MinValue;
        private DateTime mEndTime = DateTime.MinValue;
        private double mOptimun = 0.0;
        private bool mHasMinTimeTarget;
        private double mValueWeight = 1.0;
        private Gene mCurrentGene = null;
        private StopLocation mCurrentStopLocation = null;
        private StopLocation mEndStopLocation = null;

        public double ValueWeight
        {
            get { return mValueWeight; }
            set { mValueWeight = value; }
        }

        public Int64 TrainId
        {
            get { return mTrainId; }
            set { mTrainId = value; }
        }

        public DateTime InitialTime
        {
            get { return mInitialTime; }
            set { mInitialTime = value; }
        }

        public DateTime EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; }
        }

        public double Optimun
        {
            get { return mOptimun; }
            set { mOptimun = value; }
        }

        public StopLocation EndStopLocation
        {
            get { return mEndStopLocation; }
            set { mEndStopLocation = value; }
        }

        public StopLocation CurrentStopLocation
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

        public Gene CurrentGene
        {
            get
            {
                return mCurrentGene;
            }

            set
            {
                mCurrentGene = value;
            }
        }

        public bool HasMinTimeTarget
        {
            get
            {
                return mHasMinTimeTarget;
            }

            set
            {
                mHasMinTimeTarget = value;
            }
        }

        public static bool operator ==(FitnessElement obj1, FitnessElement obj2)
        {
            bool lvRes = false;

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }

            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            if ((obj1.TrainId == obj2.TrainId))
            {
                lvRes = true;
            }

            return lvRes;
        }

        public static bool operator !=(FitnessElement obj1, FitnessElement obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(FitnessElement other)
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

            if ((this.TrainId == other.TrainId) && (this.InitialTime == other.InitialTime) && (this.EndTime == other.EndTime))
            {
                lvRes = true;
            }

            return lvRes;
        }

        public override bool Equals(object obj)
        {
            bool lvRes = false;

            if (obj is FitnessElement)
            {
                lvRes = Equals(obj as FitnessElement);
            }

            return lvRes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int lvHashCode = 0;

                if (TrainId > 0.0)
                {
                    lvHashCode = TrainId.GetHashCode();
                }

                return lvHashCode;
            }
        }
    }
}