using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Shooting shooting;

    private PlayerMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        shooting=GetComponent<Shooting>();
        movement=GetComponent<PlayerMovement>();

        Game.Instance.SetTexture(gameObject, "player");    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addBullets(int impfDosen, int koBullets, int raketen)
    {
        Debug.Log(impfDosen+","+koBullets+","+raketen);
        shooting.addBullets(impfDosen,koBullets,raketen);
    }
    public float getMovementSpeed()
    {
        return movement.moveSpeed;
    }
}
