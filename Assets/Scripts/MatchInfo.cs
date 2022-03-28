using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Leaderboard{
public class MatchInfo : MonoBehaviour
{
    public void AddToScore(string playerName, int score){
      string playerName = protectedData.WinnerPlayerId();
            if (winnerPlayerId.Equals(localPlayer.PlayerName))
            {
                return localPlayer.score++;
            }
            else
            {
                return remotePlayer.score++;
            }
}
}
}