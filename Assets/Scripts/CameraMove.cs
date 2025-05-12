using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Camera thisCam;

    [SerializeField] internal bool hasTarget = true;
    [SerializeField] internal GameObject target;
    [SerializeField] internal Vector3 offset;
    [SerializeField] internal float zoom = 5;
    [SerializeField] internal float zoomDefault = 5;

    [SerializeField] internal bool moveChoppy = false;
    [SerializeField] internal float moveSpeed = 3;
    [SerializeField] internal float smoothTime = 0.1f;
    [SerializeField] internal bool zoomChoppy = false;
    [SerializeField] internal float zoomSpeed = 3;

    [SerializeField] internal Vector3 bgScaleUnit;
    [SerializeField] internal GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        thisCam = GetComponent<Camera>();
        thisCam.orthographicSize = zoomDefault;
        if (background)
            bgScaleUnit = background.transform.localScale / zoomDefault;
    }

    // Update is called once per frame
    void Update()
    {
        //Invoke(nameof(UpdateMoving), 0);
        //UpdateMoving();

        /*if (!hasTarget && Input.mouseScrollDelta.y != 0)
        {
            Vector3 deltaPos = Input.mousePosition - thisCam.WorldToScreenPoint(transform.position);
            float deltaScale = (Input.mouseScrollDelta.y > 0) ? 0.9f : 1.1f;
            zoom *= deltaScale;
            target.transform.position = thisCam.ScreenToWorldPoint(Input.mousePosition - deltaPos * deltaScale);
        }*/
    }

    void FixedUpdate()
    {
        if (thisCam)
            UpdateMoving();
    }

    internal void UpdateMoving()
    {
        //print("update camera moving");
        if (target)
        {
            float Z = transform.position.z;
            if (moveChoppy)
            {
                transform.position = target.transform.position;
            }
            else
            {
                Vector3 delta = (target.transform.position + offset) - transform.position;
                Vector3 speed = delta.normalized * moveSpeed;
                transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref speed, smoothTime);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, Z);
        }

        if (zoomChoppy)
        {
            thisCam.orthographicSize = zoom;
        }
        else
        {
            thisCam.orthographicSize = Mathf.Lerp(thisCam.orthographicSize, zoom, zoomSpeed * Time.deltaTime);
        }

        if (background)
            background.transform.localScale = bgScaleUnit * thisCam.orthographicSize;
    }

    internal float ScaleCoef()
    {
        return Camera.main.orthographicSize / zoomDefault;
    }

    internal float CornerDistanceFromCenter(Vector3 direction)
    {
        direction.Normalize();
        Vector3 size = (Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)) -
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)));
        float ans = Mathf.Infinity;
        if (direction.x != 0)
        {
            ans = Mathf.Min(ans, Mathf.Abs(size.x / 2f / direction.x));
        }
        if (direction.y != 0)
        {
            ans = Mathf.Min(ans, Mathf.Abs(size.y / 2f / direction.y));
        }
        //print($"corner dist {size.x / 2f / direction.x} , {size.y / 2f / direction.y}");
        return Mathf.Abs(ans);
    }
}
