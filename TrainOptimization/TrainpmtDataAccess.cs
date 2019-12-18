using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 20:59:39
/// <summary>

[DataObject(true)]
public class TrainpmtDataAccess
{
	public TrainpmtDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(string ppmt_id, string pprefix, DateTime pdate_hist, DateTime pprev_part, Int16 pKMOrigem, Int16 pKMDestino, Int16 psentflag, string pOS)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbtrainpmt(pmt_id, prefix, date_hist, prev_part, KMOrigem, KMDestino, sentflag, OS) ";
		lvSql += "values(@pmt_id, @prefix, @date_hist, @prev_part, @KMOrigem, @KMDestino, @sentflag, @OS)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = pprefix;

		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

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

		if(psentflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = psentflag;
		}

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = pOS;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(string ppmt_id, string pprefix, DateTime pdate_hist, DateTime pprev_part, Int16 pKMOrigem, Int16 pKMDestino, Int16 psentflag, string pOS)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrainpmt set date_hist=@date_hist, prev_part=@prev_part, KMOrigem=@KMOrigem, KMDestino=@KMDestino, sentflag=@sentflag, OS=@OS Where pmt_id=@pmt_id And prefix=@prefix";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = pprefix;
		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

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

		if(psentflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = psentflag;
		}

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = pOS;
		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(string pOrigpmt_id, string pOrigprefix, string ppmt_id, string pprefix)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbtrainpmt set pmt_id=@pmt_id, prefix=@prefix Where pmt_id=@origpmt_id And prefix=@origprefix";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;
		cmd.Parameters.Add("@origpmt_id", MySqlDbType.String).Value = pOrigpmt_id;
		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = pprefix;
		cmd.Parameters.Add("@origprefix", MySqlDbType.String).Value = pOrigprefix;
		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(string ppmt_id, string pprefix)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbtrainpmt Where pmt_id=@pmt_id And prefix=@prefix";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		cmd.Parameters.Add("@pmt_id", MySqlDbType.String).Value = ppmt_id;

		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = pprefix;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("TrainpmtDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbtrainpmt";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpmt");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(string ppmt_id = "", string pprefix = "", DateTime pdate_hist = default(DateTime), DateTime pprev_part = default(DateTime), Int16 pKMOrigem = -32768, Int16 pKMDestino = -32768, Int16 psentflag = -32768, string pOS = "", string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpmt.pmt_id, tbtrainpmt.prefix, tbtrainpmt.date_hist, tbtrainpmt.prev_part, tbtrainpmt.KMOrigem, tbtrainpmt.KMDestino, tbtrainpmt.sentflag, tbtrainpmt.OS From tbtrainpmt";

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(pprefix))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prefix like @prefix";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prefix like @prefix";
			}
		}

		if(pdate_hist > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.date_hist=@date_hist";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.date_hist=@date_hist";
			}
		}

		if(pprev_part > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prev_part=@prev_part";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prev_part=@prev_part";
			}
		}

		if(pKMOrigem > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMOrigem=@KMOrigem";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMOrigem=@KMOrigem";
			}
		}

		if(pKMDestino > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMDestino=@KMDestino";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMDestino=@KMDestino";
			}
		}

		if(psentflag > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.sentflag=@sentflag";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.sentflag=@sentflag";
			}
		}

		if(!string.IsNullOrEmpty(pOS))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.OS like @OS";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.OS like @OS";
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

		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = "%" + pprefix + "%";

		if(pdate_hist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@date_hist", MySqlDbType.DateTime).Value = pdate_hist.ToString("yyyy/MM/dd HH:mm:ss");
		}

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

		if(psentflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sentflag", MySqlDbType.Int16).Value = psentflag;
		}

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = "%" + pOS + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpmt");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(string ppmt_id = "", string pprefix = "", DateTime pdate_histInicio = default(DateTime), DateTime pdate_histFim = default(DateTime), DateTime pprev_partInicio = default(DateTime), DateTime pprev_partFim = default(DateTime), Int16 pKMOrigemInicio = -32768, Int16 pKMOrigemFim = -32768, Int16 pKMDestinoInicio = -32768, Int16 pKMDestinoFim = -32768, Int16 psentflagInicio = -32768, Int16 psentflagFim = -32768, string pOS = "", string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpmt.pmt_id, tbtrainpmt.prefix, tbtrainpmt.date_hist, tbtrainpmt.prev_part, tbtrainpmt.KMOrigem, tbtrainpmt.KMDestino, tbtrainpmt.sentflag, tbtrainpmt.OS From tbtrainpmt";

		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(pprefix))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prefix like @prefix";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prefix like @prefix";
			}
		}

		if(pdate_histInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.date_hist >= @date_histInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.date_hist >= @date_histInicio";
			}
		}

		if(pdate_histFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.date_hist <= @date_histFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.date_hist <= @date_histFim";
			}
		}

		if(pprev_partInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prev_part >= @prev_partInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prev_part >= @prev_partInicio";
			}
		}

		if(pprev_partFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prev_part <= @prev_partFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prev_part <= @prev_partFim";
			}
		}

		if(pKMOrigemInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMOrigem >= @KMOrigemInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMOrigem >= @KMOrigemInicio";
			}
		}

		if(pKMOrigemFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMOrigem <= @KMOrigemFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMOrigem <= @KMOrigemFim";
			}
		}

		if(pKMDestinoInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMDestino >= @KMDestinoInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMDestino >= @KMDestinoInicio";
			}
		}

		if(pKMDestinoFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.KMDestino <= @KMDestinoFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.KMDestino <= @KMDestinoFim";
			}
		}

		if(psentflagInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.sentflag >= @sentflagInicio";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.sentflag >= @sentflagInicio";
			}
		}

		if(psentflagFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.sentflag <= @sentflagFim";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.sentflag <= @sentflagFim";
			}
		}

		if(!string.IsNullOrEmpty(pOS))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.OS like @OS";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.OS like @OS";
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

		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = "%" + pprefix + "%";

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

		if(psentflagInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@sentflagInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sentflagInicio", MySqlDbType.Int16).Value = psentflagInicio;
		}

		if(psentflagFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@sentflagFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sentflagFim", MySqlDbType.Int16).Value = psentflagFim;
		}

		cmd.Parameters.Add("@OS", MySqlDbType.String).Value = "%" + pOS + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpmt");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(string ppmt_id = "", string pprefix = "", string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbtrainpmt.pmt_id, tbtrainpmt.prefix, tbtrainpmt.date_hist, tbtrainpmt.prev_part, tbtrainpmt.KMOrigem, tbtrainpmt.KMDestino, tbtrainpmt.sentflag, tbtrainpmt.OS From tbtrainpmt";
		if(!string.IsNullOrEmpty(ppmt_id))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.pmt_id like @pmt_id";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.pmt_id like @pmt_id";
			}
		}

		if(!string.IsNullOrEmpty(pprefix))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbtrainpmt.prefix like @prefix";
			}
			else
			{
				lvSqlWhere += " And tbtrainpmt.prefix like @prefix";
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

		cmd.Parameters.Add("@prefix", MySqlDbType.String).Value = "%" + pprefix + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbtrainpmt");
		return ds;
	}

}

