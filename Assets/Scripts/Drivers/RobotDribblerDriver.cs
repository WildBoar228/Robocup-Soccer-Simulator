using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDribblerDriver : MonoBehaviour
{
    [SerializeField] DribblerWheel wheel1;
    [SerializeField] DribblerWheel wheel2;
    [SerializeField] int turnovers = 0;
    [SerializeField] GameObject dribbler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turnovers > 0)
        {
            float rotSpeed = -dribbler.transform.localEulerAngles.z * 2;
            if (rotSpeed == 0)
                rotSpeed = turnovers * 0.05f;
            // print($"rotate: {rotSpeed}");
            dribbler.transform.Rotate(0, 0, rotSpeed, Space.Self);
        }
        else
        {
            dribbler.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    internal void write(int value)
    {
        turnovers = value;
        wheel1.turnovers = turnovers;
        wheel2.turnovers = turnovers;
    }
}
