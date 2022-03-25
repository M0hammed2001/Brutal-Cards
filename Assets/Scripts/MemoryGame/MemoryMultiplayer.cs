using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.SceneManagement;

namespace BrutalCards
{
    public class MemoryMultiplayer : MemoryGame
    {


        NetCode netCode;
        GameDataManager gameDataManager;

        SceneController sceneController;





        protected new void Awake()
        {
            base.Awake();
            
            remotePlayer.IsAI = false;

            netCode = FindObjectOfType<NetCode>();

            NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
            {
                if (successful)
                {
                    foreach(SWPlayer swPlayer in reply.players)
                    {
                        string playerName = swPlayer.GetCustomDataString();
                        string playerId = swPlayer.id;

                        if (playerId.Equals(NetworkClient.Instance.PlayerId))
                        {
                            localPlayer.PlayerId = playerId;
                            localPlayer.PlayerName = playerName;
                        }
                        else
                        {
                            remotePlayer.PlayerId = playerId;
                            remotePlayer.PlayerName = playerName;
                        }
                    }

                    gameDataManager = new GameDataManager(localPlayer, remotePlayer, NetworkClient.Lobby.RoomId);
                    netCode.EnableRoomPropertyAgent();
                }
                else
                {
                    Debug.Log("Failed to get players in room.");
                }

            });
        }
        protected new void Start()
        {
            Debug.Log("Multiplayer Game Start");
            
        }

        //****************** Game Flow *********************//

        public override void GameFlow()
        {
            Debug.LogError("Should never be here");
        }

        protected override void OnGameStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                gameState = GameState.TurnStarted;

                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }

        }
        
        protected override void OnTurnStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                sceneController.SwitchTurn();
                gameState = GameState.TurnSelectingCards;

                gameDataManager.SetCurrentTurnPlayer(sceneController.currentTurnPlayer);
                SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            }
        }

        protected override void OnTurnSelectingCards()
        {
            if (sceneController.currentTurnPlayer == localPlayer)
            {

            }
            else
            {

            }

            if (NetworkClient.Instance.IsHost)
            {
                gameState = GameState.CheckingPairs;
                SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            }
        }

        protected override void OnCheckingPairs()
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

            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }

//*********************************************************************

        public void OnGameDataReady(EncryptedData encryptedData)
        {
            if(encryptedData == null)
            {
                Debug.Log("New game");
                if (NetworkClient.Instance.IsHost)
                {
                    gameState = GameState.GameStarted;
                    SetGameState(gameState);

                    netCode.ModifyGameData(gameDataManager.EncryptedData());

                    netCode.NotifyOtherPlayersGameStateChanged();
                }
            }
            else
            {
                gameDataManager.ApplyEncrptedData(encryptedData);
                gameState = GetGameState();
                sceneController.currentTurnPlayer = gameDataManager.GetCurrentTurnPlayer();

                if(gameState > GameState.GameStarted)
                {
                    Debug.Log("Restore the game state");

                    base.GameFlow();
                }
            }
        }

        public void OnGameDataChanged(EncryptedData encryptedData)
        {
            gameDataManager.ApplyEncrptedData(encryptedData);
            gameState = GetGameState();
            sceneController.currentTurnPlayer = gameDataManager.GetCurrentTurnPlayer();
        }

        public void OnGameStateChanged()
        {
            base.GameFlow();
        }

        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

    }

}