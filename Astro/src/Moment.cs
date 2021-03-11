using System;
using System.Globalization;

namespace Qkmaxware.Astro {

/// <summary>
/// Specific moment in time, internally stored in UTC for astronomical computations
/// </summary>
public struct Moment {

    /// <summary>
    /// J2000 Epoch
    /// </summary>
    /// <returns>moment</returns>
    public static readonly Moment J2000 = new Moment(new DateTime(year: 2000, month: 01, day: 01, hour: 12, minute: 00, second: 00, millisecond: 00, new GregorianCalendar(), DateTimeKind.Utc));

    /// <summary>
    /// Current moment in time
    /// </summary>
    /// <returns>moment</returns>
    public static Moment Now => new Moment(DateTime.Now);

    /// <summary>
    /// UTC Year
    /// </summary>
    /// <value>year</value>
    public int Year {get; }
    /// <summary>
    /// UTC Month
    /// </summary>
    /// <value>month</value>
    public Month Month {get;}
    /// <summary>
    /// UTC Days
    /// </summary>
    /// <value>days</value>
    public int Day {get;}
    /// <summary>
    /// Day of the week
    /// </summary>
    /// <returns>monday through sunday</returns>
    public System.DayOfWeek DayOfWeek => ((DateTime)this).DayOfWeek;
    /// <summary>
    /// Fractional UTC Day
    /// </summary>
    /// <returns>fractional days in the moth</returns>
    public double FractionalDay => Day + (Hour / 24.0) + (Minute / 1440.0) + (Second + Millisecond / 1000.0) / 86400.0;
    /// <summary>
    /// UTC Hours
    /// </summary>
    /// <value>hours</value>
    public int Hour {get;}
    /// <summary>
    /// UTC Minutes
    /// </summary>
    /// <value>minutes</value>
    public int Minute {get;}
    /// <summary>
    /// UTC Seconds
    /// </summary>
    /// <value>seconds</value>
    public int Second {get;}
    /// <summary>
    /// UTC Milliseconds
    /// </summary>
    /// <value>milliseconds</value>
    public int Millisecond {get;}

    /// <summary>
    /// Create a moment from a date-time object
    /// </summary>
    /// <param name="dt">date-time</param>
    public Moment(DateTime dt) {
        dt = dt.ToUniversalTime(); // Convert TO UTC first?

        this.Year = dt.Year;
        this.Month = (Month)dt.Month;
        this.Day = dt.Day;
        this.Hour = dt.Hour;
        this.Minute = dt.Minute;
        this.Second = dt.Second;
        this.Millisecond = dt.Millisecond;
    }

    /// <summary>
    /// Create a moment from the given UTC date-time
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="day">day</param>
    /// <param name="hour">hour</param>
    /// <param name="minute">minute</param>
    /// <param name="second">second</param>
    /// <param name="millisecond">millisecond</param>
    public Moment(int year, Month month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0){
        this.Year = year;
        this.Month = month;
        this.Day = day;
        this.Hour = hour;
        this.Minute = minute;
        this.Second = second;
        this.Millisecond = millisecond;
    }

    /// <summary>
    /// Create a moment from the given UTC date-time
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="fractionalDays">fractional days</param>
    public Moment(int year, Month month, double fractionalDays){
        this.Year = year;
        this.Month = month;

        var ts = TimeSpan.FromDays(fractionalDays);
        this.Day = ts.Days;
        this.Hour = ts.Hours;
        this.Minute = ts.Minutes;
        this.Second = ts.Seconds;
        this.Millisecond = ts.Milliseconds;
    }

    /// <summary>
    /// Julian day at this moment in time
    /// </summary>
    /// <value>julian day</value>
    public double JulianDay {
        get {
            int Y = Year;
            int M = (int)Month;
            double B = 0; // Julian calendar default

            // if the date is Jan or Feb then it is considered to be in the 
            // 13th or 14th month of the preceding year.
            switch (M) {
                case 1:
                case 2:
                    Y -= 1;
                    M += 12;
                    break;

                default:
                    break;
            }
            
            // Convert to Gregorian Calendar
            double A = Math.Floor(Y / 100.0);
            B = 2 - A + Math.Floor(A / 4);

            return Math.Floor(365.25 * (Y + 4716)) + Math.Floor(30.6001 * (M + 1)) + FractionalDay + B - 1524.5;
        }
    }

