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
    public static float timeBetweenHusten=3f;

    private int timeToNextHusten=0;
    
    private static Color colorImpfgegner=Color.white;

    private static Color colorInfiziert=Color.red;

    private static Color colorWuetend=(Color.red + Color.white) / 2;

    private Color colorGeheilt=new Color(0.1f,0.7f,0.1f,1f);

    private Color colorNormal=new Color(0.65f,0.65f,0.65f,1f);

    private static Player player;

    private static System.Random  random;

    private Schlagstock meinSchlagstock;//Schlagstock of Impfgegner or null

    // Start is called before the first frame update
    void Awake()
    {
        if(random==null)
        {
            random=new System.Random();
        }
    }
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
        setColor();
        StartCoroutine(FightRules.coHusten(this,new System.Random(), rigidbody, timeBetweenHusten, 0.5f));
    }
    void OnDestroy()
    {
        Game.Instance.anzahlNormalos--;
        if(infiziert)
        {
            Game.Instance.anzahlInfizierte--;
        }
        if(geimpft)
        {
            Game.Instance.anzahlGeimpfte--;
        }
    }
    public void werdeImpfgegner()
    {
        Impfgegner=true;
        addSchlagstock();
        Game.Instance.SetTexture(gameObject, random.NextDouble() > 0.5 ? "attila_hildmann" : "xavier_naidoo");
    }
    public void werdePolitiker(int nr)
    {
         politiker=true;
        string[] politikerNamen = {"karl_lauterbach", "angela_merkel", "armin_laschet", "markus_soeder"};
        Game.Instance.SetTexture(gameObject, politikerNamen[nr]);
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
                    geimpftWerden();
                    
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
                if(meinSchlagstock!=null)
                {
                    Destroy(meinSchlagstock.gameObject);
                }
            }
        }

    }
    public void geimpftWerden()
    {
        if(!geimpft)
        {
            geimpft=true;
            Game.Instance.anzahlGeimpfte++;
            if(infiziert)
            {  
                Game.Instance.anzahlInfizierte--;
                infiziert=false;
            }
            if(Impfgegner)
            {
                wuetendWerden();
            }
            else if(politiker)
            {
                player.addBullets(Game.Instance.Settings.impfDosenVonPolitikern,Game.Instance.Settings.koBulletsVonPolitikern,Game.Instance.Settings.raketenVonPolitiker);
            }
            setColor();
        }
    }
    public void wuetendWerden()
    {
        wuetend=true;
        float playerSpeed=player.getMovementSpeed();
        Movement movement=gameObject.GetComponent<Movement>();
        movement.setMovementSpeed(playerSpeed);
        meinSchlagstock.speed=playerSpeed*1.2f;
    }
    public void ruhigStellen(float seconds)
    {
        Movement movement=gameObject.GetComponent<Movement>();
        movement.ruhigStellen(seconds);
    }
    public void infizieren()
    {
        if(!geimpft&&!infiziert)
        {
            infiziert=true;
            Game.Instance.anzahlInfizierte++;
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
         meinSchlagstock=script;
        Movement movement=gameObject.GetComponent<Movement>();
         script.speed=movement.moveSpeed*1.2f;
    }

    public override bool immobile()
    {
        return politiker;
    }
}
