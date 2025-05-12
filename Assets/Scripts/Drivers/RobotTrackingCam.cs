using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackingCamBlobInfo_t
{
    [SerializeField] internal Color color = Color.white;
    [SerializeField] internal int area = 0;
    [SerializeField] internal int cx = 0;
    [SerializeField] internal int left = 0;
    [SerializeField] internal int right = 0;
    [SerializeField] internal int top = 0;
}

[System.Serializable]
public class ColorWithError
{
    [SerializeField] internal Color color = Color.white;
    [SerializeField] internal Vector3 colorHSVdiap;
}

public class RobotTrackingCam : MonoBehaviour
{
    [SerializeField] float lensSize = 0.1f;
    [SerializeField] float lensWidth;
    [SerializeField] float lensH;
    [SerializeField] float viewAngle = 70;
    [SerializeField] int pixels = 320;
    [SerializeField] float heightCoef = 285;
    [SerializeField] LayerMask brightObjectMask;
    [SerializeField] LayerMask interferenceMask;
    [SerializeField] List<TrackingCamBlobInfo_t> blob;
    [SerializeField] List<ColorWithError> needColors;
    //[SerializeField] internal Color needBlobColor;
    //[SerializeField] internal Vector3 colorHSVdiap;
    [SerializeField] internal int color;
    [SerializeField] internal float centerAngle = 0;

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
        //timer += Time.deltaTime;
        //if (timer >= updateTime)
        //{
        //    readBlobs();
        //}
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
        lensWidth = Mathf.Sqrt(2 * lensSize * lensSize * (1 - Mathf.Cos(viewAngle * Mathf.Deg2Rad)));
        lensH = lensSize * lensSize * Mathf.Sin(viewAngle * Mathf.Deg2Rad) / lensWidth;
        bool blobFinished = true;
        //string output = "Read blobs: ";
        blob.Clear();
        for (int i = 0; i < pixels; i++)
        {
            float x = (1f * i / pixels) * lensWidth;
            Vector2 dir = transform.TransformDirection(new Vector2(x - lensWidth / 2, lensH));

            Vector3 axis = Vector3.Cross(dir, Vector3.up);
            if (axis == Vector3.zero) axis = Vector3.right;
            dir = Quaternion.AngleAxis(-centerAngle, axis) * dir;

            //output += $"\n\npixel {i}: ";
            RaycastHit2D[] objects = Physics2D.RaycastAll(transform.position, dir, 100, brightObjectMask | interferenceMask);
            foreach (RaycastHit2D obj in objects)
            {
                //output += $"\n{obj.collider.name} ";
                Vector2 dist = (new Vector3(obj.point.x, obj.point.y) - transform.position);
                if (obj.collider.gameObject == gameObject)
                    continue;
                BrightObject bo = obj.collider.GetComponent<BrightObject>();
                if (bo && acceptableColorIndex(bo.color) != -1)
                {
                    Color detectedColor = needColors[acceptableColorIndex(bo.color)].color;
                    if (blobFinished || blob.Count > 0 && blob[blob.Count - 1].color != detectedColor)
                    {
                        //output += "N";
                        TrackingCamBlobInfo_t b = new TrackingCamBlobInfo_t();
                        b.cx = i;
                        b.left = i;
                        b.right = i;
                        b.area = 1;
                        b.color = detectedColor;
                        /*float x2 = (1f * b.cx / pixels) * lensWidth;
                        Vector2 dirCenter = transform.TransformDirection(new Vector2(x - lensWidth / 2, lensH));
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirCenter, 100, brightObjectMask | interferenceMask);*/
                        Vector2 distCenter = (new Vector3(obj.point.x, obj.point.y) - transform.position);
                        /*print("");
                        print($"dirCenter = {dirCenter}");
                        print($"distCenter = {distCenter}");*/
                        b.top = Mathf.RoundToInt(heightCoef / (distCenter.magnitude + 0.00001f));
                        blob.Add(b);
                        blobFinished = false;
                    }
                    else
                    {
                        //output += "C";
                        blob[blob.Count - 1].area += 1;
                        blob[blob.Count - 1].right = i;
                        blob[blob.Count - 1].cx = (blob[blob.Count - 1].left + blob[blob.Count - 1].right) / 2;
                        Vector2 distCenter = (new Vector3(obj.point.x, obj.point.y) - transform.position);
                        int newTop = Mathf.RoundToInt(heightCoef / (distCenter.magnitude + 0.00001f));
                        if (blob[blob.Count - 1].top < newTop)
                            blob[blob.Count - 1].top = newTop;
                    }
                    Debug.DrawRay(transform.position, dist, bo.color, updateTime * 0.001f);
                    break;
                }
                else
                {
                    //Debug.DrawRay(transform.position, dist, Color.black, updateTime * 0.001f);
                    //output += "-";
                    blobFinished = true;
                    if (bo)
                        break;
                }
            }
        }
        //print(output);
        return blob.Count;
    }

    internal int getCamData(int color)
    { //0 - yellow, 1 - blue
        RobotTrackingCam trackingCam = this;
        //if (GameManager.Instance.millis() - camera_timer >= updateTime)
        //{
            camera_timer = GameManager.Instance.millis();
            int n = trackingCam.readBlobs(5);
            if (n == 0)
            {
                cam_angle = 360;
                return cam_angle;
            }
            TrackingCamBlobInfo_t maxBlob = null;
            if (n > 1)
            {
                for (int i = 0; i < n; i++)
                {
                    if (trackingCam.blob[i].color == needColors[color].color){
                        if (maxBlob == null || trackingCam.blob[i].area > maxBlob.area)
                            maxBlob = trackingCam.blob[i];
                    }
                }
            }
            else
            {
                if (blob.Count > 0 && trackingCam.blob[0].color == needColors[color].color)
                    maxBlob = trackingCam.blob[0];
            }
            if (maxBlob == null)
            {
                cam_angle = 360;
                right_angle = 360;
                left_angle = 360;
            }
            else
            {
                //if (maxBlob.type != color) return 0;
                cam_angle = Mathf.RoundToInt((maxBlob.cx - pixels / 2) * viewAngle / pixels);
                right_angle = Mathf.RoundToInt((maxBlob.right - pixels / 2) * viewAngle / pixels);
                left_angle = Mathf.RoundToInt((maxBlob.left - pixels / 2) * viewAngle / pixels);
                cam_height = maxBlob.top; // Mathf.RoundToInt(heightCoef / (dist.magnitude + 0.00001f));
            }

            return cam_angle;
        //}
    }

    internal int getCamHeight()
    {
        return cam_height;
    }
}
