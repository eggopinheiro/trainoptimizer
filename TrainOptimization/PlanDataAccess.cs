using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 20:59:41
/// <summary>

[DataObject(true)]
public class PlanDataAccess
{
	public PlanDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(double pplan_id, string ptrain_name, int porigem, int pdestino, DateTime pdeparture_time, DateTime phist, string ppmt_id, string poid)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbplan(plan_id, train_name, origem, destino, departure_time, hist, pmt_id, oid) ";
		lvSql += "values(@plan_id, @train_name, @origem, @destino, @departure_time, @hist, @pmt_id, @oid)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@train_name", MySqlDbType.String).Value = ptrain_name;

		if(porigem == Int32.MinValue)
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = porigem;
		}

		if(pdestino == Int32.MinValue)
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = pdestino;
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = poid;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(double pplan_id, string ptrain_name, int porigem, int pdestino, DateTime pdeparture_time, DateTime phist, string ppmt_id, string poid)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbplan set train_name=@train_name, origem=@origem, destino=@destino, departure_time=@departure_time, hist=@hist, pmt_id=@pmt_id, oid=@oid Where plan_id=@plan_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@train_name", MySqlDbType.String).Value = ptrain_name;
		if(porigem == Int32.MinValue)
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = porigem;
		}

		if(pdestino == Int32.MinValue)
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = pdestino;
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = poid;
		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(double pOrigplan_id, double pplan_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbplan set plan_id=@plan_id Where plan_id=@origplan_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		if(pOrigplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@origplan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origplan_id", MySqlDbType.Double).Value = pOrigplan_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(double pplan_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbplan Where plan_id=@plan_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("PlanDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbplan";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbplan");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(double pplan_id = -999999999999, string ptrain_name = "", int porigem = -2147483648, int pdestino = -2147483648, DateTime pdeparture_time = default(DateTime), DateTime phist = default(DateTime), string ppmt_id = "", string poid = "", Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid, tbtrainpmt.pmt_id tbtrainpmt_pmt_id, tbtrainpmt.prefix tbtrainpmt_prefix, tbtrainpmt.date_hist tbtrainpmt_date_hist, tbtrainpmt.prev_part tbtrainpmt_prev_part, tbtrainpmt.KMOrigem tbtrainpmt_KMOrigem, tbtrainpmt.KMDestino tbtrainpmt_KMDestino, tbtrainpmt.sentflag tbtrainpmt_sentflag, tbtrainpmt.OS tbtrainpmt_OS From tbplan Left Join tbtrainpmt on tbplan.pmt_id=tbtrainpmt.pmt_id";
		}
		else
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid From tbplan";
		}

		if(pplan_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.plan_id=@plan_id";
			}
			else
			{
				lvSqlWhere += " And tbplan.plan_id=@plan_id";
			}
		}

		if(!string.IsNullOrEmpty(ptrain_name))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.train_name like @train_name";
			}
			else
			{
				lvSqlWhere += " And tbplan.train_name like @train_name";
			}
		}

		if(porigem > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.origem=@origem";
			}
			else
			{
				lvSqlWhere += " And tbplan.origem=@origem";
			}
		}

		if(pdestino > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.destino=@destino";
			}
			else
			{
				lvSqlWhere += " And tbplan.destino=@destino";
			}
		}

		if(pdeparture_time > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.departure_time=@departure_time";
			}
			else
			{
				lvSqlWhere += " And tbplan.departure_time=@departure_time";
			}
		}

		if(phist > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.hist=@hist";
			}
			else
			{
				lvSqlWhere += " And tbplan.hist=@hist";
			}
		}

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbplan.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(poid))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.oid like @oid";
			}
			else
			{
				lvSqlWhere += " And tbplan.oid like @oid";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.Parameters.Add("@train_name", MySqlDbType.String).Value = "%" + ptrain_name + "%";

		if(porigem == Int32.MinValue)
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origem", MySqlDbType.Int32).Value = porigem;
		}

		if(pdestino == Int32.MinValue)
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@destino", MySqlDbType.Int32).Value = pdestino;
		}

		if(pdeparture_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_time", MySqlDbType.DateTime).Value = pdeparture_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = "%" + poid + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbplan");
		return ds;
	}

    public static DataSet GetCurrentPlans(DateTime pdeparture_timeInicio = default(DateTime), DateTime pdeparture_timeFim = default(DateTime))
    {
        string lvSql = "";
        DataSet ds = new DataSet();

        lvSql = "select tbplan.plan_id, tbtrain.train_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.origin_track From tbplan left join tbtrain on tbplan.plan_id=tbtrain.plan_id where tbtrain.status='Planejado' and (tbplan.departure_time between @begindate and @enddate) And tbplan.branch_id=@branch_id order by tbplan.departure_time asc";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (pdeparture_timeInicio == DateTime.MinValue)
        {
            cmd.Parameters.Add("@begindate", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@begindate", MySqlDbType.DateTime).Value = pdeparture_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (pdeparture_timeFim == DateTime.MinValue)
        {
            cmd.Parameters.Add("@enddate", MySqlDbType.DateTime).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@enddate", MySqlDbType.DateTime).Value = pdeparture_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
        }
        cmd.Parameters.Add("@branch_id", MySqlDbType.String).Value = SegmentDataAccess.Branch;

        cmd.CommandType = CommandType.Text;

        conn.Close();

        ConnectionManager.DebugMySqlQuery(cmd, "GetCurrentPlans.lvSql");

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbplan");
        return ds;
    }

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(double pplan_idInicio = -999999999999, double pplan_idFim = -999999999999, string ptrain_name = "", int porigemInicio = -2147483648, int porigemFim = -2147483648, int pdestinoInicio = -2147483648, int pdestinoFim = -2147483648, DateTime pdeparture_timeInicio = default(DateTime), DateTime pdeparture_timeFim = default(DateTime), DateTime phistInicio = default(DateTime), DateTime phistFim = default(DateTime), string ppmt_id = "", string poid = "", Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid, tbtrainpmt.pmt_id tbtrainpmt_pmt_id, tbtrainpmt.prefix tbtrainpmt_prefix, tbtrainpmt.date_hist tbtrainpmt_date_hist, tbtrainpmt.prev_part tbtrainpmt_prev_part, tbtrainpmt.KMOrigem tbtrainpmt_KMOrigem, tbtrainpmt.KMDestino tbtrainpmt_KMDestino, tbtrainpmt.sentflag tbtrainpmt_sentflag, tbtrainpmt.OS tbtrainpmt_OS From tbplan Left Join tbtrainpmt on tbplan.pmt_id=tbtrainpmt.pmt_id";
		}
		else
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid From tbplan";
		}

		if(pplan_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.plan_id >= @plan_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbplan.plan_id >= @plan_idInicio";
			}
		}

		if(pplan_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.plan_id <= @plan_idFim";
			}
			else
			{
				lvSqlWhere += " And tbplan.plan_id <= @plan_idFim";
			}
		}

		if(!string.IsNullOrEmpty(ptrain_name))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.train_name like @train_name";
			}
			else
			{
				lvSqlWhere += " And tbplan.train_name like @train_name";
			}
		}

		if(porigemInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.origem >= @origemInicio";
			}
			else
			{
				lvSqlWhere += " And tbplan.origem >= @origemInicio";
			}
		}

		if(porigemFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.origem <= @origemFim";
			}
			else
			{
				lvSqlWhere += " And tbplan.origem <= @origemFim";
			}
		}

		if(pdestinoInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.destino >= @destinoInicio";
			}
			else
			{
				lvSqlWhere += " And tbplan.destino >= @destinoInicio";
			}
		}

		if(pdestinoFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.destino <= @destinoFim";
			}
			else
			{
				lvSqlWhere += " And tbplan.destino <= @destinoFim";
			}
		}

		if(pdeparture_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.departure_time >= @departure_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbplan.departure_time >= @departure_timeInicio";
			}
		}

		if(pdeparture_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.departure_time <= @departure_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbplan.departure_time <= @departure_timeFim";
			}
		}

		if(phistInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.hist >= @histInicio";
			}
			else
			{
				lvSqlWhere += " And tbplan.hist >= @histInicio";
			}
		}

		if(phistFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.hist <= @histFim";
			}
			else
			{
				lvSqlWhere += " And tbplan.hist <= @histFim";
			}
		}

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbplan.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(poid))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.oid like @oid";
			}
			else
			{
				lvSqlWhere += " And tbplan.oid like @oid";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_idInicio", MySqlDbType.Double).Value = pplan_idInicio;
		}

		if(pplan_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_idFim", MySqlDbType.Double).Value = pplan_idFim;
		}

		cmd.Parameters.Add("@train_name", MySqlDbType.String).Value = "%" + ptrain_name + "%";

		if(porigemInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@origemInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origemInicio", MySqlDbType.Int32).Value = porigemInicio;
		}

		if(porigemFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@origemFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origemFim", MySqlDbType.Int32).Value = porigemFim;
		}

		if(pdestinoInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@destinoInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@destinoInicio", MySqlDbType.Int32).Value = pdestinoInicio;
		}

		if(pdestinoFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@destinoFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@destinoFim", MySqlDbType.Int32).Value = pdestinoFim;
		}

		if(pdeparture_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_timeInicio", MySqlDbType.DateTime).Value = pdeparture_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdeparture_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@departure_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@departure_timeFim", MySqlDbType.DateTime).Value = pdeparture_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phistInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = phistInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phistFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = phistFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		cmd.Parameters.Add("@oid", MySqlDbType.String).Value = "%" + poid + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbplan");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(double pplan_id = -999999999999, Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid, tbtrainpmt.pmt_id tbtrainpmt_pmt_id, tbtrainpmt.prefix tbtrainpmt_prefix, tbtrainpmt.date_hist tbtrainpmt_date_hist, tbtrainpmt.prev_part tbtrainpmt_prev_part, tbtrainpmt.KMOrigem tbtrainpmt_KMOrigem, tbtrainpmt.KMDestino tbtrainpmt_KMDestino, tbtrainpmt.sentflag tbtrainpmt_sentflag, tbtrainpmt.OS tbtrainpmt_OS From tbplan Left Join tbtrainpmt on tbplan.pmt_id=tbtrainpmt.pmt_id";
		}
		else
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid From tbplan";
		}

		if(pplan_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbplan.plan_id=@plan_id";
			}
			else
			{
				lvSqlWhere += " And tbplan.plan_id=@plan_id";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pplan_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_id", MySqlDbType.Double).Value = pplan_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbplan");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByTrainpmt(string ppmt_id = "", Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid, tbtrainpmt.pmt_id tbtrainpmt_pmt_id, tbtrainpmt.prefix tbtrainpmt_prefix, tbtrainpmt.date_hist tbtrainpmt_date_hist, tbtrainpmt.prev_part tbtrainpmt_prev_part, tbtrainpmt.KMOrigem tbtrainpmt_KMOrigem, tbtrainpmt.KMDestino tbtrainpmt_KMDestino, tbtrainpmt.sentflag tbtrainpmt_sentflag, tbtrainpmt.OS tbtrainpmt_OS From tbplan Left Join tbtrainpmt on tbplan.pmt_id=tbtrainpmt.pmt_id";
		}
		else
		{
			lvSql = "select tbplan.plan_id, tbplan.train_name, tbplan.origem, tbplan.destino, tbplan.departure_time, tbplan.hist, tbplan.pmt_id, tbplan.oid From tbplan";
		}

		lvSqlWhere = "tbplan.pmt_id like @pmt_id";

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbplan");
		return ds;
	}

}

