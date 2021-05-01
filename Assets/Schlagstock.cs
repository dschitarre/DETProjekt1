using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schlagstock : MonoBehaviour
{

    private Vector2 moveDirection;

    public Rigidbody2D rigidbody;

    public Rigidbody2D userRigidbody;

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
        setHand();
        if(inRightHand)
        {
            moveDirection.x+=0.5f;
        }
        else
        {
            moveDirection.x-=0.5f;
        }
        moveDirection=moveDirection.normalized;
    }
    void Move() {
        setMovementToUser();
        rigidbody.velocity = moveDirection*speed;
    }
    void setHand()
    {
        if(setHandCooldown>0)
        {
            setHandCooldown--;
        }
        else
        {
            setHandCooldown=100;
            if(moveDirection.x>=0)
            {
                inRightHand=true;
            }
            else
            {
                inRightHand=false;
            }
        }
    }
}
