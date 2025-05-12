using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLineMux : MonoBehaviour
{
    [SerializeField] List<LineSensor> sensors;

    private void Start()
    {

    }

    private void Update()
    {
        SerializeSensors();
    }

    internal int readMux(int channel)
    {
        return sensors[channel].intensity;
    }

    internal bool isLineOnSensor(int sensor)
    {
        return (readMux(sensor) - sensors[sensor].greenValue >= (sensors[sensor].whiteValue - sensors[sensor].greenValue) * 0.5);
    }

    internal void SerializeSensors()
    {
        for (int i = 0; i < sensors.Count; i++)
        {
            if (isLineOnSensor(i))
            {
                sensors[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                sensors[i].GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    internal void getLineDirection_Avg(out float x, out float y)
    {
        float sumX = 0;
        float sumY = 0;
        int k = 0;

        for (int i = 0; i < 16; i++)
        {
            if (isLineOnSensor(i))
            {
                float ang = (16 - i) * 22.5f;
                k++;
                sumX += Mathf.Sin(ang * Mathf.Deg2Rad);
                sumY += Mathf.Cos(ang * Mathf.Deg2Rad);
            }
        }
        if (k == 0)
        {
            x = 0;
            y = 0;
        }
        else
        {
            x = sumX / k;
            y = sumY / k;
            float length = Mathf.Sqrt(x * x + y * y);
            x /= length;
            y /= length;
        }
    }

    internal int getLineAngle_Avg()
    {
        float x, y;
        getLineDirection_Avg(out x, out y);
        if (x == 0 && y == 0)
            return 360;
        float angle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        return Mathf.RoundToInt(angle);
    }
}