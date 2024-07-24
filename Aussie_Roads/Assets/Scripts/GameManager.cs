using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager me;

    // The vehicle camera
    public VehicleCamera mainCamera;
    // The current amount of damage caused by the player
    public int damageCaused;
    // The damage limit before the player fails
    public int maxDamageCaused = 15;

    // All the UI messages
    public UIMessageScript skipMessage;
    public UIMessageScript introMessage;
    public UIMessageScript enterCarEffect;
    public UIMessageScript objectiveHint;
    public UIMessageScript fadeInMessage;
    public UIMessageScript fadeOutMessage;
    public UIMessageScript failMessage;
    public UIMessageScript succeedMessage;
    public UIMessageScript crashMessage;

    [Header("Ignore")]
    public bool started;
    public bool over;
    public bool succeded;
    public float timeScale;
    public float startT;
    public bool skipAble;
    public float additionTrafficRange = 200;
    public Canvas HUD;
    public Canvas mainMenu;
    public GameObject blocker;
    public Vector3 startPos;

    void Awake()
    {
        // Sets up variables
        me = this;
        foreach(Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.name == "UI")
                HUD = c;
            if (c.name == "Main Menu")
                mainMenu = c;
        }
        mainCamera = FindObjectOfType<VehicleCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up variables for the terrain for performance
        foreach (Terrain terrain in FindObjectsOfType<Terrain>())
        {
            terrain.treeDistance = 500;
            terrain.detailObjectDistance = 500;
            terrain.treeBillboardDistance = 1324;
        }

        // Sets up variables
        if (IntroCameraScript.me)
        {
            HUD.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
        }
        else
        {
            HUD.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(false);
        }
        startPos = mainCamera.transform.position;
        timeScale = 1;
        SetTime();
        blocker = mainCamera.transform.parent.transform.GetChild(0).gameObject;
        blocker.transform.SetParent(null);
        if (IntroCameraScript.me)
        {
            mainCamera.gameObject.SetActive(false);
            IntroCameraScript.me.gameObject.SetActive(true);
        }
        else
        {
            skipAble = true;
            StartCoroutine(WaitStart());
            Destroy(blocker);
            blocker = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Adjusts the graphics settings
        if (Input.GetKeyDown(KeyCode.Alpha1))
            if (QualitySettings.GetQualityLevel() == 0)
                QualitySettings.SetQualityLevel(1);
            else
                QualitySettings.SetQualityLevel(0);
        // Controls the main menu play button, and the skip cutscene button
        if (!succeded)
            if (IntroCameraScript.me)
            {
                mainCamera.gameObject.SetActive(started);
                IntroCameraScript.me.gameObject.SetActive(!started);
                if (skipAble)
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                        StartCoroutine(EndIntro(true));
            }
        if (mainMenu.isActiveAndEnabled)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HUD.gameObject.SetActive(true);
                mainMenu.gameObject.SetActive(false);
                StartGame();
            }

        // Main Menu Code
        if (Input.GetKeyDown(KeyCode.Backspace))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Controls the vehicle camera and the cursor.
        mainCamera.target.GetComponent<VehicleControl>().activeControl = started & !over;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StartGame()
    {
        Instantiate(fadeInMessage);
        StartCoroutine(WaitIntro());
    }

    // Controls the timescale
    public void SetTime()
    {
        Time.timeScale = timeScale;
    }

    // Waits for the intro cutscene
    public IEnumerator WaitIntro()
    {
        yield return new WaitForSeconds(0.8f);
        Instantiate(introMessage);
        StartCoroutine(WaitStart());
        yield return new WaitForSeconds(0.9f);
        skipAble = true;
        Instantiate(skipMessage);
    }

    // Waits for the game to actually start
    public IEnumerator WaitStart()
    {
        if (IntroCameraScript.me)
        {
            yield return new WaitForSeconds(5.7f);
            yield return new WaitForSeconds(2);
            DialogueManager.me.PlayIntroDialogue();
            if (IntroCameraScript.me.ani.isActiveAndEnabled)
                IntroCameraScript.me.ani.CrossFadeInFixedTime("Intro", 0.1f);
            float length = IntroCameraScript.me.ani.runtimeAnimatorController.animationClips[0].length;
            yield return new WaitForSeconds(length + 0.5f);
        }
        if (skipAble)
            StartCoroutine(EndIntro());
    }

    // Ends the intro and starts the game
    public IEnumerator EndIntro(bool skipped = false)
    {
        if (blocker)
            Destroy(blocker);
        blocker = null;
        skipAble = false;
        if(skipped)
            started = true;
        if (IntroCameraScript.me)
        {
            RaycastHit checkGround;
            if (Physics.Raycast(mainCamera.target.position, -mainCamera.target.up, out checkGround, 100))
                mainCamera.target.position = checkGround.point + (mainCamera.target.up * 0.2f);
            mainCamera.target.GetComponent<VehicleDamage>().spawnT = Time.timeSinceLevelLoad;
            foreach (VehicleDamage d in FindObjectsOfType<VehicleDamage>())
                d.spawnT = Time.timeSinceLevelLoad;
            UIMessageScript enter = Instantiate(enterCarEffect);
            if (skipped)
                yield return new WaitForSeconds(0.1f);
            foreach (UIMessageScript message in FindObjectsOfType<UIMessageScript>())
                if (message != enter)
                    Destroy(message.gameObject);
            yield return new WaitForSeconds(0.2f);
        }
        started = true;
        startT = Time.timeSinceLevelLoad;
        additionTrafficRange = 0;
        yield return new WaitForSeconds(2);
        Instantiate(objectiveHint);
    }

    // Triggers the late events
    public void Late()
    {
        if (over) return;
        DialogueManager.me.PlayLateDialogue();
    }

    // Triggers the fail events
    public void Failed()
    {
        if (over) return;
        over = true;
        DialogueManager.me.PlayFailedDialogue();
        StartCoroutine(WaitOver(5, 1));
    }

    // Triggers the succeed events
    public void Succeed()
    {
        if (over) return;
        over = true;
        succeded = true;
        DialogueManager.me.PlaySucceedDialogue();
        StartCoroutine(WaitOver(18, 3));
    }

    // Triggers the crash events
    public void Crash(int type)
    {
        if (over) return;
        damageCaused += type;
        // Controls which audio to play based on the severity
        if (damageCaused >= maxDamageCaused)
        {
            over = true;
            DialogueManager.me.PlayDeathDialogue();
            StartCoroutine(WaitOver(5, 2));
        }
        else
        {
            if (type > 7)
            {
                if (type > 13)
                    DialogueManager.me.PlayDeathDialogue();
                else
                    DialogueManager.me.PlayHardCrashDialogue();
            }
            else
                DialogueManager.me.PlayCrashDialogue();
        }
    }

    // Waits for the game to be over
    public IEnumerator WaitOver(float time, int type)
    {
        if (type == 1)
            yield return new WaitForSecondsRealtime(0.15f);
        if (type == 2)
            yield return new WaitForSecondsRealtime(0.3f);
        if (type < 3)
            timeScale = 0.4f;
        SetTime();
        yield return new WaitForSecondsRealtime(time / 2);
        // Shows the UI messages
        if (type == 1)
            Instantiate(failMessage);
        if (type == 2)
            Instantiate(crashMessage);
        if (type == 3)
            Instantiate(succeedMessage);
        if (type < 3)
            yield return new WaitForSecondsRealtime(time / 1.1f);
        else
            yield return new WaitForSecondsRealtime(4);
        Instantiate(fadeOutMessage);
        yield return new WaitForSecondsRealtime(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
