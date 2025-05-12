using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotKickerDriver : MonoBehaviour
{
    /*[SerializeField] */Animator anim;
    /*[SerializeField] */Collider2D col;
    [SerializeField] Kicker kicker;
    //[SerializeField] float forceCoef = 20;
    //[SerializeField] Rigidbody2D rb;
    //[SerializeField] bool isKicking = false;
    //[SerializeField] bool stopped = false;
    //[SerializeField] bool isReturning = false;
    //[SerializeField] float minY = 0.33f;
    //[SerializeField] float maxY = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        anim = kicker.GetComponent<Animator>();
        col = kicker.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isKicking)
        //{
        //    rb.velocity = Vector2.zero;
        //}
        //else
        //{
        //    if (!isReturning)
        //    {
        //        rb.velocity = transform.up * force;
        //    }
        //    else
        //    {
        //        rb.velocity = -transform.up * force;
        //    }
        //}
        //if (isKicking && !isReturning && rb.transform.localPosition.y >= maxY)
        //{
        //    //isReturning = true;
        //    //rb.velocity = Vector2.zero;
        //    ReturnAfterKicking();
        //    //Invoke(nameof(ReturnAfterKicking), 0.2f);
        //}
        //if (isReturning && rb.transform.localPosition.y <= minY)
        //{
        //    FinishKicking();
        //}
        //float y = Mathf.Clamp(rb.transform.localPosition.y, minY, maxY);
        //rb.transform.localPosition = new Vector3(rb.transform.localPosition.x, y, rb.transform.localPosition.z);
    }

    internal void Kick()
    {
        //if (!isKicking)
        //{
        //    print("Kick");
        //    //rb.AddForce(transform.up * force, ForceMode2D.Impulse);
        //    isKicking = true;
        //    isReturning = false;
        //}
        anim.SetTrigger("Kick");
    }

    //void ReturnAfterKicking()
    //{
    //    print("ReturnAfterKicking");
    //    //rb.AddForce(-transform.up * force, ForceMode2D.Impulse);
    //    isReturning = true;
    //    isKicking = true;
    //}

    //void FinishKicking()
    //{
    //    rb.velocity = Vector2.zero;
    //    isKicking = false;
    //    isReturning = false;
    //    rb.transform.localPosition = new Vector3(rb.transform.localPosition.x, minY, rb.transform.localPosition.z);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Vector2 normal = collision.contacts[0].normal;
    //    print($"{collision.otherRigidbody.name}  {Vector2.Reflect(collision.relativeVelocity * 10, normal)}");
    //    collision.otherRigidbody.AddForce(Vector2.Reflect(collision.relativeVelocity * 10, normal), ForceMode2D.Impulse);
    //}
}
