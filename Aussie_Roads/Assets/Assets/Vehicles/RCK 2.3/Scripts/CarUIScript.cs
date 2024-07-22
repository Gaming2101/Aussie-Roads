using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUIScript : MonoBehaviour
{
    public static CarUIScript me;
    public CarUIClass CarUI;
    public CanvasGroup hud;

    [System.Serializable]
    public class CarUIClass
    {
        public Image tachometerNeedle;
        public Image barShiftGUI;

        public Text speedText;
        public Text GearText;
    }

    void Awake()
    {
        me = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        hud.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.me.started & !GameManager.me.over)
            hud.alpha = Mathf.Lerp(hud.alpha, 1, 0.05f);
        else
            hud.alpha = Mathf.Lerp(hud.alpha, 0, 0.08f);
    }
}
