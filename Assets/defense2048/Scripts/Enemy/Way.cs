using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Way : MonoBehaviour
{
   public List<Transform> points = new List<Transform>();

   private void Awake()
   {
      points.Clear();
      for (int i = 0; i < transform.childCount; i++)
      {
         points.Add(transform.GetChild(i));
      }
   }
}
