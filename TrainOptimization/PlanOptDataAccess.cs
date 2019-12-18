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
public class PlanOptDataAccess
{
    public PlanOptDataAccess()
    {
        //
        //TODO: Add constructor logic here
        //
    }

    [DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(long ptrain_id, string ptrainname, long ptimevalue, double pposition, Int16 pTrack, Int64 pBranch)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "insert into tbtrainoptdata(train_id, train_name, timevalue, position, track, branch_id, hist) ";
        lvSql += "values(@train_id, @train_name, @timevalue, @position, @track, @branch_id, now())";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == Int64.MinValue)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Int64).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Int64).Value = ptrain_id;
        }

        cmd.Parameters.Add("@train_name", MySqlDbType.String).Value = ptrainname;

        if (ptimevalue == Int64.MinValue)
        {
            cmd.Parameters.Add("@timevalue", MySqlDbType.Int64).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@timevalue", MySqlDbType.Int64).Value = ptimevalue;
        }

        if (pposition == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@position", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@position", MySqlDbType.Double).Value = pposition;
        }

        if (pposition == Int16.MinValue)
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = pTrack;
        }

        if (pBranch == Int64.MinValue)
        {
            cmd.Parameters.Add("@branch_id", MySqlDbType.Int64).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@branch_id", MySqlDbType.Int64).Value = pBranch;
        }

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("PlanOptDataAccess => (" + lvSql + ") :: " + myex.ToString(), false);
            DebugLog.Logar("PlanOptDataAccess => " + myex.StackTrace, false);
            DebugLog.Logar("PlanOptDataAccess => Message = " + myex.Message, false);
            ConnectionManager.DebugMySqlQuery(cmd, "PlanOptDataAccess.Insert");
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("PlanOptDataAccess => (" + lvSql + ") :: " + nullex.ToString(), false);
            DebugLog.Logar("PlanOptDataAccess => " + nullex.StackTrace, false);
            DebugLog.Logar("PlanOptDataAccess => Message = " + nullex.Message, false);

            throw nullex;
        }

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Delete)]
    public static int DeleteAll()
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "delete from tbtrainoptdata";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("PlanOptDataAccess => (" + lvSql + ") :: " + myex.ToString(), false);
            DebugLog.Logar("PlanOptDataAccess => " + myex.StackTrace, false);
            DebugLog.Logar("PlanOptDataAccess => Message = " + myex.Message, false);

            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("PlanOptDataAccess => (" + lvSql + ") :: " + nullex.ToString(), false);
            DebugLog.Logar("PlanOptDataAccess => " + nullex.StackTrace, false);
            DebugLog.Logar("PlanOptDataAccess => Message = " + nullex.Message, false);

            throw nullex;
        }

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetAll()
    {
        string lvSql = "";
        DataSet ds = new DataSet();

        lvSql = "select * from tbtrainoptdata";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);
        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainoptdata");
        return ds;
    }
}
