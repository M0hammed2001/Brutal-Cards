using System;

namespace Leaderboard{
    [Serializable]
    
    public class HighScoreElement {
        public string playerName;
        public int points;
        
        
        public HighScoreElement (string name, int points){
            playerName = MultiplayerGame.Instance.PlayerName;
            this.points = points;

        }
    }
}