using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroHealth : DefenseHealth
{
    protected override void Die()
    {
        Destroy(gameObject);
    }
}