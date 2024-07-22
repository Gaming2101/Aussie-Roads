using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageScript : MonoBehaviour
{
    [Header("The variables")]
    public float flashTime = 3;
    public float startAlpha;
    public float inRate = 0.1f;
    public float outRate = 0.05f;

    [Header("Enlarging")]
    public float enlargeAmount;
    public float enlargeRate = 0.01f;

    [Header("Ignore")]
    CanvasGroup canvas;
    TextMeshProUGUI enlargeText;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up everything
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = startAlpha;
        time = Time.fixedTime;
        transform.SetAsFirstSibling();
        if(enlargeAmount > 0)
            enlargeText = GetComponentInChildren<TextMeshProUGUI>();
        Destroy(gameObject, flashTime + 5);
    }

    // Update is called once per frame
    void Update()
    {
        // Controls the visibility based on the timescale
        transform.GetChild(0).gameObject.SetActive(Time.timeScale > 0);
    }

    void FixedUpdate()
    {
        // Enlarges text
        if (enlargeText & Time.fixedTime > time + 0.1f)
            enlargeText.transform.localScale = Vector3.Lerp(enlargeText.transform.localScale, new Vector3(enlargeAmount, enlargeAmount, enlargeAmount), enlargeRate);
       
        // Controls the visibility based on the time displayed
        if (Time.fixedTime < time + flashTime)
            canvas.alpha = Mathf.Lerp(canvas.alpha, 1, inRate);
        else
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, 0, outRate);
            if (canvas.alpha <= 0.0001f)
                Destroy(gameObject);
        }
    }
}
