using System;
using UI.Dates;
using UnityEngine;

public class Testing_DatePicker : MonoBehaviour
{
    public DatePicker m_calendar;

    private void Start()
    {
        m_calendar.Config.Events.OnDaySelected.AddListener(DaySelected);
    }

    private void DaySelected(DateTime date)
    {
        Debug.Log("Day selected: " + date.Month + " " + date.Day + " " + date.Year);
    }
}