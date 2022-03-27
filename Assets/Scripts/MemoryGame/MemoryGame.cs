
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

        protected void Awake()
        {
            
        }

        
        protected void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();
        }

        [SerializeField]
        protected GameState gameState = GameState.Idle;
    

        public virtual void GameFlow(){
            if (gameState > GameState.GameStarted)
            {
                x = 1;
                if ( sceneController.bot_score >= 7)
                {
                    gameState = GameState.GameFinished;
                }
                else if(sceneController.player_score >= 7)
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
                        OnGameStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
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

        protected virtual void OnTurnStarted()
        {
            sceneController.SwitchTurn();
            gameState = GameState.TurnSelectingCards;
            GameFlow();
        }

        protected virtual void OnTurnSelectingCards()
        {
            if (sceneController.currentTurnPlayer == sceneController.localPlayer)
            {
                gameState = GameState.CheckingPairs;
            }
            if (sceneController.currentTurnPlayer.IsAI)
            {
                
                sceneController.AiCardpick();
                gameState = GameState.CheckingPairs;
            }
        }

        protected virtual void OnCheckingPairs()
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

        protected virtual void OnGameFinished()
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

        public MemoryGame.GameState GetGameState()
        {
            return gameState;
        }

        public void SetGameState(MemoryGame.GameState gameStated)
        {
            gameState = gameStated;
        }

        
    }
    
}
