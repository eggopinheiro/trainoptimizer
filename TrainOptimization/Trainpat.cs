using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 21:00:05
/// <summary>

public class Trainpat
{
    protected int coordinate;
    protected Int16 km;
    protected Int16 duration;
	protected string lvActivity;
	protected DateTime lvdate_hist;

	public Trainpat()
	{
	}

    public Trainpat(int coordinate, Int16 km, Int16 duration, Int16 KMParada, string Activity, DateTime date_hist)
	{
		this.coordinate = coordinate;
		this.km = km;
		this.duration = duration;
		this.lvActivity = Activity;
		this.lvdate_hist = date_hist;
	}

	public string Activity
	{
		get
		{
			return this.lvActivity;
		}
		set
		{
			this.lvActivity = value;
		}
	}

	public DateTime Date_hist
	{
		get
		{
			return this.lvdate_hist;
		}
		set
		{
			this.lvdate_hist = value;
		}
	}

    public int Coordinate
    {
        get
        {
            return coordinate;
        }

        set
        {
            coordinate = value;
        }
    }

    public short KM
    {
        get
        {
            return km;
        }

        set
        {
            km = value;
        }
    }

    public short Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }
}

