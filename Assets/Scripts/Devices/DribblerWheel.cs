using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DribblerWheel : MonoBehaviour
{
    [SerializeField] internal int turnovers = 0;
    [SerializeField] internal float transmissionCoef = 0.29f;
    [SerializeField] internal float speedCoef = 0;
    [SerializeField] internal bool isRightWheel = false;
    [SerializeField] internal float innerPushForce = 0.3f;
    [SerializeField] internal float outerPushForce = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        RotatedBall ball = collision.GetComponent<RotatedBall>();
        if (ball && turnovers > 0)
        {
            Vector2 rotation = ball.transform.InverseTransformDirection(-transform.up * turnovers * transmissionCoef);
            float localX = transform.InverseTransformPoint(collision.transform.position).x;
            Vector2 pushing = Vector2.zero;
            //Vector2 pushing = ball.transform.InverseTransformDirection(transform.TransformDirection(pushDirection * pushForce));
            if (localX > 0)
                pushing = transform.right;
            else if (localX < 0)
                pushing = -transform.right;
            float force = innerPushForce;
            if (localX > 0 && isRightWheel || localX < 0 && !isRightWheel)
            {
                force = outerPushForce;
            }
            rotation += pushing * turnovers * force;
            //float relativeSpeedModule = Vector3.Dot(ball.rotation, rotation);
            //if (rotation.magnitude != 0)
            //    relativeSpeedModule /= rotation.magnitude;
            ball.rotation += speedCoef * /*rotation.magnitude * */(rotation - ball.rotation);
        }
    }
}
