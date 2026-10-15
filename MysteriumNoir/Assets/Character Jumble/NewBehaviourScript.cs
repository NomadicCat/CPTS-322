using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] CharacterJumble portrait;
    [SerializeField] Character character;

    // Start is called before the first frame update
    void Start()
    {
        portrait.Apply(character.face);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
