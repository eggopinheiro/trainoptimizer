using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Threading;

namespace TrainOptimization
{
    public partial class TrainOptimizer : ServiceBase
    {
        private System.Timers.Timer mTimer = null;
        private DateTime mInitialDate;
        private DateTime mFinalDate;
        private string mInputMode = "";
        private string mLoadStopLocationsFrom = "";
        private string mTrainPriority = "";
        private int mCrossOverPoints = 0;

        public TrainOptimizer()
        {
            InitializeComponent();

            this.AutoLog = false;

            if (!System.Diagnostics.EventLog.SourceExists("TrainOptimizer"))
            {
                System.Diagnostics.EventLog.CreateEventSource("TrainOptimizer", "TrainOptimizerLog");
            }

            CultureInfo culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            eventLog1.Source = "TrainOptimizer";
            eventLog1.Log = "TrainOptimizerLog";
        }

        public void onDebug()
        {
            OnStart(null);
            //LoadTrainPerformance();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            eventLog1.WriteEntry("Train Optimizator iniciado !");
            DebugLog.Logar("OnStart", false);

            //            mTimer_Elapsed(this, null);

            /* Alternativa: */
            /* System.Threading.Timer T = new System.Threading.Timer(new TimerCallback(DoSomething), null, 0, 30000); */

            mInputMode = ConfigurationManager.AppSettings["INPUT_MODE"];

            if (!int.TryParse(ConfigurationManager.AppSettings["CROSSOVER_POINTS"], out mCrossOverPoints))
            {
                mCrossOverPoints = 0;
            }

            int lvMinCrossOverPoints = 2;
            if (!int.TryParse(ConfigurationManager.AppSettings["MIN_CROSSOVER_POINTS"], out lvMinCrossOverPoints))
            {
                lvMinCrossOverPoints = 1;
            }
            Population.MinCrossOverPoints = lvMinCrossOverPoints;

            int lvMaxCrossOverPoints = 2;
            if (!int.TryParse(ConfigurationManager.AppSettings["MAX_CROSSOVER_POINTS"], out lvMaxCrossOverPoints))
            {
                lvMaxCrossOverPoints = 2;
            }
            Population.MaxCrossOverPoints = lvMaxCrossOverPoints;

            mLoadStopLocationsFrom = ConfigurationManager.AppSettings["LOAD_STOP_LOCATIONS"];
            mTrainPriority = ConfigurationManager.AppSettings["TRAIN_PRIORITY"];

            if (mInputMode.Equals("db"))
            {
                int lvTestCount = 0;

                if (!int.TryParse(ConfigurationManager.AppSettings["TEST_COUNT"], out lvTestCount))
                {
                    lvTestCount = 0;
                }

                mTimer = new System.Timers.Timer();
                if (lvTestCount > 0)
                {
                    this.mTimer.Interval = 1000;
                }
                else
                {
                    this.mTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["SLEEP_TIME"]);
                }
                this.mTimer.AutoReset = false;
                this.mTimer.Elapsed += new ElapsedEventHandler(mTimer_Elapsed);
                this.mTimer.Enabled = true;
                this.mTimer.Start();
            }
            else if(mInputMode.Equals("xml"))
            {
                mTimer = new System.Timers.Timer();
                this.mTimer.Interval = 1000;
                this.mTimer.Elapsed += new ElapsedEventHandler(mTimerXML_Elapsed);
                this.mTimer.Enabled = true;
                this.mTimer.AutoReset = false;
                this.mTimer.Start();
            }
        }

