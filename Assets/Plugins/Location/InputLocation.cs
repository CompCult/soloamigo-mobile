using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace SingleShadePlugin
{
    public class InputLocation
    {
        private static LocationInfo m_lastData;
        private static DateTime m_lastTime = DateTime.MinValue;

        public static LocationServiceStatus status { get; private set; }

        /// <summary>
        /// Updated once per second.
        /// Altitude value can reflect if we're getting a gps or network position (0 is network position, >0 is true gps). It might be conditionned to data connectivity, not sure about that.
        /// </summary>
        public static LocationInfo lastData
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    if ((DateTime.Now - m_lastTime).TotalSeconds > 1)
                    {
                        FillWithData();
                        m_lastTime = DateTime.Now;
                    }
                }

                return m_lastData;
            }
        }

        public static bool isEnabledByUser
        {
            get { return Input.location.isEnabledByUser; }
        }

        public static void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
                LocationOverride.Instance.StartListening();
        }

        public static void Stop()
        {
            if (Application.platform == RuntimePlatform.Android)
                LocationOverride.Instance.StopListening();
        }

        /// <summary>
        /// Called once per second max.
        /// </summary>
        private static void FillWithData()
        {
            string location = LocationOverride.Instance.GetLocation();
            if (location.Equals("") == false)
            {
                string[] items = location.Split(';');

                // LastData.
                object boxedLastData = new LocationInfo();
                Type locationInfoType = typeof(LocationInfo);
                locationInfoType.GetField("m_Timestamp", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, double.Parse(items[5]) / 1000.0);
                locationInfoType.GetField("m_Latitude", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, (float)double.Parse(items[0]));
                locationInfoType.GetField("m_Longitude", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, (float)double.Parse(items[1]));
                locationInfoType.GetField("m_Altitude", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, (float)double.Parse(items[2]));
                float accuracy = (float)double.Parse(items[4]);
                locationInfoType.GetField("m_HorizontalAccuracy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, accuracy);
                locationInfoType.GetField("m_VerticalAccuracy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(boxedLastData, accuracy);
                m_lastData = (LocationInfo)boxedLastData;
            }

            // Status.
            if (LocationOverride.Instance.IsStarted)
            {
                if (location.Equals("") == false && isEnabledByUser)
                    status = LocationServiceStatus.Running;
                else
                    status = LocationServiceStatus.Initializing;
            }
            else
            {
                status = LocationServiceStatus.Stopped;
            }
        }
    }
}