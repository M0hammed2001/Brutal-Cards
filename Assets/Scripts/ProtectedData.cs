using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWNetwork;
using UnityEngine;

namespace BrutalCards
{
    /// <summary>
    /// Stores the important data of the game
    /// We will encypt the fields in a multiplayer game.
    /// </summary>
    [Serializable]
    public class ProtectedData
    {
        [SerializeField]
        List<byte> poolOfCards = new List<byte>();
        [SerializeField]
        List<byte> player1Cards = new List<byte>();
        [SerializeField]
        List<byte> player2Cards = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer1 = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer2 = new List<byte>();
        [SerializeField]
        string player1Id;
        [SerializeField]
        string player2Id;
        [SerializeField]
        string currentTurnPlayerId;
        [SerializeField]
        int currentGameState;
        [SerializeField]
        int selectedRank;
        
        [SerializeField]
        public int[] memoryGameArray;

        byte[] encryptionKey;
        byte[] safeData;

        public ProtectedData(string p1Id, string p2Id, string roomId)
        {
            player1Id = p1Id;
            player2Id = p2Id;
            currentTurnPlayerId = "";
            selectedRank = (int)Ranks.NoRanks;
            CalculateKey(roomId);
            
        }

        public void SetPoolOfCards(List<byte> cardValues)
        {
            
            poolOfCards = cardValues;
             
        }

        public List<byte> GetPoolOfCards()
        {
            List<byte> result;
             
            result = poolOfCards;
             
            return result;
        }

        public List<byte> PlayerCards(Player player)
        {
            List<byte> result;
             
            if (player.PlayerId.Equals(player1Id))
            {
                result = player1Cards;
            }
            else
            {
                result = player2Cards;
            }
             
            return result;
        }

        public List<byte> PlayerBooks(Player player)
        {
            List<byte> result;
             
            if (player.PlayerId.Equals(player1Id))
            {
                result = booksForPlayer1;
            }
            else
            {
                result = booksForPlayer2;
            }
             
            return result;
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
             
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.AddRange(cardValues);
                player1Cards.Sort();
            }
            else
            {
                player2Cards.AddRange(cardValues);
                player2Cards.Sort();
            }
             
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
             
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.Add(cardValue);
                player1Cards.Sort();
            }
            else
            {
                player2Cards.Add(cardValue);
                player2Cards.Sort();
            }
             
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
             
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else
            {
                player2Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
             
        }

        public void AddBooksForPlayer(Player player, Ranks ranks)
        {
             
            if (player.PlayerId.Equals(player1Id))
            {
                booksForPlayer1.Add((byte)ranks);
            }
            else
            {
                booksForPlayer2.Add((byte)ranks);
            }
             
        }

        public bool GameFinished()
        {
            bool result = false;
             
            if (poolOfCards.Count == 0)
            {
                result = true;
            }

            if (player1Cards.Count == 0)
            {
                result = true;
            }

            if (player2Cards.Count == 0)
            {
                result = true;
            }
             

            return result;
        }

        public string WinnerPlayerId()
        {
            string result;
             
            if (booksForPlayer1.Count > booksForPlayer2.Count)
            {
                result = player1Id;
            }
            else
            {
                result = player2Id;
            }
             
            return result;
        }

        public void SetCurrentTurnPlayerId(string playerId)
        {
             
            currentTurnPlayerId = playerId;
             
        }

        public string GetCurrentTurnPlayerId()
        {
            string result;
             
            result = currentTurnPlayerId;
             
            return result;
        }

        public void SetGameState(int gameState)
        {
             
            currentGameState = gameState;
             
        }
        public int GetGameState()
        {
            int result;
             
            result = currentGameState;
             
            return result;
        }

        public void SetSelectedRank(int rank)
        {
             
            selectedRank = rank;
             
        }

        public int GetSelectedRank()
        {
            int result;
             
            result = selectedRank;
             
            return result;
        }

        public Byte[] ToArray()
        {
            return safeData;
        }

        public void ApplyByteArray(Byte[] byteArray)
        {
            safeData = byteArray;
        }

        void CalculateKey(string roomId)
        {
            string roomIdSubString = roomId.Substring(0, 16);
            encryptionKey = Encoding.UTF8.GetBytes(roomIdSubString);
        }

    }
}