        private void ProcessRailWay(string pStrFile)
        {
            string lvStrTrainAllowed = "";
            int lvPopulationSize = 0;
            int lvMaxGenerations = 0;
            int lvMutationRate = 0;
            IFitness<TrainMovement> lvFitness = null;
            Population lvPopulation = null;
            TrainIndividual lvGeneIndividual = null;
            bool lvLogEnable;
            bool lvUsePerformanceData;
            bool lvAllowInertia;
            bool lvRes = false;
            int lvStrategyFactor = 1;
            int lvMaxDeadLockError = 0;
            int lvMaxParallelThreads = 1;
            double lvNicheDistance = 0.0;
            TrainMovement lvTrainMovement = null;
            List<StopLocation> lvStopLocations = new List<StopLocation>();
            List<Segment> lvSegments = new List<Segment>();
            List<Segment> lvSwitches = new List<Segment>();
            List<TrainMovement> lvTrainMovList = new List<TrainMovement>();
            List<TrainMovement> lvPlans = new List<TrainMovement>();
            List<TrainMovement> lvTrainsData = null;
            List<TrainMovement> lvPlansData = null;
            Segment lvSegment = null;
            StopLocation lvStartStopLocation = null;
            StopLocation lvEndStopLocation = null;
            StopLocation lvNextStopLocation = null;
            Gene lvGene = null;
            Interdicao lvInterdicao = null;
            Trainpat lvTrainpat = null;
            List<Trainpat> lvListPATs = null;
            Dictionary<Int64, List<Trainpat>> lvPATs = new Dictionary<long, List<Trainpat>>();
            string lvStrInitialLogPath = ConfigurationManager.AppSettings["LOG_PATH"] + "Logs\\";
            int lvLocation = -1;
            double lvThreshold;
            Int64 lvTrainId;
            string lvStrUD = "";
            string lvStrFileName = "";
            int lvTestCount = 1;
            int lvIndex = -1;
            int lvValue = -1;
            string lvStrMode = "";
            DateTime lvCurrTime = DateTime.MaxValue;

            lvStrMode = ConfigurationManager.AppSettings["OPT_MODE"];

            if (!bool.TryParse(ConfigurationManager.AppSettings["DEBUG_LOG_ENABLE"], out lvLogEnable))
            {
                lvLogEnable = false;
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["USE_DB_PERF_DATA"], out lvUsePerformanceData))
            {
                lvUsePerformanceData = false;
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["ALLOW_INERTIA"], out lvAllowInertia))
            {
                lvAllowInertia = true;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["STRATEGY_FACTOR"], out lvStrategyFactor))
            {
                lvStrategyFactor = 1;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["MAX_DEAD_LOCK_ERROR"], out lvMaxDeadLockError))
            {
                lvMaxDeadLockError = 1;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["MAX_PARALLEL_THREADS"], out lvMaxParallelThreads))
            {
                lvMaxParallelThreads = 1;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["TEST_COUNT"], out lvTestCount))
            {
                lvTestCount = 1;
            }

            if (!double.TryParse(ConfigurationManager.AppSettings["NICHE_DISTANCE"], out lvNicheDistance))
            {
                lvNicheDistance = 0.0;
            }

            if (!double.TryParse(ConfigurationManager.AppSettings["THRESHOLD"], out lvThreshold))
            {
                lvThreshold = 1.0;
            }

            Population.MAX_PARALLEL_THREADS = lvMaxParallelThreads;
            TrainPerformanceControl.UseDBPerformanceData = lvUsePerformanceData;
            TrainIndividual.AllowInertia = lvAllowInertia;
            TrainIndividual.StrategyFactor = lvStrategyFactor;

            TrainIndividual.VMA = double.Parse(ConfigurationManager.AppSettings["VMA"]);
            TrainIndividual.TrainLen = int.Parse(ConfigurationManager.AppSettings["TRAIN_LEN"]);
            TrainIndividual.LimitDays = int.Parse(ConfigurationManager.AppSettings["LIMIT_DAYTIME"]);
            StopLocation.MinLen = TrainIndividual.TrainLen;

            DebugLog.EnableDebug = true;

            DebugLog.Logar("TrainIndividual.VMA = " + TrainIndividual.VMA);
            DebugLog.Logar("TrainIndividual.TrainLen = " + TrainIndividual.TrainLen);

            lvPopulationSize = int.Parse(ConfigurationManager.AppSettings["POPULATION_SIZE"]);
            lvMaxGenerations = int.Parse(ConfigurationManager.AppSettings["MAX_GENERATIONS"]);
            lvMutationRate = int.Parse(ConfigurationManager.AppSettings["MUTATION_RATE"]);
            lvStrTrainAllowed = ConfigurationManager.AppSettings["TRAIN_TYPE_ALLOWED"];

            Population.TrainAllowed = lvStrTrainAllowed;
            Population.MAX_DEAD_LOCK_ERROR = lvMaxDeadLockError;

            DebugLog.Logar("lvPopulationSize = " + lvPopulationSize);
            DebugLog.Logar("lvMaxGenerations = " + lvMaxGenerations);
            DebugLog.Logar("lvMutationRate = " + lvMutationRate);
            DebugLog.Logar("lvStrTrainAllowed = " + lvStrTrainAllowed);

            DebugLog.Logar("lvInitialDate = " + mInitialDate);
            DebugLog.Logar("lvFinalDate = " + mFinalDate);

            lvStrFileName = Path.GetFileNameWithoutExtension(pStrFile);

            XmlReader lvXmlReader = XmlReader.Create(pStrFile);

            Population.LoadPriority(mTrainPriority);

            lvIndex = 0;
            while (lvXmlReader.Read())
            {
                if (lvXmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (lvXmlReader.Name)
                    {
                        case "Segment":
                            lvSegment = new Segment();

                            if (lvXmlReader["location"] != null)
                            {
                                lvSegment.Location = Convert.ToInt32(lvXmlReader["location"]);
                            }

                            if (lvXmlReader["start_coordinate"] != null)
                            {
                                lvSegment.Start_coordinate = Convert.ToInt32(lvXmlReader["start_coordinate"]);
                            }

                            if (lvXmlReader["end_coordinate"] != null)
                            {
                                lvSegment.End_coordinate = Convert.ToInt32(lvXmlReader["end_coordinate"]);
                            }

                            if (lvXmlReader["segment"] != null)
                            {
                                lvSegment.SegmentValue = lvXmlReader["segment"];
                            }

                            if (lvXmlReader["track"] != null)
                            {
                                lvSegment.Track = Convert.ToInt16(lvXmlReader["track"]);
                            }

                            lvSegment.AllowSameLineMov = false;

                            lvSegments.Add(lvSegment);

                            if(lvSegment.SegmentValue.Equals("WT"))
                            {
                                lvSegment.IsSwitch = true;

                                if (lvXmlReader["left_no_entrance"] != null)
                                {
                                    lvSegment.AddNoEntrances(lvXmlReader["left_no_entrance"], 1);
                                }

                                if (lvXmlReader["right_no_entrance"] != null)
                                {
                                    lvSegment.AddNoEntrances(lvXmlReader["right_no_entrance"], -1);
                                }

                                lvSwitches.Add(lvSegment);
                            }

                            break;
                        case "Train":
                            lvGene = new Gene();

                            if (lvXmlReader["train_id"] != null)
                            {
                                lvGene.TrainId = Convert.ToInt64(lvXmlReader["train_id"]);
                            }

                            if (lvXmlReader["name"] != null)
                            {
                                lvGene.TrainName = lvXmlReader["name"];
                            }

                            if (lvXmlReader["data_ocup"] != null)
                            {
                                lvGene.Time = DateTime.ParseExact(lvXmlReader["data_ocup"], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                lvGene.HeadWayTime = lvGene.Time;

                                if(lvGene.Time < lvCurrTime)
                                {
                                    lvCurrTime = lvGene.Time;
                                }
                            }

                            if (lvXmlReader["direction"] != null)
                            {
                                lvGene.Direction = Convert.ToInt16(lvXmlReader["direction"]);
                            }

                            if (lvXmlReader["track"] != null)
                            {
                                lvGene.Track = Convert.ToInt16(lvXmlReader["track"]);
                            }

                            if (lvXmlReader["coordinate"] != null)
                            {
                                lvGene.Coordinate = Convert.ToInt32(lvXmlReader["coordinate"]);
                            }

                            if (lvXmlReader["origem"] != null)
                            {
                                lvGene.Start = Convert.ToInt32(lvXmlReader["origem"]);
                            }

                            if (lvXmlReader["destino"] != null)
                            {
                                lvGene.End = Convert.ToInt32(lvXmlReader["destino"]);
                            }

                            if (lvXmlReader["location"] != null)
                            {
                                lvLocation = Convert.ToInt32(lvXmlReader["location"]);
                            }

                            if (lvXmlReader["ud"] != null)
                            {
                                lvStrUD = lvXmlReader["ud"];
                            }

                            if (lvXmlReader["departure_time"] != null)
                            {
                                lvGene.DepartureTime = DateTime.ParseExact(lvXmlReader["departure_time"], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            }

                            lvGene.Speed = 0.0;
                            lvGene.State = Gene.STATE.IN;
                            lvGene.ValueWeight = Population.GetPriority(lvGene);

                            lvTrainMovement = new TrainMovement();
                            lvTrainMovement.Add(lvGene);

                            lvTrainMovList.Add(lvTrainMovement);

                            break;
                        case "Plan":
                            lvGene = new Gene();

                            if (lvXmlReader["plan_id"] != null)
                            {
                                lvGene.TrainId = Convert.ToInt64(lvXmlReader["plan_id"]);
                            }

                            if (lvXmlReader["train_name"] != null)
                            {
                                lvGene.TrainName = lvXmlReader["train_name"];
                            }

                            if (lvXmlReader["origem"] != null)
                            {
                                lvGene.Start = Convert.ToInt32(lvXmlReader["origem"]);
                            }

                            if (lvXmlReader["destino"] != null)
                            {
                                lvGene.End = Convert.ToInt32(lvXmlReader["destino"]);
                            }

                            if (lvXmlReader["departure_time"] != null)
                            {
                                lvGene.DepartureTime = DateTime.ParseExact(lvXmlReader["departure_time"], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            }

                            if (lvXmlReader["direction"] != null)
                            {
                                lvGene.Direction = Convert.ToInt16(lvXmlReader["direction"]);
                            }

                            if (lvXmlReader["track"] != null)
                            {
                                lvGene.Track = Convert.ToInt16(lvXmlReader["track"]);
                            }
                            else
                            {
                                lvGene.Track = 1;
                            }

                            lvGene.Time = DateTime.MinValue;
                            lvGene.Coordinate = lvGene.Start;
                            lvGene.Speed = 0.0;
                            lvGene.ValueWeight = Population.GetPriority(lvGene);

                            lvTrainMovement = new TrainMovement();
                            lvTrainMovement.Add(lvGene);

                            lvPlans.Add(lvTrainMovement);

                            break;
                        case "Interdiction":
                            lvInterdicao = new Interdicao();

                            lvInterdicao.Ti_id = ++lvIndex;

                            if (lvXmlReader["start_pos"] != null)
                            {
                                lvInterdicao.Start_pos = Convert.ToInt32(lvXmlReader["start_pos"]);
                            }

                            if (lvXmlReader["end_pos"] != null)
                            {
                                lvInterdicao.End_pos = Convert.ToInt32(lvXmlReader["end_pos"]);
                            }

                            if (lvXmlReader["start_time"] != null)
                            {
                                lvValue = Convert.ToInt32(lvXmlReader["start_time"]);
                                lvInterdicao.Start_time = lvCurrTime.AddMinutes(lvValue);
                                //lvInterdicao.Start_time = DateTime.ParseExact(lvXmlReader["start_time"], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                if (lvXmlReader["end_time"] != null)
                                {
                                    lvValue = Convert.ToInt32(lvXmlReader["end_time"]);
                                    lvInterdicao.End_time = lvInterdicao.Start_time.AddMinutes(lvValue);
                                }
                            }

                            if (lvXmlReader["reason"] != null)
                            {
                                lvInterdicao.Reason = lvXmlReader["reason"];
                            }

                            if (lvXmlReader["ss_name"] != null)
                            {
                                lvInterdicao.Ss_name = lvXmlReader["ss_name"];
                            }

                            lvInterdicao.Field_interdicted = 1;
                            lvInterdicao.Track = Segment.GetLineByUD(lvInterdicao.Ss_name);

                            Interdicao.Add(lvInterdicao);

                            break;
                        case "PAT":
                            lvTrainpat = new Trainpat();

                            if (lvXmlReader["plan_id"] != null)
                            {
                                lvTrainId = Convert.ToInt64(lvXmlReader["plan_id"]);
                            }
                            else
                            {
                                lvTrainId = -1;
                            }

                            if (lvXmlReader["coordinate"] != null)
                            {
                                lvTrainpat.Coordinate = Convert.ToInt32(lvXmlReader["coordinate"]);
                            }

                            if (lvXmlReader["km"] != null)
                            {
                                lvTrainpat.KM = Convert.ToInt16(lvXmlReader["km"]);
                            }

                            if (lvXmlReader["duration"] != null)
                            {
                                lvTrainpat.Duration = Convert.ToInt16(lvXmlReader["duration"]);
                            }

                            if (lvXmlReader["definition"] != null)
                            {
                                lvTrainpat.Activity = Convert.ToString(lvXmlReader["definition"]);
                            }

                            if(!lvPATs.ContainsKey(lvTrainId))
                            {
                                lvListPATs = new List<Trainpat>();
                                lvListPATs.Add(lvTrainpat);
                                lvPATs.Add(lvTrainId, lvListPATs);
                            }
                            else
                            {
                                lvPATs[lvTrainId].Add(lvTrainpat);
                            }

                            break;
                        case "TrainPerformance":
                            break;
                    }
                }
            }

            if (!DateTime.TryParse(ConfigurationManager.AppSettings["INITIAL_DATE"], out mInitialDate))
            {
                mInitialDate = lvCurrTime.Date;
            }

            if (!DateTime.TryParse(ConfigurationManager.AppSettings["FINAL_DATE"], out mFinalDate))
            {
                mFinalDate = mInitialDate.Date == DateTime.Now.Date ? DateTime.Now : mInitialDate.Date.AddDays(1).AddSeconds(-1);
            }

            /* Carrega os dados em usas estruturas */

            Segment.SetList(lvSegments);
            Segment.SetListSwitch(lvSwitches);
            StopLocation.LoadList(pStrFile);
            Segment.LoadStopLocations(StopLocation.GetList());

            for (int i = 0; i < lvTrainMovList.Count; i++)
            {
                lvGene = lvTrainMovList[i].Last;

                lvGene.SegmentInstance = Segment.GetSegmentAt(lvGene.Coordinate, lvGene.Track);

                if (lvGene.SegmentInstance != null)
                {
                    lvGene.StopLocation = lvGene.SegmentInstance.OwnerStopLocation;
                }
                else
                {
                    lvGene.StopLocation = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
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

                if (lvGene.StopLocation == null)
                {
                    lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                }
                else
                {
                    lvNextStopLocation = lvGene.StopLocation.GetNextStopSegment(lvGene.Direction);
                }

                DebugLog.Logar(lvGene.ToString());

                if (lvNextStopLocation == null)
                {
                    lvTrainMovList.RemoveAt(i);
                    i--;
                    continue;
                }
                else if ((lvGene.StopLocation == lvEndStopLocation) && (lvGene.StopLocation != null))
                {
                    lvTrainMovList.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (lvEndStopLocation != null)
                {
                    if (lvGene.Direction > 0)
                    {
                        if (lvGene.Coordinate >= lvEndStopLocation.Start_coordinate)
                        {
                            lvTrainMovList.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    else
                    {
                        if (lvGene.Coordinate <= lvEndStopLocation.End_coordinate)
                        {
                            lvTrainMovList.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                }
            }

            for (int i = 0; i < lvPlans.Count; i++)
            {
                lvGene = lvPlans[i].Last;

                lvGene.StartStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Start, lvGene.Direction, out lvIndex);
                if (lvGene.StartStopLocation == null)
                {
                    lvGene.StartStopLocation = StopLocation.GetNextStopSegment(lvGene.Start, lvGene.Direction);
                }

                lvGene.EndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);
                if (lvGene.EndStopLocation == null)
                {
                    lvGene.EndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
                }

                lvGene.StopLocation = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);

                if (lvGene.StopLocation != null)
                {
                    lvSegment = lvGene.StopLocation.GetSegment(lvGene.Direction, lvGene.Track);
                    lvGene.SegmentInstance = lvSegment;
                    lvGene.Track = lvGene.SegmentInstance.Track;
                }


                DebugLog.Logar(lvGene.ToString());
            }

            DebugLog.Logar(" ");

            DebugLog.EnableDebug = lvLogEnable;

            if (lvTestCount >= 1)
            {
                for (int lvTestInd = 0; lvTestInd < lvTestCount; lvTestInd++)
                {
                    DebugLog.LogPath = lvStrInitialLogPath + lvStrFileName + "\\Test_" + (lvTestInd + 1) + "\\";
                    System.IO.Directory.CreateDirectory(DebugLog.LogPath);

                    lvTrainsData = new List<TrainMovement>(lvTrainMovList);

                    lvPlansData = new List<TrainMovement>(lvPlans);

                    try
                    {
                        lvFitness = new RailRoadFitness(TrainIndividual.VMA);
                        //RailRoadFitness.ResetFitnessCall();
                        ((RailRoadFitness)lvFitness).Population = null;
                        lvPopulation = new Population(lvFitness, lvPopulationSize, lvMutationRate, mCrossOverPoints, lvTrainsData, lvPlansData, pPATs: lvPATs);
                        ((RailRoadFitness)lvFitness).Population = lvPopulation;
                        lvPopulation.NicheDistance = lvNicheDistance;
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
                    }

                    if(lvStrMode.Equals("multstart"))
                    {
                        lvPopulation.LocalSearchAll();

                        lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();
                        //lvPopulation.Dump(lvPopulation.GetBestIndividual());
                        lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);
                    }
                    else if (lvStrMode.Equals("rlns"))
                    {
                        if (lvPopulation.UseMaxObjectiveFunctionCall())
                        {
                            lvPopulation.RLNS(0);
                        }
                        else
                        {
                            lvPopulation.RLNS(lvMaxGenerations);
                        }

                        lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();
                        //lvPopulation.Dump(lvPopulation.GetBestIndividual());
                        lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);
                    }
                    else if (lvStrMode.Equals("grasp"))
                    {
                        if (lvPopulation.UseMaxObjectiveFunctionCall())
                        {
                            lvPopulation.GRASP(lvThreshold);
                        }
                        else
                        {
                            lvPopulation.GRASP(lvThreshold, lvMaxGenerations);
                        }

                        lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();
                        //lvPopulation.Dump(lvPopulation.GetBestIndividual());
                        lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);
                    }
                    else
                    {
                        TrainIndividual.IDLog = 0;
                        /*
                        lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();
                        if (lvGeneIndividual != null)
                        {
                            lvGeneIndividual.GenerateFlotFiles(lvStrInitialLogPath + lvStrFileName + "\\");
                        }
                        */

                        /* -------------------- Debug only -------------------- */
                        //lvLogEnable = true;
                        /* ---------------------------------------------------- */

                        DebugLog.EnableDebug = true;
                        DebugLog.Logar(" ");
                        DebugLog.Logar("Individuos = " + lvPopulation.Count);
                        DebugLog.Logar(" ");
                        DebugLog.EnableDebug = lvLogEnable;

                        //((TrainIndividual)lvPopulation.GetBestIndividual()).GenerateFlotFiles(lvStrInitialLogPath + lvStrFileName + "\\");

                        if (lvPopulation.UseMaxObjectiveFunctionCall())
                        {
                            lvIndex = -1;
                            while (!lvPopulation.HasMaxObjectiveFunctionCallReached())
                            {
                                //TrainIndividual.IDLog = ++lvIndex;
                                lvIndex++;

                                DebugLog.EnableDebug = true;
                                DebugLog.Logar("Generation = " + lvIndex);
                                DebugLog.EnableDebug = lvLogEnable;
                                lvRes = lvPopulation.NextGeneration();
                                //lvPopulation.dump(lvPopulation.GetBestIndividual());

                                //lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();
                                //lvGeneIndividual.GenerateFlotFiles(lvStrInitialLogPath + lvStrFileName + "\\");

                                if (!lvRes)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < lvMaxGenerations; i++)
                            {
                                //TrainIndividual.IDLog = i;

                                DebugLog.EnableDebug = true;
                                DebugLog.Logar("Generation = " + i);
                                DebugLog.EnableDebug = lvLogEnable;
                                lvRes = lvPopulation.NextGeneration();

                                ((RailRoadFitness)lvFitness).HeaderResult += "|Ger " + (i + 1);
                                ((RailRoadFitness)lvFitness).LineResult += "|" + lvPopulation.GetBestIndividual().GetFitness();

                                if (!lvRes)
                                {
                                    break;
                                }

                                /*
                                ((TrainIndividual)lvPopulation.GetBestIndividual()).GenerateFlotFiles(lvStrInitialLogPath + lvStrFileName + "\\");
                                lvPopulation.Dump(lvPopulation.GetBestIndividual());
                                */
                            }
                        }

                        lvGeneIndividual = (TrainIndividual)lvPopulation.GetBestIndividual();

                        if (lvGeneIndividual != null)
                        {
                            //DebugLog.EnableDebug = true;
                            //DebugLog.Logar("Melhor = " + lvGeneIndividual.ToString(), false);
                            //DebugLog.Logar("Melhor = \n" + lvGeneIndividual.ToStringAnalyse(), false);

                            //DebugLog.Logar("Melhor Flot = " + lvGeneIndividual.GetFlotSeries(), false);

                            lvGeneIndividual.GenerateFlotFiles(lvStrInitialLogPath + lvStrFileName + "\\");

                            lvPopulation.SaveBestIndividuals();
                            //lvGeneIndividual.Serialize();
                            //DebugLog.Logar("Melhor = " + lvGeneIndividual.ToStringAnalyse(), false);
                            //DebugLog.EnableDebug = lvLogEnable;
                        }
                        else
                        {
                            DebugLog.Logar("A populacao nao possui individuos !", false);
                        }
                    }

                    if (lvTestInd == 0)
                    {
                        ((RailRoadFitness)lvFitness).HeaderResult += "|Arquivo";
                        ((RailRoadFitness)lvFitness).HeaderResult += "|Local Search Call";
                        DebugLog.LogInfo(((RailRoadFitness)lvFitness).HeaderResult, lvStrInitialLogPath + lvStrFileName + "\\result.txt");
                    }
                    ((RailRoadFitness)lvFitness).LineResult += "|" + lvGeneIndividual.GetUniqueId();
                    ((RailRoadFitness)lvFitness).LineResult += "|" + lvPopulation.HillClimbingCallReg;
                    DebugLog.LogInfo(((RailRoadFitness)lvFitness).LineResult, lvStrInitialLogPath + lvStrFileName + "\\result.txt");

                    lvPopulation.Clear();
                }
            }
            else
            {
                Population.LoadPriority(mTrainPriority);
                lvPopulation = new Population(lvFitness, lvPopulationSize, lvMutationRate, mCrossOverPoints, lvTrainMovList, lvPlans);
                lvPopulation.NicheDistance = lvNicheDistance;

                TrainIndividual.IDLog = 0;

                DebugLog.EnableDebug = true;
                DebugLog.Logar(" ");
                DebugLog.Logar("Individuos = " + lvPopulation.Count);
                DebugLog.Logar(" ");
                DebugLog.EnableDebug = lvLogEnable;

                for (int i = 0; i < lvMaxGenerations; i++)
                {
                    //TrainIndividual.IDLog = i;

                    DebugLog.EnableDebug = true;
                    DebugLog.Logar("Generation = " + i);
                    DebugLog.EnableDebug = lvLogEnable;
                    lvPopulation.NextGeneration();
                    //lvPopulation.Dump(lvPopulation.GetIndividualAt(0));
                }

                lvGeneIndividual = (TrainIndividual)lvPopulation.GetIndividualAt(0);

                if (lvGeneIndividual != null)
                {
                    DebugLog.EnableDebug = true;
                    DebugLog.Logar("Melhor = " + lvGeneIndividual.ToString(), false);

                    lvPopulation.SaveBestIndividuals();
                    //lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);

                    //lvGeneIndividual.Serialize();
                    DebugLog.EnableDebug = lvLogEnable;
                }
            }
            lvPopulation.Clear();
        }

        void mTimerXML_Elapsed(object sender, ElapsedEventArgs e)
        {
            string lvInputFile = "";

            lvInputFile = ConfigurationManager.AppSettings["INPUT_FILE_1"];
            if (!String.IsNullOrEmpty(lvInputFile))
            {
                ProcessRailWay(lvInputFile);
            }

            lvInputFile = ConfigurationManager.AppSettings["INPUT_FILE_2"];
            if (!String.IsNullOrEmpty(lvInputFile))
            {
                ProcessRailWay(lvInputFile);
            }

            lvInputFile = ConfigurationManager.AppSettings["INPUT_FILE_3"];
            if (!String.IsNullOrEmpty(lvInputFile))
            {
                ProcessRailWay(lvInputFile);
            }

            lvInputFile = ConfigurationManager.AppSettings["INPUT_FILE_4"];
            if (!String.IsNullOrEmpty(lvInputFile))
            {
                ProcessRailWay(lvInputFile);
            }

            lvInputFile = ConfigurationManager.AppSettings["INPUT_FILE_5"];
            if (!String.IsNullOrEmpty(lvInputFile))
            {
                ProcessRailWay(lvInputFile);
            }
        }

        void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string lvStrTrainAllowed = "";
            string lvBranch = "carajas";
            int lvPopulationSize = 0;
            int lvMaxGenerations = 0;
            int lvMutationRate = 0;
            IFitness<TrainMovement> lvFitness = null;
            Population lvPopulation = null;
            TrainIndividual lvGeneIndividual = null;
            bool lvLogEnable;
            bool lvUsePerformanceData;
            bool lvAllowInertia;
            int lvStrategyFactor = 1;
            int lvMaxDeadLockError = 0;
            int lvMaxParallelThreads = 1;
            int lvIndex;
            double lvNicheDistance = 0.0;
            double lvThreshold;
            bool lvRes = false;

            //            DebugLog.Logar("mTimer_Elapsed fired !");

            try
            { 
                DebugLog.EnableDebug = true;
                lvBranch = ConfigurationManager.AppSettings["RAILWAY_BRANCH"];
                SegmentDataAccess.LoadBranch(lvBranch);

                Segment.LoadList();
                if (mLoadStopLocationsFrom.Equals("xml"))
                {
                    StopLocation.LoadList(ConfigurationManager.AppSettings["LOG_PATH"] + "stoplocations.xml");
                }
                else
                {
                    StopLocation.LoadList();
                }
                Segment.LoadStopLocations(StopLocation.GetList());
                TrainPerformanceControl.LoadDic();

                if (!DateTime.TryParse(ConfigurationManager.AppSettings["INITIAL_DATE"], out mInitialDate))
                {
                    mInitialDate = DateTime.Now.Date;
                }

                if (!DateTime.TryParse(ConfigurationManager.AppSettings["FINAL_DATE"], out mFinalDate))
                {
                    mFinalDate = mInitialDate.Date == DateTime.Now.Date ? DateTime.Now : mInitialDate.Date.AddDays(1).AddSeconds(-1);
                }

                Interdicao.LoadList(mInitialDate, mFinalDate);

                if (!bool.TryParse(ConfigurationManager.AppSettings["DEBUG_LOG_ENABLE"], out lvLogEnable))
                {
                    lvLogEnable = false;
                }
                DebugLog.EnableDebug = lvLogEnable;

                if (!bool.TryParse(ConfigurationManager.AppSettings["USE_DB_PERF_DATA"], out lvUsePerformanceData))
                {
                    lvUsePerformanceData = false;
                }

                if (!bool.TryParse(ConfigurationManager.AppSettings["ALLOW_INERTIA"], out lvAllowInertia))
                {
                    lvAllowInertia = true;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["STRATEGY_FACTOR"], out lvStrategyFactor))
                {
                    lvStrategyFactor = 1;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["MAX_DEAD_LOCK_ERROR"], out lvMaxDeadLockError))
                {
                    lvMaxDeadLockError = 1;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["MAX_PARALLEL_THREADS"], out lvMaxParallelThreads))
                {
                    lvMaxParallelThreads = 1;
                }

                if (!double.TryParse(ConfigurationManager.AppSettings["NICHE_DISTANCE"], out lvNicheDistance))
                {
                    lvNicheDistance = 0.0;
                }

                if (!double.TryParse(ConfigurationManager.AppSettings["THRESHOLD"], out lvThreshold))
                {
                    lvThreshold = 1.0;
                }

                Population.MAX_PARALLEL_THREADS = lvMaxParallelThreads;
                TrainPerformanceControl.UseDBPerformanceData = lvUsePerformanceData;
                TrainIndividual.AllowInertia = lvAllowInertia;
                TrainIndividual.StrategyFactor = lvStrategyFactor;

                TrainIndividual.VMA = double.Parse(ConfigurationManager.AppSettings["VMA"]);
                TrainIndividual.TrainLen = int.Parse(ConfigurationManager.AppSettings["TRAIN_LEN"]);
                TrainIndividual.LimitDays = int.Parse(ConfigurationManager.AppSettings["LIMIT_DAYTIME"]);

                StopLocation.MinLen = TrainIndividual.TrainLen;

                //DebugLog.EnableDebug = true;

                //DebugLog.Logar("TrainIndividual.VMA = " + TrainIndividual.VMA);
                //DebugLog.Logar("TrainIndividual.TrainLen = " + TrainIndividual.TrainLen);

                lvPopulationSize = int.Parse(ConfigurationManager.AppSettings["POPULATION_SIZE"]);
                lvMaxGenerations = int.Parse(ConfigurationManager.AppSettings["MAX_GENERATIONS"]);
                lvMutationRate = int.Parse(ConfigurationManager.AppSettings["MUTATION_RATE"]);
                lvStrTrainAllowed = ConfigurationManager.AppSettings["TRAIN_TYPE_ALLOWED"];

                Population.TrainAllowed = lvStrTrainAllowed;
                Population.MAX_DEAD_LOCK_ERROR = lvMaxDeadLockError;

                /*
                DebugLog.Logar("lvPopulationSize = " + lvPopulationSize);
                DebugLog.Logar("lvMaxGenerations = " + lvMaxGenerations);
                DebugLog.Logar("lvMutationRate = " + lvMutationRate);
                DebugLog.Logar("lvStrTrainAllowed = " + lvStrTrainAllowed);

                DebugLog.Logar("lvInitialDate = " + mInitialDate);
                DebugLog.Logar("lvFinalDate = " + mFinalDate);

                DebugLog.Logar(" ");

                DebugLog.EnableDebug = lvLogEnable;
                */

                lvFitness = new RailRoadFitness(TrainIndividual.VMA);
                //RailRoadFitness.ResetFitnessCall();
                ((RailRoadFitness)lvFitness).Population = null;
                Population.LoadPriority(mTrainPriority);
                lvPopulation = new Population(lvFitness, lvPopulationSize, lvMutationRate, mCrossOverPoints, null, null, mInitialDate, mFinalDate);
                ((RailRoadFitness)lvFitness).Population = lvPopulation;
                lvPopulation.NicheDistance = lvNicheDistance;

