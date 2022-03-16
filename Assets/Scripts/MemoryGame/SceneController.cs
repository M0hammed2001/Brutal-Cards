using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public const int gridRows = 3;
    public const int gridCols = 8;
    public const float offsetX = 2.3f;
    public const float offsetY = 3.3f;
    public float x;

    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;

    public enum GameState
    {
        Idle,
        GameStarted,
        TurnStarted,
        TurnSelectingCards,
        CheckingPairs,
        OpponentsTurn,
        OpponentsCheckingPair,
        GameFinished
    };

    [SerializeField]
    protected GameState gameState = GameState.Idle;

    private void Start()
    {
        Vector3 startPos = originalCard.transform.position; //The position of the first card. All other cards are offset from here.

        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11};
        numbers = ShuffleArray(numbers); //This is a function we will create in a minute!

        for(int i = 0; i < gridCols; i++)
        {
            for(int j = 0; j < gridRows; j++)
            {
                MemoryCard card;
                if(i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard) as MemoryCard;
                }

                int index = j * gridCols + i;
                int id = numbers[index];
                card.ChangeSprite(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = (offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    public virtual void GameFlow(){
        if (gameState > GameState.GameStarted)
        {
            x = 1;
            if (x == 1)
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
                    break;
                }
            case GameState.TurnStarted:
                {
                    Debug.Log("TurnStarted");
                    break;
                }
            case GameState.TurnSelectingCards:
                {
                    Debug.Log("TurnSelectingNumber");
                    break;
                }
            case GameState.CheckingPairs:
                {
                    Debug.Log("TurnComfirmedSelectedNumber");
                    break;
                }
            case GameState.OpponentsTurn:
                {
                    Debug.Log("TurnWaitingForOpponentConfirmation");
                    break;
                }
            case GameState.OpponentsCheckingPair:
                {
                    Debug.Log("TurnOpponentConfirmed");
                    break;
                }
            case GameState.GameFinished:
                {
                    Debug.Log("GameFinished");
                    break;
                }
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for(int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------

    private MemoryCard _firstRevealed;
    private MemoryCard _secondRevealed;

    private int _score = 0;
    [SerializeField] private TextMesh scoreLabel;

    public bool canReveal
    {
        get { return _secondRevealed == null; }
    }

    public void CardRevealed(MemoryCard card)
    {
        if(_firstRevealed == null)
        {
            _firstRevealed = card;
        }
        else
        {
            _secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if(_firstRevealed.id == _secondRevealed.id)
        {
            _score++;
            scoreLabel.text = "Score: " + _score;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
        }

        _firstRevealed = null;
        _secondRevealed = null;

    }

    // public void Restart()
    // {
    //     SceneManager.LoadScene("Scene_001");
    // }

}