using System;
using UnityEngine;
using CosineKitty;

namespace nminhhoangit.SunCalculator
{
    public class SunCalculator : MonoBehaviour
    {
        // Public Params
        public Transform pillar;
        [Range(3f, 100f)]
        public float offsetDistance = 10f;

        // Private Params
        [SerializeField]
        private float m_Latitude;
        //
        [SerializeField]
        private float m_Longitude;
        //
        [SerializeField]
        [Range(1000, 9999)]
        private int m_Year = 1;
        //
        [SerializeField]
        [Range(1, 12)]
        private int m_Month = 1;
        //
        [SerializeField]
        [Range(1, 31)]
        private int m_Day = 1;
        //
        [SerializeField]
        [Range(0, 23)]
        private int m_Hour = 0;
        //
        [SerializeField]
        [Range(0, 59)]
        private int m_Minute = 0;
        //
        [SerializeField]
        [Range(0, 59)]
        private int m_Second = 0;

        /// <summary>
        /// Update Sun position, date & time input datas
        /// </summary>
        public void UpdateInputDatas(float latitude, float longtitude, DateTime datetime)
        {
            UpdateData(latitude, longtitude, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
        }

        /// <summary>
        /// Update date & time input datas
        /// </summary>
        public void UpdateDateTimeInputDatas(DateTime datetime)
        {
            UpdateData(m_Latitude, m_Longitude, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
        }

        /// <summary>
        /// Update Sun position, date & time datas
        /// </summary>
        public void UpdateData(float latitude, float longtitude, int year, int month, int day, int hour, int minute, int second)
        {
            m_Latitude = latitude;
            m_Longitude = longtitude;
            m_Year = year;
            m_Month = month;
            m_Day = day;
            m_Hour = hour;
            m_Minute = minute;
            m_Second = second;
        }

        /// <summary>
        /// Get current latitude data
        /// </summary>
        public float GetLatitude() { return m_Latitude; }


        /// <summary>
        /// Get current longtitude data
        /// </summary>
        public float GetLongitude() { return m_Longitude; }

        /// <summary>
        /// Get valid current data
        /// </summary>
        public DateTime GetDateTime()
        {
            if (CheckValidDateTime(m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second))
            {
                return new DateTime(m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second);
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Update call once per frame
        /// </summary>
        private void Update()
        {
            // Check valid before mapping new date & time data
            if (CheckValidDateTime(m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second))
            {
                UpdatePosition(m_Latitude, m_Longitude, m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second);
            }
            else
            {
                Debug.Log($"<color=red>Date/Time input not valid</color>: year={m_Year} month={m_Month}, day={m_Day}, hour={m_Hour}, minute={m_Minute}, second={m_Second}");
            }
        }

        /// <summary>
        /// Check if input data is valid DateTime
        /// </summary>
        private bool CheckValidDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            return DateTime.TryParse($"{year}-{month}-{day} {hour}:{minute}:{second}", out DateTime inputDateTime);
        }

        /// <summary>
        /// Update Sun's position and light direction
        /// </summary>
        private void UpdatePosition(float latitude, float longtitude, int year, int month, int day, int hour, int minute, int second)
        {
            try
            {
                // Setup Observer & AstroTime
                Observer observer = new Observer(latitude, longtitude, 0);
                AstroTime time = new AstroTime(new DateTime(year, month, day, hour, minute, second));

                // Coordinate Sun seen from Earth
                Equatorial equ_ofdate = Astronomy.Equator(Body.Sun, time, observer, EquatorEpoch.OfDate, Aberration.Corrected);
                Topocentric hor = Astronomy.Horizon(time, observer, equ_ofdate.ra, equ_ofdate.dec, Refraction.Normal);

                // Calculate object position based on azimuth, altitude, and distance from pillar
                Vector3 objectPosition = CalculateObjectPosition((float)hor.azimuth, (float)hor.altitude);

                // Update Sun's position & light with pillar
                if (pillar != null)
                {
                    // Offset distance position from pillar position
                    objectPosition = objectPosition * offsetDistance + pillar.position;

                    // Point light direction to the pillar
                    transform.LookAt(pillar);
                }
                else
                {
                    // Offset distance position
                    objectPosition = objectPosition * offsetDistance;

                    // Point light direction to the center of the scene instead
                    transform.LookAt(Vector3.zero, Vector3.up);
                }

                // Set object position
                transform.position = objectPosition;
            }
            catch (Exception ex)
            {
                Debug.Log($"CalculatePosition error: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Convert altitude and azimuth angles to spherical coordinates
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="altitude"></param>
        /// <returns></returns>
        private Vector3 CalculateObjectPosition(float azimuth, float altitude)
        {
            // Convert azimuth and altitude to radians
            float azimuthRad = Mathf.Deg2Rad * azimuth;
            float altitudeRad = Mathf.Deg2Rad * altitude;

            // Calculate object position based on azimuth, altitude, and distance from pillar
            float x = Mathf.Cos(altitudeRad) * Mathf.Sin(azimuthRad);
            float y = Mathf.Sin(altitudeRad);
            float z = Mathf.Cos(altitudeRad) * Mathf.Cos(azimuthRad);

            return new Vector3(x, y, z);
        }
    }
}
