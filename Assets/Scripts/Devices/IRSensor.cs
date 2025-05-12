using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRSensor : MonoBehaviour
{
    [SerializeField] int strength = 0;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Погрешность")]
    [SerializeField] int strengthError = 20;

    [Header("Задержка")]
    [SerializeField] float updateTime = 0.05f;
    [SerializeField] float timer = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateTime)
        {
            UpdateLocator();
            timer = 0;
        }
    }

    void UpdateLocator()
    {
        Vector2 signal = Vector2.zero;
        foreach (InfraredSource source in FindObjectsOfType<InfraredSource>())
        {
            Vector2 dist = source.transform.position - transform.position;
            if (!Physics2D.Raycast(transform.position, dist, dist.magnitude, obstacleLayer))
            {
                signal += dist.normalized * source.strength / (dist.magnitude + 0.0001f);
            }
        }

        strength = (int)signal.magnitude;

        strength += Random.Range(-strengthError, strengthError);
    }

    internal int getStrength()
    {
        return strength;
    }
}
