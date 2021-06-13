using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject ChatBG;
    public TextMeshProUGUI dialogueBox;

    public float textClearTime = 0.5f;

    bool next = false;

    public GameObject[] towns;

    private void Start()
    {
        PlayDrivingInstructions();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            next = true;
        }
    }

    void SendMessage(string message)
    {
        next = false;
        ChatBG.SetActive(true);
        dialogueBox.text = message;
    }

    void ClearDialogue()
    {
        ChatBG.SetActive(false);
    }

    void PlayDrivingInstructions()
    {
        StartCoroutine(DrivingInstructions());
    }

    IEnumerator DrivingInstructions()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "W is to go faster. This 1986 model has two speed: " +
            "kinda slow and kinda faster. The brakes are mostly shot, " +
            "but press S and you'll stop eventually. " +
            "You just need to press the button once to change gears; " +
            "don't wear it out by holding the button.";
        SendMessage(message);

        yield return new WaitForSeconds(3);
        while(!next)
        {
            yield return null;
        }

        StartCoroutine(NoBackwards());
    }

    IEnumerator NoBackwards()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "You can't go backwards. " +
            "Management assures me it's by design and not " +
            "just because they couldn't be bothered to add reverse.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        StartCoroutine(ClawInstructions());

    }

    IEnumerator ClawInstructions()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "Fire your claw with left click to pick up crates of resources. " +
            "You can find them around the area at the nearby mines, etc. " +
            "The crates will be placed on your cars. Each car holds 2 crates.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        StartCoroutine(JobDescription());

    }

    IEnumerator JobDescription()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "Your job is to take the crates to the towns that want them. " +
            "The towns here are greedy and will demand your back car whenever you drive by and " +
            "they will take all of the crates on it regardless of if they asked for those items or not. " +
            "They will store 2 crates, but the rest get gobbled up. Like I said: greedy.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        StartCoroutine(SlideRailExplanation());

    }

    IEnumerator SlideRailExplanation()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "To combat their gluttony and insane tolls, we have developed these slide rails and inserters. " +
            "Press space when near the slide rail to cut a car out of your train. " +
            "It will jet forward ready to be inserted wherever you want into your train. " +
            "Press space again to be inserted at the red indicators.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        StartCoroutine(SlideRailTips());

    }

    IEnumerator SlideRailTips()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "Use this to move cars around to deliver the right stuff. " +
            "Be careful, those slide rails are expensive so there's only a few of them. " +
            "So you won't be able to switch around many cars between towns.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while(!next)
        {
            yield return null;
        }

        StartCoroutine(LicenseInfo());
    }

    IEnumerator LicenseInfo()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "Well, now you know how to drive a train and all that. " +
            "I'm just gonna need your train license for legal stuff.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        StartCoroutine(ForgotPapers());
    }

    IEnumerator ForgotPapers()
    {
        ClearDialogue();
        yield return new WaitForSeconds(textClearTime);
        string message = "You left it at home? Your home in Canada? Uh huh. … " +
            "Whatever, you already did the training and we're short staffed as is.";
        SendMessage(message);
        yield return new WaitForSeconds(3);

        while (!next)
        {
            yield return null;
        }

        // end
        ClearDialogue();
        StartGame();
    }

    void StartGame()
    {
        for (int i = 0; i < towns.Length; i++)
        {
            towns[i].SetActive(true);
        }
    }

    
}
