using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MySql.Data.MySqlClient;

class DebugLodDataAccess
{
    [DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(string pStrInfo)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "insert into tbtrainoptlog(message, source, stacktrace, targetsite) ";
        lvSql += "values(@message, @source, @stacktrace, @targetsite)";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@message", MySqlDbType.String).Value = pStrInfo;
        cmd.Parameters.Add("@source", MySqlDbType.String).Value = "";
        cmd.Parameters.Add("@stacktrace", MySqlDbType.String).Value = "";
        cmd.Parameters.Add("@targetsite", MySqlDbType.String).Value = "";

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("DebugLodDataAccess => (" + lvSql + ") :: " + myex.ToString());
            //throw myex;
        }
        catch (NullReferenceException nullex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("DebugLodDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            //throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(Exception pEx)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "insert into tbtrainoptlog(message, source, stacktrace, targetsite) ";
        lvSql += "values(@message, @source, @stacktrace, @targetsite)";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@message", MySqlDbType.String).Value = pEx.Message;
        cmd.Parameters.Add("@source", MySqlDbType.String).Value = pEx.Source;
        cmd.Parameters.Add("@stacktrace", MySqlDbType.String).Value = pEx.StackTrace;
        cmd.Parameters.Add("@targetsite", MySqlDbType.String).Value = pEx.TargetSite.ToString();

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("DebugLodDataAccess => (" + lvSql + ") :: " + myex.ToString());
            //throw myex;
        }
        catch (NullReferenceException nullex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("DebugLodDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            //throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

}
