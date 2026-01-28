using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TriggerAttackLoop : MonoBehaviour
{

 public float attackInterval = 1.2f;
 private Animator animator;
 private Coroutine loopCo;

 private void Awake()
 {
  animator = GetComponent<Animator>();

 }

 private void OnEnable()
 {
  loopCo = StartCoroutine(AttackLoop());
 }

 private IEnumerator AttackLoop()
 {
  yield return new WaitForSeconds(Random.Range(0f, 0.3f));

  while (true)
  {
   
   animator.SetTrigger("attack");
   yield return new WaitForSeconds(attackInterval);
  }
 }

 private void OnDisable()
 {
  if(loopCo!=null)StopCoroutine(loopCo);
 }
}
   