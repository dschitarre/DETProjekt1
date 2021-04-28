using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : LivingObject
{
    // Start is called before the first frame update
    public Rigidbody2D rigidbody;
    public float bulletForce=5f;
    void Start()
    {
        leben=100;
        infiziert=true;
        StartCoroutine(FightRules.coHusten(this,rigidbody,1));
    }

    // Update is called once per frame
    void Update()
    {
        shoot();    
    }

    void shoot()
    {
        Vector3 forceDirection=FightRules.vectorToPlayer(gameObject.transform.position);
        Vector3 position=gameObject.transform.position+0.5f*forceDirection;
        FightRules.shoot(4,position,forceDirection,bulletForce,gameObject);
    }

    
    void OnTriggerEnter2D(Collider2D other) { 
        if(other.CompareTag("Bullet"))
        {
            bullet bullet=other.GetComponent<bullet>();
            if(bullet==null)
            {
                return;
            }
            else if(bullet.typ==3)
            {
                FightRules.takeDemage(10,this);
            }
        }
    } 
    public override bool immobile()
    {
        return true;
    }   
}
