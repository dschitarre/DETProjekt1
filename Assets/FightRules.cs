using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightRules
{
    private static Transform playerTransformation;//mit erstem aufruf wird es gesetzt

    private static float virusForce=10f;
    public static Transform getPlayerTransformation()
    {
        if(playerTransformation==null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerTransformation=player.transform;
        }
        return playerTransformation;
    }
    public static bool takeDemage(int lifes, LivingObject livingObject)
    {
        livingObject.leben-=10;
        if(livingObject.leben<=0)
        {
            MonoBehaviour.Destroy(livingObject.gameObject);
            return true;
        }
        return false;
    }  
    public static void shoot(int bulletTyp, Vector3 position, Vector3 force, float bulletForce, GameObject gameObject)
    {
        Debug.Log("Position: "+position+" , Force "+force);
        GameObject bullet = MonoBehaviour.Instantiate(Game.waeponPrefabs[bulletTyp], position, gameObject.transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(force * bulletForce, ForceMode2D.Impulse);
        bullet bulletScript=bullet.GetComponent<bullet>();
        bulletScript.typ=bulletTyp;
    }
    public static void shoot(int bulletTyp, Transform firePoint, float bulletForce)
    {
        GameObject bullet = MonoBehaviour.Instantiate(Game.waeponPrefabs[bulletTyp], firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        bullet bulletScript=bullet.GetComponent<bullet>();
        bulletScript.typ=bulletTyp;
    }

    public static IEnumerator coHusten(LivingObject livingObject, Rigidbody2D rigidbody, float timeBetween)
    {
        while(true)
        {
            if(livingObject.infiziert)
            {
                Vector3 personVelocity=new Vector3(rigidbody.velocity.x,rigidbody.velocity.y,0);
                if(livingObject.immobile()||personVelocity==new Vector3(0,0,0))
                {
                    personVelocity=vectorToPlayer(livingObject.transform.position);
                    Debug.Log("Vector to player: "+personVelocity);
                }
            shootThreeVirusWithAngle(personVelocity,livingObject.gameObject);
            }
            yield return new WaitForSeconds(timeBetween);
        }
    }
    private static void shootThreeVirusWithAngle(Vector3 personVelocity, GameObject gameObject)
    {
        float distanceSpawnPoint=0.5f;
        Vector3 a=personVelocity;//vector straight shoot
        Debug.Log(a);
        if(a.x==0)
        {
            a.x=0.01f;//otherwise dividing throw zero
        }
        float radian=(Mathf.PI/180)*30;//<90!
        Vector3 b1=new Vector3(0,0,0);//vector angle° right
        Vector3 b2=new Vector3(0,0,0);//vector angle° left
        float p=((-2)*a.sqrMagnitude*Mathf.Cos(radian)*a.y)/(a.y*a.y+a.x*a.x);
        float q=(a.sqrMagnitude*a.sqrMagnitude*Mathf.Cos(radian)*Mathf.Cos(radian)-a.sqrMagnitude*a.x*a.x)/(a.y*a.y+a.x*a.x);
        b1.y=-(p/2)+Mathf.Sqrt(Mathf.Abs((p/2)*(p/2)-q));
        b1.x=calculateB1(a,radian,b1.y);
        b2.y=-(p/2)-Mathf.Sqrt(Mathf.Abs((p/2)*(p/2)-q));
        b2.x=calculateB1(a,radian,b2.y);
        Vector3 positionSpawn=gameObject.transform.position+personVelocity*distanceSpawnPoint;
        shootVirus(positionSpawn,personVelocity, gameObject);
        positionSpawn=gameObject.transform.position+b1*distanceSpawnPoint;
        shootVirus(positionSpawn,b1, gameObject);
        positionSpawn=gameObject.transform.position+b2*distanceSpawnPoint;
        shootVirus(positionSpawn,b2, gameObject);
    }
    private static void shootVirus(Vector3 positionSpawn, Vector3 force3, GameObject gameObject)
    {
        Vector2 force=new Vector2(force3.x,force3.y);
        FightRules.shoot(2,positionSpawn,force,virusForce,gameObject);
    }
    private static float calculateB1(Vector2 a, float radian, float b2)
    {
        if(a.x==0)
        {
            a.x=0.01f;
        }
        return (a.sqrMagnitude*Mathf.Cos(radian)-a.y*b2)/a.x;
    }
    public static Vector3 vectorToPlayer(Vector3 position)
    {
        Vector3 vectorToPlayer=new Vector3(0,0,0);
        Vector3 positionPlayer=getPlayerTransformation().position;
        vectorToPlayer.x=positionPlayer.x-position.x;
        vectorToPlayer.y=positionPlayer.y-position.y;
        vectorToPlayer=vectorToPlayer.normalized;
        return vectorToPlayer;
    }
}
