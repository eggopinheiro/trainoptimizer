using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// Summary description for Population
/// </summary>
public class Population : IEnumerable<IIndividual<TrainMovement>>
{
    private List<IIndividual<TrainMovement>> mIndividuals = null;
    private List<IIndividual<TrainMovement>> mClustersCenter = null;
    private static Dictionary<string, double> mPriority = new Dictionary<string, double>();
    private Dictionary<int, List<IIndividual<TrainMovement>>> mClusterAssignment = null;
    private static Dictionary<Int64, List<Trainpat>> mPATs = null;
    private Dictionary<Int64, Gene[]> mTrainSequence = null;
    private IFitness<TrainMovement> mFitness = null;
    private DateTime mInitialDate;
    private DateTime mFinalDate;
    private DateTime mDateRef;
    private List<TrainMovement> mTrainList = null;
    private List<TrainMovement> mPlanList = null;
    private List<Cluster> mClusters = null;
    private bool mAllowNoDestinationTrain = true;
    private int mMutationRate = 5;
    private int mSize = 0;
    private int mMaxObjectiveFunctionCall = 0;
    private int mHillClimbingCallReg = 0;
    private int mFunctionCallReg = 0;
    private int mCrossOverPoints = 0;
    private double mNicheDistance = 0.0;
    private int mLSSteps = 1;
    private int mLSNeighbors = 1;
    private double mLSImprovement = 0.0;
    private int mMaxMutationSteps = 50;
    private int mMinMutationSteps = 1;
    private static int mSeed = -1;
    private bool mECS = false;
    private int mECSClusters = 1;
    private int mECSClusterEliteValue = 1;
    private double mECSClusterElite = 1.0;
    private HashSet<int> mECSClusterCentersSet = null;
    private int mECSClustersMaxAge = 1;
    private double mECSFadeFactor = 0.0;
    private int mCurrentGeneration = 0;
    private int mECSAMWakeUp = 1;
    private bool mECSJoinCloserCluster = true;
    private double mECSFactorRefValue = 0.0;
    private double mECSFactorMax = 0.0;
    private double mECSFactorMin = 0.0;
    private double mECSMaxValue = 0.0;
    private double mECSMinValue = 0.0;
    private double mECSRemoveFactor = 0.0;
    private bool mECSOnlyHeated = false;
    private bool mAllowDeadLockIndividual = false;
    private double mBestFitness = Double.MaxValue;
    private double mStartDelayed = 0.0;
    private int mECSRemoveValue = 0;
    private int mUniqueId = -1;
    private DELAYED_FIX_METHOD mDelayedFixedMethod = DELAYED_FIX_METHOD.ALL;
    private Random mRandom = null;

    private Stopwatch mStopWatch = null;

    private enum DELAYED_FIX_METHOD
    {
        NONE,
        ALL,
        TRACK
    }

    private enum LS_STRATEGY_ENUM
    {
        None,
        GradientDescent,
        HillClimbingBest
    }

    private enum SELECTION_ENUM
    {
        Roulette,
        Tournament
    }

    private enum DISTANCE_ENUM
    {
        LIS,
        Fitness
    }

    private enum ECS_ASSIMILATION_MODE
    {
        FITNESS,
        DISTANCE
    }

    private LS_STRATEGY_ENUM mLSStrategy = LS_STRATEGY_ENUM.None;
    private SELECTION_ENUM mSelectionMode = SELECTION_ENUM.Roulette;
    private ECS_ASSIMILATION_MODE mECSAssimilationMode = ECS_ASSIMILATION_MODE.FITNESS;
    private int mSelectionModeCount = 2;
    private static double ELITE_PERC = 0.2;
    private static int mMinCrossOverPoints = 2;
    private static int mMaxCrossOverPoints = 2;
    private static int mMaxParallelThread = 1;
    private static int mMaxDeadLockError = 100;
    private static HashSet<string> mTrainAllowed = new HashSet<string>();

    public Population(IFitness<TrainMovement> pFitness, int pSize, int pMutationRate, int pCrossOverPoints, List<TrainMovement> pTrainList, List<TrainMovement> pPlanList, DateTime pInitialDate = default(DateTime), DateTime pFinalDate = default(DateTime), Dictionary<Int64, List<Trainpat>> pPATs = null)
	{
        mDateRef = DateTime.MinValue;
        mSeed = DateTime.Now.Millisecond;
        mRandom = new Random();
        Cluster lvCluster = null;
        mUniqueId = RuntimeHelpers.GetHashCode(this);

        mFitness = pFitness;
        //((RailRoadFitness)mFitness).Population = this;
        mMutationRate = pMutationRate;
        mCrossOverPoints = pCrossOverPoints;
        mInitialDate = pInitialDate;
        mFinalDate = pFinalDate;
        mClusterAssignment = new Dictionary<int, List<IIndividual<TrainMovement>>>();

        if (!bool.TryParse(ConfigurationManager.AppSettings["ALLOW_NO_DESTINATION_TRAIN"], out mAllowNoDestinationTrain))
        {
            mAllowNoDestinationTrain = false;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["MAX_OBJECTIVE_FUNCTION_CALL"], out mMaxObjectiveFunctionCall))
        {
            mMaxObjectiveFunctionCall = 0;
        }

        int lvMaxGeneration;
        if (!Int32.TryParse(ConfigurationManager.AppSettings["MAX_GENERATIONS"], out lvMaxGeneration))
        {
            lvMaxGeneration = 0;
        }

        int lvNumLoadIndividuals;
        if (!Int32.TryParse(ConfigurationManager.AppSettings["NUM_LOAD_INDIVIDUALS"], out lvNumLoadIndividuals))
        {
            lvNumLoadIndividuals = 0;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["START_DELAYED"], out mStartDelayed))
        {
            mStartDelayed = 0.0;
        }

        double lvLocalSearchFactor;
        if (!double.TryParse(ConfigurationManager.AppSettings["LS_FACTOR"], out lvLocalSearchFactor))
        {
            lvLocalSearchFactor = 0.025;
        }

        if (mMaxObjectiveFunctionCall > 0)
        {
            mLSSteps = (int)Math.Sqrt(mMaxObjectiveFunctionCall * lvLocalSearchFactor);
            mLSNeighbors = mLSSteps;
        }
        else
        {
            mLSSteps = (int)Math.Sqrt(lvMaxGeneration * 1000 * lvLocalSearchFactor);
            mLSNeighbors = mLSSteps;
        }

        double lvMaxMutationSteps = 0.0;
        if (!double.TryParse(ConfigurationManager.AppSettings["MAX_MUTATION_STEPS"], out lvMaxMutationSteps))
        {
            lvMaxMutationSteps = 0.015;
        }

        double lvMinMutationSteps = 0.0;
        if (!double.TryParse(ConfigurationManager.AppSettings["MIN_MUTATION_STEPS"], out lvMinMutationSteps))
        {
            lvMinMutationSteps = 0.005;
        }

        if (!bool.TryParse(ConfigurationManager.AppSettings["ALLOW_DEADLOCK_INDIVIDUAL"], out mAllowDeadLockIndividual))
        {
            mAllowDeadLockIndividual = false;
        }

        if (!bool.TryParse(ConfigurationManager.AppSettings["ECS"], out mECS))
        {
            mECS = false;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["ECS_ANALYZER_MODULE_WAKEUP"], out mECSAMWakeUp))
        {
            mECSAMWakeUp = 1;
        }

        if (!bool.TryParse(ConfigurationManager.AppSettings["ECS_JOIN_CLOSER_CLUSTER"], out mECSJoinCloserCluster))
        {
            mECSJoinCloserCluster = true;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["ECS_CLUSTERS"], out mECSClusters))
        {
            mECSClusters = 10;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["ECS_CLUSTERS_MAX_AGE"], out mECSClustersMaxAge))
        {
            mECSClustersMaxAge = 1;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["ECS_FADE_FACTOR"], out mECSFadeFactor))
        {
            mECSFadeFactor = 0.9;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["ECS_REMOVE_FACTOR"], out mECSRemoveFactor))
        {
            mECSRemoveFactor = 0.01;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["ECS_MAX_FACTOR"], out mECSFactorMax))
        {
            mECSFactorMax = 0.0;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["ECS_MIN_FACTOR"], out mECSFactorMin))
        {
            mECSFactorMin = 0.0;
        }
        
        if (!bool.TryParse(ConfigurationManager.AppSettings["ECS_ONLY_HEATED"], out mECSOnlyHeated))
        {
            mECSOnlyHeated = false;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["SELECTION_COUNT"], out mSelectionModeCount))
        {
            mSelectionModeCount = 2;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["ECS_CLUSTER_ELITE"], out mECSClusterElite))
        {
            mECSClusterElite = 1.0;
        }
        mECSClusterEliteValue = (int)(mECSClusters * mECSClusterElite);

        if (!double.TryParse(ConfigurationManager.AppSettings["LS_IMPROVEMENT"], out mLSImprovement))
        {
            mLSImprovement = 0.0;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["FUNCTION_CALL_REG"], out mFunctionCallReg))
        {
            mFunctionCallReg = 0;
        }

        string lvSelectionMode = ConfigurationManager.AppSettings["SELECTION_MODE"];

        switch (lvSelectionMode)
        {
            case "roulette":
                mSelectionMode = SELECTION_ENUM.Roulette;
                break;
            case "tournament":
                mSelectionMode = SELECTION_ENUM.Tournament;
                break;
            default:
                mSelectionMode = SELECTION_ENUM.Roulette;
                break;
        }

        string lvLSStrategy = ConfigurationManager.AppSettings["LS_STRATEGY"];

        switch (lvLSStrategy)
        {
            case "gds":
                mLSStrategy = LS_STRATEGY_ENUM.GradientDescent;
                break;
            case "best":
                mLSStrategy = LS_STRATEGY_ENUM.HillClimbingBest;
                break;
            default:
                mLSStrategy = LS_STRATEGY_ENUM.None;
                break;
        }

        string lvAssimilationMode = ConfigurationManager.AppSettings["ECS_ASSIMILATION_MODE"];

        switch (lvAssimilationMode)
        {
            case "fitness":
                mECSAssimilationMode = ECS_ASSIMILATION_MODE.FITNESS;
                break;
            case "distance":
                mECSAssimilationMode = ECS_ASSIMILATION_MODE.DISTANCE;
                break;
            default:
                mECSAssimilationMode = ECS_ASSIMILATION_MODE.FITNESS;
                break;
        }

        string lvDelayedFixMethod = ConfigurationManager.AppSettings["DELAYED_FIX_METHOD"];

        switch (lvDelayedFixMethod)
        {
            case "all":
                mDelayedFixedMethod = DELAYED_FIX_METHOD.ALL;
                break;
            case "track":
                mDelayedFixedMethod = DELAYED_FIX_METHOD.TRACK;
                break;
            default:
                mDelayedFixedMethod = DELAYED_FIX_METHOD.ALL;
                break;
        }

        if ((pTrainList == null) && (pPlanList == null))
        {
            if (DateTime.Now.Date == mInitialDate.Date)
            {
                mDateRef = DateTime.Now;
            }
            else
            {
                mDateRef = DateTime.MinValue;
                foreach (TrainMovement lvTrainMov in mTrainList)
                {
                    foreach (Gene lvGene in lvTrainMov)
                    {
                        if (lvGene.Time > mDateRef)
                        {
                            mDateRef = lvGene.Time;
                        }
                    }
                }

                if ((mDateRef == DateTime.MinValue) && (mPlanList.Count > 0))
                {
                    mDateRef = mPlanList[0][0].DepartureTime;
                }
            }

            LoadTrainList();
        }
        else
        {
            Gene lvGeneSeq = null;

            mTrainList = new List<TrainMovement>(pTrainList);
            mPlanList = new List<TrainMovement>(pPlanList);
            mPATs = pPATs;

            foreach (TrainMovement lvTrainMov in mTrainList)
            {
                foreach (Gene lvGene in lvTrainMov)
                {
                    lvGene.DepartureTime = lvGene.Time;
                    if (lvGene.Time > mDateRef)
                    {
                        mDateRef = lvGene.Time;
                    }
                }

                lvGeneSeq = LoadTrainSequence(lvTrainMov.Last);

                if (lvGeneSeq != null)
                {
                    lvTrainMov.Last.EndStopLocation = lvGeneSeq.EndStopLocation;
                    lvTrainMov.Last.End = lvGeneSeq.End;
                    lvTrainMov.Last.OptimumTime = lvGeneSeq.OptimumTime;
                }
            }

            if ((mDateRef == DateTime.MinValue) && (mPlanList.Count > 0))
            {
                mDateRef = mPlanList[0][0].DepartureTime;
            }

            foreach(TrainMovement lvPlanMov in mPlanList)
            {
                lvGeneSeq = LoadTrainSequence(lvPlanMov.Last);

                if (lvGeneSeq != null)
                {
                    lvPlanMov.Last.EndStopLocation = lvGeneSeq.EndStopLocation;
                    lvPlanMov.Last.End = lvGeneSeq.End;
                    lvPlanMov.Last.OptimumTime = lvGeneSeq.OptimumTime;
                }
            }
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: mCurrentGeneration);
            DebugLog.Logar("Gerando Individuos:", pIndet: mCurrentGeneration);
        }
#endif

        mSize = pSize;

        mIndividuals = new List<IIndividual<TrainMovement>>();
        TrainIndividual.IDLog = 0;

#if DEBUG
        mStopWatch = new Stopwatch();

        mStopWatch.Start();
#endif

        List<int> lvSavedIndividualsIds = ListIndividualsToLoad();

        if (MAX_PARALLEL_THREADS > 1)
        {
            Parallel.For(0, lvSavedIndividualsIds.Count, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, i =>
            {
                IIndividual<TrainMovement> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, lvSavedIndividualsIds[i], mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    lock (mIndividuals)
                    {
                        mIndividuals.Add(lvIndividual);
                    }
                }
            });

            Parallel.For(0, pSize, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, i =>
            {
                IIndividual<TrainMovement> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, 0, mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    lock(mIndividuals)
                    {
                        mIndividuals.Add(lvIndividual);
                    }
                }
            });
        }
        else
        {
            for (int i = 0; i < lvSavedIndividualsIds.Count; i++)
            {
                IIndividual<TrainMovement> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, lvSavedIndividualsIds[i], mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    mIndividuals.Add(lvIndividual);
                }
            }

            for (int i = 0; i < pSize; i++)
            {
                IIndividual<TrainMovement> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, 0, mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    mIndividuals.Add(lvIndividual);
                    //((TrainIndividual)lvIndividual).GenerateFlotFiles(DebugLog.LogPath);
                }
            }
        }

