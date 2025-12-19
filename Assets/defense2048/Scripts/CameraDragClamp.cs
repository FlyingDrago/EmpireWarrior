using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragClamp : MonoBehaviour
{
  [Header("Refences")] public SpriteRenderer mapRenderer;
  public Camera cam;

  private Vector3 _dragOrigin;
  private float minX, maxX, minY, maxY;

  private void Start()
  {
    CalcalateBounds();
  }

  private void Update()
  {
    DragCamera();
  }

  void CalcalateBounds()
  {
    Bounds mapBounds = mapRenderer.bounds;

    float camHeight = cam.orthographicSize;
    float camWidth = camHeight * cam.aspect;

    minX = mapBounds.min.x + camWidth;
    maxX = mapBounds.max.x - camWidth;
    minY = mapBounds.min.y + camHeight;
    maxY = mapBounds.max.y - camHeight;
  }

  void DragCamera()
  {
    if (Input.GetMouseButtonDown(0))
    {
      _dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    if (Input.GetMouseButton(0))
    {
      Vector3 difference = _dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

      Vector3 targetPos = cam.transform.position + difference;

      targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
      targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

      cam.transform.position = new Vector3(targetPos.x, targetPos.y, cam.transform.position.z);
    }
  }
}
