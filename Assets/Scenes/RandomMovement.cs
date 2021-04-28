using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rigidbody;
    private Vector2 moveDirection;

    private int timeToNextMovementChange;
    
    private int timeNoMovement;
    Impfbar meineImpfung;
    static Transform playerTransformation;
    void Start()
    {
        timeToNextMovementChange=0;
        timeNoMovement=0;
        var player = GameObject.FindGameObjectWithTag("Player");
        playerTransformation=player.transform;
        meineImpfung=gameObject.GetComponent<Impfbar>();
        moveDirection=new Vector2(0,0);
    }
    void setRandomMovement()
    {
        System.Random random=new System.Random();
        moveDirection.x=(float)random.NextDouble()*2-1;
        moveDirection.y=(float)random.NextDouble()*2-1;
        timeToNextMovementChange=random.Next(60);
    }
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
    }
    void Update() {
    }

    void FixedUpdate() {
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
    }
    void Move() {
        //rigidbody.velocity = new Vector2(moveDirection.x, moveDirection.y).normalized * moveSpeed;
    }
    public void ruhigStellen(int dauer)
    {
        moveDirection=new Vector2(0,0);
        timeNoMovement=Math.Max(dauer,timeNoMovement);
    }
}
