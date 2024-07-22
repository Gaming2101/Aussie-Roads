using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public static TimerScript me;

    // The timer displayed
    public int minutes = 5;
    public int seconds;

    // The real timer that will end the game once finished
    public int realMinutes = 3;
    public int realSeconds;

    [Header("Ignore")]
    public bool late;
    public bool over;
    float lastT;

    void Awake()
    {
        me = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.me.started)
        {
            // Manages the timers
            if (seconds == 0 & minutes > 0)
            {
                minutes -= 1;
                seconds = 59;
            }
            if (realSeconds == 0 & realMinutes > 0)
            {
                realMinutes -= 1;
                realSeconds = 59;
            }

            // Counts down
            if (Time.time >= lastT + 1)
            {
                seconds -= 1;
                realSeconds -= 1;
                lastT = Time.time;
            }

            // Manages the timers
            if (minutes < 0)
                minutes = 0;
            if (seconds < 0)
                seconds = 0;
            if (realMinutes < 0)
                realMinutes = 0;
            if (realSeconds < 0)
                realSeconds = 0;

            // Checks if timers reach 0
            if (realMinutes == 0 & realSeconds == 0)
                if (!over)
                {
                    over = true;
                    GameManager.me.Failed();
                }
            // Checks if timers reach 0
            if (minutes == 0 & seconds == 0)
                if (!late)
                {
                    late = true;
                    GameManager.me.Late();
                }
        }
    }
}
