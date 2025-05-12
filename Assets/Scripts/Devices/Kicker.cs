using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicker : MonoBehaviour
{
    [SerializeField] float velocityCoef = 0;
    //[SerializeField] internal float kickVelocity = 20;
    [SerializeField] internal float force = 1;
    [SerializeField] internal float hitEfficiency = 0.5f;
    [SerializeField] Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody2D>().velocity = new Vector2(0, velocity);
        transform.localEulerAngles = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 speed = Vector2.zero; // Vector2.Reflect(collision.relativeVelocity, normal) * hitEfficiency; // collision velocity
        speed += velocityCoef * force * new Vector2(transform.up.x, transform.up.y); // velocity of kicking
        //speed += rb.GetPointVelocity(collision.contacts[0].point);
        //print($"{collision.rigidbody.name}:  {Vector2.Reflect(collision.relativeVelocity * forceCoef, normal)} (collision) + {velocity * new Vector2(transform.up.x, transform.up.y)} (kicker velocity) + {rb.GetPointVelocity(collision.contacts[0].point)} (robot velocity) ");
        if (collision.rigidbody)
            collision.rigidbody.AddForceAtPosition(speed, collision.contacts[0].point, ForceMode2D.Impulse);
    }
}
