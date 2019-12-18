using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MySql.Data.MySqlClient;

class ElapsedTimeDataAccess
{
    [DataObjectMethod(DataObjectMethodType.Insert)]
    public static int Insert(int pId, string pObjFunctionType, int pPopulationCount, double pInitialFitness)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "insert into tbtrainoptelapsed(id, start_time, objective_function, population_count, initial_fitness) ";
        lvSql += "values(@id, now(), @objfunctype, @population_count, @initial_fitness)";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = pId;
        cmd.Parameters.Add("@objfunctype", MySqlDbType.String).Value = pObjFunctionType;
        cmd.Parameters.Add("@population_count", MySqlDbType.Int32).Value = pPopulationCount;
        cmd.Parameters.Add("@initial_fitness", MySqlDbType.Double).Value = pInitialFitness;

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

    [DataObjectMethod(DataObjectMethodType.Update)]
    public static int Update(int pId, DateTime pEndTime, int pObjFunctCalled, double pObjFunctValue, int pGeneration, int pPopulationCount, int pLocalSearchCalledCount)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "update tbtrainoptelapsed set end_time=@end_time, objective_function_called=@objective_function_called, fitness_value=@fitness_value, generation=@generation, ls_called_count=@ls_called_count, population_count=@population_count Where id=@id";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = pId;

        if (pEndTime == DateTime.MinValue)
        {
            cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = pEndTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        cmd.Parameters.Add("@objective_function_called", MySqlDbType.Int32).Value = pObjFunctCalled;

        if (pObjFunctValue == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@fitness_value", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@fitness_value", MySqlDbType.Double).Value = pObjFunctValue;
        }

        cmd.Parameters.Add("@generation", MySqlDbType.Int32).Value = pGeneration;
        cmd.Parameters.Add("@population_count", MySqlDbType.Int32).Value = pPopulationCount;
        cmd.Parameters.Add("@ls_called_count", MySqlDbType.Int32).Value = pLocalSearchCalledCount;

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            ConnectionManager.DebugMySqlQuery(cmd, "lvSql");
            DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }

    [DataObjectMethod(DataObjectMethodType.Delete)]
    public static int Delete(int pId)
    {
        string lvSql = "";
        int lvRowAffects = 0;

        lvSql = "delete from tbtrainoptelapsed Where id=@id";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = pId;

        cmd.CommandType = CommandType.Text;

        try
        {
            lvRowAffects = cmd.ExecuteNonQuery();
        }
        catch (MySqlException myex)
        {
            DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
            throw myex;
        }
        catch (NullReferenceException nullex)
        {
            DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
            throw nullex;
        }

        conn.Close();

        return lvRowAffects;
    }
}
