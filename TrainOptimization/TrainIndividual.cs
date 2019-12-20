using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for TrainIndividual
/// </summary>
public class TrainIndividual : IIndividual<TrainMovement>, IComparable<IIndividual<TrainMovement>>, IEnumerable<TrainMovement>
{
    /* 
    * Gene[] em mStopLocationDeparture e mStopLocationArrival deve ser usada a formula
    * (Linha - 1) * 2 + Cod_Lado
    * Onde Lado Esquerdo = 0 e Direito = 1
    */
    private IFitness<TrainMovement> mFitness;
    private Dictionary<int, HashSet<Int64>> mStopLocationOcupation = null;
    private Dictionary<int, ISet<Gene>> mStopLocationDeparture = null;
    private Dictionary<int, ISet<Gene>> mStopLocationArrival = null;
    private Dictionary<Int64, TrainMovement> mDicTrain = null;
    private Dictionary<Int64, List<Trainpat>> mPATs = null;
    private Dictionary<int, int> mDicDistRef = null;
    private HashSet<Int64> mTrainFinished = null;
    private List<int> mForeignIndividual = null;
    private int mUniqueId = -1;
    private static int mIDLog = 0;
    private List<TrainMovement> mList;
    private DateTime mDateRef;
    private double mGBest = ConnectionManager.DOUBLE_REF_VALUE * (-1);
    private double mMinSpeedLimit = 15.0;
    private double mSpeedReductionFactor = 0.70;
    private static bool mAllowInertia = true;
    private double mCurrentFitness = ConnectionManager.DOUBLE_REF_VALUE * (-1);
    private double mBestFitness = double.MaxValue;
    private bool mCalcDistRef = false;
    private Random mRandom = null;
    private DescendingGeneTimeComparar mDescGeneTimeComparer;
    private static int mTrainLen = 350000;
    private static double mTrainLenKM = 3.5;
    private static int mLimitDays = 3;
    private static double mVMA = 80.0;
    private static int mStrategyFactor = 1;

    public TrainIndividual(List<TrainMovement> pGenes, int pUniqueId, Dictionary<int, int> pDicDistRef, IFitness<TrainMovement> pFitness, DateTime pDateRef, Dictionary<Int64, List<Trainpat>> pPATs, Random pRandom) : this(pFitness, pDateRef, pGenes, pPATs, pRandom)
    {
        mUniqueId = pUniqueId;
        mDicDistRef = pDicDistRef;
    }

    public TrainIndividual(IFitness<TrainMovement> pFitness, DateTime pDateRef, List<TrainMovement> pTrainList, Dictionary<Int64, List<Trainpat>> pPATs, Random pRandom)
    {
        mFitness = pFitness;
        mList = new List<TrainMovement>();
        mDicTrain = new Dictionary<Int64, TrainMovement>();
        mTrainFinished = new HashSet<Int64>();

        mUniqueId = RuntimeHelpers.GetHashCode(this);

        if(pRandom == null)
        {
            mRandom = new Random();
        }
        else
        {
            mRandom = pRandom;
        }

        mDateRef = pDateRef;

        if (!Double.TryParse(ConfigurationManager.AppSettings["MIN_MOV_SPEED_LIMIT"], out mMinSpeedLimit))
        {
            mMinSpeedLimit = 0.0;
        }

        if (!Double.TryParse(ConfigurationManager.AppSettings["REDUCTION_SPEED_FACTOR"], out mSpeedReductionFactor))
        {
            mSpeedReductionFactor = 0.0;
        }

        if (!bool.TryParse(ConfigurationManager.AppSettings["ECS"], out mCalcDistRef))
        {
            mCalcDistRef = false;
        }

        try
        {
            mPATs = pPATs;
            mDescGeneTimeComparer = new DescendingGeneTimeComparar();

            mStopLocationOcupation = new Dictionary<int, HashSet<Int64>>();
            foreach (StopLocation lvStopLocation in StopLocation.GetList())
            {
                mStopLocationOcupation.Add(lvStopLocation.Location, new HashSet<Int64>());
            }

            mStopLocationDeparture = new Dictionary<int, ISet<Gene>>();
            mStopLocationArrival = new Dictionary<int, ISet<Gene>>();
            foreach (StopLocation lvStopLocation in StopLocation.GetList())
            {
                mStopLocationDeparture.Add(lvStopLocation.Location, new SortedSet<Gene>(mDescGeneTimeComparer));
                mStopLocationArrival.Add(lvStopLocation.Location, new SortedSet<Gene>(mDescGeneTimeComparer));
            }

            if (mCalcDistRef)
            {
                mDicDistRef = new Dictionary<int, int>();
            }

            AddElements(pTrainList);
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public IIndividual<TrainMovement> Clone()
    {
        IIndividual<TrainMovement> lvRes = new TrainIndividual(mList, mUniqueId, mDicDistRef, mFitness, mDateRef, mPATs, mRandom);

        return lvRes;
    }

    public void Clear()
    {
        if (mList != null)
        {
            mList.Clear();
            mList = null;
        }

        if (mStopLocationOcupation != null)
        {
            mStopLocationOcupation.Clear();
            mStopLocationOcupation = null;
        }

        if (mStopLocationDeparture != null)
        {
            mStopLocationDeparture.Clear();
            mStopLocationDeparture = null;
        }

        if (mStopLocationArrival != null)
        {
            mStopLocationArrival.Clear();
            mStopLocationArrival = null;
        }

        if (mDicTrain != null)
        {
            mDicTrain.Clear();
        }

        if(mTrainFinished != null)
        {
            mTrainFinished.Clear();
        }

        if (mDicDistRef != null)
        {
            mDicDistRef.Clear();
        }

        mUniqueId = -1;
        mCurrentFitness = ConnectionManager.DOUBLE_REF_VALUE * (-1);
    }

    public Dictionary<int, HashSet<Int64>> GetStopLocationOcupation()
    {
        return mStopLocationOcupation;
    }

    public Dictionary<int, ISet<Gene>> GetStopLocationDeparture()
    {
        return mStopLocationDeparture;
    }

    public Dictionary<int, ISet<Gene>> GetStopLocationArrival()
    {
        return mStopLocationArrival;
    }

    private bool IsCloseToCross(Gene pGene)
    {
        bool lvRes = false;
        Segment lvNextSwitch = null;
        StopLocation lvStopLocation = null;

        if (pGene.StopLocation != null)
        {
            lvNextSwitch = Segment.GetNextSwitchSegment(pGene.StopLocation.Location, pGene.Direction);

            if (pGene.Direction > 0)
            {
                lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.End_coordinate + 100, pGene.Direction * (-1));
            }
            else
            {
                lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.Start_coordinate - 100, pGene.Direction * (-1));
            }

            if (pGene.StopLocation.Location == lvStopLocation.Location)
            {
                lvRes = true;
            }
        }

        return lvRes;
    }

    public IEnumerable<TrainMovement> GetElements(int pStartIndex, int pEndIndex)
    {
        int lvIndex = pEndIndex;

        if (pStartIndex < 0 || pEndIndex >= mList.Count)
        {
            return new List<TrainMovement>();
        }

        return mList.GetRange(pStartIndex, (lvIndex - pStartIndex) + 1);
    }

