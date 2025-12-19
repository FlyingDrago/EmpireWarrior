using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Level Data")]
public class LevelData : ScriptableObject
{
   public int LevelIndex;
   public List<WaveData> waves;
}
