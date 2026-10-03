using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story
{
    public List<Character> characters = new List<Character>();
    public Character killer;

    public string storyText;
    public Story() 
    {
        for(int i = 0; i < 6; i++) characters.Add(new Character());
        killer = characters[Random.Range(0, characters.Count)];
        killer.isKiller = true;
        storyText = GenerateStoryText();
        foreach(Character character in characters) character.actualStory = storyText;
    }
    public string GenerateStoryText(){storyText = "TODO"; return storyText;}

}
