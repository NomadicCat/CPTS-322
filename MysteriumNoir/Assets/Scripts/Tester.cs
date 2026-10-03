using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Story story;
    // Start is called before the first frame update
    void Start()
    {
        story = new Story();
        Debug.Log(story);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
