using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMPUDevice : MonoBehaviour
{
    [SerializeField] internal float calibrationAngle = 0;
    [SerializeField] float angleNow = 0;

    [Header("Задержка")]
    [SerializeField] float updateTime = 0.05f;
    [SerializeField] float timer = 0;

    [Header("Погрешность")]
    [SerializeField] float minError = -10;
    [SerializeField] float maxError = 10;
    [SerializeField] float kiError = 0;
    [SerializeField] float integralError = 0.1f;

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
            UpdateMPU();
            timer = 0;
        }
    }

    void UpdateMPU()
    {
        angleNow = -transform.localEulerAngles.z - calibrationAngle;
        float error = Random.Range(minError, maxError);
        angleNow += error + integralError;
        integralError += (0.5f * error) * kiError;
        angleNow = GameManager.Instance.constraintAngle(angleNow);
    }

    internal int mpuGetDegree()
    {
        return Mathf.RoundToInt(angleNow);
    }
}
