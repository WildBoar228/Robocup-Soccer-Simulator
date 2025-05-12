using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDriveModule : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float mpuCalibration = 0;
    [SerializeField] float speedCoef = 2.5f;
    [SerializeField] float rotateCoef = 2.5f;
    [SerializeField] float torqueDragKoef = 0.5f;
    [SerializeField] float moveDragKoef = 0.5f;

    //[Header("PID вращения")]
    //[SerializeField] float torque_kp;
    //[SerializeField] float torque_kd;

    //[Header("Кинематика")]
    //[SerializeField] float currentTorque;
    //[SerializeField] Vector2 currentSpeed;

    [SerializeField] List<Motor> motors;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void singleMotorControl(int num, int speed)
    {
        motors[num - 1].speed = speed;
    }

    internal void drive(int m1, int m2, int m3, int m4)
    {
        m1 = Mathf.Clamp(m1, -255, 255);
        m2 = Mathf.Clamp(m2, -255, 255);
        m3 = Mathf.Clamp(m3, -255, 255);
        m4 = Mathf.Clamp(m4, -255, 255);
        singleMotorControl(1, m1);
        singleMotorControl(2, m2);
        singleMotorControl(3, m3);
        singleMotorControl(4, m4);
    }

    internal void drive(float angle, float rotation_speed, int speed)
    {
        float k1 = Mathf.Sin((45 - angle) * Mathf.Deg2Rad);
        float k2 = Mathf.Sin((45 + angle) * Mathf.Deg2Rad);
        drive((int)(speed * k2 + rotation_speed),
              (int)(speed * k1 + rotation_speed),
              (int)(speed * k2 - rotation_speed),
              (int)(speed * k1 - rotation_speed));
    }

    internal void drive(float angle, int speed)
    {
        float k1 = Mathf.Sin((45 - angle) * Mathf.Deg2Rad);
        float k2 = Mathf.Sin((45 + angle) * Mathf.Deg2Rad);
        drive((int)(speed * k2),
              (int)(speed * k1),
              (int)(speed * k2),
              (int)(speed * k1));
    }

    internal void driveXY(int speedX, int speedY, int rotationSpeed)
    {
        float sec45 = 1.4142135623730950488016887242097f / 2;
        drive((int)((speedY + speedX) * sec45) + rotationSpeed,
              (int)((speedY - speedX) * sec45) + rotationSpeed,
              (int)((speedY + speedX) * sec45) - rotationSpeed,
              (int)((speedY - speedX) * sec45) - rotationSpeed);
    }
}
