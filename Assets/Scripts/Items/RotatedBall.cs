using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatedBall : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] internal Vector2 rotation;
    [SerializeField] Vector2 startVelocity;
    [SerializeField] float perimeter = 4.4f;
    [SerializeField] float frictionCoef = 0.1f;
    [SerializeField] float frictionPower = 1f;
    [SerializeField] float rotateCoef = 1f;
    //[SerializeField] float rotationCoef = 0.1f;
    //[SerializeField] float rotationSlowingCoef = 0.1f;
    //[SerializeField] float slippageCoef = 0.1f;
    //[SerializeField] float slippagePower = 1.5f;

    [Header("Debug")]
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 rotateDirection;
    [SerializeField] Vector2 rotateSpeed;
    [SerializeField] Vector2 forwardSpeed;
    [SerializeField] float forwardSpeedModule;
    [SerializeField] Vector2 driftSpeed;
    [SerializeField] float dv;
    [SerializeField] float dw;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (model == null)
            model = gameObject;
        rb.velocity = startVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        //print($"!!! NEW FRAME !!!");
        //print($"rb.velocity: ({rb.velocity.x}; {rb.velocity.y}) - {rb.velocity.magnitude}");
        velocity = transform.InverseTransformVector(rb.velocity);
        if (rotation == Vector2.zero)
        {
            //rotateSpeed = Vector2.zero;
            //forwardSpeed = velocity;
            //driftSpeed = Vector2.zero;
            //rotateDirection = velocity.normalized;
            float deltaSpeed = -velocity.magnitude;
            dv = frictionCoef * deltaSpeed * Time.deltaTime;
            dw = -rotateCoef * dv;
            velocity += velocity.normalized * dv;
            rotation += dw * velocity.normalized;
        }
        else
        {
            rotateSpeed = transform.TransformVector(rotation) * perimeter;
            forwardSpeed = Vector3.Project(velocity, rotateSpeed);
            forwardSpeedModule = Vector3.Dot(velocity, rotateSpeed);
            forwardSpeedModule /= rotateSpeed.magnitude;
            driftSpeed = velocity - forwardSpeed;
            rotateDirection = rotation.normalized;

            float deltaSpeed = (rotateSpeed.magnitude - forwardSpeedModule);
            dv = frictionCoef * deltaSpeed * Time.deltaTime;
            dw = -rotateCoef * dv;
            velocity += rotateDirection * dv;
            //Vector2 localRotate = transform.InverseTransformVector(rotateDirection.normalized);
            rotation += dw * rotateDirection.normalized;

            deltaSpeed = -driftSpeed.magnitude;
            dv = frictionCoef * deltaSpeed * Time.deltaTime;
            dw = -rotateCoef * dv;
            velocity += driftSpeed.normalized * dv;
            //Vector2 localDrift = transform.InverseTransformVector(driftSpeed.normalized);
            rotation += dw * driftSpeed.normalized;

            rb.velocity = transform.TransformVector(velocity);
        }
        //print($"forwardSpeed: ({forwardSpeed.x}; {forwardSpeed.y}) - {forwardSpeed.magnitude}");
        //print($"need rotation: ({rotateSpeed.x}; {rotateSpeed.y}) - {rotateSpeed.magnitude}");
        //rotateSpeed = rotation * perimeter;
        //forwardSpeed = Vector3.Project(velocity, rotateSpeed);
        //driftSpeed = velocity - forwardSpeed;
        //rotateDirection = rotation.normalized;

        model.transform.Rotate(new Vector3(-rotation.y, rotation.x, 0), -360 * rotation.magnitude * Time.deltaTime);
        //model.transform.Rotate(-360 * rotation.y * Time.deltaTime, -360 * rotation.x * Time.deltaTime, 0, Space.Self);
        //model.transform.Rotate(0, 0, 360 * rotation.magnitude * Time.deltaTime);
        //Debug.DrawRay(transform.position, transform.TransformVector(rotation), Color.gray);
        //Debug.DrawRay(transform.position, rb.velocity, Color.magenta);
    }
}
