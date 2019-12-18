using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
/// <summary>
/// Criado por Eggo Pinheiro em 09/09/2014 15:42:21
/// <summary>

public class DebugLog
{
    private static object mLock = new object();
    private static bool mEnable = false;
    private static string mLogPath = ConfigurationManager.AppSettings["LOG_PATH"] + "Logs\\";
    private static bool mSaveDB = (ConfigurationManager.AppSettings["INPUT_MODE"].Equals("db") ? true : false);

    public DebugLog()
	{
	//
	//TODO: Add constructor logic here
	//
	}

    public static bool EnableDebug
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
        }
    }

    public static string LogPath
    {
        get
        {
            return mLogPath;
        }

        set
        {
            mLogPath = value;
        }
    }

    public static bool Logar(Exception pEx, bool pUseDateInfo = true, int pIndet = 0)
    {
        bool result = false;
        string fileName = "";
        string lvStrInfo = "";
        StreamWriter tw = null;
        FileStream lvFileStrem;

        if (mEnable || !pUseDateInfo)
        {
            try
            {
                Save(pEx);

                if (pIndet > 0)
                {
                    fileName = ConfigurationManager.AppSettings["LOG_FILE_NAME"] + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_Id_" + pIndet + "_Thread_" + Thread.CurrentThread.ManagedThreadId;
                }
                else
                {
                    fileName = ConfigurationManager.AppSettings["LOG_FILE_NAME"] + "_" + Thread.CurrentThread.ManagedThreadId + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_Thread_" + Thread.CurrentThread.ManagedThreadId;
                }
                lvFileStrem = new FileStream(mLogPath + fileName + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                tw = new StreamWriter(lvFileStrem);

                if (pUseDateInfo)
                {
                    lvStrInfo = DateTime.Now + " => Erro: " + pEx.Message + "\n Trace: " + pEx.StackTrace;
                }
                else
                {
                    lvStrInfo = "Erro: " + pEx.Message + "\n Trace:" + pEx.StackTrace;
                }

                tw.WriteLine(lvStrInfo);

                result = true;
                tw.Close();
            }
            catch (Exception ex)
            { }
        }
        return result;
    }

    public static bool Logar(string strInfo, bool pUseDateInfo = true, int pIndet = 0)
	{
		bool result = false;
		string fileName = "";
		StreamWriter tw = null;
        FileStream lvFileStrem;

        if (mEnable || !pUseDateInfo)
        {
            try
            {
                if (pIndet > 0)
                {
                    fileName = ConfigurationManager.AppSettings["LOG_FILE_NAME"] + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_Id_" + pIndet + "_Thread_" + Thread.CurrentThread.ManagedThreadId;
                }
                else
                {
                    fileName = ConfigurationManager.AppSettings["LOG_FILE_NAME"] + "_" + Thread.CurrentThread.ManagedThreadId + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_Thread_" + Thread.CurrentThread.ManagedThreadId;
                }
                lvFileStrem = new FileStream(mLogPath + fileName + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                tw = new StreamWriter(lvFileStrem);

                if (pUseDateInfo)
                {
                    strInfo = DateTime.Now + " => " + strInfo;
                }

                tw.WriteLine(strInfo);
                result = true;
                tw.Close();
            }
            catch (Exception ex)
            { }
        }
		return result;
	}

    public static bool Logar(string strInfo, string strFileName, bool pUseDateInfo = true)
	{
		bool result = false;
		//string fileName = "";
		StreamWriter tw = null;
        FileStream lvFileStrem;

        //fileName = strFileName + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
        lvFileStrem = new FileStream(mLogPath + strFileName + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write);
        tw = new StreamWriter(lvFileStrem);

        if (pUseDateInfo)
        {
            strInfo = DateTime.Now + " => " + strInfo;
        }

		tw.WriteLine(strInfo);
		result = true;
		tw.Close();

		return result;
	}

    public static bool LogInfo(string strInfo, string strFileName)
    {
        bool result = false;
        //string fileName = "";
        StreamWriter tw = null;
        FileStream lvFileStrem;

        //fileName = strFileName + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
        lvFileStrem = new FileStream(strFileName, FileMode.Append, FileAccess.Write, FileShare.Write);
        tw = new StreamWriter(lvFileStrem);

        tw.WriteLine(strInfo);
        result = true;
        tw.Close();

        return result;
    }

    public static bool Logar(string strInfo, string strFileName, string strExt)
	{
		bool result = false;
		string fileName = "";
		StreamWriter tw = null;
        FileStream lvFileStrem;

        fileName = strFileName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
        lvFileStrem = new FileStream(mLogPath + fileName + "." + strExt, FileMode.Append, FileAccess.Write, FileShare.Write);
        tw = new StreamWriter(lvFileStrem);

		tw.WriteLine(strInfo);
		result = true;
		tw.Close();

		return result;
	}

    private static void Save(Exception pEx)
    {
        Exception lvExi = null;

        if (mSaveDB)
        {
            lock(mLock)
            {
                DebugLodDataAccess.Insert(pEx);

                lvExi = pEx.InnerException;

                while(lvExi != null)
                {
                    DebugLodDataAccess.Insert(lvExi);
                    lvExi = lvExi.InnerException;
                }
            }
        }
    }

    public static void Save(string strInfo)
    {
        if (mSaveDB)
        {
            lock (mLock)
            {
                DebugLodDataAccess.Insert(strInfo);
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetMyMethodName()
    {
        var st = new StackTrace(new StackFrame(1));
        return st.GetFrame(0).GetMethod().Name;
    }
}

