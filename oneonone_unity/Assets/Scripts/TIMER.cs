using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Timers;
using System.Diagnostics;

public class TIMER
{
    public static Dictionary<string, Stopwatch> times = new Dictionary<string, Stopwatch>();
    public static HashSet<string> currentTimers = new HashSet<string>();

    //public static void addTo(string name, double value)
    //{

    //    if (times.ContainsKey(name))
    //    {
    //        times[name] = times[name] + value;
    //    }
    //    else
    //    {
    //        times[name] = new Stopwatch();
    //        times[name].Start();
    //    }
    //}
    public static void startTimer(string timerName)
    {
        if (times.ContainsKey(timerName))
        {
            if(!currentTimers.Contains(timerName))
            {
                currentTimers.Add(timerName);
                times[timerName].Start();
            }
        }
        else
        {
            currentTimers.Add(timerName);
            times[timerName] = new Stopwatch();
            times[timerName].Start();
        }
    }
    public static double stopTimer(string timerName)
    {
        if (currentTimers.Contains(timerName))
        {
            times[timerName].Stop();
            currentTimers.Remove(timerName);
        }
        return getTimer(timerName);
    }



    public static double getTimer(string name)
    {  
        return times[name].Elapsed.TotalMilliseconds;
    }
    public static void eraseTimer(string name)
    {
        if (times.ContainsKey(name))
        {
            times.Remove(name);
        }
        if(currentTimers.Contains(name))
        {
            currentTimers.Remove(name);
        }
    }
    public static void eraseAllTimers()
    {
        times.Clear();
        currentTimers.Clear();
    }

    public static void logTimers()
    {
        foreach (var item in times)
        {
            UnityEngine.Debug.Log("TIMER->  |" + item.Key + "|  -> " + getTimer(item.Key)/1000 + "s");
        }
    }
}
