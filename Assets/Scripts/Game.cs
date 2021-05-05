using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Game : MonoBehaviour
{
    
    public int anzahlInfizierte=0;

    public int anzahlGeimpfte=0;

    public int anzahlNormalos=0;
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static Game Instance { get; private set; }
    private GameObject normaloPrefab;
    private GameObject bossPrefab;
    private Labyrinth lab;


    /// <summary>
    /// The game settings.
    /// </summary>
    public GameSettings Settings { get; private set; }

    //the prefabs of all weapons

    public static GameObject[] waeponPrefabs;

    public static readonly int SchlagstockNumber=5;
    ///zeigt auf erstes freies Feld vom Array normalos
    private System.Random random = new System.Random();
    private void Awake()
    {
        // there can be only one...
        if (Instance)
        {
            Debug.LogError("only one Level instance allowed");
            Destroy(gameObject); // exercise: what would be different if we used Destroy(this) instead?
            return;
        }
        else
        {
            Instance = this;
            Debug.Log("registered Level instance", Instance);
        }
        loadWeapons();
        // load settings
        Settings = GameSettings.Load();
        
        lab = ScriptableObject.CreateInstance<Labyrinth>();
        lab.SetSettings(Settings);
        lab.Generate();

        normaloPrefab = AssetDatabase.LoadAssetAtPath("Assets/Scenes/Normalo.prefab", typeof(GameObject)) as GameObject;
        bossPrefab = AssetDatabase.LoadAssetAtPath("Assets/Scenes/Boss.prefab", typeof(GameObject)) as GameObject;

        int numberOfNormalos = (int) ((float) Settings.size * (float) Settings.size * Settings.personDensity);
        
        anzahlNormalos=numberOfNormalos;

        int numberOfInfected = (int) Mathf.Ceil(numberOfNormalos * Settings.probInfected);

        int numberOfImpfgegner = (int) Mathf.Ceil(numberOfNormalos * Settings.probImpfgegner);

        Debug.Log("NoN: "+numberOfNormalos+" , NOINF: " +numberOfInfected+" , NOIG: "+numberOfImpfgegner);
        for (int i = 0; i < numberOfNormalos; i++) {
            Vector2Int cell = new Vector2Int(random.Next(Settings.size), random.Next(Settings.size));

            Vector2 pos;
            lab.GetPosFromCell(cell.x, cell.y, out pos);
            GameObject normalo = Instantiate(normaloPrefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));

            Impfbar impfbar=normalo.GetComponent<Impfbar>();
            if(i<4)
            {
                impfbar.werdePolitiker(i);
            }
            else if(i-4<numberOfInfected)
            {
                impfbar.infizieren();
            }
            if(numberOfNormalos-numberOfImpfgegner<=i)
            {
                impfbar.werdeImpfgegner();
            }
            IEnumerator normaloCoroutine=normaloBehavior(normalo);
            StartCoroutine(normaloCoroutine);
        }
        GameObject boss = Instantiate(bossPrefab, lab.exitPos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    }
    public void loadWeapons()
    {
        waeponPrefabs=new GameObject[6];
        waeponPrefabs[0]=AssetDatabase.LoadAssetAtPath("Assets/CoronaSpritze.prefab", typeof(GameObject)) as GameObject;
        waeponPrefabs[1]=AssetDatabase.LoadAssetAtPath("Assets/KoSpritze.prefab", typeof(GameObject)) as GameObject;
        waeponPrefabs[2]=AssetDatabase.LoadAssetAtPath("Assets/Corona.prefab", typeof(GameObject)) as GameObject;
        waeponPrefabs[3]=AssetDatabase.LoadAssetAtPath("Assets/CanoneBall.prefab", typeof(GameObject)) as GameObject;
        waeponPrefabs[4]=waeponPrefabs[0];//bossBullet
        waeponPrefabs[5]=AssetDatabase.LoadAssetAtPath("Assets/Schlagstock.prefab", typeof(GameObject)) as GameObject;
        if(waeponPrefabs[0]==null)
        {
            Debug.LogError("Bullet not found");
        }
        if(waeponPrefabs[2]==null)
        {
            Debug.LogError("Covid not found");
        }
        if(waeponPrefabs[5]==null)
        {
            Debug.LogError("Schlagstock not found");
        }
    }
    public void SetTexture(GameObject obj, string name) {
        Texture2D tex = new Texture2D(500, 500);
        byte[] imageData = System.IO.File.ReadAllBytes("Assets/images/" + name + ".png");
        tex.LoadImage(imageData);
        Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 250.0f);
        obj.GetComponent<SpriteRenderer>().sprite = spr;
    }

    private IEnumerator normaloBehavior(GameObject normalo) {
        Vector2 posCurrent, posNext, posDest;
        string tagCurrent, tagNext, tagDest;

        Movement movement = normalo.GetComponent<Movement>();

        // initialize current
        posCurrent = normalo.transform.position;
        lab.GetTagFromPos(posCurrent, out tagCurrent);

        tagDest = tagCurrent;
        tagNext = tagCurrent;
        posDest = posCurrent;
        posNext = posCurrent;

        while (true) {
            yield return new WaitForFixedUpdate();
            if(movement==null)
            {
                break;
            }
            if (!movement.immobile()) {
                // update current position
                posCurrent = normalo.transform.position;
                lab.GetTagFromPos(posCurrent, out tagCurrent);
                
                if (movement.wuetend()) { // if normalo is wuetend set destination cell as player's cell
                    tagDest = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().currentCell;
                    posDest = GameObject.FindGameObjectWithTag("Player").transform.position;
                } else if (tagCurrent == tagDest) { // check if normalo is in destination cell
                    // set random destination cell
                    lab.GetRandomCellAtMaxDistance(tagCurrent, 1 + random.Next(4), out tagDest);
                    int x, y;
                    lab.GetIntsFromTag(tagDest, out x, out y);
                    lab.GetRandomPosInCell(x, y, out posDest);
                }

                if (movement.wuetend() && tagCurrent == tagDest) {
                    tagNext = tagDest;
                    posNext = posDest;
                } else {
                    // set next cell on the way to destination cell
                    if (lab.nextPosToMoveToOnWayFromTo(posCurrent, posDest, out posNext)) {
                        lab.GetTagFromPos(posNext, out tagNext);
                    }
                }

                // move into target direction
                normalo.GetComponent<Rigidbody2D>().velocity = (posNext - posCurrent).normalized * movement.moveSpeed;
            }
            yield return new WaitForSeconds((float) (0.75f + 0.5 * random.NextDouble()));
        }
    }

    private void Start()
    {
        //???
    }

    private void FixedUpdate() {

    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("unregistered Level instance", Instance);
        }
    }
}