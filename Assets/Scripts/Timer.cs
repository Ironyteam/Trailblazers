using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Countdown timer for each turn.
public class Timer : MonoBehaviour
{
    float timeLeft = Constants.TurnTime;

    public string Text;
    public bool Over = false;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        Text = Mathf.Round(timeLeft).ToString();

        if (timeLeft < 0)
            Over = true;
    }
}
