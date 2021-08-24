using System;
using System.Globalization;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro {

public class Timestamp {
    public Timestamp(int year, Month month, int day, int hour, int minute, int second, int millisecond) {
        this.Year = year;
        this.Month = month;
        this.Day = day;
        this.Minute = minute;
        this.Second = second;
        this.Millisecond = millisecond;
    }

    /// <summary>
    /// Year
    /// </summary>
    /// <value>year</value>
    public int Year {get; }
    /// <summary>
    /// Month
    /// </summary>
    /// <value>month</value>
    public Month Month {get;}
    /// <summary>
    /// Day
    /// </summary>
    /// <value>days</value>
    public int Day {get;}
    /// <summary>
    /// Hours
    /// </summary>
    /// <value>hours</value>
    public int Hour {get;}
    /// <summary>
    /// Minutes
    /// </summary>
    /// <value>minutes</value>
    public int Minute {get;}
    /// <summary>
    /// Seconds
    /// </summary>
    /// <value>seconds</value>
    public int Second {get;}
    /// <summary>
    /// Milliseconds
    /// </summary>
    /// <value>milliseconds</value>
    public int Millisecond {get;}

    /// <summary>
    /// Fractional Day within the month
    /// </summary>
    /// <returns>fractional days in the month</returns>
    public double FractionalDay => Day + (Hour / 24.0) + (Minute / 1440.0) + (Second + Millisecond / 1000.0) / 86400.0;
    /// <summary>
    /// Fractional hour within the day
    /// </summary>
    /// <returns>fractional hour during the day</returns>
    public double FractionalHour => (Hour / 24.0) + (Minute / 1440.0) + (Second + Millisecond / 1000.0) / 86400.0;
}

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
    /// Timestamp in UTC 
    /// </summary>
    /// <value>utc date-time timestamp</value>
    public Timestamp UtcTimestamp {get; private set;}

    /// <summary>
    /// Timestamp in Local Time 
    /// </summary>
    /// <value>local date-time timestamp</value>
    public Timestamp LocalTimestamp {get; private set;}

    /// <summary>
    /// Create a moment from a date-time object
    /// </summary>
    /// <param name="dt">date-time</param>
    public Moment(DateTime dt) {
        var utc = dt.ToUniversalTime();
        this.UtcTimestamp = new Timestamp(
            year:           utc.Year,
            month:          (Month)utc.Month,
            day:            utc.Day,
            hour:           utc.Hour,
            minute:         utc.Minute,
            second:         utc.Second,
            millisecond:    utc.Millisecond
        );

        var local = dt.ToLocalTime();
        this.LocalTimestamp = new Timestamp(
            year:           local.Year,
            month:          (Month)local.Month,
            day:            local.Day,
            hour:           local.Hour,
            minute:         local.Minute,
            second:         local.Second,
            millisecond:    local.Millisecond
        );
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
        DateTime utc = new DateTime(year, (int)month, day, hour, minute, second, millisecond, new GregorianCalendar(), DateTimeKind.Utc);
        this.UtcTimestamp = new Timestamp(
            year:           utc.Year,
            month:          (Month)utc.Month,
            day:            utc.Day,
            hour:           utc.Hour,
            minute:         utc.Minute,
            second:         utc.Second,
            millisecond:    utc.Millisecond
        );

        var local = utc.ToLocalTime();
        this.LocalTimestamp = new Timestamp(
            year:           local.Year,
            month:          (Month)local.Month,
            day:            local.Day,
            hour:           local.Hour,
            minute:         local.Minute,
            second:         local.Second,
            millisecond:    local.Millisecond
        );
    }

    /// <summary>
    /// Create a moment from the given UTC date-time
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">month</param>
    /// <param name="fractionalDays">fractional days</param>
    public Moment(int year, Month month, double fractionalDays){
        var ts = TimeSpan.FromDays(fractionalDays);
        var day = ts.Days;
        var hour = ts.Hours;
        var minute = ts.Minutes;
        var second = ts.Seconds;
        var millisecond = ts.Milliseconds;

        DateTime utc = new DateTime(year, (int)month, day, hour, minute, second, millisecond, new GregorianCalendar(), DateTimeKind.Utc);
        this.UtcTimestamp = new Timestamp(
            year:           utc.Year,
            month:          (Month)utc.Month,
            day:            utc.Day,
            hour:           utc.Hour,
            minute:         utc.Minute,
            second:         utc.Second,
            millisecond:    utc.Millisecond
        );

        var local = utc.ToLocalTime();
        this.LocalTimestamp = new Timestamp(
            year:           local.Year,
            month:          (Month)local.Month,
            day:            local.Day,
            hour:           local.Hour,
            minute:         local.Minute,
            second:         local.Second,
            millisecond:    local.Millisecond
        );
    }

    /// <summary>
    /// Julian day at this moment in time
    /// </summary>
    /// <value>julian day</value>
    public double JulianDay {
        get {
            int Y = UtcTimestamp.Year;
            int M = (int)UtcTimestamp.Month;
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

            return Math.Floor(365.25 * (Y + 4716)) + Math.Floor(30.6001 * (M + 1)) + UtcTimestamp.FractionalDay + B - 1524.5;
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

    /*
    public Moment? Sunrise(Angle latitude, Angle longitude) {
        var lat = latitude.TotalDegrees().Clamp(-89, 89);
        var long = longitude.TotalDegrees() - 180;
        var JD = this.JulianDay;

    }

    public Moment? Sunset(Angle latitude, Angle longitude) {

    }*/

    /// <summary>
    /// Moment to string
    /// </summary>
    /// <returns>date string</returns>
    public override string ToString(){
        DateTime dt = new DateTime(UtcTimestamp.Year, (int)UtcTimestamp.Month, UtcTimestamp.Day, UtcTimestamp.Hour, UtcTimestamp.Minute, UtcTimestamp.Second, DateTimeKind.Utc);
        return dt.ToString("u");
    }

    public override int GetHashCode() {
        return this.JulianDay.GetHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj == null)
            return false;
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
        return new DateTime(moment.UtcTimestamp.Year, (int)moment.UtcTimestamp.Month, moment.UtcTimestamp.Day, moment.UtcTimestamp.Hour, moment.UtcTimestamp.Minute, moment.UtcTimestamp.Second, moment.UtcTimestamp.Millisecond, new GregorianCalendar(), DateTimeKind.Utc);
    }

    /// <summary>
    /// Amount of time between two moments
    /// </summary>
    /// <param name="start">starting time</param>
    /// <param name="end">ending time</param>
    /// <returns>time between start and end moments</returns>
    public static Duration operator - (Moment start, Moment end) {
        Scientific hrs = (end.JulianDay - start.JulianDay) * 24;
        return Duration.Hours(hrs.Abs());
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