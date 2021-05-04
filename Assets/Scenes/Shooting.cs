using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public float bulletForce = 20f;
    public int specialBullets=100;

    public int weapon2Number=1;

    private int nextShoot=0;

    void Start()
    {
        StartCoroutine(coShooting());
    }

    void Update() {
        if(Input.GetButtonDown("Fire1")) {
                nextShoot=1;
            }
            if(Input.GetButtonDown("Fire2")&&specialBullets>0)
            {
               nextShoot=2;
            }
            if(Input.GetButtonDown("Fire3"))
            {
                nextShoot=3;
            }
    }

    void ShootImpfpfeil() {
        FightRules.shoot(0,firePoint,bulletForce);
    }

    void ShootBetaeubung(){
        if(specialBullets>0)
        {
            specialBullets--;
            FightRules.shoot(1,firePoint,bulletForce);
        }
    }

    void ShootMega()
    {
        FightRules.shoot(3,firePoint,bulletForce);
    }
    void Shoot(Vector2 force, Vector3 positionSpawn)
    {
        FightRules.shoot(0,positionSpawn,force,bulletForce,gameObject);
    }
    public void addKOBullets(int count)
    {
        specialBullets+=count;
    }
    private void ShootTest()
    {
        float distanceSpawnPoint=0.5f;
        Vector2 a=firePoint.up;//vector straight shoot
        float radian=(Mathf.PI/180)*30;//<90!
        Vector3 b1=new Vector3(0,0,0);//vector angle° right
        Vector3 b2=new Vector3(0,0,0);//vector angle° left
        float p=((-2)*a.sqrMagnitude*Mathf.Cos(radian)*a.y)/(a.y*a.y+a.x*a.x);
        float q=(a.sqrMagnitude*a.sqrMagnitude*Mathf.Cos(radian)*Mathf.Cos(radian)-a.sqrMagnitude*a.x*a.x)/(a.y*a.y+a.x*a.x);
        b1.y=-(p/2)+Mathf.Sqrt((p/2)*(p/2)-q);
        b1.x=calculateB1(a,radian,b1.y);
        b2.y=-(p/2)-Mathf.Sqrt((p/2)*(p/2)-q);
        b2.x=calculateB1(a,radian,b2.y);
        Vector3 positionSpawn=gameObject.transform.position+firePoint.up*distanceSpawnPoint;
        Shoot(firePoint.up,positionSpawn);
        positionSpawn=gameObject.transform.position+b1*distanceSpawnPoint;
        Shoot(b1,positionSpawn);
        positionSpawn=gameObject.transform.position+b2*distanceSpawnPoint;
        Shoot(b2,positionSpawn);
    }
    private float calculateB1(Vector2 a, float radian, float b2)
    {
        return (a.sqrMagnitude*Mathf.Cos(radian)-a.y*b2)/a.x;
    }
    
    IEnumerator coShooting()
    {
        while(true)
        {
            if(nextShoot==1) {
                ShootImpfpfeil();
                yield return new WaitForSeconds(1f);
            }
            if(nextShoot==2)
            {
                ShootBetaeubung();
                yield return new WaitForSeconds(3f);
            }
            if(nextShoot==3)
            {
                yield return new WaitForSeconds(5f);
                ShootMega();
            }
            nextShoot=0;
            yield return null;
        }
    }
}
