using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
  public float speed = 2f;

  public List<Transform> pathPoints;
  public int _currentIndex;

  public EnemyAnimation enemyAnim;
  



  private void Awake()
  {
    enemyAnim = GetComponent<EnemyAnimation>();
   
  }
  public void Init(EnemyType data, List<Transform> path)
  {
    speed = data.speed;
    SetPath(path);
  
  }
  public void SetPath(List<Transform> points)
  {
    pathPoints = points;
    _currentIndex = 0;
    transform.position = pathPoints[0].position;

    gameObject.SetActive(true);
  }

  void Update()
  {
 
    if (pathPoints == null || _currentIndex >= pathPoints.Count)
      return;
    
    

    Transform point = pathPoints[_currentIndex];
    Vector3 target = point.position;

    transform.position = Vector3.MoveTowards(
      transform.position,
      target,
      speed * Time.deltaTime
    );

    if (Vector3.Distance(transform.position, target) < 0.05f)
    {
     
      _currentIndex++;

      if (_currentIndex >= pathPoints.Count)
      {
        ReachEnd();
      }
    }
  }







  void ReachEnd()
  {
    EnemyPool.Instance.ReturnEnemy(gameObject);
  }

  

}
