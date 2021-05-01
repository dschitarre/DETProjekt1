using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rigidbody;
    Impfbar meineImpfung;
    static Transform playerTransformation;

    float timeNoMovement;

    void Start()
    {
        timeNoMovement=0;
        if(playerTransformation!=null)
        {
             var player = GameObject.FindGameObjectWithTag("Player");
            playerTransformation=player.transform;
        }
        meineImpfung=gameObject.GetComponent<Impfbar>();
    }
    public void setMovementSpeed(float newMovementSpeed)
    {
        moveSpeed=newMovementSpeed;
    }
    /*
    void setMovementToPlayer()
    {
        if(playerTransformation==null)
        {
            return;
        }
        Vector3 positionPlayer=playerTransformation.position;
        Vector3 position=gameObject.transform.position;
        moveDirection.x=positionPlayer.x-position.x;
        moveDirection.y=positionPlayer.y-position.y;
    }*/

    /*void FixedUpdate() {
        if(timeNoMovement>0)
        {
            timeNoMovement--;
        }
        else
        {
            if(meineImpfung.politiker)
            {
                return;
            }
            if(meineImpfung.wuetend)
            {
            setMovementToPlayer();
            }
            else
            {
                if(timeToNextMovementChange==0)
                {
                    setRandomMovement();
                }
                else
                {
                    timeToNextMovementChange--;
                }
            }
        }
        Move();
    }*/
    void Move() {
        //rigidbody.velocity = new Vector2(moveDirection.x, moveDirection.y).normalized * moveSpeed;
    }
    public void ruhigStellen(float seconds)
    {
        timeNoMovement=Math.Max(seconds,timeNoMovement);
        rigidbody.velocity = new Vector2(0,0);
    }

    /*
    returns true if von Impfungen wuetend 
    */
    public bool wuetend()
    {
        return meineImpfung.wuetend;
    }

    public bool immobile()
    {
        if(meineImpfung.immobile())
        {
            return true;
        }
        if(timeNoMovement>0)
        {
            timeNoMovement=0;
            return true;
        }
        return false;
    }
}
