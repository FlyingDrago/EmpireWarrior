using UnityEngine;

public class CameraDragClamp : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public SpriteRenderer mapRenderer;

    private Vector3 lastMousePos;
    private float minX, maxX, minY, maxY;
    private bool panelClosed;

    void Start()
    {
        CalculateBounds();
    }

    void Update()
    {
        DragCamera();
    }

    void CalculateBounds()
    {
        Bounds bounds = mapRenderer.bounds;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        minX = bounds.min.x + camWidth;
        maxX = bounds.max.x - camWidth;
        minY = bounds.min.y + camHeight;
        maxY = bounds.max.y - camHeight;
    }

    void DragCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            panelClosed = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePos;

            // Đổi pixel → world unit
            float moveX = -mouseDelta.x * (cam.orthographicSize * 2f / Screen.height);
            float moveY = -mouseDelta.y * (cam.orthographicSize * 2f / Screen.height);

            if (!panelClosed && mouseDelta.sqrMagnitude > 1f)
            {
                panelClosed = true;
                TowerBuildManager.instance?.CloseBuildPanel();
            }

            Vector3 targetPos = cam.transform.position;
            targetPos.x += moveX;
            targetPos.y += moveY;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

            cam.transform.position = targetPos;

            lastMousePos = Input.mousePosition;
        }
    }
}