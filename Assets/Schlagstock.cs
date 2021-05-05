using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schlagstock : MonoBehaviour
{

    private Vector2 moveDirection;

    public Rigidbody2D rigidbody;

    public Rigidbody2D userRigidbody;

    private static Transform playerTransform;

    public float speed;//a bit higher than user speed 

    int setHandCooldown=100;
    public bool inRightHand=true;
    // Start is called before the first frame update
    void Start()
    {
        if(userRigidbody==null)
        {
            Debug.Log("No User Rigidbody");
            Destroy(gameObject);
        }
        if(playerTransform==null)
        {
             var playerObject = GameObject.FindGameObjectWithTag("Player");
            playerTransform=playerObject.GetComponent<Transform>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
       Move();
    }
    void setMovementToUser()
    {
        Vector3 positionBesitzer=userRigidbody.transform.position;
        Vector3 position=gameObject.transform.position;
        moveDirection.x=positionBesitzer.x-position.x;
        moveDirection.y=positionBesitzer.y-position.y;
        if(playerTransform)
        {
            if(playerTransform.position.x-position.x>0)
            {
                moveDirection.x+=0.5f;
            }
            else
            {
                moveDirection.x-=0.5f;
            }
            if(playerTransform.position.y-position.y>0)
            {
                moveDirection.y+=0.5f;
            }
            else
            {
                moveDirection.y-=0.5f;
            }
        }
        moveDirection=moveDirection.normalized;
    }
    void Move() {
        setMovementToUser();
        rigidbody.velocity = moveDirection*speed;
    }
    void setHand()
    {
        
    }
}
