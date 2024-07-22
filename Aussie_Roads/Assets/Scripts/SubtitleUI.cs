using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubtitleUI : MonoBehaviour
{
    public static SubtitleUI me;

    [Header("Display")]
    public CanvasGroup subtitle;
    public TextMeshProUGUI speakerT;
    public TextMeshProUGUI lineT;

    void Awake()
    {
        me = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hides the information
        subtitle.alpha = 0;
        speakerT.text = "";
        lineT.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // Controls the visibility of the displayed data based on the current conversation
        if (DialogueManager.me.currentConversation.name != "")
        {
            // Sets the UI to the current conversation data
            subtitle.alpha = Mathf.Lerp(subtitle.alpha, 1, 0.1f);
            if (DialogueManager.me.currentConversation.conversations[DialogueManager.me.alt].conversation[DialogueManager.me.currentLine].DialogueSpeaker == DialogueSpeaker.Player)
                speakerT.text = "Player";
            if (DialogueManager.me.currentConversation.conversations[DialogueManager.me.alt].conversation[DialogueManager.me.currentLine].DialogueSpeaker == DialogueSpeaker.NPC)
                speakerT.text = "Friend";
            lineT.text = DialogueManager.me.currentConversation.conversations[DialogueManager.me.alt].conversation[DialogueManager.me.currentLine].subtitle;
        }
        else
            subtitle.alpha = Mathf.Lerp(subtitle.alpha, 0, 0.1f);
    }
}
