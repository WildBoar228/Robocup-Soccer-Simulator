using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    [SerializeField] Rigidbody2D parentBody;
    [SerializeField] internal float speedKP = 2.5f;
    [SerializeField] internal float speedKD = -0.1f;
    [SerializeField] internal float speed = 0;
    [SerializeField] internal float speedMistake = 0.5f;

    [SerializeField] internal float rforce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float actualSpeed = speed + speed * Random.Range(-speedMistake, speedMistake);
        Vector2 realForce = parentBody.GetPointVelocity(transform.position);
        rforce = realForce.magnitude;

        Vector2 force = speedKP * (transform.up * actualSpeed * Time.deltaTime);
        force += speedKD * Mathf.Abs((force - realForce).magnitude) * force.normalized;
        parentBody.AddForceAtPosition(force, transform.position, ForceMode2D.Force);
        if (speed != 0)
            Debug.DrawRay(transform.position, force.normalized * 1, Color.red);
    }
}
