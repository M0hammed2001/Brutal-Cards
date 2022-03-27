using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leaderboard{
    public class GameHandler : MonoBehaviour {
        [SerializeField] PointCounter pointCounter;
        [SerializeField] HighscoreHandler highscoreHandler;
        [SerializeField] PointHUD pointHUD;
        [SerializeField] string playerName;

        public void StartGame () {
            pointCounter.StartGame ();
        }
        public void StopGame () {
            highscoreHandler.AddHighscoreIfPossible (new HighscoreElement (playerName, pointHUD.Points));
            pointCounter.StopGame ();
        }
    }
}