#if DEBUG
        mStopWatch.Stop();

        DebugLog.Logar("mStopWatch for GenerateIndividual = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);
#endif

        TrainIndividual.IDLog = 0;

        if (mIndividuals.Count == 0)
        {
            DebugLog.Save("Erro: Nenhum individuo foi criado no processo !");
        }
        else
        {
            mIndividuals.Sort();

#if DEBUG
            //DebugLog.Logar("Best Individual = " + mIndividuals[0], false, pIndet: mCurrentGeneration);
            //((TrainIndividual)mIndividuals[0]).GenerateFlotFiles(ConfigurationManager.AppSettings["LOG_PATH"] + "Logs\\");
#endif
        }

        if (mECS && (mECSClusters <= mIndividuals.Count))
        {
            if (mECSClusters > 0)
            {
                mECSFactorRefValue = pSize / mECSClusters;
                mECSMaxValue = mECSFactorRefValue * mECSFactorMax;
                mECSMinValue = mECSFactorRefValue * mECSFactorMin;
                mClustersCenter = new List<IIndividual<TrainMovement>>();
                mECSClusterCentersSet = new HashSet<int>();
                mClusters = new List<Cluster>(mECSClusters);
                for (int i = 0; i < mIndividuals.Count; i++)
                {
                    lvCluster = new Cluster(mECSFadeFactor);
                    for (int ind = 0; ind < mClusters.Count; ind++)
                    {
                        if (mClusters[ind].InsideCluster(mIndividuals[i]))
                        {
                            lvCluster = null;
                            break;
                        }
                    }

                    if (lvCluster != null)
                    {
                        lvCluster.Center = mIndividuals[i];
                        mECSClusterCentersSet.Add(mIndividuals[i].GetUniqueId());
                        mClustersCenter.Add(mIndividuals[i]);
                        mClusters.Add(lvCluster);
                        lvCluster = null;
                    }

                    if (mClusters.Count >= mECSClusters)
                    {
                        break;
                    }
                }

                CalculateRadius();

                IIndividual<TrainMovement> lvIndividual = mIndividuals[0];
                mECSRemoveValue = (int)(lvIndividual.Count * mECSRemoveFactor);
            }
        }
        else
        {
            mECS = false;
        }

        /*
#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: mCurrentGeneration);
        }

        
        TrainIndividual lvGeneIndividual = (TrainIndividual)GetBestIndividual();
        lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);
        

//        DebugLog.EnableDebug = true;
#endif
*/

        if (mIndividuals.Count > 0)
        {
            mBestFitness = mIndividuals[0].Fitness;

            if (lvMinMutationSteps <= 0.0)
            {
                mMinMutationSteps = 1;
            }
            else
            {
                mMinMutationSteps = (int)(mIndividuals[0].Count * lvMinMutationSteps);
            }

            if (lvMaxMutationSteps <= 0.0)
            {
                mMaxMutationSteps = mMinMutationSteps + 1;
            }
            else
            {
                mMaxMutationSteps = (int)(mIndividuals[0].Count * lvMaxMutationSteps);
            }

            if(mMinMutationSteps > mMaxMutationSteps)
            {
                mMinMutationSteps = mMaxMutationSteps;
            }

            //Dump(mIndividuals[0]);
        }
    }

    private List<int> ListIndividualsToLoad()
    {
        DirectoryInfo lvDir;
        FileInfo[] lvFiles;
        List<int> lvRes = new List<int>();
        int lvUniqueId;
        int lvIndexStart;
        int lvIndexEnd;

        int lvNumSavedIndividuals;

        if (!Int32.TryParse(ConfigurationManager.AppSettings["NUM_LOADED_INDIVIDUALS"], out lvNumSavedIndividuals))
        {
            lvNumSavedIndividuals = 0;
        }

        try
        {
            lvDir = new DirectoryInfo(ConfigurationManager.AppSettings["LOG_PATH"] + "Data\\");
            lvFiles = lvDir.GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();

            foreach (FileInfo file in lvFiles)
            {
                if (lvNumSavedIndividuals > 0)
                {
                    if (file.Name.StartsWith("Individual_") && file.Name.EndsWith(".bin"))
                    {
                        lvIndexStart = file.Name.IndexOf("_") + 1;
                        lvIndexEnd = file.Name.IndexOf(".");
                        if (lvIndexEnd > lvIndexStart)
                        {
                            lvUniqueId = Convert.ToInt32(file.Name.Substring(lvIndexStart, lvIndexEnd - lvIndexStart));
                            lvRes.Add(lvUniqueId);

                            lvNumSavedIndividuals--;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    public void SaveBestIndividuals()
    {
        DirectoryInfo lvDir;
        FileInfo[] lvFiles;
        int lvNumSavedIndividuals;
        IIndividual<TrainMovement> lvIndividual;

        if (!Int32.TryParse(ConfigurationManager.AppSettings["NUM_SAVED_INDIVIDUALS"], out lvNumSavedIndividuals))
        {
            lvNumSavedIndividuals = 0;
        }

        try
        {
            lvDir = new DirectoryInfo(ConfigurationManager.AppSettings["LOG_PATH"] + "Data\\");
            lvFiles = lvDir.GetFiles();

            foreach (FileInfo file in lvFiles)
            {
                file.Delete();
            }

            if ((mIndividuals.Count > lvNumSavedIndividuals) && (mIndividuals.Count > 0) && (lvNumSavedIndividuals > 0))
            {
                mIndividuals.Sort();
                for (int i = 0; i < lvNumSavedIndividuals; i++)
                {
                    lvIndividual = mIndividuals[i];

                    if (lvIndividual != null)
                    {
                        lvIndividual.Serialize();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }
    }

    private void LoadPATs(Int64 pTrainId)
    {
        DataSet ds = TrainpatDataAccess.GetPATTrain(pTrainId);
        List<Trainpat> lvTrainPat = null;

        mPATs = new Dictionary<Int64, List<Trainpat>>();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            Trainpat lvPat = new Trainpat();

            lvPat.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["coordinate"]));
            lvPat.KM = ((row["km"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["km"].ToString()));
            lvPat.Duration = ((row["duration"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["duration"]));
            lvPat.Activity = ((row["definition"] == DBNull.Value) ? "" : row["definition"].ToString());
            lvPat.Date_hist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));

            if (!mPATs.ContainsKey(pTrainId))
            {
                lvTrainPat = new List<Trainpat>();
                lvTrainPat.Add(lvPat);
                mPATs.Add(pTrainId, lvTrainPat);
            }
            else
            {
                mPATs[pTrainId].Add(lvPat);
            }
        }

        //DebugLog.Logar("LoadPATs.mPATs.Count = " + mPATs.Count);
    }

    public static void AddPAT(Int64 pID, Trainpat pPAT)
    {
        if (mPATs == null)
        {
            mPATs = new Dictionary<Int64, List<Trainpat>>();
        }

        if(!mPATs.ContainsKey(pID))
        {
            mPATs.Add(pID, new List<Trainpat>());
        }
        mPATs[pID].Add(pPAT);
    }

    public bool UseMaxObjectiveFunctionCall()
    {
        bool lvRes = false;

        if (mMaxObjectiveFunctionCall > 0)
        {
            lvRes = true;
        }

        return lvRes;
    }

    public IIndividual<TrainMovement> this[int i]
    {
        get
        {
            if ((i > 0) && (i < mIndividuals.Count))
            {
                return mIndividuals[i];
            }
            else
            {
                return null;
            }
        }
    }

    public IEnumerator<IIndividual<TrainMovement>> GetEnumerator()
    {
        return mIndividuals.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public bool HasMaxObjectiveFunctionCallReached()
    {
        bool lvRes = false;

        if ((mMaxObjectiveFunctionCall > 0) && (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall))
        {
            lvRes = true;
        }

        return lvRes;
    }

    private bool KeepingDiversity(IIndividual<TrainMovement> pIndividual)
    {
        bool lvRes = true;
        Cluster lvCloserCluster = null;
        Cluster lvCluster = null;
        int lvMinDistance = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;

        try
        {
            for (int i = 0; i < mClusters.Count; i++)
            {
                lvCluster = mClusters[i];

                lvDistance = lvCluster.Center.GetDistanceFrom(pIndividual);

                if (lvDistance < lvMinDistance)
                {
                    lvMinDistance = lvDistance;
                    lvCloserCluster = lvCluster;
                }
            }

            if (lvMinDistance <= mECSRemoveValue)
            {
                lvRes = false;
                //DebugLog.Logar("Removendo individuo " + pIndividual.GetUniqueId() + "(" + pIndividual.Fitness + ") por estar muito proximo do centro de " + lvCloserCluster.Center.GetUniqueId() + " (distancia " + lvMinDistance + " < " + mECSRemoveValue + ")", false, pIndet: mCurrentGeneration);
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    private void AssignIndividualToCluster(IIndividual<TrainMovement> pIndividual)
    {
        Cluster lvCluster = null;
        List<IIndividual<TrainMovement>> lvElements = null;
        int lvDistance = Int32.MaxValue;
        int lvMinDistance = Int32.MaxValue;
        int lvClusterIndex = -1;

        lvClusterIndex = -1;

        try
        {
            for (int i = 0; i < mClusters.Count; i++)
            {
                lvCluster = mClusters[i];

                lvDistance = lvCluster.Center.GetDistanceFrom(pIndividual);

                if (lvDistance < lvMinDistance)
                {
                    lvMinDistance = lvDistance;
                    lvClusterIndex = i;
                }
            }

            if (mECSJoinCloserCluster)
            {
                lock(mClusterAssignment)
                {
                    if (mClusterAssignment.ContainsKey(lvClusterIndex))
                    {
                        lvElements = mClusterAssignment[lvClusterIndex];
                        lvElements.Add(pIndividual);
                    }
                    else
                    {
                        lvElements = new List<IIndividual<TrainMovement>>();

                        lvElements.Add(pIndividual);
                        lock(mClusterAssignment)
                        {
                            mClusterAssignment.Add(lvClusterIndex, lvElements);
                        }
                    }
                }

                lock(lvCluster)
                {
                    lvCluster.AddElement();
                }
            }
            else if (lvMinDistance <= mClusters[lvClusterIndex].Radius)
            {
                lock (mClusterAssignment)
                {
                    if (mClusterAssignment.ContainsKey(lvClusterIndex))
                    {
                        lvElements = mClusterAssignment[lvClusterIndex];
                        lvElements.Add(pIndividual);
                    }
                    else
                    {
                        lvElements = new List<IIndividual<TrainMovement>>();

                        lvElements.Add(pIndividual);
                        lock(mClusterAssignment)
                        {
                            mClusterAssignment.Add(lvClusterIndex, lvElements);
                        }
                    }
                }

                lock (lvCluster)
                {
                    lvCluster.AddElement();
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }
    }

    private void UpdateClusters(List<IIndividual<TrainMovement>> pIndividuals)
    {
        IIndividual<TrainMovement> lvIndividual;
        Cluster lvCluster;

        try
        {
            if (mClusters.Count < mECSClusters)
            {
#if DEBUG
                mStopWatch.Reset();
                mStopWatch.Start();
#endif

                for (int i = 0; i < pIndividuals.Count; i++)
                {
                    lvIndividual = pIndividuals[i];
                    if (!mECSClusterCentersSet.Contains(lvIndividual.GetUniqueId()))
                    {
                        lvCluster = new Cluster(mECSFadeFactor);
                        lvCluster.Center = lvIndividual;
                        mECSClusterCentersSet.Add(lvIndividual.GetUniqueId());
                        mClusters.Add(lvCluster);
                        mClustersCenter.Add(lvIndividual);
                        lvCluster = null;

                        if (mClusters.Count >= mECSClusters)
                        {
                            break;
                        }
                    }
                }

                CalculateRadius();

#if DEBUG
                mStopWatch.Stop();

                DebugLog.Logar("mStopWatch for Creating new clusters = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);
                DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: mCurrentGeneration);
#endif
            }

            mClusterAssignment.Clear();

#if DEBUG
            mStopWatch.Reset();
            mStopWatch.Start();
#endif

            if (MAX_PARALLEL_THREADS > 1)
            {
                Parallel.For(0, pIndividuals.Count, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, (i, loopState) =>
                {
                    IIndividual<TrainMovement> lvThreadIndividual = pIndividuals[i];
                    AssignIndividualToCluster(lvThreadIndividual);
                });
            }
            else
            {
                for (int i = 0; i < pIndividuals.Count; i++)
                {
                    lvIndividual = pIndividuals[i];
                    AssignIndividualToCluster(lvIndividual);
                }
            }

#if DEBUG
            mStopWatch.Stop();

            DebugLog.Logar("mStopWatch for AssignIndividualToCluster = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);

            /*
            DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: mCurrentGeneration);
            foreach (int clusterNum in mClusterAssignment.Keys)
            {
                DebugLog.Logar("Cluster " + clusterNum + " = " + mClusterAssignment[clusterNum].Count, false, pIndet: mCurrentGeneration);
            }
            DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: mCurrentGeneration);
            */

            mStopWatch.Reset();
            mStopWatch.Start();
#endif

            if (MAX_PARALLEL_THREADS > 1)
            {
                Parallel.For(0, mClusters.Count, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, (i, loopState) =>
                {
                    Assimilate2(i);
                });
            }
            else
            {
                for (int i = 0; i < mClusters.Count; i++)
                {
                    Assimilate2(i);
                }
            }

#if DEBUG
            mStopWatch.Stop();

            DebugLog.Logar("mStopWatch for Assimilate = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }
    }

    private void Assimilate(int pClusterIndex)
    {
        Cluster lvCluster = null;
        List<IIndividual<TrainMovement>> lvIndividualList;
        IIndividual<TrainMovement> lvBestIndividual = null;
        IIndividual<TrainMovement> lvSon = null;
        IIndividual<TrainMovement> lvDaughter = null;
        int lvDistanceSon = Int32.MaxValue;
        int lvDistanceDaughter = Int32.MaxValue;
        int lvMinDistanceSon = Int32.MaxValue;
        int lvMinDistanceDaughter = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;

        if(!mClusterAssignment.ContainsKey(pClusterIndex))
        {
            return;
        }

        if ((pClusterIndex >= 0) && (pClusterIndex < mClusters.Count))
        {
            lvCluster = mClusters[pClusterIndex];
        }

        if (lvCluster != null)
        {
            try
            {
                lvIndividualList = mClusterAssignment[pClusterIndex];

                if (lvIndividualList != null)
                {
                    foreach (IIndividual<TrainMovement> lvIndiv in lvIndividualList)
                    {
                        DoCrossOver(lvIndiv, lvCluster.Center, out lvSon, out lvDaughter);

                        if (mECSAssimilationMode == ECS_ASSIMILATION_MODE.DISTANCE)
                        {
                            if (lvSon != null)
                            {
                                lvDistance = lvIndiv.GetDistanceFrom(lvSon);
                                lvMinDistanceSon = lvCluster.Center.GetDistanceFrom(lvSon);

                                lvDistanceSon = Math.Abs(lvDistance - lvMinDistanceSon);
                            }

                            if (lvDaughter != null)
                            {
                                lvDistance = lvIndiv.GetDistanceFrom(lvDaughter);
                                lvMinDistanceDaughter = lvCluster.Center.GetDistanceFrom(lvDaughter);

                                lvDistanceDaughter = Math.Abs(lvDistance - lvMinDistanceDaughter);
                            }

                            if ((lvMinDistanceDaughter < lvMinDistanceSon) && (lvDaughter != null))
                            {
                                if (lvSon != null)
                                {
                                    lvSon.Clear();
                                }

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                }

                                lvCluster.Center = lvDaughter;

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                                }
                            }
                            else if (lvSon != null)
                            {
                                if (lvDaughter != null)
                                {
                                    lvDaughter.Clear();
                                }

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                }

                                lvCluster.Center = lvSon;

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                                }
                            }
                        }
                        else
                        {
                            if ((lvSon != null) && (lvDaughter != null))
                            {
                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                }

                                if (lvSon.Fitness < lvDaughter.Fitness)
                                {
                                    lvCluster.Center = lvSon;
                                    lvDaughter.Clear();
                                }
                                else
                                {
                                    lvCluster.Center = lvDaughter;
                                    lvSon.Clear();
                                }

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                                }
                            }
                            else if (lvSon != null)
                            {
                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                }

                                lvCluster.Center = lvSon;
                                if (lvDaughter != null)
                                {
                                    lvDaughter.Clear();
                                }

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                                }
                            }
                            else if (lvDaughter != null)
                            {
                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                }

                                lvCluster.Center = lvDaughter;
                                if (lvSon != null)
                                {
                                    lvSon.Clear();
                                }

                                lock(mECSClusterCentersSet)
                                {
                                    mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                                }
                            }
                        }

                        if (lvBestIndividual == null)
                        {
                            lvBestIndividual = lvCluster.Center;
                        }
                        else if (lvCluster.Center != null)
                        {
                            if (lvCluster.Center.Fitness < lvBestIndividual.Fitness)
                            {
                                lvBestIndividual = lvCluster.Center;
                            }
                        }
                    }

                    lock(mClustersCenter)
                    {
                        mClustersCenter.Add(lvBestIndividual);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
            }
        }
    }

    private void Assimilate2(int pClusterIndex)
    {
        Cluster lvCluster = null;
        List<IIndividual<TrainMovement>> lvIndividualList;
        IIndividual<TrainMovement> lvIndividual = null;
        IIndividual<TrainMovement> lvSon = null;
        IIndividual<TrainMovement> lvDaughter = null;
        int lvDistanceSon = Int32.MaxValue;
        int lvDistanceDaughter = Int32.MaxValue;
        int lvMinDistanceSon = Int32.MaxValue;
        int lvMinDistanceDaughter = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;
        int lvAssimilateIndex;

        if (!mClusterAssignment.ContainsKey(pClusterIndex))
        {
            return;
        }

        if ((pClusterIndex >= 0) && (pClusterIndex < mClusters.Count))
        {
            lvCluster = mClusters[pClusterIndex];
        }

        if (lvCluster != null)
        {
            try
            {
                lvIndividualList = mClusterAssignment[pClusterIndex];

                if ((lvIndividualList != null) && (lvIndividualList.Count > 0))
                {
                    lvAssimilateIndex = mRandom.Next(lvIndividualList.Count);
                    lvIndividual = lvIndividualList[lvAssimilateIndex];

                    DoCrossOver(lvIndividual, lvCluster.Center, out lvSon, out lvDaughter);

                    if (mECSAssimilationMode == ECS_ASSIMILATION_MODE.DISTANCE)
                    {
                        if (lvSon != null)
                        {
                            lvDistance = lvIndividual.GetDistanceFrom(lvSon);
                            lvMinDistanceSon = lvCluster.Center.GetDistanceFrom(lvSon);

                            lvDistanceSon = Math.Abs(lvDistance - lvMinDistanceSon);
                        }

                        if (lvDaughter != null)
                        {
                            lvDistance = lvIndividual.GetDistanceFrom(lvDaughter);
                            lvMinDistanceDaughter = lvCluster.Center.GetDistanceFrom(lvDaughter);

                            lvDistanceDaughter = Math.Abs(lvDistance - lvMinDistanceDaughter);
                        }

                        if ((lvMinDistanceDaughter < lvMinDistanceSon) && (lvDaughter != null))
                        {
                            if (lvSon != null)
                            {
                                lvSon.Clear();
                            }

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            }

                            lvCluster.Center = lvDaughter;

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                            }
                        }
                        else if (lvSon != null)
                        {
                            if (lvDaughter != null)
                            {
                                lvDaughter.Clear();
                            }

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            }

                            lvCluster.Center = lvSon;

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                            }
                        }
                    }
                    else
                    {
                        if ((lvSon != null) && (lvDaughter != null))
                        {
                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            }

                            if (lvSon.Fitness < lvDaughter.Fitness)
                            {
                                lvCluster.Center = lvSon;
                                lvDaughter.Clear();
                            }
                            else
                            {
                                lvCluster.Center = lvDaughter;
                                lvSon.Clear();
                            }

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                            }
                        }
                        else if (lvSon != null)
                        {
                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            }

                            lvCluster.Center = lvSon;
                            if (lvDaughter != null)
                            {
                                lvDaughter.Clear();
                            }

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                            }
                        }
                        else if (lvDaughter != null)
                        {
                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            }

                            lvCluster.Center = lvDaughter;
                            if (lvSon != null)
                            {
                                lvSon.Clear();
                            }

                            lock (mECSClusterCentersSet)
                            {
                                mECSClusterCentersSet.Add(lvCluster.Center.GetUniqueId());
                            }
                        }
                    }

                    lock (mClustersCenter)
                    {
                        mClustersCenter.Add(lvCluster.Center);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
            }
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private IIndividual<TrainMovement> UpdateCluster(IIndividual<TrainMovement> pIndividual, out bool pReject)
    {
        IIndividual<TrainMovement> lvRes = null;
        Cluster lvCloserCluster = null;
        Cluster lvCluster = null;
        int lvMinDistance = Int32.MaxValue;
        int lvMinDistanceSon = Int32.MaxValue;
        int lvMinDistanceDaughter = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;
        IIndividual<TrainMovement> lvSon = null;
        IIndividual<TrainMovement> lvDaughter = null;
        int lvDistanceSon = Int32.MaxValue;
        int lvDistanceDaughter = Int32.MaxValue;

        pReject = false;

        if(pIndividual == null)
        {
            return lvRes;
        }

        try
        {
            if (mECSClusters == mClusters.Count)
            {
                for (int i = 0; i < mECSClusters; i++)
                {
                    lvCluster = mClusters[i];

                    lvDistance = lvCluster.Center.GetDistanceFrom(pIndividual);

                    if (lvDistance < lvMinDistance)
                    {
                        lvMinDistance = lvDistance;
                        lvCloserCluster = lvCluster;
                    }
                }

                if (mECSJoinCloserCluster)
                {
                    lvCloserCluster.AddElement();
                    DoCrossOver(pIndividual, lvCloserCluster.Center, out lvSon, out lvDaughter);
                }
                else if (lvMinDistance <= lvCloserCluster.Radius)
                {
                    lvCloserCluster.AddElement();
                    DoCrossOver(pIndividual, lvCloserCluster.Center, out lvSon, out lvDaughter);
                }

                if (mECSAssimilationMode == ECS_ASSIMILATION_MODE.DISTANCE)
                {
                    if (lvSon != null)
                    {
                        lvDistance = pIndividual.GetDistanceFrom(lvSon);
                        lvMinDistanceSon = lvCloserCluster.Center.GetDistanceFrom(lvSon);

                        lvDistanceSon = Math.Abs(lvDistance - lvMinDistanceSon);
                    }

                    if (lvDaughter != null)
                    {
                        lvDistance = pIndividual.GetDistanceFrom(lvDaughter);
                        lvMinDistanceDaughter = lvCloserCluster.Center.GetDistanceFrom(lvDaughter);

                        lvDistanceDaughter = Math.Abs(lvDistance - lvMinDistanceDaughter);
                    }

                    if ((lvMinDistanceDaughter < lvMinDistanceSon) && (lvDaughter != null))
                    {
                        if (lvSon != null)
                        {
                            lvSon.Clear();
                        }

                        lvRes = lvDaughter;
                        lock (mECSClusterCentersSet)
                        {
                            mECSClusterCentersSet.Remove(lvCloserCluster.Center.GetUniqueId());
                            lvCloserCluster.Center = lvRes;
                            mECSClusterCentersSet.Add(lvRes.GetUniqueId());
                        }
                    }
                    else if (lvSon != null)
                    {
                        if (lvDaughter != null)
                        {
                            lvDaughter.Clear();
                        }

                        lvRes = lvSon;
                        lock (mECSClusterCentersSet)
                        {
                            mECSClusterCentersSet.Remove(lvCloserCluster.Center.GetUniqueId());
                            lvCloserCluster.Center = lvRes;
                            mECSClusterCentersSet.Add(lvRes.GetUniqueId());
                        }
                    }
                }
                else
                {
                    if((lvSon != null) && (lvDaughter != null))
                    {
                        if(lvSon.Fitness < lvDaughter.Fitness)
                        {
                            lvRes = lvSon;
                            lvDaughter.Clear();
                        }
                        else
                        {
                            lvRes = lvDaughter;
                            lvSon.Clear();
                        }

                        lock (mECSClusterCentersSet)
                        {
                            mECSClusterCentersSet.Remove(lvCloserCluster.Center.GetUniqueId());
                            lvCloserCluster.Center = lvRes;
                            mECSClusterCentersSet.Add(lvRes.GetUniqueId());
                        }
                    }
                    else if(lvSon != null)
                    {
                        lvRes = lvSon;
                        lvDaughter.Clear();

                        lock (mECSClusterCentersSet)
                        {
                            mECSClusterCentersSet.Remove(lvCloserCluster.Center.GetUniqueId());
                            lvCloserCluster.Center = lvRes;
                            mECSClusterCentersSet.Add(lvRes.GetUniqueId());
                        }
                    }
                    else if(lvDaughter != null)
                    {
                        lvRes = lvDaughter;
                        lvSon.Clear();

                        lock (mECSClusterCentersSet)
                        {
                            mECSClusterCentersSet.Remove(lvCloserCluster.Center.GetUniqueId());
                            lvCloserCluster.Center = lvRes;
                            mECSClusterCentersSet.Add(lvRes.GetUniqueId());
                        }
                    }
                }
            }
            else
            {
                lock(mClusters)
                {
                    for (int i = 0; i < mClusters.Count; i++)
                    {
                        lvCluster = mClusters[i];

                        lvDistance = pIndividual.GetDistanceFrom(lvCluster.Center);

                        if (lvDistance < lvMinDistance)
                        {
                            lvMinDistance = lvDistance;
                            lvCloserCluster = lvCluster;
                        }
                    }

                    lvCluster = new Cluster(mECSFadeFactor);
                    lvCluster.Center = pIndividual;
                    lvCluster.Radius = (int)(lvMinDistance / 2);

                    lock (mECSClusterCentersSet)
                    {
                        mECSClusterCentersSet.Add(pIndividual.GetUniqueId());
                    }
                    mClusters.Add(lvCluster);
                    lvCluster = null;
                }
            }

            lvSon = null;
            lvDaughter = null;
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    private void UpdateClusterRadius(Cluster pRefCluster)
    {
        Cluster lvCluster;
        int lvMinDistance = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;

        for (int ind = 0; ind < mClusters.Count; ind++)
        {
            lvCluster = mClusters[ind];
            if (pRefCluster.Center.GetUniqueId() != lvCluster.Center.GetUniqueId())
            {
                if (lvCluster.hasMinRadius())
                {
                    lvDistance = pRefCluster.Center.GetDistanceFrom(lvCluster.Center);
                }
                else
                {
                    lvDistance = (lvCluster.Radius * 2);
                }

                if ((lvDistance < lvMinDistance) && (lvDistance > 0))
                {
                    lvMinDistance = lvDistance;
                }
            }
        }

        pRefCluster.Radius = lvMinDistance / 2;
    }

    private void CalculateRadius()
    {
        Cluster lvRefCluster = null;

        if (MAX_PARALLEL_THREADS > 1)
        {
            Parallel.For(0, mClusters.Count, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, (i, loopState) =>
            {
                Cluster lvThreadCluster = mClusters[i];
                UpdateClusterRadius(lvThreadCluster);
            });
        }
        else
        {
            for (int i = 0; i < mClusters.Count; i++)
            {
                lvRefCluster = mClusters[i];
                UpdateClusterRadius(lvRefCluster);
            }
        }
    }

    public void Clear()
    {
        if(mIndividuals != null)
        { 
            foreach(IIndividual<TrainMovement> lvIndividual in mIndividuals)
            {
                if(lvIndividual != null)
                {
                    lvIndividual.Clear();
                }
            }

            mIndividuals.Clear();
            mIndividuals = null;
        }

        if (mPriority != null)
        {
            mPriority.Clear();
        }

        if (mPlanList != null)
        {
            mPlanList.Clear();
            mPlanList = null;
        }

        if (mTrainList != null)
        {
            mTrainList.Clear();
            mTrainList = null;
        }
    }

    private Gene LoadTrainSequence(Gene pGene)
    {
        Gene lvRes = null;
        string lvStrKey;
        Gene[] lvGenes = null;
        Gene lvGene = null;
        int lvSeq = -1;
        int lvStartStopLocationValue;
        int lvCurrentStopLocationPos;
        int lvIndex;

        lvStrKey = pGene.TrainName.Substring(0, 1) + "_" + pGene.Direction;
        if(!StopLocation.TrainSequence.ContainsKey(lvStrKey))
        {
            lvStrKey = pGene.TrainName + "_" + pGene.Direction;
            if (!StopLocation.TrainSequence.ContainsKey(lvStrKey))
            {
                lvStrKey = "";
            }
        }

        if (lvStrKey.Length > 0)
        {
            if(mTrainSequence == null)
            {
                mTrainSequence = new Dictionary<long, Gene[]>();
            }

            if(pGene.StopLocation != null)
            {
                lvCurrentStopLocationPos = pGene.StopLocation.Location;
            }
            else
            {
                lvCurrentStopLocationPos = pGene.Coordinate;
            }

            lvGenes = new Gene[StopLocation.TrainSequence[lvStrKey].Count];

            foreach (string[] lvElements in StopLocation.TrainSequence[lvStrKey])
            {
                try
                {
                    if(lvElements.Length >= 4)
                    {
                        lvStartStopLocationValue = Convert.ToInt32(lvElements[2]);

                        if (((pGene.Direction > 0) && (lvCurrentStopLocationPos <= lvStartStopLocationValue)) || ((pGene.Direction < 0) && (lvCurrentStopLocationPos >= lvStartStopLocationValue)))
                        {
                            lvGene = pGene.Clone();

                            if (lvElements[0].Trim().Length > 0)
                            {
                                lvGene.DepartureTime = DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + lvElements[0], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                if (lvGene.DepartureTime < DateTime.Now)
                                {
                                    lvGene.DepartureTime = DateTime.ParseExact(DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy") + " " + lvElements[0], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                }
                            }

                            lvGene.OptimumTime = DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + lvElements[1], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            if(lvGene.OptimumTime < DateTime.Now)
                            {
                                lvGene.OptimumTime = DateTime.ParseExact(DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy") + " " + lvElements[1], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            }

                            lvGene.StartStopLocation = StopLocation.GetCurrentStopSegment(lvStartStopLocationValue, 1, out lvIndex);
                            lvGene.StopLocation = lvGene.StartStopLocation;
                            lvGene.SegmentInstance = lvGene.StopLocation.GetSegment(lvGene.Direction, lvGene.Track);
                            if (lvGene.Direction > 0)
                            {
                                lvGene.Start = lvGene.StartStopLocation.End_coordinate;
                            }
                            else if (lvGene.Direction < 0)
                            {
                                lvGene.Start = lvGene.StartStopLocation.Start_coordinate;
                            }
                            lvGene.Coordinate = lvGene.Start;

                            if (lvElements[3].Trim().Length > 0)
                            {
                                lvGene.EndStopLocation = StopLocation.GetCurrentStopSegment(Convert.ToInt32(lvElements[3]), 1, out lvIndex);
                            }

                            if (lvGene.Direction > 0)
                            {
                                lvGene.End = lvGene.EndStopLocation.Start_coordinate;
                            }
                            else if (lvGene.Direction < 0)
                            {
                                lvGene.End = lvGene.EndStopLocation.End_coordinate;
                            }

                            lvGene.Sequence = (short)++lvSeq;

                            lvGenes[lvSeq] = lvGene;

                            if(lvRes == null)
                            {
                                lvRes = lvGene;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
                }
            }

            mTrainSequence.Add(pGene.TrainId, lvGenes);
        }

        return lvRes;
    }

    /* This Method can fail for stop location capacity higher than 2 or overtaking. Thus, leading to wrong assumption */
    private void FixInitialTracks()
    {
        IDictionary<Int64, TrainMovement> lvMovDic = null;
        IDictionary<int, ISet<TrainMovement>> lvStopLocationDic = null;
        TrainMovement lvTrainMovement = null;
        List<StopLocation> lvStopLocations = null;
        short[] lvStopLocationAvailability = null;
        Gene lvGene = null;
        Segment lvLeftSwitch = null;
        Segment lvRightSwitch = null;
        DateTime lvLimitTime;
        StopLocation lvEndStopLocation = null;
        DataTable lvDataPlans = null;
        string lvStrTrainName = "";
        int lvLocation;
        string lvStrUD;
        int lvIndex = 0;

        if (mStartDelayed > 0.0)
        {
            lvLimitTime = mDateRef.AddHours(mStartDelayed);

            try
            {
                lvDataPlans = PlanDataAccess.GetCurrentPlansPoints(mDateRef, mFinalDate.AddDays(1)).Tables[0];

                lvMovDic = new Dictionary<Int64, TrainMovement>();
                lvStopLocationDic = new Dictionary<int, ISet<TrainMovement>>();

#if DEBUG
                DebugLog.Logar("\n ------ FixInitialTracks - Lista de Trems iniciais -------- ", false, pIndet: mCurrentGeneration);
#endif

                foreach (TrainMovement lvTrainMov in mTrainList)
                {
                    lvMovDic.Add(lvTrainMov.Last.TrainId, lvTrainMov);

#if DEBUG
                    DebugLog.Logar(lvTrainMov.ToString(), false, pIndet: mCurrentGeneration);
#endif
                }

#if DEBUG
                DebugLog.Logar(" ------------------------------------------------------------------ ", false, pIndet: mCurrentGeneration);
#endif

                mTrainList = new List<TrainMovement>();

                /* Verifica quem é primeiro em cada stop location */
                foreach (DataRow row in lvDataPlans.Rows)
                {
                    lvStrTrainName = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());

                    if (mTrainAllowed.Contains(lvStrTrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                    {
                        lvGene = new Gene();

                        lvGene.TrainName = lvStrTrainName;
                        try
                        {
                            lvGene.TrainId = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);

                            continue;
                        }

                        lvGene.Time = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                        lvLocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
                        lvStrUD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                        lvGene.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                        lvGene.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
                        lvGene.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["coordinate"]));
                        lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["destino"]));
                        lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                        if (lvGene.Time > lvGene.DepartureTime)
                        {
                            lvGene.DepartureTime = lvGene.Time;
                        }

                        lvGene.SegmentInstance = Segment.GetSegmentAt(lvGene.Coordinate, lvGene.Track);
                        if (lvGene.SegmentInstance == null)
                        {
                            lvGene.SegmentInstance = Segment.GetSegmentAt(lvLocation, lvStrUD);
                        }

                        if (lvGene.SegmentInstance == null)
                        {
                            continue;
                        }
                        else if ((lvGene.SegmentInstance.OwnerStopLocation == null) && lvGene.SegmentInstance.Track != 1)
                        {
                            /* Para entrar na linha tronco deveria fazer por Stop Location, caso a stop location para ele não tenha sido definida o trem deve ser ignorado */
                            continue;
                        }

                        //lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                        lvGene.StopLocation = lvGene.SegmentInstance.OwnerStopLocation;

                        lvGene.StartStopLocation = lvGene.StopLocation;

                        lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);
                        if (lvEndStopLocation == null)
                        {
                            lvEndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
                        }
                        lvGene.EndStopLocation = lvEndStopLocation;

                        if ((lvMovDic.ContainsKey(lvGene.TrainId)) && (lvGene.Time >= lvLimitTime))
                        {
#if DEBUG
                            DebugLog.Logar(lvGene.ToString(), false, pIndet: mCurrentGeneration);
#endif
                            lvTrainMovement = lvMovDic[lvGene.TrainId];

                            lvTrainMovement.Last.Time = lvGene.Time;
                            lvTrainMovement.Last.Coordinate = lvGene.Coordinate;
                            lvTrainMovement.Last.Track = lvGene.Track;
                            lvTrainMovement.Last.SegmentInstance = lvGene.SegmentInstance;
                            lvTrainMovement.Last.StopLocation = lvGene.StopLocation;

                            if (lvGene.StopLocation != null)
                            {
                                if (!lvStopLocationDic.ContainsKey(lvGene.StopLocation.Location))
                                {
                                    lvStopLocationDic.Add(lvGene.StopLocation.Location, new HashSet<TrainMovement>());
                                }
                                lvStopLocationDic[lvGene.StopLocation.Location].Add(lvTrainMovement);
                            }

                            mTrainList.Add(lvTrainMovement);
                            lvMovDic.Remove(lvGene.TrainId);
                        }
                    }
                }

                /* It is Settled a no conflit track to each TrainMovement */
                foreach(TrainMovement lvTrainMov in mTrainList)
                {
                    if(lvTrainMov.Last.StopLocation != null)
                    {
                        lvLeftSwitch = lvTrainMov.Last.StopLocation.GetNextSwitchSegment(-1);
                        lvRightSwitch = lvTrainMov.Last.StopLocation.GetNextSwitchSegment(1);

                        lvStopLocations = StopLocation.GetStopLocationsBetweenSwitches(lvLeftSwitch, lvRightSwitch);

                        /* Direction on track or 0 to available and (track * -1) otherwise */
                        lvStopLocationAvailability = new short[lvTrainMov.Last.StopLocation.Capacity];
                        foreach (StopLocation lvStopLoc in lvStopLocations)
                        {
                            if (lvStopLocationDic.ContainsKey(lvStopLoc.Location))
                            {
                                foreach (TrainMovement lvTrainMove in lvStopLocationDic[lvStopLoc.Location])
                                {
                                    if (lvTrainMove.Last.Track <= lvStopLoc.Capacity)
                                    {
                                        if ((lvStopLocationAvailability[lvTrainMove.Last.Track - 1] == 0) || (lvStopLocationAvailability[lvTrainMove.Last.Track - 1] == lvTrainMove.Last.Direction))
                                        {
                                            lvStopLocationAvailability[lvTrainMove.Last.Track - 1] = lvTrainMove.Last.Direction;
                                        }
                                        else
                                        {
                                            for(int i = 0; i < lvStopLocationAvailability.Length; i++)
                                            {
                                                if ((lvStopLocationAvailability[i] == 0) || (lvStopLocationAvailability[i] == lvTrainMove.Last.Direction))
                                                {
                                                    lvStopLocationAvailability[i] = lvTrainMove.Last.Direction;
                                                    lvTrainMove.Last.Track = (short)(i + 1);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                             }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
            }
        }
    }

    private void FixInitialPositions()
    {
        IIndividual<TrainMovement> lvIndividual = null;
        IDictionary<int, List<Gene>> lvDicTrainPref = null;
        IDictionary<int, ISet<Int64>> lvDicTrainPrefSet = null;
        IDictionary<Int64, TrainMovement> lvMovDic = null;
        List<Gene> lvTrainPriorityList = null;
        TrainMovement lvTrainMovRes = null;
        TrainMovement lvTrainMovement = null;
        Gene lvGene = null;
        DateTime lvLimitTime;
        DateTime lvForcedDepTime = DateTime.MinValue;
        StopLocation lvNextStopLocation = null;
        StopLocation lvNextStopLocationPriority = null;
        DataTable lvDataPlans = null;
        string lvStrTrainName = "";
        int lvLocation;
        string lvStrUD;
        bool lvWasInserted = false;
        int lvCount = 0;
        bool lvVerifyPriorityList = false;
        bool lvCanMoveAll = false;

        if (mStartDelayed > 0.0)
        {
            lvLimitTime = mDateRef.AddHours(mStartDelayed);

            lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
            lvDicTrainPref = new Dictionary<int, List<Gene>>();
            lvDicTrainPrefSet = new Dictionary<int, ISet<Int64>>();

            foreach (StopLocation lvStopLocation in StopLocation.GetList())
            {
                if(!lvDicTrainPref.ContainsKey(lvStopLocation.Location))
                {
                    lvDicTrainPref.Add(lvStopLocation.Location, new List<Gene>());
                    lvDicTrainPrefSet.Add(lvStopLocation.Location, new HashSet<Int64>());
                }
            }

            try
            {
                lvDataPlans = PlanDataAccess.GetCurrentPlansPoints(mDateRef, lvLimitTime).Tables[0];

                lvMovDic = new Dictionary<Int64, TrainMovement>();

#if DEBUG
                    DebugLog.Logar("\n ------ FixInitialPositions - Lista de Trems iniciais -------- ", false, pIndet: mCurrentGeneration);
#endif

                foreach (TrainMovement lvTrainMov in mTrainList)
                {
                    if(!lvMovDic.ContainsKey(lvTrainMov.Last.TrainId))
                    {
                        if ((lvTrainMov.Last.Time <= lvLimitTime) || (lvTrainMov.Last.DepartureTime <= lvLimitTime))
                        {
                            lvMovDic.Add(lvTrainMov.Last.TrainId, lvTrainMov);
                        }

#if DEBUG
                        DebugLog.Logar(lvTrainMov.ToString(), false, pIndet: mCurrentGeneration);
#endif
                    }
                }

#if DEBUG
                DebugLog.Logar(" ------------------------------------------------------------------ ", false, pIndet: mCurrentGeneration);
#endif

                /* Verifica quem é primeiro em cada stop location */
                foreach (DataRow row in lvDataPlans.Rows)
                {
                    lvStrTrainName = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());

                    if (mTrainAllowed.Contains(lvStrTrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                    {
                        lvGene = new Gene();

                        lvGene.TrainName = lvStrTrainName;
                        try
                        {
                            lvGene.TrainId = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);

                            continue;
                        }

                        lvGene.Time = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                        lvLocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
                        lvStrUD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                        lvGene.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                        lvGene.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
                        lvGene.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["coordinate"]));
                        lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["destino"]));

                        lvGene.SegmentInstance = Segment.GetSegmentAt(lvGene.Coordinate, lvGene.Track);
                        if (lvGene.SegmentInstance == null)
                        {
                            lvGene.SegmentInstance = Segment.GetSegmentAt(lvLocation, lvStrUD);
                        }

                        if (lvGene.SegmentInstance == null)
                        {
                            continue;
                        }
                        else if ((lvGene.SegmentInstance.OwnerStopLocation == null) && lvGene.SegmentInstance.Track != 1)
                        {
                            /* Para entrar na linha tronco deveria fazer por Stop Location, caso a stop location para ele não tenha sido definida o trem deve ser ignorado */
                            continue;
                        }

                        //lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                        lvGene.StopLocation = lvGene.SegmentInstance.OwnerStopLocation;

                        if (lvGene.StopLocation != null)
                        {
                            if(lvDicTrainPref.ContainsKey(lvGene.StopLocation.Location))
                            {
                                if(lvMovDic.ContainsKey(lvGene.TrainId) && (lvGene.StopLocation != null) && (lvMovDic[lvGene.TrainId].Last.StopLocation != null) && (lvGene.StopLocation.Location != lvMovDic[lvGene.TrainId].Last.StopLocation.Location))
                                {
                                    lvTrainPriorityList = lvDicTrainPref[lvGene.StopLocation.Location];

                                    lvWasInserted = false;
                                    for (int i = 0; i < lvTrainPriorityList.Count; i++)
                                    {
                                        if ((lvGene.Time < lvTrainPriorityList[i].Time))
                                        {
                                            lvTrainPriorityList.Insert(i, lvGene);
                                            lvDicTrainPrefSet[lvGene.StopLocation.Location].Add(lvGene.TrainId);
                                            lvWasInserted = true;
                                            break;
                                        }
                                    }

                                    if (!lvWasInserted)
                                    {
                                        lvTrainPriorityList.Add(lvGene);
                                        lvDicTrainPrefSet[lvGene.StopLocation.Location].Add(lvGene.TrainId);
                                    }
                                }
                            }
                        }
                    }
                }

#if DEBUG
                DebugLog.Logar("\n ------ FixInitialPositions - Lista de Prioridades em cada stop location -------- ", false, pIndet: mCurrentGeneration);

                foreach (StopLocation lvStopLocation in StopLocation.GetList())
                {
                    lvTrainPriorityList = lvDicTrainPref[lvStopLocation.Location];

                    DebugLog.Logar("\nStop Location: " + lvStopLocation.Location, false, pIndet: mCurrentGeneration);

                    foreach (Gene lvGen in lvTrainPriorityList)
                    {
                        DebugLog.Logar(lvGen.ToString(), false, pIndet: mCurrentGeneration);
                    }
                }

                DebugLog.Logar(" ---------------------------------------------------------------------------------- ", false, pIndet: mCurrentGeneration);
#endif

                lvCanMoveAll = true;
                while (lvMovDic.Count > 0)
                {
#if DEBUG
                    if (!lvCanMoveAll)
                    {
                        DebugLog.Logar("\n -----> Movimento bloqueado apos rodada completa <------- ", false, pIndet: mCurrentGeneration);

                        foreach (TrainMovement lvTrainMov in lvMovDic.Values)
                        {
                            DebugLog.Logar(lvTrainMov.ToString(), false, pIndet: mCurrentGeneration);
                        }
                        DebugLog.Logar(" -------------------------------------------------------- \n", false, pIndet: mCurrentGeneration);
                    }

                    lvCanMoveAll = false;
#endif

                    foreach (TrainMovement lvTrainMov in lvMovDic.Values)
                    {
                        if (lvTrainMov.Last.StopLocation == null)
                        {
                            lvNextStopLocation = StopLocation.GetNextStopSegment(lvTrainMov.Last.Coordinate, lvTrainMov.Last.Direction);
                        }
                        else
                        {
                            lvNextStopLocation = lvTrainMov.Last.StopLocation.GetNextStopSegment(lvTrainMov.Last.Direction);
                        }

                        if (lvTrainMov.Last.StopLocation != null)
                        {
                            lvTrainPriorityList = lvDicTrainPref[lvTrainMov.Last.StopLocation.Location];

                            for (int i = 0; i < lvTrainPriorityList.Count; i++)
                            {
                                if (lvTrainMov.Last.TrainId == lvTrainPriorityList[i].TrainId)
                                {
                                    lvForcedDepTime = lvTrainPriorityList[i].Time;
                                    break;
                                }
                            }
                        }

                        lvTrainMovRes = null;
                        lvVerifyPriorityList = false;
                        lvTrainPriorityList = lvDicTrainPref[lvNextStopLocation.Location];
                        if (lvTrainPriorityList.Count > 0)
                        {
                            if ((lvTrainMov.Last.TrainId == lvTrainPriorityList[0].TrainId))
                            {
                                lvVerifyPriorityList = true;
                                lvTrainMovRes = (TrainMovement)lvIndividual.MoveTrain(lvTrainMov, DateTime.MaxValue, pForcedDepTime: lvForcedDepTime);
                            }
                            else if(!lvDicTrainPrefSet[lvNextStopLocation.Location].Contains(lvTrainMov.Last.TrainId))
                            {
                                lvTrainMovRes = (TrainMovement)lvIndividual.MoveTrain(lvTrainMov, DateTime.MaxValue, pForcedDepTime: lvForcedDepTime);
                            }
                            else
                            {
                                if(lvMovDic.ContainsKey(lvTrainPriorityList[0].TrainId))
                                {
                                    lvTrainMovement = lvMovDic[lvTrainPriorityList[0].TrainId];

                                    if (lvTrainMovement.Last.StopLocation == null)
                                    {
                                        lvNextStopLocationPriority = StopLocation.GetNextStopSegment(lvTrainMovement.Last.Coordinate, lvTrainMovement.Last.Direction);
                                    }
                                    else
                                    {
                                        lvNextStopLocationPriority = lvTrainMovement.Last.StopLocation.GetNextStopSegment(lvTrainMovement.Last.Direction);
                                    }

                                    if (lvNextStopLocationPriority.Location == lvNextStopLocation.Location)
                                    {
                                        lvTrainPriorityList = lvDicTrainPref[lvTrainMov.Last.StopLocation.Location];

                                        for (int i = 0; i < lvTrainPriorityList.Count; i++)
                                        {
                                            if (lvTrainMov.Last.TrainId == lvTrainPriorityList[i].TrainId)
                                            {
                                                lvForcedDepTime = lvTrainPriorityList[i].Time;
                                                break;
                                            }
                                        }

                                        lvVerifyPriorityList = true;
                                        lvTrainMovRes = (TrainMovement)lvIndividual.MoveTrain(lvTrainMov, DateTime.MaxValue, pForcedDepTime: lvForcedDepTime);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lvTrainMovRes = (TrainMovement)lvIndividual.MoveTrain(lvTrainMov, DateTime.MaxValue, pForcedDepTime: lvForcedDepTime);
                        }

                        lvCount = lvTrainPriorityList.Count;

                        if (lvVerifyPriorityList)
                        {
#if DEBUG
                            DebugLog.Logar("\n ------ lvTrainMov = " + lvTrainMov, false, pIndet: mCurrentGeneration);
                            DebugLog.Logar("llvTrainPriorityList[" + lvNextStopLocation.Location + "].Count = " + lvTrainPriorityList.Count, false, pIndet: mCurrentGeneration);
#endif

                            for (int i = 0; i < lvCount; i++)
                            {
                                if (lvTrainPriorityList[i].TrainId == lvTrainMov.Last.TrainId)
                                {
                                    if (lvTrainPriorityList[lvTrainPriorityList.Count - 1].TrainId == lvTrainMov.Last.TrainId)
                                    {
                                        if (i >= lvTrainPriorityList.Count - 1) break;

#if DEBUG
                                        DebugLog.Logar("lvTrainPriorityList.RemoveAtLast(" + (lvTrainPriorityList.Count - 1) + ") = " + lvTrainPriorityList[lvTrainPriorityList.Count - 1] + ", Count: " + lvTrainPriorityList.Count, false, pIndet: mCurrentGeneration);
#endif
                                        lvTrainPriorityList.RemoveAt(lvTrainPriorityList.Count - 1);
                                        //lvCount--;
                                    }

#if DEBUG
                                    DebugLog.Logar("lvTrainPriorityList.Add(lvTrainPriorityList[" + i + "]) = " + lvTrainPriorityList[i] + ", Count: " + lvTrainPriorityList.Count, false, pIndet: mCurrentGeneration);
#endif
                                    lvTrainPriorityList.Add(lvTrainPriorityList[i]);

#if DEBUG
                                    DebugLog.Logar("lvTrainPriorityList.RemoveAt(" + i + ") = " + lvTrainPriorityList[i] + ", Count: " + lvTrainPriorityList.Count, false, pIndet: mCurrentGeneration);
#endif

                                    lvTrainPriorityList.RemoveAt(i);

                                    i--;
                                    lvCount--;

#if DEBUG
                                    DebugLog.Logar("i = " + i + "; lvTrainPriorityList.Count = " + lvTrainPriorityList.Count, false, pIndet: mCurrentGeneration);
#endif
                                }

                                if ((lvTrainPriorityList.Count == 0) || (i >= lvTrainPriorityList.Count - 1)) break;
                            }
                        }

                        if (lvTrainMovRes != null)
                        {
                            if (lvMovDic.ContainsKey(lvTrainMovRes.Last.TrainId))
                            {
                                lvMovDic[lvTrainMovRes.Last.TrainId] = lvTrainMovRes;
                            }
                            else if(lvTrainMovRes.Last.Time < lvLimitTime)
                            {
                                lvMovDic.Add(lvTrainMovRes.Last.TrainId, lvTrainMovRes);
                            }

                            if (lvTrainMov.Last.StopLocation != null)
                            {
                                lvTrainPriorityList = lvDicTrainPref[lvTrainMov.Last.StopLocation.Location];

                                for (int i = lvTrainPriorityList.Count - 1; i >= 0; i--)
                                {
                                    if (lvTrainPriorityList[i].TrainId == lvTrainMovRes.Last.TrainId)
                                    {
                                        lvTrainPriorityList.RemoveAt(i);
                                        lvDicTrainPrefSet[lvTrainMov.Last.StopLocation.Location].Remove(lvTrainMovRes.Last.TrainId);
                                        break;
                                    }
                                }
                            }

                            if ((lvTrainMovRes.Last.Time >= lvLimitTime) || (lvTrainMovRes.Last.StopLocation.Location == lvTrainMovRes.Last.EndStopLocation.Location))
                            {
                                lvMovDic.Remove(lvTrainMov.Last.TrainId);
                            }

#if DEBUG
                            DebugLog.Logar("Adicionando: " + lvTrainMovRes, false, pIndet: mCurrentGeneration);
                            lvCanMoveAll = true;
#endif

                            mTrainList.Add(lvTrainMovRes);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
            }

            if(lvIndividual != null)
            {
                ((TrainIndividual)lvIndividual).GenerateFlotFiles(DebugLog.LogPath);
            }
        }
    }

    private void LoadTrainList()
    {
        HashSet<Int64> lvTrainSet = new HashSet<Int64>();
        DataTable lvDataTrains = null;
        DataTable lvDataPlans = null;
        TrainMovement lvTrainMovement = null;
        Gene lvGene = null;
        Gene lvGeneSeq = null;
        Segment lvSegment = null;
        StopLocation lvCurrentStopSegment = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvStartStopLocation = null;
        StopLocation lvEndStopLocation = null;
        double lvMeanSpeed = 0.0;
        int lvIndex;
        string lvStrTrainName = "";
        double lvVMA = TrainIndividual.VMA;

        int lvCoordinate;
        int lvDirection;
        int lvLocation;
        string lvStrUD;
        DateTime lvOcupTime;
        DateTime lvCreationtime;

        mTrainList = new List<TrainMovement>();

#if DEBUG
        DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
        DebugLog.Logar("Listando trens a serem considerados:", false, pIndet: mCurrentGeneration);
#endif

        lvDataTrains = TrainmovsegmentDataAccess.GetCurrentTrainsData(mInitialDate, mFinalDate).Tables[0];

        try
        {
            foreach (DataRow row in lvDataTrains.Rows)
            {
                lvStrTrainName = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());

                if (mTrainAllowed.Contains(lvStrTrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                {
                    lvGene = new Gene();

                    lvGene.TrainName = lvStrTrainName;
                    try
                    {
                        lvGene.TrainId = ((row["train_id"] == DBNull.Value) ? Int64.MinValue : Convert.ToInt64(row["train_id"]));
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);

                        continue;
                    }

                    lvGene.Time = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                    lvLocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
                    lvStrUD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                    lvGene.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                    lvGene.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
                    lvGene.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["coordinate"]));
                    lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["origem"]));
                    lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["destino"]));
                    lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                    if (lvGene.Time > lvGene.DepartureTime)
                    {
                        lvGene.DepartureTime = lvGene.Time;
                    }
                    lvGene.State = Gene.STATE.IN;
                    lvCreationtime = ((row["creation_tm"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["creation_tm"].ToString()));

                    //lvGene.SegmentInstance = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, lvGene.Track, out lvIndex);
                    lvGene.SegmentInstance = Segment.GetSegmentAt(lvGene.Coordinate, lvGene.Track);

                    if(lvGene.SegmentInstance == null)
                    {
                        lvGene.SegmentInstance = Segment.GetSegmentAt(lvLocation, lvStrUD);
                    }

                    if(lvGene.SegmentInstance == null)
                    {
                        continue;
                    }
                    else if((lvGene.SegmentInstance.OwnerStopLocation == null) && lvGene.SegmentInstance.Track != 1)
                    {
                        /* Para entrar na linha tronco deveria fazer por Stop Location, caso a stop location para ele não tenha sido definida o trem deve ser ignorado */
                        continue;
                    }

                    if (lvGene.DepartureTime.AddYears(1) < lvCreationtime)
                    {
                        lvGene.DepartureTime = lvCreationtime;
                    }

                    if (lvGene.DepartureTime == DateTime.MinValue)
                    {
                        lvGene.DepartureTime = ((row["plan_departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["plan_departure_time"].ToString()));
                    }

                    if(lvGene.DepartureTime > lvGene.Time)
                    {
                        continue;
                    }

                    if (lvGene.Start == -99999999)
                    {
                        lvGene.Start = Int32.MinValue;
                    }

                    if (lvGene.End == -99999999)
                    {
                        if (lvGene.Direction > 0)
                        {
                            lvGene.End = Int32.MaxValue;
                        }
                        else
                        {
                            lvGene.End = Int32.MinValue;
                        }
                    }

                    if (!mAllowNoDestinationTrain)
                    {
                        if (lvGene.End == Int32.MinValue)
                        {
                            continue;
                        }
                    }

                    //lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                    lvGene.StopLocation = lvGene.SegmentInstance.OwnerStopLocation;

                    if (lvGene.StopLocation == null)
                    {
                        lvGene.State = Gene.STATE.UNDEF;
                        lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                    }
                    else
                    {
                        lvNextStopLocation = lvGene.StopLocation.GetNextStopSegment(lvGene.Direction);
                    }

                    lvStartStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Start, lvGene.Direction, out lvIndex);
                    if (lvStartStopLocation == null)
                    {
                        lvStartStopLocation = StopLocation.GetNextStopSegment(lvGene.Start, lvGene.Direction);
                    }
                    lvGene.StartStopLocation = lvStartStopLocation;

                    lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);

                    if (lvEndStopLocation == null)
                    {
                        lvEndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
                    }
                    lvGene.EndStopLocation = lvEndStopLocation;

                    if (lvNextStopLocation == null)
                    {
                        continue;
                    }
                    else if ((lvGene.StopLocation == lvEndStopLocation) && (lvGene.StopLocation != null))
                    {
                        continue;
                    }
                    else if (lvEndStopLocation != null)
                    {
                        if (lvGene.Direction > 0)
                        {
                            if (lvGene.Coordinate >= lvEndStopLocation.Start_coordinate)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (lvGene.Coordinate <= lvEndStopLocation.End_coordinate)
                            {
                                continue;
                            }
                        }
                    }

                    lvGene.ValueWeight = GetPriority(lvGene);

                    lvMeanSpeed = TrainmovsegmentDataAccess.GetMeanSpeed(lvGene.TrainId, mFinalDate, lvVMA, out lvCoordinate, out lvDirection, out lvLocation, out lvStrUD, out lvOcupTime);
                    if (lvVMA > lvMeanSpeed)
                    {
                        lvMeanSpeed = lvVMA;
                    }
                    lvGene.Speed = lvMeanSpeed;

                    if (lvGene.StopLocation != null)
                    {
                        if ((lvGene.SegmentInstance.SegmentValue.Equals("CV03B") && (lvGene.Direction == -1)) || (lvGene.SegmentInstance.SegmentValue.Equals("CV03C") && (lvGene.Direction == 1)) || lvGene.SegmentInstance.SegmentValue.StartsWith("SW") || lvGene.SegmentInstance.SegmentValue.Equals("WT") || lvGene.SegmentInstance.IsSwitch)
                        {
                            lvGene.StopLocation = null;
                            lvGene.State = Gene.STATE.UNDEF;

                            if (lvGene.Track != 0)
                            {
                                lvGeneSeq = LoadTrainSequence(lvGene);

                                if (lvGeneSeq != null)
                                {
                                    lvGene.EndStopLocation = lvGeneSeq.EndStopLocation;
                                    lvGene.End = lvGeneSeq.End;
                                    lvGene.OptimumTime = lvGeneSeq.OptimumTime;
                                }

                                if (!lvTrainSet.Contains(lvGene.TrainId))
                                {
                                    lvTrainMovement = new TrainMovement();
                                    lvTrainMovement.Add(lvGene);
                                    lvTrainMovement.LoadId();

                                    mTrainList.Insert(0, lvTrainMovement);
                                    lvTrainSet.Add(lvGene.TrainId);

#if DEBUG
                                    DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ", Coordinate: " + lvGene.Coordinate + ")", false, pIndet: mCurrentGeneration);
#endif
                                }
                            }
                        }
                        else
                        {
                            if (lvGene.Track != 0)
                            {
                                lvGeneSeq = LoadTrainSequence(lvGene);

                                if (lvGeneSeq != null)
                                {
                                    lvGene.EndStopLocation = lvGeneSeq.EndStopLocation;
                                    lvGene.End = lvGeneSeq.End;
                                    lvGene.OptimumTime = lvGeneSeq.OptimumTime;
                                }

                                if (!lvTrainSet.Contains(lvGene.TrainId))
                                {
                                    lvTrainMovement = new TrainMovement();
                                    lvTrainMovement.Add(lvGene);
                                    lvTrainMovement.LoadId();

                                    mTrainList.Add(lvTrainMovement);
                                    lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                                    DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ", Coordinate: " + lvGene.Coordinate + ")", false, pIndet: mCurrentGeneration);
#endif
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lvGene.Track != 0)
                        {
                            lvGeneSeq = LoadTrainSequence(lvGene);

                            if (lvGeneSeq != null)
                            {
                                lvGene.EndStopLocation = lvGeneSeq.EndStopLocation;
                                lvGene.End = lvGeneSeq.End;
                                lvGene.OptimumTime = lvGeneSeq.OptimumTime;
                            }

                            if (!lvTrainSet.Contains(lvGene.TrainId))
                            {
                                lvTrainMovement = new TrainMovement();
                                lvTrainMovement.Add(lvGene);
                                lvTrainMovement.LoadId();

                                mTrainList.Insert(0, lvTrainMovement);
                                lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                                DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ", Coordinate: " + lvGene.Coordinate + ")", false, pIndet: mCurrentGeneration);
#endif
                            }
                        }
                    }

                    LoadPATs(lvGene.TrainId);
                }
            }

#if DEBUG
            DebugLog.Logar("Total = " + mTrainList.Count, false, pIndet: mCurrentGeneration);
#endif
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        try
        {
            mPlanList = new List<TrainMovement>();

#if DEBUG
            DebugLog.Logar(" ------------------------------------------------------------------------------------------------------ ", false, pIndet: mCurrentGeneration);
            DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);

            DebugLog.Logar("Listando Planos a serem considerados:", false, pIndet: mCurrentGeneration);
#endif

            if (DateTime.Now.Date == mInitialDate.Date)
            {
                lvDataPlans = PlanDataAccess.GetCurrentPlans(DateTime.Now, mFinalDate.AddDays(1)).Tables[0];
            }
            else
            {
                lvDataPlans = PlanDataAccess.GetCurrentPlans(mFinalDate, mFinalDate.AddDays(1)).Tables[0];
            }

            foreach (DataRow row in lvDataPlans.Rows)
            {
                lvGene = new Gene();

                try
                {
                    lvGene.TrainId = ((row["train_id"] == DBNull.Value) ? Convert.ToInt64(row["plan_id"]) : Convert.ToInt64(row["train_id"]));
                }
                catch (Exception ex)
                {
                    DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);

                    continue;
                }

                if (!lvTrainSet.Contains(lvGene.TrainId))
                {
                    lvGene.TrainName = ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString());

                    if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                    {
                        if (lvGene.TrainName.Trim().Length == 0) continue;

                        lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["origem"]));
                        lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : Convert.ToInt32(row["destino"]));
                        lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                        lvGene.Track = ((row["origin_track"] == DBNull.Value) ? Convert.ToInt16(1) : Convert.ToInt16(row["origin_track"]));
                        lvGene.Time = DateTime.MinValue;
                        lvGene.Coordinate = lvGene.Start;
                        lvGene.Direction = Int16.Parse(lvGene.TrainName.Substring(1));
                        lvGene.Speed = 0.0;
                        lvGene.State = Gene.STATE.IN;

                        if ((lvGene.Direction % 2) == 0)
                        {
                            lvGene.Direction = -1;
                        }
                        else
                        {
                            lvGene.Direction = 1;
                        }

                        lvStartStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Start, lvGene.Direction, out lvIndex);
                        if (lvStartStopLocation == null)
                        {
                            lvStartStopLocation = StopLocation.GetNextStopSegment(lvGene.Start, lvGene.Direction);
                        }
                        lvGene.StartStopLocation = lvStartStopLocation;

                        lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);
                        if (lvEndStopLocation == null)
                        {
                            lvEndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
                        }
                        lvGene.EndStopLocation = lvEndStopLocation;

                        lvSegment = Segment.GetSegmentAt(lvGene.Coordinate, lvGene.Track);

                        if (lvSegment != null)
                        {
                            lvGene.SegmentInstance = lvSegment;

                            lvCurrentStopSegment = lvSegment.OwnerStopLocation;

                            if (lvCurrentStopSegment == null)
                            {
                                lvCurrentStopSegment = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                            }
                            lvGene.StopLocation = lvCurrentStopSegment;
                        }
                        else
                        {
                            lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);

                            if (lvCurrentStopSegment == null)
                            {
                                lvCurrentStopSegment = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                            }
                            lvGene.StopLocation = lvCurrentStopSegment;

                            if (lvGene.StopLocation != null)
                            {
                                lvGene.SegmentInstance = lvGene.StopLocation.GetSegment(lvGene.Direction, 1);
                            }

                            DebugLog.Logar("Não tem segment !", pIndet: mCurrentGeneration);
                        }

                        lvGene.ValueWeight = GetPriority(lvGene);

                        if (mTrainAllowed.Count == 0)
                        {
                            lvGeneSeq = LoadTrainSequence(lvGene);

                            if (lvGeneSeq != null)
                            {
                                lvGene.EndStopLocation = lvGeneSeq.EndStopLocation;
                                lvGene.End = lvGeneSeq.End;
                                lvGene.OptimumTime = lvGeneSeq.OptimumTime;
                            }

                            if (!lvTrainSet.Contains(lvGene.TrainId))
                            {
                                lvTrainMovement = new TrainMovement();
                                lvTrainMovement.Add(lvGene);
                                mPlanList.Add(lvTrainMovement);
                                lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                                DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Stop Location: " + lvGene.StopLocation.Location + ") - End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ")", false, pIndet: mCurrentGeneration);
#endif
                            }
                        }
                        else if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)))
                        {
                            lvGeneSeq = LoadTrainSequence(lvGene);

                            if (lvGeneSeq != null)
                            {
                                lvGene.EndStopLocation = lvGeneSeq.EndStopLocation;
                                lvGene.End = lvGeneSeq.End;
                                lvGene.OptimumTime = lvGeneSeq.OptimumTime;
                            }

                            if (!lvTrainSet.Contains(lvGene.TrainId))
                            {
                                lvTrainMovement = new TrainMovement();
                                lvTrainMovement.Add(lvGene);
                                mPlanList.Add(lvTrainMovement);
#if DEBUG
                                DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Stop Location: " + lvGene.StopLocation.Location + ") - End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ")", false, pIndet: mCurrentGeneration);
#endif
                            }
                        }

                        LoadPATs(lvGene.TrainId);
                    }
                }
            }

