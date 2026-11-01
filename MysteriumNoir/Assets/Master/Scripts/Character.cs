using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Character : MonoBehaviour
{
    #region attributes
    public string characterName;
    public int age;
    public string gender;
    public string singleWordPersonality;

    [Range(0f, 10f)]
    public float trust;
    [Range(0f, 1f)]
    public float pessimism;
    [Range(0f, 1f)]
    public float fear;
    
    public string backStory;

    public string AIArchetype;

    public List<string> keyInfo = new List<string>();

    //TODO
    public string job;
    public bool isKiller;
    public string actualStory;
    #endregion

    #region memberFunctions
    //Sends text to the AI with this character's archetype and full conversation
    //history, then remembers the exchange like a typical AI chat wrapper.
    //Set remember to false for one-off setup prompts that shouldn't enter memory.
    public async Task<string> sendToAI(string text, bool remember = true)
    {
        List<ChatMessage> messages = new List<ChatMessage>();
        if (!string.IsNullOrEmpty(AIArchetype))
        {
            messages.Add(new ChatMessage("system", AIArchetype));
        }
        messages.AddRange(memory);
        messages.Add(new ChatMessage("user", text));

        string response = await AIManager.Instance.GenerateResponse(messages);

        if (remember && !string.IsNullOrEmpty(response))
        {
            memory.Add(new ChatMessage("user", text));
            memory.Add(new ChatMessage("assistant", response));
        }

        return response;
    }





    //public GameObject characterJumble;
    
    public List<ChatMessage> memory = new List<ChatMessage>();

    //Pools of modern names and jobs. A character can only take a name or job
    //that no other existing character is already using.
    private static readonly string[] firstNames =
    {
        "Liam", "Noah", "Oliver", "Elijah", "Mateo", "Lucas", "Levi", "Asher",
        "James", "Leo", "Grayson", "Ethan", "Jackson", "Aiden", "Mason", "Carter",
        "Olivia", "Emma", "Charlotte", "Amelia", "Sophia", "Isabella", "Ava",
        "Mia", "Luna", "Harper", "Camila", "Gianna", "Evelyn", "Nora", "Zoe", "Layla"
    };

    private static readonly string[] lastNames =
    {
        "Smith", "Johnson", "Brown", "Garcia", "Miller", "Davis", "Rodriguez",
        "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Walker",
        "Hall", "Young", "King", "Wright", "Scott", "Green", "Baker", "Adams"
    };

    private static readonly string[] jobs =
    {
        "Teacher", "Nurse", "Software Developer", "Electrician", "Barista",
        "Accountant", "Mechanic", "Chef", "Graphic Designer", "Plumber",
        "Real Estate Agent", "Pharmacist", "Personal Trainer", "Journalist",
        "Photographer", "Dental Hygienist", "Social Worker", "Bartender",
        "Marketing Manager", "Construction Worker"
    };

    private static readonly HashSet<string> usedNames = new HashSet<string>();
    private static readonly HashSet<string> usedJobs = new HashSet<string>();

    //Runs for both "new Character()" and AddComponent<Character>(). Picks a
    //unique name and job, then kicks off AI generation of the backstory.
    public Character()
    {
        trust = 0;
        pessimism = Random.Range(0f, 1f);
        fear = Random.Range(0f, 1f);
        //GameObject characterJumble = getComponent<GameObject>();
        //will generate key info once Story class is implemented
        GenerateIdentity();
    }

    private void GenerateIdentity()
    {
        characterName = PickUniqueName();
        gender = Random.value < 0.5f ? "Male" : "Female";
        age = Random.Range(18, 81);
        job = PickUniqueJob();
        singleWordPersonality = "Reserved";

        GenerateBackStory();
        BuildAIArchetype();
    }

    private static string PickUniqueName()
    {
        //Try random combinations first; fall back to a full scan if the pool
        //is nearly exhausted so we never loop forever.
        for (int attempt = 0; attempt < 100; attempt++)
        {
            string candidate = firstNames[Random.Range(0, firstNames.Length)] + " " +
                               lastNames[Random.Range(0, lastNames.Length)];
            if (usedNames.Add(candidate)) return candidate;
        }
        foreach (string first in firstNames)
        {
            foreach (string last in lastNames)
            {
                string candidate = first + " " + last;
                if (usedNames.Add(candidate)) return candidate;
            }
        }
        string fallback = "Character " + usedNames.Count;
        usedNames.Add(fallback);
        return fallback;
    }

    private static string PickUniqueJob()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            string candidate = jobs[Random.Range(0, jobs.Length)];
            if (usedJobs.Add(candidate)) return candidate;
        }
        foreach (string candidate in jobs)
        {
            if (usedJobs.Add(candidate)) return candidate;
        }
        string fallback = "Job " + usedJobs.Count;
        usedJobs.Add(fallback);
        return fallback;
    }

    private async void GenerateBackStory()
    {
        //will generate a backstory once Story class is implemented
        backStory = await sendToAI("Generate a short (less than 300 word) point of view story for the character named " + characterName + ". Who is a " + singleWordPersonality + " " + job + ". Based on the events that occurred. Here are those actual events: " + actualStory, false);
        BuildAIArchetype();
    }

    private void BuildAIArchetype()
    {
        AIArchetype = "[-----------------------------------------------------------------------]"
         + "You  are "+ characterName + " they are a " + singleWordPersonality + " " + job + 
        ". They are " + age + " years old, " + gender + 
        ". On a scale of 0 to 10 where 0 is no trust and 10 is complete and total trust in the detective " + characterName +" is a " + trust + "/10" 
        +" they are " + pessimism*100 + "% pessimistic. With 0% being an optimist and 100% being a pessimist." +
         "They also have a fear level of " + fear*100 + "%. With 100% being total panic."
        + " Their recollection of the events: " + backStory + " ";
        foreach(string info in keyInfo) { AIArchetype += "They know: " + info + ";"; }
        if(isKiller) AIArchetype += " " + characterName + " is the killer.";
        else AIArchetype += " " + characterName + " is not the killer.";
        AIArchetype += "[-----------------------------------------------------------] " + "please respond in character";
    }
    #endregion
}
