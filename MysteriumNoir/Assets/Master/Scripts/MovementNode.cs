using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : MonoBehaviour
{
    [SerializeField] private bool startEnabled;
    [SerializeField] private MovementNode[] nearbyNodes;
    [SerializeField] private Collider trigger;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject sprite;

    private void Start()
    {
        if(startEnabled)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    public void LeaveNode()
    {
        DisableNearby();
        Enable();
    }

    public void EnterNode()
    {
        EnableNearby();
        Disable();
    }

    public void Enable()
    {
        trigger.enabled = true;
        particles.Play();
        sprite.SetActive(true); 
    }

    public void Disable()
    {
        trigger.enabled = false;
        particles.Stop();
        particles.Clear();
        sprite.SetActive(false);
    }

    public void EnableNearby()
    {
        foreach(MovementNode node in nearbyNodes)
        {
            node.Enable();
        }          
    }

    public void DisableNearby()
    {
        foreach(MovementNode node in nearbyNodes)
        {
            node.Disable();
        }            
    }
}
