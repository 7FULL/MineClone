using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController3D : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 43.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private World world;

    [SerializeField] private float reachDistance = 5;
    
    [SerializeField]private LayerMask groundMask;
    [SerializeField]private float minDistanceToPlace = 0.48f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Time.timeScale = 1;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        world = FindObjectOfType<World>();
    }

    void Update()
    {
	    Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKey(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
            {
                ModifyTerrain(hit, BlockType.Air);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
            {
                Vector3 aux = hit.point + (hit.normal * 0.5f);

                //Para impedir poder poner un bloque donde esta el jugador posicionado calculamos todas las posiciones alrededor suya y no dejamos colocar bloques ahi
                if (Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,1,0)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(minDistanceToPlace,1,0)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(-minDistanceToPlace,1,0)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,1,minDistanceToPlace)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,1,-minDistanceToPlace)) &&
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(minDistanceToPlace,0,0)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(-minDistanceToPlace,0,0)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,0,minDistanceToPlace)) && 
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,0,-minDistanceToPlace))&&
                    Vector3Int.RoundToInt(aux) != Vector3Int.RoundToInt(transform.position+new Vector3(0,-minDistanceToPlace,0)))
                {
                    ModifyTerrain(hit, BlockType.Dirt, aux);
                }
            }
        }
    }
    
    private void ModifyTerrain(RaycastHit hit, BlockType blockType)
    {
        world.SetBlock(hit, blockType);
    }
    
    private void ModifyTerrain(RaycastHit hit, BlockType blockType, Vector3 posicionAColocar)
    {
        world.SetBlock(hit, blockType, posicionAColocar);
    }
}
