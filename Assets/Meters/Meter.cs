using System;
using UnityEngine;

[Serializable]
public class Meter
{
    [SerializeField] private float _min = 0;
    [SerializeField] private float _max;
    [SerializeField] private float _value;
    [SerializeField] private float _rateUp = 1f;
    [SerializeField] private float _rateDown = 1f;
    public string name = "New Meter";

    /// <summary>
    /// Returns the minimum value of the meter
    /// </summary>
    public float Min { get => _min; }
    /// <summary>
    /// Returns the maximum value of the meter
    /// </summary>
    public float Max { get => _max; }
    /// <summary>
    /// Returns the current value of the meter
    /// </summary>
    public float Value { get => _value; }
    /// <summary>
    /// Returns how full the meter is, 0 being at minimum value and 1 being at maximum value
    /// </summary>
    public float Percent { get => (_value - _min) / (_max - _min); }
    /// <summary>
    /// Returns the rate the meter will adjust by when moving up
    /// </summary>
    public float RateUp { get => _rateUp; }
    /// <summary>
    /// Returns the rate the meter will adjust by when moving up
    /// </summary>
    public float RateDown { get => _rateDown; }
    /// <summary>
    /// Returns true if the meter is at or above maximum value
    /// </summary>
    public bool IsFull { get => _value >= _max; }
    /// <summary>
    /// Returns true if the meter is at or below minimum value
    /// </summary>
    public bool IsEmpty { get => _value <= _min; }
    /// <summary>
    /// Returns the numerical range of the meter
    /// </summary>
    public float Range { get => _max - _min; }

    /// <summary>
    /// Triggered when the meter reaches minimum value or lower
    /// </summary>
    public Action onMin;

    /// <summary>
    /// Triggered when the meter reaches maximum value or higher
    /// </summary>
    public Action onMax;

    /// <summary>
    /// Adjust the value of the meter by f, optionally disabling clamping to the min/max values
    /// </summary>
    /// <param name="f"></param>
    /// <param name="clamp"></param>
    public void Adjust(float f, bool clamp = true)
    {
        float rate = f > 0 ? _rateUp : _rateDown;
        f *= rate;
        _value = clamp ? Mathf.Clamp(_value + f, _min, _max) : _value + f;
        CheckForAction();
    }

    /// <summary>
    /// Adjust the value of the meter by f and out the overflow value, optionally disable clamping
    /// </summary>
    /// <param name="f"></param>
    /// <param name="overflow"></param>
    public void Adjust(float f, out float overflow, bool clamp = true)
    {
        float rate = f > 0 ? _rateUp : _rateDown;
        float n = f * rate;

        _value += n;

        overflow = 0;

        if (_value < _min || _value > _max)
        {
            float checkPoint = _value < _min ? _min : _max;
            overflow = (checkPoint - _value) / rate;

            if (clamp)
                _value = Mathf.Clamp(_value, _min, _max);
        }

        CheckForAction();
    }

    /// <summary>
    /// Set the current value to the maximum value
    /// </summary>
    public void Fill()
    {
        _value = _max;
    }

    /// <summary>
    /// Set the current value to the minimum value, and optionally disable triggering onMin event
    /// </summary>
    /// <param name="trigger"></param>
    public void Empty(bool trigger = true)
    {
        _value = _min;
        if (trigger)
            CheckForAction();
    }

    /// <summary>
    /// Clamp the current value between the minimum and maximum values
    /// </summary>
    public void Clamp()
    {
        _value = Mathf.Clamp(_value, _min, _max);
    }

    /// <summary>
    /// Set new minimum and maximum values for the meter, optionally disable clamping the current value between them
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="clip"></param>
    public void SetNewBounds(float min, float max, bool clip = true)
    {
        _min = min;
        _max = max;
        if (clip)
            _value = Mathf.Clamp(_value, _min, _max);
    }

    /// <summary>
    /// Checks if either min or max event should trigger.
    /// </summary>
    private void CheckForAction()
    {
        CheckOnMin();
        CheckOnMax();
    }

    /// <summary>
    /// Checks if current is at or lower than minimum, and invokes onMin if true.
    /// </summary>
    private void CheckOnMin()
    {
        if (_value > _min)
            return;
        onMin?.Invoke();
    }

    /// <summary>
    /// Checks if current is at or larger than maximum, and invokes onMax if true.
    /// </summary>
    private void CheckOnMax()
    {
        if (_value < _max)
            return;
        onMax?.Invoke();
    }

}
