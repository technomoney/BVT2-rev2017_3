using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class TimeActivateScript : MonoBehaviour
    {
        public bool sunday;
        public bool monday;
        public bool tuesday;
        public bool wednesday;
        public bool thursday;
        public bool friday;
        public bool saturday;

        [Range(0, 23)]
        public int startHour;
        [Range(0, 59)]
        public int startMinutes;
        [Range(0, 23)]
        public int endHour;
        [Range(0, 59)]
        public int endMinutes;

        public bool forceHidden;

        bool[] activeDay;

        [System.NonSerialized]
        public bool active;

        // Only allow one script to activate/deactivate children
        [System.NonSerialized]
        public bool isParent;


        TimeActivateScript[] otherTimeScripts;

        void Awake()
        {
            CheckIfIsParent();

            CreateActiveDay();
            CheckIfShouldDisplay();
        }

        void Update()
        {
            CreateActiveDay();
            CheckIfShouldDisplay();
        }

        void CheckIfShouldDisplay()
        {
            active = false;
            float waitTime = 0;
            System.DateTime now = System.DateTime.Now;
            System.DateTime endOfDay = new System.DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            if (activeDay[(int)System.DateTime.Now.DayOfWeek])
            {
                System.DateTime startTime = new System.DateTime(now.Year, now.Month, now.Day, startHour, startMinutes, 0);
                System.DateTime endTime = new System.DateTime(now.Year, now.Month, now.Day, endHour, endMinutes, 59);
                if (System.DateTime.Now < startTime)
                    waitTime = (float)((startTime - now).TotalSeconds + 2);
                else if (System.DateTime.Now <= endTime)
                    active = true;
                else
                    waitTime = (float)((endOfDay - now).TotalSeconds + 2);
            }
            else
                waitTime = (float)((endOfDay - now).TotalSeconds + 2);

            if (isParent)
            {
                bool anyActive = active;
                foreach (TimeActivateScript otherTimeScript in otherTimeScripts)
                {
                    anyActive |= otherTimeScript.active && !otherTimeScript.forceHidden;
                    if (anyActive)
                        break;
                }

                foreach (Transform child in transform)
                    child.gameObject.SetActive(anyActive && !forceHidden);
            }

            // Wait until tomorrow if done for the day or today is excluded from possible days
            if (waitTime > 0)
                StartCoroutine(UpdateAfterDelay(waitTime));
        }

        IEnumerator UpdateAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);
            CheckIfShouldDisplay();
        }

        void CreateActiveDay()
        {
            activeDay = new bool[7];
            activeDay[0] = sunday;
            activeDay[1] = monday;
            activeDay[2] = tuesday;
            activeDay[3] = wednesday;
            activeDay[4] = thursday;
            activeDay[5] = friday;
            activeDay[6] = saturday;
        }

        void CheckIfIsParent()
        {
            List<TimeActivateScript> allTimeScripts = new List<TimeActivateScript>(gameObject.GetComponents<TimeActivateScript>());
            allTimeScripts.Remove(this);
            otherTimeScripts = allTimeScripts.ToArray();

            isParent |= otherTimeScripts.Length == 0;
            bool anyOtherParent = false;
            foreach (TimeActivateScript otherTimeScript in otherTimeScripts)
            {
                if (otherTimeScript.isParent)
                {
                    anyOtherParent = true;
                    break;
                }
            }

            isParent |= !anyOtherParent;
            if (!isParent)
                otherTimeScripts = null;
        }
    }
}