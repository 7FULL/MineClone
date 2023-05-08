using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Animal : Entity
{
    public float speed = 5f;
    public float jumpSpeed = 5f;
    public float maxFallDistance = 5f;
    public float stopDistance = 0.1f;

    private Vector3 destination;
    private bool isMoving;

    public Transform jumpLaser;
    public Transform fallLaser;

    private Rigidbody rb;
    
    public float moveRadius;
    private bool jumped = false;
    
    private bool falling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        randomDirection();
    }

    void Update()
    {
        if (isMoving && !jumped && !falling)
        {
            MoveTowardsDestination();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            CheckForObstacles();
        }
    }

    public void SetDestination(Vector3 newDestination)
    {
        Debug.Log("Nuevo destino: "+newDestination);
        
        destination = newDestination;
        isMoving = true;

        //Si el punto esta debajo se va a rotar entero para intentar mirarlo 
        newDestination.y = transform.position.y;
        
        transform.LookAt(newDestination);
    }

    private void MoveTowardsDestination()
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0;
        direction.Normalize();
        
        //Al normalizar no podemos aplicarle la de rb porque es mas que 1 y se va muy lejos
        //direction.y = rb.velocity.y;
        
        //Al no asignarle ninguna velocidad se vera un poco mal la caida

        rb.velocity = direction * speed;

        Vector2 auxThis = new Vector2(transform.position.x, transform.position.z); 
        Vector2 auxDestination = new Vector2(destination.x, destination.z); 
        
        if (Vector2.Distance(auxThis,auxDestination) <= stopDistance)
        {
            Debug.Log("He llegado");
            stop();
            randomDirection();
        }
    }

    private void Jump()
    {
        Debug.Log("He saltado");
        rb.AddForce(Vector3.up * jumpSpeed,ForceMode.Impulse);
    }

    private void CheckForObstacles()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(jumpLaser.position, jumpLaser.forward, out hitInfo, 1f))
        {
            //para comprobar si en el bloque de arriba hay un hueco en el que coloarnos
            if (!Physics.Raycast(jumpLaser.position + Vector3.up, jumpLaser.forward, out hitInfo, 1f))
            {
                if (!jumped)
                {
                    jumped = true;
                    Jump();
                }
            }
            else
            {
                Debug.Log("No nos caemos pero no hay bloque al que saltar");
                randomDirection();
            }
        }
        else
        {
            if (!jumped)
            {
                CheckForFalling();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumped = false;

        falling = false;
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        falling = false;
    }

    private void OnCollisionExit(Collision other)
    {
        falling = true;
    }

    private void CheckForFalling()
    {
        RaycastHit hitInfo;

        if (!Physics.Raycast(fallLaser.position, -fallLaser.up, out hitInfo, maxFallDistance))
        {
            Debug.Log("Me caigo");
            stop();
            randomDirection();
        }
    }

    private void stop()
    {
        Debug.Log("Parado");
        
        isMoving = false;
        rb.velocity = new Vector3(0, 0, 0);
    }
    
    private void randomDirection()
    {
        stop();
        
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        SetDestination(randomDirection);
    }
    
    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay(jumpLaser.position, jumpLaser.forward);
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(jumpLaser.position, jumpLaser.position + jumpLaser.forward);
        
        Gizmos.DrawWireSphere(transform.position,moveRadius);
        
        Gizmos.DrawLine(fallLaser.position,fallLaser.position + new Vector3(0,-maxFallDistance,0));
    }
}