using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEngine.UI;


namespace BrutalCards
{
    public class Game : MonoBehaviour
    {

         public AudioSource audioSource;
        public AudioClip pick, collect;
        
        public Text MessageText;
        public Text DrawnCard;
        public Text RemoteName;
        public Text LocalName;

        Card cardConfirm = null;

        public GameObject PopoverBackground;
        public GameObject OptionsPopover;
        public GameObject deadPopover;
        public GameObject winnerPopover;
        public GameObject RulesPopover;

        protected CardAnimator cardAnimator;

        [SerializeField]
        protected GameDataManager gameDataManager;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> BookPositions = new List<Transform>();

        [SerializeField]
        protected Player localPlayer;
        [SerializeField]
        protected Player remotePlayer;

        [SerializeField]
        protected Player currentTurnPlayer;
        [SerializeField]
        protected Player currentTurnTargetPlayer;

        [SerializeField]
        protected Card selectedCard;
        [SerializeField]
        protected Ranks selectedRank;
        protected Suits selectedSuit;

        public enum GameState
        {
            Idle,
            GameStarted,
            TurnStarted,
            TurnSelectingNumber,
            TurnConfirmedSelectedNumber,
            TurnWaitingForOpponentConfirmation,
            TurnOpponentConfirmed,
            TurnBrutalFish,
            GameFinished
        };

        [SerializeField]
        protected GameState gameState = GameState.Idle;

        protected void Awake(){
            Debug.Log("base awake");
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";
            localPlayer.Position = PlayerPositions[0].position;
            localPlayer.BookPosition = BookPositions[0].position;

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.Position = PlayerPositions[1].position;
            remotePlayer.BookPosition = BookPositions[1].position;
            remotePlayer.IsAI = true;

            cardAnimator = FindObjectOfType<CardAnimator>();
        }

        protected void Start(){
            
            gameState = GameState.GameStarted;
            GameFlow();
        }

        //****************** Game Flow *********************//
        public virtual void GameFlow(){
            if (gameState > GameState.GameStarted)
            {
                CheckPlayersBooks();
                ShowAndHidePlayersDisplayingCards();

                if (gameDataManager.GameFinished())
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
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
                case GameState.TurnSelectingNumber:
                    {
                        Debug.Log("TurnSelectingNumber");
                        OnTurnSelectingNumber();
                        break;
                    }
                case GameState.TurnConfirmedSelectedNumber:
                    {
                        Debug.Log("TurnComfirmedSelectedNumber");
                        OnTurnConfirmedSelectedNumber();
                        break;
                    }
                case GameState.TurnWaitingForOpponentConfirmation:
                    {
                        Debug.Log("TurnWaitingForOpponentConfirmation");
                        OnTurnWaitingForOpponentConfirmation();
                        break;
                    }
                case GameState.TurnOpponentConfirmed:
                    {
                        Debug.Log("TurnOpponentConfirmed");
                        OnTurnOpponentConfirmed();
                        break;
                    }
                case GameState.TurnBrutalFish:
                    {
                        Debug.Log("TurnBrutalFish");
                        OnTurnBrutalFish();
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

        protected virtual void OnGameStarted(){
            
            gameDataManager = new GameDataManager(localPlayer, remotePlayer);
            SetLocal(localPlayer.PlayerName);
            SetRemote(remotePlayer.PlayerName);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DrawCardValue();

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            gameState = GameState.TurnStarted;
        }

        protected virtual void OnTurnStarted(){
            SwitchTurn();
            gameState = GameState.TurnSelectingNumber;
            GameFlow();
        }

        public void OnTurnSelectingNumber(){
            ResetSelectedCard();

            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn. Pick a card from your hand.");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn");
            }

            if (currentTurnPlayer.IsAI)
            {
                selectedRank = gameDataManager.SelectRandomRanksFromPlayersCardValues(currentTurnPlayer);
                gameState = GameState.TurnConfirmedSelectedNumber;
                GameFlow();
            }
        }

        protected virtual void OnTurnConfirmedSelectedNumber(){
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Asking {currentTurnTargetPlayer.PlayerName} for {selectedRank}s...");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName} is asking for {selectedRank}s...");
            }

            gameState = GameState.TurnWaitingForOpponentConfirmation;
            GameFlow();
        }

        public void OnTurnWaitingForOpponentConfirmation(){
            if (currentTurnTargetPlayer.IsAI)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }
        protected virtual void OnTurnOpponentConfirmed(){
            List<byte> cardValuesFromTargetPlayer = gameDataManager.TakeCardValuesWithRankFromPlayer(currentTurnTargetPlayer, selectedRank);
            
            if (cardValuesFromTargetPlayer.Count > 0)
            {
                audioSource.PlayOneShot(pick, 1f);
                gameDataManager.AddCardValuesToPlayer(currentTurnPlayer, cardValuesFromTargetPlayer);

                bool senderIsLocalPlayer = currentTurnTargetPlayer == localPlayer;
                currentTurnTargetPlayer.SendDisplayingCardToPlayer(currentTurnPlayer, cardAnimator, cardValuesFromTargetPlayer, senderIsLocalPlayer);
                gameState = GameState.TurnSelectingNumber;
            }
            else
            {
                gameState = GameState.TurnBrutalFish;
                GameFlow();
            }
        }

