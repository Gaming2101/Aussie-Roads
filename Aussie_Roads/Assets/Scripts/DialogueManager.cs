using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueSpeaker { Player, NPC }

// Controls all the conversations
[System.Serializable]
public class DialogueConversations
{
    public string name;
    public List<DialogueAltConversations> conversations;
}

// The different types of conversations
[System.Serializable]
public class DialogueAltConversations
{
    public List<DialogueConversation> conversation;
}

// All the data in a dialogue conversation
[System.Serializable]
public class DialogueConversation
{
    [TextArea(2, 5)]
    public string subtitle;
    public AudioClip clip;
    public DialogueSpeaker DialogueSpeaker;
    public float lengthModifier;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager me;
    // The conversations
    public List<DialogueConversations> conversations = new List<DialogueConversations>();

    [Header("Ignore")]
    public int alt;
    public int currentLine;
    [HideInInspector] public DialogueConversations currentConversation;
    [HideInInspector] public AudioSource playerAudio;
    [HideInInspector] public AudioSource npcAudio;

    void Awake()
    {
        me = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up variables
        foreach (AudioSource source in GameManager.me.mainCamera.target.GetComponentsInChildren<AudioSource>())
        {
            if (source.name == "Player Audio")
                playerAudio = source;
            if (source.name == "NPC Audio")
                npcAudio = source;
        }
        StopDialogue();
    }

    // Resets the dialogue variables
    public void StopDialogue()
    {
        alt = 0;
        currentLine = 0;
        currentConversation = new DialogueConversations();
        currentConversation.name = "";
        playerAudio.Stop();
        playerAudio.clip = null;
        npcAudio.Stop();
        npcAudio.clip = null;
    }

    public void PlayDialogue(string convo, int line)
    {
        foreach (DialogueConversations conversation in conversations)
            if (conversation.name == convo)
                currentConversation = conversation;
        if (line == 0)
            alt = Random.Range(0, currentConversation.conversations.Count);
        currentLine = line;
        if (currentConversation.conversations[alt].conversation[currentLine].DialogueSpeaker == DialogueSpeaker.Player)
        {
            playerAudio.clip = currentConversation.conversations[alt].conversation[currentLine].clip;
            playerAudio.Play();
        }
        if (currentConversation.conversations[alt].conversation[currentLine].DialogueSpeaker == DialogueSpeaker.NPC)
        {
            npcAudio.clip = currentConversation.conversations[alt].conversation[currentLine].clip;
            npcAudio.Play();
        }
        StartCoroutine(WaitLine(convo));
    }

    // Waits for the current line to finish
    public IEnumerator WaitLine(string convo)
    {
        yield return new WaitForSecondsRealtime(currentConversation.conversations[alt].conversation[currentLine].clip.length - currentConversation.conversations[alt].conversation[currentLine].lengthModifier + 0.3f);
        if (currentConversation.name == convo)
            if (currentLine < currentConversation.conversations[alt].conversation.Count - 1)
                PlayDialogue(currentConversation.name, currentLine + 1);
            else
                StopDialogue();
    }

    // Plays the appropriate dialogue
    public void PlayIntroDialogue()
    {
        StopDialogue();
        PlayDialogue("Intro", 0);
    }

    // Plays the appropriate dialogue
    public void PlayLateDialogue()
    {
        StopDialogue();
        PlayDialogue("Late", 0);
    }

    // Plays the appropriate dialogue
    public void PlayFailedDialogue()
    {
        StopDialogue();
        PlayDialogue("Failed", 0);
    }

    // Plays the appropriate dialogue
    public void PlayCrashDialogue()
    {
        StopDialogue();
        PlayDialogue("Crash", 0);
    }

    // Plays the appropriate dialogue
    public void PlayHardCrashDialogue()
    {
        StopDialogue();
        PlayDialogue("Hard Crash", 0);
    }

    // Plays the appropriate dialogue
    public void PlayDeathDialogue()
    {
        StopDialogue();
        PlayDialogue("Death", 0);
    }

    // Plays the appropriate dialogue
    public void PlaySucceedDialogue()
    {
        StopDialogue();
        PlayDialogue("Succeed", 0);
    }
}
