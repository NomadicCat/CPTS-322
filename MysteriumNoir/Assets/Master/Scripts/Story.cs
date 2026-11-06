using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Story : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    public Character killer;

    public string storyText;

    //Awake runs before any other script's Start, so the cast and the killer
    //are always assigned before the player can ask anyone a question.
    public void Awake()
    {
        CaseFile.Reset();
        for(int i = 0; i < 6; i++) characters.Add(CreateCharacter(i));
        AssignKiller();
    }

    //Creates a real GameObject with a Character component, parented under
    //this Story, so the character shows up in the Hierarchy and Inspector.
    private Character CreateCharacter(int index)
    {
        GameObject characterObject = new GameObject("Character " + (index + 1));
        characterObject.transform.SetParent(transform);
        return characterObject.AddComponent<Character>();
    }

    public void Start()
    {
        GenerateStory();
    }

    //Guarantees exactly one killer: clears any stale flags, then picks one.
    private void AssignKiller()
    {
        foreach(Character character in characters) character.isKiller = false;
        killer = characters[Random.Range(0, characters.Count)];
        killer.isKiller = true;
    }

    //Builds the ground truth of the case, then gives every character their
    //own partial knowledge (clues they glimpsed, things they know about
    //their neighbors) before they write their personal retelling.
    private async void GenerateStory()
    {
        storyText = await GenerateStoryText();

        string roster = BuildRoster();
        List<Task> knowledgeTasks = new List<Task>();
        foreach(Character character in characters)
        {
            character.actualStory = storyText;
            knowledgeTasks.Add(character.GenerateKeyInfo(roster));
        }
        await Task.WhenAll(knowledgeTasks);

        foreach(Character character in characters) character.GenerateBackStory();
    }

    //The true account of the murder. Includes the method and concrete
    //verifiable details but never names which suspect did it, so no
    //innocent character can quote the culprit from it.
    public async Task<string> GenerateStoryText()
    {
        List<ChatMessage> messages = new List<ChatMessage>
        {
            new ChatMessage("system", "You are a mystery writer creating the hidden truth of a murder case for a detective game."),
            new ChatMessage("user",
                "Write the true account (under 300 words) of a murder involving these suspects:\n" + BuildRoster() +
                "\nThe victim is NOT one of the suspects. Describe how the murder was done and include concrete details a detective could verify: approximate times, places, sounds, objects, and small incidents before and during the crime. Spread the events across different parts of the property (kitchen, garden, upstairs, study, garage, living room, porch, basement) so that people in different places would each have noticed different fragments. Do NOT reveal which suspect is the killer.")
        };

        string result = await AIManager.Instance.GenerateResponse(messages);
        if (string.IsNullOrEmpty(result))
        {
            result = "A guest was found dead after the lights went out at 9pm. When they came back on, the victim was on the floor and a window stood open. Everyone claims they were alone at the time.";
        }
        storyText = result;
        return storyText;
    }

    private string BuildRoster()
    {
        string roster = "";
        foreach(Character character in characters)
        {
            roster += "- " + character.characterName + ", " + character.age + ", " + character.gender +
                      ", " + character.singleWordPersonality + " " + character.job + "\n";
        }
        return roster;
    }
}