#if !DEBUG
                ElapsedTimeDataAccess.Delete(lvPopulation.UniqueId);

                lvGeneIndividual = null;
                if (lvPopulation.Count > 0)
                {
                    lvGeneIndividual = (TrainIndividual)lvPopulation.GetIndividualAt(0);
                }

                if (lvGeneIndividual != null)
                {
                    ElapsedTimeDataAccess.Insert(lvPopulation.UniqueId, lvFitness.Type, lvPopulation.Count, lvGeneIndividual.Fitness);
                }
                else
                {
                    ElapsedTimeDataAccess.Insert(lvPopulation.UniqueId, lvFitness.Type, lvPopulation.Count, 0.0);
                }
#endif
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);

                return;
            }

            try
            {

                TrainIndividual.IDLog = 0;

            /*
            DebugLog.Logar(" ");
            DebugLog.Logar("Individuos = " + lvPopulation.Count);
            DebugLog.Logar(" ");
            */

                if (lvPopulation.UseMaxObjectiveFunctionCall())
                {
                    lvIndex = -1;
                    while (!lvPopulation.HasMaxObjectiveFunctionCallReached())
                    {
                        TrainIndividual.IDLog = ++lvIndex;

                        //DebugLog.Logar("Generation = " + lvIndex);
                        //DebugLog.EnableDebug = lvLogEnable;
                        try
                        {
                            lvRes = lvPopulation.NextGeneration();
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
                        }

                        //lvPopulation.dump(lvPopulation.GetBestIndividual());

                        if (!lvRes)
                        {
                            DebugLog.Logar("Processo abortado na execucao da geracao !!!", false, pIndet: TrainIndividual.IDLog);
                            DebugLog.Save("Processo abortado na execucao da geracao !!!");
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < lvMaxGenerations; i++)
                    {
                        TrainIndividual.IDLog = i;

                        //DebugLog.Logar("Generation = " + i);
                        //DebugLog.EnableDebug = lvLogEnable;
                        try
                        {
                            lvRes = lvPopulation.NextGeneration();
                        }
                        catch (Exception ex)
                        {
                            DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
                        }

                        if (!lvRes)
                        {
                            break;
                        }
                    }
                }

                if (lvPopulation != null)
                {
                    lvGeneIndividual = (TrainIndividual)lvPopulation.GetIndividualAt(0);

                    if (lvGeneIndividual != null)
                    {
                        //DebugLog.EnableDebug = true;
                        //DebugLog.Logar("Melhor = " + lvGeneIndividual.ToString(), false);

#if DEBUG
                        lvGeneIndividual.GenerateFlotFiles(DebugLog.LogPath);
#else
                        lvGeneIndividual.Save();
                        ElapsedTimeDataAccess.Update(lvPopulation.UniqueId, DateTime.Now, lvFitness.FitnessCallNum, lvGeneIndividual.Fitness, lvPopulation.CurrentGeneration, lvPopulation.Count, lvPopulation.HillClimbingCallReg);
#endif

                        lvPopulation.SaveBestIndividuals();

                        //DebugLog.Logar("Melhor = " + lvGeneIndividual.ToStringAnalyse(), false);

                        //DebugLog.Logar("Flot Res = " + lvGeneIndividual.GetFlotSeries(), false);
                        //DebugLog.EnableDebug = lvLogEnable;
                    }
                    lvPopulation = null;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            }

            this.mTimer.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            try
            {
                mTimer.Enabled = false;
                mTimer = null;
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex, false, pIndet: TrainIndividual.IDLog);
            }

            DebugLog.Logar("TrainOptimizer finalizado !", false);
            eventLog1.WriteEntry("TrainOptimizer finalizado !");
        }

        protected override void OnPause()
        {
            base.OnPause();

            DebugLog.Logar("TrainOptimizer interrompido !", false);
            eventLog1.WriteEntry("TrainOptimizer interrompido !");
        }

        protected override void OnContinue()
        {
            base.OnContinue();

            DebugLog.Logar("TrainOptimizer resumido !", false);
            eventLog1.WriteEntry("TrainOptimizer resumido !");
        }
    }
}
