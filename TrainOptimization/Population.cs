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

/// <summary>
/// Summary description for Population
/// </summary>
public class Population
{
    private List<IIndividual<Gene>> mIndividuals = null;
    private List<IIndividual<Gene>> mClustersCenter = null;
    private static Dictionary<string, double> mPriority = new Dictionary<string, double>();
    private Dictionary<int, List<IIndividual<Gene>>> mClusterAssignment = null;
    private static Dictionary<Int64, List<Trainpat>> mPATs = null;
    private IFitness<Gene> mFitness = null;
    private DateTime mInitialDate;
    private DateTime mFinalDate;
    private DateTime mDateRef;
    private List<Gene> mTrainList = null;
    private List<Gene> mPlanList = null;
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
    private int mECSRemoveValue = 0;
    private int mUniqueId = -1;
    private Random mRandom = null;

    private Stopwatch mStopWatch = null;

    private enum LS_STRATEGY_ENUM
    {
        None,
        GradientDescent,
        PTH,
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

    public Population(IFitness<Gene> pFitness, int pSize, int pMutationRate, int pCrossOverPoints, List<Gene> pTrainList, List<Gene> pPlanList, DateTime pInitialDate = default(DateTime), DateTime pFinalDate = default(DateTime), Dictionary<Int64, List<Trainpat>> pPATs = null)
	{
        mDateRef = DateTime.MinValue;
        mSeed = DateTime.Now.Millisecond;
        Random mRandom = new Random();
        Cluster lvCluster = null;
        mUniqueId = RuntimeHelpers.GetHashCode(this);

        mFitness = pFitness;
        //((RailRoadFitness)mFitness).Population = this;
        mMutationRate = pMutationRate;
        mCrossOverPoints = pCrossOverPoints;
        mInitialDate = pInitialDate;
        mFinalDate = pFinalDate;
        mClusterAssignment = new Dictionary<int, List<IIndividual<Gene>>>();

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
            case "pth":
                mLSStrategy = LS_STRATEGY_ENUM.PTH;
                break;
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

        if ((pTrainList == null) && (pPlanList == null))
        {
            LoadTrainList();

            if (DateTime.Now.Date == mInitialDate.Date)
            {
                mDateRef = DateTime.Now;
            }
            else
            {
                mDateRef = DateTime.MinValue;
                foreach (Gene lvGene in mTrainList)
                {
                    if (lvGene.Time > mDateRef)
                    {
                        mDateRef = lvGene.Time;
                    }
                }

                if ((mDateRef == DateTime.MinValue) && (mPlanList.Count > 0))
                {
                    mDateRef = mPlanList[0].DepartureTime;
                }
            }
        }
        else
        {
            mTrainList = new List<Gene>(pTrainList);
            mPlanList = new List<Gene>(pPlanList);
            mPATs = pPATs;
            foreach (Gene lvGene in mTrainList)
            {
                lvGene.DepartureTime = lvGene.Time;
                if (lvGene.Time > mDateRef)
                {
                    mDateRef = lvGene.Time;
                }
            }

            if ((mDateRef == DateTime.MinValue) && (mPlanList.Count > 0))
            {
                mDateRef = mPlanList[0].DepartureTime;
            }
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("Gerando Individuos:", pIndet: TrainIndividual.IDLog);
        }
#endif

        mSize = pSize;

        mIndividuals = new List<IIndividual<Gene>>();
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
                IIndividual<Gene> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
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
                IIndividual<Gene> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
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
                IIndividual<Gene> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, lvSavedIndividualsIds[i], mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    mIndividuals.Add(lvIndividual);
                }
            }

            for (int i = 0; i < pSize; i++)
            {
                IIndividual<Gene> lvIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
                TrainIndividual.IDLog = lvIndividual.GetUniqueId();
                bool lvValidIndividual = lvIndividual.GenerateIndividual(mPlanList, 0, mAllowDeadLockIndividual);

                if (lvValidIndividual)
                {
                    mIndividuals.Add(lvIndividual);
                }
            }
        }

#if DEBUG
        mStopWatch.Stop();

        DebugLog.Logar("mStopWatch for GenerateIndividual = " + mStopWatch.Elapsed, false, pIndet: TrainIndividual.IDLog);
#endif

        TrainIndividual.IDLog = 0;

        if (mIndividuals.Count == 0)
        {
            DebugLog.Save("Erro: Nenhum individuo foi criado no processo !");
        }
        else
        {
            mIndividuals.Sort();
        }

        if (mECS && (mECSClusters <= mIndividuals.Count))
        {
            if (mECSClusters > 0)
            {
                mECSFactorRefValue = pSize / mECSClusters;
                mECSMaxValue = mECSFactorRefValue * mECSFactorMax;
                mECSMinValue = mECSFactorRefValue * mECSFactorMin;
                mClustersCenter = new List<IIndividual<Gene>>();
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

                IIndividual<Gene> lvIndividual = mIndividuals[0];
                mECSRemoveValue = (int)(lvIndividual.Count * mECSRemoveFactor);
            }
        }
        else
        {
            mECS = false;
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
        }