        protected virtual void OnTurnBrutalFish(){
            SetMessage($"Brutal fish!");

            byte cardValue = gameDataManager.DrawCardValue();

            if (cardValue == Constants.POOL_IS_EMPTY)
            {
                Debug.LogError("Pool is empty");
                return;
            }

            if (Card.GetRank(cardValue) == selectedRank)
            {
                audioSource.PlayOneShot(pick, 1f);
                cardAnimator.DrawDisplayingCard(currentTurnPlayer, cardValue);
            }
            else
            {
                audioSource.PlayOneShot(pick, 1f);
                cardAnimator.DrawDisplayingCard(currentTurnPlayer);
                gameState = GameState.TurnStarted;
            }
            if (currentTurnPlayer == localPlayer) {
                SetDrawnCard($"You just drew the " + Card.GetRank(cardValue).ToString() + $" of " + Card.GetSuit(cardValue).ToString());
            }

            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
        }

        public void OnGameFinished(){
            if (gameDataManager.Winner() == localPlayer)
            {
                SetMessage(winnerPopover.SetActive(true); );
);
            }
            else
            {
                SetMessage(deadPopover.SetActive(true); );
);
            }
        }
        //****************** Helper Methods *********************//

        public void ResetSelectedCard(){
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
                
                selectedRank = 0;
            }
        }

        protected void SetMessage(string message){
            MessageText.text = message;
        }

        protected void SetDrawnCard(string message)
        {
            DrawnCard.text = message;
        }
        protected void SetLocal(string message)
        {
            LocalName.text = message;
        }
        protected void SetRemote(string message)
        {
            RemoteName.text = message;
        }

        public void SwitchTurn(){
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
                return;
            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
                currentTurnTargetPlayer = localPlayer;
            }
            else
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
            }
        }

        public void PlayerShowBooksIfNecessary(Player player){
            Dictionary<Ranks, List<byte>> books = gameDataManager.GetBooks(player);

            if (books != null)
            {
                audioSource.PlayOneShot(collect, 1f);
                foreach (var book in books)
                {
                    player.ReceiveBook(book.Key, cardAnimator);

                    gameDataManager.RemoveCardValuesFromPlayer(player, book.Value);
                    gameDataManager.AddBooksForPlayer(player, book.Key);
                }
            }
        }

        public void CheckPlayersBooks(){
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(localPlayer);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer);
            remotePlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(remotePlayer);
        }

        public void ShowAndHidePlayersDisplayingCards(){
            localPlayer.ShowCardValues();
            remotePlayer.HideCardValues();
        }

        //****************** User Interaction *********************//
        public void OnOptionsClicked(){
            Debug.Log("OnOptionsClicked");
            PopoverBackground.SetActive(true);
            OptionsPopover.SetActive(true); 
        }
        

        public void OnRulesClicked()
        {
            Debug.Log("OnRulesClicked");
            OptionsPopover.SetActive(false);
            RulesPopover.SetActive(true);
        }
        
        void OnGUI(){
            if (Input.GetKeyDown("escape")){
                Debug.Log("KeyCode down: escape");
                OnOptionsClicked();
            }   
        }

        public void OnCancelClicked(){
            PopoverBackground.SetActive(false);
            OptionsPopover.SetActive(false);

        }

        public void OnRulesCancelClicked(){
            OptionsPopover.SetActive(true);
            RulesPopover.SetActive(false);
        }


        public void OnCardSelected(Card card){   

            if (gameState == GameState.TurnSelectingNumber)
            {
                if (card.OwnerId == currentTurnPlayer.PlayerId)
                {
                    if (selectedCard != null)
                    {
                        audioSource.PlayOneShot(pick, 1f);
                        selectedCard.OnSelected(false);
                        selectedRank = 0;
                    }
                    
                    if (cardConfirm == card){
                        OnOkSelected();
                        cardConfirm = null;
                    }

                    selectedCard = card;
                    selectedSuit = selectedCard.Suit;
                    selectedRank = selectedCard.Rank;
                    selectedCard.OnSelected(true);
                    SetMessage($"Ask {currentTurnTargetPlayer.PlayerName} for {selectedCard.Rank}s ?");
                    cardConfirm = card;
                }
            }
        }

        public virtual void OnOkSelected(){
            if (gameState == GameState.TurnSelectingNumber && localPlayer == currentTurnPlayer)
            {
                if (selectedCard != null)
                {
                    gameState = GameState.TurnConfirmedSelectedNumber;
                    GameFlow();
                }
            }
            else if (gameState == GameState.TurnWaitingForOpponentConfirmation && localPlayer == currentTurnTargetPlayer)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }
        


        //****************** Animator Event *********************//
        public virtual void AllAnimationsFinished(){
            GameFlow();
        }
    }
}
