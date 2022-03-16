using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D;

namespace BrutalCards
{
    /// <summary>
    /// SetFaceUp(false) clears card's face value
    /// To display a card's value, call SetCardValue(byte) to assign the Rank and the Suit to the card, then call SetFaceUp(true)
    /// </summary>
    public class Card : MonoBehaviour
    {
        public SceneController sc;
        public GameObject Memory_Card_Back;
        public static Ranks GetRank(byte value){
            return (Ranks)(value / 4 + 1);
        }

        public static Suits GetSuit(byte value){
            return (Suits)(value % 4);
        }
        public SpriteAtlas Atlas;

        public Suits Suit = Suits.NoSuits;
        public Ranks Rank = Ranks.NoRanks;

        public string OwnerId;

        SpriteRenderer spriteRenderer;

        bool faceUp = false;
        void Awake(){
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start(){
            UpdateSprite();
        }

        public void SetFaceUp(bool value){
            faceUp = value;
            UpdateSprite();

            // Setting faceup to false also resets card's value.
            if (value == false)
            {
                Rank = Ranks.NoRanks;
                Suit = Suits.NoSuits;
            }
        }

        public void SetCardsvalue(byte value){
            Rank = (Ranks)(value / 4 + 1);


            Suit = (Suits)(value % 4);
        }

        void UpdateSprite(){
            if (faceUp)
            {
                spriteRenderer.sprite = Atlas.GetSprite(SpriteName());
            }
            else
            {
                spriteRenderer.sprite = Atlas.GetSprite(Constants.CARD_BACK_SPRITE);
            }
        }

        string GetRankDesc(){
            FieldInfo fieldInfo = Rank.GetType().GetField(Rank.ToString());
            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes[0].Description;
        }

        string SpriteName(){
            string testName = $"card{Suit}{GetRankDesc()}";
            return testName;
        }

        public void SetDispOrder(int order){
            spriteRenderer.sortingOrder = order;
        }

        public void OnSelected(bool selected){
            if (selected)
            {

                transform.position = (Vector2)transform.position + Vector2.up * Constants.CARD_SELECTED_OFFSET;
            }
            else
            {
                transform.position = (Vector2)transform.position - Vector2.up * Constants.CARD_SELECTED_OFFSET;
            }
        }
        // ---------------------------------------------------------------------------------------------------------------
        //this is the Code for Memory game's cards
    

        public void OnMouseDown()
        {
            if(Memory_Card_Back.activeSelf && sc.canReveal)
            {
                Memory_Card_Back.SetActive(false);
            sc.CardRevealed(this);
            }
        }

        private int _id;
        public int id
        {
            get { return _id; }
        }

        public void ChangeSprite(int id, Sprite image)
        {
            _id = id;
            GetComponent<SpriteRenderer>().sprite = image; //This gets the sprite renderer component and changes the property of it's sprite!
        }

        public void Unreveal()
        {
            Memory_Card_Back.SetActive(true);
        }

    }
}

