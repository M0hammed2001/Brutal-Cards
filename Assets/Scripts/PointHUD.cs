using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Leaderboard{
    public class PointHUD : MonoBehaviour {
        [SerializeField] Text pointText;
        [SerializeField] GameObject pointChangePrefab;
        [SerializeField] Transform pointParent;
        [SerializeField] RectTransform endPoint;

        [SerializeField] Color colorGreen;
        [SerializeField] Color colorRed;

        int points = 0;

        private void Awake () {
            UpdateHUD ();
        }

        public int Points {
            get {
                return points;
            }

            set {
                ShowPointChange (value - points);
                points = value;
                UpdateHUD ();
            }
        }

        public void ResetPoints () {
            points = 0;
            UpdateHUD ();
        }

        private void ShowPointChange (int change) {
            var inst = Instantiate (pointChangePrefab, Vector3.zero, Quaternion.identity);
            inst.transform.SetParent (pointParent, false);

            RectTransform rect = inst.GetComponent<RectTransform> ();

            Text text = inst.GetComponent<Text> ();

            text.text = (change > 0 ? "+ " : "") + change.ToString ();
            text.color = change > 0 ? colorGreen : colorRed;

            LeanTween.moveY (rect, endPoint.anchoredPosition.y, 1.5f).setOnComplete (() => {
                Destroy (inst);
            });
            LeanTween.alphaText (rect, 0.25f, 1.5f);
        }

        private void UpdateHUD () {
            pointText.text = points.ToString ();

        }

    }
}
