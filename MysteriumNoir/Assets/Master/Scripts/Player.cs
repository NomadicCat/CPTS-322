using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private int startingMoveCount;
    [SerializeField] private float teleportSpeed;
    
    [Header("Object Interaction")]
    [SerializeField] private float objectRotationSpeed;
    [SerializeField] private float objectMoveSpeed;
    [SerializeField] private float objectScaleSpeed;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask whatIsObject;

    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform objectHolder;
    [SerializeField] private Rigidbody rb;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moveCounter;
    [SerializeField] private TextMeshProUGUI message;

    private int moveCount;
    private float lookX;
    private float lookY;
    private float cameraBob;
    private Vector3 moveVector;
    private Vector3 moveDirection;
    private MovementNode targetNode;
    private InteractableObject heldObject;
    #endregion

    private void Start()
    {
        moveCount = startingMoveCount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        message.text = "";
        moveCounter.text = "Moves Remaining: " + moveCount;

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("menu-main");
        }

        // CAMERA ===============================================================================
        #region Camera

        // camera look
        if(heldObject == null)
        {
            lookX += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("sensitivity", lookSpeed);
            lookY -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("sensitivity", lookSpeed);
            lookY = Mathf.Clamp(lookY, -90, 90);

            cameraHolder.transform.localRotation = Quaternion.Euler(new Vector3(lookY, lookX, 0));
            orientation.localRotation = Quaternion.Euler(new Vector3(0, lookX, 0));            
        }

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
            RaycastHit nodeHit;
            if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out nodeHit, Mathf.Infinity, whatIsObject, QueryTriggerInteraction.Collide))
            {
                if(nodeHit.collider.tag == "MovementNode")
                {
                    message.text = "Click To Move";

                    if(Input.GetKeyDown(KeyCode.Mouse0))
                    {       
                        MovementNode movementNode = nodeHit.collider.GetComponent<MovementNode>();
                        if(movementNode != null)
                        {
                            if(targetNode != null) targetNode.LeaveNode();
                            targetNode = movementNode;
                            targetNode.EnterNode();

                            moveCount--;
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

        // INTERACTION ============================================================================
        #region Interaction

        // release object
        bool hasPutDownObject = false;
        if(heldObject != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            heldObject.PutDown();
            heldObject = null;
            hasPutDownObject = true;
        }

        // pickup object
        RaycastHit objectHit;
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out objectHit, interactionDistance, whatIsObject, QueryTriggerInteraction.Collide))
        {
            if(objectHit.collider.tag == "Object" && heldObject == null)
            {
                message.text = "Click To Inspect";

                if(Input.GetKeyDown(KeyCode.Mouse0) && heldObject == null && !hasPutDownObject)
                {
                    InteractableObject targetObject = objectHit.collider.GetComponent<InteractableObject>();
                    if(targetObject != null)
                    {
                        heldObject = targetObject;
                        heldObject.PickUp();
                    }
                }
            }
        }        

        // inspect object
        if(heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, objectHolder.position, Time.deltaTime * objectMoveSpeed);

            heldObject.transform.Rotate(cameraTransform.up, -Input.GetAxis("Mouse X") * objectRotationSpeed, Space.World);
            heldObject.transform.Rotate(cameraTransform.right, Input.GetAxis("Mouse Y") * objectRotationSpeed, Space.World);
        }

        #endregion

    }
    
}
