using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Collider meshCollider;
    
    private bool isPickedUp;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    private void Update()
    {
        if(!isPickedUp)
        {
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void PutDown()
    {
        isPickedUp = false;
        meshCollider.enabled = true;
    }

    public void PickUp()
    {
        isPickedUp = true;
        meshCollider.enabled = false;
    }
}
