using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSoftware : MonoBehaviour
{
    protected RobotDriveModule rdm;
    protected RobotMPUDevice mpu;
    protected RobotLineMux mux;
    protected RobotLocator loc;
    protected RobotTrackingCam cam;
    protected RobotOmniCamera omnicam;
    protected RobotKickerDriver kicker;
    protected RobotDribblerDriver dribbler;
    protected IRSensor irs;

    public enum RobotGameMode
    {
        None,
        YellowForward,
        YellowGoalkeeper,
        BlueForward,
        BlueGoalkeeper,
    }

    [SerializeField] RobotGameMode gameMode = RobotGameMode.None;

    [Header("Константы")]
    [SerializeField] internal float loopCallInterval = 0;

    [Header("Управляющие переменные")]
    [SerializeField] internal float timer = 0;
    [SerializeField] internal int gameState = 0;

    [Header("Игровая логика")]
    [SerializeField] internal float moveAngle = 0;
    [SerializeField] internal float deltaAngle = 0;
    [SerializeField] internal int speed = 0;

    [SerializeField] internal float ballAngle = 0;
    [SerializeField] internal float lineAngle = 0;
    [SerializeField] internal float cameraAngle = 0;
    [SerializeField] internal int robotAngle = 0;

    [SerializeField] internal int speedX = 0;
    [SerializeField] internal int speedY = 0;

    [SerializeField] internal float isBallDiap = 20;
    [SerializeField] internal float isBallStrength = 70;

    internal const float DEG_TO_RAD = Mathf.Deg2Rad;
    internal const float RAD_TO_DEG = Mathf.Rad2Deg;

    protected bool firstIter = true;

    void Start()
    {
        rdm = GetComponent<RobotDriveModule>();
        mpu = GetComponent<RobotMPUDevice>();
        mux = GetComponent<RobotLineMux>();
        loc = GetComponent<RobotLocator>();
        cam = GetComponent<RobotTrackingCam>();
        omnicam = GetComponent<RobotOmniCamera>();
        kicker = GetComponent<RobotKickerDriver>();
        dribbler = GetComponent<RobotDribblerDriver>();
        irs = GetComponent<IRSensor>();

        Invoke(nameof(SwitchMode), 0.5f);
    }

    internal void SwitchMode()
    {
        switch (gameMode)
        {
            case RobotGameMode.YellowForward:
                StartCoroutine(nameof(playForward), 0);
                break;
            case RobotGameMode.BlueForward:
                StartCoroutine(nameof(playForward), 1);
                break;
            case RobotGameMode.YellowGoalkeeper:
                StartCoroutine(nameof(playGoalkeeper), 0);
                break;
            case RobotGameMode.BlueGoalkeeper:
                StartCoroutine(nameof(playGoalkeeper), 1);
                break;
        }
    }

    void Update()
    {

    }

    IEnumerator playForward(int color)
    {
        while (true)
        {
            yield return new WaitForSeconds(loopCallInterval / 1000f);

            cameraAngle = omnicam.getLargestObject(color ^ 1).center_angle;
            ballAngle = loc.getBallAngle();
            lineAngle = mux.getLineAngle_Avg();

            if (cameraAngle == 360)
                continue;

            if (lineAngle == 360)
            {
                rdm.drive(ballAngle, cameraAngle * 0.5f, speed);
            }
            else
            {
                rdm.drive(fitAngle(lineAngle + 180), 0, 80);
            }

            if (omnicam.getLargestObject(color ^ 1).distance < 5)
                kicker.Kick();
        }
    }

    IEnumerator playGoalkeeper(int color)
    {
        while (true)
        {
            yield return new WaitForSeconds(loopCallInterval / 1000f);

            cameraAngle = omnicam.getLargestObject(color).center_angle;
            ballAngle = loc.getBallAngle();
            lineAngle = mux.getLineAngle_Avg();
            robotAngle = mpu.mpuGetDegree();

            if (cameraAngle == 360)
                continue;

            if (lineAngle == 360)
            {
                moveAngle = (ballAngle > 0 ? 100 : -100);
                rdm.drive(moveAngle, -robotAngle * 0.5f, speed);
            }
            else
            {
                rdm.drive(fitAngle(lineAngle + 180), 0, 80);
            }
        }
    }

    protected float fitAngle(float angle)
    {
        return GameManager.Instance.constraintAngle(angle);
    }

    protected bool isBall()
    {
        if (irs)
            return irs.getStrength() >= isBallStrength;
        return (loc.getStrength() >= isBallStrength) && (Mathf.Abs(loc.getBallAngle()) <= isBallDiap);
    }

    protected int constrain(int val, int minim, int maxim)
    {
        return Mathf.Clamp(val, minim, maxim);
    }

    protected float constrain(float val, float minim, float maxim)
    {
        return Mathf.Clamp(val, minim, maxim);
    }

    protected OmniCamBlobInfo_t gate(int color)
    {
        return omnicam.getLargestObject(color);
    }
}
