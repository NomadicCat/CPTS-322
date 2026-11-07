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

    //How rattled the character is under questioning. Rises with each
    //question - more for specific, detailed ones - and makes slip-ups and
    //contradictions more likely. Decays a little between questions.
    [Range(0f, 10f)]
    public float stress;
    
    public string backStory;

    public string AIArchetype;

    //Small details this character personally glimpsed or overheard.
    public List<string> keyInfo = new List<string>();

    //What this character knows or believes about each of their neighbors.
    public List<string> relationships = new List<string>();

    //Where this character was and what they were doing around the events.
    public string vantagePoint;

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
        //Lets characters reference what others have already told the
        //detective. Never includes the lie flags - those are system-only.
        string caseSummary = CaseFile.BuildCharacterSummary();
        if (!string.IsNullOrEmpty(caseSummary))
        {
            messages.Add(new ChatMessage("system", caseSummary));
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

    //Answers the detective in character, then records the exchange in the
    //shared case file so other characters can reference it later. The system
    //quietly classifies each answer as truth, lie, or speculation.
    public async Task<string> AnswerDetective(string question)
    {
        ApplyQuestionStress(question);
        string response = await sendToAI(question);
        if (!string.IsNullOrEmpty(response)) RecordTestimony(question, response);
        return response;
    }

    //Stress builds incrementally: a small step per question, plus a modest
    //bonus for specific, detailed ones. The killer feels it more keenly -
    //they have something to hide.
    private void ApplyQuestionStress(string question)
    {
        float increase = 0.25f + Mathf.Clamp01((question.Length - 40) / 120f) * 0.75f;
        if (isKiller) increase *= 1.25f;
        stress = Mathf.Clamp(stress + increase, 0f, 10f);
        BuildAIArchetype();
    }

    //Fire-and-forget: runs after the answer is already on screen. The
    //character judges their own answer (they know what they know and whether
    //they are the killer), so the player never sees the classification.
    private async void RecordTestimony(string question, string response)
    {
        string verdict = await sendToAI(
            "The detective asked you: \"" + question + "\" and you answered: \"" + response + "\". " +
            "Given what you actually know and whether you are the killer, judge your own answer. " +
            "Reply with exactly one word: TRUTH if it was honest, LIE if you deliberately deceived the detective, " +
            "or SPECULATION if you improvised beyond what you actually know.", false);

        bool isLie = false;
        bool isSpeculation = false;
        if (!string.IsNullOrEmpty(verdict))
        {
            string[] words = verdict.ToUpperInvariant().Split(new[] { ' ', '\r', '\n', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                string w = word.Trim('.', ',', '!', '?', '"', '\'');
                if (w == "LIE" || w == "LIED" || w == "LYING") isLie = true;
                else if (w.StartsWith("SPECULAT")) isSpeculation = true;
            }
        }

        CaseFile.Record(characterName, question, response, isLie, isSpeculation);

        //A moment to breathe after answering lets them recover a little.
        stress = Mathf.Max(0f, stress - 0.15f);
        BuildAIArchetype();
    }


    [SerializeField] private CharacterJumble portrait;
    public int faceSeed;
    
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

    private static readonly string[] personalities =
    {
        "Reserved", "Cheerful", "Grumpy", "Nervous", "Curious", "Stoic",
        "Chatty", "Suspicious", "Eccentric", "Calm"
    };

    //Distinct places and activities so each character experiences the events
    //from a different vantage point instead of all sharing one scene.
    private static readonly string[] vantagePoints =
    {
        "in the kitchen, cleaning up and in and out of the pantry",
        "out in the garden, having a smoke and pacing by the fence",
        "in the upstairs hallway, half-dozing in an armchair",
        "in the study, absorbed in paperwork with the radio on low",
        "in the garage, working on a project with the door half open",
        "in the living room, dozing in front of the television",
        "on the front porch, watching the street and the weather",
        "in the laundry room in the basement, running a load of wash"
    };

    private static readonly HashSet<string> usedVantagePoints = new HashSet<string>();

    private static readonly HashSet<string> usedNames = new HashSet<string>();
    private static readonly HashSet<string> usedJobs = new HashSet<string>();

    //Runs when the component is added or the scene loads. Picks a unique
    //name and job. The Story triggers AI generation of this character's
    //knowledge and backstory once the story exists.
    private void Awake()
    {
        faceSeed = portrait.GenerateFaceSeed();
        portrait.Generate(faceSeed);
        trust = 0;
        pessimism = Random.Range(0f, 1f);
        fear = Random.Range(0f, 1f);
        stress = 0;
        GenerateIdentity();
    }

    //Keeps the GameObject's name in sync so the character is easy to find
    //in the Hierarchy and Inspector.
    private void SyncGameObjectName()
    {
        if (this != null && gameObject != null && !string.IsNullOrEmpty(characterName))
            gameObject.name = characterName;
    }

    private void GenerateIdentity()
    {
        characterName = PickUniqueName();
        gender = Random.value < 0.5f ? "Male" : "Female";
        age = Random.Range(18, 81);
        job = PickUniqueJob();
        singleWordPersonality = personalities[Random.Range(0, personalities.Length)];
        vantagePoint = PickUniqueVantagePoint();

        SyncGameObjectName();
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

    private static string PickUniqueVantagePoint()
    {
        foreach (string candidate in vantagePoints)
        {
            if (usedVantagePoints.Add(candidate)) return candidate;
        }
        string fallback = "somewhere on their own, away from the others (" + usedVantagePoints.Count + ")";
        usedVantagePoints.Add(fallback);
        return fallback;
    }

    //Builds this character's personal knowledge from the actual events,
    //filtered through their own vantage point so no two characters share the
    //same experience. The killer instead recalls details only they could
    //know, plus cover stories.
    public async Task GenerateKeyInfo(string roster)
    {
        string prompt;
        if (isKiller)
        {
            prompt = "You are " + characterName + ", a " + singleWordPersonality + " " + job + ". Here is what actually happened: " + actualStory +
                "\n\nYou are the killer, though the account above does not say so. Your cover story is that you were " + vantagePoint + " at the time. List 5-7 things you know: details only the killer would know about how it happened, things you noticed about the others while moving around, and 1-2 believable lies you will tell the detective to deflect suspicion. Make your lies imperfect: each one should contain a small inconsistency - a time that does not quite line up, a detail that contradicts something else you claim, or a claim that conflicts with a verifiable fact from the events - so a sharp detective could catch the contradiction." +
                "\nThe people involved:\n" + roster;
        }
        else
        {
            prompt = "You are " + characterName + ", a " + singleWordPersonality + " " + job + ". Here is what actually happened: " + actualStory +
                "\n\nYou did NOT commit the murder and do not know who did. Around the time of the events you were " + vantagePoint + ". List 5-7 things you personally know, but ONLY things you could plausibly have seen, heard, or noticed from that spot and while doing that activity - muffled sounds through walls, glimpses through doorways or windows, people passing by, things out of place you stumbled on later. Do NOT copy details from the account that you could not have perceived from where you were, and never place yourself anywhere else. Also include things you know about your neighbors (habits, grudges, recent odd behavior). You only saw fragments - never the murder itself - and some of what you noticed may be irrelevant or misleading." +
                "\nThe people involved:\n" + roster;
        }
        prompt += "\nFormat every line as either 'CLUE: ...' for things you saw, heard, or will claim, or 'NEIGHBOR: <name> - ...' for knowledge about another person. No other text.";

        string response = await sendToAI(prompt, false);
        if (!string.IsNullOrEmpty(response)) ParseKeyInfo(response);
        BuildAIArchetype();
    }

    private void ParseKeyInfo(string response)
    {
        keyInfo.Clear();
        relationships.Clear();
        List<string> unprefixed = new List<string>();
        string[] lines = response.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim().TrimStart('-', '*', ' ');
            if (line.StartsWith("CLUE:", System.StringComparison.OrdinalIgnoreCase))
                keyInfo.Add(line.Substring(5).Trim());
            else if (line.StartsWith("NEIGHBOR:", System.StringComparison.OrdinalIgnoreCase))
                relationships.Add(line.Substring(9).Trim());
            else if (line.Length > 0)
                unprefixed.Add(line);
        }
        //If the model ignored the format, keep its knowledge rather than nothing.
        if (keyInfo.Count == 0 && relationships.Count == 0)
            keyInfo.AddRange(unprefixed);
    }

    //Called by Story once the actual events and this character's knowledge
    //exist, so the retelling is grounded in both.
    public async void GenerateBackStory()
    {
        string knowledge = "";
        foreach (string info in keyInfo) knowledge += "- " + info + "\n";
        foreach (string rel in relationships) knowledge += "- " + rel + "\n";

        backStory = await sendToAI("Write a first-person account (300-400 words) as told by " + characterName + ", a " + singleWordPersonality + " " + job + ", about the events that occurred. Here are those actual events: " + actualStory +
            "\nAround the time of the events they were " + vantagePoint + "." +
            "\nThings " + characterName + " personally knows or noticed:\n" + knowledge +
            "\nWrite it as their personal story of that day, in their own voice: what they were doing, what they saw and heard, who they ran into, what they made of it, and how they felt. Include small sensory details (sounds, smells, weather, objects) that make it feel lived-in. Stay anchored to where they were and what they were doing, weaving in only what they personally know. Do not include details from the actual events that they could not have perceived.", false);
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
        + " Their current stress level under questioning is " + stress + "/10, where 0 is calm and 10 is about to crack."
        + " Around the time of the events they were " + vantagePoint + "."
        + " This is their story of that day, in their own words - it is the main thing they draw on when answering: \"" + backStory + "\" ";
        foreach(string info in keyInfo) { AIArchetype += "They personally know: " + info + "; "; }
        foreach(string rel in relationships) { AIArchetype += "They know this about a neighbor: " + rel + "; "; }
        if(isKiller) AIArchetype += " " + characterName + " is the killer. They must never confess; they deflect and lie, and never reveal or hint that they are lying. But they are not a perfect liar: when pressed on details they sometimes contradict themselves - a time that shifts, a detail that does not match their own story or the facts - and they do not notice the slip. The higher their stress level, the more often and more obviously they slip up; near 0 stress their lies are careful, near 10 they contradict themselves frequently. They never lie so cleanly that there is nothing to catch.";
        else AIArchetype += " " + characterName + " is not the killer and does not know who is. They only know what they personally saw or heard and can only speculate beyond that.";
        AIArchetype += " If the detective asks about something not covered above - such as their relationship to the victim - they invent a plausible answer that fits their personality and everything they have already said, and stay consistent with it in future answers. They never break character by saying they do not know.";
        AIArchetype += " When answering the detective, speak naturally in first person as " + characterName + ": conversational, detailed, and grounded in their story above. Answer in a few sentences to a short paragraph, weaving in the specific details, people, and feelings from their story rather than giving flat one-line replies. Let their personality color the tone.";
        AIArchetype += "[-----------------------------------------------------------] " + "please respond in character";
    }
    #endregion
}