#if DEBUG
            DebugLog.Logar("Total = " + mPlanList.Count, false, pIndet: mCurrentGeneration);

            DebugLog.Logar(" --------------------------------------------------------------------------------------------- ", false, pIndet: mCurrentGeneration);
            DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        if (mDelayedFixedMethod == DELAYED_FIX_METHOD.TRACK)
        {
            FixInitialTracks();
        }
        else if(mDelayedFixedMethod == DELAYED_FIX_METHOD.ALL)
        {
            FixInitialPositions();
        }
    }

    public static double GetPriority(Gene pGene)
    {
        double lvRes = 1.0;
        string lvStrKey;

        if (mPriority.Keys.Count > 0)
        {
            if (mPriority.ContainsKey(pGene.TrainName))
            {
                lvRes = mPriority[pGene.TrainName];
            }
            else
            {
                lvStrKey = pGene.TrainName.Substring(0, 1) + pGene.Direction;
                if (mPriority.ContainsKey(lvStrKey))
                {
                    lvRes = mPriority[lvStrKey];
                }
            }
        }

        return lvRes;
    }

    public static void LoadPriority(string pStrPriority)
    {
        string[] lvVarElement;
        string lvStrTrainType = "";
        int lvDirection = 0;
        double lvValue = 0;
        string lvKey = "";
        string[] lvVarPriority;

        if (pStrPriority.Length > 0)
        {
            lvVarPriority = pStrPriority.Split('|');

            foreach (string lvPriority in lvVarPriority)
            {
                lvVarElement = lvPriority.Split(':');

                if (lvVarElement.Length == 3)
                {
                    lvStrTrainType = lvVarElement[0];
                    lvDirection = Int32.Parse(lvVarElement[1]);
                    lvValue = Convert.ToDouble(lvVarElement[2]);

                    lvKey = lvStrTrainType + lvDirection;
                }
                else if(lvVarElement.Length == 2)
                {
                    lvKey = lvVarElement[0];
                    lvValue = Convert.ToDouble(lvVarElement[1]);
                }

                if (lvKey.Length > 0)
                {
                    if (!mPriority.ContainsKey(lvKey))
                    {
                        mPriority.Add(lvKey, lvValue);
                    }
                }

                lvKey = "";
                lvValue = 0;
            }
        }
    }

    public int Count
    {
        get
        {
            int lvRes = 0;

            if (mIndividuals != null)
            {
                lvRes = mIndividuals.Count;
            }

            return lvRes;
        }
    }

    public IIndividual<TrainMovement> GetBestIndividual()
    {
        IIndividual<TrainMovement> lvRes = null;

        mIndividuals.Sort();

        for (int i = 0; i < mIndividuals.Count; i++)
        {
            lvRes = mIndividuals[i];

            if (lvRes != null)
            {
                if (lvRes.GetUniqueId() != -1)
                {
                    break;
                }
            }
            else
            {
                mIndividuals.RemoveAt(i--);
            }
        }

        if (mClustersCenter != null)
        {
            mClustersCenter.Sort();
            if (mClustersCenter[0].Fitness < lvRes.Fitness)
            {
                lvRes = mClustersCenter[0];
            }
        }

        return lvRes;
    }

    public IIndividual<TrainMovement> GetIndividualAt(int pIndex)
    {
        IIndividual<TrainMovement> lvRes = null;

        if (mIndividuals != null)
        {
            if ((pIndex >= 0) && (pIndex < mIndividuals.Count))
            {
                lvRes = mIndividuals[pIndex];
            }
        }

        return lvRes;
    }

    private void GenerateChildren(IIndividual<TrainMovement> pFather, IIndividual<TrainMovement> pMother)
    {
        int lvRandomValue = mRandom.Next(1, 101);
        IIndividual<TrainMovement> lvSon = null;
        IIndividual<TrainMovement> lvDaughter = null;
        IIndividual<TrainMovement> lvMutated = null;

        DoCrossOver(pFather, pMother, out lvSon, out lvDaughter);

        if (lvSon != null)
        {
            if (lvRandomValue <= mMutationRate)
            {
                lvMutated = Mutate(lvSon, mMinMutationSteps);
            }

            if (lvMutated != null)
            {
                lvSon.Clear();

                /*
                if ((mECSClusters > 0) && mECS)
                {
                    if ((lvMutated.Fitness >= mBestFitness) && !KeepingDiversity(lvMutated))
                    {
                        lvMutated.Clear();
                        lvMutated = null;
                    }
                }
                */

                if (lvMutated != null)
                {
                    lvMutated.GetFitness();

                    lock (mIndividuals)
                    {
                        mIndividuals.Add(lvMutated);
                    }
                }
            }
            else
            {
                /*
                if ((mECSClusters > 0) && mECS)
                {
                    if ((lvSon.Fitness >= mBestFitness) && !KeepingDiversity(lvSon))
                    {
                        lvSon.Clear();
                        lvSon = null;
                    }
                }
                */

                if (lvSon != null)
                {
                    lvSon.GetFitness();

                    lock (mIndividuals)
                    {
                        mIndividuals.Add(lvSon);
                    }
                }
            }

            lvSon = null;
            lvMutated = null;
        }

        if (lvDaughter != null)
        {
            if (lvRandomValue <= mMutationRate)
            {
                lvMutated = Mutate(lvDaughter, mMinMutationSteps);
            }

            if (lvMutated != null)
            {
                lvDaughter.Clear();

                /*
                if ((mECSClusters > 0) && mECS)
                {
                    if ((lvMutated.Fitness >= mBestFitness) && !KeepingDiversity(lvMutated))
                    {
                        lvMutated.Clear();
                        lvMutated = null;
                    }
                }
                */

                if (lvMutated != null)
                {
                    lvMutated.GetFitness();

                    lock (mIndividuals)
                    {
                        mIndividuals.Add(lvMutated);
                    }
                }
            }
            else
            {
                /*
                if ((mECSClusters > 0) && mECS)
                {
                    if ((lvDaughter.Fitness >= mBestFitness) && !KeepingDiversity(lvDaughter))
                    {
                        lvDaughter.Clear();
                        lvDaughter = null;
                    }
                }
                */

                if (lvDaughter != null)
                {
                    lvDaughter.GetFitness();

                    lock (mIndividuals)
                    {
                        mIndividuals.Add(lvDaughter);
                    }
                }
            }

            lvDaughter = null;
            lvMutated = null;
        }
    }

    private bool UpdateOffSpring2(List<IIndividual<TrainMovement>> pIndividuals)
    {
        bool lvRes = true;
        IIndividual<TrainMovement> lvFather = null;
        IIndividual<TrainMovement> lvMother = null;
        List<IIndividual<TrainMovement>> lvSelectedIndividuals = new List<IIndividual<TrainMovement>>();
        bool lvValid = false;

        try
        {
            for (int ind = 0; ind < pIndividuals.Count / 2; ind++)
            {
                if (mSelectionMode == SELECTION_ENUM.Roulette)
                {
                    lvValid = RouletteWheelSelection(pIndividuals, out lvFather, out lvMother);
                }
                else if (mSelectionMode == SELECTION_ENUM.Tournament)
                {
                    lvValid = TournamentSelection(pIndividuals, mSelectionModeCount, mRandom, out lvFather, out lvMother);
                }

                if(lvValid)
                {
                    lvSelectedIndividuals.Add(lvFather);
                    lvSelectedIndividuals.Add(lvMother);
                }
            }

            if (mECS && (mECSClusters > 0))
            {
                UpdateClusters(lvSelectedIndividuals);
            }

            lvFather = null;
            lvMother = null;

#if DEBUG
            mStopWatch.Reset();
            mStopWatch.Start();
#endif

            if (MAX_PARALLEL_THREADS > 1)
            {
                /* lvFather = 2 * i; lvMother = 2 * i + 1 */
                Parallel.For(0, (lvSelectedIndividuals.Count / 2), new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, (i, loopState) =>
                {
                    lvFather = lvSelectedIndividuals[2*i];
                    lvMother = lvSelectedIndividuals[(2*i)+1];

                    GenerateChildren(lvFather, lvMother);

                    if (mMaxObjectiveFunctionCall > 0)
                    {
                        if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                        {
                            lvRes = false;
#if DEBUG
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
#endif
                            loopState.Stop();
                            return;
                        }
                    }
                });
            }
            else
            {
                for (int i = 0; i < lvSelectedIndividuals.Count; i+=2)
                {
                    lvFather = lvSelectedIndividuals[i];
                    lvMother = lvSelectedIndividuals[i+1];

                    GenerateChildren(lvFather, lvMother);

                    if (mMaxObjectiveFunctionCall > 0)
                    {
                        if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                        {
                            lvRes = false;
#if DEBUG
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
#endif
                            break;
                        }
                    }
                }
            }

#if DEBUG
            mStopWatch.Stop();

            DebugLog.Logar("mStopWatch for GenerateChildren = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);
#endif

            if (mMaxObjectiveFunctionCall > 0)
            {
                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    lvRes = false;
#if DEBUG
                    DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
#endif
                    return lvRes;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    private bool UpdateOffSpring(List<IIndividual<TrainMovement>> pIndividuals)
	{
        bool lvRes = true;
        int lvRandomValue = mRandom.Next(1, 101);
        IIndividual<TrainMovement> lvFather = null;
		IIndividual<TrainMovement> lvMother = null;
		IIndividual<TrainMovement> lvSon = null;
		IIndividual<TrainMovement> lvDaughter = null;
        IIndividual<TrainMovement> lvMutated = null;
        IIndividual<TrainMovement> lvClusterCenter = null;
        Cluster lvCluster = null;
        bool lvValid = false;
        bool lvRejected = false;

        try
        {
            for (int ind = 0; ind < pIndividuals.Count / 2; ind++)
            {
                if (mSelectionMode == SELECTION_ENUM.Roulette)
                {
                    lvValid = RouletteWheelSelection(pIndividuals, out lvFather, out lvMother);
                }
                else if (mSelectionMode == SELECTION_ENUM.Tournament)
                {
                    lvValid = TournamentSelection(pIndividuals, mSelectionModeCount, mRandom, out lvFather, out lvMother);
                }

                //DebugLog.Logar("UpdateOffSpring.lvFather.VerifyConflict() = " + ((TrainIndividual)lvFather).VerifyConflict(), pIndet: mCurrentGeneration);
                //DebugLog.Logar("UpdateOffSpring.lvMother.VerifyConflict() = " + ((TrainIndividual)lvMother).VerifyConflict(), pIndet: mCurrentGeneration);

                if (lvValid)
                {
                    if (mECS)
                    {
                        if ((mECSClusters > 0) && (mClusters.Count == mECSClusters))
                        {
                            lvClusterCenter = UpdateCluster(lvFather, out lvRejected);
                            if (lvClusterCenter != null)
                            {
                                lvClusterCenter.GetFitness();
                                mClustersCenter.Add(lvClusterCenter);
                            }

                            lvClusterCenter = UpdateCluster(lvMother, out lvRejected);
                            if (lvClusterCenter != null)
                            {
                                lvClusterCenter.GetFitness();
                                mClustersCenter.Add(lvClusterCenter);
                            }
                        }
                        else if ((mECSClusters > 0) && (mClusters.Count < mECSClusters))
                        {
                            for (int i = 0; i < pIndividuals.Count; i++)
                            {
                                if (!mECSClusterCentersSet.Contains(pIndividuals[i].GetUniqueId()))
                                {
                                    lvCluster = new Cluster(mECSFadeFactor);
                                    lvCluster.Center = pIndividuals[i];
                                    if (mClusters.Count < mECSClusters)
                                    {
                                        mClusters.Add(lvCluster);
                                        mECSClusterCentersSet.Add(pIndividuals[i].GetUniqueId());
                                        mClustersCenter.Add(pIndividuals[i]);
                                    }
                                    lvCluster = null;
                                }

                                if (mClusters.Count >= mECSClusters)
                                {
                                    break;
                                }
                            }

                            CalculateRadius();
                        }
                    }

                    DoCrossOver(lvFather, lvMother, out lvSon, out lvDaughter);
                }

                lvFather = null;
                lvMother = null;

                if (lvSon != null)
                {
                    if (lvRandomValue <= mMutationRate)
                    {
                        lvMutated = Mutate(lvSon, mMinMutationSteps);
                    }

                    if (lvMutated != null)
                    {
                        lvSon.Clear();

                        if ((mECSClusters > 0) && mECS)
                        {
                            if (!KeepingDiversity(lvMutated) && (lvMutated.Fitness >= mBestFitness))
                            {
                                lvMutated.Clear();
                                lvMutated = null;
                            }
                        }

                        if (lvMutated != null)
                        {
                            lvMutated.GetFitness();

                            lock (mIndividuals)
                            {
                                mIndividuals.Add(lvMutated);
                            }
                        }
                    }
                    else
                    {
                        if ((mECSClusters > 0) && mECS)
                        {
                            if (!KeepingDiversity(lvSon) && (lvSon.Fitness >= mBestFitness))
                            {
                                lvSon.Clear();
                                lvSon = null;
                            }
                        }

                        if (lvSon != null)
                        {
                            lvSon.GetFitness();

                            lock (mIndividuals)
                            {
                                mIndividuals.Add(lvSon);
                            }
                        }
                    }

                    lvSon = null;
                    lvMutated = null;

                    if (mMaxObjectiveFunctionCall > 0)
                    {
                        if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                        {
                            lvRes = false;
#if DEBUG
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
#endif
                            return lvRes;
                        }
                    }
                }

                if (lvDaughter != null)
                {
                    if (lvRandomValue <= mMutationRate)
                    {
                        lvMutated = Mutate(lvDaughter, mMinMutationSteps);
                    }

                    if (lvMutated != null)
                    {
                        lvDaughter.Clear();

                        if ((mECSClusters > 0) && mECS)
                        {
                            if (!KeepingDiversity(lvMutated) && (lvMutated.Fitness >= mBestFitness))
                            {
                                lvMutated.Clear();
                                lvMutated = null;
                            }
                        }

                        if (lvMutated != null)
                        {
                            lvMutated.GetFitness();

                            lock (mIndividuals)
                            {
                                mIndividuals.Add(lvMutated);
                            }
                        }
                    }
                    else
                    {
                        if ((mECSClusters > 0) && mECS)
                        {
                            if (!KeepingDiversity(lvDaughter) && (lvDaughter.Fitness >= mBestFitness))
                            {
                                lvDaughter.Clear();
                                lvDaughter = null;
                            }
                        }

                        if (lvDaughter != null)
                        {
                            lvDaughter.GetFitness();

                            lock (mIndividuals)
                            {
                                mIndividuals.Add(lvDaughter);
                            }
                        }
                    }

                    lvFather = null;
                    lvMother = null;

                    lvDaughter = null;
                    lvMutated = null;

                    if (mMaxObjectiveFunctionCall > 0)
                    {
                        if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                        {
                            lvRes = false;
#if DEBUG
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
#endif
                            return lvRes;
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }
	
    private IIndividual<TrainMovement> HillClimbingSearch(IIndividual<TrainMovement> pIndividual)
    {
        IIndividual<TrainMovement> lvRes = pIndividual;
        IIndividual<TrainMovement> lvIndividual = null;
        StringBuilder lvStrRes = new StringBuilder();
        int lvMutationSteps;
        double lvRefFitness = pIndividual.Fitness;
        object lvLock = new object();
        bool lvFoundNewSolution = false;

        int lvInitFitnessCallNum = mFitness.FitnessCallNum;

        if ((pIndividual == null) || (pIndividual.GetUniqueId() == -1))
        {
            return null;
        }

        mHillClimbingCallReg++;

        //long lvInd = DateTime.Now.Ticks;

        //((RailRoadFitness)mFitness).Population = null;

        for (int lvStepInd = 0; lvStepInd < mLSSteps; lvStepInd++)
        {
            lvFoundNewSolution = false;
            if (MAX_PARALLEL_THREADS > 1)
            {
                Parallel.For(0, mLSNeighbors, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, (i, loopState) =>
                {
                    int lvThreadMutationSteps = mRandom.Next(mMinMutationSteps, mMaxMutationSteps + 1);
                    IIndividual<TrainMovement> lvThreadIndividual = Mutate(lvRes, lvThreadMutationSteps, false);

                    if (lvThreadIndividual != null)
                    {
                        if (lvThreadIndividual.Fitness < lvRes.Fitness)
                        {
                            if ((mLSImprovement > 0.0))
                            {
                                lock(lvLock)
                                {
                                    if ((lvRefFitness - lvThreadIndividual.Fitness) >= mLSImprovement)
                                    {
                                        lvRefFitness = lvThreadIndividual.Fitness;
                                        lvRes = lvThreadIndividual;
                                        lvFoundNewSolution = true;
                                        loopState.Stop();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                lock (lvLock)
                                {
                                    lvRefFitness = lvThreadIndividual.Fitness;
                                    lvRes = lvThreadIndividual;
                                    lvFoundNewSolution = true;
                                    loopState.Stop();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            lvThreadIndividual.Clear();
                            lvThreadIndividual = null;
                        }

                        if (mMaxObjectiveFunctionCall > 0)
                        {
                            if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                            {
                                loopState.Stop();
                                return;
                            }
                        }
                    }
                });

                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    return lvRes;
                }
            }
            else
            {
                for (int lvNeighborInd = 0; lvNeighborInd < mLSNeighbors; lvNeighborInd++)
                {
                    lvMutationSteps = mRandom.Next(mMinMutationSteps, mMaxMutationSteps + 1);
                    lvIndividual = Mutate(lvRes, lvMutationSteps, false);

                    if (lvIndividual != null)
                    {
                        if (lvIndividual.Fitness < lvRes.Fitness)
                        {
                            if ((mLSImprovement > 0.0))
                            {
                                if ((lvRefFitness - lvIndividual.Fitness) >= mLSImprovement)
                                {
                                    lvRes = lvIndividual;
                                    lvRefFitness = lvRes.Fitness;
                                    lvFoundNewSolution = true;
                                    break;
                                }
                            }
                            else
                            {
                                lvRes = lvIndividual;
                                lvRefFitness = lvRes.Fitness;
                                lvFoundNewSolution = true;
                                break;
                            }
                        }
                        else
                        {
                            lvIndividual.Clear();
                            lvIndividual = null;
                        }

                        if (mMaxObjectiveFunctionCall > 0)
                        {
                            if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                            {
                                /*
                                lvStrRes.Clear();
                                lvStrRes.Append("HillClimbingSearch => Fitness Called Num = ");
                                lvStrRes.Append(RailRoadFitness.FitnessCallNum);
                                lvStrRes.Append(" - lvRes = ");
                                lvStrRes.Append(lvRes.Fitness);

                                //dump(GetBestIndividual(), lvRes, " - HillClimbingSearch " + lvInd + " - RailRoadFitness.FitnessCallNum = " + RailRoadFitness.FitnessCallNum);
                                dump(GetBestIndividual(), lvRes);
                                DebugLog.Logar(lvStrRes.ToString(), false);
                                */

                //((RailRoadFitness)mFitness).Population = this;
                return lvRes;
                            }/*
                        else if ((RailRoadFitness.FitnessCallNum % mFunctionCallReg) == 0)
                        {
                            lvStrRes.Clear();
                            lvStrRes.Append("HillClimbingSearch => Fitness Called Num = ");
                            lvStrRes.Append(RailRoadFitness.FitnessCallNum);
                            lvStrRes.Append(" - lvRes = ");
                            lvStrRes.Append(lvRes.Fitness);

                            //dump(GetBestIndividual(), lvRes, " - HillClimbingSearch " + lvInd + " - RailRoadFitness.FitnessCallNum = " + RailRoadFitness.FitnessCallNum);
                            dump(GetBestIndividual(), lvRes);
                            DebugLog.Logar(lvStrRes.ToString(), false);
                        }*/
                        }
                    }
                }
            }

            if (mMaxObjectiveFunctionCall > 0)
            {
                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    /*
                    lvStrRes.Clear();
                    lvStrRes.Append("GDSearch => Fitness Called Num = ");
                    lvStrRes.Append(RailRoadFitness.FitnessCallNum);
                    lvStrRes.Append(" - lvRes = ");
                    lvStrRes.Append(lvRes.Fitness);

                    //dump(GetBestIndividual(), lvRes, " - GDSearch " + lvInd + " - RailRoadFitness.FitnessCallNum = " + RailRoadFitness.FitnessCallNum);
                    dump(GetBestIndividual(), lvRes);
                    DebugLog.Logar(lvStrRes.ToString(), false, pIndet: mCurrentGeneration);
                    */
                    //((RailRoadFitness)mFitness).Population = this;
                    return lvRes;
                } /*
                else if ((RailRoadFitness.FitnessCallNum % mFunctionCallReg) == 0)
                {
                    lvStrRes.Clear();
                    lvStrRes.Append("GDSearch => Fitness Called Num = ");
                    lvStrRes.Append(RailRoadFitness.FitnessCallNum);
                    lvStrRes.Append(" - lvRes = ");
                    lvStrRes.Append(lvRes.Fitness);

                    //dump(GetBestIndividual(), lvRes, " - GDSearch " + lvInd + " - RailRoadFitness.FitnessCallNum = " + RailRoadFitness.FitnessCallNum);
                    dump(GetBestIndividual(), lvRes);
                    DebugLog.Logar(lvStrRes.ToString(), false, pIndet: mCurrentGeneration);
                } */
            }

            if (!lvFoundNewSolution)
            {
                break;
            }
        }

        /*
        if(lvRes != null)
        {
            lvRes.GetFitness();
        }
        */

        /*
        if (mMaxObjectiveFunctionCall > 0)
        {
            lvStrRes.Clear();
            lvStrRes.Append("HillClimbingSearch Ended => Fitness Called Num = ");
            lvStrRes.Append(RailRoadFitness.FitnessCallNum);
            lvStrRes.Append(" - Cost = ");
            lvStrRes.Append(RailRoadFitness.FitnessCallNum - lvInitFitnessCallNum);
            lvStrRes.Append(" - lvRes = ");
            lvStrRes.Append(lvRes.Fitness);

            DebugLog.Logar(lvStrRes.ToString(), false);
        }
        */

        //((RailRoadFitness)mFitness).Population = this;

        return lvRes;
    }

    public void LocalSearchAll()
    {
        IIndividual<TrainMovement> lvIndividual = null;

        for(int i = 0; i < mIndividuals.Count; i++)
        {
            lvIndividual = HillClimbingSearch(mIndividuals[i]);
            mIndividuals[i] = lvIndividual;
        }
    }

    public void RLNS(int pCount = 0)
    {
        IIndividual<TrainMovement> lvBestIndividual = null;
        IIndividual<TrainMovement> lvCandidateIndividual = null;
        int lvInd = 0;

        lvBestIndividual = mIndividuals[0];

        while (true)
        {
            lvCandidateIndividual = HillClimbingSearch(lvBestIndividual);

            if(lvCandidateIndividual.Fitness < lvBestIndividual.Fitness)
            {
                lvBestIndividual = lvCandidateIndividual;
                mIndividuals[0] = lvBestIndividual;
            }

            if (UseMaxObjectiveFunctionCall())
            {
                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    break;
                }
            }
            else
            {
                lvInd++;

                if (lvInd > pCount)
                {
                    break;
                }
            }
        }
    }

    public void GRASP(double pAlpha, int pCount = 0)
    {
        IIndividual<TrainMovement> lvCandidateIndividual = null;
        int lvIndex = -1;
        int lvInd = 0;
        int lvLimitPos = (int)(mIndividuals.Count * pAlpha);

        while (true)
        {
            lvIndex = mRandom.Next(lvLimitPos + 1);
            lvCandidateIndividual = HillClimbingSearch(mIndividuals[lvIndex]);
            mIndividuals[lvIndex] = lvCandidateIndividual;

            if (UseMaxObjectiveFunctionCall())
            {
                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    break;
                }
            }
            else
            {
                lvInd++;

                if(lvInd > pCount)
                {
                    break;
                }
            }
        }
    }

    public bool NextGeneration()
    {
        bool lvRes = true;
        int lvIndElite = 0;
        int lvQuantToRemove = 0;
        int lvClusterCenterCount = 0;
        int lvIndex = -1;
        double lvPrevFitness = -1.0;
        double lvFitnessDistance = 0.0;
        IIndividual<TrainMovement> lvCurrentIndividual = null;
        Cluster lvCluster = null;

        //DebugLog.EnableDebug = true;

        bool lvLogEnabled = DebugLog.EnableDebug;

        if (mIndividuals.Count == 0)
        {
            return false;
        }

        if(mRandom == null)
        {
            mRandom = new Random();
        }

        mCurrentGeneration++;

        lvRes = UpdateOffSpring2(mIndividuals);
        //lvRes = UpdateOffSpring(new List<IIndividual<TrainMovement>>(mIndividuals));

        if (!lvRes)
        {
            return lvRes;
        }

        if(mECS)
        {
            if (mECSClusters > 0)
            {
                mClusters.Sort();

                if ((mCurrentGeneration % mECSAMWakeUp) == 0)
                {
#if DEBUG
                    mStopWatch.Reset();
                    mStopWatch.Start();

                    DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
                    DebugLog.Logar(" ---------------------------- Clusters Values for generation " + mCurrentGeneration + "(Min: " + mECSMinValue + " - Max: " + mECSMaxValue + ") ------------------------------------ ", false, pIndet: mCurrentGeneration);
#endif

                    if (!mECSOnlyHeated)
                    {
                        for (int i = 0; i < mECSClusterEliteValue; i++)
                        {
                            lvCluster = mClusters[i];
#if DEBUG
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Tentando Local Search: " + lvCluster.Center.Fitness + " ...", false, pIndet: mCurrentGeneration);
#endif
                            lvCurrentIndividual = HillClimbingSearch(lvCluster.Center);
                            if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < lvCluster.Center.Fitness))
                            {
#if DEBUG
                                DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Local Search: " + lvCluster.Center.Fitness + " -> " + lvCurrentIndividual.Fitness, false, pIndet: mCurrentGeneration);
                                //((TrainIndividual)lvCurrentIndividual).GenerateFlotFiles(DebugLog.LogPath);
#endif
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                lvCluster.Center = lvCurrentIndividual;
                                mECSClusterCentersSet.Add(lvCurrentIndividual.GetUniqueId());
                                mIndividuals.Add(lvCurrentIndividual);
                            }
                            lvCluster.Reset();
                            lvCurrentIndividual = null;

                            if (mMaxObjectiveFunctionCall > 0)
                            {
                                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        mECSClusterEliteValue = 0;
                    }

                    for (int i = mClusters.Count - 1; i >= mECSClusterEliteValue; i--)
                    {
                        lvCluster = mClusters[i];
                        //                        lvCluster.Chill();

                        if (lvCluster.Value > mECSMaxValue)
                        {
#if DEBUG
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Tentando Local Search: " + lvCluster.Center.Fitness + " ...", false, pIndet: mCurrentGeneration);
#endif
                            lvCurrentIndividual = HillClimbingSearch(lvCluster.Center);

                            if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < lvCluster.Center.Fitness))
                            {
#if DEBUG
                                DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Local Search: " + lvCluster.Center.Fitness + " -> " + lvCurrentIndividual.Fitness, false, pIndet: mCurrentGeneration);
                                //((TrainIndividual)lvCurrentIndividual).GenerateFlotFiles(DebugLog.LogPath);
#endif
                                mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                                lvCluster.Center = lvCurrentIndividual;
                                mECSClusterCentersSet.Add(lvCurrentIndividual.GetUniqueId());
                                mIndividuals.Add(lvCurrentIndividual);
                            }
                            lvCluster.Reset();
                            lvCurrentIndividual = null;

                            if (mMaxObjectiveFunctionCall > 0)
                            {
                                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                                {
                                    return false;
                                }
                            }
                        }
                        else if (lvCluster.Value < mECSMinValue)
                        {
#if DEBUG
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " eliminado = " + lvCluster.Value, false, pIndet: mCurrentGeneration);
#endif
                            lvCluster.Reset();
                            mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            mClusters.RemoveAt(i);
                        }
#if DEBUG
                        DebugLog.Logar("Cluster " + lvCluster.UniqueId + " (Fitness: " + lvCluster.Center.Fitness + " - Radius: " + lvCluster.Radius + ") = " + lvCluster.Value, false, pIndet: mCurrentGeneration);
#endif
                    }

#if DEBUG
                    /* Verificando o valor de aquecimento de cada Cluster */
                    DebugLog.Logar("Total Clusters = " + mClusters.Count + "; Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: mCurrentGeneration);
                    DebugLog.Logar(" --------------------------------------------------------------------------------------------------------------------------- ", false, pIndet: mCurrentGeneration);

                    mStopWatch.Stop();

                    DebugLog.Logar("mStopWatch for Local Search in Clusters = " + mStopWatch.Elapsed, false, pIndet: mCurrentGeneration);
#endif
                }

                foreach (Cluster lvItemCluster in mClusters)
                {
                    lvItemCluster.Chill();
                }

                mIndividuals.Sort();

                try
                {
                    mClustersCenter.Sort();
                }
                catch (Exception ex)
                {
#if DEBUG
                    DebugLog.EnableDebug = lvLogEnabled;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("mClustersCenter.Count => " + mClustersCenter.Count, false, pIndet: mCurrentGeneration);

                        DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
                        foreach (IIndividual<TrainMovement> lvIndv in mClustersCenter)
                        {
                            DebugLog.Logar("lvIndv " + lvIndv.GetUniqueId() + " = " + lvIndv.Fitness, false, pIndet: mCurrentGeneration);
                        }
                        DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
                    }
                    DebugLog.EnableDebug = lvLogEnabled;
#endif

                    DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
                }

                if ((mIndividuals.Count > 0) && (mIndividuals[0].Fitness < mBestFitness))
                {
                    mBestFitness = mIndividuals[0].Fitness;
                }

                if ((mClustersCenter.Count > 0) && (mClustersCenter[0].Fitness < mBestFitness))
                {
                    mBestFitness = mClustersCenter[0].Fitness;
                    mIndividuals.Insert(0, mClustersCenter[0]);
                }
            }
            else
            {
                mIndividuals.Sort();
                if (mIndividuals[0].Fitness < mBestFitness)
                {
                    mBestFitness = mIndividuals[0].Fitness;
                }
            }

            for (int i = mIndividuals.Count-1; i >= 0; i--)
            {
                lvCurrentIndividual = mIndividuals[i];
                if ((lvCurrentIndividual == null) || (lvCurrentIndividual.GetUniqueId() == -1))
                {
                    mIndividuals.RemoveAt(i);
                }
            }
        }
        else
        {
            mIndividuals.Sort();
            if (mIndividuals[0].Fitness < mBestFitness)
            {
                if(mLSStrategy == LS_STRATEGY_ENUM.HillClimbingBest)
                {
#if DEBUG
                    DebugLog.Logar("Individual " + mIndividuals[0].GetUniqueId() + " | Tentando Local Search: " + mIndividuals[0].Fitness + " ...", false, pIndet: mCurrentGeneration);
#endif
                    lvCurrentIndividual = HillClimbingSearch(mIndividuals[0]);
                    if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < mIndividuals[0].Fitness))
                    {
#if DEBUG
                        DebugLog.Logar("Individual " + mIndividuals[0].GetUniqueId() + " => " + lvCurrentIndividual.GetUniqueId() + " | Local Search: " + mIndividuals[0].Fitness + " -> " + lvCurrentIndividual.Fitness, false, pIndet: mCurrentGeneration);
#endif
                        mIndividuals[0] = lvCurrentIndividual;
                    }
                }

                mBestFitness = mIndividuals[0].Fitness;
            }
        }

        //Dump(mIndividuals[0]);

        if (mIndividuals.Count >= mSize)
        {
            lvQuantToRemove = mIndividuals.Count - mSize;
        }
        else
        {
            lvQuantToRemove = 0;
        }

        try
        {
            /*
            if (mLSStrategy != LS_STRATEGY_ENUM.None)
            {
                if (mLSStrategy == LS_STRATEGY_ENUM.GradientDescent)
                {
                    mIndividuals.Sort();
                }
                else if(mLSStrategy == LS_STRATEGY_ENUM.PTH)
                {
                    PTHSorter lvComparer = new PTHSorter();
                    mIndividuals.Sort(lvComparer);
                    mIndividuals.Reverse();

                }
                lvIndElite = (int)(mSize * ELITE_PERC);

                if (MAX_PARALLEL_THREADS > 1)
                {
                    Parallel.For(0, lvIndElite, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLEL_THREADS }, i =>
                    {
                        IIndividual<TrainMovement> lvCurrIndividual = HillClimbingSearch(mIndividuals[i]);
                        if (lvCurrIndividual != null)
                        {
                            mIndividuals[i].Clear();
                            mIndividuals[i] = lvCurrIndividual;
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < lvIndElite; i++)
                    {
                        if (mIndividuals[i].GetUniqueId() >= 0)
                        {
                            lvCurrentIndividual = HillClimbingSearch(mIndividuals[i]);
                            if (lvCurrentIndividual != null)
                            {
                                mIndividuals[i].Clear();
                                mIndividuals[i] = lvCurrentIndividual;
                            }
                        }
                    }
                }
            }
            else
            {
            */
                mIndividuals.Sort();
                lvIndElite = (int)(mSize * ELITE_PERC);
            //}

#if DEBUG
            DebugLog.Logar("Best Fitness = " + GetBestIndividual().Fitness, false, pIndet: mCurrentGeneration);
            DebugLog.Logar(" ", false, pIndet: mCurrentGeneration);
#endif

            if (mNicheDistance > 0.0)
            {
                for (int i = mIndividuals.Count-1; i >= 0; i--)
                {
                    lvCurrentIndividual = mIndividuals[i];

                    if (lvPrevFitness == -1.0)
                    {
                        lvPrevFitness = lvCurrentIndividual.Fitness;
                    }
                    else
                    {
                        lvFitnessDistance = Math.Abs(lvCurrentIndividual.Fitness - lvPrevFitness);
                        if (lvFitnessDistance <= mNicheDistance)
                        {
                            mIndividuals[i] = null;
                            mIndividuals.RemoveAt(i);
                            lvQuantToRemove--;
                            //i--;
                        }
                        else
                        {
                            lvPrevFitness = lvCurrentIndividual.Fitness;
                        }
                    }

                    if (lvQuantToRemove <= 0)
                    {
                        break;
                    }
                }
            }

            if (lvQuantToRemove > 0)
            {
                lvIndElite = (int)(mSize * ELITE_PERC);
                /*
                if (mECS && (mECSClusters > 0))
                {
                    mIndividuals.InsertRange(lvIndElite, mClustersCenter.GetRange(0, (int)(mECSClusterEliteUsed * mSize)));
                    lvQuantToRemove = mIndividuals.Count - mSize;
                    lvIndElite += (int)(mECSClusterEliteUsed * mSize);
                }
                */

                lvIndex = mRandom.Next(lvIndElite, mIndividuals.Count) - lvQuantToRemove;

                if (lvIndex <= lvIndElite)
                {
                    lvIndex = lvIndElite;
                }

                //for (int i = lvQuantToRemove; i > 0; i--)
                for (int i = 0; i < lvQuantToRemove; i++)
                {
                    mIndividuals[lvIndex] = null;
                    mIndividuals.RemoveAt(lvIndex);

                    if(mECS)
                    {
                        if ((mECSClusters > 0) && (lvIndElite < mClustersCenter.Count))
                        {
                            if (!mECSClusterCentersSet.Contains(mClustersCenter[lvIndElite].GetUniqueId()))
                            {
                                mClustersCenter[lvIndElite] = null;
                                mClustersCenter.RemoveAt(lvIndElite);
                            }
                            else
                            {
                                lvIndElite++;
                                if (lvIndElite < mClustersCenter.Count)
                                {
                                    mClustersCenter[lvIndElite] = null;
                                    mClustersCenter.RemoveAt(lvIndElite);
                                }
                            }
                        }
                    }
                }
            }

            if (mECS)
            {
                if ((mECSClusters > 0) && (lvIndElite < mClustersCenter.Count))
                {
                    lvClusterCenterCount = mClustersCenter.Count;
                    for (int i = lvIndElite; i < lvClusterCenterCount; i++)
                    {
                        if (!mECSClusterCentersSet.Contains(mClustersCenter[lvIndElite].GetUniqueId()))
                        {
                            mClustersCenter[lvIndElite] = null;
                            mClustersCenter.RemoveAt(lvIndElite);
                        }
                        else
                        {
                            lvIndElite++;
                            i++;
                            if (lvIndElite < mClustersCenter.Count)
                            {
                                mClustersCenter[lvIndElite] = null;
                                mClustersCenter.RemoveAt(lvIndElite);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    private void PTHSelection(List<IIndividual<TrainMovement>> pIndividuals, out IIndividual<TrainMovement> pFather, out IIndividual<TrainMovement> pMother)
    {
        int lvRandomValue1;
        int lvRandomValue2;
        int lvEliteLimit = -1;

        pFather = null;
        pMother = null;

        lvEliteLimit = (int)(pIndividuals.Count * ELITE_PERC);

        lvRandomValue1 = mRandom.Next((lvEliteLimit));
        lvRandomValue2 = mRandom.Next(pIndividuals.Count);

        pFather = pIndividuals[lvRandomValue1];
        pMother = pIndividuals[lvRandomValue2];
    }

    private bool TournamentSelection(List<IIndividual<TrainMovement>> pIndividuals, int pCount, Random pRandom, out IIndividual<TrainMovement> pFather, out IIndividual<TrainMovement> pMother)
    {
        bool lvRes = true;
        int lvFatherIndex = -1;
        int lvMotherIndex = -1;
        int lvindElite = (int)(mSize * ELITE_PERC);
        IIndividual<TrainMovement> lvFather = null;
        IIndividual<TrainMovement> lvMother = null;

        pFather = null;
        pMother = null;

        if(pRandom == null)
        {
            return false;
        }

        try
        {
            for (int i = 0; i < pCount; i++)
            {
                if(lvindElite >= pIndividuals.Count)
                {
                    lvindElite = pIndividuals.Count;
                }

                lvFatherIndex = pRandom.Next(lvindElite);

                if (lvFatherIndex < pIndividuals.Count)
                {
                    lvFather = pIndividuals[lvFatherIndex];

                    if ((lvFather != null) && (lvFather.GetUniqueId() != -1))
                    {
                        if ((pFather == null) || (lvFather.Fitness < pFather.Fitness))
                        {
                            pFather = lvFather;
                        }
                    }
                    else
                    {
                        pIndividuals.RemoveAt(lvFatherIndex);
                    }
                }

                lvMotherIndex = pRandom.Next(pIndividuals.Count);

                if (lvMotherIndex < pIndividuals.Count)
                {
                    lvMother = pIndividuals[lvMotherIndex];

                    if ((lvMother != null) && (lvMother.GetUniqueId() != -1))
                    {
                        if ((pMother == null) || (lvMother.Fitness < pMother.Fitness))
                        {
                            pMother = lvMother;
                        }
                    }
                    else
                    {
                        pIndividuals.RemoveAt(lvMotherIndex);
                    }
                }
            }

            if (pFather == null || pMother == null)
            {
                lvRes = false;
            }
            else if (pFather.GetUniqueId() == pMother.GetUniqueId())
            {
                lvRes = false;
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        return lvRes;
    }

    private bool RouletteWheelSelection(List<IIndividual<TrainMovement>> pIndividuals, out IIndividual<TrainMovement> pFather, out IIndividual<TrainMovement> pMother)
    {
    	double lvTotalFitness = 0.0;
    	double lvRandomValue1;
    	double lvRandomValue2;
    	double lvTotal = 0.0;
        IIndividual<TrainMovement> lvIndividual = null;
    	bool lvRes = true;

        pFather = null;
        pMother = null;
    	
        for(int i = 0; i < pIndividuals.Count; i++)
    	{
            lvIndividual = pIndividuals[i];

            if ((lvIndividual != null) && (lvIndividual.GetUniqueId() != -1))
            {
                lvTotalFitness += (1 / (1 + lvIndividual.Fitness));
            }
            else
            {
                pIndividuals.RemoveAt(i);
                i--;
            }
    	}
    	
    	lvRandomValue1 = lvTotalFitness * mRandom.NextDouble();
    	lvRandomValue2 = lvTotalFitness * mRandom.NextDouble();

        for (int i = 0; i < pIndividuals.Count; i++)
    	{
            lvTotal += (1 / (1 + pIndividuals[i].Fitness));
    		
    		if(lvRandomValue1 <= lvTotal)
    		{
    			if(pFather == null)
    			{
                    pFather = pIndividuals[i];

                    if (pMother == null)
                    {
                        if (lvRandomValue2 <= lvTotal)
                        {
                            if (i < pIndividuals.Count - 1)
                            {
                                pMother = pIndividuals[i + 1];
                            }
                            else
                            {
                                pMother = pIndividuals[0];
                            }
                        }
                    }
                }
    		}
    		
    		if(lvRandomValue2 <= lvTotal)
    		{
    			if(pMother == null)
    			{
                    pMother = pIndividuals[i];
    			}
    		}

            if ((pFather != null) && (pMother != null))
            {
                if (pFather.GetUniqueId() == pMother.GetUniqueId())
                {
                    lvRes = false;
                }
                break;
            }
    	}

        if(pFather == null || pMother == null)
        {
            lvRes = false;
        }

        return lvRes;
    }

    private void AddFromQueue(IIndividual<TrainMovement> pIndividual, Queue<TrainMovement> pQueue, HashSet<int> pHashRef = null)
    {
        TrainMovement lvTrainMovement = null;
        IEnumerable<Gene> lvTrainMovRes = null;
        int lvQueueCount = pQueue.Count;
        DateTime lvTimeLine = DateTime.MaxValue;

        bool lvIsLogEnables = DebugLog.EnableDebug;

        for (int i = 0; i < lvQueueCount; i++)
        {
            lvTrainMovement = pQueue.Dequeue();

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.EnableDebug = lvIsLogEnables;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("Tentando AddFromQueue.MoveTrain(" + lvTrainMovement + ")", pIndet: mCurrentGeneration);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
            }
#endif

            lvTrainMovRes = pIndividual.MoveTrain(lvTrainMovement, lvTimeLine);
            if (lvTrainMovRes != null)
            {
#if DEBUG
                DebugLog.EnableDebug = lvIsLogEnables;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("Adicionado lvTrainMovRes = " + (TrainMovement)lvTrainMovRes, pIndet: mCurrentGeneration);

                    ((TrainIndividual)pIndividual).DumpTrain(lvTrainMovement.Last.TrainId);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                if (pHashRef != null)
                {
                    pHashRef.Add(lvTrainMovement.GetID());
                }
            }
            else
            {
#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.EnableDebug = lvIsLogEnables;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("pQueue.Enqueue(lvTrainMovement)", pIndet: mCurrentGeneration);
                    }
                    DebugLog.EnableDebug = lvIsLogEnables;
                }
#endif

                if (!pIndividual.hasArrived(lvTrainMovement.Last.TrainId))
                {
                    pQueue.Enqueue(lvTrainMovement);
                }
            }
        }
    }

    private void DoCrossOver(IIndividual<TrainMovement> pFather, IIndividual<TrainMovement> pMother, out IIndividual<TrainMovement> pSon, out IIndividual<TrainMovement> pDaughter, double pFatherRate = 0.0)
    {
        int lvCount = 0;
        List<int> lvCrossOverPos = null;
        int lvIndividualSize = pFather.Count - 1;
        int lvCurrValue = -1;
        int lvCrossOverPointsNum = 0;
        int lvFatherRateValue = 0;
        TrainMovement lvMovFather = null;
        TrainMovement lvMovMother = null;
        Queue<TrainMovement> lvQueueSon = new Queue<TrainMovement>();
        Queue<TrainMovement> lvQueueDaughter = new Queue<TrainMovement>();
        IEnumerable<TrainMovement> lvInitialSon = null;
        IEnumerable<TrainMovement> lvInitialDaughter = null;
        IEnumerable<Gene> lvTrainMovRes = null;
        HashSet<int> lvCrossOverPointsCheck = new HashSet<int>();
        HashSet<int> lvHashSetSon = new HashSet<int>();
        HashSet<int> lvHashSetDaughter = new HashSet<int>();
        DateTime lvTimeLine = DateTime.MaxValue;
        pSon = null;
        pDaughter = null;

        //DebugLog.EnableDebug = true;

        bool lvIsLogEnables = DebugLog.EnableDebug;

#if DEBUG
        DebugLog.EnableDebug = lvIsLogEnables;
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ------------------------------------------ DoCrossOver  ------------------------------------------", pIndet: mCurrentGeneration);
        }
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

        lvCount = pMother.Count;
        if (pFather.Count < lvCount)
        {
            lvCount = pFather.Count;
        }

        if (mCrossOverPoints > 0)
        {
            lvCrossOverPointsNum = mCrossOverPoints;
            lvCrossOverPos = new List<int>(lvCrossOverPointsNum);
        }
        else if(pFatherRate > 0.0)
        {
            if(mTrainList.Count > (lvCount - 3))
            {
                return;
            }

            lvCurrValue = mRandom.Next(mTrainList.Count + 1, lvCount - 3);
            lvFatherRateValue = (int)(lvCount * pFatherRate);
            if ((lvCurrValue + lvFatherRateValue) >= (lvCount - 1))
            {
                lvCurrValue = (int)(lvCount - lvFatherRateValue - 1);
            }

            lvCrossOverPointsNum = 2;
            lvCrossOverPos = new List<int>(lvCrossOverPointsNum);
            lvCrossOverPos.Add(lvCurrValue);
            lvCrossOverPos.Add(lvCurrValue + lvFatherRateValue);
        }
        else
        {
            lvCrossOverPointsNum = mRandom.Next(mMinCrossOverPoints, mMaxCrossOverPoints + 1);
            lvCrossOverPos = new List<int>(lvCrossOverPointsNum);
        }

        try
        {
            while (lvCrossOverPos.Count < lvCrossOverPointsNum)
            {
                lvCurrValue = mRandom.Next(mTrainList.Count + 1, lvCount - 3);
                if (!lvCrossOverPointsCheck.Contains(lvCurrValue))
                {
                    lvCrossOverPos.Add(lvCurrValue);
                    lvCrossOverPointsCheck.Add(lvCurrValue);
                }
            }
            if (lvCrossOverPos.Count > 1)
            {
                lvCrossOverPos.Sort();
            }

/*
#if DEBUG
            DebugLog.EnableDebug = true;
            DebugLog.Logar("AddElements: \n", pIndet: mCurrentGeneration);
#endif
*/

            lvInitialSon = pFather.GetElements(mTrainList.Count, lvCrossOverPos[0]);
            pSon = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
            pSon.AddElements(lvInitialSon);

            foreach (TrainMovement lvTrainMov in lvInitialSon)
            {
                lvHashSetSon.Add(lvTrainMov.GetID());
            }
            //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: mCurrentGeneration);

#if DEBUG
            DebugLog.Logar("\n", pIndet: mCurrentGeneration);
#endif

            lvInitialDaughter = pMother.GetElements(mTrainList.Count, lvCrossOverPos[0]);
            pDaughter = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
            pDaughter.AddElements(lvInitialDaughter);

            foreach (TrainMovement lvTrainMov in lvInitialDaughter)
            {
                lvHashSetDaughter.Add(lvTrainMov.GetID());
            }
            //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: mCurrentGeneration);

#if DEBUG
            DebugLog.EnableDebug = lvIsLogEnables;
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("\nDoCrossOver => Inicio de adicao de Genes por MoveTrain (pSon: " + pSon.GetUniqueId() + ", pDaughter: " + pDaughter.GetUniqueId() + ") ...", pIndet: mCurrentGeneration);
                DebugLog.Logar("(pFather: " + pFather.GetUniqueId() + ") = " + pFather + "\n", pIndet: mCurrentGeneration);
                DebugLog.Logar("(pMother: " + pMother.GetUniqueId() + ") = " + pMother + "\n", pIndet: mCurrentGeneration);
                DebugLog.Logar("(pSon: " + pSon.GetUniqueId() + ") = " + pSon + "\n", pIndet: mCurrentGeneration);
                ((TrainIndividual)pSon).DumpCurrentState("pSon");
                ((TrainIndividual)pSon).DumpCurrentPosDic("pSon");
                DebugLog.Logar("(pDaughter: " + pDaughter.GetUniqueId() + ") = " + pDaughter + "\n", pIndet: mCurrentGeneration);
                ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                ((TrainIndividual)pDaughter).DumpCurrentPosDic("pDaughter");
                ((TrainIndividual)pSon).DumpStopArrivalLocation(null, 0, "pSon");
                ((TrainIndividual)pDaughter).DumpStopArrivalLocation(null, 0, "pDaughter");
                //((TrainIndividual)pSon).GenerateFlotFiles(DebugLog.LogPath);
                //((TrainIndividual)pDaughter).GenerateFlotFiles(DebugLog.LogPath);
            }
            DebugLog.EnableDebug = lvIsLogEnables;

            /*
            Gene lvGen = ((TrainIndividual)pSon).GetLastStep(10000, out lvCurrValue, Gene.STATE.IN);

            if(lvGen != null && lvGen.StopLocation.Location == 17364800)
            {
                ((TrainIndividual)pSon).GenerateFlotFiles(DebugLog.LogPath);
            }

            lvGen = ((TrainIndividual)pDaughter).GetLastStep(10000, out lvCurrValue, Gene.STATE.IN);

            if (lvGen != null && lvGen.StopLocation.Location == 17364800)
            {
                ((TrainIndividual)pDaughter).GenerateFlotFiles(DebugLog.LogPath);
            }
            */

#endif

            for (int ind = 1; ind <= lvCrossOverPointsNum; ind++)
            {
                for (int i = 0; i < lvCount; i++)
                {
                    lvMovFather = pFather[i];
                    lvMovMother = pMother[i];

                    /* Son */
                    if ((pSon != null) && (lvMovMother != null) && !pSon.hasArrived(lvMovMother[0].TrainId) && ((ind % 2) != 0) && (((ind < lvCrossOverPos.Count) && (pSon.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if (!lvHashSetSon.Contains(lvMovMother.GetID()))
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("Tentando pSon.MoveTrain(" + lvMovMother + ")\n", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvTrainMovRes = pSon.MoveTrain(lvMovMother, lvTimeLine);
                            if (lvTrainMovRes != null)
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Adicionado lvTrainMovRes = " + (TrainMovement)lvTrainMovRes, pIndet: mCurrentGeneration);
                                }
                                //((TrainIndividual)pSon).DumpTrain(lvMovMother.Last.TrainId);
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvHashSetSon.Add(lvMovMother.GetID());

                                if (lvQueueSon.Count > 0)
                                {
                                    AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                                }
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvQueueSon.Enqueue(lvMovMother)", pIndet: mCurrentGeneration);
                                    DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: mCurrentGeneration);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvQueueSon.Enqueue(lvMovMother);
#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pSon).hasInDic(lvMovMother[0].TrainId))
                                    {
                                        ((TrainIndividual)pSon).DumpCurrentPosDic("pSon");
                                        ((TrainIndividual)pSon).DumpCurrentState("pSon");
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvMovMother[0], pIndet: mCurrentGeneration);
                                        DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: mCurrentGeneration);
                                        ((TrainIndividual)pSon).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueSon.Peek() = " + lvQueueSon.Peek(), pIndet: mCurrentGeneration);

                                        /*
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pFather: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToString(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pMother: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToString(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        */

                                        //                                    lvGenes = pSon.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueSon.Count > mMaxDeadLockError)
                                {
                                    pSon = null;
                                    DebugLog.Logar("lvQueueSon.Count > Limite", pIndet: mCurrentGeneration);
                                }
                            }
                        }
                        /*
                        else
                        {
#if DEBUG
                            DebugLog.EnableDebug = true;
                            DebugLog.Logar("lvHashSetSon.ContainsMother(" + lvMovMother.GetID() + ": " + lvMovMother + ")\n", pIndet: mCurrentGeneration);
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        }
                        */
                    }
                    else if ((pSon != null) && (lvMovFather != null) && !pSon.hasArrived(lvMovFather[0].TrainId) && ((ind % 2) == 0) && (((ind < lvCrossOverPos.Count) && (pSon.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if (!lvHashSetSon.Contains(lvMovFather.GetID()))
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("Tentando pSon.MoveTrain(" + lvMovFather + ")\n", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvTrainMovRes = pSon.MoveTrain(lvMovFather, lvTimeLine);
                            if (lvTrainMovRes != null)
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Adicionado lvTrainMovRes = " + (TrainMovement)lvTrainMovRes, pIndet: mCurrentGeneration);
                                    //((TrainIndividual)pSon).DumpTrain(lvMovFather.Last.TrainId);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvHashSetSon.Add(lvMovFather.GetID());

                                if (lvQueueSon.Count > 0)
                                {
                                    AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                                }
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvQueueSon.Enqueue(lvMovFather)", pIndet: mCurrentGeneration);
                                    DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: mCurrentGeneration);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvQueueSon.Enqueue(lvMovFather);

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pSon).hasInDic(lvMovFather[0].TrainId))
                                    {
                                        ((TrainIndividual)pSon).DumpCurrentPosDic("pSon");
                                        ((TrainIndividual)pSon).DumpCurrentState("pSon");
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvMovFather[0], pIndet: mCurrentGeneration);
                                        DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: mCurrentGeneration);
                                        ((TrainIndividual)pSon).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueSon.Peek() = " + lvQueueSon.Peek(), pIndet: mCurrentGeneration);

                                        /*
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pFather: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pMother: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        */

                                        //                                    lvGenes = pSon.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueSon.Count > mMaxDeadLockError)
                                {
                                    pSon = null;
                                    DebugLog.Logar("lvQueueSon.Count > Limite", pIndet: mCurrentGeneration);
                                }
                            }
                        }
                        /*
                        else
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvISLogEnables;
                            DebugLog.Logar("lvHashSetSon.ContainsFather(" + lvMovFather.GetID() + ": " + lvMovFather +")\n", pIndet: mCurrentGeneration);
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        }
                        */
                    }

                    /* Daughter */
                    if ((pDaughter != null) && (lvMovFather != null) && !pDaughter.hasArrived(lvMovFather[0].TrainId) && ((ind % 2) != 0) && (((ind < lvCrossOverPos.Count) && (pDaughter.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if (!lvHashSetDaughter.Contains(lvMovFather.GetID()))
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("Tentando pDaughter.MoveTrain(" + lvMovFather + ")\n", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvTrainMovRes = pDaughter.MoveTrain(lvMovFather, lvTimeLine);
                            if (lvTrainMovRes != null)
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Adicionado lvTrainMovRes = " + (TrainMovement)lvTrainMovRes, pIndet: mCurrentGeneration);
                                    //((TrainIndividual)pDaughter).DumpTrain(lvMovFather.Last.TrainId);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvHashSetDaughter.Add(lvMovFather.GetID());

                                if (lvQueueDaughter.Count > 0)
                                {
                                    AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                                }
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvQueueDaughter.Enqueue(lvMovFather)", pIndet: mCurrentGeneration);
                                    DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: mCurrentGeneration);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvQueueDaughter.Enqueue(lvMovFather);

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pDaughter).hasInDic(lvMovFather[0].TrainId))
                                    {
                                        ((TrainIndividual)pDaughter).DumpCurrentPosDic("pDaughter");
                                        ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvMovFather[0], pIndet: mCurrentGeneration);
                                        DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: mCurrentGeneration);
                                        ((TrainIndividual)pDaughter).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueDaughter.Peek() = " + lvQueueDaughter.Peek(), pIndet: mCurrentGeneration);

                                        /*
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pFather: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pMother: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        */

                                        //                                    lvGenes = pDaughter.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueDaughter.Count > mMaxDeadLockError)
                                {
                                    pDaughter = null;
                                    DebugLog.Logar("lvQueueDaughter.Count > Limite", pIndet: mCurrentGeneration);
                                }
                            }
                        }
                        /*
                        else
                        {
#if DEBUG
                            DebugLog.EnableDebug = true;
                            DebugLog.Logar("lvHashSetDaughter.ContainsFather(" + lvMovFather.GetID() + ": " + lvMovFather + ")\n", pIndet: mCurrentGeneration);
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        }
                        */
                    }
                    else if ((pDaughter != null) && (lvMovMother != null) && !pDaughter.hasArrived(lvMovMother[0].TrainId) && ((ind % 2) == 0) && (((ind < lvCrossOverPos.Count) && (pDaughter.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if (!lvHashSetDaughter.Contains(lvMovMother.GetID()))
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("Tentando pDaughter.MoveTrain(" + lvMovMother + ")\n", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvTrainMovRes = pDaughter.MoveTrain(lvMovMother, lvTimeLine);
                            if (lvTrainMovRes != null)
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Adicionado lvTrainMovRes = " + (TrainMovement)lvTrainMovRes, pIndet: mCurrentGeneration);
                                    //((TrainIndividual)pDaughter).DumpTrain(lvMovMother.Last.TrainId);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvHashSetDaughter.Add(lvMovMother.GetID());

                                if (lvQueueDaughter.Count > 0)
                                {
                                    AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                                }
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvQueueDaughter.Enqueue(lvMovMother)", pIndet: mCurrentGeneration);
                                    DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: mCurrentGeneration);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvQueueDaughter.Enqueue(lvMovMother);

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pDaughter).hasInDic(lvMovMother[0].TrainId))
                                    {
                                        ((TrainIndividual)pDaughter).DumpCurrentPosDic("pDaughter");
                                        ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvMovMother[0], pIndet: mCurrentGeneration);
                                        DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: mCurrentGeneration);
                                        ((TrainIndividual)pDaughter).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueDaughter.Peek() = " + lvQueueDaughter.Peek(), pIndet: mCurrentGeneration);

                                        /*
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pFather: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("pMother: \n", pIndet: mCurrentGeneration);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: mCurrentGeneration);
                                        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
                                        */

                                        //                                    lvGenes = pDaughter.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueDaughter.Count > mMaxDeadLockError)
                                {
                                    pDaughter = null;
                                    DebugLog.Logar("lvQueueDaughter.Count > Limite", pIndet: mCurrentGeneration);
                                }
                            }
                        }
                        /*
                        else
                        {
#if DEBUG
                            DebugLog.EnableDebug = true;
                            DebugLog.Logar("lvHashSetDaughter.ContainsMother(" + lvMovMother.GetID() + ": " + lvMovMother + ")\n", pIndet: mCurrentGeneration);
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        }
                        */
                    }

                    if ((ind < lvCrossOverPos.Count) && ((pSon == null) || (pSon.Count >= lvCrossOverPos[ind])) && ((pDaughter == null) || (pDaughter.Count >= lvCrossOverPos[ind])))
                    {
                        break;
                    }
                }

                //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: mCurrentGeneration);
                //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: mCurrentGeneration);
            }

            if (pSon != null)
            {
                ((TrainIndividual)pSon).DumpStopArrivalLocation(null, -1, "pSon");

                if (lvQueueSon.Count > 0)
                {
                    AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                }

                if (pSon.Count != lvCount)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pSon) => ", false, pIndet: mCurrentGeneration);
#if DEBUG
                    DebugLog.EnableDebug = true;
                    ((TrainIndividual)pSon).DumpCurrentState("pSon");
                    ((TrainIndividual)pSon).DumpStopLocation(null);
                    DebugLog.Logar("(pSon: " + pSon.GetUniqueId() + ") = " + pSon, pIndet: mCurrentGeneration);
                    ((TrainIndividual)pFather).DumpDifference(pSon);
                    ((TrainIndividual)pFather).GenerateFlotFiles(DebugLog.LogPath);
                    ((TrainIndividual)pSon).GenerateFlotFiles(DebugLog.LogPath);
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif
                    pSon.Clear();
                    pSon = null;
                }
            }

            if (pDaughter != null)
            {
                //((TrainIndividual)pDaughter).DumpStopArrivalLocation(null, -1, "pDaughter");

                if (lvQueueDaughter.Count > 0)
                {
                    AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                }

                if (pDaughter.Count != lvCount)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pDaughter) => ", false, pIndet: mCurrentGeneration);
#if DEBUG
                    DebugLog.EnableDebug = true;
                    ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                    ((TrainIndividual)pDaughter).DumpStopLocation(null);
                    DebugLog.Logar("(pDaughter: " + pDaughter.GetUniqueId() + ") = pDaughter", pIndet: mCurrentGeneration);
                    ((TrainIndividual)pMother).DumpDifference(pDaughter);
                    ((TrainIndividual)pMother).GenerateFlotFiles(DebugLog.LogPath);
                    ((TrainIndividual)pDaughter).GenerateFlotFiles(DebugLog.LogPath);
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif
                    pDaughter.Clear();
                    pDaughter = null;
                }
            }

