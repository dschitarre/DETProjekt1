using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Impfbar : LivingObject
{
    public Renderer renderer;

    public Rigidbody2D rigidbody;
    public int distanceInfizierung=50;
    public bool geimpft = false;

    public bool politiker=false;  
    public bool Impfgegner=false;

    public bool wuetend=false;

    public int koBulletsFromPolitics=10;

    public static float timeBetweenHusten=1f;

    private int timeToNextHusten=0;
    
    private static Color colorImpfgegner=new Color(1f, 0.92f, 0.016f, 1f);

    private static Color colorInfiziert=Color.red;

    private static Color colorWuetend=new Color(1f, 0f, 1f, 1f);

    private Color colorGeheilt=Color.blue;

    private Color colorNormal=new Color(0f,1f,0f,1f);

    private static Player player;

    private static System.Random  random;
    // Start is called before the first frame update
    void Start()
    {
        leben=1;
        if(player==null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            player=playerObject.GetComponent<Player>();
            if(player==null)
            {
                Debug.LogError("No Player");
                if(playerObject==null)
                {
                    Debug.Log("Also Player not found by Tag");
                }
                Destroy(gameObject);
                return;
            }
        }
        if(rigidbody==null)
        {
            Debug.LogError("No rigidbody");
            Destroy(gameObject);
            return;
        }
        if(random==null)
        {
            random=new System.Random();
        }
        if(random.Next(10)<=1)
        {
            Impfgegner=true;
            addSchlagstock();
        }
        if(random.Next(10)<=1)
        {
            infiziert=true;
        }
        if(random.Next(10)<=1)
        {
            politiker=true;
        }
        setColor();
        StartCoroutine(FightRules.coHusten(this,rigidbody,timeBetweenHusten));
    }
    private void setColor()
    {
        Color nextColor;
        if(Impfgegner)
        {
            if(wuetend)
            {
                nextColor=colorWuetend;
            }
            else
            {
                nextColor=colorImpfgegner;
            }
        }
        else if(infiziert)
        {
            nextColor=colorInfiziert;
        }
        else if(geimpft)
        {
            nextColor=colorGeheilt;
        }
        else
        {
            nextColor=colorNormal;
        }
        renderer.material.SetColor("_Color", nextColor);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other) { 
        if(other.CompareTag("Bullet")||other.CompareTag("WaffeGegner"))
        {
            bullet bullet=other.GetComponent<bullet>();
            if(bullet==null)
            {
                return;
            }
            if(bullet.typ==0)
            {
                if(!geimpft)
                {
                    geimpft=true;
                    infiziert=false;
                    if(Impfgegner)
                    {
                        wuetend=true;
                    }
                    else if(politiker)
                    {
                        player.addKOBullets(koBulletsFromPolitics);
                    }
                    setColor();
                }
            }
            else if(bullet.typ==1)
            {
                ruhigStellen(1f);   
            }
            else if(bullet.typ==2)
            {
                if(!infiziert)
                {
                    infizieren();
                }
            }
            else if(bullet.typ==3)
            {
                Destroy(gameObject);
            }
        }

    }
    public void ruhigStellen(float seconds)
    {
        Movement movement=gameObject.GetComponent<Movement>();
        movement.ruhigStellen(seconds);
    }
    public void infizieren()
    {
        if(!geimpft)
        {
            infiziert=true;
            setColor();
        }
        
    }
    private void addSchlagstock()
    {
        Vector3 positionSpawn=gameObject.transform.position;
        positionSpawn.x-=0.6f;
         GameObject schlagstock = Instantiate(Game.waeponPrefabs[Game.SchlagstockNumber],positionSpawn , gameObject.transform.rotation);
        if(schlagstock==null)
        {
            Debug.Log("No schlagstock");
        }
         Schlagstock script=schlagstock.GetComponent<Schlagstock>();
         if(script==null)
         {
             Debug.Log("No script");
         }
         script.userRigidbody=rigidbody;
        Movement movement=gameObject.GetComponent<Movement>();
         script.speed=movement.moveSpeed*1.2f;
    }

    public override bool immobile()
    {
        return politiker;
    }
}
