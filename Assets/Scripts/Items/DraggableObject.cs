using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Vector3 localClickPoint;
    [SerializeField] internal bool useRigidbody = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        localClickPoint = transform.InverseTransformPoint(mousePos);
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (useRigidbody)
        {
            rb.AddForceAtPosition((mousePos - transform.TransformPoint(localClickPoint)), localClickPoint, ForceMode2D.Force);
        }
        else
        {
            transform.position += mousePos - transform.TransformPoint(localClickPoint);
        }

        if (Input.GetKey(KeyCode.O))
        {
            int index = GameManager.Instance.robots.IndexOf(gameObject);
            print($"out index = {index}");
            if (index != -1)
            {
                GameManager.Instance.StartCoroutine(nameof(GameManager.RobotOut), index);
            }
        }
    }
}