#if DEBUG
            DebugLog.EnableDebug = lvIsLogEnables;
            if (DebugLog.EnableDebug)
            {
                //DebugLog.Logar("(pSon: " + pSon.GetUniqueId() + ") = " + pSon, pIndet: mCurrentGeneration);
                //DebugLog.Logar("(pDaughter: " + pDaughter.GetUniqueId() + ") = pDaughter", pIndet: mCurrentGeneration);
                DebugLog.Logar(" -----------------------------------------------------------------------------------------------------", pIndet: mCurrentGeneration);
            }
            DebugLog.EnableDebug = lvIsLogEnables;

            //((TrainIndividual)pSon).GenerateFlotFiles(DebugLog.LogPath);
            //((TrainIndividual)pDaughter).GenerateFlotFiles(DebugLog.LogPath);
#endif

            //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: mCurrentGeneration);
            //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: mCurrentGeneration);
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        //DebugLog.EnableDebug = false;
    }

    /*
    private IIndividual<TrainMovement> HasteGene(IIndividual<TrainMovement> pIndividual, double pTrainId)
    {
        IIndividual<TrainMovement> lvRes = pIndividual;
        IIndividual<TrainMovement> lvIndividual = null;
        Gene lvGene = null;
        int lvRef = Convert.ToInt32(pIndividual.Count * 0.2);
        int lvCount = 0;
        int lvRandomValue = mRandom.Next(lvRef, pIndividual.Count);// - (lvRef / 2));

        for (int i = lvRandomValue; i < pIndividual.Count; i++)
        {
            lvGene = pIndividual[i];
            if((lvGene.TrainId == pTrainId) && (lvGene.State == Gene.STATE.OUT))
            {
                lvIndividual = Mutate(lvRes, i, -1, true);

                if(lvIndividual != null)
                {
                    lvRes = lvIndividual;
                }

                lvCount++;

                if (lvCount >= mMinMutationSteps)
                {
                    break;
                }
            }
        }

        return lvRes;
    }
    */

    private IIndividual<TrainMovement> Mutate(IIndividual<TrainMovement> pIndividual, int pSteps, bool pUpdate = true)
    {
        int lvNewPosition = -1;
        int lvInitialPos = -1;
        TrainMovement lvTrainMovement = null;
        IIndividual<TrainMovement> lvMutatedIndividual = null;
        IEnumerable<Gene> lvTrainMovRes = null;
        List<int> lvRefPos = new List<int>();
        Queue<TrainMovement> lvQueue = new Queue<TrainMovement>();
        TrainMovement lvRefTrainMovement = null;
        DateTime lvTimeLine = DateTime.MaxValue;
        int lvPrevTrainMovementIndex = -1;
        int lvEndIndex = -1;
        bool lvOnStartStopLocation = false;

        bool lvIsLogEnables = DebugLog.EnableDebug;

#if DEBUG
        DebugLog.EnableDebug = lvIsLogEnables;
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ------------------------ Mutate(" + pIndividual.GetUniqueId() + ", " + pSteps + ", " + pUpdate + ") -------------------------- ", pIndet: mCurrentGeneration);
        }
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

        try
        {
            if ((pSteps > 0) && (pIndividual != null) && (pIndividual.GetUniqueId() >= 0))
            {
                lvInitialPos = mRandom.Next(mTrainList.Count + 1 + (int)(pIndividual.Count * 0.2), (int)(pIndividual.Count * 0.9));

                lvTrainMovement = pIndividual[lvInitialPos];

#if DEBUG
                DebugLog.EnableDebug = lvIsLogEnables;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvInitialPos = " + lvInitialPos, pIndet: mCurrentGeneration);
                    DebugLog.Logar("lvTrainMovement = " + lvTrainMovement, pIndet: mCurrentGeneration);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                //DebugLog.Logar("lvInitialPos = " + lvInitialPos, pIndet: mCurrentGeneration);

                lvRefTrainMovement = lvTrainMovement;

                if ((lvTrainMovement != null) && (lvTrainMovement[0].StopLocation == null))
                {
                    return lvMutatedIndividual;
                }

                if (lvTrainMovement[0].StopLocation.Location == lvTrainMovement[0].StartStopLocation.Location)
                {
                    lvOnStartStopLocation = true;
                }

                if (pSteps > 1)
                {
                    for (int i = lvInitialPos+1; i < pIndividual.Count; i++)
                    {
                        if (pIndividual[i].Last.TrainId == lvTrainMovement.Last.TrainId)
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("lvRefPos.Add(" + i + ")", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvRefPos.Add(i);

                            if((lvRefPos.Count >= pSteps) || lvOnStartStopLocation)
                            {
                                break;
                            }
                        }
                        else if(lvOnStartStopLocation && (pIndividual[i][0].StopLocation.Location == pIndividual[i][0].StartStopLocation.Location) && (pIndividual[i].Last.Direction == lvTrainMovement.Last.Direction))
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("lvRefPos.Add(" + i + ")", pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvRefPos.Add(i+1);

                            break;
                        }
                    }
                }

                lvPrevTrainMovementIndex = -1;
                for (int i = lvInitialPos - 2; i >= 0; i--)
                {
                    if (pIndividual[i].Last.TrainId == lvTrainMovement.Last.TrainId)
                    {
                        lvPrevTrainMovementIndex = i;
                        break;
                    }
                    else if (lvOnStartStopLocation && (pIndividual[i][0].StopLocation.Location == pIndividual[i][0].StartStopLocation.Location) && (pIndividual[i].Last.Direction == lvTrainMovement.Last.Direction))
                    {
                        lvPrevTrainMovementIndex = i+1;
                        break;
                    }
                }
#if DEBUG
                DebugLog.EnableDebug = lvIsLogEnables;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvPrevTrainMovementIndex = " + lvPrevTrainMovementIndex, pIndet: mCurrentGeneration);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                if (lvPrevTrainMovementIndex < 0)
                {
                    return lvMutatedIndividual;
                }

                if ((lvPrevTrainMovementIndex + 1) < lvInitialPos)
                {
                    if (lvPrevTrainMovementIndex + 1 > mTrainList.Count)
                    {
                        lvNewPosition = mRandom.Next(lvPrevTrainMovementIndex + 1, lvInitialPos);
                    }
                    else
                    {
                        lvNewPosition = mRandom.Next(mTrainList.Count + 1, lvInitialPos);
                    }
                    lvTrainMovement = pIndividual[lvNewPosition];
                }
                else
                {
                    return null;
                }

#if DEBUG
                DebugLog.EnableDebug = lvIsLogEnables;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvNewPosition = " + lvNewPosition, pIndet: mCurrentGeneration);
                    DebugLog.Logar("lvTrainMovement = " + lvTrainMovement, pIndet: mCurrentGeneration);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                if ((lvRefTrainMovement.Last.TrainId != lvTrainMovement.Last.TrainId) && ((mTrainList.Count + 1) < lvInitialPos))
                {
                    lvMutatedIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mTrainSequence, mRandom);
#if DEBUG
                    DebugLog.EnableDebug = lvIsLogEnables;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("\n" + lvTrainMovement, pIndet: mCurrentGeneration);
                        DebugLog.Logar("lvMutatedIndividual criado = " + lvMutatedIndividual, pIndet: mCurrentGeneration);
                        DebugLog.Logar("lvMutatedIndividual.Count = " + lvMutatedIndividual.Count, pIndet: mCurrentGeneration);
                    }
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif
                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);
                    lvEndIndex = lvNewPosition - 1;

                    if (lvEndIndex < 0)
                    {
                        lvEndIndex = 0;
                    }

                    //DebugLog.Logar("lvEndIndex = " + lvEndIndex);

                    lvMutatedIndividual.AddElements(pIndividual.GetElements(mTrainList.Count, lvEndIndex));

