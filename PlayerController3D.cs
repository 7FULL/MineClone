using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController3D : Entity
{
    public float maxSpeed = 9f;
    
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;

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

    public GameObject inventoryGameObject;

    private bool ableToMove = true;

    public GameObject dropableItem;

    public Inventory inventory;

    public CraftingManager craftingManager;
    private int itemIndex = 0;
    
    private DragDropItem[] items = new DragDropItem[9];
    
    private int PreviousItemIndex=-1;

    public Image handedItem;

    private DragDropItem actualItem;
    
    public float attackSpeed = 5;

    private float countdown = 0.55f;

    private float startedTime = 5;

    public ChunkRenderer lastChunkRenderer;

    void Start()
    {
        startedTime = attackSpeed;
        
        rb = GetComponent<Rigidbody>();
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        world = FindObjectOfType<World>();
        
        romperSprite.transform.SetParent(world.transform);

        grounded = GetComponentInChildren<Grounded>();

        actualizarItems();

        //actualItem = GameManager.instance.defaultItem;

        //Actualizamos la vida a la del jugador
        Life = 20;
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
        Debug.Break();
    }

    public void actualizarItems()
    {
        try
        {
            items = inventory.getHandSlots();
        }
        catch (NullReferenceException e)
        {
            //No hacer nada ya sabemos que devuelve null lo hemos hecho aposta
        }
        EquipItem(itemIndex);
    }
    
    public void pararse()
    {
        rb.velocity = new Vector3(0, 0, 0);
        inventoryGameObject.SetActive(true);
        ableToMove = true;
        Cursor.lockState = CursorLockMode.None;
        ableToMove = false;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 && !inventoryGameObject.activeInHierarchy)
        {
            itemIndex++;
            
            if (itemIndex > items.Length - 1)
            {
                itemIndex = 0;
            }
            
            EquipItem(itemIndex);
            //Debug.Log(itemIndex);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !inventoryGameObject.activeInHierarchy)
        {
            itemIndex--;
            
            if (itemIndex < 0)
            {
                itemIndex = items.Length - 1;
            }
            
            EquipItem(itemIndex);

            //Debug.Log(itemIndex);
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (items[itemIndex] != null)
            {
                if (items[itemIndex].dropearSoloUno())
                {
                    handedItem.sprite = GameManager.instance.invisibleSprite;
                }
            }
            
            //EquipItem(itemIndex);
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(inventoryGameObject.activeInHierarchy)
            {
                craftingManager.craftingTable.gameObject.SetActive(false);
                craftingManager.gameObject.SetActive(true);

                inventoryGameObject.SetActive(false);
                ableToMove = true;
                Cursor.lockState = CursorLockMode.Locked;
                ableToMove = true;
                Cursor.visible = false;
            }
            else
            {
                pararse();
            }
        }

        if (ableToMove)
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

            bool atacado = false;
            
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (attackSpeed <= 0 && Input.GetMouseButton(0))
            {
                if (Physics.Raycast(ray, out RaycastHit hit2, reachDistance))
                {
                    if (hit2.collider.gameObject.GetComponent<Entity>() != null)
                    {
                        atacado = true;
                        
                        hit2.collider.gameObject.GetComponent<Entity>().takeDamage(1);
                        hit2.collider.gameObject.GetComponent<Entity>().Jump((transform.forward*0.5f+Vector3.up));
                        hit2.collider.gameObject.GetComponent<Entity>().huir();
                    }
                }
                attackSpeed = startedTime;
            }
        
            if (Input.GetMouseButton(0) && !atacado)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
                {
                    romperSprite.gameObject.SetActive(true);

                    BlockType blockType = world.GetBlock(hit);
                
                    lookingBlockPos = world.GetBlockPos(hit);

                    lastChunkRenderer = hit.collider.gameObject.GetComponent<ChunkRenderer>();

                    if (blockType != BlockType.AIR && blockType != BlockType.NOTHING && blockType != BlockType.WATER && lookingBlockPos != lastBlockPos)
                    {

                        Block block = GameManager.instance.getBlockData(blockType);
                        
                        resistanceBlock = block.durability;
                        auxParticleMaterial = block.particleMaterial;
                        resistanceBlockAux = resistanceBlock;

                        breakingBlock = true;
                    
                        romperSprite.transform.position = lookingBlockPos;
                        lastBlockPos = lookingBlockPos;
                    }

                    if (broken)
                    {
                        BlockType blockTypeToCompare = world.GetBlock((hit.point - hit.normal * 0.5f),hit.collider.gameObject.GetComponent<ChunkRenderer>());

                        GameObject drop = null;
                        
                        if (blockTypeToCompare != BlockType.AIR && blockTypeToCompare != BlockType.WATER && blockTypeToCompare != BlockType.NOTHING
                            && blockTypeToCompare != BlockType.TREE_LEAFS_SOLID && blockTypeToCompare != BlockType.TREE_LEAFES_TRANSPARENT)
                        {
                            GameObject x = Instantiate(romperParticulas, lookingBlockPos, Quaternion.identity);

                            x.GetComponent<ParticleSystemRenderer>().material = auxParticleMaterial;

                            Destroy(x,2);
                            
                            drop = Instantiate(dropableItem,world.GetBlockPos((hit.point - hit.normal * 0.5f)), Quaternion.identity);

                            drop.GetComponent<DropableItem>().chunkRenderer =
                                hit.collider.gameObject.GetComponent<ChunkRenderer>();

                            if (blockTypeToCompare == BlockType.GRASS_DIRT)
                            {
                                blockTypeToCompare = BlockType.DIRT;
                            }

                            Item item = GameManager.instance.getItem(blockTypeToCompare);
                            
                            //Debug.Log(blockTypeToCompare);

                            drop.GetComponent<DropableItem>().Item = item;
                            drop.GetComponent<DropableItem>().itemMuch = 1;

                            Block block = GameManager.instance.getBlockData(blockTypeToCompare);
                            
                            drop.GetComponent<MeshRenderer>().material = block.particleMaterial;
                        }
                        
                        ModifyTerrain(hit, BlockType.AIR);

                        if (drop != null)
                        {
                            Vector3 direccionAleatoria = Random.onUnitSphere * 100;
                            direccionAleatoria.y = Mathf.Abs(direccionAleatoria.y * 2f);

                            drop.GetComponent<Rigidbody>().AddForce(direccionAleatoria);
                        }

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
                if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
                {
                    Vector3 aux = hit.point + (hit.normal * 0.5f);
                    
                    BlockType blockTypeToCompare = world.GetBlock((hit.point - hit.normal * 0.5f),hit.collider.gameObject.GetComponent<ChunkRenderer>());
                    
                    Item item = GameManager.instance.getItem(blockTypeToCompare);

                    if (!item.interactable)
                    {
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
                            if (actualItem != GameManager.instance.defaultItem && actualItem != null)
                            {
                                
                                ModifyTerrain(hit, actualItem.item.BlockType, aux);

                                int x = items[itemIndex].restarCantidad(1);

                                actualizarItems();
                                
                                if (x == 0)
                                {
                                    handedItem.sprite = GameManager.instance.invisibleSprite;
                                }
                            }
                            else
                            {
                                handedItem.sprite = GameManager.instance.invisibleSprite;
                            }
                        }
                    }
                    else
                    {
                        item.interact();
                    }
                }
            }
        }
    }

    public override void Jump(Vector3 x)
    {
        
    }

    public override void randomDirection()
    {
        
    }

    public override void huir()
    {
        
    }

    private void EquipItem(int index)
    {
        //Debug.Log(index);

        if (items[index] != null)
        {
            handedItem.sprite = items[index].item.sprite;
        }
        else
        {
            handedItem.sprite = GameManager.instance.invisibleSprite;
            //actualItem = GameManager.instance.defaultItem;
        }

        actualItem = items[index];
    }

    private void FixedUpdate()
    {
        attackSpeed -= countdown;

        if (ableToMove)
        {
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
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
    }

    private void ModifyTerrain(RaycastHit hit, BlockType blockType)
    {
        BlockType blockAbove = world.GetBlock((hit.point-hit.normal * 0.5f)+new Vector3(0,1,0),hit.collider.gameObject.GetComponent<ChunkRenderer>());
        
        int x = world.SetBlockInt(hit, blockType);

        //Es decir habia un bloque con gravedad arriba de el
        if (x == 2)
        {
            GameObject z=  Instantiate(bloqueGravedad, world.GetBlockPos((hit.point-hit.normal * 0.5f)+new Vector3(0,1,0)), Quaternion.identity);
            z.GetComponent<GravityBlock>().blockType = blockAbove;
            
            z.GetComponent<GravityBlock>().chunkRenderer = hit.collider.gameObject.GetComponent<ChunkRenderer>();
            
            z.GetComponent<MeshRenderer>().material = GameManager.instance.getBlockData(blockAbove).particleMaterial;
            
            StartCoroutine(llamarDeNuevoTerrainGravity((hit.point-hit.normal * 0.5f)+new Vector3(0,1,0),blockType,hit.collider.gameObject.GetComponent<ChunkRenderer>()));
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
            
            z.GetComponent<GravityBlock>().chunkRenderer = y;
            
            z.GetComponent<MeshRenderer>().material = GameManager.instance.getBlockData(blockAbove).particleMaterial;
            
            StartCoroutine(llamarDeNuevoTerrainGravity(hit+new Vector3(0,1,0),blockType,y));
        }
    }

    IEnumerator llamarDeNuevoTerrainGravity(Vector3 hit, BlockType blockType, ChunkRenderer y)
    {
        yield return new WaitForSecondsRealtime(.2f);
        ModifyTerrainGravity(hit,blockType,y);
    }
    
    private void ModifyTerrain(RaycastHit hit, BlockType blockType, Vector3 posicionAColocar)
    {
        BlockType blocKbelow = world.GetBlock((hit.point+hit.normal*0.5f)+new Vector3(0,-1,0),hit.collider.gameObject.GetComponent<ChunkRenderer>());
        
        
        //O para que si sale que en el bloque de abajo es solido no instanciar un bloque con gravedad seria absurdo
        if (!GameManager.instance.getBlockData(blockType).isGravitationalBlock || GameManager.instance.getBlockData(blocKbelow).isSolid)
        {
            world.SetBlockInt(hit, blockType, posicionAColocar);
        }
        else
        {
            GameObject z=  Instantiate(bloqueGravedad, world.GetBlockPos((hit.point+hit.normal*0.5f)), Quaternion.identity);
            
            z.GetComponent<GravityBlock>().blockType = blockType;
            
            z.GetComponent<GravityBlock>().chunkRenderer = hit.collider.gameObject.GetComponent<ChunkRenderer>();
            
            z.GetComponent<MeshRenderer>().material = GameManager.instance.getBlockData(blockType).particleMaterial;
        }
    }

    //Utilizar para debuguear el cambio de biomas en minecraft
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        //128 es el tamaño del mapa de minecraft en pixeles
        Gizmos.DrawWireCube(transform.position,new Vector3(128,500,128));
    }*/
}
