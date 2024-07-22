using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public static TimerUI me;

    [Header("The different parts of the timer")]
    public CanvasGroup hud;
    public CanvasGroup timer;
    public CanvasGroup clock;
    public CanvasGroup lateMsg;

    [Header("The timers")]
    public TextMeshProUGUI minutesT;
    public TextMeshProUGUI secondsT;

    [Header("Different colours for the text")]
    public TMP_FontAsset redCol;
    public TMP_FontAsset lateCol;

    // Ignore
    TMP_FontAsset normalCol;
    bool flash;
    float lastFlash;

    void Awake()
    {
        // Sets the reference point for other scripts to access this script
        me = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets everything to invisible
        timer.alpha = 0;
        clock.alpha = 1;
        lateMsg.alpha = 0;
        minutesT.text = "";
        secondsT.text = "";
        normalCol = minutesT.font;
    }

    // Update is called once per frame
    void Update()
    {
        // Controls the HUD visibility
        if (GameManager.me.started & !GameManager.me.over)
            hud.alpha = Mathf.Lerp(hud.alpha, 1, 0.05f);
        else
            hud.alpha = Mathf.Lerp(hud.alpha, 0, 0.08f);

        // Checks that the game has started
        if (Time.timeSinceLevelLoad > 0.08f & GameManager.me.started)
        {
            // Sets the minutes and second to the right data
            timer.alpha = Mathf.Lerp(timer.alpha, 1, 0.01f);
            string mCount = "";
            string sCount = "";
            string m = " : Minutes";
            string s = " : Seconds";
            if (TimerScript.me.minutes < 10)
                mCount = "0";
            if (TimerScript.me.seconds < 10)
                sCount = "0";
            if (TimerScript.me.minutes < 2)
                m = " : Minute";
            if (TimerScript.me.seconds < 2)
                s = " : Second";
            minutesT.text = mCount + TimerScript.me.minutes + m;
            secondsT.text = sCount + TimerScript.me.seconds + s;

            // Checks if the player is late
            if (TimerScript.me.late)
            {
                // Shows the late message and hides the timer texts
                clock.alpha = Mathf.Lerp(clock.alpha, 0, 0.15f);
                lateMsg.alpha = Mathf.Lerp(lateMsg.alpha, 1, 0.01f);
                minutesT.font = lateCol;
                secondsT.font = lateCol;
                timer.GetComponent<RectTransform>().localScale = Vector3.Lerp(timer.GetComponent<RectTransform>().localScale, new Vector3(0.8f, 0.8f, 0.8f), 0.05f);
            }
            else
            {
                if (TimerScript.me.minutes < 1 & TimerScript.me.seconds < 40)
                {
                    // Makes the timer flash red and white when the timer is low in order to stress the player out
                    if (Time.time > lastFlash + 0.7f)
                    {
                        if (flash)
                        {
                            minutesT.font = lateCol;
                            secondsT.font = lateCol;
                        }
                        else
                        {
                            minutesT.font = redCol;
                            secondsT.font = redCol;
                        }
                        flash = !flash;
                        lastFlash = Time.time;
                    }
                }
                else
                {
                    // Shows the normal text colours
                    minutesT.font = normalCol;
                    secondsT.font = normalCol;
                }
            }
        }
    }
}