#if DEBUG
                    DebugLog.EnableDebug = lvIsLogEnables;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("\n lvTrainMovement = " + lvTrainMovement, pIndet: mCurrentGeneration);
                        DebugLog.Logar("lvMutatedIndividual apos adicionar elementos ate lvEndIndex(" + lvEndIndex + ") = " + lvMutatedIndividual, pIndet: mCurrentGeneration);
                    }

                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("\n" + lvTrainMovement, pIndet: mCurrentGeneration);
                        DebugLog.Logar("inserindo Movimento da mutacao obtido de pIndividual[" + lvInitialPos + "](lvInitialPos) = " + pIndividual[lvInitialPos], pIndet: mCurrentGeneration);
                    }
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif

                    /* insere o RefGene correspondente */
                    lvTrainMovRes = lvMutatedIndividual.MoveTrain(pIndividual[lvInitialPos], lvTimeLine, pUpdate);
                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);

                    if (lvTrainMovRes == null)
                    {
#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvQueue.Enqueue(pIndividual[lvInitialPos])", pIndet: mCurrentGeneration);
                            DebugLog.Logar("lvQueue.Count = " + lvQueue.Count, pIndet: mCurrentGeneration);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif

                        lvQueue.Enqueue(pIndividual[lvInitialPos]);
                    }
                    else
                    {
#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("Movimento inserido !", pIndet: mCurrentGeneration);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        if (lvQueue.Count > 0)
                        {
                            AddFromQueue(lvMutatedIndividual, lvQueue);
                        }
                    }

                    /* Insere os genes restantes */
                    for (int i = 0; i < lvRefPos.Count; i++)
                    {
                        lvInitialPos = lvNewPosition;
                        lvEndIndex = lvRefPos[i];
                        /* Procurar novo Gene com mesmo trainId de RefGene para estabelecer os limites */
                        lvNewPosition = mRandom.Next(lvInitialPos + 1, lvEndIndex);

                        /* Insere o Gene igual a RefGene no intervalo aleatório */
                        for (int ind = lvInitialPos; ind < lvNewPosition; ind++)
                        {
                            if (pIndividual[ind].Last.TrainId != lvRefTrainMovement.Last.TrainId)
                            {
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Tentando lvMutatedIndividual.MoveTrain(" + pIndividual[ind] + ")", pIndet: mCurrentGeneration);
                                }
                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                lvTrainMovRes = lvMutatedIndividual.MoveTrain(pIndividual[ind], lvTimeLine, pUpdate);
                                //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);

                                if (lvTrainMovRes == null)
                                {
#if DEBUG
                                    DebugLog.EnableDebug = lvIsLogEnables;
                                    if (DebugLog.EnableDebug)
                                    {
                                        DebugLog.Logar("lvQueue.Enqueue(pIndividual[ind])", pIndet: mCurrentGeneration);
                                        DebugLog.Logar("lvQueue.Count = " + lvQueue.Count, pIndet: mCurrentGeneration);
                                    }
                                    DebugLog.EnableDebug = lvIsLogEnables;
#endif

                                    lvQueue.Enqueue(pIndividual[ind]);
                                }
                                else
                                {
                                    if (lvQueue.Count > 0)
                                    {
                                        AddFromQueue(lvMutatedIndividual, lvQueue);
                                    }
                                }
                            }
                        }

