using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class SidewaysCamera : MonoBehaviour
{
    public static float CameraSpeed = .5f;
    public static float DefaultCameraZoom = 14f;
    public static float CameraBorders = 33f;
    public static float CameraSlide = .3f;
    public static float CameraSlideShort = .2f;

    public static int CameraZoom = 2;
    public static int CameraMinScale = 3;
    public static int CameraMaxScale = 10;

    public static SidewaysCamera active;

    Camera cam { get => gameObject.GetComponent<Camera>(); }
    public PixelPerfectCamera ppc { get => gameObject.GetComponent<PixelPerfectCamera>(); }
    
    public Vector2 CameraOrder = Vector2.zero;
    public Vector2 CameraOrigin = Vector2.zero;
    public Vector2 CameraTime = Vector2.zero;
    public Rect SoftBounds;
    public Rect HardBounds;
    public Rect AbsBounds;

    private void Awake()
    {
        CameraOrder = Vector2.zero;
        CameraOrigin = Vector2.zero;
        CameraTime = Vector2.one;
        active = this;
    }

    void ClearOrder()
    {
        CameraOrder = new Vector2(-1, -1);
        CameraTime = Vector2.zero;
    }

    public bool IsIdle()
    {
        return CameraTime.x == 0 && CameraTime.y == 0;
    }

    public void Update()
    {
        HandleCameraMovement();
    }
    public void MoveToTile(SidewaysTile tile)
    {
        MoveToTile(tile.gridPos.x, tile.gridPos.y);
    }

    public void MoveToTile(float x, float y)
    {
        IssueOrder(SidewaysMap.main.TranslateGridPosition(new Vector2Int((int)x,(int)y)) + new Vector2( 1 / 2f, - 1 / 2f));
    }
    public void HandleCameraMovement()
    {
        if (Input.mousePosition.x < CameraBorders)
        {
            Move(-1, 0);
        }
        if (Input.mousePosition.x > Screen.width - CameraBorders)
        {
            Move(1, 0);
        }
        if (Input.mousePosition.y < CameraBorders)
        {
            Move(0, -1);
        }
        if (Input.mousePosition.y > Screen.height - CameraBorders)
        {
            Move(0, 1);
        }
        if (Input.mousePosition.y < CameraBorders)
        {
            Move(0, -1);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            ZoomCamera(-1);
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            ZoomCamera(1);
        }
    }
    public void ZoomCamera(int dir)
    {
        ppc.assetsPPU = Mathf.Clamp(ppc.assetsPPU + dir, CameraMinScale, CameraMaxScale);
        if (IsIdle())
        {
            IssueOrder(transform.position,CameraSlideShort);
        }
    }

    public void Move(float X, float Y)
    {
        if (X != 0 || Y != 0)
        {
            if (CameraTime.x == 0)
            {
                IssueOrder(
                    new Vector2(
                    transform.position.x + X * CameraSpeed,
                    transform.position.y + Y * CameraSpeed),
                    CameraSlideShort
                );
            }
            else
            {
                IssueOrder(
                    new Vector2(
                    CameraOrder.x + X * CameraSpeed,
                    CameraOrder.y + Y * CameraSpeed),
                    CameraSlideShort
                );
            }
        }
    }

    public void FixedUpdate()
    {
        if (CameraTime.x + CameraTime.y > Time.time)
        {
            float delta = 1f / (CameraTime.x) * Time.deltaTime;
            if (CameraOrder.x != -1 && CameraOrder.y != 1)
            {
                cam.transform.position = new Vector3(
                    cam.transform.position.x + (CameraOrder.x - CameraOrigin.x) * delta,
                    cam.transform.position.y + (CameraOrder.y - CameraOrigin.y) * delta,
                    cam.transform.position.z);
            }
        }
        else
        {
            Snap();
        }
    }

    public void Snap()
    {
        if (CameraOrder.x != -1 && CameraOrder.y != 1)
        {
            cam.transform.position = new Vector3(CameraOrder.x, CameraOrder.y, cam.transform.position.z);
        }
        ClearOrder();
    }

    public void IssueOrder(Vector2 position)
    {
        IssueOrder(position, CameraSlide);
    }
    public void IssueOrder(Vector2 position, float time)
    {
        //Debug.Log("[entityCamera] Move camera at " + position + " over " + time);
        CameraOrigin = new Vector3(transform.position.x, transform.position.y, cam.orthographicSize);
        CameraOrder = position;
        CameraTime = new Vector2(time, Time.time);

        float H = cam.orthographicSize;
        float W = H * cam.aspect;

        if (W * 2 >= AbsBounds.width )
        {
            CameraOrder.x = AbsBounds.center.x;
        }
        else
        {
            CameraOrder.x = Mathf.Clamp(CameraOrder.x, AbsBounds.xMin + W, AbsBounds.xMax - W);
        }

        if (H * 2 >= AbsBounds.height )
        {
            CameraOrder.y = AbsBounds.center.y;
        }
        else
        {
            CameraOrder.y = Mathf.Clamp(CameraOrder.y, AbsBounds.yMin + H, AbsBounds.yMax - H);
        }

        if (time < 0)
        { Snap(); }
    }

    /*public void UpdateCameraBounds(Vector2 min, Vector2 max)
    {
        Vector2 dims = (max - min);
        CameraBounds = new Rect(-dims.x * .5f, -dims.y * .5f, dims.x, dims.y);

        Debug.Log("[entityCamera] Update Camera Bounds " + CameraBounds);
    }*/

    public void SetSoftCameraBounds(Vector2 min, Vector2 max)
    {
        SoftBounds = new Rect(min, max - min);
        Debug.Log("[entityCamera] Update Camera Soft Bounds " + SoftBounds);
        UpdateAbsBounds();
    }

    public void SetHardCameraBounds(Vector2 min, Vector2 max)
    {
        HardBounds = new Rect(min, max - min);
        Debug.Log("[entityCamera] Update Camera Hard Bounds " + HardBounds);
        UpdateAbsBounds();
    }

    void UpdateAbsBounds()
    {
        AbsBounds.xMin = Mathf.Max(HardBounds.xMin, SoftBounds.xMin) ;
        AbsBounds.xMax = Mathf.Min(HardBounds.xMax, SoftBounds.xMax) ;
        AbsBounds.yMin = Mathf.Max(HardBounds.yMin, SoftBounds.yMin) ;
        AbsBounds.yMax = Mathf.Min(HardBounds.yMax, SoftBounds.yMax) ;
        Debug.Log("[entityCamera] Update Camera Abs Bounds " + AbsBounds);
    }
}