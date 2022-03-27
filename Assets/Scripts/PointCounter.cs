using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Leaderboard{
    public class PointCounter : MonoBehaviour {
        [SerializeField] PointHUD pointHUD;
        public string Winner(){
                string winnerPlayerId = publicData.WinnerPlayerId();
                if (winnerPlayerId.Equals(MultiplayerGame.Instance.playerName))
                {
                    playerName.pointHUD++;
                    return playerName.pointHUD;
                }
        }
    }
}