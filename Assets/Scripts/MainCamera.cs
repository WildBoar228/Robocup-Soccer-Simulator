using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] internal float speed;
    [SerializeField] internal float borderRight = 15;
    [SerializeField] internal float borderLeft = 15;
    [SerializeField] internal float borderDown = 7;
    [SerializeField] internal float borderTop = 7;
    [SerializeField] internal float deltaSize = 0.1f;
    [SerializeField] internal float resize_intensity = 10;
    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime * cam.rect.width / 18 * cam.orthographicSize, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-speed * Time.deltaTime * cam.rect.width / 18 * cam.orthographicSize, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0, -speed * Time.deltaTime * cam.rect.width / 18 * cam.orthographicSize, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0, speed * Time.deltaTime * cam.rect.width / 18 * cam.orthographicSize, 0);
        }
        
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 deltaPos = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
            cam.orthographicSize *= 1 - Input.mouseScrollDelta.y * Time.deltaTime * resize_intensity;
            transform.position = cam.ScreenToWorldPoint(Input.mousePosition - deltaPos * (1 - Input.mouseScrollDelta.y * Time.deltaTime * resize_intensity));
        }
    }
}
