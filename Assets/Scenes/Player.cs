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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addKOBullets(int count)
    {
        shooting.addKOBullets(count);
    }
    public float getMovementSpeed()
    {
        return movement.moveSpeed;
    }
}
