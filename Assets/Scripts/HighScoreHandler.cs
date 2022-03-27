using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Leaderboard{
    public class HighscoreHandler : MonoBehaviour {

        List<HighScoreElement> highscoreList = new List<HighScoreElement> ();
        [SerializeField] int maxCount = 9;
        [SerializeField] string filename;

        public delegate void OnHighscoreListChanged (List<HighScoreElement> list);
        public static event OnHighscoreListChanged onHighscoreListChanged;

        private void Start () {
            LoadHighscores ();
        }

        private void LoadHighscores () {
            highscoreList = FileHandler.ReadListFromJSON<HighscoreElement> (filename);

            while (highscoreList.Count > maxCount) {
                highscoreList.RemoveAt (maxCount);
            }

            if (onHighscoreListChanged != null) {
                onHighscoreListChanged.Invoke (highscoreList);
            }
        }

        private void SaveHighscore () {
            FileHandler.SaveToJSON<HighscoreElement> (highscoreList, filename);
        }

        public void AddHighscoreIfPossible (List<HighScoreElement> highscorelist) {

            for (int i = 0; i < maxCount; i++) {
                if (i >= highscoreList.Count || element.points > highscoreList[i].points) {
                    // add new high score
                    highscoreList.Insert (i, element);

                    while (highscoreList.Count > maxCount) {
                        highscoreList.RemoveAt (maxCount);
                    }

                    SaveHighscore ();

                    if (onHighscoreListChanged != null) {
                        onHighscoreListChanged.Invoke (highscoreList);
                    }

                    break;
                }
            }
        }

    }
}