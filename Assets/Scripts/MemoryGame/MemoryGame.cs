using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using Unity;

namespace BrutalCards
{
    public class MemoryGame : MonoBehaviour
    {
        SceneController sceneController;

        public Player localPlayer;
        public Player remotePlayer;
        public float x;
        
        public enum GameState
        {
            Idle,
            GameStarted,
            TurnStarted,
            TurnSelectingCards,
            CheckingPairs,
            GameFinished
        };

        protected void Awake(){

        }

        
        protected void Start(){
            
            gameState = GameState.GameStarted;
            GameFlow();
        }

        [SerializeField]
        protected GameState gameState = GameState.Idle;
    

        public virtual void GameFlow(){
            if (gameState > GameState.GameStarted)
            {
                x = 1;
                if (x != 1)
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        OnGameStarted();
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnturnStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnturnStarted();
                        break;
                    }
                case GameState.TurnSelectingCards:
                    {
                        Debug.Log("TurnSelectingCards");
                        OnTurnSelectingCards();
                        break;
                    }
                case GameState.CheckingPairs:
                    {
                        Debug.Log("CheckingPairs");
                        OnCheckingPairs();
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        protected virtual void OnGameStarted()
        {

            gameState = GameState.TurnStarted;
        }

        protected void OnturnStarted()
        {
            sceneController.SwitchTurn();
            gameState = GameState.TurnSelectingCards;
        }

        protected void OnTurnSelectingCards()
        {
            if (sceneController.currentTurnPlayer == sceneController.localPlayer)
            {

            }
            if (sceneController.currentTurnPlayer.IsAI)
            {

            }
        }

        protected void OnCheckingPairs()
        {
            if( sceneController.CheckingMatch() == true)
            {
                gameState = GameState.TurnSelectingCards;
            }
            else
            {
                sceneController.SwitchTurn();
                gameState = GameState.TurnSelectingCards;
            }
        }

        protected void OnGameFinished()
        {
            if(sceneController.bot_score < sceneController.player_score)
            {
                
            }
            else if(sceneController.player_score < sceneController.bot_score)
            {

            }
            else
            {
                
            }
        }
        
    }
    
}
