using System;
using UnityEngine;
using CosineKitty;
using Unity.Mathematics;

public class SunCalculator : MonoBehaviour
{
    public Transform pillar;

    [Range(3f, 100f)]
    public float offsetDistance = 10f;

    [SerializeField]
    private float m_Latitude;

    [SerializeField]
    private float m_Longtitude;

    [SerializeField]
    private uint m_Year;

    [SerializeField]
    private uint m_Month;

    [SerializeField]
    private uint m_Day;

    [SerializeField]
    [Range(0, 23)]
    private uint m_Hour;

    [SerializeField]
    [Range(0, 59)]
    private uint m_Minute;

    [SerializeField]
    [Range(0, 59)]
    private uint m_Second;

    void Start()
    {
        DateTime now = DateTime.Now;
        UpdateData(0f, 0f, (uint)now.Year, (uint)now.Month, (uint)now.Day, (uint)now.Hour, (uint)now.Minute, (uint)now.Second);
    }

    void Update()
    {
        CalculatePosition(m_Latitude, m_Longtitude, m_Year, m_Month, m_Day, m_Hour, m_Minute, m_Second);
    }

    /// <summary>
    /// Update Sun position, date & time data
    /// </summary>
    public bool UpdateData(float latitude, float longtitude, uint year, uint month, uint day, uint hour, uint minute, uint second)
    {
        m_Latitude = latitude;
        m_Longtitude = longtitude;

        // Check valid before mapping new date & time data
        if (DateTime.TryParse($"{year}-{month}-{day} {hour}:{minute}:{second}", out DateTime inputDateTime))
        {
            m_Year = year;
            m_Month = month;
            m_Day = day;
            m_Hour = hour;
            m_Minute = minute;
            m_Second = second;

            return true;
        }
        else
        {
            return false;
        }
    }

    private void CalculatePosition(float latitude, float longtitude, uint year, uint month, uint day, uint hour, uint minute, uint second)
    {
        try
        {
            // Setup Observer & AstroTime
            Observer observer = new Observer(latitude, longtitude, 0);
            AstroTime time = new AstroTime(new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second));

            // Coordinate Sun seen from Earth
            Equatorial equ_ofdate = Astronomy.Equator(Body.Sun, time, observer, EquatorEpoch.OfDate, Aberration.Corrected);
            Topocentric hor = Astronomy.Horizon(time, observer, equ_ofdate.ra, equ_ofdate.dec, Refraction.Normal);

            // Calculate object position based on azimuth, altitude, and distance from pillar
            Vector3 objectPosition = CalculateObjectPosition((float)hor.azimuth, (float)hor.altitude);

            // Update Sun's position & light with pillar
            if(pillar != null)
            {
                // Offset distance position from pillar position
                objectPosition = objectPosition * offsetDistance + pillar.position;

                // Point light directly to the pillar
                transform.LookAt(pillar);
            }
            else
            {
                // Offset distance position
                objectPosition = objectPosition * offsetDistance;

                // Point light directly to the center of the scene instead
                transform.LookAt(Vector3.zero, Vector3.up);
            }

            // Set object position
            transform.position = objectPosition;
        }
        catch(Exception ex)
        {
            Debug.Log($"CalculatePosition error: { ex.ToString() }");
        }
    }

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

