using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] CharacterJumble portrait; 

    // Start is called before the first frame update
    void Start()
    {
        portrait.Apply(CharacterJumble.Roll());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
