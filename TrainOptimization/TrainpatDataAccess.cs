using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 21:00:03
/// <summary>

[DataObject(true)]
public class TrainpatDataAccess
{
	public TrainpatDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(string ppmt_id, DateTime pprev_part, Int16 pKMOrigem, Int16 pKMDestino, Int16 pKMParada, string pActivity, int pEspera, DateTime pdate_hist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbtrainpat(pmt_id, prev_part, KMOrigem, KMDestino, KMParada, Activity, Espera, date_hist) ";
		lvSql += "values(@pmt_id, @prev_part, @KMOrigem, @KMDestino, @KMParada, @Activity, @Espera, @date_hist)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		if(pprev_part == DateTime.MinValue)
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = pprev_part.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pKMOrigem == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = pKMOrigem;
		}

		if(pKMDestino == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = pKMDestino;
		}

		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		cmd.Parameters.Add("@Activity", MySqlDbType.String).Value = pActivity;

		if(pEspera == Int32.MinValue)
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = pEspera;
		}

		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(string ppmt_id, DateTime pprev_part, Int16 pKMOrigem, Int16 pKMDestino, Int16 pKMParada, string pActivity, int pEspera, DateTime pdate_hist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrainpat set prev_part=@prev_part, KMOrigem=@KMOrigem, KMDestino=@KMDestino, Activity=@Activity, Espera=@Espera, date_hist=@date_hist Where pmt_id=@pmt_id And KMParada=@KMParada";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		if(pprev_part == DateTime.MinValue)
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = pprev_part.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pKMOrigem == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = pKMOrigem;
		}

		if(pKMDestino == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = pKMDestino;
		}

		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		cmd.Parameters.Add("@Activity", MySqlDbType.String).Value = pActivity;
		if(pEspera == Int32.MinValue)
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = pEspera;
		}

		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(string pOrigpmt_id, Int16 pOrigKMParada, string ppmt_id, Int16 pKMParada)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrainpat set pmt_id=@pmt_id, KMParada=@KMParada Where pmt_id=@origpmt_id And KMParada=@origKMParada";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		cmd.Parameters.Add("@origpmt_id", MySqlDbType.String).Value = pOrigpmt_id;
		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		if(pOrigKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@origKMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origKMParada", MySqlDbType.Int16).Value = pOrigKMParada;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(string ppmt_id, Int16 pKMParada)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbtrainpat Where pmt_id=@pmt_id And KMParada=@KMParada";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpatDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbtrainpat";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpat");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(string ppmt_id = "", DateTime pprev_part = default(DateTime), Int16 pKMOrigem = -32768, Int16 pKMDestino = -32768, Int16 pKMParada = -32768, string pActivity = "", int pEspera = -2147483648, DateTime pdate_hist = default(DateTime), string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpat.pmt_id, tbtrainpat.prev_part, tbtrainpat.KMOrigem, tbtrainpat.KMDestino, tbtrainpat.KMParada, tbtrainpat.Activity, tbtrainpat.Espera, tbtrainpat.date_hist From tbtrainpat";

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.pmt_id like @pmt_id";
			}
		}

		if(pprev_part > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.prev_part=@prev_part";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.prev_part=@prev_part";
			}
		}

		if(pKMOrigem > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMOrigem=@KMOrigem";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMOrigem=@KMOrigem";
			}
		}

		if(pKMDestino > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMDestino=@KMDestino";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMDestino=@KMDestino";
			}
		}

		if(pKMParada > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMParada=@KMParada";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMParada=@KMParada";
			}
		}

		if(!string.IsNullOrEmpty(pActivity))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.Activity like @Activity";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.Activity like @Activity";
			}
		}

		if(pEspera > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.Espera=@Espera";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.Espera=@Espera";
			}
		}

		if(pdate_hist > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.date_hist=@date_hist";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.date_hist=@date_hist";
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

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		if(pprev_part == DateTime.MinValue)
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@prev_part", MySqlDbType.DateTime).Value = pprev_part.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pKMOrigem == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMOrigem", MySqlDbType.Int16).Value = pKMOrigem;
		}

		if(pKMDestino == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMDestino", MySqlDbType.Int16).Value = pKMDestino;
		}

		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		cmd.Parameters.Add("@Activity", MySqlDbType.String).Value = "%" + pActivity + "%";

		if(pEspera == Int32.MinValue)
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@Espera", MySqlDbType.Int32).Value = pEspera;
		}

		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpat");
		return ds;
	}

    public static DataSet GetPATTrain(Int64 pTrainId)
    {
        string lvSql = "";
        DataSet ds = new DataSet();

        lvSql = "select train_id, branch_id, coordinate, km, duration, definition, hist From tbtrainactivity where train_id=@train_id order by km, coordinate";

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (pTrainId == Int64.MinValue)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Int64).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Int64).Value = pTrainId;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbtrainactivity");
        return ds;
    }

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(string ppmt_id = "", DateTime pprev_partInicio = default(DateTime), DateTime pprev_partFim = default(DateTime), Int16 pKMOrigemInicio = -32768, Int16 pKMOrigemFim = -32768, Int16 pKMDestinoInicio = -32768, Int16 pKMDestinoFim = -32768, Int16 pKMParadaInicio = -32768, Int16 pKMParadaFim = -32768, string pActivity = "", int pEsperaInicio = -2147483648, int pEsperaFim = -2147483648, DateTime pdate_histInicio = default(DateTime), DateTime pdate_histFim = default(DateTime), string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpat.pmt_id, tbtrainpat.prev_part, tbtrainpat.KMOrigem, tbtrainpat.KMDestino, tbtrainpat.KMParada, tbtrainpat.Activity, tbtrainpat.Espera, tbtrainpat.date_hist From tbtrainpat";

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.pmt_id = @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.pmt_id = @pmt_id";
			}
		}

		if(pprev_partInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.prev_part >= @prev_partInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.prev_part >= @prev_partInicio";
			}
		}

		if(pprev_partFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.prev_part <= @prev_partFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.prev_part <= @prev_partFim";
			}
		}

		if(pKMOrigemInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMOrigem >= @KMOrigemInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMOrigem >= @KMOrigemInicio";
			}
		}

		if(pKMOrigemFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMOrigem <= @KMOrigemFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMOrigem <= @KMOrigemFim";
			}
		}

		if(pKMDestinoInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMDestino >= @KMDestinoInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMDestino >= @KMDestinoInicio";
			}
		}

		if(pKMDestinoFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMDestino <= @KMDestinoFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMDestino <= @KMDestinoFim";
			}
		}

		if(pKMParadaInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMParada >= @KMParadaInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMParada >= @KMParadaInicio";
			}
		}

		if(pKMParadaFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMParada <= @KMParadaFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMParada <= @KMParadaFim";
			}
		}

		if(!string.IsNullOrEmpty(pActivity))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.Activity like @Activity";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.Activity like @Activity";
			}
		}

		if(pEsperaInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.Espera >= @EsperaInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.Espera >= @EsperaInicio";
			}
		}

		if(pEsperaFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.Espera <= @EsperaFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.Espera <= @EsperaFim";
			}
		}

		if(pdate_histInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.date_hist >= @date_histInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.date_hist >= @date_histInicio";
			}
		}

		if(pdate_histFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.date_hist <= @date_histFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.date_hist <= @date_histFim";
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

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "'" + ppmt_id + "'";

		if(pprev_partInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@prev_partInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@prev_partInicio", MySqlDbType.DateTime).Value = pprev_partInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pprev_partFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@prev_partFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@prev_partFim", MySqlDbType.DateTime).Value = pprev_partFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pKMOrigemInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMOrigemInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMOrigemInicio", MySqlDbType.Int16).Value = pKMOrigemInicio;
		}

		if(pKMOrigemFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMOrigemFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMOrigemFim", MySqlDbType.Int16).Value = pKMOrigemFim;
		}

		if(pKMDestinoInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMDestinoInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMDestinoInicio", MySqlDbType.Int16).Value = pKMDestinoInicio;
		}

		if(pKMDestinoFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMDestinoFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMDestinoFim", MySqlDbType.Int16).Value = pKMDestinoFim;
		}

		if(pKMParadaInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParadaInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParadaInicio", MySqlDbType.Int16).Value = pKMParadaInicio;
		}

		if(pKMParadaFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParadaFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParadaFim", MySqlDbType.Int16).Value = pKMParadaFim;
		}

		cmd.Parameters.Add("@Activity", MySqlDbType.String).Value = "%" + pActivity + "%";

		if(pEsperaInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@EsperaInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@EsperaInicio", MySqlDbType.Int32).Value = pEsperaInicio;
		}

		if(pEsperaFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@EsperaFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@EsperaFim", MySqlDbType.Int32).Value = pEsperaFim;
		}

		if(pdate_histInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_histInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_histInicio", MySqlDbType.DateTime).Value = pdate_histInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pdate_histFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_histFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_histFim", MySqlDbType.DateTime).Value = pdate_histFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpat");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(string ppmt_id = "", Int16 pKMParada = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpat.pmt_id, tbtrainpat.prev_part, tbtrainpat.KMOrigem, tbtrainpat.KMDestino, tbtrainpat.KMParada, tbtrainpat.Activity, tbtrainpat.Espera, tbtrainpat.date_hist From tbtrainpat";
		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.pmt_id like @pmt_id";
			}
		}

		if(pKMParada > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpat.KMParada=@KMParada";
			}
			else
			{
				lvSqlWhere += " And tbtrainpat.KMParada=@KMParada";
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

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = "%" + ppmt_id + "%";

		if(pKMParada == Int16.MinValue)
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@KMParada", MySqlDbType.Int16).Value = pKMParada;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpat");
		return ds;
	}

}

