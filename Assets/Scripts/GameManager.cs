using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] internal List<Collider2D> lines;
    [SerializeField] internal int startTime;

    [Header("Правила игры")]
    [SerializeField] internal bool isGame = true;
    [SerializeField] internal GameObject ball;
    [SerializeField] internal float criticalDist = 3;
    [SerializeField] internal float outTime = 60;
    [SerializeField] internal List<GameObject> robots; // yf, yg, bf, bg
    [SerializeField] List<GameObject> neutralPoints;

    [SerializeField] Transform yellowFront;
    [SerializeField] Transform yellowMiddle;
    [SerializeField] Transform yellowBack;
    [SerializeField] Transform blueFront;
    [SerializeField] Transform blueMiddle;
    [SerializeField] Transform blueBack;

    // Start is called before the first frame update
    void Start()
    {
        Instance = FindObjectOfType<GameManager>();
        startTime = (int)(Time.time * 1000);
    }

    // Update is called once per frame
    void Update()
    {
        //print($"millis(): {millis()}");
        float zoom = Camera.main.GetComponent<CameraMove>().zoom;
        zoom = Mathf.Clamp(zoom + -Input.mouseScrollDelta.y, 1, 15);
        Camera.main.GetComponent<CameraMove>().zoom = zoom;

        if (Input.GetKeyDown(KeyCode.N))
        {
            BallToNeutralPoint();
        }

        if (Input.GetKey(KeyCode.O))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(nameof(RobotOut), 0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(nameof(RobotOut), 1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(nameof(RobotOut), 2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StartCoroutine(nameof(RobotOut), 3);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                RestartGame(-1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RestartGame(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RestartGame(1);
            }
        }
    }

    void BallToNeutralPoint()
    {
        float minDist = Mathf.Infinity;
        Vector2 pos = new Vector2(1000, 1000);
        foreach (GameObject p in neutralPoints)
        {
            float distToRobots = criticalDist;
            foreach (GameObject robot in robots)
            {
                if (robot && robot.activeSelf)
                    distToRobots = Mathf.Min(distToRobots, (robot.transform.position - p.transform.position).magnitude);
            }

            if (distToRobots < minDist)
            {
                if (distToRobots > criticalDist)
                {
                    print($"set minDist to {distToRobots}");
                    minDist = distToRobots;
                    pos = p.transform.position;
                }
            }
        }

        if (minDist == Mathf.Infinity)
        {
            float maxDist = 0;
            foreach (GameObject p in neutralPoints)
            {
                float distToRobots = criticalDist;
                foreach (GameObject robot in robots)
                {
                    if (robot && robot.activeSelf)
                        distToRobots = Mathf.Min(distToRobots, (robot.transform.position - p.transform.position).magnitude);
                }

                if (distToRobots > maxDist)
                {
                    maxDist = distToRobots;
                    pos = p.transform.position;
                }
            }
        }

        ball.transform.position = pos;
    }

    internal IEnumerator RobotOut(int index)
    {
        if (index < robots.Count && robots[index])
        {
            robots[index].SetActive(false);

            yield return new WaitForSeconds(outTime);

            float maxDist = 0;
            Vector2 pos = neutralPoints[0].transform.position;
            foreach (GameObject p in neutralPoints)
            {
                float distToRobots = Mathf.Infinity;
                foreach (GameObject robot in robots)
                {
                    if (robot && robot.activeSelf)
                        distToRobots = Mathf.Min(distToRobots, (robot.transform.position - p.transform.position).magnitude);
                }
                if (distToRobots > maxDist)
                {
                    maxDist = distToRobots;
                    pos = p.transform.position;
                }
            }

            robots[index].transform.position = pos;
            robots[index].SetActive(true);

            robots[index].transform.eulerAngles = new Vector3(0, 0,
                constraintAngle(robots[index].GetComponent<RobotMPUDevice>().calibrationAngle + 180));

            robots[index].GetComponent<RobotSoftware>().SwitchMode();
        }
    }

    public void RestartGame(int side = -1)
    {
        if (robots[0])
        {
            robots[0].SetActive(true);
            robots[0].GetComponent<RobotSoftware>().gameState = 0;
        }
        if (robots[1])
        {
            robots[1].SetActive(true);
            robots[1].GetComponent<RobotSoftware>().gameState = 0;
        }
        if (robots[2])
        {
            robots[2].SetActive(true);
            robots[2].GetComponent<RobotSoftware>().gameState = 0;
        }
        if (robots[3])
        {
            robots[3].SetActive(true);
            robots[3].GetComponent<RobotSoftware>().gameState = 0;
        }

        StopAllCoroutines();

        if (side == -1)
        {
            if (robots[0])
            {
                robots[0].transform.position = yellowMiddle.position;
                robots[0].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[1])
            {
                robots[1].transform.position = yellowBack.position;
                robots[1].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[2])
            {
                robots[2].transform.position = blueMiddle.position;
                robots[2].transform.eulerAngles = new Vector3(0, 0, 180);
            }
            if (robots[3])
            {
                robots[3].transform.position = blueBack.position;
                robots[3].transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }
        if (side == 0)
        {
            if (robots[0])
            {
                robots[0].transform.position = yellowFront.position;
                robots[0].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[1])
            {
                robots[1].transform.position = yellowMiddle.position;
                robots[1].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[2])
            {
                robots[2].transform.position = blueMiddle.position;
                robots[2].transform.eulerAngles = new Vector3(0, 0, 180);
            }
            if (robots[3])
            {
                robots[3].transform.position = blueBack.position;
                robots[3].transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }
        if (side == 1)
        {
            if (robots[0])
            {
                robots[0].transform.position = yellowMiddle.position;
                robots[0].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[1])
            {
                robots[1].transform.position = yellowBack.position;
                robots[1].transform.eulerAngles = new Vector3(0, 0, 0);
            }
            if (robots[2])
            {
                robots[2].transform.position = blueFront.position;
                robots[2].transform.eulerAngles = new Vector3(0, 0, 180);
            }
            if (robots[3])
            {
                robots[3].transform.position = blueMiddle.position;
                robots[3].transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }

        foreach (GameObject rob in robots)
            if (rob)
                rob.GetComponent<RobotSoftware>().SwitchMode();

        ball.transform.position = (Vector2)neutralPoints[0].transform.position;
        ball.transform.eulerAngles = Vector3.zero;
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball.GetComponent<RotatedBall>().rotation = Vector2.zero;
    }

    internal int millis()
    {
        return (int)(Time.time * 1000) - startTime;
    }

    internal float constraintAngle(float angle)
    {
        angle %= 360;
        if (angle < -180)
            angle += 360;
        if (angle > 180)
            angle -= 360;
        return angle;
    }

    internal int constraintAngle(int angle)
    {
        angle %= 360;
        if (angle < -180)
            angle += 360;
        if (angle > 180)
            angle -= 360;
        return angle;
    }
}
