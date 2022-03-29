using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrutalCards{

[System.Serializable] public class ScoreboardItem : MonoBehaviour
{
  [SerializeField] public string playerName = string.Empty;
  [SerializeField] public int score = 0;

  public void SetData(MatchInfo info)
  {
    playerName = info.PlayerName;
    score = info.Score;

  }
}
}
