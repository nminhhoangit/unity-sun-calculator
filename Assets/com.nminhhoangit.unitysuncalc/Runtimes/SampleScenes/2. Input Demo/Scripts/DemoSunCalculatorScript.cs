using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nminhhoangit.SunCalculator
{
    public class DemoSunCalculatorScript : MonoBehaviour
    {
        // Singleton
        public static DemoSunCalculatorScript Api;

        // UIs
        public DemoSunCalculatorUIScript UIScript;

        // Scenes
        public SunCalculator SunCalculator;

        // Public Params
        public bool IsIncreaseTime = false;
        public bool IsDecreaseTime = false;
        public bool IsUpdateRealtime = false;

        private void Awake()
        {
            Api = this;
        }

        private void Start()
        {
            InitView();
        }

        private void OnDestroy()
        {
            // Unregister event
            if (UIScript != null)
            {
                UIScript.OnInputDatasChangedEvent -= OnInputDatasChangedHandler;
            }
        }

        private void InitView()
        {
            // Init UIs
            UIScript?.InitView();

            // Init components
            OnInputDatasChangedHandler(0f, 0f, DateTime.Now);

            // Register event
            if (UIScript != null)
            {
                UIScript.OnInputDatasChangedEvent += OnInputDatasChangedHandler;
            }
        }

        /// <summary>
        /// New Latitude, lontitude, date & time input datas responsed from UI
        /// </summary>
        private void OnInputDatasChangedHandler(float latitude, float longtitude, DateTime datetime)
        {
            SunCalculator?.UpdateInputDatas(latitude, longtitude, datetime);
        }

        /// <summary>
        /// New date & time update from Main's button UI 
        /// </summary>
        /// <param name="datetime"></param>
        public void UpdateDateTimeInputDatas(DateTime datetime)
        {
            SunCalculator?.UpdateDateTimeInputDatas(datetime);
        }

        private void Update()
        {
            if (SunCalculator == null)
                return;

            // Update SunCalculator with real time
            if(IsUpdateRealtime)
            {
                SunCalculator.UpdateDateTimeInputDatas(DateTime.Now);
            }

            // Simulate auto increase/decrease datetime
            else if (IsDecreaseTime || IsIncreaseTime)
            {
                try
                {
                    DateTime newDateTimeInput = SunCalculator.GetDateTime().AddMinutes(IsIncreaseTime ? 1 : -1);
                    SunCalculator.UpdateDateTimeInputDatas(newDateTimeInput);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }

            // Set text date time on UI
            UIScript.SetTextSunCalcUI(SunCalculator.GetLatitude(), SunCalculator.GetLongitude(), SunCalculator.GetDateTime());
        }

        public void GetInputDatas(Action<float, float, DateTime> callback)
        {
            if (SunCalculator == null)
                return;

            callback?.Invoke(SunCalculator.GetLatitude(), SunCalculator.GetLongitude(), SunCalculator.GetDateTime());
        }
    }
}
