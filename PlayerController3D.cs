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

    public bool ableToMove = true;

    public bool explosionCurrent = false;

    public GameObject dropableItem;

    public Inventory inventory;

    public CraftingManager craftingManager;
    private int itemIndex = 0;
    
    private DragDropItem[] items = new DragDropItem[9];
    
    private int PreviousItemIndex=-1;

    public Image handedItem;

    private DragDropItem actualItem;
    
    [Header("50 -> 1s")]
    public float attackSpeed = 5;

    private float countdown = 0.55f;

    private float startedTime = 5;

    public ChunkRenderer lastChunkRenderer;

    public Sprite[] uiSprites;

    public Image[] uiImages;

    private int actualLife = 20;

    private int actualHunger = 20;

    private int hungerCooldown;
    
    private int lifeHungerTimeCooldown = 500;

    [Tooltip("La cantidad de tiempo que tiene que pasar para que se quite media barrita de comida")]
    [Header("50 -> 1s")]
    public int hungerTime = 1000;

    [Tooltip("La cantidad de tiempo que tiene que pasar para que se quite medio corazon cuando estas sin comida")]
    [Header("50 -> 1s")]
    public int lifeHungerTime = 500;

    [Space] 
    public InventorySlots[] activeItems;

    public InventorySlots[] handSlotsItems;

    public GameObject dragDropItem;

    public Transform itemsFather;

    public RectTransform activeItemBox;

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

        updateItems();

        //actualItem = GameManager.instance.defaultItem;

        //Actualizamos la vida a la del jugador
        actualLife = Life;
        actualLife = 20;

        actualHunger = 20;
        
        calcularCorazones();
        calcularComida();

        hungerCooldown = hungerTime;

        lifeHungerTimeCooldown = lifeHungerTime;
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
        Debug.Break();
    }

    public void updateItems()
    {
        try
        {
            items = inventory.getHandSlots();
        }
        catch (NullReferenceException e)
        {
            //No hacer nada ya sabemos que devuelve null lo hemos hecho aposta
            Debug.Log("null");
        }
        EquipItem(itemIndex);
        
        updateHandSlots();
    }

    public void updateHandSlots()
    {
        for (int i = 0; i < activeItems.Length; i++)
        {
            if (activeItems[i].item != null)
            {
                Destroy(activeItems[i].item.gameObject);
            }
        }

        for (int i = 0; i < handSlotsItems.Length; i++)
        {
            if (handSlotsItems[i].item != null)
            {
                GameObject x = Instantiate(dragDropItem,itemsFather);

                DragDropItem y = x.GetComponent<DragDropItem>();
                
                x.GetComponent<RectTransform>().anchoredPosition = activeItems[i].GetComponent<RectTransform>().anchoredPosition;

                y.item = handSlotsItems[i].item.item;
                    
                activeItems[i].GetComponent<InventorySlots>().implementDrag(y);
                
                y.actualizarCantidad(handSlotsItems[i].item.itemMuch);
                    
                y.inicializarFoto();
            }
        }
    }
    
    public void stop()
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
                    updateHandSlots();
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
                stop();
            }
        }
        
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

        if (!inventoryGameObject.activeInHierarchy)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        
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
                
                //Debug.Log(lookingBlockPos);

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
        
        if (Input.GetMouseButtonDown(1) && !inventoryGameObject.activeInHierarchy)
        {
            if (actualItem != null)
            {
                if (actualItem.item.interactable && !actualItem.item.isPlacable)
                {
                    actualItem.item.interact();
                    updateHandSlots();
                }
            }
            if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, groundMask))
            {
                Vector3 aux = hit.point + (hit.normal * 0.5f);
                    
                BlockType blockTypeToCompare = world.GetBlock((hit.point - hit.normal * 0.5f),hit.collider.gameObject.GetComponent<ChunkRenderer>());
                    
                Item item = GameManager.instance.getItem(blockTypeToCompare);

                if (!item.interactable && item.isPlacable)
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

                            updateItems();
                                
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
                    item.interact(Vector3Int.RoundToInt(aux),hit);
                }
            }
        }
        
    }

    public void receiveExplosion()
    {
        ableToMove = false;

        explosionCurrent = true;
    }

    public void usedItem()
    {
        int x = actualItem.restarCantidad(1);
        if (x != 0)
        {
            EquipItem(itemIndex);
        }
        else
        {
            handedItem.sprite = GameManager.instance.invisibleSprite;
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

        activeItemBox.anchoredPosition = new Vector2((float)(7.399994 + 72.5 * index),activeItemBox.anchoredPosition.y);
        
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
    
    public void takeDamage(int damage)
    {
        actualLife -= damage;

        calcularCorazones();
        
        if (actualLife <= 0)
        {
            die();   
        }
    }

    private void calcularCorazones()
    {
        Image[] corazones = new[] { uiImages[0],uiImages[1],uiImages[2],uiImages[3],uiImages[4],uiImages[5],uiImages[6],uiImages[7],uiImages[8],uiImages[9] };

        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].sprite = uiSprites[0];//Corazon vacio
        }

        int auxLife = actualLife;

        bool half = false;
        
        if (auxLife % 2 != 0)
        {
            half = true;
            auxLife--;
        }

        for (int i = 0; i < auxLife/2; i++)
        {
            corazones[i].sprite = uiSprites[1];//Corazon lleno
        }

        if (half)
        {
            corazones[auxLife/2].sprite = uiSprites[2];//Corazon a la mitad
        }
    }
    
    private void calcularComida()
    {
        Image[] corazones = new[] { uiImages[10],uiImages[11],uiImages[12],uiImages[13],uiImages[14],uiImages[15],uiImages[16],uiImages[17],uiImages[18],uiImages[19] };

        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].sprite = uiSprites[3];//Comida vacio
        }

        int auxLife = actualHunger;

        bool half = false;
        
        if (auxLife % 2 != 0)
        {
            half = true;
            auxLife--;
        }

        for (int i = 0; i < auxLife/2; i++)
        {
            corazones[i].sprite = uiSprites[4];//Comida lleno
        }

        if (half)
        {
            corazones[auxLife/2].sprite = uiSprites[5];//Comida a la mitad
        }
    }

    public bool eat(int cantidad)
    {
        if (actualHunger == 20)
        {
            return false;
        }
        else
        {
            actualHunger += cantidad;
            calcularComida();
            return true;
        }
    }

    private void lostHunger(int cantidad)
    {
        if (actualHunger > 0)
        {
            actualHunger -= cantidad;
            calcularComida();
        }
    }

    private void addLife(int cantidad)
    {
        actualLife += cantidad;

        if (actualLife > 20)
        {
            actualLife = 20;
        }
    }

    private void FixedUpdate()
    {
        if (grounded.isGrounded && explosionCurrent)
        {
            ableToMove = true;
            explosionCurrent = false;
        }
        
        attackSpeed -= countdown;

        if (hungerTime > 0)
        {
            hungerTime--;
        }
        else
        {
            if (actualLife != 20)
            {
                addLife(1);
                lostHunger(2);
                hungerTime = hungerCooldown;
            }
            else
            {
                lostHunger(1);
                hungerTime = hungerCooldown;
            }
            
        }

        if (actualHunger == 0)
        {
            if (lifeHungerTime > 0)
            {
                lifeHungerTime--;
            }
            else
            {
                takeDamage(1);
                lifeHungerTime = lifeHungerTimeCooldown;
            }
        }
        
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
