using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
/// <summary>
/// Criado por Eggo Pinheiro em 13/02/2015 14:03:59
/// <summary>

[DataObject(true)]
public class TrainmovsegmentDataAccess
{
    private static Dictionary<string, Segment> mDicSegments = new Dictionary<string, Segment>();

    public TrainmovsegmentDataAccess()
    {
        //
        //TODO: Add constructor logic here
        //
    }

    [DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(double ptrain_id, DateTime pdata_ocup, DateTime pdata_desocup, Int16 plocation, string pud, Int16 pdirection, Int16 ptrack, DateTime pdate_hist, int pcoordinate)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "insert into tbtrainmovsegment(train_id, data_ocup, data_desocup, location, ud, direction, track, date_hist, coordinate) ";
        lvSql += "values(@train_id, @data_ocup, @data_desocup, @location, @ud, @direction, @track, @date_hist, @coordinate)";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_desocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = pdata_desocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (plocation == Int16.MinValue)
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
        }

        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;

        if (pdirection == Int16.MinValue)
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
        }

        if (ptrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
        }

        if (pdate_hist == DateTime.MinValue)
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pcoordinate == Int32.MinValue)
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
        }

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Update)]
    public static int Update(double ptrain_id, DateTime pdata_ocup, DateTime pdata_desocup, Int16 plocation, string pud, Int16 pdirection, Int16 ptrack, DateTime pdate_hist, int pcoordinate)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "update tbtrainmovsegment set data_desocup=@data_desocup, location=@location, ud=@ud, direction=@direction, track=@track, date_hist=@date_hist, coordinate=@coordinate Where train_id=@train_id And data_ocup=@data_ocup";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_desocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = pdata_desocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (plocation == Int16.MinValue)
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
        }

        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pud;
        if (pdirection == Int16.MinValue)
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
        }

        if (ptrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
        }

        if (pdate_hist == DateTime.MinValue)
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pcoordinate == Int32.MinValue)
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
        }

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Update)]
    public static int UpdateKey(double pOrigtrain_id, DateTime pOrigdata_ocup, double ptrain_id, DateTime pdata_ocup)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "update tbtrainmovsegment set train_id=@train_id, data_ocup=@data_ocup Where train_id=@origtrain_id And data_ocup=@origdata_ocup";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pOrigtrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = pOrigtrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pOrigdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@origdata_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@origdata_ocup", MySqlDbType.DateTime).Value = pOrigdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Delete)]
    public static int Delete(double ptrain_id, DateTime pdata_ocup)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "delete from tbtrainmovsegment Where train_id=@train_id And data_ocup=@data_ocup";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("TrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetAll()
    {
        string lvSql = "";
        DataSet ds = new DataSet();

        lvSql = "select * from tbtrainmovsegment";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);
        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

    public static void LoadSegments()
    {
        string lvSql;
        DataSet ds = new DataSet();
        Segment lvSegment = null;

        mDicSegments.Clear();

        lvSql = "select location, segment, start_coordinate, end_coordinate from tbsegment";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbsegment");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvSegment = new Segment();
            lvSegment.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvSegment.SegmentValue = ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString());
            lvSegment.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvSegment.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);

            mDicSegments.Add(lvSegment.Location + "." + lvSegment.SegmentValue, lvSegment);
            lvSegment = null;
        }
    }

    public static DateTime GetHeadWayEnd(double pTrainId, Segment pSegment, StopLocation pNextSegment, int pLocation, string pStrUD, int pDirection, Int16 pDestTrack, out DateTime pOcupTime)
    {
        DateTime lvRes = DateTime.MinValue;
        DateTime lvOcupTime = DateTime.MinValue;
        Segment lvSegment;
        string lvStrUD = "";
        int lvLocation = Int32.MinValue;
        int lvIndex;

        pOcupTime = DateTime.MinValue;

        DebugLog.Logar(" --------------------------------------------- GetHeadWayEnd(pTrainId = " + pTrainId + ", pLocation = " + pLocation + ", pStrUD = " + pStrUD + ", pDirection = " + pDirection + ", pDestTrack = " + pDestTrack + ") ------------------------------ ");

        if (pDirection > 0)
        {
            lvSegment = Segment.GetCurrentSegment(pNextSegment.Start_coordinate, pDirection, pDestTrack, out lvIndex);
        }
        else if (pDirection < 0)
        {
            lvSegment = Segment.GetCurrentSegment(pNextSegment.End_coordinate, pDirection, pDestTrack, out lvIndex);
        }
        else
        {
            return DateTime.MinValue;
        }

        lvRes = GetHeadWayEndTime(pTrainId, lvSegment.Location, lvSegment.SegmentValue, pDestTrack, out pOcupTime);

        if (!lvSegment.SegmentValue.StartsWith("CV03") && (!lvSegment.SegmentValue.StartsWith("SW")))
        {
            if (pDirection > 0)
            {
                if (lvSegment.SegmentValue.StartsWith("CDV_") || lvSegment.SegmentValue == "WT")
                {
                    lvStrUD = pStrUD;
                    lvLocation = pLocation;
                    lvRes = GetHeadWayEndTime(pTrainId, lvLocation, lvStrUD, 0, out lvOcupTime);
                }
            }
            else if (pDirection < 0)
            {
                if (lvSegment.SegmentValue.StartsWith("CDV_") || lvSegment.SegmentValue == "WT")
                {
                    lvStrUD = pStrUD;
                    lvLocation = pLocation;
                    lvRes = GetHeadWayEndTime(pTrainId, lvLocation, lvStrUD, 0, out lvOcupTime);
                }
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        if (pOcupTime == DateTime.MinValue)
        {
            lvRes = DateTime.MinValue;
        }

        DebugLog.Logar(" ---------------------------------------------------------------------------- Fim de GetHeadWayEnd ---------------------------------------------------------------------------- ");

        return lvRes;
    }

    private static DateTime GetHeadWayEndTime(double pTrainId, int pLocation, string pStrSegment, Int16 pDestTrack, out DateTime pOcupTime)
    {
        DateTime lvRes = DateTime.MinValue;
        pOcupTime = DateTime.MinValue;
        DataSet ds = new DataSet();
        string lvSql = "";

        DebugLog.Logar(" --------------------------------------------- GetHeadWayEndTime(pTrainId = " + pTrainId + ", pLocation = " + pLocation + ", pStrSegment = " + pStrSegment + ", pDestTrack = " + pDestTrack + ") ------------------------------ ");

        if (pDestTrack == 0)
        {
            lvSql = "select tbtrainmovsegment.train_id,  tbtrainmovsegment.direction, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.track from tbtrainmovsegment where train_id=@trainid and tbtrainmovsegment.location=@location and tbtrainmovsegment.ud=@ud order by data_desocup, data_ocup limit 1";
        }
        else
        {
            lvSql = "select tbtrainmovsegment.train_id,  tbtrainmovsegment.direction, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.track from tbtrainmovsegment where train_id=@trainid and tbtrainmovsegment.location=@location and tbtrainmovsegment.ud=@ud and tbtrainmovsegment.track=@track order by data_desocup, data_ocup limit 1";
        }
        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@trainid", MySqlDbType.Double).Value = pTrainId;
        cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = pLocation;
        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pStrSegment;
        cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = pDestTrack;

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtraindataheadway");

        ConnectionManager.DebugMySqlQuery(cmd, "GetHeadWayEndTime.lvSql");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvRes = ((row["data_desocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_desocup"].ToString()));
            pOcupTime = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));

            if(lvRes == DateTime.MinValue)
            {
                lvRes = pOcupTime;
                DebugLog.Logar("Usando lvOcupTime...");
            }
        }

        DebugLog.Logar(" --------------------------------------------------------------- Fim de GetHeadWayEndTime --------------------------------------------------------------- ");

        return lvRes;
    }

    public static double GetMeanTimeHist(double pTrainId, string pStrTrainType, Segment pSegment, StopLocation pNextSegment, int pDirection, int pLocation, string pStrUD, DateTime pOcupTime, int pTrainAnalysisCount, double pTrainMinSpeed, double pTrainMaxSpeed, Int16 pDestTrack, out double pMeanTime)
    {
        double lvRes;
        int lvCountTimeHeadWay = 0;
        int lvCountTime = 0;
        int lvStartCoordinate = Int32.MinValue;
        int lvEndCoordinate = Int32.MinValue;
        double lvTrainId;
        string lvStrTrainName;
        DateTime lvDepTime;
        DateTime lvStartOcupTime;
        DateTime lvEndDesOcupTime;
        DateTime lvEndOcupTime;
        double lvMeanSpeed;
        double lvMeanSpeedHeadWay;
        double lvTimeHeadWay;
        double lvTime;
        double lvMeanTimeHeadWay = 0.0;
        double lvMeanTime = 0.0;
        HashSet<double> lvTrainList = new HashSet<double>();

        DataSet ds = new DataSet();

        DebugLog.Logar("");
        DebugLog.Logar(" --------------------------------------------- GetMeanTimeHist(pTrainId = " + pTrainId + ", pStrTrainType = " + pStrTrainType + ", pDirection = " + pDirection + ", pLocation = " + pLocation + ", pStrUD = " + pStrUD + ", pOcupTime = " + pOcupTime + ", pTrainAnalysisCount = " + pTrainAnalysisCount + ", pTrainMinSpeed = " + pTrainMinSpeed + ", pTrainMaxSpeed = " + pTrainMaxSpeed + ", pDestTrack = " + pDestTrack + ") ------------------------------ ");

        string lvSql = "select tbtrain.train_id, tbtrain.name, tbtrain.departure_time, tbtrainmovsegment.data_ocup, tbtrainmovsegment.coordinate from tbtrain, tbtrainmovsegment where tbtrain.train_id=tbtrainmovsegment.train_id and tbtrain.train_id<>@trainid and tbtrain.name like '" + pStrTrainType + "%' and tbtrainmovsegment.direction=@direction and tbtrainmovsegment.location=@location and tbtrainmovsegment.ud=@ud and data_ocup<@ocuptime order by data_ocup desc limit " + pTrainAnalysisCount;

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@trainid", MySqlDbType.Double).Value = pTrainId;
        cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pDirection;
        cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = pLocation;
        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = pStrUD;
        cmd.Parameters.Add("@ocuptime", MySqlDbType.DateTime).Value = pOcupTime;

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtraindata");

        ConnectionManager.DebugMySqlQuery(cmd, "GetMeanSpeedHist.lvSql");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvTrainId = ((row["train_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["train_id"]);

            if (!lvTrainList.Contains(lvTrainId))
            {
                lvStrTrainName = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());
                lvDepTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                lvStartOcupTime = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                lvStartCoordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);

                DebugLog.Logar(" ");
                DebugLog.Logar("Trem = " + lvTrainId + " : " + lvStrTrainName + " : lvStartOcupTime = " + lvStartOcupTime + " : lvStartCoordinate = " + lvStartCoordinate);

                lvEndDesOcupTime = GetHeadWayEnd(lvTrainId, pSegment, pNextSegment, pLocation, pStrUD, pDirection, pDestTrack, out lvEndOcupTime);
                DebugLog.Logar("lvEndDesOcupTime = " + lvEndDesOcupTime);
                DebugLog.Logar("lvEndOcupTime = " + lvEndOcupTime);
                DebugLog.Logar("lvStartOcupTime = " + lvStartOcupTime);
                DebugLog.Logar("lvStartCoordinate = " + lvStartCoordinate);

                if (pDirection > 0)
                {
                    lvEndCoordinate = pNextSegment.Start_coordinate;
                }
                else if (pDirection < 0)
                {
                    lvEndCoordinate = pNextSegment.End_coordinate;
                }
                DebugLog.Logar("lvEndCoordinate = " + lvEndCoordinate);

                lvMeanSpeed = (Math.Abs(lvEndCoordinate - lvStartCoordinate) / 100000.0) / (lvEndOcupTime - lvStartOcupTime).TotalHours;
                lvMeanSpeedHeadWay = (Math.Abs(lvEndCoordinate - lvStartCoordinate) / 100000.0) / (lvEndDesOcupTime - lvStartOcupTime).TotalHours;

                DebugLog.Logar("lvMeanSpeed = " + lvMeanSpeed);
                DebugLog.Logar("lvMeanSpeedHeadWay = " + lvMeanSpeedHeadWay);

                if (lvEndDesOcupTime > DateTime.MinValue)
                {
                    lvTimeHeadWay = (lvEndDesOcupTime - lvStartOcupTime).TotalMinutes;
                    lvTime = (lvEndOcupTime - lvStartOcupTime).TotalMinutes;

                    if (lvTime < 0)
                    {
                        lvTime = 0;
                    }

                    DebugLog.Logar("lvTimeHeadWay = " + lvTimeHeadWay);
                    DebugLog.Logar("lvTime = " + lvTime);

                    if ((lvMeanSpeedHeadWay > pTrainMinSpeed) && (lvMeanSpeedHeadWay < pTrainMaxSpeed))
                    {
                        lvMeanTimeHeadWay += lvTimeHeadWay;
                        lvCountTimeHeadWay++;
                    }

                    if ((lvMeanSpeed > pTrainMinSpeed) && (lvMeanSpeed < pTrainMaxSpeed))
                    {
                        lvMeanTime += lvTime;
                        lvCountTime++;
                    }
                }
            }
            lvTrainList.Add(lvTrainId);
        }

        if (lvCountTimeHeadWay > 0)
        {
            lvMeanTimeHeadWay /= lvCountTimeHeadWay;
        }

        if (lvCountTime > 0)
        {
            lvMeanTime /= lvCountTime;
        }

        pMeanTime = lvMeanTime;
        lvRes = lvMeanTimeHeadWay;

        DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ");

        return lvRes;
    }

    public static double GetMeanSpeed(double ptrain_id, DateTime pLimitDate, double pVMA)
    {
        double lvRes;
        int lvCoordinate;
        int lvDirection;
        int lvLocation;
        string lvStrUD;
        DateTime lvOcupTime;

        lvRes = GetMeanSpeed(ptrain_id, pLimitDate, pVMA, out lvCoordinate, out lvDirection, out lvLocation, out lvStrUD, out lvOcupTime);

        return lvRes;
    }

    public static double GetMeanSpeed(double ptrain_id, DateTime pLimitDate, double pVMA, out int pCoordinate, out int pDirection, out int pLocation, out string pStrUD, out DateTime pOcupTime)
    {
        double lvRes = 0.0;
        DateTime lvinitOcupTime = DateTime.MinValue;
        DateTime lvEndOcupTime = DateTime.MinValue;
        int lvInitCoordinate = 0;
        int lvEndCoordinate = 0;
        int lvCount;
        string lvSql;

        pCoordinate = Int32.MinValue;
        pDirection = 0;
        pLocation = 0;
        pStrUD = "";
        pOcupTime = DateTime.MinValue;
        DataSet ds = new DataSet();

        if (pLimitDate == DateTime.MinValue)
        {
            lvSql = "select location, ud, data_ocup, data_desocup, direction, track, coordinate from tbtrainmovsegment WHERE train_id=@trainId and ud <> 'SBA' order by data_ocup desc limit 2";
        }
        else
        {
            lvSql = "select location, ud, data_ocup, data_desocup, direction, track, coordinate from tbtrainmovsegment WHERE train_id=@trainId and data_ocup<=@limitdate and ud <> 'SBA' order by data_ocup desc limit 2";
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@trainId", MySqlDbType.Double).Value = ptrain_id;

        if (pLimitDate > DateTime.MinValue)
        {
            cmd.Parameters.Add("@limitdate", MySqlDbType.DateTime).Value = pLimitDate;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtraindata");

//        ConnectionManager.DebugMySqlQuery(cmd, "GetMeanSpeed.lvSql");

        lvCount = 0;
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvCount++;

            if (lvCount == 1)
            {
                lvEndOcupTime = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                lvEndCoordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
                pDirection = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                pLocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : (Int16)row["location"]);
                pStrUD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                pOcupTime = lvEndOcupTime;

                pCoordinate = lvEndCoordinate;
            }

            if (lvCount == 2)
            {
                lvinitOcupTime = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                lvInitCoordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
            }
        }

        if (lvCount == 2)
        {
            lvRes = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / (lvEndOcupTime - lvinitOcupTime).TotalHours;
        }

        /*
        if(lvRes > pVMA)
        {
            DebugLog.Logar("\n Velocidade inicial muito alta:" + lvRes);
            DebugLog.Logar("lvEndOcupTime = " + lvEndOcupTime);
            DebugLog.Logar("lvinitOcupTime = " + lvinitOcupTime);
            DebugLog.Logar("lvEndCoordinate = " + lvEndCoordinate);
            DebugLog.Logar("lvInitCoordinate = " + lvInitCoordinate);
            DebugLog.Logar("");

            ConnectionManager.DebugMySqlQuery(cmd, "GetMeanSpeed.lvSql");

            lvRes = pVMA;
        }
        */

        return lvRes;
    }

    public static DataSet GetCurrentTrainsData(DateTime pInitialDate, DateTime pFinalDate)
    {
        DataSet ds = new DataSet();
        string lvSql;

        if (pInitialDate.Date < DateTime.Now.Date)
        {
            lvSql = "select t1.train_id, t1.data_ocup, t1.location, t1.ud, t1.direction, t1.track, t1.coordinate, tbplan.plan_id plan_id, tbplan.origem, tbplan.destino, tbtrain.name, tbtrain.departure_time, tbtrain.creation_tm, tbplan.departure_time plan_departure_time from tbtrainmovsegment t1 join (select train_id, max(data_ocup) data_ocup from tbtrainmovsegment where (data_ocup BETWEEN @InitialDate AND @FinalDate) group by train_id order by data_ocup desc) t2 on (t1.train_id=t2.train_id and t1.data_ocup=t2.data_ocup) Join tbtrain on (t1.train_id=tbtrain.train_id) left join tbplan on (tbtrain.plan_id=tbplan.plan_id) where tbtrain.name not like 'X%' order by tbtrain.departure_time asc";
        }
        else
        {
            lvSql = "select t1.train_id, t1.data_ocup, t1.location, t1.ud, t1.direction, t1.track, t1.coordinate, tbplan.plan_id plan_id, tbplan.origem, tbplan.destino, tbtrain.name, tbtrain.departure_time, tbtrain.creation_tm, tbplan.departure_time plan_departure_time from tbtrainmovsegment t1 join (select train_id, max(data_ocup) data_ocup from tbtrainmovsegment where (data_ocup BETWEEN @InitialDate AND @FinalDate) group by train_id order by data_ocup desc) t2 on (t1.train_id=t2.train_id and t1.data_ocup=t2.data_ocup) Join tbtrain on (t1.train_id=tbtrain.train_id) left join tbplan on (tbtrain.plan_id=tbplan.plan_id) where tbtrain.name not like 'X%' and (tbtrain.status = 'Circulando' Or tbtrain.status = 'Planejado') order by tbtrain.departure_time asc";
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@InitialDate", MySqlDbType.DateTime).Value = pInitialDate;
        cmd.Parameters.Add("@FinalDate", MySqlDbType.DateTime).Value = pFinalDate;

        cmd.CommandType = CommandType.Text;

        conn.Close();

        ConnectionManager.DebugMySqlQuery(cmd, "GetCurrentTrainsData.lvSql");

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtraindata");

        return ds;
    }
    
    public static DataSet GetTrainData(DateTime pInitialDate, DateTime pFinalDate, string pStrTrainType = "", int pDirection = 0, string pStrSortField = "")
    {
        DataSet ds = new DataSet();
        string lvSqlWhere = "";
        string lvSql;

        if (pInitialDate.Date < DateTime.Now.Date)
        {
            lvSql = "select distinct tbtrain.train_id, tbtrain.name, tbtrain.departure_time, tbtrain.status FROM tbtrain JOIN tbtrainmovsegment ON tbtrain.train_id = tbtrainmovsegment.train_id WHERE tbtrain.name NOT LIKE 'X%' AND tbtrainmovsegment.data_ocup BETWEEN @InitialDate AND @FinalDate";
        }
        else
        {
            lvSql = "select distinct tbtrain.train_id, tbtrain.name, tbtrain.departure_time, tbtrain.status FROM tbtrain JOIN tbtrainmovsegment ON tbtrain.train_id = tbtrainmovsegment.train_id WHERE tbtrain.name NOT LIKE 'X%' AND tbtrainmovsegment.data_ocup BETWEEN @InitialDate AND @FinalDate And (tbtrain.status = 'Circulando' Or tbtrain.status = 'Planejado')";
        }

        if (pStrTrainType.Length > 0)
        {
            lvSqlWhere += " And tbtrain.name like @traintype";
        }

        if(pDirection != 0)
        {
            lvSqlWhere += " And tbtrainmovsegment.direction = @direction";
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (pStrSortField.Length > 0)
        {
            lvSql += " order by " + pStrSortField;
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@InitialDate", MySqlDbType.DateTime).Value = pInitialDate;
        cmd.Parameters.Add("@FinalDate", MySqlDbType.DateTime).Value = pFinalDate;

        if (pStrTrainType.Length > 0)
        {
            lvSql += " And tbtrain.name like @traintype";
        }

        if (pDirection != 0)
        {
            lvSql += " And tbtrainmovsegment.direction = @direction";
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtraindata");

        ConnectionManager.DebugMySqlQuery(cmd, "GetTrainData.lvSql");

        return ds;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetData(double ptrain_id = -999999999999, DateTime pdata_ocup = default(DateTime), DateTime pdata_desocup = default(DateTime), Int16 plocation = -32768, string pud = "", Int16 pdirection = -32768, Int16 ptrack = -32768, DateTime pdate_hist = default(DateTime), int pcoordinate = -2147483648, Boolean pUseForeignKey = false, string pStrSortField = "")
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        if (pUseForeignKey)
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbtrainmovsegment Left Join tbtrain on tbtrainmovsegment.train_id=tbtrain.train_id";
        }
        else
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate From tbtrainmovsegment";
        }

        if (ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.train_id=@train_id";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.train_id=@train_id";
            }
        }

        if (pdata_ocup > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_ocup=@data_ocup";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_ocup=@data_ocup";
            }
        }

        if (pdata_desocup > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_desocup=@data_desocup";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_desocup=@data_desocup";
            }
        }

        if (plocation > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.location=@location";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.location=@location";
            }
        }

        if (!string.IsNullOrEmpty(pud))
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.ud like @ud";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.ud like @ud";
            }
        }

        if (pdirection > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.direction=@direction";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.direction=@direction";
            }
        }

        if (ptrack > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.track=@track";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.track=@track";
            }
        }

        if (pdate_hist > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.date_hist=@date_hist";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.date_hist=@date_hist";
            }
        }

        if (pcoordinate > Int32.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.coordinate=@coordinate";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.coordinate=@coordinate";
            }
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        DebugLog.Logar("lvSql = " + lvSql);
        DebugLog.Logar("ptrain_id = " + ptrain_id);

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_desocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_desocup", MySqlDbType.DateTime).Value = pdata_desocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (plocation == Int16.MinValue)
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
        }

        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = "%" + pud + "%";

        if (pdirection == Int16.MinValue)
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@direction", MySqlDbType.Int16).Value = pdirection;
        }

        if (ptrack == Int16.MinValue)
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
        }

        if (pdate_hist == DateTime.MinValue)
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pcoordinate == Int32.MinValue)
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetDataByRange(double ptrain_idInicio = -999999999999, double ptrain_idFim = -999999999999, DateTime pdata_ocupInicio = default(DateTime), DateTime pdata_ocupFim = default(DateTime), DateTime pdata_desocupInicio = default(DateTime), DateTime pdata_desocupFim = default(DateTime), Int16 plocationInicio = -32768, Int16 plocationFim = -32768, string pud = "", Int16 pdirectionInicio = -32768, Int16 pdirectionFim = -32768, Int16 ptrackInicio = -32768, Int16 ptrackFim = -32768, DateTime pdate_histInicio = default(DateTime), DateTime pdate_histFim = default(DateTime), int pcoordinateInicio = -2147483648, int pcoordinateFim = -2147483648, Boolean pUseForeignKey = false, string pStrSortField = "")
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        if (pUseForeignKey)
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbtrainmovsegment Left Join tbtrain on tbtrainmovsegment.train_id=tbtrain.train_id";
        }
        else
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate From tbtrainmovsegment";
        }

        if (ptrain_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.train_id >= @train_idInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.train_id >= @train_idInicio";
            }
        }

        if (ptrain_idFim > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.train_id <= @train_idFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.train_id <= @train_idFim";
            }
        }

        if (pdata_ocupInicio > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_ocup >= @data_ocupInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_ocup >= @data_ocupInicio";
            }
        }

        if (pdata_ocupFim > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_ocup <= @data_ocupFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_ocup <= @data_ocupFim";
            }
        }

        if (pdata_desocupInicio > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_desocup >= @data_desocupInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_desocup >= @data_desocupInicio";
            }
        }

        if (pdata_desocupFim > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_desocup <= @data_desocupFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_desocup <= @data_desocupFim";
            }
        }

        if (plocationInicio > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.location >= @locationInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.location >= @locationInicio";
            }
        }

        if (plocationFim > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.location <= @locationFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.location <= @locationFim";
            }
        }

        if (!string.IsNullOrEmpty(pud))
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.ud like @ud";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.ud like @ud";
            }
        }

        if (pdirectionInicio > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.direction >= @directionInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.direction >= @directionInicio";
            }
        }

        if (pdirectionFim > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.direction <= @directionFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.direction <= @directionFim";
            }
        }

        if (ptrackInicio > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.track >= @trackInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.track >= @trackInicio";
            }
        }

        if (ptrackFim > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.track <= @trackFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.track <= @trackFim";
            }
        }

        if (pdate_histInicio > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.date_hist >= @date_histInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.date_hist >= @date_histInicio";
            }
        }

        if (pdate_histFim > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.date_hist <= @date_histFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.date_hist <= @date_histFim";
            }
        }

        if (pcoordinateInicio > Int32.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.coordinate >= @coordinateInicio";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.coordinate >= @coordinateInicio";
            }
        }

        if (pcoordinateFim > Int16.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.coordinate <= @coordinateFim";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.coordinate <= @coordinateFim";
            }
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        DebugLog.Logar("lvSql = " + lvSql);

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = ptrain_idInicio;
        }

        if (ptrain_idFim == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = ptrain_idFim;
        }

        if (pdata_ocupInicio == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocupInicio", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocupInicio", MySqlDbType.DateTime).Value = pdata_ocupInicio.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_ocupFim == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocupFim", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocupFim", MySqlDbType.DateTime).Value = pdata_ocupFim.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_desocupInicio == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_desocupInicio", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_desocupInicio", MySqlDbType.DateTime).Value = pdata_desocupInicio.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdata_desocupFim == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_desocupFim", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_desocupFim", MySqlDbType.DateTime).Value = pdata_desocupFim.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (plocationInicio == Int16.MinValue)
        {
            cmd.Parameters.Add("@locationInicio", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@locationInicio", MySqlDbType.Int16).Value = plocationInicio;
        }

        if (plocationFim == Int16.MinValue)
        {
            cmd.Parameters.Add("@locationFim", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@locationFim", MySqlDbType.Int16).Value = plocationFim;
        }

        cmd.Parameters.Add("@ud", MySqlDbType.String).Value = "%" + pud + "%";

        if (pdirectionInicio == Int16.MinValue)
        {
            cmd.Parameters.Add("@directionInicio", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@directionInicio", MySqlDbType.Int16).Value = pdirectionInicio;
        }

        if (pdirectionFim == Int16.MinValue)
        {
            cmd.Parameters.Add("@directionFim", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@directionFim", MySqlDbType.Int16).Value = pdirectionFim;
        }

        if (ptrackInicio == Int16.MinValue)
        {
            cmd.Parameters.Add("@trackInicio", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@trackInicio", MySqlDbType.Int16).Value = ptrackInicio;
        }

        if (ptrackFim == Int16.MinValue)
        {
            cmd.Parameters.Add("@trackFim", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@trackFim", MySqlDbType.Int16).Value = ptrackFim;
        }

        if (pdate_histInicio == DateTime.MinValue)
        {
            cmd.Parameters.Add("@date_histInicio", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@date_histInicio", MySqlDbType.DateTime).Value = pdate_histInicio.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdate_histFim == DateTime.MinValue)
        {
            cmd.Parameters.Add("@date_histFim", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@date_histFim", MySqlDbType.DateTime).Value = pdate_histFim.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pcoordinateInicio == Int32.MinValue)
        {
            cmd.Parameters.Add("@coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@coordinateInicio", MySqlDbType.Int32).Value = pcoordinateInicio;
        }

        if (pcoordinateFim == Int32.MinValue)
        {
            cmd.Parameters.Add("@coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@coordinateFim", MySqlDbType.Int32).Value = pcoordinateFim;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetDataByKey(double ptrain_id = -999999999999, DateTime pdata_ocup = default(DateTime), Boolean pUseForeignKey = false, string pStrSortField = "")
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        if (pUseForeignKey)
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbtrainmovsegment Left Join tbtrain on tbtrainmovsegment.train_id=tbtrain.train_id";
        }
        else
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate From tbtrainmovsegment";
        }

        if (ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.train_id=@train_id";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.train_id=@train_id";
            }
        }

        if (pdata_ocup > DateTime.MinValue)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.data_ocup=@data_ocup";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.data_ocup=@data_ocup";
            }
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        if (pdata_ocup == DateTime.MinValue)
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@data_ocup", MySqlDbType.DateTime).Value = pdata_ocup.ToString("yyyy/MM/dd HH:mm:ss");
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetDataByTrain(double ptrain_id = -999999999999, Boolean pUseForeignKey = false, string pStrSortField = "")
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        if (pUseForeignKey)
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbtrainmovsegment Left Join tbtrain on tbtrainmovsegment.train_id=tbtrain.train_id";
        }
        else
        {
            lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate From tbtrainmovsegment";
        }

        lvSqlWhere = "tbtrainmovsegment.train_id=@train_id";

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetCrossData(double ptrain_id, string pStrSortField)
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        lvSql = "select tbtrainmovsegment.train_id, tbtrainmovsegment.data_ocup, tbtrainmovsegment.data_desocup, tbtrainmovsegment.location, tbtrainmovsegment.ud, tbtrainmovsegment.direction, tbtrainmovsegment.track, tbtrainmovsegment.date_hist, tbtrainmovsegment.coordinate, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbtrainmovsegment Inner Join tbtrain on tbtrainmovsegment.train_id=tbtrain.train_id";

        if (ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbtrainmovsegment.train_id=@train_id";
            }
            else
            {
                lvSqlWhere += " And tbtrainmovsegment.train_id=@train_id";
            }
        }


        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainmovsegment");
        return ds;
    }

}