#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvMutatedIndividual apos inserir os movimentos restantes = " + lvMutatedIndividual, pIndet: mCurrentGeneration);
                            DebugLog.Logar("lvMutatedIndividual.Count = " + lvMutatedIndividual.Count, pIndet: mCurrentGeneration);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif

#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("Tentando lvMutatedIndividual.MoveTrain(" + pIndividual[lvEndIndex] + ")", pIndet: mCurrentGeneration);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif

                        /* insere o RefGene correspondente */
                        lvTrainMovRes = lvMutatedIndividual.MoveTrain(pIndividual[lvEndIndex], lvTimeLine, pUpdate);
                        //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);

                        if (lvTrainMovRes == null)
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("lvQueue.Enqueue(pIndividual[lvEndIndex])" + pIndividual[lvEndIndex], pIndet: mCurrentGeneration);
                                DebugLog.Logar("lvQueue.Count = " + lvQueue.Count, pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvQueue.Enqueue(pIndividual[lvEndIndex]);
                        }
                        else
                        {
                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }
                        }
                    }

                    /* preenche com o restante do indivíduo */
                    for (int ind = lvNewPosition; ind < pIndividual.Count; ind++)
                    {
#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("Tentando lvMutatedIndividual.MoveTrain(" + pIndividual[ind] + ")", pIndet: mCurrentGeneration);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif

                        lvTrainMovRes = lvMutatedIndividual.MoveTrain(pIndividual[ind], lvTimeLine, pUpdate);
                        //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);

                        if (lvTrainMovRes == null)
                        {
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("lvQueue.Enqueue(pIndividual[ind])", pIndet: mCurrentGeneration);
                                DebugLog.Logar("lvQueue.Count = " + lvQueue.Count, pIndet: mCurrentGeneration);
                            }
                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            lvQueue.Enqueue(pIndividual[ind]);
                        }
                        else
                        {
                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }
                        }
                    }

                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    if (lvMutatedIndividual.Count != pIndividual.Count)
                    {
                        DebugLog.Logar("Mutate DeadLock Found !!! => ", false, pIndet: mCurrentGeneration);
#if DEBUG
                        DebugLog.EnableDebug = true;
                        DebugLog.Logar("(pIndividual: " + pIndividual.GetUniqueId() + ") = " + pIndividual + "\n", pIndet: mCurrentGeneration);
                        DebugLog.Logar("(lvMutatedIndividual: " + lvMutatedIndividual.GetUniqueId() + ") = " + lvMutatedIndividual + "\n", pIndet: mCurrentGeneration);
                        ((TrainIndividual)pIndividual).DumpDifference(lvMutatedIndividual);
                        ((TrainIndividual)pIndividual).GenerateFlotFiles(DebugLog.LogPath);
                        ((TrainIndividual)lvMutatedIndividual).GenerateFlotFiles(DebugLog.LogPath);
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif
                        lvMutatedIndividual.Clear();
                        lvMutatedIndividual = null;
                    }

                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: mCurrentGeneration);
                }
            }

