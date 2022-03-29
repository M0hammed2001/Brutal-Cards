
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
        [SerializeField]
        public SceneController sceneController;



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

        [SerializeField]
        protected GameState gameState = GameState.Idle;

        protected void Awake()
        {

        }

        
        protected void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();

        }

    

        public virtual void GameFlow(){
            if (gameState > GameState.GameStarted)
            {
                if (sceneController.bot_score >= 7)
                {
                    gameState = GameState.GameFinished;
                }
                else if(sceneController.player_score >= 7)
                {
                    gameState = GameState.GameFinished;
                }
            }
            else
            {
                gameState = GameState.GameStarted;
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
            GameFlow();
            
        }

        protected virtual void OnTurnStarted()
        {
            sceneController.SwitchTurn();
            gameState = GameState.TurnSelectingCards;
            GameFlow();
        }

        protected virtual void OnTurnSelectingCards()
        {
            Debug.Log("this is called" + sceneController.localPlayer);
            if (sceneController.currentTurnPlayer == sceneController.localPlayer )
            {
                
                gameState = GameState.CheckingPairs;
                Debug.Log("this flows 1"  + gameState);
            }
            else
            {  
                sceneController.AiCardpick();
                gameState = GameState.CheckingPairs;
                GameFlow();
            }
        }
        

        protected virtual void OnCheckingPairs()
        {
            if( sceneController.checkingMatch == true)
            {
                gameState = GameState.TurnStarted;
            }
            else
            {
                gameState = GameState.TurnStarted;
                sceneController.SwitchTurn();    
                Debug.Log("this flows 2" + gameState);
            }
            GameFlow();
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

        public virtual void OnOkSelected()
        {
            GameFlow();
        }


        
        
    }
    
}
