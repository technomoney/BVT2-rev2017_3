﻿namespace HeathenEngineering.Scriptable
{
    public interface IValueSensor<T> : IValueSensor
    {
        new T Value { get; }
    }

    public interface IValueSensor
    {
        object Value { get; }

        void Refresh();
    }
}