#if DEBUG
            DebugLog.EnableDebug = lvIsLogEnables;
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" --------------------------------------------------------------------------------------------------------------- ", pIndet: mCurrentGeneration);
            }
            DebugLog.EnableDebug = lvIsLogEnables;

            /*
            if (lvMutatedIndividual != null)
            {
                ((TrainIndividual)lvMutatedIndividual).GenerateFlotFiles(DebugLog.LogPath);
            }
            */
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: mCurrentGeneration);
        }

        DebugLog.EnableDebug = lvIsLogEnables;

        return lvMutatedIndividual;
    }

    public static string TrainAllowed
    {
        set
        {
            if (value.Trim().Length > 0)
            {
                string[] lvStrTrens = value.Trim().Split('|');
                foreach (string lvTrem in lvStrTrens)
                {
                    mTrainAllowed.Add(lvTrem);
                }
            }
        }
    }

    public static int MAX_DEAD_LOCK_ERROR
    {
        get
        {
            return mMaxDeadLockError;
        }

        set
        {
            mMaxDeadLockError = value;
        }
    }

    public double NicheDistance
    {
        get
        {
            return mNicheDistance;
        }

        set
        {
            mNicheDistance = value;
        }
    }

    public static int MAX_PARALLEL_THREADS
    {
        get
        {
            return mMaxParallelThread;
        }

        set
        {
            mMaxParallelThread = value;
        }
    }

    public static int MaxCrossOverPoints
    {
        get
        {
            return mMaxCrossOverPoints;
        }

        set
        {
            mMaxCrossOverPoints = value;
        }
    }

    public static int MinCrossOverPoints
    {
        get
        {
            return mMinCrossOverPoints;
        }

        set
        {
            mMinCrossOverPoints = value;
        }
    }

    public int HillClimbingCallReg
    {
        get
        {
            return mHillClimbingCallReg;
        }
    }

    public int MaxObjectiveFunctionCall
    {
        get
        {
            return mMaxObjectiveFunctionCall;
        }

        set
        {
            mMaxObjectiveFunctionCall = value;
        }
    }

    public static Dictionary<string, double> Priority
    {
        get
        {
            return mPriority;
        }
    }

    public int UniqueId
    {
        get
        {
            return mUniqueId;
        }
    }

    public int CurrentGeneration
    {
        get
        {
            return mCurrentGeneration;
        }
    }

    public void Dump(IIndividual<TrainMovement> pBestIndividual)
    {
        int lvInd = 0;
        StringBuilder lvResText;

        DebugLog.Logar(" ");
        DebugLog.Logar(" -------------------------- dump Individuals --------------------------- ", pIndet: mCurrentGeneration);
        DebugLog.Logar(" ");
        foreach (IIndividual<TrainMovement> lvIndividual in mIndividuals)
        {
            lvResText = new StringBuilder("Individual ");
            lvResText.Append(lvIndividual.GetUniqueId());
            lvResText.Append(" | ");
            lvResText.Append(lvIndividual.Fitness);
            lvResText.Append(" | Distancia: ");
            lvResText.Append(pBestIndividual.GetDistanceFrom(lvIndividual));
            lvResText.Append(" | Tamanho: ");
            lvResText.Append(lvIndividual.Count);

            DebugLog.Logar(lvResText.ToString(), false, pIndet: mCurrentGeneration);

            if(lvInd == 0)
            {
                DebugLog.Logar(pBestIndividual.Fitness.ToString(), "log_result_" + mSeed, false);
            }
            lvInd++;
        }
        DebugLog.Logar(" ", pIndet: mCurrentGeneration);
        DebugLog.Logar("Count = " + mIndividuals.Count, pIndet: mCurrentGeneration);
        DebugLog.Logar(" ----------------------------------------------------------------------  ", pIndet: mCurrentGeneration);
    }

    public static void dump(IIndividual<TrainMovement> pBestIndividual, IIndividual<TrainMovement> pBestCandidateIndividual, string pStr = "")
    {
        if ((pBestCandidateIndividual == null) || (pBestIndividual.Fitness < pBestCandidateIndividual.Fitness))
        {
            DebugLog.Logar(pBestIndividual.Fitness.ToString() + pStr, "log_result_" + mSeed, false);
        }
        else
        {
            DebugLog.Logar(pBestCandidateIndividual.Fitness.ToString() + pStr, "log_result_" + mSeed, false);
        }
    }
}