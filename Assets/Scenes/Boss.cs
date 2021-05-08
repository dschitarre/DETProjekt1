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
         Game.Instance.SetTexture(gameObject, "bill_gates", 1.0f);
        leben=Game.Instance.Settings.bossLives;
        infiziert=true;
        StartCoroutine(FightRules.coHusten(this, new System.Random(gameObject.transform.position.GetHashCode()), rigidbody,1, 1f));
        StartCoroutine(coShooting());
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity=new Vector2(0,0);
    }

    void shoot()
    {
        Vector3 forceDirection=FightRules.vectorToPlayer(gameObject.transform.position);
        Vector3 position=gameObject.transform.position+1f*forceDirection;
        FightRules.shoot(4,position,forceDirection,bulletForce,gameObject);
    }
    IEnumerator coShooting()
    {
        yield return new WaitForSeconds(0.5f);
        while(true)
        {
            shoot();
            yield return new WaitForSeconds(1f);
        }
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
