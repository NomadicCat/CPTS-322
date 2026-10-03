using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name;
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

    //member functions
    public string sendToAI(string text) {return "TODO";}





    //public GameObject characterJumble;
    public Character()
    {
        
        trust = 0;
        pessimism = Random.Range(0f, 1f);
        fear = Random.Range(0f, 1f);
        string response = sendToAI("I am going to give you a template with multiple blanks, I want you to give me back a response that is an identical string with the only exception being the blanks filled in with your choices. The template is as follows: Name: ____|Gender: ____|Age: ____|Profession: ____|singleWordPersonality: ____");
        string[] responseParts = response.Split('|');
        name = responseParts[0].Split(':')[1].Trim();
        gender = responseParts[1].Split(':')[1].Trim();
        age = int.Parse(responseParts[2].Split(':')[1].Trim());
        job = responseParts[3].Split(':')[1].Trim();
        singleWordPersonality = responseParts[4].Split(':')[1].Trim();
        isKiller = false;
        //GameObject characterJumble = getComponent<GameObject>();
        //will generate a backstory once Story class is implemented
        //will generate key info once Story class is implemented
        backStory = sendToAI("Generate a short (less than 300 word) point of view story for the character named " + name + ". Who is a " + singleWordPersonality + " " + job + ". Based on the events that occurred. Here are those actual events: " + actualStory);


        AIArchetype = "[-----------------------------------------------------------------------]"
         + name + " is a " + singleWordPersonality + " " + job + 
        ". They are " + age + " years old, " + gender + 
        ". On a scale of 0 to 10 where 0 is no trust and 10 is complete and total trust in the detective " + name +" is a " + trust + "/10" 
        +" they are " + pessimism*100 + "% pessimistic. With 0% being an optimist and 100% being a pessimist." +
         "They also have a fear level of " + fear*100 + "%. With 100% being total panic."
        + " Their recollection of the events: " + backStory + " ";
        foreach(string info in keyInfo) { AIArchetype += "They know: " + info + ";"; }
        if(isKiller) AIArchetype += " " + name + " is the killer.";
        else AIArchetype += " " + name + " is not the killer.";
        AIArchetype += "[-----------------------------------------------------------------------]";
    }

}
