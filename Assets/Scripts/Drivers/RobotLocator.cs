using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLocator : MonoBehaviour
{
    //[SerializeField] internal InfraredSource signalSource;
    [SerializeField] int angle = 0;
    [SerializeField] int strength = 0;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Погрешность")]
    [SerializeField] int constantAngleError = -10;
    [SerializeField] int angleError = 20;
    //[SerializeField] float kiAngleError = 0.01f;
    //[SerializeField] float integralAngleError = 0.1f;
    [SerializeField] int strengthError = 40;

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
            //RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(transform.position, dist, dist.magnitude, obstacleLayer);
            //string s = "";
            //foreach (RaycastHit2D hit in raycastHit2D)
            //{
            //    s += hit.collider.name + "   ";
            //}
            //print(s);
            if (!Physics2D.Raycast(transform.position, dist, dist.magnitude, obstacleLayer))
            {
                signal += dist.normalized * source.strength / (dist.magnitude + 0.0001f);
            }
        }

        angle = (int)(Mathf.Atan2(signal.x, signal.y) * Mathf.Rad2Deg + transform.eulerAngles.z) / 5 * 5;
        strength = (int)signal.magnitude;

        int angError = Random.Range(-angleError, angleError);
        angle += constantAngleError + angError;
        angle = GameManager.Instance.constraintAngle(angle);

        strength += Random.Range(-strengthError, strengthError);
    }

    internal int getBallAngle()
    {
        return angle;
    }

    internal int getStrength()
    {
        return strength;
    }
}