#endif

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

        if (!Int32.TryParse(ConfigurationManager.AppSettings["NUM_SAVED_INDIVIDUALS"], out lvNumSavedIndividuals))
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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    public void SaveBestIndividuals()
    {
        DirectoryInfo lvDir;
        FileInfo[] lvFiles;
        int lvNumSavedIndividuals;
        IIndividual<Gene> lvIndividual;

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

            if ((mIndividuals.Count > lvNumSavedIndividuals) && (mIndividuals.Count > 0))
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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
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

    public IList<IIndividual<Gene>> GetIndividuals()
    {
        return mIndividuals.AsReadOnly();
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

    private bool KeepingDiversity(IIndividual<Gene> pIndividual)
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
                //DebugLog.Logar("Removendo individuo " + pIndividual.GetUniqueId() + "(" + pIndividual.Fitness + ") por estar muito proximo do centro de " + lvCloserCluster.Center.GetUniqueId() + " (distancia " + lvMinDistance + " < " + mECSRemoveValue + ")", false, pIndet: TrainIndividual.IDLog);
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    private void AssignIndividualToCluster(IIndividual<Gene> pIndividual)
    {
        Cluster lvCluster = null;
        List<IIndividual<Gene>> lvElements = null;
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
                        lvElements = new List<IIndividual<Gene>>();

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
                        lvElements = new List<IIndividual<Gene>>();

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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    private void UpdateClusters(List<IIndividual<Gene>> pIndividuals)
    {
        IIndividual<Gene> lvIndividual;
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

                DebugLog.Logar("mStopWatch for Creating new clusters = " + mStopWatch.Elapsed, false, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: TrainIndividual.IDLog);
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
                    IIndividual<Gene> lvThreadIndividual = pIndividuals[i];
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

            DebugLog.Logar("mStopWatch for AssignIndividualToCluster = " + mStopWatch.Elapsed, false, pIndet: TrainIndividual.IDLog);

            /*
            DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: TrainIndividual.IDLog);
            foreach (int clusterNum in mClusterAssignment.Keys)
            {
                DebugLog.Logar("Cluster " + clusterNum + " = " + mClusterAssignment[clusterNum].Count, false, pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar(" ------------------------------------------------------------------", false, pIndet: TrainIndividual.IDLog);
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

            DebugLog.Logar("mStopWatch for Assimilate = " + mStopWatch.Elapsed, false, pIndet: TrainIndividual.IDLog);
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    private void Assimilate(int pClusterIndex)
    {
        Cluster lvCluster = null;
        List<IIndividual<Gene>> lvIndividualList;
        IIndividual<Gene> lvBestIndividual = null;
        IIndividual<Gene> lvSon = null;
        IIndividual<Gene> lvDaughter = null;
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
                    foreach (IIndividual<Gene> lvIndiv in lvIndividualList)
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
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            }
        }
    }

    private void Assimilate2(int pClusterIndex)
    {
        Cluster lvCluster = null;
        List<IIndividual<Gene>> lvIndividualList;
        IIndividual<Gene> lvIndividual = null;
        IIndividual<Gene> lvSon = null;
        IIndividual<Gene> lvDaughter = null;
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
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            }
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private IIndividual<Gene> UpdateCluster(IIndividual<Gene> pIndividual, out bool pReject)
    {
        IIndividual<Gene> lvRes = null;
        Cluster lvCloserCluster = null;
        Cluster lvCluster = null;
        int lvMinDistance = Int32.MaxValue;
        int lvMinDistanceSon = Int32.MaxValue;
        int lvMinDistanceDaughter = Int32.MaxValue;
        int lvDistance = Int32.MaxValue;
        IIndividual<Gene> lvSon = null;
        IIndividual<Gene> lvDaughter = null;
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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
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
            foreach(IIndividual<Gene> lvIndividual in mIndividuals)
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

    private void LoadTrainList()
    {
        HashSet<Int64> lvTrainSet = new HashSet<Int64>();
        DataTable lvDataTrains = null;
        DataTable lvDataPlans = null;
        Gene lvGene = null;
        Segment lvSegment = null;
        StopLocation lvCurrentStopSegment = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvStartStopLocation = null;
        StopLocation lvEndStopLocation = null;
        double lvMeanSpeed = 0.0;
        int lvIndex;
        string lvKey;
        string lvStrTrainName = "";
        double lvVMA = TrainIndividual.VMA;

        int lvCoordinate;
        int lvDirection;
        int lvLocation;
        string lvStrUD;
        DateTime lvOcupTime;
        DateTime lvCreationtime;

        mTrainList = new List<Gene>();

#if DEBUG
        DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
        DebugLog.Logar("Listando trens a serem considerados:", false, pIndet: TrainIndividual.IDLog);
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
                        DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);

                        continue;
                    }

                    lvGene.Time = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                    lvLocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
                    lvStrUD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                    lvGene.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                    lvGene.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
                    lvGene.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
                    lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
                    lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
                    //lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                    lvGene.DepartureTime = lvGene.Time;
                    lvGene.State = Gene.STATE.IN;
                    lvCreationtime = ((row["creation_tm"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["creation_tm"].ToString()));

                    lvGene.SegmentInstance = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, lvGene.Track, out lvIndex);

                    if (lvGene.DepartureTime.AddYears(1) < lvCreationtime)
                    {
                        lvGene.DepartureTime = lvCreationtime;
                    }

                    if (lvGene.DepartureTime == DateTime.MinValue)
                    {
                        lvGene.DepartureTime = ((row["plan_departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["plan_departure_time"].ToString()));
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

                    lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                    lvGene.StopLocation = lvCurrentStopSegment;

                    if (lvCurrentStopSegment == null)
                    {
                        lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                    }
                    else
                    {
                        lvNextStopLocation = lvCurrentStopSegment.GetNextStopSegment(lvGene.Direction);
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
                    else if ((lvCurrentStopSegment == lvEndStopLocation) && (lvCurrentStopSegment != null))
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

                    if (mPriority.Keys.Count > 0)
                    {
                        lvKey = lvGene.TrainName.Substring(0, 1) + lvGene.Direction;
                        if (mPriority.ContainsKey(lvKey))
                        {
                            lvGene.ValueWeight = mPriority[lvKey];
                        }
                    }

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

                            if (lvGene.Track != 0)
                            {
                                mTrainList.Insert(0, lvGene);
                                lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                                DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ")", false, pIndet: TrainIndividual.IDLog);
#endif
                            }
                        }
                        else
                        {
                            if (lvGene.Track != 0)
                            {
                                mTrainList.Add(lvGene);
                                lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                                DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ")", false, pIndet: TrainIndividual.IDLog);
#endif
                            }
                        }
                    }
                    else
                    {
                        if (lvGene.Track != 0)
                        {
                            mTrainList.Insert(0, lvGene);
                            lvTrainSet.Add(lvGene.TrainId);
#if DEBUG
                            DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Time: " + lvGene.Time + "; Partida: " + lvGene.DepartureTime + "; End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ") - Location: " + lvGene.SegmentInstance.Location + "." + lvGene.SegmentInstance.SegmentValue + ", Track" + lvGene.Track + ")", false, pIndet: TrainIndividual.IDLog);
#endif
                        }
                    }

                    LoadPATs(lvGene.TrainId);
                }
            }

#if DEBUG
            DebugLog.Logar("Total = " + mTrainList.Count, false, pIndet: TrainIndividual.IDLog);
#endif
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        try
        {
            mPlanList = new List<Gene>();

#if DEBUG
            DebugLog.Logar(" ------------------------------------------------------------------------------------------------------ ", false, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar("Listando Planos a serem considerados:", false, pIndet: TrainIndividual.IDLog);
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
                    DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);

                    continue;
                }

                if (!lvTrainSet.Contains(lvGene.TrainId))
                {
                    lvGene.TrainName = ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString());

                    if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                    {
                        if (lvGene.TrainName.Trim().Length == 0) continue;

                        lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
                        lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
                        lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                        lvGene.Time = DateTime.MinValue;
                        lvGene.Coordinate = lvGene.Start;
                        lvGene.Direction = Int16.Parse(lvGene.TrainName.Substring(1));
                        lvGene.Speed = 0.0;

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

                        lvSegment = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, 1, out lvIndex);

                        if (lvSegment != null)
                        {
                            lvGene.SegmentInstance = lvSegment;
                            lvGene.Track = 1;

                            lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);

                            if (lvCurrentStopSegment == null)
                            {
                                lvCurrentStopSegment = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                            }
                            lvGene.StopLocation = lvCurrentStopSegment;
                        }
                        else
                        {
                            DebugLog.Logar("Não tem segment !", pIndet: TrainIndividual.IDLog);
                        }

                        if (mPriority.Keys.Count > 0)
                        {
                            lvKey = lvGene.TrainName.Substring(0, 1) + lvGene.Direction;
                            if (mPriority.ContainsKey(lvKey))
                            {
                                lvGene.ValueWeight = mPriority[lvKey];
                            }
                        }

                        if (mTrainAllowed.Count == 0)
                        {
                            mPlanList.Add(lvGene);
#if DEBUG
                            DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Stop Location: " + lvGene.StopLocation.Location + ") - End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ")", false, pIndet: TrainIndividual.IDLog);
#endif
                        }
                        else if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)))
                        {
                            mPlanList.Add(lvGene);
#if DEBUG
                            DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Stop Location: " + lvGene.StopLocation.Location + ") - End: " + lvGene.End + " (End " + lvGene.EndStopLocation + ")", false, pIndet: TrainIndividual.IDLog);
#endif
                        }

                        LoadPATs(lvGene.TrainId);
                    }
                }
            }

#if DEBUG
            DebugLog.Logar("Total = " + mPlanList.Count, false, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" --------------------------------------------------------------------------------------------- ", false, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public static void LoadPriority(string pStrPriority)
    {
        string[] lvVarElement;
        string lvStrTrainType = "";
        int lvDirection = 0;
        double lvValue = 0;
        string lvKey;
        string[] lvVarPriority;

        if (pStrPriority.Length > 0)
        {
            lvVarPriority = pStrPriority.Split('|');

            foreach (string lvPriority in lvVarPriority)
            {
                lvVarElement = lvPriority.Split(':');

                if (lvVarElement.Length >= 3)
                {
                    lvStrTrainType = lvVarElement[0];
                    lvDirection = Int32.Parse(lvVarElement[1]);
                    lvValue = Convert.ToDouble(lvVarElement[2]);

                    lvKey = lvStrTrainType + lvDirection;

                    if (!mPriority.ContainsKey(lvKey))
                    {
                        mPriority.Add(lvKey, lvValue);
                    }
                }
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

    public IIndividual<Gene> GetBestIndividual()
    {
        IIndividual<Gene> lvRes = null;

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

    public IIndividual<Gene> GetIndividualAt(int pIndex)
    {
        IIndividual<Gene> lvRes = null;

        if (mIndividuals != null)
        {
            if ((pIndex >= 0) && (pIndex < mIndividuals.Count))
            {
                lvRes = mIndividuals[pIndex];
            }
        }

        return lvRes;
    }

    private void GenerateChildren(IIndividual<Gene> pFather, IIndividual<Gene> pMother)
    {
        int lvRandomValue = mRandom.Next(1, 101);
        IIndividual<Gene> lvSon = null;
        IIndividual<Gene> lvDaughter = null;
        IIndividual<Gene> lvMutated = null;

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

    private bool UpdateOffSpring2(List<IIndividual<Gene>> pIndividuals)
    {
        bool lvRes = true;
        IIndividual<Gene> lvFather = null;
        IIndividual<Gene> lvMother = null;
        List<IIndividual<Gene>> lvSelectedIndividuals = new List<IIndividual<Gene>>();
        bool lvValid = false;

        try
        {
            for (int ind = 0; ind < pIndividuals.Count / 2; ind++)
            {
                if (mLSStrategy == LS_STRATEGY_ENUM.PTH)
                {
                    PTHSelection(pIndividuals, out lvFather, out lvMother);
                    lvValid = true;
                }
                else
                {
                    if (mSelectionMode == SELECTION_ENUM.Roulette)
                    {
                        lvValid = RouletteWheelSelection(pIndividuals, out lvFather, out lvMother);
                    }
                    else if (mSelectionMode == SELECTION_ENUM.Tournament)
                    {
                        lvValid = TournamentSelection(pIndividuals, mSelectionModeCount, mRandom, out lvFather, out lvMother);
                    }
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
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: TrainIndividual.IDLog);
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
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: TrainIndividual.IDLog);
#endif
                            break;
                        }
                    }
                }
            }

#if DEBUG
            mStopWatch.Stop();

            DebugLog.Logar("mStopWatch for GenerateChildren = " + mStopWatch.Elapsed, false, pIndet: TrainIndividual.IDLog);
#endif

            if (mMaxObjectiveFunctionCall > 0)
            {
                if (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall)
                {
                    lvRes = false;
#if DEBUG
                    DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: TrainIndividual.IDLog);
#endif
                    return lvRes;
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    private bool UpdateOffSpring(List<IIndividual<Gene>> pIndividuals)
	{
        bool lvRes = true;
        int lvRandomValue = mRandom.Next(1, 101);
        IIndividual<Gene> lvFather = null;
		IIndividual<Gene> lvMother = null;
		IIndividual<Gene> lvSon = null;
		IIndividual<Gene> lvDaughter = null;
        IIndividual<Gene> lvMutated = null;
        IIndividual<Gene> lvClusterCenter = null;
        Cluster lvCluster = null;
        bool lvValid = false;
        bool lvRejected = false;

        try
        {
            for (int ind = 0; ind < pIndividuals.Count / 2; ind++)
            {
                if (mLSStrategy == LS_STRATEGY_ENUM.PTH)
                {
                    PTHSelection(pIndividuals, out lvFather, out lvMother);
                    lvValid = true;
                }
                else
                {
                    if (mSelectionMode == SELECTION_ENUM.Roulette)
                    {
                        lvValid = RouletteWheelSelection(pIndividuals, out lvFather, out lvMother);
                    }
                    else if (mSelectionMode == SELECTION_ENUM.Tournament)
                    {
                        lvValid = TournamentSelection(pIndividuals, mSelectionModeCount, mRandom, out lvFather, out lvMother);
                    }
                }

                //DebugLog.Logar("UpdateOffSpring.lvFather.VerifyConflict() = " + ((TrainIndividual)lvFather).VerifyConflict(), pIndet: TrainIndividual.IDLog);
                //DebugLog.Logar("UpdateOffSpring.lvMother.VerifyConflict() = " + ((TrainIndividual)lvMother).VerifyConflict(), pIndet: TrainIndividual.IDLog);

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
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: TrainIndividual.IDLog);
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
                            DebugLog.Logar("UpdateOffSpring => Fitness Called Num = " + mFitness.FitnessCallNum, false, pIndet: TrainIndividual.IDLog);
#endif
                            return lvRes;
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }
	
    private IIndividual<Gene> HillClimbingSearch(IIndividual<Gene> pIndividual)
    {
        IIndividual<Gene> lvRes = pIndividual;
        IIndividual<Gene> lvIndividual = null;
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
                    IIndividual<Gene> lvThreadIndividual = Mutate(lvRes, lvThreadMutationSteps, false);

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
                    DebugLog.Logar(lvStrRes.ToString(), false, pIndet: TrainIndividual.IDLog);
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
                    DebugLog.Logar(lvStrRes.ToString(), false, pIndet: TrainIndividual.IDLog);
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

    private bool PTHTraining(IIndividual<Gene> pIndividual)
    {
        bool lvRes = true;
        double lvTrainId = 0.0;
        StringBuilder lvStrInfo = new StringBuilder();
        IIndividual<Gene> lvIndividual = null;
        IIndividual<Gene> lvBestIndividual = pIndividual;

        for (int lvNeighborInd = 0; lvNeighborInd < mLSNeighbors; lvNeighborInd++)
        {
            lvTrainId = mFitness.GetMostDelayedStochastic(pIndividual);
            lvIndividual = HasteGene(pIndividual, lvTrainId);
            lvIndividual.GetFitness();

            if(lvIndividual.Fitness < lvBestIndividual.Fitness)
            {
                lvBestIndividual = lvIndividual;
            }

            if ((mMaxObjectiveFunctionCall > 0) && (mFitness.FitnessCallNum >= mMaxObjectiveFunctionCall))
            {
                lvRes = false;
                break;
            }
        }

        pIndividual.RefFitnessValue = lvBestIndividual.Fitness;
        //pIndividual.RefFitnessValue = ((lvBestIndividual.Fitness - lvIndividual.GBest) / lvIndividual.GBest) + (lvIndividual.Fitness - lvBestIndividual.Fitness);
        /*
        DebugLog.Logar(lvStrInfo.ToString(), false);
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(((TrainIndividual)lvBestIndividual).ToStringAnalyse());
        }
        */

        return lvRes;
    }

    public void LocalSearchAll()
    {
        IIndividual<Gene> lvIndividual = null;

        for(int i = 0; i < mIndividuals.Count; i++)
        {
            lvIndividual = HillClimbingSearch(mIndividuals[i]);
            mIndividuals[i] = lvIndividual;
        }
    }

    public void RLNS(int pCount = 0)
    {
        IIndividual<Gene> lvBestIndividual = null;
        IIndividual<Gene> lvCandidateIndividual = null;
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
        IIndividual<Gene> lvCandidateIndividual = null;
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
        double lvGMax = double.MinValue;
        IIndividual<Gene> lvCurrentIndividual = null;
        Cluster lvCluster = null;

        if (mIndividuals.Count == 0)
        {
            return false;
        }

        if(mRandom == null)
        {
            mRandom = new Random();
        }

        if (mLSStrategy == LS_STRATEGY_ENUM.PTH)
        {
            foreach (IIndividual<Gene> lvIndividual in mIndividuals)
            {
                lvRes = PTHTraining(lvIndividual);
                if (lvIndividual.RefFitnessValue > lvGMax)
                {
                    lvGMax = lvIndividual.RefFitnessValue;
                }

                if(!lvRes)
                {
                    return false;
                }
            }

            /* fazer o calculo para depois ordenar */
            foreach (IIndividual<Gene> lvIndividual in mIndividuals)
            {
                /* alterar o valor de d para encontrar melhores resultados */
                lvIndividual.RefFitnessValue = ((lvGMax - lvIndividual.RefFitnessValue) / lvGMax) - (lvIndividual.Fitness - lvIndividual.RefFitnessValue);
            }

            PTHSorter lvComparer = new PTHSorter();
            mIndividuals.Sort(lvComparer);
            mIndividuals.Reverse();
        }

        mCurrentGeneration++;

        lvRes = UpdateOffSpring2(mIndividuals);
        //lvRes = UpdateOffSpring(new List<IIndividual<Gene>>(mIndividuals));

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

                    DebugLog.Logar(" ", false);
                    DebugLog.Logar(" ---------------------------- Clusters Values for generation " + mCurrentGeneration + "(Min: " + mECSMinValue + " - Max: " + mECSMaxValue + ") ------------------------------------ ", false);
#endif

                    if (!mECSOnlyHeated)
                    {
                        for (int i = 0; i < mECSClusterEliteValue; i++)
                        {
                            lvCluster = mClusters[i];
#if DEBUG
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Tentando Local Search: " + lvCluster.Center.Fitness + " ...", false);
#endif
                            lvCurrentIndividual = HillClimbingSearch(lvCluster.Center);
                            if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < lvCluster.Center.Fitness))
                            {
#if DEBUG
                                DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Local Search: " + lvCluster.Center.Fitness + " -> " + lvCurrentIndividual.Fitness, false);
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
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Tentando Local Search: " + lvCluster.Center.Fitness + " ...", false);
#endif
                            lvCurrentIndividual = HillClimbingSearch(lvCluster.Center);

                            if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < lvCluster.Center.Fitness))
                            {
#if DEBUG
                                DebugLog.Logar("Cluster " + lvCluster.UniqueId + " = " + lvCluster.Value + " | Local Search: " + lvCluster.Center.Fitness + " -> " + lvCurrentIndividual.Fitness, false);
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
                            DebugLog.Logar("Cluster " + lvCluster.UniqueId + " eliminado = " + lvCluster.Value, false);
#endif
                            lvCluster.Reset();
                            mECSClusterCentersSet.Remove(lvCluster.Center.GetUniqueId());
                            mClusters.RemoveAt(i);
                        }
#if DEBUG
                        DebugLog.Logar("Cluster " + lvCluster.UniqueId + " (Fitness: " + lvCluster.Center.Fitness + " - Radius: " + lvCluster.Radius + ") = " + lvCluster.Value, false);
#endif
                    }

#if DEBUG
                    /* Verificando o valor de aquecimento de cada Cluster */
                    DebugLog.Logar("Total Clusters = " + mClusters.Count + "; Fitness Called Num = " + mFitness.FitnessCallNum, false);
                    DebugLog.Logar(" --------------------------------------------------------------------------------------------------------------------------- ", false);

                    mStopWatch.Stop();

                    DebugLog.Logar("mStopWatch for Local Search in Clusters = " + mStopWatch.Elapsed, false);
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
                    DebugLog.Logar("mClustersCenter.Count => " + mClustersCenter.Count, false, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
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
                    DebugLog.Logar("Individual " + mIndividuals[0].GetUniqueId() + " | Tentando Local Search: " + mIndividuals[0].Fitness + " ...", false);
#endif
                    lvCurrentIndividual = HillClimbingSearch(mIndividuals[0]);
                    if ((lvCurrentIndividual != null) && (lvCurrentIndividual.Fitness < mIndividuals[0].Fitness))
                    {
#if DEBUG
                        DebugLog.Logar("Individual " + mIndividuals[0].GetUniqueId() + " => " + lvCurrentIndividual.GetUniqueId() + " | Local Search: " + mIndividuals[0].Fitness + " -> " + lvCurrentIndividual.Fitness, false);
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
                        IIndividual<Gene> lvCurrIndividual = HillClimbingSearch(mIndividuals[i]);
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
            DebugLog.Logar("Best Fitness = " + GetBestIndividual().Fitness, false);
            DebugLog.Logar(" ", false);
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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    private void PTHSelection(List<IIndividual<Gene>> pIndividuals, out IIndividual<Gene> pFather, out IIndividual<Gene> pMother)
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

    private bool TournamentSelection(List<IIndividual<Gene>> pIndividuals, int pCount, Random pRandom, out IIndividual<Gene> pFather, out IIndividual<Gene> pMother)
    {
        bool lvRes = true;
        int lvFatherIndex = -1;
        int lvMotherIndex = -1;
        int lvindElite = (int)(mSize * ELITE_PERC);
        IIndividual<Gene> lvFather = null;
        IIndividual<Gene> lvMother = null;

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
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvRes;
    }

    private bool RouletteWheelSelection(List<IIndividual<Gene>> pIndividuals, out IIndividual<Gene> pFather, out IIndividual<Gene> pMother)
    {
    	double lvTotalFitness = 0.0;
    	double lvRandomValue1;
    	double lvRandomValue2;
    	double lvTotal = 0.0;
        IIndividual<Gene> lvIndividual = null;
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

    private void AddFromQueue(IIndividual<Gene> pIndividual, Queue<Gene> pQueue, HashSet<int> pHashRef = null)
    {
        Gene lvGene = null;
        bool lvRes;
        int lvQueueCount = pQueue.Count;
        DateTime lvTimeLine = DateTime.MaxValue;

        for (int i = 0; i < lvQueueCount; i++)
        {
            lvGene = pQueue.Dequeue();
            lvRes = pIndividual.ProcessGene(lvGene, lvTimeLine);
            if (lvRes)
            {
                if (pHashRef != null)
                {
                    pHashRef.Add(pIndividual[pIndividual.Count-2].GetID());
                }
            }
            else
            {
                pQueue.Enqueue(lvGene);
            }
        }
    }

    private void DoCrossOver(IIndividual<Gene> pFather, IIndividual<Gene> pMother, out IIndividual<Gene> pSon, out IIndividual<Gene> pDaughter, double pFatherRate = 0.0)
    {
        int lvCount = 0;
        bool lvProcessGeneResult;
        List<int> lvCrossOverPos = null;
        int lvIndividualSize = pFather.Count - 1;
        int lvCurrValue = -1;
        int lvCrossOverPointsNum = 0;
        int lvFatherRateValue = 0;
        Gene lvGeneFather = null;
        Gene lvGeneMother = null;
        Queue<Gene> lvQueueSon = new Queue<Gene>();
        Queue<Gene> lvQueueDaughter = new Queue<Gene>();
        List<Gene> lvInitialSon = null;
        List<Gene> lvInitialDaughter = null;
        HashSet<int> lvCrossOverPointsCheck = new HashSet<int>();
        HashSet<int> lvHashSetSon = new HashSet<int>();
        HashSet<int> lvHashSetDaughter = new HashSet<int>();
        HashSet<double> lvHashSetSonNotAllowed = new HashSet<double>();
        HashSet<double> lvHashSetDaughterNotAllowed = new HashSet<double>();
        DateTime lvTimeLine = DateTime.MaxValue;
        pSon = null;
        pDaughter = null;

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
            lvCrossOverPos.Sort();

            lvInitialSon = pFather.GetGenes(mTrainList.Count, lvCrossOverPos[0]);
            pSon = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
            pSon.AddGenes(lvInitialSon);
            foreach (Gene lvGen in lvInitialSon)
            {
                if (lvGen.State == Gene.STATE.OUT)
                {
                    lvHashSetSon.Add(lvGen.GetID());
                }
            }
            //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: TrainIndividual.IDLog);

            lvInitialDaughter = pMother.GetGenes(mTrainList.Count, lvCrossOverPos[0]);
            pDaughter = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
            pDaughter.AddGenes(lvInitialDaughter);

            foreach (Gene lvGen in lvInitialDaughter)
            {
                if (lvGen.State == Gene.STATE.OUT)
                {
                    lvHashSetDaughter.Add(lvGen.GetID());
                }
            }
            //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: TrainIndividual.IDLog);

            for (int ind = 1; ind <= lvCrossOverPointsNum; ind++)
            {
                for (int i = 0; i < lvCount; i++)
                {
                    lvGeneFather = pFather[i];
                    lvGeneMother = pMother[i];

                    /* Son */
                    if ((pSon != null) && (lvGeneMother != null) && ((ind % 2) != 0) && (((ind < lvCrossOverPos.Count) && (pSon.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if ((lvGeneMother.State == Gene.STATE.OUT) && !lvHashSetSon.Contains(lvGeneMother.GetID()))
                        {
                            if (lvQueueSon.Count > 0)
                            {
                                AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                            }

                            if ((lvGeneMother.StopLocation != null) && !lvHashSetSonNotAllowed.Contains(lvGeneMother.TrainId) && (lvGeneMother.StopLocation.Location != lvGeneMother.StartStopLocation.Location))
                            {
                                pSon.AddGeneRef(lvGeneMother);
                            }

                            lvProcessGeneResult = pSon.ProcessGene(lvGeneMother, lvTimeLine);
                            if (lvProcessGeneResult)
                            {
                                lvHashSetSon.Add(pSon[pSon.Count-2].GetID());
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
                                lvQueueSon.Enqueue(lvGeneMother);
                                if (!lvHashSetSonNotAllowed.Contains(lvGeneMother.TrainId))
                                {
                                    lvHashSetSonNotAllowed.Add(lvGeneMother.TrainId);
                                }
                                
#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pSon).hasInDic(lvGeneMother.TrainId) && (!lvHashSetSonNotAllowed.Contains(lvGeneMother.TrainId)))
                                    {
                                        ((TrainIndividual)pSon).DumpCurrentPosDic("pSon", 0);
                                        ((TrainIndividual)pSon).DumpCurrentState("pSon", 0);
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvGeneMother, pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: TrainIndividual.IDLog);
                                        ((TrainIndividual)pSon).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueSon.Peek() = " + lvQueueSon.Peek(), pIndet: TrainIndividual.IDLog);

                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pFather: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pMother: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                                        //                                    lvGenes = pSon.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueSon.Count > mMaxDeadLockError)
                                {
                                    pSon = null;
                                    DebugLog.Logar("lvQueueSon.Count > Limite", pIndet: TrainIndividual.IDLog);
                                }
                            }
                        }
                    }
                    else if ((pSon != null) && (lvGeneFather != null) && ((ind % 2) == 0) && (((ind < lvCrossOverPos.Count) && (pSon.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if ((lvGeneFather.State == Gene.STATE.OUT) && !lvHashSetSon.Contains(lvGeneFather.GetID()))
                        {
                            if (lvQueueSon.Count > 0)
                            {
                                AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                            }

                            if ((lvGeneFather.StopLocation != null) && !lvHashSetSonNotAllowed.Contains(lvGeneFather.TrainId) && (lvGeneFather.StopLocation.Location != lvGeneFather.StartStopLocation.Location))
                            {
                                pSon.AddGeneRef(lvGeneFather);
                            }

                            lvProcessGeneResult = pSon.ProcessGene(lvGeneFather, lvTimeLine);
                            if (lvProcessGeneResult)
                            {
                                lvHashSetSon.Add(pSon[pSon.Count-2].GetID());
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
                                lvQueueSon.Enqueue(lvGeneFather);
                                if (!lvHashSetSonNotAllowed.Contains(lvGeneFather.TrainId))
                                {
                                    lvHashSetSonNotAllowed.Add(lvGeneFather.TrainId);
                                }

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pSon).hasInDic(lvGeneFather.TrainId) && (!lvHashSetSonNotAllowed.Contains(lvGeneFather.TrainId)))
                                    {
                                        ((TrainIndividual)pSon).DumpCurrentPosDic("pSon", 0);
                                        ((TrainIndividual)pSon).DumpCurrentState("pSon", 0);
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvGeneFather, pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("lvQueueSon.Count = " + lvQueueSon.Count, pIndet: TrainIndividual.IDLog);
                                        ((TrainIndividual)pSon).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueSon.Peek() = " + lvQueueSon.Peek(), pIndet: TrainIndividual.IDLog);

                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pFather: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pMother: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                                        //                                    lvGenes = pSon.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueSon.Count > mMaxDeadLockError)
                                {
                                    pSon = null;
                                    DebugLog.Logar("lvQueueSon.Count > Limite", pIndet: TrainIndividual.IDLog);
                                }
                            }
                        }
                    }

                    /* Daughter */
                    if ((pDaughter != null) && (lvGeneFather != null) && ((ind % 2) != 0) && (((ind < lvCrossOverPos.Count) && (pDaughter.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if ((lvGeneFather.State == Gene.STATE.OUT) && !lvHashSetDaughter.Contains(lvGeneFather.GetID()))
                        {
                            if (lvQueueDaughter.Count > 0)
                            {
                                AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                            }

                            if ((lvGeneFather.StopLocation != null) && !lvHashSetDaughterNotAllowed.Contains(lvGeneFather.TrainId) && (lvGeneFather.StopLocation.Location != lvGeneFather.StartStopLocation.Location))
                            {
                                pDaughter.AddGeneRef(lvGeneFather);
                            }

                            lvProcessGeneResult = pDaughter.ProcessGene(lvGeneFather, lvTimeLine);
                            if (lvProcessGeneResult)
                            {
                                lvHashSetDaughter.Add(pDaughter[pDaughter.Count-2].GetID());
                                    //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
                                lvQueueDaughter.Enqueue(lvGeneFather);
                                if (!lvHashSetDaughterNotAllowed.Contains(lvGeneFather.TrainId))
                                {
                                    lvHashSetDaughterNotAllowed.Add(lvGeneFather.TrainId);
                                }

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pDaughter).hasInDic(lvGeneFather.TrainId) && (!lvHashSetDaughterNotAllowed.Contains(lvGeneFather.TrainId)))
                                    {
                                        ((TrainIndividual)pDaughter).DumpCurrentPosDic("pDaughter", 0);
                                        ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter", 0);
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvGeneFather, pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: TrainIndividual.IDLog);
                                        ((TrainIndividual)pDaughter).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueDaughter.Peek() = " + lvQueueDaughter.Peek(), pIndet: TrainIndividual.IDLog);

                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pFather: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pMother: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                                        //                                    lvGenes = pDaughter.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueDaughter.Count > mMaxDeadLockError)
                                {
                                    pDaughter = null;
                                    DebugLog.Logar("lvQueueDaughter.Count > Limite", pIndet: TrainIndividual.IDLog);
                                }
                            }
                        }
                    }
                    else if ((pDaughter != null) && (lvGeneMother != null) && ((ind % 2) == 0) && (((ind < lvCrossOverPos.Count) && (pDaughter.Count < lvCrossOverPos[ind])) || (ind >= lvCrossOverPos.Count)))
                    {
                        if ((lvGeneMother.State == Gene.STATE.OUT) && !lvHashSetDaughter.Contains(lvGeneMother.GetID()))
                        {
                            if (lvQueueDaughter.Count > 0)
                            {
                                AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                            }

                            if ((lvGeneMother.StopLocation != null) && !lvHashSetDaughterNotAllowed.Contains(lvGeneMother.TrainId) && (lvGeneMother.StopLocation.Location != lvGeneMother.StartStopLocation.Location))
                            {
                                pDaughter.AddGeneRef(lvGeneMother);
                            }

                            lvProcessGeneResult = pDaughter.ProcessGene(lvGeneMother, lvTimeLine);
                            if (lvProcessGeneResult)
                            {
                                lvHashSetDaughter.Add(pDaughter[pDaughter.Count-2].GetID());
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                            else
                            {
                                lvQueueDaughter.Enqueue(lvGeneMother);
                                if (!lvHashSetDaughterNotAllowed.Contains(lvGeneMother.TrainId))
                                {
                                    lvHashSetDaughterNotAllowed.Add(lvGeneMother.TrainId);
                                }

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    if (!((TrainIndividual)pDaughter).hasInDic(lvGeneMother.TrainId) && (!lvHashSetDaughterNotAllowed.Contains(lvGeneMother.TrainId)))
                                    {
                                        ((TrainIndividual)pDaughter).DumpCurrentPosDic("pDaughter", 0);
                                        ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter", 0);
                                        DebugLog.Logar("Erro ao tentar adicoinar Gene em crossover:" + lvGeneMother, pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("lvQueueDaughter.Count = " + lvQueueDaughter.Count, pIndet: TrainIndividual.IDLog);
                                        ((TrainIndividual)pDaughter).DumpStopLocation(null);
                                        DebugLog.Logar("lvQueueDaughter.Peek() = " + lvQueueDaughter.Peek(), pIndet: TrainIndividual.IDLog);

                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pFather: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pFather).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar("pMother: \n", pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(((TrainIndividual)pMother).ToStringAnalyse(), pIndet: TrainIndividual.IDLog);
                                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                                        //                                    lvGenes = pDaughter.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                                    }
                                }
#endif

                                if (lvQueueDaughter.Count > mMaxDeadLockError)
                                {
                                    pDaughter = null;
                                    DebugLog.Logar("lvQueueDaughter.Count > Limite", pIndet: TrainIndividual.IDLog);
                                }
                            }
                        }
                    }

                    if ((ind < lvCrossOverPos.Count) && ((pSon == null) || (pSon.Count >= lvCrossOverPos[ind])) && ((pDaughter == null) || (pDaughter.Count >= lvCrossOverPos[ind])))
                    {
                        break;
                    }
                }

                //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: TrainIndividual.IDLog);
                //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: TrainIndividual.IDLog);
            }

            if (pSon != null)
            {
                if (lvQueueSon.Count > 0)
                {
                    AddFromQueue(pSon, lvQueueSon, lvHashSetSon);
                }

                if (lvQueueSon.Count > 0)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pSon) => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                    ((TrainIndividual)pSon).DumpCurrentState("pSon");
                    ((TrainIndividual)pSon).DumpStopLocation(null);
#endif
                    pSon.Clear();
                    pSon = null;
                }
                else if (pSon.Count != lvCount)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pSon) => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                    ((TrainIndividual)pSon).DumpCurrentState("pSon");
                    ((TrainIndividual)pSon).DumpStopLocation(null);
#endif
                    pSon.Clear();
                    pSon = null;
                }
            }

            if (pDaughter != null)
            {
                if (lvQueueDaughter.Count > 0)
                {
                    AddFromQueue(pDaughter, lvQueueDaughter, lvHashSetDaughter);
                }

                if (lvQueueDaughter.Count > 0)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pDaughter) => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                    ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                    ((TrainIndividual)pDaughter).DumpStopLocation(null);
#endif
                    pDaughter.Clear();
                    pDaughter = null;
                }
                else if (pDaughter.Count != lvCount)
                {
                    DebugLog.Logar("DoCrossOver DeadLock Found (pDaughter) => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                    ((TrainIndividual)pDaughter).DumpCurrentState("pDaughter");
                    ((TrainIndividual)pDaughter).DumpStopLocation(null);
                    ((TrainIndividual)pDaughter).Dump(0L, null);
#endif
                    pDaughter.Clear();
                    pDaughter = null;
                }
            }

            //DebugLog.Logar("DoCrossOver.pSon.VerifyConflict() = " + ((TrainIndividual)pSon).VerifyConflict(), pIndet: TrainIndividual.IDLog);
            //DebugLog.Logar("DoCrossOver.pDaughter.VerifyConflict() = " + ((TrainIndividual)pDaughter).VerifyConflict(), pIndet: TrainIndividual.IDLog);
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    private IIndividual<Gene> HasteGene(IIndividual<Gene> pIndividual, double pTrainId)
    {
        IIndividual<Gene> lvRes = pIndividual;
        IIndividual<Gene> lvIndividual = null;
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

    private IIndividual<Gene> Mutate(IIndividual<Gene> pIndividual, int pSteps, bool pUpdate = true)
    {
        bool lvRes;
        int lvNewPosition = -1;
        int lvInitialPos = -1;
        IIndividual<Gene> lvMutatedIndividual = null;
        List<int> lvRefPos = new List<int>();
        Queue<Gene> lvQueue = new Queue<Gene>();
        Gene lvRefGene = null;
        Gene lvGene = null;
        HashSet<double> lvHashSetNotAllowed = new HashSet<double>();
        HashSet<Gene> lvHashRefGene = new HashSet<Gene>();
        DateTime lvTimeLine = DateTime.MaxValue;
        int lvRefIndex = -1;
        int lvPrevGene = -1;
        int lvEndIndex = -1;

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ------------------------ Mutate(" + pIndividual.GetUniqueId() + ", " + pSteps + ", " + pUpdate + ") -------------------------- ");
        }
#endif

        try
        {
            if ((pSteps > 0) && (pIndividual != null) && (pIndividual.GetUniqueId() >= 0))
            {
                lvInitialPos = mRandom.Next(mTrainList.Count + 1 + (int)(pIndividual.Count * 0.2), (int)(pIndividual.Count * 0.9));

                lvGene = pIndividual[lvInitialPos];
                if (lvGene.State == Gene.STATE.IN)
                {
                    lvInitialPos--;
                }

                //DebugLog.Logar("lvInitialPos = " + lvInitialPos);

                lvRefGene = pIndividual[lvInitialPos];

                if((lvRefGene.StopLocation == null) || (lvRefGene.StopLocation.Location == lvRefGene.StartStopLocation.Location))
                {
                    return lvMutatedIndividual;
                }

                if (pSteps > 1)
                {
                    for (int i = lvInitialPos+1; i < pIndividual.Count; i++)
                    {
                        lvGene = pIndividual[i];
                        if ((lvGene.TrainId == lvRefGene.TrainId) && (lvGene.State == Gene.STATE.OUT))
                        {
                            lvRefPos.Add(i);
                        }
                    }
                }

                lvPrevGene = -1;
                for (int i = lvInitialPos - 2; i >= 0; i--)
                {
                    lvGene = pIndividual[i];

                    if ((lvGene.TrainId == lvRefGene.TrainId) && (lvGene.State == Gene.STATE.OUT))
                    {
                        lvPrevGene = i + 1;
                        break;
                    }
                }

                if(lvPrevGene < 0)
                {
                    return lvMutatedIndividual;
                }

                if ((lvPrevGene + 1) < lvInitialPos)
                {
                    if (lvPrevGene + 1 > mTrainList.Count)
                    {
                        lvNewPosition = mRandom.Next(lvPrevGene + 1, lvInitialPos);
                    }
                    else
                    {
                        lvNewPosition = mRandom.Next(mTrainList.Count + 1, lvInitialPos);
                    }
                    lvGene = pIndividual[lvNewPosition];

                    if(lvGene.State == Gene.STATE.IN)
                    {
                        lvNewPosition--;
                    }
                }
                else
                {
                    return null;
                }

                if ((lvRefGene.TrainId != lvGene.TrainId) && ((mTrainList.Count + 1) < lvInitialPos))
                {
                    lvMutatedIndividual = new TrainIndividual(mFitness, mDateRef, mTrainList, mPATs, mRandom);
                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);
                    lvEndIndex = lvNewPosition - 1;

                    if (lvEndIndex < 0)
                    {
                        lvEndIndex = 0;
                    }

                    //DebugLog.Logar("lvEndIndex = " + lvEndIndex);

                    lvMutatedIndividual.AddGenes(pIndividual.GetGenes(mTrainList.Count, lvEndIndex));

                    //DebugLog.Logar("lvMutatedIndividual.Count = " + lvMutatedIndividual.Count);

                    /* Insere o Gene na nova posição */
                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    //DebugLog.Logar("lvRefGene = " + lvRefGene);

                    if ((lvRefGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvRefGene.TrainId) && (lvRefGene.StopLocation.Location != lvRefGene.StartStopLocation.Location))
                    {
                        lvMutatedIndividual.AddGeneRef(lvRefGene);
                    }
                    //DebugLog.Logar("lvMutatedIndividual.Count = " + lvMutatedIndividual.Count);

                    /* insere o RefGene correspondente */
                    lvGene = pIndividual[lvInitialPos];

                    lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine, pUpdate);
                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);

                    if (!lvRes)
                    {
                        lvQueue.Enqueue(lvRefGene);
                        if (!lvHashSetNotAllowed.Contains(lvRefGene.TrainId))
                        {
                            lvHashSetNotAllowed.Add(lvRefGene.TrainId);
                        }
                    }
                    lvHashRefGene.Add(lvGene);

                    if (lvRefPos.Count > 0)
                    {
                        /* Insere os genes restantes */
                        for (int i = 0; i < pSteps; i++)
                        {
                            lvInitialPos = lvNewPosition;
                            lvEndIndex = lvRefPos[++lvRefIndex];
                            /* Procurar novo Gene com mesmo trainId de RefGene para estabelecer os limites */
                            lvNewPosition = mRandom.Next(lvInitialPos, lvEndIndex);
                            lvGene = pIndividual[lvNewPosition];

                            if (lvGene.State == Gene.STATE.IN)
                            {
                                lvNewPosition--;
                            }

                            /* Insere o Gene igual a RefGene no intervalo aleatório */
                            for (int ind = lvInitialPos; ind < lvNewPosition; ind++)
                            {
                                lvGene = pIndividual[ind];
                                if ((lvGene.TrainId != lvRefGene.TrainId) && (lvGene.State == Gene.STATE.OUT))
                                {
                                    if (lvQueue.Count > 0)
                                    {
                                        AddFromQueue(lvMutatedIndividual, lvQueue);
                                    }

                                    if ((lvGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvGene.TrainId) && (lvGene.StopLocation.Location != lvGene.StartStopLocation.Location))
                                    {
                                        lvMutatedIndividual.AddGeneRef(lvGene);
                                    }

                                    lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine, pUpdate);
                                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);

                                    if (!lvRes)
                                    {
                                        lvQueue.Enqueue(lvGene);
                                        if (!lvHashSetNotAllowed.Contains(lvGene.TrainId))
                                        {
                                            lvHashSetNotAllowed.Add(lvGene.TrainId);
                                        }
                                    }
                                }
                            }

                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }

                            /* insere o RefGene correspondente */
                            lvGene = pIndividual[lvEndIndex];

                            lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine, pUpdate);
                            //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);

                            if (!lvRes)
                            {
                                lvQueue.Enqueue(lvRefGene);
                                if (!lvHashSetNotAllowed.Contains(lvRefGene.TrainId))
                                {
                                    lvHashSetNotAllowed.Add(lvRefGene.TrainId);
                                }
                            }
                            lvHashRefGene.Add(lvGene);

                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }

                            if (lvRefIndex >= (lvRefPos.Count - 1))
                            {
                                break;
                            }
                        }
                    }

                    /* preenche com o restante do indivíduo */
                    for (int ind = lvNewPosition; ind < pIndividual.Count; ind++)
                    {
                        lvGene = pIndividual[ind];
                        if ((lvGene.State == Gene.STATE.OUT) && (!lvHashRefGene.Contains(lvGene)))
                        {
                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }

                            if ((lvGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvGene.TrainId) && (lvGene.StopLocation.Location != lvGene.StartStopLocation.Location))
                            {
                                lvMutatedIndividual.AddGeneRef(lvGene);
                            }

                            lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine, pUpdate);
                            //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);

                            if (!lvRes)
                            {
                                lvQueue.Enqueue(lvGene);
                                if (!lvHashSetNotAllowed.Contains(lvGene.TrainId))
                                {
                                    lvHashSetNotAllowed.Add(lvGene.TrainId);
                                }
                            }
                        }
                    }

                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    if (lvMutatedIndividual.Count != pIndividual.Count)
                    {
                        DebugLog.Logar("Mutate DeadLock Found !!! => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                        ((TrainIndividual)pIndividual).Dump(0, null);
                        ((TrainIndividual)lvMutatedIndividual).Dump(0, null);
#endif
                        lvMutatedIndividual.Clear();
                        lvMutatedIndividual = null;
                    }

                    if ((lvQueue.Count > 0) && (lvMutatedIndividual != null))
                    {
                        DebugLog.Logar("Mutate DeadLock Found !!! => ", false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                        ((TrainIndividual)pIndividual).Dump(0, null);
                        ((TrainIndividual)lvMutatedIndividual).Dump(0, null);
#endif
                        lvMutatedIndividual.Clear();
                        lvMutatedIndividual = null;
                    }

                    //DebugLog.Logar("Mutate.lvMutatedIndividual.VerifyConflict() = " + ((TrainIndividual)lvMutatedIndividual).VerifyConflict(), pIndet: TrainIndividual.IDLog);
                }
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" --------------------------------------------------------------------------------------------------------------- ");
            }
#endif

        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        return lvMutatedIndividual;
    }

    private IIndividual<Gene> Mutate(IIndividual<Gene> pIndividual, int pPos, int pDirection, bool pForce = false)
    {
        bool lvRes;
        int lvRandomValue = mRandom.Next(1, 101);
        int lvNewPosition = -1;
        IIndividual<Gene> lvMutatedIndividual = null;
        Queue<Gene> lvQueue = new Queue<Gene>();
        Gene lvRefGene = null;
        Gene lvGene = null;
        HashSet<double> lvHashSetNotAllowed = new HashSet<double>();
        DateTime lvTimeLine = DateTime.MaxValue;
        int lvPrevGene = -1;
        int lvNextGene = -1;
        int lvEndIndex = -1;

        try
        {
            if (pForce || (lvRandomValue <= mMutationRate))
            {
                if ((pPos >= 2) && (pPos < pIndividual.Count))
                {
                    lvRandomValue = pPos;
                    lvRefGene = pIndividual[pPos];
                }
                else
                {
                    lvRandomValue = mRandom.Next(0, pIndividual.Count - 1);
                    lvRefGene = pIndividual[lvRandomValue];
                    while (lvRefGene.State == Gene.STATE.IN)
                    {
                        lvRandomValue++;
                        lvRefGene = pIndividual[lvRandomValue];

                        if (lvRefGene == null)
                        {
                            lvRandomValue = pIndividual.Count - 1;
                        }
                    }
                }

                if (pDirection > 0)
                {
                    lvPrevGene = pPos + 1;
                    lvGene = pIndividual[lvPrevGene];
                    if (lvGene.TrainId == lvRefGene.TrainId)
                    {
                        lvPrevGene++;
                    }
                }
                else
                {
                    lvPrevGene = 0;
                    for (int i = lvRandomValue - 1; i >= 0; i--)
                    {
                        lvGene = pIndividual[i];

                        if ((lvGene.TrainId == lvRefGene.TrainId) && (lvRefGene.State == Gene.STATE.OUT))
                        {
                            lvPrevGene = i + 1;
                            break;
                        }
                    }
                }

                if (pDirection < 0)
                {
                    lvNextGene = lvRandomValue - 1;
                    lvGene = pIndividual[lvNextGene];
                    if (lvGene.TrainId == lvRefGene.TrainId)
                    {
                        lvNextGene--;
                    }
                }
                else
                {
                    lvNextGene = pIndividual.Count - 1;
                    for (int i = lvRandomValue + 2; i < pIndividual.Count; i++)
                    {
                        lvGene = pIndividual[i];

                        if ((lvGene.TrainId == lvRefGene.TrainId) && (lvRefGene.State == Gene.STATE.OUT))
                        {
                            lvNextGene = i - 1;
                            break;
                        }
                    }
                }

                if (lvPrevGene < lvNextGene)
                {
                    lvNewPosition = mRandom.Next(lvPrevGene, lvNextGene + 1);
                    lvGene = pIndividual[lvNewPosition];
                }
                else
                {
                    return null;
                }

                if (lvRefGene.TrainId != lvGene.TrainId)
                {
                    lvMutatedIndividual = new TrainIndividual(mFitness, mDateRef, null, mPATs, mRandom);
                    if (lvNewPosition < lvRandomValue)
                    {
                        lvEndIndex = lvNewPosition - 1;
                    }
                    else
                    {
                        lvEndIndex = lvRandomValue - 1;
                    }

                    if(lvEndIndex < 0)
                    {
                        lvEndIndex = 0;
                    }

                    lvMutatedIndividual.AddGenes(pIndividual.GetGenes(0, lvEndIndex));

                    for (int i = lvRandomValue + 1; i < lvNewPosition; i++)
                    {
                        lvGene = pIndividual[i];
                        if (lvGene.State == Gene.STATE.OUT)
                        {
                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }

                            if ((lvGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvGene.TrainId) && (lvGene.StopLocation.Location != lvGene.StartStopLocation.Location))
                            {
                                lvMutatedIndividual.AddGeneRef(lvGene);
                            }

                            lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine);
                            if (!lvRes)
                            {
                                lvQueue.Enqueue(lvGene);
                                if (!lvHashSetNotAllowed.Contains(lvGene.TrainId))
                                {
                                    lvHashSetNotAllowed.Add(lvGene.TrainId);
                                }
                            }
                        }
                    }

                    /* Insere o Gene na nova posição */
                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    if ((lvRefGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvRefGene.TrainId) && (lvRefGene.StopLocation.Location != lvRefGene.StartStopLocation.Location))
                    {
                        lvMutatedIndividual.AddGeneRef(lvRefGene);
                    }

                    lvRes = lvMutatedIndividual.ProcessGene(lvRefGene, lvTimeLine);
                    if (!lvRes)
                    {
                        lvQueue.Enqueue(lvRefGene);
                        if (!lvHashSetNotAllowed.Contains(lvRefGene.TrainId))
                        {
                            lvHashSetNotAllowed.Add(lvRefGene.TrainId);
                        }
                    }

                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    for (int i = lvNewPosition; i < pIndividual.Count; i++)
                    {
                        lvGene = pIndividual[i];
                        if ((lvGene.State == Gene.STATE.OUT) && (lvGene != lvRefGene))
                        {
                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvMutatedIndividual, lvQueue);
                            }

                            if ((lvGene.StopLocation != null) && !lvHashSetNotAllowed.Contains(lvGene.TrainId) && (lvGene.StopLocation.Location != lvGene.StartStopLocation.Location))
                            {
                                lvMutatedIndividual.AddGeneRef(lvGene);
                            }

                            lvRes = lvMutatedIndividual.ProcessGene(lvGene, lvTimeLine);
                            if (!lvRes)
                            {
                                lvQueue.Enqueue(lvGene);
                                if (!lvHashSetNotAllowed.Contains(lvGene.TrainId))
                                {
                                    lvHashSetNotAllowed.Add(lvGene.TrainId);
                                }
                            }
                        }
                    }

                    if (lvQueue.Count > 0)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue);
                    }

                    if (lvMutatedIndividual.Count != pIndividual.Count)
                    {
                        DebugLog.Logar("Mutate DeadLock Found !!! => ", false, pIndet: TrainIndividual.IDLog);
                        lvMutatedIndividual.Clear();
                        lvMutatedIndividual = null;
                    }

                    if (lvQueue.Count > 0)
                    {
                        DebugLog.Logar("Mutate DeadLock Found !!! => ", false, pIndet: TrainIndividual.IDLog);
                        lvMutatedIndividual.Clear();
                        lvMutatedIndividual = null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

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

    public void Dump(IIndividual<Gene> pBestIndividual)
    {
        int lvInd = 0;
        StringBuilder lvResText;

        DebugLog.Logar(" ");
        DebugLog.Logar(" -------------------------- dump Individuals --------------------------- ");
        DebugLog.Logar(" ");
        foreach (IIndividual<Gene> lvIndividual in mIndividuals)
        {
            lvResText = new StringBuilder("Individual ");
            lvResText.Append(lvIndividual.GetUniqueId());
            lvResText.Append(" | ");
            lvResText.Append(lvIndividual.Fitness);
            lvResText.Append(" | Distancia: ");
            lvResText.Append(pBestIndividual.GetDistanceFrom(lvIndividual));
            lvResText.Append(" | Tamanho: ");
            lvResText.Append(lvIndividual.Count);

            DebugLog.Logar(lvResText.ToString(), false);

            if(lvInd == 0)
            {
                DebugLog.Logar(pBestIndividual.Fitness.ToString(), "log_result_" + mSeed, false);
            }
            lvInd++;
        }
        DebugLog.Logar(" ");
        DebugLog.Logar("Count = " + mIndividuals.Count);
        DebugLog.Logar(" ----------------------------------------------------------------------  ");
    }

    public static void dump(IIndividual<Gene> pBestIndividual, IIndividual<Gene> pBestCandidateIndividual, string pStr = "")
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