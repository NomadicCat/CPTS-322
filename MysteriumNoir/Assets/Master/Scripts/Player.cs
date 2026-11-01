using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayerMovementType
{
    WASD,
    PointAndClick
}

public class Player : MonoBehaviour
{
    // VARIABLES ==========================================================================
    #region Variables
    [Header("Movement Type")]
    [SerializeField] private PlayerMovementType movementType;

    [Header("Camera Movement")]
    [SerializeField] private float lookSpeed;
    [SerializeField] private float bobSpeed;
    [SerializeField] private float bobAmplitude;
    [SerializeField] private float bobSmoothing;

    [Header("WASD Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveAcceleration;

    [Header("Point And Click Movement")]
    [SerializeField] private int maxMoveCount;
    [SerializeField] private float teleportSpeed;
    
    [Header("Object Interaction")]
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask whatIsObject;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;

    private int moveCount;
    private float lookX;
    private float lookY;
    private float cameraBob;
    private Vector3 moveVector;
    private Vector3 moveDirection;
    private MovementNode targetNode;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // CAMERA ===============================================================================
        #region Camera

        // camera look
        lookX += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("sensitivity", lookSpeed);
        lookY -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("sensitivity", lookSpeed);
        lookY = Mathf.Clamp(lookY, -90, 90);

        cameraHolder.transform.localRotation = Quaternion.Euler(new Vector3(lookY, lookX, 0));
        orientation.localRotation = Quaternion.Euler(new Vector3(0, lookX, 0));

        cameraHolder.position = cameraAnchor.position;

        // camera animation
        int doCameraBob = moveDirection != Vector3.zero ? 1 : 0;
        float bobPosition = (float)Math.Sin(Time.time * bobSpeed);
        cameraBob = Mathf.Lerp(cameraBob, bobPosition * bobAmplitude * doCameraBob, Time.deltaTime * bobSmoothing);
        cameraTransform.localPosition = new Vector3(0, cameraBob, 0);

        #endregion

        // MOVEMENT ============================================================================
        #region Movement
        
        // WASD movement
        if(movementType == PlayerMovementType.WASD)
        {
            moveDirection = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");
            moveVector = Vector3.Lerp(moveVector, moveDirection * moveSpeed, Time.deltaTime * moveAcceleration);
            rb.velocity = moveVector;
        }

        // point and click movement
        if(movementType == PlayerMovementType.PointAndClick)
        {
            if(Input.GetKey(KeyCode.Mouse0))
            {
                RaycastHit hit;
                if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, whatIsObject, QueryTriggerInteraction.Collide))
                {
                    if(hit.collider.tag == "MovementNode")
                    {
                        MovementNode movementNode = hit.collider.GetComponent<MovementNode>();
                        if(movementNode != null)
                        {
                            movementNode.Disable();
                            movementNode.EnableNearby();
                            targetNode = movementNode;
                        }
                    }
                }
            }

            if(targetNode != null)
            {
                transform.position = Vector3.Lerp(transform.position, targetNode.transform.position, Time.deltaTime * teleportSpeed);
            }
        }

        #endregion

    }
    
}
