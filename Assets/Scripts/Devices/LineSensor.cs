using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSensor : MonoBehaviour
{
    CircleCollider2D cld;
    [SerializeField] LayerMask lineLayer;
    [SerializeField] internal int greenValue = 2000;
    [SerializeField] internal int whiteValue = 4000;
    [SerializeField] float lineEdge = 0.1734f;

    [SerializeField] internal float distance = 0;
    [SerializeField] internal int intensity = 0;
    [SerializeField] internal int maxError = 100;

    [Header("Задержка")]
    [SerializeField] float updateTime = 0.05f;
    [SerializeField] float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        cld = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateTime)
        {
            UpdateSensor();
            timer = 0;
        }
    }

    void UpdateSensor()
    {
        distance = Mathf.Infinity;
        foreach (Collider2D col in GameManager.Instance.lines)
        {
            distance = Mathf.Min(distance, cld.Distance(col).distance);// + cld.radius;
        }

        if (distance <= 0)
        {
            intensity = Mathf.FloorToInt(greenValue + (whiteValue - greenValue) * Mathf.Abs(distance) / (2 * cld.radius));
        }
        else
        {
            intensity = greenValue;
        }
        intensity = Mathf.Clamp(intensity, greenValue, whiteValue);
        intensity += Random.Range(-maxError, maxError);

        //GetComponent<SpriteRenderer>().color = Color.HSVToRGB((intensity - greenValue) / (whiteValue - greenValue), 1, 1);

        //if (GetComponent<Collider2D>().IsTouchingLayers(lineLayer))
        //{
        //    intensity = whiteValue;
        //    GetComponent<SpriteRenderer>().color = Color.red;
        //}
        //else
        //{
        //    intensity = greenValue;
        //    GetComponent<SpriteRenderer>().color = Color.green;
        //}
    }
}
