using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrashStatistics : MonoBehaviour
{
    [Header("Information")]
    [TextArea(3, 5)]
    public string[] messages;
    public string[] statistics;

    [Header("Information Texts")]
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI statText;

    // Ignore
    float spawnT;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up everything
        statText.GetComponent<CanvasGroup>().alpha = 0;
        mainText.GetComponent<CanvasGroup>().alpha = 0;

        // Randomly sets the text to a pool of apropriate information
        mainText.text = messages[Random.Range(0, messages.Length)];
        statText.text = statistics[Random.Range(0, statistics.Length)];
        spawnT = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the text to the right information
        if (Time.timeSinceLevelLoad > spawnT + 0.5f)
            mainText.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(mainText.GetComponent<CanvasGroup>().alpha, 1, 0.1f);
        if (Time.timeSinceLevelLoad > spawnT + 1)
            statText.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(statText.GetComponent<CanvasGroup>().alpha, 1, 0.1f);
    }
}
