using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.SceneManagement;

namespace BrutalCards
{
    public class MemoryCardMultiplayer : MemoryGame
    {


        NetCode netCode;
        GameDataManager gameDataManager;





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
    }

}