using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Alarm
{
    #region Pooling
    private static List<Alarm> _alarmPool = new List<Alarm>();
    private static List<Alarm> _alarmsInUse = new List<Alarm>();
    /// <summary>
    /// A list of all alarms currently in use.
    /// </summary>
    public static List<Alarm> AlarmsInUse
    {
        get => _alarmsInUse;
    }
    public static int maxAlarmsAllowed = 10;
    #endregion

    #region Variables
    [SerializeField] private string _name;
    [SerializeField] private int _id;
    [SerializeField] private float _timeRemaining;
    [SerializeField] private float _timeMax;
    [SerializeField] private float _timeScale = 1f;
    [SerializeField] private bool _looping = false;
    private bool _paused;
    private bool _stopped;
    [SerializeField] private bool _autoRelease = true;

    public enum Type { scaled, unscaled, @fixed }
    private Type _type;

    /// <summary>
    /// Triggered when the alarm reaches 0.
    /// </summary>
    public Action onComplete;
    /// <summary>
    /// Triggered when the alarm reaches 0.  Listening methods must accept an Alarm parameter, which will be this alarm. Use this to remove listening methods on destroyed objects.
    /// </summary>
    public Action<Alarm> onCompleteDestroy;
    #endregion

    #region Properties
    /// <summary>
    /// Returns the name of the timer, "New" by default.
    /// </summary>
    public string Name { get => $"Alarm {_id}: {_name}"; }
    /// <summary>
    /// Return the time remaining, between 0 and maximum, clipped to decimal places set in Alarm Options
    /// </summary>
    public float TimeRemaining { get => Mathf.Max(ClipToDecimalPlace(_timeRemaining, alarmPrecision), 0f); }
    /// <summary>
    /// Return the maximum time for the alarm, clipped to decimal places set in Alarm Options
    /// </summary>
    public float TimeMax { get => ClipToDecimalPlace(_timeMax, alarmPrecision); }
    /// <summary>
    /// Return the time scale of the alarm
    /// </summary>
    public float TimeScale { get => _timeScale; }
    /// <summary>
    /// Returns true if the timer is neither paused nor stopped and there is time remaining
    /// </summary>
    public bool Playing { get => !_paused && !_stopped && _timeRemaining > 0; }
    /// <summary>
    /// Returns true if the alarm is paused
    /// </summary>
    public bool Paused { get => _paused; }
    /// <summary>
    /// Returns true if the alarm is stopped
    /// </summary>
    public bool Stopped { get => _stopped; }
    /// <summary>
    /// Returns true if the alarm is looping
    /// </summary>
    public bool Looping { get => _looping; }
    /// <summary>
    /// Returns true if the alarm will auto-release
    /// </summary>
    public bool AutoRelease { get => _autoRelease; }
    /// <summary>
    /// Returns the percent of time left in the alarm, starting at 1 and approaching 0 as the alarm runs
    /// </summary>
    public float PercentRemaining { get => _timeRemaining / _timeMax; }
    /// <summary>
    /// Returns the percent of time completed in the alarm, starting at 0 and approaching 1 as the alarm runs
    /// </summary>
    public float PercentComplete { get => 1 - _timeRemaining / _timeMax; }
    #endregion

    public static int alarmPrecision = 2;
    public static bool disableAllAutoRelease = false;
    public static bool disableAllComplete = false;
    private static int count;

    /// <summary>
    /// Runs the alarm for one frame.
    /// </summary>
    public void Run()
    {
        if (_type == Type.@fixed)
            return;

        TickToZero();
    }

    public void RunFixed()
    {
        if (_type != Type.@fixed)
            return;
        TickToZero();
    }

    private float ClipToDecimalPlace(float t, float decimals)
    {
        float precision = Mathf.Max(1, Mathf.Pow(10, decimals));
        return Mathf.Ceil(t * precision) / precision;
    }

    private void TickToZero()
    {
        if (_paused || _stopped)
            return;

        if (_timeRemaining == 0)
        {
            AttemptComplete(disableAllComplete);
        }

        if (_timeRemaining <= 0)
        {
            ManageAlarmAtZero();
            return;
        }

        _timeRemaining = Mathf.MoveTowards(_timeRemaining, 0, _timeScale * (_type == Type.scaled ? Time.deltaTime : _type == Type.unscaled ? Time.unscaledDeltaTime : Time.fixedDeltaTime));
    }

    private void AttemptComplete(bool disable)
    {
        if (disable)
            return;

        onComplete?.Invoke();
        onCompleteDestroy?.Invoke(this);
    }

    private void ManageAlarmAtZero()
    {
        if (_looping && !_stopped)
        {
            Reset();
            return;
        }

        if (_autoRelease && !disableAllAutoRelease)
        {
            Release();
            return;
        }

        Stop();
    }

    /// <summary>
    /// Play the alarm from its current time (a alarm at 0sec will loop/release but will not trigger the Complete event).
    /// </summary>
    public void Play()
    {
        _paused = false;
        _stopped = false;
    }

    /// <summary>
    /// Stop the alarm and set it to 0 (this will not release the alarm).
    /// </summary>
    public void Stop()
    {
        _stopped = true;
        _timeRemaining = -1;
    }

    /// <summary>
    /// Pause a alarm at its current time.
    /// </summary>
    public void Pause()
    {
        _paused = true;
    }

    /// <summary>
    /// Reset a alarm to maximum value (does not restart a paused/stopped alarm).
    /// </summary>
    public void Reset()
    {
        _timeRemaining = _timeMax;
    }

    /// <summary>
    /// Reset a alarm and its maximum to t (does not restart a paused/stopped alarm).
    /// </summary>
    public void Reset(float t)
    {
        _timeMax = t;
        _timeRemaining = _timeMax;
    }

    /// <summary>
    /// Reset a alarm to maximum value and start it.
    /// </summary>
    public void ResetAndPlay()
    {
        Reset();
        Play();
    }

    /// <summary>
    /// Reset a alarm and its maximum to t, and start it.
    /// </summary>
    public void ResetAndPlay(float t)
    {
        Reset(t);
        Play();
    }

    /// <summary>
    /// Adds or subtracts t seconds, clamping between 0 and maximum. Hitting 0 will trigger Complete.
    /// </summary>
    /// <param name="t"></param>
    public void AdjustTimeRemaining(float t)
    {
        _timeRemaining = Mathf.Clamp(_timeRemaining + t, 0, _timeMax);
    }

    /// <summary>
    /// Sets the time remaining to t seconds, clamping between 0 and maximum.
    /// </summary>
    /// <param name="t"></param>
    public void SetTimeRemaining(float t)
    {
        _timeRemaining = Mathf.Clamp(t, 0, _timeMax);
    }

    /// <summary>
    /// Sets the maximum time for the alarm to t seconds. If time remaning is larger than t, it will be set to t.
    /// </summary>
    /// <param name="t"></param>
    public void SetTimeMaximum(float t)
    {
        _timeMax = t;
        _timeRemaining = Mathf.Clamp(_timeRemaining, 0, _timeMax);
    }

    /// <summary>
    /// Release a alarm, effectively removing it from use (auto-releasing alarms will do this automatically on Complete, unless looping).
    /// </summary>
    public void Release()
    {
        onComplete = null;
        _alarmsInUse.Remove(this);
        if (_alarmPool.Count > maxAlarmsAllowed)
        {
            _alarmPool.Remove(this);
        }
    }

    /// <summary>
    /// Set whether the alarm loops automatically on completion.
    /// </summary>
    /// <param name="loop"></param>
    public void SetLooping(bool loop)
    {
        _looping = loop;
    }

    /// <summary>
    /// Set whether the alarm automatically releases on completion.
    /// </summary>
    /// <param name="release"></param>
    public void SetAutoRelease(bool release)
    {
        _autoRelease = release;
    }

    /// <summary>
    /// Set and start an alarm for t seconds, optionally set if it should loop or auto-release on complete, and what time scale to use. Add listeners to the Complete event with "myAlarm.Complete += MyMethod;"
    /// </summary>
    /// <param name="t"></param>
    /// <param name="looping"></param>
    /// <param name="scale"></param>
    /// <param name="autoRelease"></param>
    /// <returns></returns>
    public static Alarm Get(float t, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
    {
        if (AlarmRunner.Singleton == null)
        {
            Debug.LogWarning("No AlarmRunner found in scene, Alarms will not work. Creating a AlarmRunner.");
            AlarmRunner.CreateAlarmRunner();
        }

        Alarm alarm = null;
        bool newAlarm = true;

        if (_alarmsInUse.Count < _alarmPool.Count)  //if we have more alarms available than in use, we can grab from the pool
        {
            foreach (var curAlarm in _alarmPool)    //check the pool for the first alarm not in use
            {
                if (!_alarmsInUse.Contains(curAlarm))
                {
                    alarm = curAlarm;
                    newAlarm = false;
                    break;
                }
            }
        }

        if (alarm == null)
        {
            alarm = new();
            alarm._id = count;
            count++;
        }


        alarm.ApplyAlarmProperties("New", t, scale, autoRelease, looping, type);
        alarm._timeRemaining = t;
        alarm._stopped = false;
        alarm._paused = false;

        if (newAlarm)
            _alarmPool.Add(alarm);

        _alarmsInUse.Add(alarm);
        return alarm;
    }

    /// <summary>
    /// Set and start an alarm for t seconds and name it, optionally set if it should loop or auto-release on complete, and what time scale to use. Add listeners to the Complete event with "myAlarm.Complete += MyMethod;"
    /// </summary>
    /// <param name="t"></param>
    /// <param name="name"></param>
    /// <param name="looping"></param>
    /// <param name="autoRelease"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Alarm Get(float t, string name, bool looping = false, bool autoRelease = true, float scale = 1f, Type type = Type.scaled)
    {
        if (AlarmRunner.Singleton == null)
        {
            Debug.LogWarning("No AlarmRunner found in scene, Alarms will not work. Creating a AlarmRunner.");
            AlarmRunner.CreateAlarmRunner();
        }

        Alarm alarm = null;
        bool newAlarm = true;

        if (_alarmsInUse.Count < _alarmPool.Count)  //if we have more alarms available than in use, we can grab from the pool
        {
            foreach (var curAlarm in _alarmPool)    //check the pool for the first alarm not in use
            {
                if (!_alarmsInUse.Contains(curAlarm))
                {
                    alarm = curAlarm;
                    newAlarm = false;
                    break;
                }
            }
        }

        if (alarm == null)
        {
            alarm = new();
            alarm._id = count;
            count++;
        }

        alarm.ApplyAlarmProperties(name, t, scale, autoRelease, looping, type);
        alarm._timeRemaining = t;
        alarm._stopped = false;
        alarm._paused = false;

        if (newAlarm)
            _alarmPool.Add(alarm);

        _alarmsInUse.Add(alarm);

        return alarm;
    }

    private void ApplyAlarmProperties(string name, float t, float scale, bool autoRelease, bool looping, Type type)
    {
        _name = name;
        _timeMax = Mathf.Abs(t);
        _timeScale = scale;
        _autoRelease = autoRelease;
        _looping = looping;

    }

    /// <summary>
    /// Stops all alarms and sets their time to 0 (this will not trigger release).
    /// </summary>
    public static void StopAll()
    {
        foreach (var alarm in _alarmsInUse)
        {
            alarm.Stop();
        }
    }
    /// <summary>
    /// Pause all alarms at their current time.
    /// </summary>
    public static void PauseAll()
    {
        foreach (var alarm in _alarmsInUse)
        {
            alarm.Pause();
        }
    }
    /// <summary>
    /// Play all alarms from their current time.
    /// </summary>
    public static void PlayAll()
    {
        foreach (var alarm in _alarmsInUse)
        {
            alarm.Play();
        }
    }

    /// <summary>
    /// Reset all alarms to their max time.
    /// </summary>
    public static void ResetAll()
    {
        foreach (var alarm in _alarmsInUse)
        {
            alarm.Reset();
        }
    }

    /// <summary>
    /// Release all alarms.
    /// </summary>
    public static void ReleaseAll()
    {
        for (int i = 0; i < _alarmsInUse.Count; i++)
        {
            _alarmsInUse[i].Release();
            i--;
        }
    }
}

