using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointHUD : MonoBehaviour
{
    [SerializeField] TextMeshPro BrutalwinsText;

    int points = 0;

    public int Points
    {
        get
        {
            return points;
        }

        set
        {
            points = value;
            UpdateHUD();
        }
    }

        private void UpdateHUD ()
    {
        BrutalwinsText.text = points.ToString();

    }

    }
 }

