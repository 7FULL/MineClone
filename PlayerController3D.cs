using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController3D : MonoBehaviour
{
    public float maxSpeed = 9f;
    
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;

    public float gravity = 1f;
    
    /*--------------------------*/
    
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 43.0f;
    
    public Sprite[] romperFotos;

    public GameObject romperSprite;
    public GameObject romperParticulas;

    [HideInInspector]
    public bool canMove = true;

    private World world;

    [SerializeField] private float reachDistance = 5;
    
    [SerializeField]private LayerMask groundMask;
    [SerializeField]private float minDistanceToPlace = 0.48f;

    private Vector3Int lookingBlockPos;
    private Vector3Int lastBlockPos;

    private bool breakingBlock = false;

    private int resistanceBlock;
    private int resistanceBlockAux;

    private bool broken = false;

    public BlockDataSO blockData;

    public SpriteRenderer[] fotosSprites;

    private Material auxParticleMaterial;
    
    float rotationX = 0;

    private Grounded grounded;

    private bool sprint = false;

    private Rigidbody rb;
    
    Vector3 moveDirection = Vector3.zero;

    private float auxX;
    private float auxZ;

    public GameObject bloqueGravedad;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        world = FindObjectOfType<World>();
        
        romperSprite.transform.SetParent(world.transform);

        grounded = GetComponentInChildren<Grounded>();
    }

    void Update()
    {
        sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if (rb.velocity.magnitude < maxSpeed)
        {
            if (grounded.isGrounded && Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(jumpSpeed * Vector3.up,ForceMode.Impulse);
                grounded.isGrounded = false;
            }
        }
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        
        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
            {
                romperSprite.gameObject.SetActive(true);

                BlockType blockType = world.GetBlock(hit);
                
                lookingBlockPos = world.GetBlockPos(hit);

                if (blockType != BlockType.AIR && blockType != BlockType.NOTHING && blockType != BlockType.WATER && lookingBlockPos != lastBlockPos)
                {
                    
                    for (int i = 0; i < blockData.blockDataList.Count; i++)
                    {
                        if (blockData.blockDataList[i].blockType == blockType)
                        {
                            resistanceBlock = blockData.blockDataList[i].durability;
                            auxParticleMaterial = blockData.blockDataList[i].particleMaterial;
                            resistanceBlockAux = resistanceBlock;
                        }
                    }
                    
                    breakingBlock = true;
                    
                    romperSprite.transform.position = lookingBlockPos;
                    lastBlockPos = lookingBlockPos;
                }

                if (broken)
                {
                    GameObject x = Instantiate(romperParticulas, lookingBlockPos, Quaternion.identity);

                    x.GetComponent<ParticleSystemRenderer>().material = auxParticleMaterial;

                    Destroy(x,2);

                    ModifyTerrainGravity(hit, BlockType.AIR);
                    
                    //ModifyTerrain(hit, BlockType.AIR);
                    breakingBlock = false;
                    broken = false;
                }
            }
            else
            {
                romperSprite.SetActive(false);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            romperSprite.gameObject.SetActive(false);
            breakingBlock = false;
            broken = false;
            lastBlockPos = new Vector3Int(0, 20000, 0);
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
                    ModifyTerrain(hit, BlockType.SAND, aux);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
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
                    ModifyTerrain(hit, BlockType.GRAVEL, aux);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

            rb.AddForce(gravity * Vector3.down);
        }

        if (!sprint)
        {
            if (playerCamera.fieldOfView > 60)
            {
                playerCamera.fieldOfView--;
            }
        }
        else
        {
            if (playerCamera.fieldOfView < 65 && rb.velocity.magnitude > 3)
            {
                playerCamera.fieldOfView++;
            }
        }

        if (rb.velocity.x < 3 && rb.velocity.z < 3)
        {
            if (playerCamera.fieldOfView > 60)
            {
                playerCamera.fieldOfView--;
            }
        }
        
        if (breakingBlock)
        {
            if (resistanceBlock > 0)
            {
                resistanceBlock--;
            }

            float x = resistanceBlockAux / romperFotos.Length;
            

            float[] y = new float[romperFotos.Length];

            for (int i = 1; i < y.Length; i++)
            {
                y[i] = resistanceBlockAux - x*i;
            }
            
            int indexFoto = 0;
            
            for (int i = 0; i < y.Length; i++)
            {
                if (resistanceBlock < y[i])
                {
                    indexFoto = i;
                }
            }

            for (int i = 0; i < fotosSprites.Length; i++)
            {
                fotosSprites[i].sprite = romperFotos[indexFoto];
            }
            
            if (resistanceBlock == 0)
            {
                broken = true;
                for (int i = 0; i < fotosSprites.Length; i++)
                {
                    fotosSprites[i].sprite = romperFotos[0];
                }
            }
        }
    }

    private void ModifyTerrain(RaycastHit hit, BlockType blockType)
    {
        world.SetBlock(hit, blockType);
    }
    
    private void ModifyTerrainGravity(RaycastHit hit, BlockType blockType)
    {
        BlockType blockAbove = world.GetBlock((hit.point-hit.normal * 0.01f)+new Vector3(0,1,0),hit.collider.gameObject.GetComponent<ChunkRenderer>());
        
        int x = world.SetBlockInt(hit, blockType);

        //Es decir habia un bloque con gravedad arriba de el
        if (x == 2)
        {
            GameObject z=  Instantiate(bloqueGravedad, world.GetBlockPos((hit.point-hit.normal * 0.01f)+new Vector3(0,1,0)), Quaternion.identity);
            z.GetComponent<GravityBlock>().blockType = blockAbove;
            
            for (int i = 0; i < blockData.blockDataList.Count; i++)
            {
                if (blockData.blockDataList[i].blockType == blockAbove)
                {
                    z.GetComponent<MeshRenderer>().material = blockData.blockDataList[i].particleMaterial;
                }
            }
            
            StartCoroutine(llamarDeNuevoTerrainGravity((hit.point-hit.normal * 0.01f)+new Vector3(0,1,0),blockType,hit.collider.gameObject.GetComponent<ChunkRenderer>()));
        }
    }
    
    private void ModifyTerrainGravity(Vector3 hit, BlockType blockType, ChunkRenderer y)
    {
        BlockType blockAbove = world.GetBlock((hit)+new Vector3(0,1,0),y);
        
        int x = world.SetBlockInt(hit, blockType, y);

        //Es decir habia un bloque con gravedad arriba de el
        if (x == 2)
        {
            GameObject z=  Instantiate(bloqueGravedad, world.GetBlockPos((hit)+new Vector3(0,1,0)), Quaternion.identity);
            z.GetComponent<GravityBlock>().blockType = blockAbove;
            
            for (int i = 0; i < blockData.blockDataList.Count; i++)
            {
                if (blockData.blockDataList[i].blockType == blockAbove)
                {
                    z.GetComponent<MeshRenderer>().material = blockData.blockDataList[i].particleMaterial;
                }
            }
            
            StartCoroutine(llamarDeNuevoTerrainGravity(hit+new Vector3(0,1,0),blockType,y));
        }
    }

    IEnumerator llamarDeNuevoTerrainGravity(Vector3 hit, BlockType blockType, ChunkRenderer y)
    {
        yield return new WaitForSecondsRealtime(.25f);
        ModifyTerrainGravity(hit,blockType,y);
    }

    private void ModifyTerrain(RaycastHit hit, BlockType blockType, Vector3 posicionAColocar)
    {
        world.SetBlock(hit, blockType, posicionAColocar);
    }
}