    public IEnumerator<TrainMovement> GetEnumerator()
    {
        return mList.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void AddElements(IEnumerable<TrainMovement> pElements, bool pNeedUpdate = true)
    {
        Gene lvObjGene;
        ISet<Gene> lvGenesStopLocationSet = null;
        HashSet<Int64> lvListGeneStopLocation = null;

        try
        {
            if (pElements != null)
            {
                if (pNeedUpdate)
                {
                    foreach (TrainMovement lvTrainMovement in pElements)
                    {
                        foreach(Gene lvGene in lvTrainMovement)
                        { 
                            if (lvGene.State == Gene.STATE.OUT)
                            {
                                if (lvGene.StopLocation != null)
                                {
                                    lvGenesStopLocationSet = mStopLocationDeparture[lvGene.StopLocation.Location];
                                    if (lvGene.Track <= lvGene.StopLocation.Capacity)
                                    {
                                        /* 
                                        * Gene[] em mStopLocationDeparture e mStopLocationArrival deve ser usada a formula
                                        * (Linha - 1) * 2 + Cod_Lado
                                        * Onde Lado Esquerdo = 0 e Direito = 1
                                        */

                                        //lvGenesStopLocation[(lvGene.Track - 1) * 2 + Math.Max(0, (int)lvGene.Direction)] = lvGene;
                                        lvGenesStopLocationSet.Add(lvGene);
                                    }
                                }
                            }
                            else if (lvGene.State == Gene.STATE.IN)
                            {
                                if (lvGene.StopLocation != null)
                                {
                                    lvGenesStopLocationSet = mStopLocationArrival[lvGene.StopLocation.Location];
                                    if (lvGene.Track <= lvGene.StopLocation.Capacity)
                                    {
                                        lvGenesStopLocationSet.Add(lvGene);
                                    }
                                }
                            }
                            else
                            {
                                DebugLog.Logar("Problema em AddElements", false, pIndet: TrainIndividual.IDLog);
                            }
                        }

                        if ((lvTrainMovement.Last.StopLocation != null) && (lvTrainMovement.Last.StopLocation.Location == lvTrainMovement.Last.EndStopLocation.Location))
                        {
                            mDicTrain.Remove(lvTrainMovement.Last.TrainId);
                            mTrainFinished.Add(lvTrainMovement.Last.TrainId);

                            if (lvTrainMovement.Last.StopLocation != null)
                            {
                                lvListGeneStopLocation = mStopLocationOcupation[lvTrainMovement.Last.StopLocation.Location];
                                lvListGeneStopLocation.Remove(lvTrainMovement.Last.TrainId);
                            }

                            if (lvTrainMovement.Last.StopLocation.DwellTimeOnEndStopLocation > 0)
                            {
                                lvObjGene = lvTrainMovement.Last.Clone();
                                lvObjGene.State = Gene.STATE.OUT;

                                lvGenesStopLocationSet = mStopLocationDeparture[lvObjGene.StopLocation.Location];
                                if (lvObjGene.Track <= lvObjGene.StopLocation.Capacity)
                                {
                                    lvObjGene.Time = lvObjGene.Time.AddSeconds(lvObjGene.StopLocation.DwellTimeOnEndStopLocation);
                                    //lvGenesStopLocation[(lvObjGene.Track - 1) * 2 + Math.Max(0, (int)lvObjGene.Direction)] = lvObjGene;
                                    lvGenesStopLocationSet.Add(lvObjGene);
                                }
                            }
                        }
                        else if (!mDicTrain.ContainsKey(lvTrainMovement.Last.TrainId))
                        {
                            if (lvTrainMovement[0].Speed == 0.0)
                            {
                                lvTrainMovement[0].Speed = mMinSpeedLimit;
                            }

                            if (lvTrainMovement[0].HeadWayTime <= lvTrainMovement[0].Time)
                            {
                                lvTrainMovement[0].HeadWayTime = lvTrainMovement[0].Time.AddHours(mTrainLenKM / lvTrainMovement[0].Speed);
                            }

                            mDicTrain.Add(lvTrainMovement.Last.TrainId, lvTrainMovement);
                        }
                        else
                        {
                            mDicTrain[lvTrainMovement.Last.TrainId] = lvTrainMovement;
                        }

                        if ((lvTrainMovement[0].StopLocation != null) && (lvTrainMovement.Count > 1))
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[lvTrainMovement[0].StopLocation.Location];
                            lvListGeneStopLocation.Remove(lvTrainMovement[0].TrainId);

                            lvListGeneStopLocation = mStopLocationOcupation[lvTrainMovement.Last.StopLocation.Location];
                            lvListGeneStopLocation.Add(lvTrainMovement.Last.TrainId);
                        }

                        if (mCalcDistRef)
                        {
                            if (!mDicDistRef.ContainsKey(lvTrainMovement.GetID()))
                            {
                                mDicDistRef.Add(lvTrainMovement.GetID(), mList.Count-1);
                            }
                        }

                        mList.Add(lvTrainMovement);
                    }
                }
                else
                {
                    foreach (TrainMovement lvTrainMovement in pElements)
                    {
                        mList.Add(lvTrainMovement);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public int GetDistanceFrom(IIndividual<TrainMovement> pIndividual)
    {
        int lvRes = -1;
        int lvPositionID;
        int lvValue = -1;
        TrainMovement lvTrainMovement;

        if (mCalcDistRef)
        {
            try
            {
                lock (mDicDistRef)
                {
                    if (mDicDistRef.Count < pIndividual.Count)
                    {
                        LoadDistRef();
                    }

                    if ((mForeignIndividual == null) || (mForeignIndividual.Count < pIndividual.Count))
                    {
                        mForeignIndividual = new List<int>(new int[pIndividual.Count]);
                    }

                    for (int i = 0; i < pIndividual.Count; i++)
                    {
                        lvTrainMovement = pIndividual[i];

                        lvPositionID = lvTrainMovement.GetID();

                        if (mDicDistRef.ContainsKey(lvPositionID))
                        {
                            //lvForeignIndividual[i] = mDicDistRef[lvPositionID];
                            if (!mDicDistRef.TryGetValue(lvPositionID, out lvValue))
                            {
                                lvValue = -1;
                            }
                            mForeignIndividual[i] = lvValue;
                        }
#if DEBUG
                        else
                        {
                            DebugLog.Logar("mDicDistRef.ContainsKey(lvPositionID) Nao Encontrado ", pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }

                    lvRes = pIndividual.Count - GetLIS(mForeignIndividual);
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            }
        }

        return lvRes;
    }

    private int GetLIS(List<int> pElements)
    {
        int lvIndex;
        List<int> lvLISArray = new List<int>();

        foreach (int lvValue in pElements)
        {
            lvIndex = lvLISArray.BinarySearch(lvValue);

            if (lvIndex < 0)
            {
                lvIndex = ~lvIndex;
            }

            if (lvIndex != lvLISArray.Count)
            {
                lvLISArray[lvIndex] = lvValue;
            }
            else
            {
                lvLISArray.Add(lvValue);
            }
        }

        return lvLISArray.Count;
    }

    public int Count
    {
        get
        {
            return mList.Count;
        }
    }

    public double Fitness
    {
        get
        {
            if((mCurrentFitness == (ConnectionManager.DOUBLE_REF_VALUE * (-1))) && (mList != null) && (mList.Count > 0))
            {
                GetFitness();
            }

            return mCurrentFitness;
        }
        set
        {
            mCurrentFitness = value;
        }
    }

    public double GetFitness()
    {
        if (mFitness != null)
        {
            mCurrentFitness = mFitness.GetFitness(this);
        }

        return mCurrentFitness;
    }

    public int CompareTo(IIndividual<TrainMovement> pOther)
    {
        int lvRes = 0;

        if (pOther == null) return -1;

        if (this == null) return 1;

        if (pOther.Fitness == this.Fitness)
        {
            lvRes = 0;
        }
        else if (this.Fitness > pOther.Fitness)
        {
            lvRes = 1;
        }
        else if (this.Fitness < pOther.Fitness)
        {
            lvRes = -1;
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return mUniqueId;
        }
    }

    private void DumpNewMov(List<Gene> pGenes)
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (Gene lvGene in pGenes)
        {
            if (lvRes.Length > 0)
            {
                lvRes.Append(" => ");
                lvRes.Append(lvGene);
            }
            else
            {
                lvRes.Append(lvGene);
            }
        }

        DebugLog.Logar("NewGenes = " + lvRes.ToString(), pIndet: TrainIndividual.IDLog);
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (TrainMovement lvTrainMovement in mList)
        {
            if (lvRes.Length > 0)
            {
                lvRes.Append(" ;\n ");
            }
            lvRes.Append(lvTrainMovement);
        }

        return lvRes.ToString();
    }

    public string ToString(Int64 pTrainId)
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (TrainMovement lvTrainMovement in mList)
        {
            if ((lvTrainMovement[0] != null) && (pTrainId == lvTrainMovement[0].TrainId))
            {
                if (lvRes.Length > 0)
                {
                    lvRes.Append(" ; ");
                }
                lvRes.Append(lvTrainMovement);
            }
        }

        return lvRes.ToString();
    }

    public string GetFlotSeries()
    {
        Dictionary<double, StringBuilder> lvDicSeries = new Dictionary<double, StringBuilder>();
        StringBuilder lvStrSerie = null;
        StringBuilder lvRes = new StringBuilder();
        DateTime lvCurrentDate = DateTime.MinValue;
        string lvStrTrainLabel = "";
        string lvXValues = "";
        string lvYValues = "";
        string lvStrColor = "";
        string lvStrReason = "";

        lvCurrentDate = mDateRef;

        foreach (TrainMovement lvTrainMovement in mList)
        {
            foreach (Gene lvGene in lvTrainMovement)
            {
                if (lvDicSeries.ContainsKey(lvGene.TrainId))
                {
                    lvStrSerie = lvDicSeries[lvGene.TrainId];

                    lvXValues = ConnectionManager.GetUTCDateTime(lvGene.HeadWayTime).ToString();
                    if (lvGene.StopLocation != null)
                    {
                        lvYValues = ((double)lvGene.StopLocation.Location / 100000.0).ToString();
                    }
                    else
                    {
                        lvYValues = ((double)lvGene.Coordinate / 100000.0).ToString();
                    }
                    lvXValues = lvXValues.Replace(",", ".");
                    lvYValues = lvYValues.Replace(",", ".");

                    lvStrSerie.Append(", [");
                    lvStrSerie.Append(lvXValues);
                    lvStrSerie.Append(", ");
                    lvStrSerie.Append(lvYValues);
                    lvStrSerie.Append("]");
                    lvStrSerie = null;
                }
                else
                {
                    lvStrSerie = new StringBuilder();
                    lvStrColor = Train.GetColorByTrainType(lvGene.TrainName.Substring(0, 1).ToUpper());
                    lvStrTrainLabel = string.Format("{0} - {1}", lvGene.TrainName, lvGene.TrainId);
                    lvStrSerie.Append("{\"color\": \"");
                    lvStrSerie.Append(lvStrColor);
                    lvStrSerie.Append("\", \"label\": \"");
                    lvStrSerie.Append(lvStrTrainLabel);
                    lvStrSerie.Append("\", \"ident\": \"");
                    lvStrSerie.Append(lvGene.TrainId);
                    lvStrSerie.Append("\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");

                    lvXValues = ConnectionManager.GetUTCDateTime(lvGene.Time).ToString();
                    if (lvGene.StopLocation != null)
                    {
                        lvYValues = ((double)lvGene.StopLocation.Location / 100000.0).ToString();
                    }
                    else
                    {
                        lvYValues = ((double)lvGene.Coordinate / 100000.0).ToString();
                    }
                    lvXValues = lvXValues.Replace(",", ".");
                    lvYValues = lvYValues.Replace(",", ".");

                    lvStrSerie.Append("[");
                    lvStrSerie.Append(ConnectionManager.GetUTCDateTime(lvCurrentDate));
                    lvStrSerie.Append(", ");
                    lvStrSerie.Append(lvYValues);
                    lvStrSerie.Append("]");

                    lvStrSerie.Append(", [");
                    lvStrSerie.Append(lvXValues);
                    lvStrSerie.Append(", ");
                    lvStrSerie.Append(lvYValues);
                    lvStrSerie.Append("]");

                    lvDicSeries.Add(lvGene.TrainId, lvStrSerie);
                    lvStrSerie = null;
                }
            }
        }

        foreach (StringBuilder lvStrTrem in lvDicSeries.Values)
        {
            lvStrTrem.Append("]}");

            if (lvRes.Length > 0)
            {
                lvRes.Append(",");
            }
            lvRes.Append(lvStrTrem.ToString());
        }

        lvStrSerie = new StringBuilder();
        foreach (Interdicao interd in Interdicao.GetList())
        {
            lvStrReason = string.Format("{0} - {1}", interd.Ti_id, interd.Reason);

            /* DebugLog.Logar("lvRestId = " + lvRestId); */

            // restricao
            lvStrColor = "rgba(255, 255, 50, 0.5)";

            if (interd.Field_interdicted == 1)
            {
                lvStrColor = "rgba(255, 0, 0, 0.5)";
            }
            else
            {
                lvStrColor = "rgba(32, 143, 255, 0.5)";
            }

            if (lvRes.Length > 0)
            {
                lvRes.Append(",");
            }

            if (interd.End_time > interd.Start_time)
            {
                lvRes.Append(ConnectionManager.GetFlotSerieBlock(lvStrColor, lvStrReason, interd.Start_time, interd.End_time, ((double)(interd.Start_pos / 100000.0)), ((double)interd.End_pos / 100000.0), "interdicao"));
            }
        }

        lvRes.Insert(0, "[");
        lvRes.Append("]");

        return lvRes.ToString();
    }

    private void RemoveGeneFromAllLocation(Gene pGene)
    {
        bool lvValue = false;
        bool lvRes = false;

        foreach (HashSet<Int64> lvGenes in mStopLocationOcupation.Values)
        {
            lvValue = lvGenes.Remove(pGene.TrainId);

            if (lvValue)
            {
                lvRes = true;
            }
        }

#if DEBUG
        if (DebugLog.EnableDebug && lvRes)
        {
            DebugLog.Logar("RemoveGeneFromAllLocation.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
        }
#endif

    }

    private void DumpDuplicatePosition()
    {
        Dictionary<double, int> lvDicCountDuplicate = new Dictionary<double, int>();

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ----------------------------- DumpDuplicatePosition() ------------------------------- ", pIndet: TrainIndividual.IDLog);
        }

        foreach (KeyValuePair<int, HashSet<Int64>> lvEntry in mStopLocationOcupation)
        {
            foreach (double lvTrainId in lvEntry.Value)
            {
                if (!lvDicCountDuplicate.ContainsKey(lvTrainId))
                {
                    lvDicCountDuplicate.Add(lvTrainId, 1);
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("Trem " + lvTrainId + " inserido na lista de verificação de duplicação !", pIndet: TrainIndividual.IDLog);
                    }
                }
                else
                {
                    lvDicCountDuplicate[lvTrainId]++;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("Trem " + lvTrainId + " duplicado em " + lvEntry.Key, pIndet: TrainIndividual.IDLog);
                    }
                }
            }
        }

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
        }
    }

    public bool IsValid()
    {
        bool lvRes = true;
        Dictionary<double, StopLocation> lvGenesStopLocation = new Dictionary<double, StopLocation>();
        StopLocation lvStopLocation = null;
        StopLocation lvPrevStopLocation = null;

        foreach (TrainMovement lvTrainMov in mList)
        {
            foreach (Gene lvGene in lvTrainMov)
            {
                lvStopLocation = lvGene.StopLocation;

                if (lvStopLocation != null)
                {
                    lvPrevStopLocation = lvStopLocation.GetNextStopSegment(lvGene.Direction * (-1));
                    //lvPrevStopLocation = StopLocation.GetNextStopSegment(lvStopLocation.Location, lvGene.Direction * (-1));
                }
                else
                {
                    lvPrevStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction * (-1));
                }

                if (!lvGenesStopLocation.ContainsKey(lvGene.TrainId))
                {
                    lvGenesStopLocation.Add(lvGene.TrainId, lvStopLocation);
                }
                else
                {
                    if ((lvPrevStopLocation != null) && (lvStopLocation != null))
                    {
                        if (lvPrevStopLocation != lvGenesStopLocation[lvGene.TrainId])
                        {
                            lvRes = false;
                            //DebugLog.Logar("Sequencia de Genes invalida (" + lvGene + "):");
                            //DebugLog.Logar(this.ToString());
                            break;
                        }
                    }

                    lvGenesStopLocation[lvGene.TrainId] = lvStopLocation;
                }
            }
        }

        return lvRes;
    }

    public Gene DumpGene(Gene pGene)
    {
        Gene lvResGene = null;
        double lvTrainId = -1;
        StringBuilder lvRes = new StringBuilder();

        if (DebugLog.EnableDebug)
        {
            if (pGene != null)
            {
                lvTrainId = pGene.TrainId;
            }

            if (lvTrainId == -1)
            {
                foreach (TrainMovement lvTrainMov in mList)
                {
                    foreach(Gene lvGene in lvTrainMov)
                    {
                        if (lvRes.Length > 0)
                        {
                            lvRes.Append(" <= ");
                        }

                        lvRes.Append("(");
                        lvRes.Append(lvGene.TrainId);
                        lvRes.Append(" - " + lvGene.TrainName + ") ");
                        lvRes.Append(lvGene.TrainName);
                        lvRes.Append(") ");

                        if (lvGene.State == Gene.STATE.OUT)
                        {
                            lvRes.Append(lvGene.SegmentInstance.Location);
                            lvRes.Append(".");
                            lvRes.Append(lvGene.SegmentInstance.SegmentValue);
                            lvRes.Append(" (Saida: ");
                            lvRes.Append(lvGene.Time);
                            lvRes.Append(")");
                        }
                        else if (lvGene.State == Gene.STATE.IN)
                        {
                            lvRes.Append(lvGene.SegmentInstance.Location);
                            lvRes.Append(".");
                            lvRes.Append(lvGene.SegmentInstance.SegmentValue);
                            lvRes.Append(" (Chegada: ");
                            lvRes.Append(lvGene.Time);
                            lvRes.Append(")");
                        }
                    }
                }
            }
            else
            {
                foreach (TrainMovement lvTrainMov in mList)
                {
                    foreach (Gene lvGene in lvTrainMov)
                    {
                        if (lvGene.TrainId == lvTrainId)
                        {
                            if (lvRes.Length > 0)
                            {
                                lvRes.Append(" <= ");
                            }

                            if (lvGene.State == Gene.STATE.OUT)
                            {
                                lvRes.Append(lvGene.SegmentInstance.Location);
                                lvRes.Append(".");
                                lvRes.Append(lvGene.SegmentInstance.SegmentValue);
                                lvRes.Append(" (Saida: ");
                                lvRes.Append(lvGene.Time);
                                lvRes.Append(")");
                            }
                            else if (lvGene.State == Gene.STATE.IN)
                            {
                                lvRes.Append(lvGene.SegmentInstance.Location);
                                lvRes.Append(".");
                                lvRes.Append(lvGene.SegmentInstance.SegmentValue);
                                lvRes.Append(" (Chegada: ");
                                lvRes.Append(lvGene.Time);
                                lvRes.Append(")");
                            }

                            lvResGene = lvGene;
                        }
                    }
                }
            }

            if (lvRes.Length > 0)
            {
                if (pGene != null)
                {
                    lvRes.Insert(0, ": ");
                    lvRes.Insert(0, pGene.TrainName);
                    lvRes.Insert(0, " - ");
                    lvRes.Insert(0, pGene.TrainId);
                }
            }

            if (lvRes.Length > 0)
            {
                DebugLog.Logar("Gene " + lvRes.ToString(), pIndet: TrainIndividual.IDLog);
            }
            else
            {
                DebugLog.Logar("O Gene especificado nao esta nesse individuo !", pIndet: TrainIndividual.IDLog);
            }
        }

        return lvResGene;
    }

    public void DumpStopLocationByGene(Gene pGene)
    {
        if (DebugLog.EnableDebug)
        {
            List<StopLocation> lvListStopLocation = null;
            StopLocation lvNextStopLocation = StopLocation.GetNextStopSegment(pGene.Coordinate, pGene.Direction);

            if (lvNextStopLocation != null)
            {
                HashSet<Int64> lvGenes = mStopLocationOcupation[lvNextStopLocation.Location];

                DebugLog.Logar(" ----------------------------  DumpStopLocationByGene(" + pGene.TrainId + " - " + pGene.TrainName + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                lvListStopLocation = StopLocation.GetList();

                foreach (StopLocation lvStopLocation in lvListStopLocation)
                {
                    lvGenes = mStopLocationOcupation[lvStopLocation.Location];
                    foreach (double lvTrainId in lvGenes)
                    {
                        if (lvTrainId == pGene.TrainId)
                        {
                            DebugLog.Logar(lvStopLocation.ToString(), pIndet: TrainIndividual.IDLog);
                        }
                    }
                }

                DebugLog.Logar(" -------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
            }
        }
    }

    private void DumpStopDepLocation(StopLocation pStopLocation)
    {
        ISet<Gene> lvGenes = null;

        if (DebugLog.EnableDebug)
        {

            if (pStopLocation != null)
            {
                DebugLog.Logar(" ----------------------------  DumpStopDepLocation (" + pStopLocation + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);
                if (mStopLocationDeparture.ContainsKey(pStopLocation.Location))
                {
                    lvGenes = mStopLocationDeparture[pStopLocation.Location];
                    foreach (Gene lvGene in lvGenes)
                    {
                        if (lvGene != null)
                        {
                            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.TrainId = " + lvGene.TrainId, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.TrainName = " + lvGene.TrainName, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Location = " + lvGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.UD = " + lvGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Coordinate = " + lvGene.Coordinate, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Track = " + lvGene.Track, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Direction = " + lvGene.Direction, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.End = " + lvGene.End, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Time = " + lvGene.Time, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("pGene.Speed = " + lvGene.Speed, pIndet: TrainIndividual.IDLog);
                        }
                    }
                }

                DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
            }
        }
    }

    public void DumpStopArrivalLocation(StopLocation pStopLocation, int pCount, string pStrInfo = "")
    {
        ISet<Gene> lvGenes = null;
        int lvCount;
        int lvIndex;

        if (DebugLog.EnableDebug)
        {
            if (pStopLocation != null)
            {
                DebugLog.Logar(" ----------------------------  DumpStopArrivalLocation (" + pStopLocation.Location + " -> " + pStrInfo + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);
                if (mStopLocationArrival.ContainsKey(pStopLocation.Location))
                {
                    lvGenes = mStopLocationArrival[pStopLocation.Location];

                    lvIndex = 0;
                    foreach (Gene lvGene in lvGenes)
                    {
                        if (lvIndex >= pCount)
                        {
                            break;
                        }

                        if (lvGene != null)
                        {
                            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Location = " + lvGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.UD = " + lvGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Track = " + lvGene.Track, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Direction = " + lvGene.Direction, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.End = " + lvGene.End, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Time = " + lvGene.Time, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.HeadWayTime = " + lvGene.HeadWayTime, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Speed= " + lvGene.Speed, pIndet: TrainIndividual.IDLog);
                        }

                        lvIndex++;
                    }
                }
            }
            else
            {
                DebugLog.Logar(" ----------------------------  DumpStopArrivalLocation (" + pStrInfo + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);

                lvCount = 0;
                foreach (KeyValuePair<int, ISet<Gene>> lvStopLocationEntry in mStopLocationArrival)
                {
                    if (pCount > -1)
                    {
                        DebugLog.Logar("StopLocation = " + lvStopLocationEntry.Key, pIndet: TrainIndividual.IDLog);
                    }

                    foreach(Gene lvGen in lvStopLocationEntry.Value)
                    {
                        if (pCount > -1)
                        {
                            DebugLog.Logar("\t Gene = " + lvGen.TrainId + " - " + lvGen.TrainName + "; Track: " + lvGen.Track + "; Time: " + lvGen.Time + "; Headway: " + lvGen.HeadWayTime + "; Dir: " + lvGen.Direction, pIndet: TrainIndividual.IDLog);
                        }
                        lvCount++;
                    }

                    if (pCount > -1)
                    {
                        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                    }
                }

                DebugLog.Logar("Total mStopLocationArrival = " + lvCount, pIndet: TrainIndividual.IDLog);

                lvCount = 0;
                foreach (TrainMovement lvTrainMovement in mList)
                {
                    lvCount++;
                }

                DebugLog.Logar("Total mList = " + lvCount, pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public static void Dump(List<Gene> pGenes, Int64 pTrainId)
    {
        int lvCount = 0;

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ----------------------------------  Dump (TrainId:" + pTrainId + ") ------------------------------------------ ", pIndet: TrainIndividual.IDLog);

            foreach (Gene lvGene in pGenes)
            {
                if (pTrainId != 0L)
                {
                    if (pTrainId == lvGene.TrainId)
                    {
                        DebugLog.Logar(lvGene.ToString(), pIndet: TrainIndividual.IDLog);
                        lvCount++;
                    }
                }
                else
                {
                    DebugLog.Logar(lvGene.ToString(), pIndet: TrainIndividual.IDLog);
                    lvCount++;
                }
            }

            DebugLog.Logar("lvCount = " + lvCount, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public void Dump(Int64 pTrainId, StopLocation pEndStopLocation)
    {
        int lvCount = 0;

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ----------------------------  Dump (" + mUniqueId + ":" + pTrainId + ", pEndStopLocation = " + pEndStopLocation + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);

            foreach (TrainMovement lvTrainMov in mList)
            {
                foreach (Gene lvGene in lvTrainMov)
                {
                    if (pTrainId != 0L)
                    {
                        if (pTrainId == lvGene.TrainId)
                        {
                            DebugLog.Logar(lvGene.ToString(), pIndet: TrainIndividual.IDLog);
                            lvCount++;
                        }
                    }
                    else
                    {
                        DebugLog.Logar(lvGene.ToString(), pIndet: TrainIndividual.IDLog);
                        lvCount++;
                    }
                }
            }

            DebugLog.Logar("lvCount = " + lvCount, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public void DumpDifference(IIndividual<TrainMovement> pOtherIndividual)
    {
        int lvCount = 0;
        bool lvFound;

        if (pOtherIndividual == null) return;

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ----------------------------  DumpDifference (this: " + mUniqueId + " to " + pOtherIndividual.GetUniqueId() + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);

            foreach (TrainMovement lvTrainMov in mList)
            {
                lvFound = false;
                foreach (TrainMovement lvOtherMov in pOtherIndividual)
                {
                    if(lvOtherMov == lvTrainMov)
                    {
                        lvFound = true;
                        break;
                    }
                }

                if(!lvFound)
                {
                    lvCount++;
                    DebugLog.Logar(lvTrainMov.ToString(), pIndet: TrainIndividual.IDLog);
                }
            }

            DebugLog.Logar("lvCount = " + lvCount, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public void DumpCurrentState(string pStrInfo)
    {
        Dictionary<Int64, Gene> lvGenePosition = null;

        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ----------------------------  DumpCurrentState (" + pStrInfo + ", mUniqueId = " + mUniqueId + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);
            lvGenePosition = new Dictionary<Int64, Gene>();

            foreach (TrainMovement lvTrainMov in mList)
            {
                foreach (Gene lvGene in lvTrainMov)
                {
                    if (!lvGenePosition.ContainsKey(lvGene.TrainId))
                    {
                        lvGenePosition.Add(lvGene.TrainId, lvGene);
                    }
                    else
                    {
                        lvGenePosition[lvGene.TrainId] = lvGene;
                    }
                }
            }

            foreach (Gene lvGene in lvGenePosition.Values)
            {
                DebugLog.Logar(lvGene.ToString(), pIndet: TrainIndividual.IDLog);
            }

            DebugLog.Logar(lvGenePosition.Count.ToString(), pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public void DumpCurrentPosDic(string pStrInfo)
    {
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ----------------------------  DumpCurrentPosDic (" + pStrInfo + ", mUniqueId = " + mUniqueId + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);

            foreach (TrainMovement lvTrainMov in mDicTrain.Values)
            {
                DebugLog.Logar(lvTrainMov.ToString(), pIndet: TrainIndividual.IDLog);
            }

            DebugLog.Logar(mDicTrain.Count.ToString(), pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    private void DumpNextStopLocation(Gene pGene)
    {
        int lvCoordinate = -1;
        int lvDirectionRef = 0;
        int lvDirection = 0;
        int lvIndex = 0;
        TrainMovement lvTrainMovement;
        Gene lvGene = null;
        Gene lvOtherGene = null;
        Interdicao lvInterdicao = null;

        if (DebugLog.EnableDebug)
        {
            StopLocation lvNextStopLocation = StopLocation.GetNextStopSegment(pGene.Coordinate, pGene.Direction);

            if (lvNextStopLocation == null)
            {
                return;
            }

            HashSet<Int64> lvNextGenes = mStopLocationOcupation[lvNextStopLocation.Location];

            DebugLog.Logar(" ----------------------------  DumpNextStopLocation (Quant Proximos: " + lvNextGenes.Count + ") ------------------------------------ ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.TrainId = " + pGene.TrainId, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.TrainName = " + pGene.TrainName, pIndet: TrainIndividual.IDLog);
            if (pGene.StopLocation != null)
            {
                DebugLog.Logar("pGene.StopLocation.Location = " + pGene.StopLocation.Location, pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar("pGene.Location = " + pGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.UD = " + pGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Coordinate = " + pGene.Coordinate, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Track = " + pGene.Track, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Direction = " + pGene.Direction, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.End = " + pGene.End, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Time = " + pGene.Time, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Speed = " + pGene.Speed, pIndet: TrainIndividual.IDLog);
            lvDirectionRef = pGene.Direction;

            foreach (Int64 lvTrainId in lvNextGenes)
            {
                lvTrainMovement = mDicTrain[lvTrainId];
                lvGene = lvTrainMovement.Last;

                if (lvGene != null)
                {
                    DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName, pIndet: TrainIndividual.IDLog);
                    if (lvGene.StopLocation != null)
                    {
                        DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location, pIndet: TrainIndividual.IDLog);
                    }
                    DebugLog.Logar("lvGene.Location = " + lvGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.UD = " + lvGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.Track = " + lvGene.Track, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.Direction = " + lvGene.Direction, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.End = " + lvGene.End, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.Time = " + lvGene.Time, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvGene.Speed = " + lvGene.Speed, pIndet: TrainIndividual.IDLog);
                    lvDirection += lvGene.Direction;
                }
            }

            lvCoordinate = lvNextStopLocation.Start_coordinate + (lvNextStopLocation.End_coordinate - lvNextStopLocation.Start_coordinate) / 2;
            lvInterdicao = Interdicao.GetCurrentInterdiction(lvCoordinate, out lvIndex);

            if (lvInterdicao != null)
            {
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.Ti_id = " + lvInterdicao.Ti_id, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.Ss_name = " + lvInterdicao.Ss_name, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.Start_pos = " + lvInterdicao.Start_pos, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.End_pos = " + lvInterdicao.End_pos, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.Start_time = " + lvInterdicao.Start_time, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvInterdicao.End_time = " + lvInterdicao.End_time, pIndet: TrainIndividual.IDLog);
            }

            if (lvDirectionRef > 0)
            {
                if (lvDirection < 0)
                {
                    DebugLog.Logar("Dump em StopLocation (" + pGene.StopLocation + ")", pIndet: TrainIndividual.IDLog);
                    if (pGene.StopLocation != null)
                    {
                        DumpStopLocation(pGene.StopLocation);
                    }

                    DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("DeadLock Found:", pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                    for (int i = mList.Count - 1; i >= 0; i--)
                    {
                        lvGene = mList[i][0];
                        if (lvGene.TrainId == pGene.TrainId)
                        {
                            if (lvGene.State == Gene.STATE.OUT)
                            {
                                DebugLog.Logar("Ultimo movimento do Gene que tentou se mover = " + mList[i], pIndet: TrainIndividual.IDLog);
                                break;
                            }
                        }
                    }

                    foreach (Int64 lvTrainId in lvNextGenes)
                    {
                        lvTrainMovement = mDicTrain[lvTrainId];
                        lvGene = lvTrainMovement.Last;

                        if (lvGene != null)
                        {
                            for (int i = mList.Count - 1; i >= 0; i--)
                            {
                                lvOtherGene = mList[i][0];
                                if (lvOtherGene.TrainId == lvGene.TrainId)
                                {
                                    if (lvOtherGene.State == Gene.STATE.OUT)
                                    {
                                        DebugLog.Logar("Ultimo movimento do Gene que estava no local = " + mList[i], pIndet: TrainIndividual.IDLog);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (lvDirection > 0)
                {
                    DebugLog.Logar("Dump em StopLocation (" + pGene.StopLocation + ")", pIndet: TrainIndividual.IDLog);
                    if (pGene.StopLocation != null)
                    {
                        DumpStopLocation(pGene.StopLocation);
                    }

                    DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("DeadLock Found:", pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

                    for (int i = mList.Count - 1; i >= 0; i--)
                    {
                        lvGene = mList[i][0];
                        if (lvGene.TrainId == pGene.TrainId)
                        {
                            if (lvGene.State == Gene.STATE.OUT)
                            {
                                DebugLog.Logar("Ultimo movimento do Gene que tentou se mover = " + mList[i], pIndet: TrainIndividual.IDLog);
                                break;
                            }
                        }
                    }

                    foreach (Int64 lvTrainId in lvNextGenes)
                    {
                        lvTrainMovement = mDicTrain[lvTrainId];
                        lvGene = lvTrainMovement.Last;

                        if (lvGene != null)
                        {
                            for (int i = mList.Count - 1; i >= 0; i--)
                            {
                                lvOtherGene = mList[i][0];
                                if (lvOtherGene.TrainId == lvGene.TrainId)
                                {
                                    if (lvOtherGene.State == Gene.STATE.OUT)
                                    {
                                        DebugLog.Logar("Ultimo movimento do Gene que estava no local = " + mList[i], pIndet: TrainIndividual.IDLog);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            DebugLog.Logar(" -------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
    }

    public bool Save()
    {
        bool lvRes = false;
        double lvPosition = 0.0;

        ConnectionManager.CloseConnection();
        ConnectionManager.HasTransaction = true;

        PlanOptDataAccess.DeleteAll();

        foreach (TrainMovement lvTrainMov in mList)
        {
            foreach (Gene lvGene in lvTrainMov)
            {
                if (lvGene.StopLocation != null)
                {
                    lvPosition = (double)lvGene.StopLocation.Location / 100000.0;
                }
                else
                {
                    lvPosition = (double)lvGene.Coordinate / 100000.0;
                }

                PlanOptDataAccess.Insert(lvGene.TrainId, lvGene.TrainName, ConnectionManager.GetUTCDateTime(lvGene.HeadWayTime), lvPosition, lvGene.Track, SegmentDataAccess.Branch);
            }
        }

        ConnectionManager.CloseConnection();

        return lvRes;
    }

    public void GenerateFlotFiles(string pStrPath)
    {
        string lvStrInfo = GetFlotSeries();
        string lvStrIndividualFileName = "Individual_" + mUniqueId + ".json";
        string lvStrLocationFileName = "Locations_" + mUniqueId + ".json";
        string lvStrXLimitsFileName = "XLimits_" + mUniqueId + ".json";
        StreamWriter lvStreamWriter = null;
        FileStream lvFileStrem;
        DateTime lvInitialDate = DateTime.MaxValue;
        DateTime lvFinalDate = DateTime.MinValue;

        lvFileStrem = new FileStream(pStrPath + lvStrIndividualFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
        lvStreamWriter = new StreamWriter(lvFileStrem);
        lvStreamWriter.WriteLine(lvStrInfo);
        lvStreamWriter.Close();

        lvStrInfo = StopLocation.GetFlotLocations();
        lvFileStrem = new FileStream(pStrPath + lvStrLocationFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
        lvStreamWriter = new StreamWriter(lvFileStrem);
        lvStreamWriter.WriteLine(lvStrInfo);
        lvStreamWriter.Close();

        foreach (TrainMovement lvTrainMov in mList)
        {
            foreach (Gene lvGene in lvTrainMov)
            {
                if (lvGene.Time < lvInitialDate)
                {
                    lvInitialDate = lvGene.Time;
                }

                if (lvGene.Time > lvFinalDate)
                {
                    lvFinalDate = lvGene.Time;
                }
            }
        }

        lvStrInfo = "[" + ConnectionManager.GetUTCDateTime(lvInitialDate) + ", " + ConnectionManager.GetUTCDateTime(lvFinalDate) + ", " + ConnectionManager.GetUTCDateTime(lvFinalDate) + "]";
        lvFileStrem = new FileStream(pStrPath + lvStrXLimitsFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
        lvStreamWriter = new StreamWriter(lvFileStrem);
        lvStreamWriter.WriteLine(lvStrInfo);
        lvStreamWriter.Close();
    }

    /*
    public void BranchAndBound(IIndividual<Gene> pRefIndividual, List<Gene> pTrainList, List<Gene> pPlanList)
    {
        DateTime lvCurrentTime = mDateRef;
        TrainMovement lvRes;
        Gene lvGeneItem = null;
        List<Int64> lvNewPlanIns = new List<Int64>();
        mBestFitness = pRefIndividual.Fitness;
        double lvBestFitness = mBestFitness;
        double lvFitness = 0.0;
        Int64 lvTrainId;
        List<Gene> lvGenes = null;
        List<TrainMovement> lvBestIndividual = ((TrainIndividual)pRefIndividual).GetElements();
        List<Gene> lvTrains = null;
        List<Gene> lvPlans = new List<Gene>(pPlanList);

        mTotalGens = ((mStopLocationOcupation.Count - 1) * (pPlanList.Count) * 2) + (pTrainList.Count * 2);

        for (int i = 0; i < pTrainList.Count; i++)
        {
            mDicTrain.Add(pTrainList[i].TrainId, pTrainList[i]);

            if (pTrainList[i].StopLocation == null)
            {
                MoveTrain(pTrainList[i], lvCurrentTime);
            }
        }

        if(lvPlans.Count > 0)
        {
            mDicTrain.Add(lvPlans[0].TrainId, lvPlans[0]);
            if (lvPlans[0].DepartureTime > lvCurrentTime)
            {
                lvCurrentTime = lvPlans[0].DepartureTime;
            }
            lvPlans.RemoveAt(0);
        }

        if (lvPlans.Count > 0)
        {
            mDicTrain.Add(lvPlans[0].TrainId, lvPlans[0]);
            if (lvPlans[0].DepartureTime > lvCurrentTime)
            {
                lvCurrentTime = lvPlans[0].DepartureTime;
            }
            lvPlans.RemoveAt(0);
        }

        lvTrains = new List<Gene>(mDicTrain.Values);

        foreach (Gene lvGene in lvTrains)
        {
            lvRes = (TrainMovement)MoveTrain(lvGene, lvCurrentTime);

            if (lvRes != null)
            {
                if (lvGene.StopLocation.Location == lvGene.StartStopLocation.Location)
                {
                    for (int i = 0; i < lvPlans.Count; i++)
                    {
                        if (lvGene.Direction == lvPlans[i].Direction)
                        {
                            mDicTrain.Add(lvPlans[i].TrainId, lvPlans[i]);
                            lvNewPlanIns.Add(lvPlans[i].TrainId);
                            if (lvCurrentTime < lvPlans[i].DepartureTime)
                            {
                                lvCurrentTime = lvPlans[i].DepartureTime;
                            }
                            lvPlans.RemoveAt(i);
                            break;
                        }
                    }
                }

                lvGenes = BackTrack(lvPlans, lvCurrentTime);

                if (lvGenes != null)
                {
                    if(lvBestIndividual == null)
                    {
                        lvBestIndividual = lvGenes;
                        lvBestFitness = mBestFitness;
                    }
                    else
                    {
                        lvFitness = mFitness.GetFitness(lvGenes);

                        if(lvFitness < lvBestFitness)
                        {
                            lvBestIndividual = lvGenes;
                            lvBestFitness = mBestFitness;
                        }
                    }
                }

                lvGeneItem = mList[mList.Count - 1];
                if (lvGeneItem.StopLocation != null)
                {
                    mStopLocationOcupation[lvGeneItem.StopLocation.Location].Remove(lvGeneItem.TrainId);
                }
                mList.RemoveAt(mList.Count - 1);

                lvGeneItem = mList[mList.Count - 1];
                if (lvGeneItem.StopLocation != null)
                {
                    if ((lvGeneItem.Direction > 0) && (lvGeneItem.StopLocation.End_coordinate > lvGeneItem.Start))
                    {
                        mStopLocationOcupation[lvGeneItem.StopLocation.Location].Add(lvGeneItem.TrainId);
                    }
                    else if ((lvGeneItem.Direction < 0) && (lvGeneItem.StopLocation.Start_coordinate < lvGeneItem.Start))
                    {
                        mStopLocationOcupation[lvGeneItem.StopLocation.Location].Add(lvGeneItem.TrainId);
                    }

                    if (lvGeneItem.Track <= lvGeneItem.StopLocation.Capacity)
                    {
                        //mStopLocationDeparture[lvGeneItem.StopLocation.Location][(lvGeneItem.Track - 1) * 2 + Math.Max(0, (int)lvGeneItem.Direction)] = null;
//                        mStopLocationArrival[lvGeneItem.StopLocation.Location][(lvGeneItem.Track - 1) * 2 + Math.Max(0, (int)lvGeneItem.Direction)] = null;
                    }
                }
                mList.RemoveAt(mList.Count - 1);

                if (mDicTrain.ContainsKey(lvGene.TrainId))
                {
                    mDicTrain[lvGene.TrainId] = lvGene;
                }
                else
                {
                    mDicTrain.Add(lvGene.TrainId, lvGene);
                }

                for (int i = 0; i < lvNewPlanIns.Count; i++)
                {
                    lvTrainId = lvNewPlanIns[0];

                    if (mDicTrain.ContainsKey(lvTrainId))
                    {
                        lvGeneItem = mDicTrain[lvTrainId];
                        lvPlans.Insert(0, lvGeneItem);
                        mDicTrain.Remove(lvTrainId);
                    }
                    lvNewPlanIns.RemoveAt(0);
                }
            }
        }

        if (lvBestIndividual != null)
        {
            mList = lvBestIndividual;
            GetFitness();
        }
    }

    private List<Gene> BackTrack(List<Gene> pPlanList, DateTime pCurrentTime)
    {
        DateTime lvCurrentTime = pCurrentTime;
        List<Int64> lvNewPlan = new List<Int64>();
        List<Int64> lvNewPlanIns = new List<Int64>();
        Gene lvGeneItem = null;
        bool lvMoveTrainResult;
        List<Gene> lvRes = null;
        List<Gene> lvGenes = null;
        double lvFitness = 0.0;
        Int64 lvTrainId;
        List<Gene> lvNewGenes = null;
        List<Gene> lvPlans = new List<Gene>(pPlanList);
        List<Gene> lvTrains = null;

        if (mDicTrain.Count == 0)
        {
            if (lvPlans.Count == 0)
            {
                if (mList.Count >= mTotalGens)
                {
                    lvRes = new List<Gene>(mList);
                    return lvRes;
                }
            }
            else
            {
                if (lvPlans.Count > 0)
                {
                    mDicTrain.Add(lvPlans[0].TrainId, lvPlans[0]);
                    lvNewPlan.Add(lvPlans[0].TrainId);
                    if (lvCurrentTime < lvPlans[0].DepartureTime)
                    {
                        lvCurrentTime = lvPlans[0].DepartureTime;
                    }
                    lvPlans.RemoveAt(0);
                }

                if (lvPlans.Count > 0)
                {
                    mDicTrain.Add(lvPlans[0].TrainId, lvPlans[0]);
                    lvNewPlan.Add(lvPlans[0].TrainId);
                    if (lvCurrentTime < lvPlans[0].DepartureTime)
                    {
                        lvCurrentTime = lvPlans[0].DepartureTime;
                    }
                    lvPlans.RemoveAt(0);
                }
            }
        }

        lvTrains = new List<Gene>(mDicTrain.Values);

        foreach (Gene lvGene in lvTrains)
        {
            lvMoveTrainResult = MoveTrain(lvGene, lvCurrentTime);

            if (lvMoveTrainResult)
            {
                if (lvGene.StopLocation.Location == lvGene.StartStopLocation.Location)
                {
                    for (int i = 0; i < lvPlans.Count; i++)
                    {
                        if (lvGene.Direction == lvPlans[i].Direction)
                        {
                            mDicTrain.Add(lvPlans[i].TrainId, lvPlans[i]);
                            lvNewPlanIns.Add(lvPlans[i].TrainId);
                            if (lvCurrentTime < lvPlans[i].DepartureTime)
                            {
                                lvCurrentTime = lvPlans[i].DepartureTime;
                            }
                            lvPlans.RemoveAt(i);
                            break;
                        }
                    }
                }

                mList.AddRange(lvNewGenes);

                //[{mList[0].TrainId}, {mList[2].TrainId}, {mList[4].TrainId}, {mList[6].TrainId}, {mList[8].TrainId}, {mList[10].TrainId}, {mList[12].TrainId}, {mList[14].TrainId}, {mList[16].TrainId}, {mList[18].TrainId}, . . ., {mList[mList.Count-3].TrainId}, {mList[mList.Count-2].TrainId}, {mList[mList.Count-1].TrainId}]
                lvFitness = mFitness.GetFitness(mList);

                if (lvFitness < mBestFitness)
                {
                    lvGenes = BackTrack(lvPlans, lvCurrentTime);

                    if (lvGenes != null)
                    {
                        //lvFitness = mFitness.GetFitness(lvGenes);
                        lvRes = lvGenes;

                        if (mList.Count == lvGenes.Count)
                        {
                            mBestFitness = lvFitness;
                        }
                    }
                }

                lvGeneItem = mList[mList.Count - 1];
                if (lvGeneItem.StopLocation != null)
                {
                    mStopLocationOcupation[lvGeneItem.StopLocation.Location].Remove(lvGeneItem.TrainId);
                }
                mList.RemoveAt(mList.Count - 1);

                lvGeneItem = mList[mList.Count - 1];
                if (lvGeneItem.StopLocation != null)
                {
                    if((lvGeneItem.Direction > 0) && (lvGeneItem.StopLocation.Start_coordinate > lvGeneItem.Start))
                    {
                        mStopLocationOcupation[lvGeneItem.StopLocation.Location].Add(lvGeneItem.TrainId);
                    }
                    else if((lvGeneItem.Direction < 0) && (lvGeneItem.StopLocation.End_coordinate < lvGeneItem.Start))
                    {
                        mStopLocationOcupation[lvGeneItem.StopLocation.Location].Add(lvGeneItem.TrainId);
                    }

                    if (lvGeneItem.Track <= lvGeneItem.StopLocation.Capacity)
                    {
                        //mStopLocationDeparture[lvGeneItem.StopLocation.Location][(lvGeneItem.Track - 1) * 2 + Math.Max(0, (int)lvGeneItem.Direction)] = null;
                        //mStopLocationArrival[lvGeneItem.StopLocation.Location][(lvGeneItem.Track - 1) * 2 + Math.Max(0, (int)lvGeneItem.Direction)] = null;
                    }
                }
                mList.RemoveAt(mList.Count - 1);

                if (mDicTrain.ContainsKey(lvGene.TrainId))
                {
                    mDicTrain[lvGene.TrainId] = lvGene;
                }
                else
                {
                    mDicTrain.Add(lvGene.TrainId, lvGene);
                }

                for(int i = 0; i < lvNewPlanIns.Count; i++)
                {
                    lvTrainId = lvNewPlanIns[0];

                    if (mDicTrain.ContainsKey(lvTrainId))
                    {
                        lvGeneItem = mDicTrain[lvTrainId];
                        lvPlans.Insert(0, lvGeneItem);
                        mDicTrain.Remove(lvTrainId);
                    }
                    lvNewPlanIns.RemoveAt(0);
                }
            }
        }

        for (int i = 0; i < lvNewPlanIns.Count; i++)
        {
            lvTrainId = lvNewPlanIns[0];
            mDicTrain.Remove(lvTrainId);
        }

        return lvRes;
    }
*/

    private bool VerifyLoadedData(List<TrainMovement> ploadedList, List<TrainMovement> pMovList, List<TrainMovement> pPlanList, out HashSet<Int64> pLoadedSet, out HashSet<Int64> pPlanSet)
    {
        bool lvRes = false;
        pLoadedSet = new HashSet<Int64>();
        pPlanSet = new HashSet<Int64>();

        if ((ploadedList != null) && (ploadedList.Count > 0))
        {
            lvRes = true;

            foreach (TrainMovement lvTrainMov in ploadedList)
            {
                if ((lvTrainMov.Count > 0) && !pLoadedSet.Contains(lvTrainMov.Last.TrainId))
                {
                    pLoadedSet.Add(lvTrainMov.Last.TrainId);
                }
            }

            foreach (TrainMovement lvTrainMov in pMovList)
            {
                if (!pLoadedSet.Contains(lvTrainMov.Last.TrainId))
                {
                    lvRes = false;
                    break;
                }
            }

            foreach (TrainMovement lvTrainMov in pPlanList)
            {
                if (!pLoadedSet.Contains(lvTrainMov.Last.TrainId))
                {
                    lvRes = false;
                }

                if(!pPlanSet.Contains(lvTrainMov.Last.TrainId))
                {
                    pPlanSet.Add(lvTrainMov.Last.TrainId);
                }
            }
        }

        return lvRes;
    }

    private void AddFromQueue(Queue<TrainMovement> pQueue, DateTime pCurrentTime)
    {
        TrainMovement lvTrainMovement = null;
        IEnumerable<Gene> lvRes;

        for (int i = 0; i < pQueue.Count; i++)
        {
            lvTrainMovement = pQueue.Dequeue();
            lvRes = MoveTrain(lvTrainMovement, pCurrentTime);
            if (lvRes != null)
            {
                pQueue.Enqueue(lvTrainMovement);
            }
        }
    }

    private int LoadedDataComparer(List<TrainMovement> pLoadedData)
    {
        int lvResPos = -1;
        int lvIndex = -1;
        TrainMovement lvLoadedTrainMovement;

        for(int i = 0; i < mList.Count; i++)
        {
            lvLoadedTrainMovement = pLoadedData[i];

            if(mList[i] == lvLoadedTrainMovement)
            {
                lvResPos = lvIndex;
            }
            else
            {
                break;
            }
        }

        return lvResPos;
    }

    public bool GenerateIndividual(IEnumerable<TrainMovement> pPlanList, int pUniqueId = 0, bool pAllowDeadLockIndividual = false)
    {
        bool lvRes = false;
        List<TrainMovement> lvMovTurn = null;
        List<TrainMovement> lvPlans = null;
        List<TrainMovement> lvLoadedData = null;
        HashSet<Int64> lvLoadedSet = null;
        HashSet<Int64> lvPlannedSet = null;
        Queue<TrainMovement> lvQueue = null;
        TrainMovement lvTrainMovement = null;
        TrainMovement lvPlanMovement = null;
        IEnumerable<Gene> lvTrainMovRes = null;
        bool lvLoadedDataStatus = false;
        DateTime lvCurrentTime = DateTime.MaxValue;
        int lvTotalValue = 0;
        int lvTotal = 0;
        int lvRandomValue;
        int lvGeneCount = -1;
        int lvDicTrainCount = -1;
        bool lvLogEnabled = DebugLog.EnableDebug;

        List<TrainMovement> lvRefLoadedData = null;

        // Gerando individuo considerando a hora atual
        try
        {
            lvMovTurn = new List<TrainMovement>(mDicTrain.Values);
            lvPlans = new List<TrainMovement>(pPlanList);

            if (pUniqueId != 0)
            {
                lvLoadedData = UnSerialize(pUniqueId);
                lvLoadedDataStatus = VerifyLoadedData(lvLoadedData, lvMovTurn, lvPlans, out lvLoadedSet, out lvPlannedSet);

                if ((lvLoadedData != null) && (lvLoadedData.Count > 0))
                {
                    lvQueue = new Queue<TrainMovement>();
                }

                lvRefLoadedData = new List<TrainMovement>(lvLoadedData);
            }

            lvCurrentTime = mDateRef;

            /*
            * 0 = Estratégia do mais atrasado tem maior probabilidade (Não Faz Nada)
            * 1 = Estratégia em que todo mundo tem a mesma probabilidade (Value = 1)
            * 2 = Estratégia em que quem está subindo tem a maior probabilidade
            * 3 = Estratégia em que quem está descendo tem a maior probabilidade
            */
            foreach (TrainMovement lvTrainMov in lvMovTurn)
            {
                /* Se não estiver me região de parada tem prioridade */
                if (lvTrainMov.Last.StopLocation == null)
                {
                    lvTrainMovRes = MoveTrain(lvTrainMov, lvCurrentTime);

                    if(lvTrainMovRes == null)
                    {
                        return lvRes;
                    }
                }
                else if((lvTrainMov.Last.StopLocation.NoStopSet != null) && (lvTrainMov.Last.StopLocation.NoStopSet.Count > 0))
                {
                    if(lvTrainMov.Last.StopLocation.HasNoStop(lvTrainMov.Last.TrainName.Substring(0, 1) + lvTrainMov.Last.Direction))
                    {
                        lvTrainMovRes = MoveTrain(lvTrainMov, lvCurrentTime);

                        if (lvTrainMovRes == null)
                        {
                            return lvRes;
                        }
                    }
                }
            }

            lvMovTurn = new List<TrainMovement>(mDicTrain.Values);

            /* Se não tem trem circulando coloca o primeiro trem planejado como circulando */
            if ((mDicTrain.Count == 0) && (lvPlans.Count > 0))
            {
                if (lvCurrentTime <= lvPlans[0].Last.DepartureTime)
                {
                    lvCurrentTime = lvPlans[0].Last.DepartureTime;
                    mDicTrain.Add(lvPlans[0].Last.TrainId, lvPlans[0]);
                    lvMovTurn.Add(lvPlans[0]);
                    lvPlans.RemoveAt(0);
                }
            }

            while ((mDicTrain.Count > 0) || (lvPlans.Count > 0))
            {
                if (lvLoadedDataStatus)
                {
                    lvTrainMovement = lvLoadedData[0];
                    if ((lvTrainMovement.Count > 1) && (lvTrainMovement[0].StopLocation != null))
                    {
                        if (mDicTrain.ContainsKey(lvTrainMovement.Last.TrainId))
                        {
                            lvTrainMovement = mDicTrain[lvTrainMovement.Last.TrainId];
                        }
                        else
                        {
                            if(!lvPlannedSet.Contains(lvTrainMovement.Last.TrainId))
                            {
                                lvTrainMovement = null;
                            }
                        }

                        if (lvTrainMovement != null)
                        {
                            if (lvTrainMovement.Last.Time > lvCurrentTime)
                            {
                                lvCurrentTime = lvTrainMovement.Last.Time;
                            }
                            else if (lvTrainMovement.Last.DepartureTime > lvCurrentTime)
                            {
                                lvCurrentTime = lvTrainMovement.Last.DepartureTime;
                            }

                            if (lvQueue.Count > 0)
                            {
                                AddFromQueue(lvQueue, lvCurrentTime);
                            }

                            lvTrainMovRes = MoveTrain(lvTrainMovement, lvCurrentTime);

                            if (lvTrainMovRes == null)
                            {
                                lvQueue.Enqueue(lvTrainMovement);
                            }
                            else
                            {
                                if (lvPlannedSet.Contains(lvTrainMovement.Last.TrainId))
                                {
                                    lvPlannedSet.Remove(lvTrainMovement.Last.TrainId);

                                    for(int i = 0; i < lvPlans.Count; i++)
                                    {
                                        lvPlanMovement = lvPlans[i];
                                        if(lvPlanMovement.Last.TrainId == lvTrainMovement.Last.TrainId)
                                        {
                                            lvPlans.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                                else if((lvPlannedSet.Count == 0) && (lvPlans.Count > 0))
                                {
                                    lvPlans = new List<TrainMovement>();
                                }
                            }
                        }
                    }

                    lvLoadedData.RemoveAt(0);

                    /* Se não tem trem circulando coloca o primeiro trem planejado como circulando */
                    if ((mDicTrain.Count == 0) && (lvPlans.Count > 0))
                    {
                        if (lvCurrentTime <= lvPlans[0].Last.DepartureTime)
                        {
                            lvCurrentTime = lvPlans[0].Last.DepartureTime;
                            mDicTrain.Add(lvPlans[0].Last.TrainId, lvPlans[0]);
                            lvMovTurn.Add(lvPlans[0]);
                            lvPlans.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    foreach (TrainMovement lvTrainMov in mDicTrain.Values)
                    {
                        if (lvTrainMov.Last.Time > lvCurrentTime)
                        {
                            lvCurrentTime = lvTrainMov.Last.Time;
                        }
                    }

                    lvMovTurn = new List<TrainMovement>(mDicTrain.Values);

                    while (lvMovTurn.Count > 0)
                    {
                        /* Obtem o total para rodar a roleta */
                        lvTotalValue = 0;
                        foreach (TrainMovement lvTrainMov in lvMovTurn)
                        {
                            lvTotalValue += Convert.ToInt32(GetOptTimeToEnd(lvTrainMov.Last.TrainId) * 100);
                            //lvTotalValue += Convert.ToInt32(lvGen.ValueWeight * 100);
                        }
                        lvRandomValue = mRandom.Next(lvTotalValue + 1);

                        lvTotal = 0;
                        for (int i = 0; i < lvMovTurn.Count; i++)
                        {
                            lvTotal += Convert.ToInt32(GetOptTimeToEnd(lvMovTurn[i].Last.TrainId) * 100);
                            //lvTotal += Convert.ToInt32(lvMovTurn[i].ValueWeight * 100);

                            if (lvTotal >= lvRandomValue)
                            {
                                if ((lvLoadedData != null) && (lvLoadedData.Count > 0))
                                {
                                    if(lvLoadedSet.Contains(lvMovTurn[i].Last.TrainId))
                                    {
                                        lvTrainMovement = lvLoadedData[0];

                                        if ((lvTrainMovement.Count > 0) && (lvTrainMovement[0].StopLocation != null))
                                        {
                                            lvTrainMovRes = MoveTrain(lvTrainMovement, lvCurrentTime);

                                            if (lvTrainMovRes != null)
                                            {
                                                if (lvPlannedSet.Contains(lvTrainMovement.Last.TrainId))
                                                {
                                                    lvPlannedSet.Remove(lvTrainMovement.Last.TrainId);

                                                    for (int ind = 0; ind < lvPlans.Count; ind++)
                                                    {
                                                        lvPlanMovement = lvPlans[ind];
                                                        if (lvPlanMovement.Last.TrainId == lvTrainMovement.Last.TrainId)
                                                        {
                                                            lvPlans.RemoveAt(ind);
                                                            break;
                                                        }
                                                    }
                                                }
                                                else if ((lvPlannedSet.Count == 0) && (lvPlans.Count > 0))
                                                {
                                                    lvPlans = new List<TrainMovement>();
                                                }
                                            }
                                            else
                                            {
                                                if(lvPlannedSet.Contains(lvTrainMovement.Last.TrainId))
                                                {
                                                    lvPlannedSet.Remove(lvTrainMovement.Last.TrainId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MoveTrain(lvMovTurn[i], lvCurrentTime);
                                        }

                                        lvLoadedData.RemoveAt(0);
                                    }
                                    else
                                    {
                                        MoveTrain(lvMovTurn[i], lvCurrentTime);
                                    }
                                }
                                else
                                {
                                    MoveTrain(lvMovTurn[i], lvCurrentTime);
                                }
                                lvMovTurn.RemoveAt(i);

                                //DebugLog.Logar("GenerateIndividual().VerifyConflict() = " + VerifyConflict(), pIndet: TrainIndividual.IDLog);

                                break;
                            }
                        }
                    }

                    for (int i = 0; i < lvPlans.Count; i++)
                    {
                        if (!mDicTrain.ContainsKey(lvPlans[i].Last.TrainId))
                        {
                            lvTrainMovRes = MoveTrain(lvPlans[i], lvCurrentTime);

                            if (lvTrainMovRes != null)
                            {
                                if ((lvPlannedSet != null) && (lvPlannedSet.Contains(lvPlans[i].Last.TrainId)))
                                {
                                    lvPlannedSet.Remove(lvPlans[i].Last.TrainId);
                                }
                                lvPlans.RemoveAt(i);
                                //DebugLog.Logar("GenerateIndividual().VerifyConflict() = " + VerifyConflict(), pIndet: TrainIndividual.IDLog);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            lvPlans.RemoveAt(i--);
                        }
                    }

                    if (mDicTrain.Count > 0)
                    {
                        if ((lvDicTrainCount == mDicTrain.Count) && (lvGeneCount == mList.Count))
                        {
                            if ((!pAllowDeadLockIndividual) || ((lvLoadedData != null) && (lvLoadedData.Count > 0)))
                            {
                                DebugLog.EnableDebug = true;
                                DebugLog.Logar("Individuo criado com deadlock -> Inválido !", false, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("mList.Count = " + mList.Count, false, pIndet: TrainIndividual.IDLog);
#if DEBUG
                                DumpDicTrain(mDicTrain);
                                DumpDeadLockPoint();
                                DumpStopLocation(null, false);
                                GenerateFlotFiles(DebugLog.LogPath);
#endif
                                //Dump(mList, 0L);
                                DebugLog.EnableDebug = lvLogEnabled;
                                lvRes = false;
                                DebugLog.Save("Individuo " + mUniqueId + " criado com deadlock -> Inválido !");

                                break;
                            }
                            else
                            {
                                lvGeneCount = mList.Count;
                                lvDicTrainCount = mDicTrain.Count;
                                lvRes = true;
                                mCurrentFitness = GetFitness();

                                break;
                            }
                        }
                        else
                        {
                            //DumpCountStopLocations(null);
                            lvGeneCount = mList.Count;
                            lvDicTrainCount = mDicTrain.Count;
                        }
                    }

                    if ((mDicTrain.Count == 0) && (lvPlans.Count > 0))
                    {
                        if (lvCurrentTime <= lvPlans[0].Last.DepartureTime)
                        {
                            lvCurrentTime = lvPlans[0].Last.DepartureTime;
                            mDicTrain.Add(lvPlans[0].Last.TrainId, lvPlans[0]);
                            lvPlans.RemoveAt(0);
                        }
                    }
                }
            }

            if ((mDicTrain.Count == 0) && (mList.Count > 0))
            {
                //DebugLog.Logar("GenerateIndividual().VerifyConflict() = " + VerifyConflict(), pIndet: TrainIndividual.IDLog);
                lvRes = true;
                mCurrentFitness = GetFitness();
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            DebugLog.EnableDebug = lvLogEnabled;
        }

        return lvRes;
    }

    private void DumpDicTrain(Dictionary<Int64, TrainMovement> pDicTrain)
    {
        Gene lvGene;
        StringBuilder lvStrInfo = new StringBuilder();

        DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
        DebugLog.Logar(" ---------------------------- pDicTrain" + "(count: " + pDicTrain.Count + ") ------------------------------- ", false, pIndet: TrainIndividual.IDLog);

        foreach (TrainMovement lvTrainMov in pDicTrain.Values)
        {
            lvGene = lvTrainMov.Last;

            lvStrInfo.Clear();

            lvStrInfo.Append(lvGene.TrainId);
            lvStrInfo.Append(" - ");
            lvStrInfo.Append(lvGene.TrainName);
            lvStrInfo.Append(" | ");
            lvStrInfo.Append(lvGene.Time);
            lvStrInfo.Append(" | (Local: ");
            lvStrInfo.Append(lvGene.SegmentInstance.Location);
            lvStrInfo.Append(".");
            lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);
            lvStrInfo.Append(", Linha: ");
            lvStrInfo.Append(lvGene.Track);

            if (lvGene.State == Gene.STATE.OUT)
            {
                lvStrInfo.Append(", Satus: Saída)");
            }
            else if (lvGene.State == Gene.STATE.IN)
            {
                lvStrInfo.Append(", Satus: Chegada)");
            }
            else
            {
                lvStrInfo.Append(", Satus: Indefinido)");
            }
            DebugLog.Logar(lvStrInfo.ToString(), false, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" ------------------------------------------------------------------------------ ", false, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
        }
    }

    private Interdicao GetInterdiction(int pInitCoordinate, int pEndCoordinate, DateTime pTimeRef, int pLine)
    {
        Interdicao lvRes = null;
        int lvIndex = -1;
        int lvCoordinate = 0;
        List<Interdicao> lvInterdictions = null;
        Interdicao lvInterdicao = null;
        int lvCurrentLine = 0;

        lvInterdictions = Interdicao.GetList();

        if (lvInterdictions.Count > 0)
        {
            lvCoordinate = pInitCoordinate + (pEndCoordinate - pInitCoordinate) / 2;
            lvInterdicao = Interdicao.GetCurrentInterdiction(lvCoordinate, out lvIndex);

            if (lvIndex >= 0)
            {
                for (int i = lvIndex; i < lvInterdictions.Count; i++)
                {
                    lvInterdicao = lvInterdictions[i];
                    if ((lvInterdicao.Start_time <= pTimeRef) && (lvInterdicao.End_time > pTimeRef) && (lvCoordinate >= lvInterdicao.Start_pos) && (lvCoordinate <= lvInterdicao.End_pos))
                    {
                        lvCurrentLine = lvInterdicao.Track;
                        if (lvCurrentLine == pLine)
                        {
                            lvRes = lvInterdicao;
                            break;
                        }
                    }
                }

                if(lvRes == null)
                {
                    for (int i = lvIndex; i < lvInterdictions.Count; i++)
                    {
                        lvInterdicao = lvInterdictions[i];
                        if ((lvInterdicao.End_time < pTimeRef) && (lvCoordinate >= lvInterdicao.Start_pos) && (lvCoordinate <= lvInterdicao.End_pos))
                        {
                            lvCurrentLine = lvInterdicao.Track;
                            if (lvCurrentLine == pLine)
                            {
                                lvRes = lvInterdicao;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        return lvRes;
    }

    private DateTime[] GetCurrentFirstOutputTime(Gene pGene, StopLocation pStopLocation)
    {
        DateTime[] lvRes = new DateTime[pStopLocation.Capacity];
        ISet<Gene> lvGeneStopLocationSet;
        Interdicao lvInterdition;
        int lvStopLocationValue = pStopLocation.Location;

        if (mStopLocationDeparture.ContainsKey(lvStopLocationValue) && (pGene.Time > DateTime.MinValue))
        {
            lvGeneStopLocationSet = mStopLocationDeparture[lvStopLocationValue];

            foreach(Gene lvGene in lvGeneStopLocationSet)
            {
                if (lvGene.Direction == pGene.Direction)
                {
                    if ((lvGene != null) && (pGene.TrainId != lvGene.TrainId))
                    {
                        if (lvGene.Time > pGene.Time)
                        {
                            if (lvGene.Time > lvRes[lvGene.Track - 1])
                            {
                                lvRes[lvGene.Track - 1] = lvGene.HeadWayTime;

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("Atraso do trem " + pGene.TrainId + " Em " + pGene.SegmentInstance.Location + "." + pGene.SegmentInstance.SegmentValue + " com tempo anteior de " + pGene.Time + " foi para " + lvGene.HeadWayTime + " por lvLastDepTime (" + lvGene + ") na linha " + lvGene.Track, pIndet: TrainIndividual.IDLog);
                                }
#endif
                            }
                            //DumpStopDepLocation(pStopLocation);
                        }
                        else
                        {
#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("GetCurrentFirstOutputTime break !", pIndet: TrainIndividual.IDLog);
                            }
#endif

                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < pStopLocation.Capacity; i++)
            {
                lvInterdition = GetInterdiction(pStopLocation.Start_coordinate, pStopLocation.End_coordinate, pGene.Time, i + 1);
                if (lvInterdition != null)
                {
                    if ((lvInterdition.End_time >= pGene.Time) && (lvInterdition.End_time > lvRes[lvInterdition.Track - 1]))
                    {
                        lvRes[lvInterdition.Track - 1] = lvInterdition.End_time;

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("Atraso do trem " + pGene.TrainId + " Em " + pGene.SegmentInstance.Location + "." + pGene.SegmentInstance.SegmentValue + " com tempo anteior de " + pGene.Time + " foi para " + lvInterdition.End_time + " por lvLastDepTime interdicao na linha " + (i + 1), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }
                }
            }
        }

        return lvRes;
    }

    private void GetHeadWays(Gene pGene, StopLocation pStopLocation, int pDirection, ref List<Gene[]> pGeneHeadwaysTime)
    {
        Gene[] lvValues;
        ISet<Gene> lvGenes = null;
        StopLocation lvPrevStopLocation;
        int lvStopLocationValue = -1;
        int lvOutIndex;

        if (pStopLocation != null)
        {
            lvStopLocationValue = pStopLocation.Location;

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DumpStopArrivalLocation(pStopLocation, 6);
            }
#endif

            if (mStopLocationArrival.ContainsKey(lvStopLocationValue))
            {
                lvGenes = mStopLocationArrival[lvStopLocationValue];

                foreach(Gene lvGene in lvGenes)
                {
                    if (lvGene != null)
                    {
                        if (pGene.TrainId != lvGene.TrainId)
                        {
                            if ((lvGene.Time > pGene.Time) && (pDirection != lvGene.Direction))
                            {
                                lvValues = new Gene[2];

                                try
                                {
                                    lvPrevStopLocation = lvGene.StopLocation.GetNextStopSegment(-1 * lvGene.Direction);
                                    lvValues[0] = GetLastStep(lvGene, out lvOutIndex, Gene.STATE.OUT, lvPrevStopLocation);
                                    lvValues[1] = lvGene;

                                    if ((lvValues[0] != null) && (lvValues[1].HeadWayTime > lvValues[0].Time))
                                    {
                                        pGeneHeadwaysTime.Add(lvValues);

#if DEBUG
                                        if (DebugLog.EnableDebug)
                                        {
                                            StringBuilder lvStrInfo = new StringBuilder();

                                            lvStrInfo.Append("GetHeadWays => adicionando jornada de (");
                                            lvStrInfo.Append(lvGene.TrainId);
                                            lvStrInfo.Append(" - ");
                                            lvStrInfo.Append(lvGene.TrainName);
                                            lvStrInfo.Append(": ");
                                            lvStrInfo.Append(lvValues[0]);
                                            lvStrInfo.Append(" => ");
                                            lvStrInfo.Append(lvValues[1]);
                                            lvStrInfo.Append(" com chegada em ");
                                            lvStrInfo.Append(lvGene.SegmentInstance.Location);
                                            lvStrInfo.Append(".");
                                            lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);
                                            lvStrInfo.Append(", HeadWayTime = ");
                                            lvStrInfo.Append(lvGene.HeadWayTime);
                                            lvStrInfo.Append(") devido ao Gene (");
                                            lvStrInfo.Append(pGene.TrainId);
                                            lvStrInfo.Append(" - ");
                                            lvStrInfo.Append(pGene.TrainName);
                                            lvStrInfo.Append(" em ");
                                            lvStrInfo.Append(lvGene.SegmentInstance.Location);
                                            lvStrInfo.Append(".");
                                            lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);
                                            lvStrInfo.Append(", considerando Direction: ");
                                            lvStrInfo.Append(pDirection);
                                            lvStrInfo.Append(", Stop Location: ");
                                            lvStrInfo.Append(pStopLocation.Location);

                                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                                            DumpStopLocation(pStopLocation);
                                        }
#endif
                                    }
                                    else
                                    {
#if DEBUG
                                        if (DebugLog.EnableDebug)
                                        {
                                            StringBuilder lvStrInfo = new StringBuilder();

                                            lvStrInfo.Append("GetHeadWays => ignorado (");
                                            lvStrInfo.Append(lvGene.TrainId);
                                            lvStrInfo.Append(" - ");
                                            lvStrInfo.Append(lvGene.TrainName);
                                            lvStrInfo.Append(": ");
                                            lvStrInfo.Append(lvValues[0]);
                                            lvStrInfo.Append(" => ");
                                            lvStrInfo.Append(lvValues[1]);

                                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                        }
#endif
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
                                }
                            }
                            else if((lvGene.Time < pGene.Time))
                            {
#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    StringBuilder lvStrInfo = new StringBuilder();

                                    lvStrInfo.Append("GetHeadWays => Breaking devido a lvGene.Time < pGene.Time (lvGene.Time: ");
                                    lvStrInfo.Append(lvGene.Time);
                                    lvStrInfo.Append("; lvGene.HeadWayTime: ");
                                    lvStrInfo.Append(lvGene.HeadWayTime);
                                    lvStrInfo.Append("; pGene.Time: ");
                                    lvStrInfo.Append(pGene.Time);
                                    lvStrInfo.Append("; pGene.HeadWayTime: ");
                                    lvStrInfo.Append(pGene.HeadWayTime);
                                    lvStrInfo.Append(")");

                                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                }
#endif

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private int GetPAT(StopLocation pStopLocation, Int64 pTrainId)
    {
        int lvRes = 0;
        List<Trainpat> lvPATs = null;
        int lvStopCoordinate = -1;

        try
        {
            if (pStopLocation != null)
            {
                if (mPATs.ContainsKey(pTrainId))
                {
                    lvPATs = mPATs[pTrainId];

                    foreach (Trainpat lvPAT in lvPATs)
                    {
                        lvStopCoordinate = lvPAT.Coordinate;

                        if (pStopLocation.Start_coordinate <= lvStopCoordinate && pStopLocation.End_coordinate >= lvStopCoordinate)
                        {
                            lvRes = lvPAT.Duration;
                            break;
                        }
                        else if(lvStopCoordinate > pStopLocation.End_coordinate)
                        {
                            break;
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

    public DateTime GetOptimumEndTime(Gene pGene)
    {
        DateTime lvRes = DateTime.MaxValue;
        double lvTotalTime = 0.0;
        double lvMinTime = double.MaxValue;
        double lvTime = double.MaxValue;
        List<Trainpat> lvPATs = null;
        StopLocation lvStopLocation = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvDestLocation = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSegment = null;
        int lvMeanCoordinate = 0;

        lvStopLocation = pGene.StopLocation;

        if (pGene.SegmentInstance == null)
        {
            if (lvStopLocation != null)
            {
                lvCurrentSegment = lvStopLocation.GetSegment(pGene.Direction * (-1), pGene.Track);
            }
            else
            {
                lvCurrentSegment = Segment.GetSegmentAt(pGene.Coordinate, pGene.Track);
            }
        }
        else
        {
            lvCurrentSegment = pGene.SegmentInstance;
        }

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment);
        }
        */

        lvDestLocation = pGene.EndStopLocation;

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvDestLocation = " + lvDestLocation);
        }
        */

        while ((lvStopLocation != lvDestLocation) && (lvCurrentSegment != null))
        {
            if (lvStopLocation != null)
            {
                lvNextStopLocation = lvStopLocation.GetNextStopSegment(pGene.Direction);
                lvCurrentSegment = lvStopLocation.GetSegment(pGene.Direction, 1);
            }
            else
            {
                lvNextStopLocation = lvCurrentSegment.GetNextStopLocation(pGene.Direction);
                lvCurrentSegment = pGene.SegmentInstance;
            }

            if (lvNextStopLocation == null)
            {
                break;
            }

            /*
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("GetOptimum.lvNextStopLocation = " + lvNextStopLocation, pIndet: TrainIndividual.IDLog);
            }
            */

            lvMinTime = double.MaxValue;
            for (int i = (lvNextStopLocation.Capacity - 1); i >= 0; i--)
            {
                lvTime = double.MaxValue;

                lvNextSegment = lvNextStopLocation.GetSegment(pGene.Direction * (-1), (i + 1));

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvNextSegment = " + lvNextSegment, pIndet: TrainIndividual.IDLog);
                }
                */

                if (lvNextSegment != null)
                {
                    if (pGene.Direction > 0)
                    {
                        lvMeanCoordinate = lvNextSegment.Start_coordinate - lvCurrentSegment.End_coordinate;
                    }
                    else
                    {
                        lvMeanCoordinate = lvCurrentSegment.Start_coordinate - lvNextSegment.End_coordinate;
                    }

                    lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                }

                if (lvTime < lvMinTime)
                {
                    lvMinTime = lvTime;

                    /*
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("GetOptimum.lvMinTime = " + lvMinTime, pIndet: TrainIndividual.IDLog);
                    }
                    */
                }
            }

            lvTotalTime += lvMinTime;

            /*
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
            }
            */

            if (lvNextStopLocation != lvDestLocation)
            {
                lvCurrentSegment = lvNextSegment;
                lvNextSegment = lvNextStopLocation.GetSegment(pGene.Direction, 1);

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("GetOptimum.lvNextSegment = " + lvNextSegment, pIndet: TrainIndividual.IDLog);
                }
                */

                if (lvNextSegment != null)
                {
                    if (pGene.Direction > 0)
                    {
                        lvMeanCoordinate = lvNextSegment.End_coordinate - lvCurrentSegment.Start_coordinate;
                    }
                    else
                    {
                        lvMeanCoordinate = lvCurrentSegment.End_coordinate - lvNextSegment.Start_coordinate;
                    }

                    lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                }

                lvTotalTime += lvTime;

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
                }
                */
            }

            lvStopLocation = lvNextStopLocation;
        }

        if (mPATs.ContainsKey(pGene.TrainId))
        {
            lvPATs = mPATs[pGene.TrainId];

            foreach (Trainpat lvPAT in lvPATs)
            {
                lvTotalTime += (double)lvPAT.Duration / 60.0;
            }
        }

        lvRes = pGene.Time.AddHours(lvTotalTime);

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ------------------------------------------------------------------------------------------------------ ", pIndet: TrainIndividual.IDLog);
        }
        */

        return lvRes;
    }

    public double GetOptimum(Gene pGene)
    {
        double lvRes = 0.0;
        double lvMinTime = double.MaxValue;
        double lvTime = double.MaxValue;
        List<Trainpat> lvPATs = null;
        StopLocation lvStopLocation = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvDestLocation = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSegment = null;
        int lvMeanCoordinate = 0;

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" --------------------------------- GetOptimum (" + pGene + ") ------------------------------------------ ", pIndet: TrainIndividual.IDLog);
        }
        */

        lvStopLocation = pGene.StopLocation;

        if (pGene.SegmentInstance == null)
        {
            if (lvStopLocation != null)
            {
                lvCurrentSegment = lvStopLocation.GetSegment(pGene.Direction * (-1), pGene.Track);
            }
            else
            {
                lvCurrentSegment = Segment.GetSegmentAt(pGene.Coordinate, pGene.Track);
            }
        }
        else
        {
            lvCurrentSegment = pGene.SegmentInstance;
        }

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment);
        }
        */

        lvDestLocation = pGene.EndStopLocation;

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvDestLocation = " + lvDestLocation);
        }
        */

        while ((lvStopLocation != lvDestLocation) && (lvCurrentSegment != null))
        {
            if (lvStopLocation != null)
            {
                lvNextStopLocation = lvStopLocation.GetNextStopSegment(pGene.Direction);
                lvCurrentSegment = lvStopLocation.GetSegment(pGene.Direction, 1);
            }
            else
            {
                lvNextStopLocation = lvCurrentSegment.GetNextStopLocation(pGene.Direction);
                lvCurrentSegment = pGene.SegmentInstance;
            }

            if (lvNextStopLocation == null)
            {
                break;
            }

            /*
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("GetOptimum.lvNextStopLocation = " + lvNextStopLocation, pIndet: TrainIndividual.IDLog);
            }
            */

            lvMinTime = double.MaxValue;
            for (int i = (lvNextStopLocation.Capacity - 1); i >= 0; i--)
            {
                lvTime = double.MaxValue;

                lvNextSegment = lvNextStopLocation.GetSegment(pGene.Direction * (-1), (i + 1));

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvNextSegment = " + lvNextSegment, pIndet: TrainIndividual.IDLog);
                }
                */

                if (lvNextSegment != null)
                {
                    if (pGene.Direction > 0)
                    {
                        lvMeanCoordinate = lvNextSegment.Start_coordinate - lvCurrentSegment.End_coordinate;
                    }
                    else
                    {
                        lvMeanCoordinate = lvCurrentSegment.Start_coordinate - lvNextSegment.End_coordinate;
                    }

                    lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                }

                if (lvTime < lvMinTime)
                {
                    lvMinTime = lvTime;

                    /*
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("GetOptimum.lvMinTime = " + lvMinTime, pIndet: TrainIndividual.IDLog);
                    }
                    */
                }
            }

            lvRes += lvMinTime;

            /*
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
            }
            */

            if (lvNextStopLocation != lvDestLocation)
            {
                lvCurrentSegment = lvNextSegment;
                lvNextSegment = lvNextStopLocation.GetSegment(pGene.Direction, 1);

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvCurrentSegment = " + lvCurrentSegment, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("GetOptimum.lvNextSegment = " + lvNextSegment, pIndet: TrainIndividual.IDLog);
                }
                */

                if (lvNextSegment != null)
                {
                    if (pGene.Direction > 0)
                    {
                        lvMeanCoordinate = lvNextSegment.End_coordinate - lvCurrentSegment.Start_coordinate;
                    }
                    else
                    {
                        lvMeanCoordinate = lvCurrentSegment.End_coordinate - lvNextSegment.Start_coordinate;
                    }

                    lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                }

                lvRes += lvTime;

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
                }
                */
            }

            lvStopLocation = lvNextStopLocation;
        }

        if (mPATs.ContainsKey(pGene.TrainId))
        {
            lvPATs = mPATs[pGene.TrainId];

            foreach (Trainpat lvPAT in lvPATs)
            {
                lvRes += (double)lvPAT.Duration / 60.0;
            }
        }

        /*
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetOptimum.lvRes = " + lvRes, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ------------------------------------------------------------------------------------------------------ ", pIndet: TrainIndividual.IDLog);
        }
        */

        return lvRes;
    }

    private void DumpDeadLockVerify(Gene pRefGene, Gene pGene, Segment pNextSwitch, int pLine)
    {
        int lvLimitPosition = -1;

        if (DebugLog.EnableDebug)
        {
            if (pNextSwitch == null)
            {
                return;
            }

            if (pGene.Direction > 0)
            {
                lvLimitPosition = pNextSwitch.Start_coordinate;
            }
            else
            {
                lvLimitPosition = pNextSwitch.End_coordinate;
            }

            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" --------------------------  DumpDeadLockVerify -------------------------------------- ", pIndet: TrainIndividual.IDLog);

            DebugLog.Logar("lvLimitPosition = " + lvLimitPosition, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pLine = " + pLine, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

            DebugLog.Logar("pRefGene.TrainId = " + pRefGene.TrainId, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.TrainName = " + pRefGene.TrainName, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.Location = " + pRefGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.UD = " + pRefGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.Time = " + pRefGene.Time, pIndet: TrainIndividual.IDLog);
            if (pRefGene.StopLocation != null)
            {
                DebugLog.Logar("pRefGene.StopLocation.Location = " + pRefGene.StopLocation.Location, pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar("pRefGene.Coordinate = " + pRefGene.Coordinate, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.Track = " + pRefGene.Track, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pRefGene.Direction = " + pRefGene.Direction, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);

            DebugLog.Logar("pGene.TrainId = " + pGene.TrainId, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.TrainName = " + pGene.TrainName, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Location = " + pGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.UD = " + pGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Time = " + pGene.Time, pIndet: TrainIndividual.IDLog);
            if (pGene.StopLocation != null)
            {
                DebugLog.Logar("pGene.StopLocation.Location = " + pGene.StopLocation.Location, pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar("pGene.Coordinate = " + pGene.Coordinate, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Track = " + pGene.Track, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("pGene.Direction = " + pGene.Direction, pIndet: TrainIndividual.IDLog);

            DebugLog.Logar(" ------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
        }
    }

    private void DumpBoolArray(bool[] pArray)
    {
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ------------------------------------------------------ ", pIndet: TrainIndividual.IDLog);
            for (int i = 0; i < pArray.Length; i++)
            {
                DebugLog.Logar("pArray[" + i + "] = " + pArray[i], pIndet: TrainIndividual.IDLog);
            }
            DebugLog.Logar(" ------------------------------------------------------ ", pIndet: TrainIndividual.IDLog);
            DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
        }
    }

    private bool[] VerifyNextSegment(Gene pGene, StopLocation pNextStopLocation, Segment pNextSwitch, bool pVerifyFirstRound, out bool pHasSameDirection, out int pSumDir, out bool[] pOccup, out int pCount)
    {
        bool[] lvRes = new bool[pNextStopLocation.Capacity];
        StopLocation lvStopLocation = null;
        ISet<int> lvDependencySet;
        DateTime lvEndInterditionTime = DateTime.MinValue;
        Gene lvGene = null;
        TrainMovement lvTrainMovement;
        int lvLimitPosition = Int32.MinValue;
        int lvCurrentPosition = Int32.MinValue;
        int lvCapacity = 0;
        int lvOccupCount = 0;
        int lvSumSameDir = 0;
        bool lvIsFirstRound = true;
        bool lvMovAllowed = true;

        pOccup = new bool[pNextStopLocation.Capacity];
        pHasSameDirection = false;
        pSumDir = 0;
        pCount = 0;
        for (int i = 0; i < lvRes.Length; i++)
        {
            lvRes[i] = true;
        }

        if(pNextSwitch == null)
        {
            return lvRes;
        }

        if (pGene.Direction > 0)
        {
            lvLimitPosition = pNextSwitch.Start_coordinate;
        }
        else
        {
            lvLimitPosition = pNextSwitch.End_coordinate;
        }
        lvStopLocation = pNextStopLocation;
        lvCurrentPosition = lvStopLocation.Location;

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("VerifyNextSegment.lvStopLocation = " + lvStopLocation, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("VerifyNextSegment para " + pGene, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("VerifyNextSegment.pCapacity = " + lvStopLocation.Capacity, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("VerifyNextSegment.lvLimitPosition = " + lvLimitPosition, pIndet: TrainIndividual.IDLog);
            //DumpNextStopLocation(pGene);
        }
#endif

        while (((lvCurrentPosition < lvLimitPosition) && (pGene.Direction > 0)) || ((lvCurrentPosition > lvLimitPosition) && (pGene.Direction < 0)))
        {
            if (mStopLocationOcupation.ContainsKey(lvStopLocation.Location))
            {
                foreach (Int64 lvTrainId in mStopLocationOcupation[lvStopLocation.Location])
                {
                    if (mDicTrain.ContainsKey(lvTrainId))
                    {
                        lvTrainMovement = mDicTrain[lvTrainId];
                        lvGene = lvTrainMovement.Last;

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("VerifyNextSegment.Verificando (" + lvGene + ")", pIndet: TrainIndividual.IDLog);
                        }
#endif
                        
                        if ((lvGene.Track <= lvStopLocation.Capacity) && (lvGene.StopLocation.Location == lvStopLocation.Location))
                        {
#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("VerifyNextSegment.pGene.Direction = " + pGene.Direction + " => " + pGene, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("VerifyNextSegment.lvGene.Direction = " + lvGene.Direction + " => " + lvGene, pIndet: TrainIndividual.IDLog);
                            }
#endif

                            if (pGene.Direction != lvGene.Direction)
                            {
                                lvRes[lvGene.Track - 1] = false;
                            }
                            else if (pGene.Direction == lvGene.Direction)
                            {
                                if (!pOccup[lvGene.Track - 1])
                                {
                                    lvSumSameDir++;
                                }

                                if (lvSumSameDir >= (pNextStopLocation.Capacity - 1))
                                {
                                    pHasSameDirection = true;
                                }
                            }

                            if (!pOccup[lvGene.Track - 1])
                            {
                                pOccup[lvGene.Track - 1] = true;
                                pSumDir += lvGene.Direction;
                                lvOccupCount++;
                            }

                            pCount++;
                        }
                    }
                    else
                    {
                        DebugLog.Logar("!mDicTrain.ContainsKey(" + lvTrainId + ")", pIndet: TrainIndividual.IDLog);
                    }
                }
            }
            else
            {
                return lvRes;
            }

            if (pVerifyFirstRound && lvIsFirstRound)
            {
                // Verifica se o próximo local de parada tem todos os destinos ocupados e interrompe a verificação
                lvMovAllowed = false;
                for (int i = 0; i < pOccup.Length; i++)
                {
                    if (!pOccup[i])
                    {
                        lvMovAllowed = true;

                        lvDependencySet = lvStopLocation.HasDependency(i + 1, pGene.Direction * (-1));

                        if (lvDependencySet != null)
                        {
                            lvRes[i] = false;
                        }
                    }
                    else
                    {
                        lvRes[i] = false;
                    }
                }

                if (!lvMovAllowed)
                {
                    pHasSameDirection = false;

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("VerifyNextSegment.bloqueando no first round !", pIndet: TrainIndividual.IDLog);
                    }
#endif

                    return lvRes;
                }
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("VerifyNextSegment.lvStopLocation Antes de Atualizar = " + lvStopLocation, pIndet: TrainIndividual.IDLog);
            }
#endif

            lvCapacity = lvStopLocation.Capacity;

            lvStopLocation = lvStopLocation.GetNextStopSegment(pGene.Direction);
            lvIsFirstRound = false;
            if (lvStopLocation == null)
            {
                break;
            }
            else
            {
                lvCurrentPosition = lvStopLocation.Location;
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("VerifyNextSegment.lvStopLocation Depois de atualizar = " + lvStopLocation, pIndet: TrainIndividual.IDLog);
            }

            if (DebugLog.EnableDebug)
            {
                DumpStopLocation(lvStopLocation);
                DumpBoolArray(lvRes);
            }
#endif
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("VerifyNextSegment.pSumDir = " + pSumDir, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("VerifyNextSegment.lvOccupCount = " + lvOccupCount, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("VerifyNextSegment.pCount = " + pCount, pIndet: TrainIndividual.IDLog);
        }
#endif

        return lvRes;
    }

    /* pNextSwitch de pNextStopLocation */
    private bool[] VerifySegmentDeadLock(Gene pGene, StopLocation pNextStopLocation, Segment pNextSwitch, out bool pHasSameDirection, out int pSumDir, out bool[] pLastOcup)
    {
        bool[] lvRes = null;
        bool[] lvInternRes = null;
        bool[] lvOccup = null;
        bool[] lvLastOccup = null;
        bool[] lvResSameDir = new bool[pNextStopLocation.Capacity];
        bool lvHasSameDirection = false;
        StopLocation lvStopLocation = null;
        Segment lvNextSwitch = null;
        Segment lvSegment = null;
        DateTime lvEndInterditionTime = DateTime.MinValue;
        int lvSumSameDir = 0;
        int lvLastSumSameDir = 0;
        int lvLimitPosition = Int32.MinValue;
        int lvCapacity = pNextStopLocation.Capacity;
        int lvCount = 0;
        int lvLastCount = 0;
        int lvLastTotalOcup = 0;

        bool lvIsLogEnables = DebugLog.EnableDebug;

        pHasSameDirection = false;
        pLastOcup = null;
        pSumDir = 0;

#if DEBUG
        DebugLog.EnableDebug = true;
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("\n ------------------------- VerifySegmentDeadLock para (" + pGene + " em " + pNextStopLocation + " com pNextSwitch: " + pNextSwitch + ") -------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

        if (pGene.Track == 0)
        {
            return new bool[lvCapacity];
        }

        if (pNextSwitch == null)
        {
            lvSegment = pGene.EndStopLocation.GetSegment(pGene.Direction, pGene.Track);
        }
        else
        {
            lvSegment = pNextSwitch;
        }

#if DEBUG
        DebugLog.EnableDebug = true;
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("Verificando ate " + lvSegment.Location, pIndet: TrainIndividual.IDLog);
        }
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

#if DEBUG
        DebugLog.EnableDebug = true;
#endif
        lvRes = VerifyNextSegment(pGene, pNextStopLocation, lvSegment, true, out lvHasSameDirection, out lvSumSameDir, out lvOccup, out lvCount);
        pSumDir = lvSumSameDir;

#if DEBUG
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

        if (lvHasSameDirection && (pNextSwitch != null))
        {
#if DEBUG
            DebugLog.EnableDebug = true;
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("lvHasSameDirection (" + pGene + "), existe trem no mesmo sentido !", pIndet: TrainIndividual.IDLog);
                //DumpNextStopLocation(pGene);
            }
            DebugLog.EnableDebug = lvIsLogEnables;
#endif

            if (lvSumSameDir == (pGene.Direction * lvCapacity)) /* Não permitir colocar trem na calda se todos estiverem no mesmo sentido e não houver vaga */
            {
                lvRes = new bool[lvCapacity];
#if DEBUG
                DebugLog.EnableDebug = true;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("Evitando DeadLock de " + pGene.TrainId + " (" + pGene.TrainName + ") em " + pGene.SegmentInstance.Location + "." + pGene.SegmentInstance.SegmentValue + " indo para " + pNextStopLocation.Location, pIndet: TrainIndividual.IDLog);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif
            }
            else if((lvSumSameDir == (pGene.Direction * (lvCapacity - 1))) && (lvCount >= lvCapacity))
            {
                lvRes = new bool[lvCapacity];
#if DEBUG
                DebugLog.EnableDebug = true;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("Evitando DeadLock por excesso trens considerando " + pGene.TrainId + " (" + pGene.TrainName + ") em " + pGene.SegmentInstance.Location + "." + pGene.SegmentInstance.SegmentValue + " indo para " + pNextStopLocation.Location, pIndet: TrainIndividual.IDLog);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif
            }
            else
            {
                pHasSameDirection = true;

                // Verificar o próximo do próximo vizinho (próximo do atual)
                lvStopLocation = pNextSwitch.GetNextStopLocation(pGene.Direction);
#if DEBUG
                DebugLog.EnableDebug = true;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvStopLocation = " + lvStopLocation, pIndet: TrainIndividual.IDLog);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                if (lvStopLocation != null)
                {
                    lvNextSwitch = lvStopLocation.GetNextSwitchSegment(pGene.Direction);

                    if (lvNextSwitch != null)
                    {
                        if (pGene.Direction > 0)
                        {
                            lvLimitPosition = lvNextSwitch.Start_coordinate;
                        }
                        else
                        {
                            lvLimitPosition = lvNextSwitch.End_coordinate;
                        }

#if DEBUG
                        DebugLog.EnableDebug = true;
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvNextSwitch = " + lvNextSwitch, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvLimitPosition = " + lvNextSwitch.Start_coordinate, pIndet: TrainIndividual.IDLog);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
#endif
                    }
                    else
                    {
                        if (pGene.Direction > 0)
                        {
                            lvLimitPosition = pGene.EndStopLocation.End_coordinate;
                        }
                        else
                        {
                            lvLimitPosition = pGene.EndStopLocation.Start_coordinate;
                        }
                    }
                }
                else
                {
#if DEBUG
                    DebugLog.EnableDebug = true;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("lvHasSameDirection lvStopLocation == null, abortando verificacao para Gene (" + pGene + ")", pIndet: TrainIndividual.IDLog);
                    }
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif
                    return lvRes;
                }

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DumpStopLocation(lvStopLocation);
                    DebugLog.Logar("Verificando ate " + lvLimitPosition, pIndet: TrainIndividual.IDLog);
                }
#endif

                if (lvNextSwitch != null)
                {
                    lvResSameDir = VerifyNextSegment(pGene, lvStopLocation, lvNextSwitch, false, out lvHasSameDirection, out lvLastSumSameDir, out lvLastOccup, out lvLastCount);
                }
                else
                {
                    lvResSameDir = VerifyNextSegment(pGene, lvStopLocation, Segment.GetSegmentAt(lvLimitPosition, 1), false, out lvHasSameDirection, out lvLastSumSameDir, out lvLastOccup, out lvLastCount);
                }

#if DEBUG
                DebugLog.EnableDebug = true;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvLastSumSameDir = " + lvLastSumSameDir, pIndet: TrainIndividual.IDLog);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                lvLastTotalOcup = 0;
                for (int i = 0; i < lvLastOccup.Length; i++)
                {
                    if (lvLastOccup[i])
                    {
                        lvLastTotalOcup++;
                    }
                }
                pLastOcup = lvLastOccup;

#if DEBUG
                DebugLog.EnableDebug = true;
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvLastTotalOcup = " + lvLastTotalOcup, pIndet: TrainIndividual.IDLog);
                    DebugLog.Logar("lvStopLocation.Capacity = " + lvStopLocation.Capacity, pIndet: TrainIndividual.IDLog);
                }
                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                if ((lvLastTotalOcup == lvStopLocation.Capacity))
                {
                    for (int i = 0; i < lvOccup.Length; i++)
                    {
                        if (!lvOccup[i])
                        {
                            lvRes[i] = false;
                        }
                    }

#if DEBUG
                    DebugLog.EnableDebug = true;
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("Evitando DeadLock de " + pGene.TrainId + " (" + pGene.TrainName + ") em " + pGene.SegmentInstance.Location + "." + pGene.SegmentInstance.SegmentValue + " indo para " + pNextStopLocation.Location, pIndet: TrainIndividual.IDLog);
                    }
                    DebugLog.EnableDebug = lvIsLogEnables;
#endif
                }
                else if ((lvStopLocation.NoStopSet != null) && (lvStopLocation.NoStopSet.Count > 0))
                {
                    if (lvStopLocation.HasNoStop(pGene.TrainName.Substring(0, 1) + pGene.Direction))
                    {
                        lvInternRes = VerifySegmentDeadLock(pGene, lvStopLocation, lvNextSwitch, out lvHasSameDirection, out lvLastSumSameDir, out lvLastOccup);

                        lvCount = 0;

                        foreach(bool lvValue in lvInternRes)
                        {
                            if(lvValue)
                            {
                                break;
                            }

                            lvCount++;
                        }

                        if(lvCount >= lvStopLocation.Capacity)
                        {
                            lvRes = new bool[pNextStopLocation.Capacity];
                        }
                    }
                    else
                    {
                        if((pGene.Direction > 0) && lvStopLocation.HasBackwardDirection)
                        {
                            lvRes = new bool[pNextStopLocation.Capacity];
                        }
                        else if((pGene.Direction < 0) && lvStopLocation.HasForwardDirection)
                        {
                            lvRes = new bool[pNextStopLocation.Capacity];
                        }
                    }
                }
            }
        }

#if DEBUG
        DebugLog.EnableDebug = true;
        if (DebugLog.EnableDebug)
        {
            DumpBoolArray(lvRes);
            DebugLog.Logar(" ------------------------- Fim de VerifySegmentDeadLock -------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
        }
        DebugLog.EnableDebug = lvIsLogEnables;
#endif

        DebugLog.EnableDebug = lvIsLogEnables;

        return lvRes;
    }

    public void LoadDistRef()
    {
        TrainMovement lvTrainMovement = null;

        mDicDistRef.Clear();

        for(int i = 0; i < mList.Count; i++)
        {
            lvTrainMovement = mList[i];

            lock (mDicDistRef)
            {
                if (!mDicDistRef.ContainsKey(lvTrainMovement.GetID()))
                {
                    mDicDistRef.Add(lvTrainMovement.GetID(), i);
                }
            }
        }
    }

    private Gene GetLastStep(Gene pGene, out int pOutIndex, Gene.STATE pState = Gene.STATE.UNDEF, StopLocation pStopLocation = null, int pInitialIndex=-1)
    {
        Gene lvRes = null;
        TrainMovement lvTrainMovement;
        int lvIndex = pInitialIndex - 1;
        pOutIndex = -1;

        try
        {
            if (pInitialIndex == -1)
            {
                lvIndex = mList.Count - 1;
            }

            for (int i = lvIndex; i >= 0; i--)
            {
                lvTrainMovement = mList[i];

                for(int ind = lvTrainMovement.Count - 1; ind >= 0; ind--)
                {
                    lvRes = lvTrainMovement[ind];

                    if ((lvRes.TrainId == pGene.TrainId) && ((lvRes.State == pState) || (pState == Gene.STATE.UNDEF)) && ((pStopLocation == null) || ((lvRes.StopLocation != null) && (lvRes.StopLocation.Location == pStopLocation.Location))))
                    {
                        pOutIndex = i;
                        break;
                    }
                    else
                    {
                        lvRes = null;
                    }
                }

                if(lvRes != null)
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

    private void UpdateCrossingFail(Gene pRefGene)
    {
        TrainPerformanceControl lvTrainPerformance = null;
        List<Gene[]> lvListGenes = new List<Gene[]>();
        Gene lvPrevGene;
        Gene lvNextGene = null;
        Gene lvGene = null;
        Gene lvCurrentGene = null;
        StopLocation lvNextStopLocation = null;
        double lvSpentTime = 0.0;
        double lvMeanSpeed = 0.0;
        int lvDistance;
        int lvPrevGeneIndex;

        int lvOutIndex;

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("UpdateCrossingFail.lvRefGene = " + pRefGene, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("UpdateCrossingFail.mList.Count = " + mList.Count, pIndet: TrainIndividual.IDLog);
        }
#endif

        if ((pRefGene == null) || (pRefGene.StopLocation == null)) return;

        if (!GetTransitInfo(pRefGene, out lvPrevGene, out lvDistance, out lvPrevGeneIndex, -1)) return;

        if (pRefGene.State == Gene.STATE.IN)
        {
            GetHeadWays(pRefGene, pRefGene.StopLocation, pRefGene.Direction, ref lvListGenes);
            GetHeadWays(lvPrevGene, lvPrevGene.StopLocation, lvPrevGene.Direction, ref lvListGenes);

            if (lvListGenes.Count > 1)
            {
                lvListGenes.Sort(new HeadWayGeneTimeComparer());
            }

            lvCurrentGene = pRefGene;

            for (int i = 0; i < lvListGenes.Count; i++)
            {
                if ((lvListGenes[i] != null) && (lvListGenes[i].Length == 2))
                {
                    if ((lvListGenes[i][0].Time < lvCurrentGene.HeadWayTime) && (lvListGenes[i][1].HeadWayTime > lvPrevGene.Time))
                    {
                        lvGene = GetLastStep(lvListGenes[i][0], out lvOutIndex, Gene.STATE.IN, lvListGenes[i][0].StopLocation);
                        lvListGenes[i][0].Time = lvCurrentGene.HeadWayTime;

                        if (lvGene != null)
                        {
                            lvListGenes[i][0].Speed = ((lvGene.StopLocation.End_coordinate - lvGene.StopLocation.Start_coordinate) / 100000.0) / (lvListGenes[i][0].Time - lvGene.HeadWayTime).TotalHours;
                        }
                        else
                        {
                            lvListGenes[i][0].Speed = mMinSpeedLimit;
                        }
                        lvListGenes[i][0].HeadWayTime = lvListGenes[i][0].Time.AddHours(mTrainLenKM / lvListGenes[i][0].Speed);

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("UpdateCrossingFail.lvOtherTrainGene atualizado = " + lvListGenes[i][0], pIndet: TrainIndividual.IDLog);
                        }
#endif

                        if (lvTrainPerformance == null)
                        {
                            if (lvDistance < mTrainLen)
                            {
                                if (lvListGenes[i][0].Speed <= mMinSpeedLimit)
                                {
                                    lvMeanSpeed = (mMinSpeedLimit + mMinSpeedLimit * 0.35);
                                }
                                else
                                {
                                    lvMeanSpeed = lvListGenes[i][0].Speed;
                                }
                            }
                            else
                            {
                                lvMeanSpeed = (lvListGenes[i][0].Speed + mVMA) / 2.0;
                            }

                            lvSpentTime = (lvDistance / 100000.0) / lvMeanSpeed;
                            lvListGenes[i][1].Time = lvListGenes[i][0].Time.AddHours(lvSpentTime);
                            lvListGenes[i][1].HeadWayTime = lvListGenes[i][0].HeadWayTime.AddHours(lvSpentTime);

#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("UpdateCrossingFail.lvNextGene lvMeanSpeed used = " + lvMeanSpeed, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("UpdateCrossingFail.lvNextGene atualizado = " + lvNextGene, pIndet: TrainIndividual.IDLog);
                            }
#endif
                        }
                        else
                        {
                            if (lvTrainPerformance.TimeStop <= 0.0)
                            {
                                if (lvDistance < mTrainLen)
                                {
                                    lvMeanSpeed = (mMinSpeedLimit + mMinSpeedLimit * 0.35);
                                }
                                else
                                {
                                    lvMeanSpeed = (mMinSpeedLimit + mVMA) / 2.0;
                                }

                                lvSpentTime = (lvDistance / 100000.0) / lvMeanSpeed;
                                lvListGenes[i][1].Time = lvListGenes[i][0].Time.AddHours(lvSpentTime);
                                lvListGenes[i][1].HeadWayTime = lvListGenes[i][0].HeadWayTime.AddHours(lvSpentTime);
                            }
                            else
                            {
                                lvListGenes[i][1].Time = lvListGenes[i][0].Time.AddHours(lvTrainPerformance.TimeStop / 60.0);
                                lvListGenes[i][1].HeadWayTime = lvListGenes[i][1].Time.AddHours(lvTrainPerformance.TimeHeadWayStop / 60.0);
                            }

#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("UpdateCrossingFail.lvNextGene lvMeanSpeed used = " + lvMeanSpeed, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("UpdateCrossingFail.lvNextGene atualizado = " + lvNextGene, pIndet: TrainIndividual.IDLog);
                            }
#endif
                        }

                        lvNextGene = GetLastStep(lvListGenes[i][1], out lvOutIndex, Gene.STATE.OUT, lvListGenes[i][1].StopLocation);
                        UpdateCrossingFail(lvNextGene);

                        lvCurrentGene = lvListGenes[i][1];
                        lvPrevGene = lvListGenes[i][0];
                    }
                }
            }
        }
        else if(pRefGene.State == Gene.STATE.OUT)
        {
            lvDistance = pRefGene.StopLocation.End_coordinate - pRefGene.StopLocation.Start_coordinate;
            lvMeanSpeed = (lvDistance / 100000.0) / (pRefGene.Time - lvPrevGene.HeadWayTime).TotalHours;

            if(lvMeanSpeed > mVMA)
            {
                lvSpentTime = (lvDistance / 100000.0) / mVMA;
                pRefGene.Time = lvPrevGene.Time.AddHours(lvSpentTime);
                pRefGene.HeadWayTime = lvPrevGene.HeadWayTime.AddHours(lvSpentTime);

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("UpdateCrossingFail.pRefGene atualizado = " + pRefGene, pIndet: TrainIndividual.IDLog);
                }
#endif

                lvNextStopLocation = pRefGene.StopLocation.GetNextStopSegment(pRefGene.Direction);

                if (lvNextStopLocation != null)
                {
                    lvNextGene = GetLastStep(pRefGene, out lvOutIndex, Gene.STATE.IN, lvNextStopLocation);
                    UpdateCrossingFail(lvNextGene);
                }
            }
        }
    }

    private bool GetTransitInfo(Gene pRefGene, out Gene pPrevGene, out int pPrevGeneIndex, out int pDistance, int pIndex = -1)
    {
        bool lvRes = false;
        int lvEndCoordinate = 0;
        int lvInitCoordinate = 0;

        pPrevGene = null;
        pDistance = 0;
        pPrevGeneIndex = -1;

        if (pRefGene.StopLocation == null) return lvRes;

        if (pRefGene.State == Gene.STATE.IN)
        {
            if (pIndex > 0)
            {
                pPrevGene = mList[pIndex - 1][0];
            }
            else
            {
                pPrevGene = null;
            }
        }
        else if(pRefGene.State == Gene.STATE.OUT)
        {
            if (pIndex == -1)
            {
                pPrevGene = GetLastStep(pRefGene, out pPrevGeneIndex, Gene.STATE.IN, pRefGene.StopLocation, -1);
            }
            else
            {
                pPrevGene = GetLastStep(pRefGene, out pPrevGeneIndex, Gene.STATE.IN, pRefGene.StopLocation, pIndex);
            }
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetTransitInfo.pIndex = " + pIndex, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("GetTransitInfo.lvPrevGene = " + pPrevGene, pIndet: TrainIndividual.IDLog);
        }
#endif

        if (pPrevGene == null) return lvRes;

        if (pPrevGene.StopLocation != null)
        {
            if (pRefGene.Direction > 0)
            {
                lvInitCoordinate = pPrevGene.StopLocation.End_coordinate;
                lvEndCoordinate = pRefGene.StopLocation.Start_coordinate;
            }
            else
            {
                lvInitCoordinate = pPrevGene.StopLocation.Start_coordinate;
                lvEndCoordinate = pRefGene.StopLocation.End_coordinate;
            }
        }
        else
        {
            if (pRefGene.Direction > 0)
            {
                lvInitCoordinate = pPrevGene.SegmentInstance.End_coordinate;
                lvEndCoordinate = pRefGene.StopLocation.Start_coordinate;
            }
            else
            {
                lvInitCoordinate = pPrevGene.SegmentInstance.Start_coordinate;
                lvEndCoordinate = pRefGene.StopLocation.End_coordinate;
            }
        }
        pDistance = Math.Abs(lvEndCoordinate - lvInitCoordinate);

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("GetTransitInfo.lvInitCoordinate = " + lvInitCoordinate, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("GetTransitInfo.lvEndCoordinate = " + lvEndCoordinate, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("GetTransitInfo.pDistance = " + pDistance, pIndet: TrainIndividual.IDLog);
        }
#endif

        lvRes = true;

        return lvRes;
    }

    private void UpdateLastArrival(Gene pGene)
    {
        Gene lvPrevGene;
        int lvPrevGeneIndex;
        TrainPerformanceControl lvTrainPerformance = null;
        double lvSpentTime = 0.0;
        int lvDistance;

        /* Deve ser chamado apenas se detectar que parou quando partiu */

        try
        {
            /* 0 - Atualiza chegada do trem atual */
#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" ------------------------------- UpdateLastArrival (Gene: " + pGene.TrainId + ") ------------------------------- ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("UpdateLastArrival for " + pGene, pIndet: TrainIndividual.IDLog);
                DumpStopLocation(pGene.StopLocation);
            }
#endif

            /* Se já estava com velocidade reduzida não toma ação */
            if (pGene.Speed <= mMinSpeedLimit)
            {
                return;
            }

            lvTrainPerformance = TrainPerformanceControl.GetElementByKey(pGene.TrainName.Substring(0, 1), pGene.Direction, pGene.SegmentInstance.Location, pGene.SegmentInstance.SegmentValue, pGene.Track);

            //GenerateFlotFiles(DebugLog.LogPath);

            if (!GetTransitInfo(pGene, out lvPrevGene, out lvPrevGeneIndex, out lvDistance)) return;

            if (lvTrainPerformance == null)
            {
                pGene.Speed = pGene.Speed * mSpeedReductionFactor;
                lvSpentTime = (lvDistance / 100000.0) / pGene.Speed;
                pGene.Time = lvPrevGene.Time.AddHours(lvSpentTime);
                pGene.HeadWayTime = pGene.Time.AddHours(mTrainLenKM / pGene.Speed);

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("UpdateLastArrival.pGene atualizado = " + pGene, pIndet: TrainIndividual.IDLog);
                }
#endif
            }
            else
            {
                if (lvPrevGene.Speed > mMinSpeedLimit)
                {
                    if (lvTrainPerformance.TimeMovStop <= 0.0)
                    {
                        pGene.Speed = pGene.Speed * mSpeedReductionFactor;
                        lvSpentTime = (lvDistance / 100000.0) / pGene.Speed;
                        pGene.Time = lvPrevGene.Time.AddHours(lvSpentTime);
                        pGene.HeadWayTime = pGene.Time.AddHours(mTrainLenKM / pGene.Speed);
                    }
                    else
                    {
                        pGene.Speed = (lvPrevGene.Speed + ((lvDistance / 100000.0) / (lvTrainPerformance.TimeMovStop / 60.0))) / 2.0;
                        pGene.Time = lvPrevGene.Time.AddHours(lvTrainPerformance.TimeMovStop / 60.0);
                        pGene.HeadWayTime = lvPrevGene.Time.AddHours(lvTrainPerformance.TimeHeadwayMovStop / 60.0);
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("UpdateLastArrival.pGene atualizado = " + pGene, pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
                else
                {
                    if (lvTrainPerformance.TimeStopStop <= 0.0)
                    {
                        pGene.Speed = pGene.Speed * mSpeedReductionFactor;
                        lvSpentTime = (lvDistance / 100000.0) / pGene.Speed;
                        pGene.Time = lvPrevGene.Time.AddHours(lvSpentTime);
                        pGene.HeadWayTime = pGene.Time.AddHours(mTrainLenKM / pGene.Speed);
                    }
                    else
                    {
                        pGene.Speed = (lvPrevGene.Speed + ((lvDistance / 100000.0) / (lvTrainPerformance.TimeStopStop / 60.0))) / 2.0;
                        pGene.Time = lvPrevGene.Time.AddHours(lvTrainPerformance.TimeStopStop / 60.0);
                        pGene.HeadWayTime = lvPrevGene.Time.AddHours(lvTrainPerformance.TimeHeadwayStopStop / 60.0);
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("UpdateLastArrival.pGene atualizado = " + pGene, pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
            }

            UpdateCrossingFail(pGene);

            //DebugLog.Logar("UpdateLastArrival.VerifyConflict() = " + VerifyConflict(), pIndet: TrainIndividual.IDLog);

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar(" ----------------------------------------- Fim de UpdateLastArrival -------------------------------------------- ", pIndet: TrainIndividual.IDLog);
            }
#endif
        }
        catch(Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }
    }

    public double GetOptTimeToEnd(Int64 pTrainId)
    {
        double lvRes = 0;
        Gene lvGene = null;
        TrainMovement lvTrainMovement;

        if (mDicTrain.ContainsKey(pTrainId))
        {
            lvTrainMovement = mDicTrain[pTrainId];
            lvGene = lvTrainMovement.Last;

            if (lvGene.StopLocation != null)
            {
                if (lvGene.Direction > 0)
                {
                    lvRes = (Math.Abs(lvGene.EndStopLocation.Start_coordinate - lvGene.StopLocation.End_coordinate) / 100000.0) / mVMA;
                }
                else
                {
                    lvRes = (Math.Abs(lvGene.EndStopLocation.End_coordinate - lvGene.StopLocation.Start_coordinate) / 100000.0) / mVMA;
                }
            }
            else
            {
                if (lvGene.Direction > 0)
                {
                    lvRes = (Math.Abs(lvGene.EndStopLocation.Start_coordinate - lvGene.Coordinate) / 100000.0) / mVMA;
                }
                else
                {
                    lvRes = (Math.Abs(lvGene.EndStopLocation.End_coordinate - lvGene.Coordinate) / 100000.0) / mVMA;
                }
            }

            lvRes *= lvGene.ValueWeight;
        }

        return lvRes;
    }

    /*
    public bool IsCausingDeadLock(Gene pGene)
    {
        bool lvRes = true;
        Gene lvGene = null;
        StopLocation lvNextStopLocation = null;
        Segment lvNextSwitch = null;
        HashSet<Int64> lvListGeneStopLocation = null;

        try
        {
            if (pGene.StopLocation != null)
            {
                lvNextSwitch = pGene.StopLocation.GetNextSwitchSegment(pGene.Direction);
                lvNextStopLocation = pGene.StopLocation.GetNextStopSegment(pGene.Direction);

                if (lvNextStopLocation != null)
                {
                    lvRes = !VerifyMovement(pGene);

                    if (lvRes)
                    {
                        if ((((pGene.Direction > 0) && (lvNextStopLocation.Start_coordinate < pGene.StopLocation.NextSwitch.Start_coordinate)) || ((pGene.Direction < 0) && (lvNextStopLocation.End_coordinate > pGene.StopLocation.PrevSwitch.End_coordinate))) && mDicTrain.ContainsKey(pGene.TrainId))
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocation.Location];

                            if (lvListGeneStopLocation.Count == 0)
                            {
                                lvRes = false;
                            }
                            else
                            {
                                foreach (long lvTrainId in lvListGeneStopLocation)
                                {
                                    if (mDicTrain.ContainsKey(lvTrainId))
                                    {
                                        lvGene = mDicTrain[lvTrainId];

                                        if ((lvGene.Track == pGene.Track) && (lvGene.Direction != pGene.Direction))
                                        {
                                            lvRes = !VerifyMovement(lvGene);
                                        }
                                        else
                                        {
                                            lvRes = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[pGene.StopLocation.Location];

                            if (lvListGeneStopLocation.Count < pGene.StopLocation.Capacity)
                            {
                                lvRes = false;
                            }
                            else
                            {
                                foreach (long lvTrainId in lvListGeneStopLocation)
                                {
                                    if (mDicTrain.ContainsKey(lvTrainId))
                                    {
                                        lvGene = mDicTrain[lvTrainId];

                                        if (lvGene.TrainId != pGene.TrainId)
                                        {
                                            if (lvGene.Direction == pGene.Direction)
                                            {
                                                lvRes = !VerifyMovement(lvGene);

                                                if (!lvRes)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                lvRes = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (lvRes)
                            {
                                lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocation.Location];

                                if (lvListGeneStopLocation.Count < lvNextStopLocation.Capacity)
                                {
                                    lvRes = false;
                                }
                                else
                                {
                                    foreach (long lvTrainId in lvListGeneStopLocation)
                                    {
                                        if (mDicTrain.ContainsKey(lvTrainId))
                                        {
                                            lvGene = mDicTrain[lvTrainId];

                                            if (lvGene.TrainId != pGene.TrainId)
                                            {
                                                lvRes = !VerifyMovement(lvGene);

                                                if (!lvRes)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (lvRes)
                    {
                        DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
                        DebugLog.Logar(" ----------> Individuo sendo criado com deadlock -> Inválido !", false, pIndet: TrainIndividual.IDLog);
                        DebugLog.Logar("mList.Count = " + mList.Count, false, pIndet: TrainIndividual.IDLog);
                        DumpDicTrain(mDicTrain);
                        DebugLog.Logar("pGene = " + pGene, false, pIndet: TrainIndividual.IDLog);
                        DumpStopLocation(pGene.StopLocation, false);
                        DumpStopLocation(lvNextStopLocation, false);
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

    private bool VerifyMovement(Gene pGene)
    {
        bool lvRes = false;
        bool[] lvNextAvailable = null;
        Gene lvGene = null;
        Gene lvGen = null;
        StopLocation lvNextStopLocation = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSwitch = null;
        DateTime lvFirstEndInterdictionTime = DateTime.MaxValue;
        int lvNextStopLocationValue = 0;
        int lvNextCapacity = 0;
        int lvIndex;
        int lvDestTrack = 0;
        int lvSumDir;
        bool[] lvLastOccup = null;
        bool lvHasSameDirection;

        try
        {
            if (mDicTrain.ContainsKey(pGene.TrainId))
            {
                lvGene = mDicTrain[pGene.TrainId];

#if DEBUG
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("VerifyMovement => Gene obtido de mDicTrain(");
                lvStrInfo.Append(lvGene.ToString());
                lvStrInfo.Append(")");

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
#endif
            }
            else
            {
                if (pGene.StopLocation == null)
                {
                    lvGene = pGene;
                }
                else if (pGene.StopLocation.Location == pGene.StartStopLocation.Location)
                {
                    lvGene = pGene;
#if DEBUG
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("VerifyMovement => Gene aceito por pGene.StopLocation.Location == pGene.StartStopLocation.Location(");
                    lvStrInfo.Append(lvGene.ToString());
                    lvStrInfo.Append(")");

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
#endif
                }
                else
                {
                    return lvRes;
                }
            }

            if (lvGene.StopLocation != null)
            {
                lvNextStopLocation = lvGene.StopLocation.GetNextStopSegment(lvGene.Direction);
            }
            else
            {
                lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
            }

            if (lvGene.SegmentInstance == null)
            {
                lvCurrentSegment = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, lvGene.Track, out lvIndex);
            }
            else
            {
                lvCurrentSegment = lvGene.SegmentInstance;
            }

            if (lvNextStopLocation == null)
            {
#if DEBUG
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("VerifyMovement => lvNextStopLocation == null para ");
                lvStrInfo.Append(lvGene.ToString());

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
#endif

                return lvRes;
            }

            lvNextStopLocationValue = lvNextStopLocation.Location;

            if ((lvGene.StopLocation == lvGene.EndStopLocation) && (lvGene.StopLocation != null))
            {
#if DEBUG
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("VerifyMovement => (lvStopLocation == lvEndStopLocation) && (lvStopLocation != null) para ");
                lvStrInfo.Append(lvGene.ToString());

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
#endif

                return lvRes;
            }

            if (lvCurrentSegment == null)
            {
                lvCurrentSegment = Segment.GetNextSegment(lvGene.Coordinate, lvGene.Direction);
            }

            lvNextSwitch = lvNextStopLocation.GetNextSwitchSegment(lvGene.Direction);

            if (lvGene.StopLocation != null)
            {
                if (lvGene.Direction > 0)
                {
                    if ((lvGene.StopLocation.NextSwitch == null) || ((lvNextStopLocation.Start_coordinate < lvGene.StopLocation.NextSwitch.Start_coordinate) && mDicTrain.ContainsKey(lvGene.TrainId)))
                    {
                        lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            lvNextAvailable[i] = true;
                            lvNextCapacity++;
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                    }
                    else
                    {
                        lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);
                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            if (lvNextAvailable[i])
                            {
                                lvNextCapacity++;
                            }
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                    }
                }
                else
                {
                    if ((lvGene.StopLocation.PrevSwitch == null) || ((lvNextStopLocation.End_coordinate > lvGene.StopLocation.PrevSwitch.End_coordinate) && mDicTrain.ContainsKey(lvGene.TrainId)))
                    {
                        lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            lvNextAvailable[i] = true;
                            lvNextCapacity++;
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                    }
                    else
                    {
                        lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            if (lvNextAvailable[i])
                            {
                                lvNextCapacity++;
                            }
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                    }
                }
            }
            else
            {
                lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                lvNextCapacity = 0;
                for (int i = 0; i < lvNextAvailable.Length; i++)
                {
                    if (lvNextAvailable[i])
                    {
                        lvNextCapacity++;
                    }
                }

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                    DumpBoolArray(lvNextAvailable);
                }
#endif
            }

            if (lvNextCapacity == 0)
            {
#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvNextCapacity == 0 após VerifySegmentDeadLock !");
                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif

                return lvRes;
            }

            if (lvGene.SegmentInstance != null)
            {
                lvNextSwitch = lvGene.SegmentInstance.GetNextSwitchSegment(lvGene.Direction);
            }
            else
            {
                lvNextSwitch = Segment.GetNextSwitchSegment(lvGene.Coordinate, lvGene.Direction);
            }

            if ((lvGene.Track <= lvNextCapacity) && (((lvGene.Direction > 0) && ((lvNextSwitch == null) || (lvNextStopLocation.Start_coordinate < lvNextSwitch.Start_coordinate))) || ((lvGene.Direction < 0) && ((lvNextSwitch == null) || (lvNextStopLocation.End_coordinate > lvNextSwitch.End_coordinate)))))
            {
                lvNextCapacity = 0;
                for (int i = 0; i < lvNextAvailable.Length; i++)
                {
                    if (((lvGene.Track - 1) == i) && lvNextAvailable[i])
                    {
                        lvNextAvailable[i] = true;
                        lvNextCapacity++;
                    }
                    else
                    {
                        lvNextAvailable[i] = false;
                    }
                }

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("NextCapacity reduzida para 1 devido ao local atual do Gene ser ");
                    lvStrInfo.Append(lvGene.SegmentInstance.Location);
                    lvStrInfo.Append(".");
                    lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                    DumpBoolArray(lvNextAvailable);
                }
#endif

                if (mStopLocationOcupation.ContainsKey(lvNextStopLocationValue))
                {
                    foreach (Int64 lvTrainId in mStopLocationOcupation[lvNextStopLocationValue])
                    {
                        lvGen = mDicTrain[lvTrainId];

                        if ((lvGene.Track == lvGen.Track) && lvGene.TrainId != lvGen.TrainId)
                        {
                            lvNextCapacity = 0;
                            if (lvGene.Track <= lvNextAvailable.Length)
                            {
                                lvNextAvailable[lvGene.Track - 1] = false;

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    StringBuilder lvStrInfo = new StringBuilder();

                                    lvStrInfo.Clear();
                                    lvStrInfo.Append("NextCapacity reduzida em 1 devido ao local de destino (");
                                    lvStrInfo.Append(lvNextStopLocation);
                                    lvStrInfo.Append(") estar ocupada pelo Gene ");
                                    lvStrInfo.Append(lvGen.TrainId);
                                    lvStrInfo.Append(" - ");
                                    lvStrInfo.Append(lvGen.TrainName);
                                    lvStrInfo.Append(" em ");
                                    lvStrInfo.Append(lvGen.SegmentInstance.Location);
                                    lvStrInfo.Append(".");
                                    lvStrInfo.Append(lvGen.SegmentInstance.SegmentValue);
                                    lvStrInfo.Append(", track ");
                                    lvStrInfo.Append(lvGen.Track);

                                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                }
#endif
                            }

                            return lvRes;
                        }
                    }
                }
            }
            else
            {
                if (mStopLocationOcupation.ContainsKey(lvNextStopLocationValue))
                {
                    foreach (Int64 lvTrainId in mStopLocationOcupation[lvNextStopLocationValue])
                    {
                        lvGen = mDicTrain[lvTrainId];

                        if (lvGen.Track <= lvNextAvailable.Length)
                        {
                            if ((lvNextAvailable[lvGen.Track - 1] == true) && (lvGene.TrainId != lvGen.TrainId))
                            {
                                lvNextAvailable[lvGen.Track - 1] = false;
                                lvNextCapacity--;

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    StringBuilder lvStrInfo = new StringBuilder();

                                    lvStrInfo.Clear();
                                    lvStrInfo.Append("NextCapacity reduzida em 1 devido ao local de destino (");
                                    lvStrInfo.Append(lvNextStopLocation);
                                    lvStrInfo.Append(") estar ocupada pelo Gene ");
                                    lvStrInfo.Append(lvGen.TrainId);
                                    lvStrInfo.Append(" - ");
                                    lvStrInfo.Append(lvGen.TrainName);
                                    lvStrInfo.Append(" em ");
                                    lvStrInfo.Append(lvGen.SegmentInstance.Location);
                                    lvStrInfo.Append(".");
                                    lvStrInfo.Append(lvGen.SegmentInstance.SegmentValue);
                                    lvStrInfo.Append(", track ");
                                    lvStrInfo.Append(lvGen.Track);

                                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                }
#endif
                            }
                        }
                    }

                    if (lvNextCapacity == 0)
                    {
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("Retornando por lvNextCapacity == 0 após verificar próximo stop location");
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            DumpStopLocation(lvNextStopLocation);
                        }
#endif

                        return lvRes;
                    }
                }

                if (lvNextSwitch.AllowSameLineMov && (lvGene.Track <= lvNextAvailable.Length) && lvNextAvailable[lvGene.Track - 1])
                {
                    lvDestTrack = lvGene.Track;
                }
                else
                {
                    lvDestTrack = 0;
                }
            }

            if (lvNextCapacity > 0)
            {
                lvRes = true;
            }
            else
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

    private bool VerifyInconsistentStopLocationOcupation(Int64 pTrainId)
    {
        bool lvRes = false;
        List<int> lvStopLocationsFound = new List<int>();

        foreach (KeyValuePair<int, HashSet<Int64>> lvStopLocationEntry in mStopLocationOcupation)
        {
            if(lvStopLocationEntry.Value.Contains(pTrainId))
            {
                lvStopLocationsFound.Add(lvStopLocationEntry.Key);
            }
        }

        if(lvStopLocationsFound.Count > 1)
        {
            DebugLog.Logar("Trem " + pTrainId + " está nas stop locations: ", false, pIndet: TrainIndividual.IDLog);

            foreach(int lvStopLocationNum in lvStopLocationsFound)
            {
                DebugLog.Logar(lvStopLocationNum.ToString(), false, pIndet: TrainIndividual.IDLog);
            }
        }

        return lvRes;
    }
    */

    public IEnumerable<Gene> MoveTrain(TrainMovement pTrainMov, DateTime pInitialTime = default(DateTime), bool pUpdate = true)
    {
        TrainMovement lvRes = null;
        TrainMovement lvNoStopMovement = null;
        TrainMovement lvCurrentTrainMovement = null;
        bool[] lvNextAvailable = null;
        bool[] lvInternRes = null;
        HashSet<Int64> lvListGeneStopLocation = null;
        ISet<Gene> lvGenesStopLocationSet = null;
        ISet<int> lvDependencySet = null;
        ISet<int> lvNoEntranceSet = null;
        Gene lvNewGene = null;
        Gene lvGene = null;
        Gene lvGen = null;
        TrainPerformanceControl lvTrainPerformance = null;
        TrainPerformanceControl lvPrevTrainPerformance = null;
        StopLocation lvStopLocation = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvEndStopLocation = null;
        StopLocation lvStopLocationAfterSwitch = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSegment = null;
        Segment lvPrevNextSegment = null;
        Segment lvLocDepartureSegment = null;
        Segment lvNextSwitch = null;
        Segment lvNextSwitchAfterStopLocation = null;
        Interdicao lvInterdition = null;
        DateTime lvFirstEndInterdictionTime = DateTime.MaxValue;
        DateTime lvCurrentTime = DateTime.MinValue;
        DateTime lvDepTime;
        DateTime lvArrTime;
        List<Gene[]> lvGeneHeadwaysTime = new List<Gene[]>();
        DateTime[] lvArrLastDepTime;
        DateTime lvLastDepTime = DateTime.MinValue;
        double lvMeanSpeed = 0.0;
        double lvHeadWayTime = 0.0;
        double lvSpentTime = 0.0;
        double lvStayTime = 0.0;
        int lvStopLocationValue = 0;
        int lvNextStopLocationValue = 0;
        int lvInitCoordinate = Int32.MinValue;
        int lvEndCoordinate = Int32.MinValue;
        int lvNextCapacity = 0;
        int lvPATTime = 0;
        int lvIndex;
        int lvDistance;
        int lvCount;
        int lvDestTrack = 0;
        int lvLastOccupCount = 0;
        int lvSumDir = 0;
        int lvInterdCount = 0;
        bool[] lvLastOccup = null;
        bool lvHasSameDirection = false;
        bool lvIsBetweenSwitch = false;

        bool lvIsLogEnables = DebugLog.EnableDebug;

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            StringBuilder lvStrInfo = new StringBuilder();

            lvStrInfo.Clear();
            lvStrInfo.Append("------------------------------------------------ MoveTrain(");
            lvStrInfo.Append(pTrainMov.ToString());
            lvStrInfo.Append(") ---------------------------------------------------------");

            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
        }
#endif

        lvGene = pTrainMov.Last;

        if (lvGene == null) return lvRes;

        if (lvGene.DepartureTime > pInitialTime)
        {
#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("Abortando devido a lvGene.DepartureTime > pInitialTime (lvGene.DepartureTime: " + lvGene.DepartureTime + ", pInitialTime: " + pInitialTime + ")", pIndet: TrainIndividual.IDLog);
            }
#endif

            return lvRes;
        }

        if(mTrainFinished.Contains(lvGene.TrainId))
        {
            if (mDicTrain.ContainsKey(lvGene.TrainId))
            {
                mDicTrain.Remove(lvGene.TrainId);
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("Abortando devido a lvGene.TrainId(" + lvGene.TrainId + ") está em mTrainFinished.", pIndet: TrainIndividual.IDLog);
            }
#endif

            return lvRes;
        }

        try
        {
            if (mDicTrain.ContainsKey(lvGene.TrainId))
            {
                lvCurrentTrainMovement = mDicTrain[lvGene.TrainId];
                lvGene = lvCurrentTrainMovement.Last;

                if (pUpdate && (lvGene.Time == DateTime.MinValue))
                {
                    lvGene.Time = lvGene.DepartureTime;
                }

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("MoveTrain => Gene obtido de mDicTrain(");
                    lvStrInfo.Append(lvGene.ToString());
                    lvStrInfo.Append(")");

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
            }
            else
            {
                lvGene = pTrainMov[0];

                if (lvGene.StopLocation.Location == lvGene.StartStopLocation.Location)
                {
                    if (mStopLocationOcupation.ContainsKey(lvGene.StopLocation.Location))
                    {
                        foreach (Int64 lvTrainId in mStopLocationOcupation[lvGene.StopLocation.Location])
                        {
                            if (mDicTrain.ContainsKey(lvTrainId))
                            {
                                lvGen = mDicTrain[lvTrainId].Last;

                                if ((lvGene.Track == lvGen.Track) && lvGene.TrainId != lvGen.TrainId)
                                {
#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        StringBuilder lvStrInfo = new StringBuilder();

                                        lvStrInfo.Clear();
                                        lvStrInfo.Append("MoveTrain => pGene.StopLocation.Location == pGene.StartStopLocation.Location(");
                                        lvStrInfo.Append(lvGene.ToString());
                                        lvStrInfo.Append(") - no entanto já existe um trem (" + lvGen + ") circulando nesse local de parada");

                                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                    }
#endif

                                    return lvRes;
                                }
                            }
                        }
                    }

                    if (pUpdate && (lvGene.Time == DateTime.MinValue))
                    {
                        lvGene.Time = lvGene.DepartureTime;
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("MoveTrain => Gene aceito por pGene.StopLocation.Location == pGene.StartStopLocation.Location(");
                        lvStrInfo.Append(lvGene.ToString());
                        lvStrInfo.Append(")");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
                else
                {
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        DebugLog.Logar("Abortando devido ao Gene (" + lvGene + ") nao esta contido em mDicTrain e nao ter origem em sua Stop Location inicial (" + lvGene.StartStopLocation + ") e pInitialTime = " + pInitialTime, pIndet: TrainIndividual.IDLog);
                    }
#endif

                    return lvRes;
                }
            }

#if DEBUG
            DebugLog.EnableDebug = lvIsLogEnables;

            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("UniqueId: ");
                lvStrInfo.Append(mUniqueId);
                lvStrInfo.Append(" => ");
                lvStrInfo.Append("MoveTrain( ");
                lvStrInfo.Append(lvGene.ToString());
                lvStrInfo.Append(" ), pInitialTime = ");
                lvStrInfo.Append(pInitialTime);

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }

            DebugLog.EnableDebug = lvIsLogEnables;
#endif

            if (lvGene.StopLocation != null)
            {
                lvStopLocation = lvGene.StopLocation;
            }
            else
            {
                lvStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                if (pUpdate)
                {
                    lvGene.StopLocation = lvStopLocation;
                }
            }

            if (lvStopLocation != null)
            {
                lvNextStopLocation = lvStopLocation.GetNextStopSegment(lvGene.Direction);
                lvStopLocationValue = lvStopLocation.Location;
            }
            else
            {
                lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
            }

            lvEndStopLocation = lvGene.EndStopLocation;

            if (lvGene.SegmentInstance != null)
            {
                lvCurrentSegment = lvGene.SegmentInstance;
            }

            if (lvNextStopLocation == null)
            {
                mDicTrain.Remove(lvGene.TrainId);
                mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("Gene removido de mDicTrain(");
                    lvStrInfo.Append(lvGene.ToString());
                    lvStrInfo.Append(") ! MoveTrain => lvNextStopLocation == null");

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif

                RemoveGeneFromAllLocation(lvGene);
                return lvRes;
            }

            lvNextStopLocationValue = lvNextStopLocation.Location;

            if ((lvStopLocation == lvEndStopLocation) && (lvStopLocation != null))
            {
                mDicTrain.Remove(lvGene.TrainId);
                mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("Gene removido de mDicTrain(");
                    lvStrInfo.Append(lvGene.ToString());
                    lvStrInfo.Append(") ! MoveTrain => (lvStopLocation == lvEndStopLocation) && (lvStopLocation != null)");

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif

                lvListGeneStopLocation = mStopLocationOcupation[lvStopLocation.Location];
                lvListGeneStopLocation.Remove(lvGene.TrainId);

                return lvRes;
            }

            if (lvGene.Direction > 0)
            {
                if (lvStopLocation != null)
                {
                    if (lvStopLocationValue >= lvEndStopLocation.Location)
                    {
                        mDicTrain.Remove(lvGene.TrainId);
                        mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("Gene removido de mDicTrain(");
                            lvStrInfo.Append(lvGene.ToString());
                            lvStrInfo.Append(") ! MoveTrain => lvStopLocation.Location >= lvEndStopLocation.Location");

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif

                        RemoveGeneFromAllLocation(lvGene);
                        return lvRes;
                    }
                }
                else if (lvGene.Coordinate >= lvEndStopLocation.Start_coordinate)
                {
                    mDicTrain.Remove(lvGene.TrainId);
                    mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("Gene removido de mDicTrain(");
                        lvStrInfo.Append(lvGene.ToString());
                        lvStrInfo.Append(") ! MoveTrain => lvGene.Coordinate >= lvEndStopLocation.Start_coordinate");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif

                    RemoveGeneFromAllLocation(lvGene);
                    return lvRes;
                }
            }
            else
            {
                if (lvStopLocation != null)
                {
                    if (lvStopLocationValue <= lvEndStopLocation.Location)
                    {
                        mDicTrain.Remove(lvGene.TrainId);
                        mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("Gene removido de mDicTrain(");
                            lvStrInfo.Append(lvGene.ToString());
                            lvStrInfo.Append(") ! MoveTrain => lvStopLocation.Location <= lvEndStopLocation.Location");

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif

                        RemoveGeneFromAllLocation(lvGene);
                        return lvRes;
                    }
                }
                if (lvGene.Coordinate <= lvEndStopLocation.End_coordinate)
                {
                    mDicTrain.Remove(lvGene.TrainId);
                    mTrainFinished.Add(lvGene.TrainId);

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("Gene removido de mDicTrain(");
                        lvStrInfo.Append(lvGene.ToString());
                        lvStrInfo.Append(") ! MoveTrain => lvGene.Coordinate <= lvEndStopLocation.End_coordinate");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif

                    RemoveGeneFromAllLocation(lvGene);
                    return lvRes;
                }
            }

            if (lvCurrentSegment == null)
            {
                lvCurrentSegment = Segment.GetNextSegment(lvGene.Coordinate, lvGene.Direction);
            }

            lvNextSwitch = lvNextStopLocation.GetNextSwitchSegment(lvGene.Direction);

            if (lvStopLocation != null)
            {
                if (lvGene.Direction > 0)
                {
                    if ((lvStopLocation.NextSwitch == null) || ((lvNextStopLocation.Start_coordinate < lvStopLocation.NextSwitch.Start_coordinate) && mDicTrain.ContainsKey(lvGene.TrainId)))
                    {
                        lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            if ((lvGene.Track - 1) == i)
                            {
                                lvNextAvailable[i] = true;
                                lvNextCapacity++;
                            }
                            else
                            {
                                lvNextAvailable[i] = false;
                            }
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("NextCapacity reduzida para 1 devido ao local atual do Gene ser ");
                            lvStrInfo.Append(lvGene.SegmentInstance.Location);
                            lvStrInfo.Append(".");
                            lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            DumpBoolArray(lvNextAvailable);
                        }
#endif

                        lvIsBetweenSwitch = false;
                    }
                    else
                    {
                        if (lvStopLocation.NextSwitch != null)
                        {
                            lvNoEntranceSet = lvStopLocation.NextSwitch.GetNoEntrance(lvGene.Track, lvGene.Direction);

                            if ((lvNoEntranceSet != null) && (lvNoEntranceSet.Count > 0))
                            {
                                lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                                lvNextCapacity = 0;
                                foreach (int lvNoEntranceTrack in lvNoEntranceSet)
                                {
                                    if (lvNextAvailable[lvNoEntranceTrack - 1])
                                    {
                                        lvNextAvailable[lvNoEntranceTrack - 1] = false;
                                        lvNextCapacity--;
                                    }
                                }

                                if (lvNextCapacity == 1)
                                {
#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        StringBuilder lvStrInfo = new StringBuilder();

                                        lvStrInfo.Clear();
                                        lvStrInfo.Append("lvNextCapacity == 1 após verificar lvNoEntranceSet !");
                                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                                        DumpBoolArray(lvNextAvailable);
                                    }
#endif

                                    lvIsBetweenSwitch = false;
                                }
                                else
                                {
                                    lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);
                                    lvNextCapacity = 0;
                                    for (int i = 0; i < lvNextAvailable.Length; i++)
                                    {
                                        if (lvNextAvailable[i])
                                        {
                                            lvNextCapacity++;
                                        }
                                    }

                                    lvIsBetweenSwitch = true;
#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                                        DumpBoolArray(lvNextAvailable);
                                    }
#endif
                                }
                            }
                            else
                            {
                                lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);
                                lvNextCapacity = 0;
                                for (int i = 0; i < lvNextAvailable.Length; i++)
                                {
                                    if (lvNextAvailable[i])
                                    {
                                        lvNextCapacity++;
                                    }
                                }

                                lvIsBetweenSwitch = true;
#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                                    DumpBoolArray(lvNextAvailable);
                                }
#endif
                            }
                        }
                    }
                }
                else
                {
                    if ((lvStopLocation.PrevSwitch == null) || ((lvNextStopLocation.End_coordinate > lvStopLocation.PrevSwitch.End_coordinate) && mDicTrain.ContainsKey(lvGene.TrainId)))
                    {
                        lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                        lvNextCapacity = 0;
                        for (int i = 0; i < lvNextAvailable.Length; i++)
                        {
                            if ((lvGene.Track - 1) == i)
                            {
                                lvNextAvailable[i] = true;
                                lvNextCapacity++;
                            }
                            else
                            {
                                lvNextAvailable[i] = false;
                            }
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("NextCapacity reduzida para 1 devido ao local atual do Gene ser ");
                            lvStrInfo.Append(lvGene.SegmentInstance.Location);
                            lvStrInfo.Append(".");
                            lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            DumpBoolArray(lvNextAvailable);
                        }
#endif

                        lvIsBetweenSwitch = false;
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                    }
                    else
                    {
                        if (lvStopLocation.PrevSwitch != null)
                        {
                            lvNoEntranceSet = lvStopLocation.PrevSwitch.GetNoEntrance(lvGene.Track, lvGene.Direction);

                            if ((lvNoEntranceSet != null) && (lvNoEntranceSet.Count > 0))
                            {
                                lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                                lvNextCapacity = 0;
                                foreach (int lvNoEntranceTrack in lvNoEntranceSet)
                                {
                                    if (lvNextAvailable[lvNoEntranceTrack - 1])
                                    {
                                        lvNextAvailable[lvNoEntranceTrack - 1] = false;
                                        lvNextCapacity--;
                                    }
                                }

                                if (lvNextCapacity == 1)
                                {
#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        StringBuilder lvStrInfo = new StringBuilder();

                                        lvStrInfo.Clear();
                                        lvStrInfo.Append("lvNextCapacity == 1 após verificar lvNoEntranceSet !");
                                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                                        DumpBoolArray(lvNextAvailable);
                                    }
#endif

                                    lvIsBetweenSwitch = false;
                                }
                                else
                                {

                                    lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                                    lvNextCapacity = 0;
                                    for (int i = 0; i < lvNextAvailable.Length; i++)
                                    {
                                        if (lvNextAvailable[i])
                                        {
                                            lvNextCapacity++;
                                        }
                                    }

                                    lvIsBetweenSwitch = true;
#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                                        DumpBoolArray(lvNextAvailable);
                                    }
#endif
                                }
                            }
                            else
                            {
                                lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                                lvNextCapacity = 0;
                                for (int i = 0; i < lvNextAvailable.Length; i++)
                                {
                                    if (lvNextAvailable[i])
                                    {
                                        lvNextCapacity++;
                                    }
                                }

                                lvIsBetweenSwitch = true;
#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                                    DumpBoolArray(lvNextAvailable);
                                }
#endif
                            }
                        }
                    }
                }
            }
            else
            {
                lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation, lvNextSwitch, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                lvNextCapacity = 0;
                for (int i = 0; i < lvNextAvailable.Length; i++)
                {
                    if (lvNextAvailable[i])
                    {
                        lvNextCapacity++;
                    }
                }

                lvIsBetweenSwitch = true;

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvNextCapacity = " + lvNextCapacity + " devido a VerifySegmentDeadLock", pIndet: TrainIndividual.IDLog);
                    DumpBoolArray(lvNextAvailable);
                }
#endif
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("lvStopLocation = " + lvStopLocation);
                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                lvStrInfo.Clear();
                lvStrInfo.Append("lvNextStopLocation = " + lvNextStopLocation);
                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                lvStrInfo.Clear();
                lvStrInfo.Append("lvNextSwitch = " + lvNextSwitch);
                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }
#endif

            if (lvNextCapacity == 0)
            {
#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvNextCapacity == 0 após VerifySegmentDeadLock !");
                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif

                return lvRes;
            }

            /* Obtem o segmento de partida do Gene */
            if (lvStopLocation != null)
            {
                if (lvGene.Track > lvStopLocation.Capacity)
                {
                    lvLocDepartureSegment = lvCurrentSegment;
                }
                else
                {
                    lvLocDepartureSegment = lvStopLocation.GetSegment(lvGene.Direction, lvGene.Track);

                    if(lvLocDepartureSegment == null)
                    {
                        lvLocDepartureSegment = lvCurrentSegment;
                    }
                }
            }
            else
            {
                lvLocDepartureSegment = lvCurrentSegment;
            }
            /* ------------------------------------  */

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                DebugLog.Logar("lvLocDepartureSegment = " + lvLocDepartureSegment, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" --------- MoveTrain ---------- ", pIndet: TrainIndividual.IDLog);

                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("pInitialTime para Gene (");
                lvStrInfo.Append(lvGene.TrainId);
                lvStrInfo.Append(" - ");
                lvStrInfo.Append(lvGene.TrainName);
                lvStrInfo.Append("; CurrentTime: ");
                lvStrInfo.Append(lvGene.Time);
                lvStrInfo.Append(") pInitialTime = ");
                lvStrInfo.Append(pInitialTime);

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                DebugLog.Logar("lvStopLocation = " + lvStopLocation, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("lvNextStopLocation = " + lvNextStopLocation, pIndet: TrainIndividual.IDLog);
            }
#endif

            if (lvGene.SegmentInstance != null)
            {
                lvNextSwitch = lvGene.SegmentInstance.GetNextSwitchSegment(lvGene.Direction);
            }
            else
            {
                lvNextSwitch = Segment.GetNextSwitchSegment(lvGene.Coordinate, lvGene.Direction);
            }

            lvCurrentTime = lvGene.HeadWayTime;

            if ((lvGene.Time > lvGene.HeadWayTime) && (lvGene.Time > lvCurrentTime))
            {
                lvCurrentTime = lvGene.Time;

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvGene.Time em lvCurrentTime do Gene (");
                    lvStrInfo.Append(lvGene.TrainId);
                    lvStrInfo.Append(" - ");
                    lvStrInfo.Append(lvGene.TrainName);
                    lvStrInfo.Append(") = ");
                    lvStrInfo.Append(lvGene.Time);
                    lvStrInfo.Append(" por ser maior que o headway atual ");
                    lvStrInfo.Append(lvGene.HeadWayTime);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
            }
            else if ((lvGene.DepartureTime > lvGene.HeadWayTime) && (lvGene.HeadWayTime > lvCurrentTime))
            {
                lvCurrentTime = lvGene.HeadWayTime;

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvGene.DepartureTime em lvCurrentTime do Gene (");
                    lvStrInfo.Append(lvGene.TrainId);
                    lvStrInfo.Append(" - ");
                    lvStrInfo.Append(lvGene.TrainName);
                    lvStrInfo.Append(") = ");
                    lvStrInfo.Append(lvGene.DepartureTime);
                    lvStrInfo.Append(" por ser maior que o headway atual ");
                    lvStrInfo.Append(lvGene.HeadWayTime);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
            }

            DebugLog.EnableDebug = lvIsLogEnables;
            lvArrLastDepTime = GetCurrentFirstOutputTime(lvGene, lvNextStopLocation);
            DebugLog.EnableDebug = lvIsLogEnables;

            lvInterdition = null;
            if ((lvGene.Track <= lvNextCapacity) && !lvIsBetweenSwitch)
            {
                if (mStopLocationOcupation.ContainsKey(lvNextStopLocationValue))
                {
                    foreach (Int64 lvTrainId in mStopLocationOcupation[lvNextStopLocationValue])
                    {
                        if (mDicTrain.ContainsKey(lvTrainId))
                        {
                            lvGen = mDicTrain[lvTrainId].Last;

                            if ((lvGene.Track == lvGen.Track) && lvGene.TrainId != lvGen.TrainId)
                            {
                                if (lvGene.Track < lvNextAvailable.Length)
                                {
                                    lvNextCapacity = 0;
                                    lvNextAvailable[lvGene.Track - 1] = false;

#if DEBUG
                                    if (DebugLog.EnableDebug)
                                    {
                                        StringBuilder lvStrInfo = new StringBuilder();

                                        lvStrInfo.Clear();
                                        lvStrInfo.Append("NextCapacity reduzida em 1 devido ao local de destino (");
                                        lvStrInfo.Append(lvNextStopLocation);
                                        lvStrInfo.Append(") estar ocupada pelo Gene ");
                                        lvStrInfo.Append(lvGen.TrainId);
                                        lvStrInfo.Append(" - ");
                                        lvStrInfo.Append(lvGen.TrainName);
                                        lvStrInfo.Append(" em ");
                                        lvStrInfo.Append(lvGen.SegmentInstance.Location);
                                        lvStrInfo.Append(".");
                                        lvStrInfo.Append(lvGen.SegmentInstance.SegmentValue);
                                        lvStrInfo.Append(", track ");
                                        lvStrInfo.Append(lvGen.Track);

                                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                    }
#endif
                                }

                                return lvRes;
                            }
                        }
                    }
                }

                if (lvNextCapacity == 1)
                {
                    if (lvArrLastDepTime != null)
                    {
                        lvDestTrack = lvGene.Track;
                        lvLastDepTime = lvArrLastDepTime[lvDestTrack-1];

                        DebugLog.EnableDebug = lvIsLogEnables;
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvArrLastDepTime[");
                            lvStrInfo.Append(lvDestTrack);
                            lvStrInfo.Append("] = ");
                            lvStrInfo.Append(lvArrLastDepTime[lvDestTrack-1]);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                        DebugLog.EnableDebug = lvIsLogEnables;
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvCurrentTime = ");
                        lvStrInfo.Append(lvCurrentTime);
                        lvStrInfo.Append("; lvLastDepTime = ");
                        lvStrInfo.Append(lvLastDepTime);

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif

                    if ((lvLastDepTime > lvGene.HeadWayTime) && (lvLastDepTime > lvCurrentTime) && (lvLastDepTime < DateTime.MaxValue))
                    {
                        lvCurrentTime = lvLastDepTime;
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvLastDepTime em lvCurrentTime do Gene (");
                            lvStrInfo.Append(lvGene.TrainId);
                            lvStrInfo.Append(" - ");
                            lvStrInfo.Append(lvGene.TrainName);
                            lvStrInfo.Append(") = ");
                            lvStrInfo.Append(lvLastDepTime);
                            lvStrInfo.Append(" por ser maior que o headway atual ");
                            lvStrInfo.Append(lvGene.HeadWayTime);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }

                    if (Interdicao.GetList().Count > 0)
                    {
                        lvInterdition = GetInterdiction(lvNextStopLocation.Start_coordinate, lvNextStopLocation.End_coordinate, lvGene.Time, lvGene.Track);

                        if (lvInterdition != null)
                        {
                            if (lvInterdition.End_time > lvCurrentTime)
                            {
                                lvCurrentTime = lvInterdition.End_time;
                            }
                        }
                    }
                }
            }
            else
            {
                if (mStopLocationOcupation.ContainsKey(lvNextStopLocationValue))
                {
                    foreach (Int64 lvTrainId in mStopLocationOcupation[lvNextStopLocationValue])
                    {
                        if (mDicTrain.ContainsKey(lvTrainId))
                        {
                            lvGen = mDicTrain[lvTrainId].Last;

                            if (lvGen.Track <= lvNextAvailable.Length)
                            {
                                if (lvGene.TrainId != lvGen.TrainId)
                                {
                                    if (lvNextAvailable[lvGen.Track - 1] && (lvGene.TrainId != lvGen.TrainId))
                                    {
                                        lvNextAvailable[lvGen.Track - 1] = false;
                                        lvNextCapacity--;

#if DEBUG
                                        if (DebugLog.EnableDebug)
                                        {
                                            StringBuilder lvStrInfo = new StringBuilder();

                                            lvStrInfo.Clear();
                                            lvStrInfo.Append("NextCapacity reduzida em 1 devido ao local de destino (");
                                            lvStrInfo.Append(lvNextStopLocation);
                                            lvStrInfo.Append(") estar ocupada pelo Gene ");
                                            lvStrInfo.Append(lvGen.TrainId);
                                            lvStrInfo.Append(" - ");
                                            lvStrInfo.Append(lvGen.TrainName);
                                            lvStrInfo.Append(" em ");
                                            lvStrInfo.Append(lvGen.SegmentInstance.Location);
                                            lvStrInfo.Append(".");
                                            lvStrInfo.Append(lvGen.SegmentInstance.SegmentValue);
                                            lvStrInfo.Append(", track ");
                                            lvStrInfo.Append(lvGen.Track);

                                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                        }
#endif
                                    }

                                    lvDependencySet = lvNextStopLocation.HasDependency(lvGen.Track, lvGene.Direction);

                                    if (lvDependencySet != null)
                                    {
                                        foreach (int lvTrackNum in lvDependencySet)
                                        {
                                            if (lvNextAvailable[lvTrackNum - 1])
                                            {
                                                lvNextAvailable[lvTrackNum - 1] = false;
                                                lvNextCapacity--;
                                            }
                                        }

                                        if (lvNextCapacity == 0)
                                        {
#if DEBUG
                                            if (DebugLog.EnableDebug)
                                            {
                                                StringBuilder lvStrInfo = new StringBuilder();

                                                lvStrInfo.Clear();
                                                lvStrInfo.Append("lvNextCapacity == 0 após verificar lvDependencySet !");
                                                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                            }
#endif

                                            return lvRes;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (lvNextCapacity == 0)
                    {
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("Retornando por lvNextCapacity == 0 após verificar próximo stop location");
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            DumpStopLocation(lvNextStopLocation);
                        }
#endif

                        return lvRes;
                    }
                }

                if (lvArrLastDepTime != null)
                {
                    lvLastDepTime = DateTime.MaxValue;
                    for (int i = 0; i < lvArrLastDepTime.Length; i++)
                    {
                        if ((lvArrLastDepTime[i] < lvLastDepTime) && lvNextAvailable[i])
                        {
                            lvDestTrack = i + 1;
                            lvLastDepTime = lvArrLastDepTime[i];
                        }

                        DebugLog.EnableDebug = lvIsLogEnables;
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvArrLastDepTime[");
                            lvStrInfo.Append(i);
                            lvStrInfo.Append("] = ");
                            lvStrInfo.Append(lvArrLastDepTime[i]);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                        DebugLog.EnableDebug = lvIsLogEnables;

                    }
                }

                DebugLog.EnableDebug = lvIsLogEnables;
#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvCurrentTime = ");
                    lvStrInfo.Append(lvCurrentTime);
                    lvStrInfo.Append("; lvLastDepTime = ");
                    lvStrInfo.Append(lvLastDepTime);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
                DebugLog.EnableDebug = lvIsLogEnables;

                if ((lvLastDepTime > lvGene.HeadWayTime) && (lvLastDepTime > lvCurrentTime) && (lvLastDepTime < DateTime.MaxValue))
                {
                    lvCurrentTime = lvLastDepTime;
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvLastDepTime em lvCurrentTime do Gene (");
                        lvStrInfo.Append(lvGene.TrainId);
                        lvStrInfo.Append(" - ");
                        lvStrInfo.Append(lvGene.TrainName);
                        lvStrInfo.Append(") = ");
                        lvStrInfo.Append(lvLastDepTime);
                        lvStrInfo.Append(" por ser maior que o headway atual ");
                        lvStrInfo.Append(lvGene.HeadWayTime);

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }

                if ((lvDestTrack == 0) && (lvNextSwitch != null) && lvNextSwitch.AllowSameLineMov && (lvGene.Track <= lvNextAvailable.Length) && lvNextAvailable[lvGene.Track - 1])
                {
                    lvDestTrack = lvGene.Track;
                }
                else
                {
                    lvDestTrack = 0;
                }

                DebugLog.EnableDebug = lvIsLogEnables;

                if (lvStopLocation != null)
                {
                    GetHeadWays(lvGene, lvStopLocation, lvGene.Direction, ref lvGeneHeadwaysTime);
                }

                if (lvNextStopLocation != null)
                {
                    GetHeadWays(lvGene, lvNextStopLocation, lvGene.Direction * (-1), ref lvGeneHeadwaysTime);
                }

                DebugLog.EnableDebug = lvIsLogEnables;

                if (lvGeneHeadwaysTime.Count > 1)
                {
                    lvGeneHeadwaysTime.Sort(new HeadWayGeneTimeComparer());
                }
            }

            lvPATTime = GetPAT(lvStopLocation, lvGene.TrainId);

            if (lvPATTime > 0)
            {
                if (lvGene.HeadWayTime.AddMinutes(lvPATTime) > lvCurrentTime)
                {
                    lvCurrentTime = lvGene.Time.AddMinutes(lvPATTime);

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvCurrentTime atualizado por lvPATTime para Gene (");
                        lvStrInfo.Append(lvGene.TrainId);
                        lvStrInfo.Append(" - ");
                        lvStrInfo.Append(lvGene.TrainName);
                        lvStrInfo.Append(") = ");
                        lvStrInfo.Append(lvCurrentTime);

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
            }

            if (Interdicao.GetList().Count > 0)
            {
                if (lvGene.Direction > 0)
                {
                    if (lvStopLocation != null)
                    {
                        lvInterdition = GetInterdiction(lvStopLocation.End_coordinate, lvNextStopLocation.Start_coordinate, lvCurrentTime, 1);
                    }
                    else
                    {
                        lvInterdition = GetInterdiction(lvGene.Coordinate, lvNextStopLocation.Start_coordinate, lvCurrentTime, 1);
                    }
                }
                else
                {
                    if (lvStopLocation != null)
                    {
                        lvInterdition = GetInterdiction(lvNextStopLocation.End_coordinate, lvStopLocation.Start_coordinate, lvCurrentTime, 1);
                    }
                    else
                    {
                        lvInterdition = GetInterdiction(lvNextStopLocation.End_coordinate, lvGene.Coordinate, lvCurrentTime, 1);
                    }
                }

                if ((lvInterdition != null) && (lvInterdition.End_time > lvCurrentTime))
                {
                    lvCurrentTime = lvInterdition.End_time;

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvCurrentTime atualizado por lvEndInterditionTime para Gene (");
                        lvStrInfo.Append(lvGene.TrainId);
                        lvStrInfo.Append(" - ");
                        lvStrInfo.Append(lvGene.TrainName);
                        lvStrInfo.Append(") = ");
                        lvStrInfo.Append(lvCurrentTime);

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
            }

            /*
            if (lvCurrentTime > mDateRef.AddDays(mLimitDays))
            {
                if (mDicTrain.ContainsKey(lvGene.TrainId))
                {
                    mDicTrain.Remove(lvGene.TrainId);
                }
                return lvRes;
            }
            */

            if (lvGene.DepartureTime == lvGene.Time)
            {
                lvSpentTime = 0.0;
            }
            else
            {
                /* Tempo de permancencia no Stop Location */
                if (lvStopLocation != null)
                {
                    if (lvGene.Speed > mMinSpeedLimit)
                    {
                        lvSpentTime = (Math.Abs(lvStopLocation.Start_coordinate - lvStopLocation.End_coordinate) / 100000.0) / lvGene.Speed;
                    }
                    else
                    {
                        lvSpentTime = (Math.Abs(lvStopLocation.Start_coordinate - lvStopLocation.End_coordinate) / 100000.0) / mMinSpeedLimit;
                    }
                }
                else
                {
                    if (lvGene.Speed > mMinSpeedLimit)
                    {
                        lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / lvGene.Speed;
                    }
                    else
                    {
                        lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / mMinSpeedLimit;
                    }
                }
            }

            if (lvCurrentTime < mDateRef)
            {
                lvCurrentTime = mDateRef;

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvCurrentTime atualizado por mDateRef para Gene (");
                    lvStrInfo.Append(lvGene.TrainId);
                    lvStrInfo.Append(" - ");
                    lvStrInfo.Append(lvGene.TrainName);
                    lvStrInfo.Append(") = ");
                    lvStrInfo.Append(lvCurrentTime);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif
            }

            if (lvLocDepartureSegment != null)
            {
                if (lvGene.Direction > 0)
                {
                    lvInitCoordinate = lvLocDepartureSegment.Start_coordinate;
                }
                else
                {
                    lvInitCoordinate = lvLocDepartureSegment.End_coordinate;
                }
            }
            else
            {
                lvLocDepartureSegment = lvStopLocation.GetSegment(lvGene.Direction, lvGene.Track);

                if (lvGene.Direction > 0)
                {
                    lvInitCoordinate = lvLocDepartureSegment.Start_coordinate;
                }
                else
                {
                    lvInitCoordinate = lvLocDepartureSegment.End_coordinate;
                }
            }

            lvLastOccupCount = lvNextCapacity;
            lvInterdCount = 0;

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("lvLastOccupCount = ");
                lvStrInfo.Append(lvLastOccupCount);

                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }
#endif

            for (int i = 0; i < lvNextAvailable.Length; i++)
            {
                if (lvNextAvailable[i])
                {
                    if (Interdicao.GetList().Count > 0)
                    {
                        lvInterdition = GetInterdiction(lvNextStopLocation.Start_coordinate, lvNextStopLocation.End_coordinate, lvCurrentTime, i + 1);

                        if (lvInterdition != null)
                        {
                            if (lvInterdition.End_time >= lvCurrentTime)
                            {
                                lvLastOccupCount--;
                                lvInterdCount++;

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    StringBuilder lvStrInfo = new StringBuilder();

                                    lvStrInfo.Clear();
                                    lvStrInfo.Append("lvLastOccupCount = ");
                                    lvStrInfo.Append(lvLastOccupCount);
                                    lvStrInfo.Append("; lvInterdCount = ");
                                    lvStrInfo.Append(lvInterdCount);

                                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                }
#endif

                                if (lvInterdition.End_time < lvFirstEndInterdictionTime)
                                {
                                    lvFirstEndInterdictionTime = lvInterdition.End_time;
                                }

                                if (lvLastOccupCount <= 0)
                                {
                                    if ((lvFirstEndInterdictionTime > lvCurrentTime) && (lvFirstEndInterdictionTime < DateTime.MaxValue))
                                    {
                                        lvCurrentTime = lvFirstEndInterdictionTime;

#if DEBUG
                                        if (DebugLog.EnableDebug)
                                        {
                                            StringBuilder lvStrInfo = new StringBuilder();

                                            lvStrInfo.Clear();
                                            lvStrInfo.Append("lvCurrentTime foi adiado para lvFirstEndInterdictionTime(");
                                            lvStrInfo.Append(lvFirstEndInterdictionTime);
                                            lvStrInfo.Append(") devido interdicao ");
                                            lvStrInfo.Append(lvInterdition.Ti_id);

                                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                        }
#endif
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("lvSpentTime = ");
                lvStrInfo.Append(lvSpentTime);
                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }
#endif

            lvStayTime = (lvCurrentTime - lvGene.Time).TotalHours;

            if (lvSpentTime < lvStayTime)
            {
                lvSpentTime = lvStayTime;
            }

#if DEBUG
            if (DebugLog.EnableDebug)
            {
                StringBuilder lvStrInfo = new StringBuilder();

                lvStrInfo.Clear();
                lvStrInfo.Append("lvStayTime = ");
                lvStrInfo.Append(lvStayTime);
                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
            }
#endif

            if ((lvNextCapacity == 1) && ((lvHasSameDirection) || (lvNextStopLocation.Capacity - (lvSumDir * lvGene.Direction + lvInterdCount) == lvLastOccupCount)))
            {
                lvLastOccupCount = 0;
                lvStopLocationAfterSwitch = null;
                if (lvNextSwitch != null)
                {
                    lvStopLocationAfterSwitch = lvNextSwitch.GetNextStopLocation(lvGene.Direction);
                }

                if (lvStopLocationAfterSwitch != null)
                {
                    lvNextSwitchAfterStopLocation = lvStopLocationAfterSwitch.GetNextSwitchSegment(lvGene.Direction);

                    if (lvLastOccup == null)
                    {
                        VerifyNextSegment(lvGene, lvStopLocationAfterSwitch, lvNextSwitchAfterStopLocation, true, out lvHasSameDirection, out lvSumDir, out lvLastOccup, out lvLastOccupCount);

                        if((lvSumDir * lvGene.Direction * -1) >= lvStopLocationAfterSwitch.Capacity)
                        {
                            lvLastOccupCount = lvStopLocationAfterSwitch.Capacity;

#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                StringBuilder lvStrInfo = new StringBuilder();

                                lvStrInfo.Clear();
                                lvStrInfo.Append("NextCapacity reduzida para 0 para evitar DeadLocks por causa de elementos finais estarem em sentido contra e sem lvHasSameDirection !!!");

                                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                            }
#endif
                        }
                    }

                    if((lvLastOccupCount < lvStopLocationAfterSwitch.Capacity) && (lvNextSwitchAfterStopLocation != null))
                    {
                        for (int i = 0; i < lvStopLocationAfterSwitch.Capacity; i++)
                        {
                            if ((i < lvLastOccup.Length) && lvLastOccup[i])
                            {
                                lvLastOccupCount++;
                            }
                            else
                            {
                                if (Interdicao.GetList().Count > 0)
                                {
                                    if (lvGene.Direction > 0)
                                    {
                                        lvInterdition = GetInterdiction(lvStopLocationAfterSwitch.Start_coordinate, lvNextSwitchAfterStopLocation.Start_coordinate, lvCurrentTime, i + 1);
                                    }
                                    else
                                    {
                                        lvInterdition = GetInterdiction(lvStopLocationAfterSwitch.End_coordinate, lvNextSwitchAfterStopLocation.End_coordinate, lvCurrentTime, i + 1);
                                    }

                                    if (lvInterdition != null)
                                    {
                                        if (lvInterdition.End_time >= lvCurrentTime)
                                        {
                                            lvLastOccupCount++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(lvLastOccupCount >= lvStopLocationAfterSwitch.Capacity)
                    {
                        lvNextAvailable = new bool[lvNextStopLocation.Capacity];
                        lvNextCapacity = 0;

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("NextCapacity reduzida para 0 para evitar DeadLocks por causa de interdições");

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }
                    else if((lvStopLocationAfterSwitch.NoStopSet != null) && (lvStopLocationAfterSwitch.NoStopSet.Count > 0))
                    {
                        if(lvStopLocationAfterSwitch.HasNoStop(lvGene.TrainName.Substring(0, 1) + lvGene.Direction))
                        {
                            lvInternRes = VerifySegmentDeadLock(lvGene, lvStopLocationAfterSwitch, lvNextSwitchAfterStopLocation, out lvHasSameDirection, out lvSumDir, out lvLastOccup);

                            lvCount = 0;

                            foreach(bool lvValue in lvInternRes)
                            {
                                if(lvValue)
                                {
                                    break;
                                }

                                lvCount++;
                            }

                            if (lvCount >= lvStopLocation.Capacity)
                            {
                                return lvRes;
                            }
                        }
                        else
                        {
                            if ((lvGene.Direction > 0) && lvStopLocationAfterSwitch.HasBackwardDirection)
                            {
                                return lvRes;
                            }
                            else if ((lvGene.Direction < 0) && lvStopLocationAfterSwitch.HasForwardDirection)
                            {
                                return lvRes;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < lvNextAvailable.Length; i++)
            {
                if (lvNextAvailable[i])
                {
                    lvNextSegment = lvNextStopLocation.GetSegment(lvGene.Direction * (-1), (i + 1));

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvNextStopLocation = " + lvNextStopLocation);
                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvNextSegment = " + lvNextSegment);
                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif

                    if (lvNextSegment == null)
                    {
                        lvNextSegment = lvNextStopLocation.GetSegment(lvGene.Direction * (-1), (i + 1));
                    }

                    if ((lvNextSegment != null) && (lvGene.TrainName.Length > 0))
                    {
                        lvTrainPerformance = TrainPerformanceControl.GetElementByKey(lvGene.TrainName.Substring(0, 1), lvGene.Direction, lvGene.SegmentInstance.Location, lvGene.SegmentInstance.SegmentValue, (i + 1));

                        if (lvPrevTrainPerformance != null && lvTrainPerformance != null)
                        {
                            if (lvGene.Speed > mMinSpeedLimit)
                            {
                                if (lvTrainPerformance.TimeHeadWayMov < lvPrevTrainPerformance.TimeHeadWayMov)
                                {
                                    lvTrainPerformance = lvPrevTrainPerformance;
                                    lvNextSegment = lvPrevNextSegment;
                                }
                            }
                            else
                            {
                                if (lvTrainPerformance.TimeHeadWayStop < lvPrevTrainPerformance.TimeHeadWayStop)
                                {
                                    lvTrainPerformance = lvPrevTrainPerformance;
                                    lvNextSegment = lvPrevNextSegment;
                                }
                            }
                        }

                        if (lvTrainPerformance != null)
                        {
                            lvPrevTrainPerformance = lvTrainPerformance;
                            lvPrevNextSegment = lvNextSegment;
                        }
                    }
                }
            }

#if DEBUG
            DumpStopLocation(lvNextStopLocation);
#endif

            if (lvNextCapacity > 0)
            {
                if (lvGene.Direction > 0)
                {
                    lvEndCoordinate = lvLocDepartureSegment.End_coordinate;
                }
                else
                {
                    lvEndCoordinate = lvLocDepartureSegment.Start_coordinate;
                }

                if ((lvDestTrack == 0) || (!lvNextAvailable[lvDestTrack - 1]))
                {
                    lvLastDepTime = DateTime.MaxValue;
                    for (int i = 0; i < lvNextAvailable.Length; i++)
                    {
                        if (lvNextAvailable[i])
                        {
                            if (lvArrLastDepTime[i] < lvLastDepTime)
                            {
                                lvDestTrack = i + 1;
                                lvLastDepTime = lvArrLastDepTime[i];
                            }
                        }
                    }

                    if (lvLastDepTime > lvCurrentTime)
                    {
                        lvCurrentTime = lvLastDepTime;
                    }
                }

                DebugLog.EnableDebug = lvIsLogEnables;

                lvDistance = Math.Abs(lvEndCoordinate - lvInitCoordinate);
                lvMeanSpeed = GetJourneyTimes(lvGene, lvDistance, lvNextStopLocation, lvEndCoordinate, lvSpentTime, out lvDepTime, out lvArrTime);

                DebugLog.EnableDebug = lvIsLogEnables;

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvHeadwaysTime.Count = " + lvGeneHeadwaysTime.Count, pIndet: TrainIndividual.IDLog);
                }
#endif
                for (int ind = 0; ind < lvGeneHeadwaysTime.Count; ind++)
                {
                    if ((lvGeneHeadwaysTime[ind] != null) && (lvGeneHeadwaysTime[ind].Length == 2))
                    {
                        DebugLog.EnableDebug = lvIsLogEnables;
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("lvHeadwaysTime[" + ind + "] = " + lvGeneHeadwaysTime[ind][0] + " - " + lvGeneHeadwaysTime[ind][1], pIndet: TrainIndividual.IDLog);
                        }
#endif

                        //if (((lvGeneHeadwaysTime[ind][0].Time < lvArrTime) && (lvGeneHeadwaysTime[ind][1].HeadWayTime > lvDepTime)) || ((lvGeneHeadwaysTime[ind][0].Time > lvDepTime) && (lvGeneHeadwaysTime[ind][1].HeadWayTime < lvArrTime)))
                        if ((lvGeneHeadwaysTime[ind][0].Time < lvArrTime) && (lvGeneHeadwaysTime[ind][1].HeadWayTime > lvDepTime))
                        {
#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("usando " + ind + " = " + lvGeneHeadwaysTime[ind], pIndet: TrainIndividual.IDLog);
                            }
#endif

                            if(lvGeneHeadwaysTime[ind][1].HeadWayTime > lvCurrentTime)
                            {
                                lvCurrentTime = lvGeneHeadwaysTime[ind][1].HeadWayTime;

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("lvCurrentTime = " + lvCurrentTime, pIndet: TrainIndividual.IDLog);
                                }
#endif
                            }

                            lvStayTime = (lvCurrentTime - lvGene.Time).TotalHours;

                            if (lvSpentTime < lvStayTime)
                            {
                                lvSpentTime = lvStayTime;
                            }

                            lvMeanSpeed = GetJourneyTimes(lvGene, lvDistance, lvNextStopLocation, lvEndCoordinate, lvSpentTime, out lvDepTime, out lvArrTime);
                        }
                        DebugLog.EnableDebug = lvIsLogEnables;
                    }
                }

                if ((lvMeanSpeed <= mMinSpeedLimit) && (lvGene.Time > lvGene.DepartureTime) && lvIsBetweenSwitch)
                {
                    DebugLog.EnableDebug = lvIsLogEnables;
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("UpdateLastArrival => lvSpentTime = " + lvSpentTime, pIndet: TrainIndividual.IDLog);
                        DebugLog.Logar("UpdateLastArrival => lvStayTime = " + lvStayTime, pIndet: TrainIndividual.IDLog);
                        DebugLog.Logar("UpdateLastArrival => lvEndCoordinate = " + lvEndCoordinate, pIndet: TrainIndividual.IDLog);
                        DebugLog.Logar("UpdateLastArrival => lvGene.Coordinate = " + lvGene.Coordinate, pIndet: TrainIndividual.IDLog);
                    }
#endif

                    UpdateLastArrival(lvGene);

                    DebugLog.EnableDebug = lvIsLogEnables;

                    /* Tempo de permancencia no Stop Location */
                    if (lvStopLocation != null)
                    {
                        if (lvGene.Speed > mMinSpeedLimit)
                        {
                            lvSpentTime = (Math.Abs(lvStopLocation.Start_coordinate - lvStopLocation.End_coordinate) / 100000.0) / lvGene.Speed;
                        }
                        else
                        {
                            lvSpentTime = (Math.Abs(lvStopLocation.Start_coordinate - lvStopLocation.End_coordinate) / 100000.0) / mMinSpeedLimit;
                        }
                    }
                    else
                    {
                        if (lvGene.Speed > mMinSpeedLimit)
                        {
                            lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / lvGene.Speed;
                        }
                        else
                        {
                            lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / mMinSpeedLimit;
                        }
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvSpentTime = ");
                        lvStrInfo.Append(lvSpentTime);
                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif

                    lvStayTime = (lvCurrentTime - lvGene.Time).TotalHours;

                    if (lvSpentTime < lvStayTime)
                    {
                        lvSpentTime = lvStayTime;
                    }

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("lvStayTime = ");
                        lvStrInfo.Append(lvStayTime);
                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }

                /* Finalmente monta o Gene de saída !!!! */
                lvNewGene = lvGene.Clone();

                /* Gene de saída */
                lvNewGene.State = Gene.STATE.OUT;
                lvNewGene.Time = lvGene.Time.AddHours(lvSpentTime);
                lvLastDepTime = lvNewGene.Time;
                lvNewGene.SegmentInstance = lvLocDepartureSegment;
                lvNewGene.Coordinate = lvEndCoordinate;
                lvNewGene.Speed = lvMeanSpeed;

                /* Tempo de permancencia no Stop Location */
                if (lvNewGene.Speed > mMinSpeedLimit)
                {
                    lvHeadWayTime = (mTrainLen / 100000.0) / lvNewGene.Speed;
                }
                else
                {
                    lvHeadWayTime = (mTrainLen / 100000.0) / mMinSpeedLimit;
                }
                lvNewGene.HeadWayTime = lvNewGene.Time.AddHours(lvHeadWayTime);

                lvRes = new TrainMovement();
                lvRes.Add(lvNewGene);

                /* Atualizando a lista de Stop Location Departure */
                if (lvNewGene.StopLocation != null)
                {
                    lvGenesStopLocationSet = mStopLocationDeparture[lvNewGene.StopLocation.Location];
                    if (lvGene.Track <= lvNextAvailable.Length)
                    {
                        //lvGenesStopLocation[(lvNewGene.Track - 1) * 2 + Math.Max(0, (int)lvNewGene.Direction)] = lvNewGene;
                        lvGenesStopLocationSet.Add(lvNewGene);

                        //DumpStopDepLocation(lvStopLocation);
                    }
                }

                /*Gene de chegada do próximo Stop Location */
                lvNewGene = lvNewGene.Clone();
                lvNewGene.State = Gene.STATE.IN;
                lvNewGene.Track = (short)lvDestTrack;

                /*
                if (DebugLog.EnableDebug)
                {
                    DebugLog.Logar("lvNewGene.Speed = " + lvNewGene.Speed);
                    DebugLog.Logar("lvLastDepTime = " + lvLastDepTime, pIndet: TrainIndividual.IDLog);
                }
                */

                lvNextSegment = lvNextStopLocation.GetSegment(lvGene.Direction * (-1), lvNewGene.Track);

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvGene.SegmentInstance = " + lvGene.SegmentInstance);
                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvNextStopLocation = " + lvNextStopLocation);
                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                    lvStrInfo.Clear();
                    lvStrInfo.Append("lvNextSegment = " + lvNextSegment);
                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                }
#endif

                lvInitCoordinate = lvEndCoordinate;
                if (lvGene.Direction > 0)
                {
                    lvEndCoordinate = lvNextSegment.Start_coordinate;
                }
                else
                {
                    lvEndCoordinate = lvNextSegment.End_coordinate;
                }
                lvDistance = Math.Abs(lvEndCoordinate - lvInitCoordinate);
                lvNewGene.Coordinate = lvEndCoordinate;

                if (lvTrainPerformance == null || (lvDestTrack > 0))
                {
                    if (!mAllowInertia)
                    {
                        lvMeanSpeed = mVMA;
                    }

                    if (lvNewGene.Track == 0)
                    {
#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            DumpBoolArray(lvNextAvailable);
                        }
#endif
                        return lvRes;
                    }

                    if (mAllowInertia)
                    {
                        if(lvDistance == 0)
                        {
                            lvNewGene.Speed = lvMeanSpeed;
                        }
                        else if (lvDistance < mTrainLen)
                        {
                            lvNewGene.Speed = (lvMeanSpeed + lvMeanSpeed * 0.35);
                        }
                        else
                        {
                            lvNewGene.Speed = (lvMeanSpeed + mVMA) / 2.0;
                        }

                        if(lvNewGene.Speed > mVMA)
                        {
                            lvNewGene.Speed = mVMA;
                        }
                    }
                    else
                    {
                        lvNewGene.Speed = mVMA;
                    }

                    if (lvDistance == 0)
                    {
                        lvNewGene.Time = lvNewGene.HeadWayTime;
                    }
                    else
                    {
                        lvSpentTime = (lvDistance / 100000.0) / lvNewGene.Speed;
                        lvNewGene.Time = lvLastDepTime.AddHours(lvSpentTime);
                    }
                    lvNewGene.HeadWayTime = lvNewGene.Time.AddHours(mTrainLenKM / lvNewGene.Speed);

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        DebugLog.Logar("lvNewGene.HeadWayTime = " + lvNewGene.HeadWayTime, pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
                else
                {
                    if (lvMeanSpeed > mMinSpeedLimit)
                    {
                        if (lvTrainPerformance.TimeMov <= 0.0)
                        {
                            if (lvDistance == 0)
                            {
                                lvNewGene.Speed = lvMeanSpeed;
                            }
                            else if (lvDistance < mTrainLen)
                            {
                                lvNewGene.Speed = (lvMeanSpeed + lvMeanSpeed * 0.35);
                            }
                            else
                            {
                                lvNewGene.Speed = (lvMeanSpeed + mVMA) / 2.0;
                            }
                        }
                        else
                        {
                            lvNewGene.Speed = (lvMeanSpeed + (lvDistance / 100000.0) / (lvTrainPerformance.TimeMov / 60.0)) / 2.0;
                        }

                        if (lvNewGene.Speed > mVMA)
                        {
                            lvNewGene.Speed = mVMA;
                        }

                        if (lvNewGene.Speed > 0.0)
                        {
                            lvSpentTime = (lvDistance / 100000.0) / lvNewGene.Speed;
                        }
                        else
                        {
                            lvSpentTime = 0.0;
                        }

                        if (lvTrainPerformance.TimeHeadWayMov > 0)
                        {
                            lvHeadWayTime = lvTrainPerformance.TimeHeadWayMov / 60.0;
                        }
                        else
                        {
                            if (lvGene.Direction > 0)
                            {
                                lvMeanSpeed = (Math.Abs(lvNextSegment.End_coordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
                            }
                            else
                            {
                                lvMeanSpeed = (Math.Abs(lvNextSegment.Start_coordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
                            }

                            lvHeadWayTime = lvSpentTime + mTrainLenKM / lvMeanSpeed;
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvHeadWayTime obtido de TrainPerformance lvMeanSpeed(");
                            lvStrInfo.Append(lvMeanSpeed);
                            lvStrInfo.Append(") > mMinSpeedLimit = ");
                            lvStrInfo.Append(lvHeadWayTime);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }
                    else
                    {
                        if (lvTrainPerformance.TimeStop <= 0.0)
                        {
                            lvMeanSpeed = (lvNewGene.Speed + mVMA) / 2.0;
                            lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;
                        }
                        else
                        {
                            lvMeanSpeed = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
                            //                        lvMeanSpeed = (mVMA + lvMeanSpeed) / 2.0;
                            //                        lvMeanSpeed = (lvNewGene.Speed + lvMeanSpeed) / 2.0;
                            //                        lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;

                            lvSpentTime = (lvTrainPerformance.TimeStop / 60.0);
                        }

                        if (lvTrainPerformance.TimeHeadWayStop > 0)
                        {
                            lvHeadWayTime = lvTrainPerformance.TimeHeadWayStop / 60.0;
                        }
                        else
                        {
                            if (lvGene.Direction > 0)
                            {
                                lvMeanSpeed = (Math.Abs(lvNextSegment.End_coordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
                            }
                            else
                            {
                                lvMeanSpeed = (Math.Abs(lvNextSegment.Start_coordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
                            }

                            lvHeadWayTime = lvSpentTime + mTrainLenKM / lvMeanSpeed;
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvHeadWayTime obtido de TrainPerformance lvMeanSpeed(");
                            lvStrInfo.Append(lvMeanSpeed);
                            lvStrInfo.Append(") <= mMinSpeedLimit = ");
                            lvStrInfo.Append(lvHeadWayTime);

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }

                    if (lvDistance == 0)
                    {
                        lvNewGene.Time = lvNewGene.HeadWayTime;
                    }
                    else
                    {
                        lvNewGene.Time = lvLastDepTime.AddHours(lvSpentTime);
                        lvNewGene.Speed = lvMeanSpeed;
                    }
                    lvNewGene.HeadWayTime = lvLastDepTime.AddHours(lvHeadWayTime);
                }
                lvNewGene.SegmentInstance = lvNextSegment;
                lvNewGene.StopLocation = lvNextStopLocation;

                if (lvNewGene.State == Gene.STATE.OUT)
                {
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("Gene (");
                        lvStrInfo.Append(lvNewGene);
                        lvStrInfo.Append(") nao possui Headway na chegada !");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }

                if (lvSpentTime >= 2.0)
                {
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append(lvNewGene);
                        lvStrInfo.Append(" demorou muito para chegar no proximo patio, verificar trainPerformance !");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }

                /* Atualizando a lista de Stop Location Arrival */
                if (lvNewGene.StopLocation != null)
                {
                    lvGenesStopLocationSet = mStopLocationArrival[lvNewGene.StopLocation.Location];
                    if (lvNewGene.Track <= lvNextAvailable.Length)
                    {
                        lvGenesStopLocationSet.Add(lvNewGene);

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("UniqueId: ");
                            lvStrInfo.Append(mUniqueId);
                            lvStrInfo.Append(" => Trem ");
                            lvStrInfo.Append(lvNewGene.TrainId);
                            lvStrInfo.Append(" - ");
                            lvStrInfo.Append(lvNewGene.TrainName);
                            lvStrInfo.Append(" adicionado em mStopLocationArrival[");
                            lvStrInfo.Append(lvNewGene.StopLocation.Location);
                            lvStrInfo.Append("]");

                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }
                }

                lvRes.Add(lvNewGene);
                lvRes.LoadId();

                if (!mDicTrain.ContainsKey(lvNewGene.TrainId))
                {
                    mDicTrain.Add(lvNewGene.TrainId, lvRes);
#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("Gene adicionado em mDicTrain(");
                        lvStrInfo.Append(lvNewGene.TrainId);
                        lvStrInfo.Append(") em ");
                        lvStrInfo.Append(lvNewGene.StopLocation);
                        lvStrInfo.Append(" !");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
                else
                {
                    mDicTrain[lvNewGene.TrainId] = lvRes;

#if DEBUG
                    if (DebugLog.EnableDebug)
                    {
                        StringBuilder lvStrInfo = new StringBuilder();

                        lvStrInfo.Clear();
                        lvStrInfo.Append("Gene atualizado em mDicTrain(");
                        lvStrInfo.Append(lvNewGene.TrainId);
                        lvStrInfo.Append(") em ");
                        lvStrInfo.Append(lvNewGene.StopLocation);
                        lvStrInfo.Append(" !");

                        DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                    }
#endif
                }

#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    DumpCountStopLocations(lvGene);
                }
#endif

                if (lvNextStopLocation != null)
                {
                    if (((lvNewGene.Direction > 0) && (lvNextStopLocationValue < lvEndStopLocation.Location)) || ((lvNewGene.Direction < 0) && (lvNextStopLocation.Location > lvEndStopLocation.Location)))
                    {
                        if (lvRes[0].StopLocation != null)
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[lvRes[0].StopLocation.Location];
                            lvListGeneStopLocation.Remove(lvRes[0].TrainId);
                        }

                        if ((lvNextStopLocation.NoStopSet != null) && (lvNextStopLocation.NoStopSet.Count > 0))
                        {
                            if(lvNextStopLocation.HasNoStop(lvNewGene.TrainName.Substring(0, 1) + lvNewGene.Direction))
                            {
                                lvNoStopMovement = (TrainMovement)MoveTrain(lvRes, pInitialTime);

                                if(lvNoStopMovement != null)
                                {
                                    lvNoStopMovement.Add(lvRes, 0);
                                    lvRes = lvNoStopMovement;
                                }
                                else
                                {
                                    if ((lvRes[0].StopLocation != null) && (lvRes.Count > 1))
                                    {
                                        lvListGeneStopLocation = mStopLocationOcupation[lvRes[1].StopLocation.Location];
                                        lvListGeneStopLocation.Remove(lvRes[1].TrainId);

                                        lvListGeneStopLocation = mStopLocationOcupation[lvRes[0].StopLocation.Location];
                                        lvListGeneStopLocation.Add(lvRes[0].TrainId);
                                    }

                                    lvRes = null;

                                    if (lvCurrentTrainMovement != null)
                                    {
                                        mDicTrain[lvNewGene.TrainId] = lvCurrentTrainMovement;
                                    }
                                    else
                                    {
                                        mDicTrain.Remove(lvNewGene.TrainId);
                                    }
                                }
                            }
                            else
                            {
                                lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocationValue];
                                lvListGeneStopLocation.Add(lvNewGene.TrainId);

#if DEBUG
                                if (DebugLog.EnableDebug)
                                {
                                    StringBuilder lvStrInfo = new StringBuilder();

                                    lvStrInfo.Clear();
                                    lvStrInfo.Append("Colocando (");
                                    lvStrInfo.Append(lvNewGene);
                                    lvStrInfo.Append(") na stop location (");
                                    lvStrInfo.Append(lvNextStopLocation);
                                    lvStrInfo.Append(")");

                                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                    //DumpStopLocation(lvStopLocation);
                                    DumpStopLocation(lvNextStopLocation);
                                }
#endif

                                mList.Add(lvRes);
#if DEBUG
                                DebugLog.EnableDebug = lvIsLogEnables;

                                if (DebugLog.EnableDebug)
                                {
                                    DebugLog.Logar("(UniqueId: " + mUniqueId + ") Movimento adicionado: " + lvRes.ToString(), pIndet: TrainIndividual.IDLog);
                                }

                                DebugLog.EnableDebug = lvIsLogEnables;
#endif

                            }
                        }
                        else
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocationValue];
                            lvListGeneStopLocation.Add(lvNewGene.TrainId);

#if DEBUG
                            if (DebugLog.EnableDebug)
                            {
                                StringBuilder lvStrInfo = new StringBuilder();

                                lvStrInfo.Clear();
                                lvStrInfo.Append("Colocando (");
                                lvStrInfo.Append(lvNewGene);
                                lvStrInfo.Append(") na stop location (");
                                lvStrInfo.Append(lvNextStopLocation);
                                lvStrInfo.Append(")");

                                DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                                //DumpStopLocation(lvStopLocation);
                                DumpStopLocation(lvNextStopLocation);
                            }
#endif

                            mList.Add(lvRes);
#if DEBUG
                            DebugLog.EnableDebug = lvIsLogEnables;

                            if (DebugLog.EnableDebug)
                            {
                                DebugLog.Logar("(UniqueId: " + mUniqueId + ") Movimento adicionado: " + lvRes.ToString(), pIndet: TrainIndividual.IDLog);
                            }

                            DebugLog.EnableDebug = lvIsLogEnables;
#endif

                        }
                    }
                    else
                    {
                        mList.Add(lvRes);
#if DEBUG
                        DebugLog.EnableDebug = lvIsLogEnables;

                        if (DebugLog.EnableDebug)
                        {
                            DebugLog.Logar("(UniqueId: " + mUniqueId + ") Movimento adicionado: " + lvRes.ToString(), pIndet: TrainIndividual.IDLog);
                        }

                        DebugLog.EnableDebug = lvIsLogEnables;
#endif

                        /* Atualizando a lista de Stop Location Departure no Stop Location Final para contar o tempo de permanência nele */
                        if ((lvNewGene.StopLocation != null) && (lvNewGene.StopLocation.DwellTimeOnEndStopLocation > 0))
                        {
                            lvNewGene = lvNewGene.Clone();
                            lvNewGene.State = Gene.STATE.OUT;

                            lvGenesStopLocationSet = mStopLocationDeparture[lvNewGene.StopLocation.Location];
                            if (lvNewGene.Track <= lvNewGene.StopLocation.Capacity)
                            {
                                lvNewGene.Time = lvNewGene.Time.AddSeconds(lvNewGene.StopLocation.DwellTimeOnEndStopLocation);
                                //lvGenesStopLocation[(lvNewGene.Track - 1) * 2 + Math.Max(0, (int)lvNewGene.Direction)] = lvNewGene;
                                lvGenesStopLocationSet.Add(lvNewGene);

                                //DumpStopDepLocation(lvStopLocation);
                            }
                        }

                        mDicTrain.Remove(lvNewGene.TrainId);
                        mTrainFinished.Add(lvNewGene.TrainId);

                        if (lvRes[0].StopLocation != null)
                        { 
                            lvListGeneStopLocation = mStopLocationOcupation[lvRes[0].StopLocation.Location];
                            lvListGeneStopLocation.Remove(lvNewGene.TrainId);
                        }

#if DEBUG
                        if (DebugLog.EnableDebug)
                        {
                            StringBuilder lvStrInfo = new StringBuilder();

                            lvStrInfo.Clear();
                            lvStrInfo.Append("Gene removido de mDicTrain(");
                            lvStrInfo.Append(lvNewGene.ToString());
                            lvStrInfo.Append(") ! MoveTrain => else de ((lvNewGene.Direction > 0) && (lvNextStopLocation.Location < lvEndStopLocation.Location)) || ((lvNewGene.Direction < 0) && (lvNextStopLocation.Location > lvEndStopLocation.Location))");
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvNewGene.Direction = ");
                            lvStrInfo.Append(lvNewGene.Direction);
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvNextStopLocationValue = ");
                            lvStrInfo.Append(lvNextStopLocationValue);
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvNextStopLocation.Location = ");
                            lvStrInfo.Append(lvNextStopLocation.Location);
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                            lvStrInfo.Clear();
                            lvStrInfo.Append("lvEndStopLocation.Location = ");
                            lvStrInfo.Append(lvEndStopLocation.Location);
                            DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);
                        }
#endif
                    }
                }

                if (lvRes != null)
                {
                    if (mCalcDistRef)
                    {
                        if (!mDicDistRef.ContainsKey(lvRes.GetID()))
                        {
                            mDicDistRef.Add(lvRes.GetID(), mList.Count-1);
                        }
                    }
                }
            }
            else
            {
#if DEBUG
                if (DebugLog.EnableDebug)
                {
                    StringBuilder lvStrInfo = new StringBuilder();

                    lvStrInfo.Clear();
                    lvStrInfo.Append("Gene ");
                    lvStrInfo.Append(lvGene.TrainId);
                    lvStrInfo.Append(" (");
                    lvStrInfo.Append(lvGene.TrainName);
                    lvStrInfo.Append(") nao pode sair de ");
                    lvStrInfo.Append(lvGene.SegmentInstance.Location);
                    lvStrInfo.Append(".");
                    lvStrInfo.Append(lvGene.SegmentInstance.SegmentValue);
                    lvStrInfo.Append(" devido aos destinos estarem ocupados ! lvNextCapacity = ");
                    lvStrInfo.Append(lvNextCapacity);

                    DebugLog.Logar(lvStrInfo.ToString(), pIndet: TrainIndividual.IDLog);

                    /*
                    if (lvStopLocation != null)
                    {
                        DumpStopLocation(lvStopLocation);
                    }
                    DumpNextStopLocation(lvGene);
                    */
                }
#endif

                lvRes = null;
            }
        }
        catch (Exception ex)
        {
            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
        }

        DebugLog.EnableDebug = lvIsLogEnables;

        return lvRes;
    }

    private double GetJourneyTimes(Gene pGene, int pStopLocationDistance, StopLocation pNextStopLocation, int pEndCoordinate, double pSpentTime, out DateTime pDepTime, out DateTime pArrTime)
    {
        double lvRes;
        int lvDistance;

        lvRes = (pStopLocationDistance / 100000.0) / pSpentTime;

        if (lvRes > mVMA)
        {
            lvRes = mVMA;
        }
        else if (lvRes < mMinSpeedLimit)
        {
            lvRes = mMinSpeedLimit;
        }
        else
        {
            if (pStopLocationDistance < mTrainLen)
            {
                lvRes += lvRes * 0.35;
            }
            else
            {
                lvRes = (lvRes + mVMA) / 2.0;
            }
        }

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("lvMeanSpeed = " + lvRes, pIndet: TrainIndividual.IDLog);
        }
#endif

        pDepTime = pGene.Time.AddHours(pSpentTime);
        if (pGene.Direction > 0)
        {
            lvDistance = pNextStopLocation.Start_coordinate - pEndCoordinate + mTrainLen;
        }
        else
        {
            lvDistance = pEndCoordinate - pNextStopLocation.End_coordinate + mTrainLen;
        }

        pArrTime = pDepTime.AddHours((lvDistance / 100000.0) / lvRes);

#if DEBUG
        if (DebugLog.EnableDebug)
        {
            DebugLog.Logar("lvDepTime = " + pDepTime, pIndet: TrainIndividual.IDLog);
            DebugLog.Logar("lvArrTime = " + pArrTime, pIndet: TrainIndividual.IDLog);
        }
#endif

        return lvRes;
    }

    private void DumpCountStopLocations(Gene pGene)
    {
        int lvCount = 0;

        if (DebugLog.EnableDebug)
        {
            if (pGene == null)
            {
                foreach (TrainMovement lvTrainMov in mDicTrain.Values)
                {
                    lvCount = 0;
                    foreach (int lvStopLocationValue in mStopLocationOcupation.Keys)
                    {
                        foreach (Int64 lvTrainId in mStopLocationOcupation[lvStopLocationValue])
                        {
                            if (lvTrainId == lvTrainMov.Last.TrainId)
                            {
                                lvCount++;
                            }
                        }
                    }

#if DEBUG
                    if (lvCount > 1)
                    {
                        if (lvTrainMov.Last.StopLocation == null)
                        {
                            DebugLog.Logar("Stop Locations Count para (" + lvTrainMov.Last + ") = " + lvCount, pIndet: TrainIndividual.IDLog);
                        }
                        else
                        {
                            DebugLog.Logar("Stop Locations Count para (" + lvTrainMov.Last + ", Current: " + lvTrainMov.Last.StopLocation + ") = " + lvCount, pIndet: TrainIndividual.IDLog);
                        }
                    }
#endif
                }
            }
            else
            {
                lvCount = 0;
                foreach (int lvStopLocationValue in mStopLocationOcupation.Keys)
                {
                    foreach (double lvTrainId in mStopLocationOcupation[lvStopLocationValue])
                    {
                        if (lvTrainId == pGene.TrainId)
                        {
                            lvCount++;
#if DEBUG
                            DebugLog.Logar("Stop Locations para Gene (" + pGene + ") = " + lvStopLocationValue, pIndet: TrainIndividual.IDLog);
#endif
                        }
                    }
                }

                if (lvCount > 0)
                {
#if DEBUG
                    if (pGene.StopLocation == null)
                    {
                        DebugLog.Logar("Stop Locations Count para (" + pGene + ") = " + lvCount, pIndet: TrainIndividual.IDLog);
                    }
                    else
                    {
                        DebugLog.Logar("Stop Locations Count para (" + pGene + ", Current: " + pGene.StopLocation + ") = " + lvCount, pIndet: TrainIndividual.IDLog);
                    }
#endif
                }
            }
        }
    }

    public void Serialize()
    {
        string lvSerializationFile = ConfigurationManager.AppSettings["SAVE_DATA_PATH"] + "Individual_" + mUniqueId + ".bin";

        if (Directory.Exists(ConfigurationManager.AppSettings["SAVE_DATA_PATH"]))
        {
            using (Stream stream = File.Open(lvSerializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, mList);
            }
        }
    }

    public void UnSerialize()
    {
        string lvSerializationFile = ConfigurationManager.AppSettings["SAVE_DATA_PATH"] + "Individual_" + mUniqueId + ".bin";

        if (File.Exists(lvSerializationFile))
        {
            using (Stream stream = File.Open(lvSerializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                mList = (List<TrainMovement>)bformatter.Deserialize(stream);
            }
        }
    }

    public static List<TrainMovement> UnSerialize(int pUniqueId)
    {
        List<TrainMovement> lvRes = null;

        string lvSerializationFile = ConfigurationManager.AppSettings["SAVE_DATA_PATH"] + "Individual_" + pUniqueId + ".bin";

        if (File.Exists(lvSerializationFile))
        {
            using (Stream stream = File.Open(lvSerializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                lvRes = (List<TrainMovement>)bformatter.Deserialize(stream);
            }
        }

        return lvRes;
    }

    private void DumpStopLocationGene(Gene pGene)
    {
        Gene lvGene = null;
        TrainMovement lvTrainMovement;

        if (DebugLog.EnableDebug)
        {
            if (pGene != null)
            {
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" -------------------------------------- DumpStopLocationsByGene ----------------------------------- ", pIndet: TrainIndividual.IDLog);
                foreach (int lvStopLocationValue in mStopLocationOcupation.Keys)
                {
                    foreach (Int64 lvTrainId in mStopLocationOcupation[lvStopLocationValue])
                    {
                        lvTrainMovement = mDicTrain[lvTrainId];
                        lvGene = lvTrainMovement.Last;

                        if (lvGene != null)
                        {
                            if (pGene.TrainId == lvGene.TrainId)
                            {
                                DebugLog.Logar(" -----------------------", pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("StopLocation = " + lvStopLocationValue, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName, pIndet: TrainIndividual.IDLog);
                                if (lvGene.StopLocation != null)
                                {
                                    DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location, pIndet: TrainIndividual.IDLog);
                                }
                                DebugLog.Logar("lvGene.Track = " + lvGene.Track, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.Location = " + lvGene.SegmentInstance.Location, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.UD = " + lvGene.SegmentInstance.SegmentValue, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.End = " + lvGene.End, pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar("lvGene.Time = " + lvGene.Time, pIndet: TrainIndividual.IDLog);

                                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar(" -----------------------", pIndet: TrainIndividual.IDLog);
                                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
                            }
                        }
                    }
                }
                DebugLog.Logar(" ------------------------------------------------------------------------------------------- ", pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
            }
        }
    }

    public void DumpDeadLockPoint()
    {
        Gene lvLastForward = null;
        Gene lvLastReverse = null;
        Gene lvGene = null;
        TrainMovement lvTrainMovement;

        DebugLog.Logar(" ", pIndet: TrainIndividual.IDLog);
        DebugLog.Logar(" -------------------------------------- DumpDeadLockPoint ----------------------------------- ", false, pIndet: TrainIndividual.IDLog);
        foreach (KeyValuePair<int, HashSet<Int64>> lvStopLocationEntry in mStopLocationOcupation)
        {
            foreach (Int64 lvTrainId in lvStopLocationEntry.Value)
            {
                if (mDicTrain.ContainsKey(lvTrainId))
                {
                    lvTrainMovement = mDicTrain[lvTrainId];
                    lvGene = lvTrainMovement.Last;

                    if (lvGene.Direction > 0)
                    {
                        if (lvLastForward == null)
                        {
                            lvLastForward = lvGene;
                        }
                        else
                        {
                            if (lvGene.SegmentInstance.Start_coordinate > lvLastForward.SegmentInstance.Start_coordinate)
                            {
                                lvLastForward = lvGene;
                            }
                        }
                    }
                    else
                    {
                        if (lvLastReverse == null)
                        {
                            lvLastReverse = lvGene;
                        }
                        else
                        {
                            if (lvGene.SegmentInstance.End_coordinate < lvLastReverse.SegmentInstance.End_coordinate)
                            {
                                lvLastReverse = lvGene;
                            }
                        }
                    }
                }
            }
        }

        if (lvLastForward != null)
        {
            DumpStopLocation(lvLastForward.StopLocation, false);
        }

        if (lvLastReverse != null)
        {
            DumpStopLocation(lvLastReverse.StopLocation, false);
        }

        DebugLog.Logar(" ------------------------------------------------------------------------------------------- ", false, pIndet: TrainIndividual.IDLog);
        DebugLog.Logar(" ", false, pIndet: TrainIndividual.IDLog);
    }

    public void DumpStopLocation(StopLocation pStopLocation, bool pUseDateInfo = true)
    {
        int lvCount = 0;
        Gene lvGene = null;
        TrainMovement lvTrainMovement;

        if (DebugLog.EnableDebug || !pUseDateInfo)
        {
            if (pStopLocation != null)
            {
                DebugLog.Logar(" ", pUseDateInfo, TrainIndividual.IDLog);
                DebugLog.Logar(" -------------------------------------- DumpStopLocation ----------------------------------- ", pUseDateInfo, TrainIndividual.IDLog);
                foreach (Int64 lvTrainId in mStopLocationOcupation[pStopLocation.Location])
                {
                    if (mDicTrain.ContainsKey(lvTrainId))
                    {
                        lvTrainMovement = mDicTrain[lvTrainId];
                        lvGene = lvTrainMovement.Last;

                        DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName, pUseDateInfo, TrainIndividual.IDLog);
                        if (lvGene.StopLocation != null)
                        {
                            DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location, pUseDateInfo, TrainIndividual.IDLog);
                        }
                        DebugLog.Logar("lvGene.Track = " + lvGene.Track, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.Location = " + lvGene.SegmentInstance.Location, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.UD = " + lvGene.SegmentInstance.SegmentValue, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.Direction = " + lvGene.Direction, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.End = " + lvGene.End, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.Time = " + lvGene.Time, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.HeadWayTime = " + lvGene.HeadWayTime, pUseDateInfo, TrainIndividual.IDLog);
                        DebugLog.Logar("lvGene.Speed = " + lvGene.Speed, pUseDateInfo, TrainIndividual.IDLog);

                        DebugLog.Logar(" ", pUseDateInfo, TrainIndividual.IDLog);
                        lvCount++;
                    }
                }
                DebugLog.Logar(" ", pUseDateInfo, TrainIndividual.IDLog);
                DebugLog.Logar("Total = " + lvCount, pUseDateInfo, TrainIndividual.IDLog);
                DebugLog.Logar(" ------------------------------------------------------------------------------------------- ", pUseDateInfo, TrainIndividual.IDLog);
                DebugLog.Logar(" ", pUseDateInfo, TrainIndividual.IDLog);
            }
            else
            {
                DebugLog.Logar(" ", pUseDateInfo, TrainIndividual.IDLog);
                DebugLog.Logar(" -------------------------------------- DumpStopLocations ----------------------------------- ", pUseDateInfo, TrainIndividual.IDLog);
                foreach (KeyValuePair<int, HashSet<Int64>> lvStopLocationEntry in mStopLocationOcupation)
                {
                    foreach (Int64 lvTrainId in lvStopLocationEntry.Value)
                    {
                        if (mDicTrain.ContainsKey(lvTrainId))
                        {
                            lvTrainMovement = mDicTrain[lvTrainId];
                            lvGene = lvTrainMovement.Last;

                            DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            if (lvGene.StopLocation != null)
                            {
                                DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            }
                            DebugLog.Logar("lvGene.Track = " + lvGene.Track, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Location = " + lvGene.SegmentInstance.Location, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.UD = " + lvGene.SegmentInstance.SegmentValue, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Direction = " + lvGene.Direction, pUseDateInfo, TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.End = " + lvGene.End, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Time = " + lvGene.Time, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            DebugLog.Logar("lvGene.Speed = " + lvGene.Speed, pUseDateInfo, pIndet: TrainIndividual.IDLog);

                            DebugLog.Logar(" ", pUseDateInfo, pIndet: TrainIndividual.IDLog);
                            lvCount++;
                        }
                    }
                }
                DebugLog.Logar(" ", pUseDateInfo, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar("Total = " + lvCount, pUseDateInfo, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ------------------------------------------------------------------------------------------- ", pUseDateInfo, pIndet: TrainIndividual.IDLog);
                DebugLog.Logar(" ", pUseDateInfo, pIndet: TrainIndividual.IDLog);
            }
        }
    }

    public bool hasInDic(Int64 pTrainId)
    {
        return mDicTrain.ContainsKey(pTrainId);
    }

    public bool hasArrived(Int64 pTrainId)
    {
        return mTrainFinished.Contains(pTrainId);
    }

    public void AddElementRef(TrainMovement pElement)
    {
        if(!mDicTrain.ContainsKey(pElement.Last.TrainId))
        {
            mDicTrain.Add(pElement.Last.TrainId, pElement);
        }
    }

    public int GetUniqueId()
    {
        return mUniqueId;
    }

    public TrainMovement this[int index]
    {
        get
        {
            if ((index < mList.Count) && (index >= 0))
            {
                return mList[index];
            }
            else
            {
                return null;
            }
        }
        set
        {
            if ((index < mList.Count) && (index >= 0))
            {
                mList[index] = value;
            }
        }
    }

    public static double VMA
    {
        get { return TrainIndividual.mVMA; }
        set { TrainIndividual.mVMA = value; }
    }

    public static int TrainLen
    {
        get { return TrainIndividual.mTrainLen; }
        set
        {
            TrainIndividual.mTrainLen = value;
            TrainIndividual.mTrainLenKM = (double)mTrainLen / 100000.0;
        }
    }

    public static int LimitDays
    {
        get { return TrainIndividual.mLimitDays; }
        set { TrainIndividual.mLimitDays = value; }
    }

    public static bool AllowInertia
    {
        get
        {
            return mAllowInertia;
        }

        set
        {
            mAllowInertia = value;
        }
    }

    public static int StrategyFactor
    {
        get
        {
            return mStrategyFactor;
        }

        set
        {
            mStrategyFactor = value;
        }
    }

    public static int IDLog
    {
        get
        {
            return mIDLog;
        }

        set
        {
            mIDLog = value;
        }
    }

    public double GBest
    {
        get
        {
            return mGBest;
        }

        set
        {
            mGBest = value;
        }
    }

    public double RefFitnessValue
    {
        get
        {
            return mBestFitness;
        }

        set
        {
            mBestFitness = value;
        }
    }

    private class HeadWayGeneTimeComparer : IComparer<Gene[]>
    {
        public int Compare(Gene[] x, Gene[] y)
        {
            int lvRes;

            if ((x == null) || (x.Length != 2))
            {
                lvRes = 1;
            }
            else if ((y == null) || (y.Length != 2))
            {
                lvRes = -1;
            }
            else if (x[1].HeadWayTime > y[1].HeadWayTime)
            {
                lvRes = 1;
            }
            else if (x[1].HeadWayTime < y[1].HeadWayTime)
            {
                lvRes = -1;
            }
            else
            {
                if (x[0].HeadWayTime > y[0].HeadWayTime)
                {
                    lvRes = 1;
                }
                else if (x[0].HeadWayTime < y[0].HeadWayTime)
                {
                    lvRes = -1;
                }
                else
                {
                    lvRes = 0;
                }
            }

            return lvRes;
        }

    }

    private class DescendingGeneTimeComparar : IComparer<Gene>
    {
        public int Compare(Gene x, Gene y)
        {
            int lvRes;

            if (x == null)
            {
                lvRes = 1;
            }
            else if (y == null)
            {
                lvRes = -1;
            }
            else if (x.HeadWayTime > y.HeadWayTime)
            {
                lvRes = -1;
            }
            else if (x.HeadWayTime < y.HeadWayTime)
            {
                lvRes = 1;
            }
            else
            {
                if (x.Time > y.Time)
                {
                    lvRes = -1;
                }
                else if (x.Time < y.Time)
                {
                    lvRes = 1;
                }
                else
                {
                    lvRes = 0;
                }
            }

            return lvRes;
        }
    }

    private class DescendingTimeComparar : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            int lvRes;

            if (x == null)
            {
                lvRes = 1;
            }
            else if (y == null)
            {
                lvRes = -1;
            }
            else if (x > y)
            {
                lvRes = -1;
            }
            else if (x < y)
            {
                lvRes = 1;
            }
            else
            {
                lvRes = 0;
            }

            return lvRes;
        }
    }
}