    /// <summary>
    /// Create a moment from julian days
    /// </summary>
    /// <param name="days">julian days</param>
    /// <returns>moment</returns>
    public static Moment FromJulianDays(double days) {
        return DateTime.FromOADate(days - 2415018.5);
    }

    /// <summary>
    /// Create a moment from a julian year
    /// </summary>
    /// <param name="year">year</param>
    /// <returns>moment</returns>
    public static Moment FromJulianYear(int year) {
        return new DateTime(year: year, month: 1, day: 1, new JulianCalendar());
    }

    /// <summary>
    /// Greenwich mean sidereal time at this moment in time
    /// </summary>
    /// <value>gmst</value>
    public TimeSpan GreenwichMeanSiderealTime() {
        // Meeus, Chap 12 (pp 87-89) 
        var JD = this.JulianDay;
        var T = ((JD - 2451545.0) / 36525);
        var theta = 280.46061837 + 360.98564736629 * (JD - 2451545.0) + (0.000387933 * T * T) - (T * T * T / 38710000.0);
        
        // Reduce angle (0, 360)
        theta %= 360;
        if (theta < 0) {
            theta += 360;
        }

        // Convert to time
        return TimeSpan.FromHours(theta/15);
    }

    /// <summary>
    /// Moment to string
    /// </summary>
    /// <returns>date string</returns>
    public override string ToString(){
        DateTime dt = new DateTime(Year, (int)Month, Day, Hour, Minute, Second, DateTimeKind.Utc);
        return dt.ToString("u");
    }

    public override int GetHashCode() {
        return HashCode.Combine(
            this.Year,
            this.Month, 
            this.Day,
            this.Hour,
            this.Minute,
            this.Second,
            this.Millisecond
        );
    }

    public override bool Equals(object obj) {
        return obj switch {
            Moment m2 => this.JulianDay == m2.JulianDay,
            _ => base.Equals(obj)
        };
    }

    /// <summary>
    /// Implicitly convert from a date-time
    /// </summary>
    /// <param name="dtime">date-time</param>
    public static implicit operator Moment(DateTime dtime) {
        return new Moment(dtime);
    }

    /// <summary>
    /// Implicitly convert to a date-time
    /// </summary>
    /// <param name="moment">UTC moment</param>
    public static implicit operator DateTime(Moment moment) {
        return new DateTime(moment.Year, (int)moment.Month, moment.Day, moment.Hour, moment.Minute, moment.Second, moment.Millisecond, new GregorianCalendar(), DateTimeKind.Utc);
    }

    /// <summary>
    /// Amount of time between two moments
    /// </summary>
    /// <param name="start">starting time</param>
    /// <param name="end">ending time</param>
    /// <returns>time between start and end moments</returns>
    public static TimeSpan operator - (Moment start, Moment end) {
        return ((DateTime)start) - ((DateTime)end);
    }

    /// <summary>
    /// Check if two moments are equal
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if moments are equal</returns>
    public static bool operator == (Moment m1, Moment m2){
        return m1.Equals(m2);
    }

    /// <summary>
    /// Check if two moments are not equal
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if moments are not equal</returns>
    public static bool operator != (Moment m1, Moment m2){
        return !m1.Equals(m2);
    }

    /// <summary>
    /// Check if moment is later than another
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if first moment is later than the second</returns>
    public static bool operator > (Moment m1, Moment m2) {
        return m1.JulianDay > m2.JulianDay;
    }

    /// <summary>
    /// Check if moment is earlier than another
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if first moment is earlier than the second</returns>
    public static bool operator < (Moment m1, Moment m2) {
        return m1.JulianDay < m2.JulianDay;
    }

    /// <summary>
    /// Check if moment is later than or equivalent to another
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if first moment is later than or equal to the second</returns>
    public static bool operator >= (Moment m1, Moment m2) {
        return m1.JulianDay >= m2.JulianDay;
    }

    /// <summary>
    /// Check if moment is earlier than or equivalent to another
    /// </summary>
    /// <param name="m1">first moment</param>
    /// <param name="m2">second moment</param>
    /// <returns>true if first moment is earlier than or equal to the second</returns>
    public static bool operator <= (Moment m1, Moment m2) {
        return m1.JulianDay <= m2.JulianDay;
    }
}

}