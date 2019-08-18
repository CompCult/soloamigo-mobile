using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SingleShadePlugin
{
    internal class LocationOverride
    {
        private AndroidJavaClass m_pluginClass = null;
        private static LocationOverride m_instance = null;
        private bool m_started = false;

        public bool IsStarted
        {
            get { return m_started; }
            set { m_started = value; }
        }

        public static LocationOverride Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new LocationOverride();

                    AndroidJavaClass unityInstance = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject context = unityInstance.GetStatic<AndroidJavaObject>("currentActivity");

                    // Let our plugin now about the Unity Activity.
                    m_instance.m_pluginClass = new AndroidJavaClass("com.singleshade.unitylocationoverride.UnityLocationOverride");
                    m_instance.m_pluginClass.CallStatic("Init", context);
                }

                return m_instance;
            }
        }

        private LocationOverride()
        {
            // Use a standard Unity location function to force 
            // the manifest to be populated with GPS permissions,
            // but never call it.
            bool a = false;
            if (a) Input.location.Start();
        }

        public void StartListening()
        {
            m_pluginClass.CallStatic("StartListening");
            m_started = true;
        }

        public void StopListening()
        {
            m_pluginClass.CallStatic("StopListening");
            m_started = false;
        }

        public string GetLocation()
        {
            return m_pluginClass.CallStatic<string>("GetLocation");
        }
    }
}