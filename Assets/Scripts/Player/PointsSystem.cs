using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;
    public int points;

    public void AddPoints(int amount)
    {
        points += amount;
        pointsText.text = $"POINTS: {points}";
    }

    public void RemovePoints(int amount) 
    {
        points -= amount;
        pointsText.text = $"POINTS: {points}";
    }
}
