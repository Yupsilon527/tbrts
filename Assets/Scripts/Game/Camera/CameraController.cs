using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    CameraBounds currentBounds;

    public float cameraScreenBorders = .2f;
    public float cameraScreenDragSpeed = .2f;
    public float cameraScreenZoomSpeed = .2f;

    Vector2 cameraSize = Vector2.one;
    public float borderbounds = .2f;
    public float defaultCameraScale = 16;
    public float cameraBound = .2f;
    float desiredSize = 3;

    public float zoomBackTime = 2f;
    public static CameraController main;

    private void Awake()
    {
        main = this;
        if (camera == null)
            camera = GetComponent<Camera>();
        Readjust();
    }
    private void Update()
    {
        RefitSize();
        HandleCameraMovement();
    }
    public void HandleCameraMovement()
    {
        if (Input.mousePosition.x < cameraScreenBorders)
        {
            MoveDirection(Vector2.left, cameraScreenDragSpeed*Time.deltaTime);
        }
        if (Input.mousePosition.x > Screen.width - cameraScreenBorders)
        {
            MoveDirection(Vector2.right, cameraScreenDragSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y < cameraScreenBorders)
        {
            MoveDirection(Vector2.down, cameraScreenDragSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y > Screen.height - cameraScreenBorders)
        {
            MoveDirection(Vector2.up, cameraScreenDragSpeed * Time.deltaTime);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            desiredSize = Mathf.Clamp(desiredSize - Input.GetAxis("Mouse ScrollWheel")  * cameraScreenZoomSpeed, defaultCameraScale, currentBounds.maxScale);
        }
    }
    public void SetBounds(CameraBounds newbounds)
    {
        currentBounds = newbounds;
        camera.orthographicSize = currentBounds.maxScale;
        MovePosition(newbounds.transform.position);
    }

    public void JumptoMob(DataItemObject followChar)
    {
        if (followChar == null) return;

        MovePosition(followChar.GetOccupiedTiles()[0].GetWorldPosition());
    }
    public Vector2 PointInsideBorders(Vector3 point)
    {
        Vector2 dir = Vector2.zero;
        if (point.x < transform.position.x - camera.orthographicSize * camera.aspect * cameraBound)
        {
            dir.x = -1;
        }
        else if (point.x > transform.position.x + camera.orthographicSize * camera.aspect * cameraBound)
        {
            dir .x= 1;
        }

        if (point.y < transform.position.y - camera.orthographicSize * cameraBound)
        {
            dir.y = -1;
        }
        else if (point.y > transform.position.y + camera.orthographicSize * cameraBound)
        {
            dir .y= 1;
        }

        return dir;
    }
    public void MoveDirection(Vector2 direction, float speed = 1)
    {
        MovePosition((Vector2)transform.position + direction * speed);
    }
    public void CenterOnTile(Vector2Int title)
    {
        MovePosition(SidewaysMap.main.TranslateEntityPosition(title));
    }
    public void CenterOnGameObject(GameObject gob)
    {
        MovePosition(gob.transform.position);
    }
    public void MovePosition(Vector2 center)
    {
        if (camera == null) return;
        if (currentBounds == null)
        {
            center = Vector2.zero;
        }
        else
        {
            if (cameraSize.x * 2 >= currentBounds.rBounds.width)
            {
                center.x = currentBounds.rBounds.center.x;
            }
            else
            {
                center.x = Mathf.Clamp(center.x, currentBounds.rBounds.xMin + cameraSize.x * borderbounds, currentBounds.rBounds.xMax - cameraSize.x * borderbounds);
            }
            if (cameraSize.y * 2 >= currentBounds.rBounds.height)
            {
                center.y = currentBounds.rBounds.center.y;
            }
            else
            {
                center.y = Mathf.Clamp(center.y, currentBounds.rBounds.yMin + cameraSize.y * borderbounds, currentBounds.rBounds.yMax - cameraSize.y * borderbounds);
            }
        }
        transform.position = center;
    }
    void Readjust()
    {
        if (camera == null) return;
        cameraSize.y = camera.orthographicSize;
        cameraSize.x = cameraSize.y * camera.aspect;
    }

    public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        return camera.ScreenToWorldPoint(screenPoint);
    }
    public DataItemObject MobFromWorldPoint(Vector2 worldPoint, bool playerOwned = false)
    {
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(worldPoint, .1f, Vector2.zero))
        {
            if (hit.transform.TryGetComponent(out DataItemObject target))
            {
                if (!playerOwned || target.GetPlayerOwner().isPlayer())
                    return target;
            }
        }
        return null;
    }
    public DataItemObject MobFromScreenPoint(Vector2 screenPoint)
    {
        return MobFromWorldPoint(ScreenToWorldPoint(screenPoint));
    }
    /* public DataItemObject MobFromWorldPointForAbility(Vector2 worldPoint, PropertyAbility ability)
     {
         foreach (RaycastHit2D hit in Physics2D.CircleCastAll(worldPoint, .1f, Vector2.zero))
         {
             if (hit.transform.TryGetComponent(out DataItemObject target))
             {
                 if (ability.CanCastOnTarget(target))
                     return target;
             }
         }
         return null;
     }
     public DataItemObject MobFromScreenPointForAbility(Vector2 screenPoint, PropertyAbility ability)
     {
         return MobFromWorldPointForAbility(ScreenToWorldPoint(screenPoint), ability);
     }*/
    public void MoveCameraWithBorders(Vector2 screenPoint, float moveSpeed)
    {
        float screenBorders = Mathf.Min(Screen.width * .2f, Screen.height * .2f);
        Vector2 speed = Vector2.zero;
        if (screenPoint.x < screenBorders)
        {
            speed.x = 1;
        }
        else if (screenPoint.x > Screen.width - screenBorders)
        {
            speed.x = -1;
        }
        if (screenPoint.y < screenBorders)
        {
            speed.y = 1;
        }
        else if (screenPoint.y > Screen.height - screenBorders)
        {
            speed.y = -1;
        }
        MoveDirection(speed * moveSpeed);
    }
    public void Zoom(float delta, bool instant)
    {
        if (instant)
            ChangeScale(camera.orthographicSize + delta);
        else
            desiredSize = delta;
    }
    void ChangeScale(float scale)
    {
        if (currentBounds != null)
        {
            camera.orthographicSize = Mathf.Clamp(scale, defaultCameraScale, currentBounds.maxScale);
            Readjust();
            MoveDirection(Vector2.zero);
        }
    }
    void RefitSize()
    {
        if (camera.orthographicSize != desiredSize)
        {
            ChangeScale(camera.orthographicSize + (desiredSize - camera.orthographicSize) / zoomBackTime * Time.deltaTime);
            Readjust();
        }
    }
}
