using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : LivingObject
{
    public float moveSpeed;
    public Rigidbody2D rigidbody;
    public Camera cam;
    private Vector2 moveDirection;
    private Vector2 mousePos;
    public string currentCell = "0,0";
    // Update is called once per frame
    void Start(){
        leben=100;
        infiziert=false;
    }
    void Update() {
        processInputs();
    }

    void FixedUpdate() {
        MoveCamera();
        Move();
    }

    void MoveCamera() {
        Vector2 relPos = 0.04f * (gameObject.transform.position - cam.transform.position);
        cam.transform.position += (Vector3) relPos;
    }

    void processInputs() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY);

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void Move() {
        Labyrinth.Instance.GetTagFromPos(gameObject.transform.position, out currentCell);

        rigidbody.velocity = new Vector2(moveDirection.x, moveDirection.y).normalized * moveSpeed;

        Vector2 lookDir = mousePos - rigidbody.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rigidbody.rotation = angle;
    }
    void OnTriggerEnter2D(Collider2D other) { 
        if(Corona(other.gameObject))
        {
            FightRules.takeDemage(5,this);
        }
        if(Schlagstock(other.gameObject))
        {
            
            if(!FightRules.takeDemage(10,this))
            {
                Vector3 positionPlayer=transform.position;
                Vector3 positionOther= other.gameObject.transform.position;
                Vector3 flug=(positionPlayer-positionOther);
                rigidbody.MovePosition(rigidbody.position+new Vector2(flug.x,flug.y));
            }
        } 
        if(BossAttack(other.gameObject))
        {
            Destroy(gameObject);
        }
    }
    /*
    returns whether the player dies or not
    */
    
    void OnCollisionEnter2D(Collision2D collision) {
         
    }
    bool Schlagstock(GameObject other)
    {
        if(!other.CompareTag("WaffeGegner"))
        {
            return false;
        }
        Schlagstock schlagstock=other.GetComponent<Schlagstock>();
        return schlagstock!=null;
    }
    bool Corona(GameObject other)
    {
        if(!other.CompareTag("WaffeGegner"))
        {
            return false;
        }
        bullet bullet=other.GetComponent<bullet>();
        if(bullet==null)
        {
            return false;
        }
        return bullet.typ==2;
    }
    bool BossAttack(GameObject other)
    {
        bullet bullet=other.GetComponent<bullet>();
        if(bullet==null)
        {
            return false;
        }
        return bullet.typ==4;
    }
    public override bool immobile()
    {
        return false;
    }
}