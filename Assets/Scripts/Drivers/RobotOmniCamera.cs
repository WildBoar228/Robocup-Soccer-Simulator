using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OmniCamBlobInfo_t
{
    [SerializeField] internal Color color = Color.white;
    [SerializeField] internal float left_angle = 360;
    [SerializeField] internal float right_angle = 360;
    [SerializeField] internal float center_angle = 360;
    [SerializeField] internal float clos_angle = 360;
    [SerializeField] internal float width = 0;
    [SerializeField] internal float distance = 1000;
}

public class RobotOmniCamera : MonoBehaviour
{
    [SerializeField] LayerMask brightObjectMask;
    [SerializeField] LayerMask interferenceMask;
    [SerializeField] List<OmniCamBlobInfo_t> blob;
    [SerializeField] List<ColorWithError> needColors;
    [SerializeField] internal int color;

    //[SerializeField] internal float minDist = 1;
    //[SerializeField] internal float maxDist = 1;
    [SerializeField] internal int sectors = 100;

    [SerializeField] internal float height;

    [Header("Задержка")]
    [SerializeField] int camera_timer = 0;
    [SerializeField] int updateTime = 30;
    //[SerializeField] float timer = 0;

    [Header("Данные")]
    [SerializeField] int cam_angle = 0;
    [SerializeField] internal int right_angle = 0;
    [SerializeField] internal int left_angle = 0;
    [SerializeField] int cam_height = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        readBlobs(0);
    }

    int acceptableColorIndex(Color clr)
    {
        for (int i = 0; i < needColors.Count; i++)
        //foreach (ColorWithError cwe in needColors)
        {
            ColorWithError cwe = needColors[i];
            float needH, needS, needV;
            Color.RGBToHSV(cwe.color, out needH, out needS, out needV);
            float h, s, v;
            Color.RGBToHSV(clr, out h, out s, out v);
            //print($"need color (h={needH}, s={needS}, v={needV})\ncurrent color (h={h}, s={s}, v={v})");
            if ((Mathf.Abs(needH - h) <= cwe.colorHSVdiap.x &&
                 Mathf.Abs(needS - s) <= cwe.colorHSVdiap.y &&
                 Mathf.Abs(needV - v) <= cwe.colorHSVdiap.z))
                return i;
        }
        //print($"NO SUCH COLOR {clr}");
        return -1;
    }

    int readBlobs(int arg)
    {
        float minDist = height * Mathf.Sqrt(3);
        bool blobFinished = true;
        blob.Clear();
        for (int i = 0; i < sectors; i++)
        {
            float angle = i * 360f / sectors;
            if (angle > 180)
                angle -= 360;
            Vector2 dir = new Vector2(
                Mathf.Sin((angle - transform.localEulerAngles.z) * Mathf.Deg2Rad),
                Mathf.Cos((angle - transform.localEulerAngles.z) * Mathf.Deg2Rad));

            Color c = new Color(i * 1f / sectors, i * 1f / sectors, i * 1f / sectors);
            //Debug.DrawRay((Vector2)transform.position + dir * minDist, dir * 10, c);
            RaycastHit2D[] objects = Physics2D.RaycastAll((Vector2)transform.position + dir * minDist, dir, 100, brightObjectMask | interferenceMask);
            foreach (RaycastHit2D obj in objects)
            {
                Vector2 dist = (new Vector3(obj.point.x, obj.point.y) - transform.position);
                if (obj.collider.gameObject == gameObject)
                    continue;
                BrightObject bo = obj.collider.GetComponent<BrightObject>();
                if (bo && acceptableColorIndex(bo.color) != -1)
                {
                    Color detectedColor = needColors[acceptableColorIndex(bo.color)].color;
                    if (blobFinished || blob.Count > 0 && blob[blob.Count - 1].color != detectedColor)
                    {
                        OmniCamBlobInfo_t b = new OmniCamBlobInfo_t();
                        b.left_angle = angle;
                        b.right_angle = angle;
                        b.center_angle = angle;
                        b.clos_angle = angle;
                        b.width = 1;
                        b.color = detectedColor;
                        b.distance = dist.magnitude; // height * (dist.magnitude - minDist) / (height + dist.magnitude * Mathf.Sqrt(3));
                        blob.Add(b);
                        blobFinished = false;
                    }
                    else
                    {
                        blob[blob.Count - 1].width += 360f / sectors;
                        blob[blob.Count - 1].right_angle = angle;
                        blob[blob.Count - 1].center_angle += 180f / sectors;
                        blob[blob.Count - 1].center_angle = GameManager.Instance.constraintAngle(blob[blob.Count - 1].center_angle);
                        float newDist = dist.magnitude; //height * (dist.magnitude - minDist) / (height + dist.magnitude * Mathf.Sqrt(3));
                        if (newDist < blob[blob.Count - 1].distance)
                        {
                            blob[blob.Count - 1].clos_angle = angle;
                            blob[blob.Count - 1].distance = newDist;
                        }
                    }
                    break;
                }
                else
                {
                    blobFinished = true;
                    if (bo)
                        break;
                }
            }
        }

        if (blob.Count >= 2 && blob[0].left_angle - blob[blob.Count - 1].right_angle <= 360f / sectors + 0.1f)
        {
            if (blob[0].color == blob[blob.Count - 1].color)
            {
                blob[0].left_angle = blob[blob.Count - 1].left_angle;
                blob[0].width += blob[blob.Count - 1].width;

                blob[0].center_angle = (blob[0].left_angle + blob[0].right_angle) / 2;
                if (blob[0].left_angle > blob[0].right_angle)
                    blob[0].center_angle -= 180;
                blob[0].center_angle = GameManager.Instance.constraintAngle(blob[0].center_angle);

                if (blob[blob.Count - 1].distance < blob[0].distance)
                {
                    blob[0].distance = blob[blob.Count - 1].distance;
                    blob[0].clos_angle = blob[blob.Count - 1].clos_angle;
                }
                else
                {
                    blob[0].distance = blob[0].distance;
                    blob[0].clos_angle = blob[0].clos_angle;
                }
                //blob[0].distance = Mathf.Min(blob[0].distance, blob[blob.Count - 1].distance);
                blob.RemoveAt(blob.Count - 1);
            }
        }

        //for (int i = 0; i < blob.Count; ++i)
        //{
        //    blob[i].center_angle = -blob[i].center_angle;
        //    blob[i].left_angle = -blob[i].left_angle;
        //    blob[i].right_angle = -blob[i].right_angle;
        //    blob[i].clos_angle = -blob[i].clos_angle;
        //}

        return blob.Count;
    }

    internal OmniCamBlobInfo_t getClosestObject(int colorIndex)
    {
        int index = -1;
        for (int i = 0; i < blob.Count; ++i)
        {
            if (acceptableColorIndex(blob[i].color) == colorIndex)
            {
                if (index == -1 || blob[i].distance < blob[index].distance)
                {
                    index = i;
                }
            }
        }

        return (index >= 0) ? blob[index] : null;
    }

    internal OmniCamBlobInfo_t getLargestObject(int colorIndex)
    {
        int index = -1;
        for (int i = 0; i < blob.Count; ++i)
        {
            if (acceptableColorIndex(blob[i].color) == colorIndex)
            {
                if (index == -1 || blob[i].width > blob[index].width)
                {
                    index = i;
                }
            }
        }

        if (index == -1)
        {
            OmniCamBlobInfo_t empty = new OmniCamBlobInfo_t();
            return empty;
        }
        return blob[index];
    }
}
