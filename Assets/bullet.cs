using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int typ=0;//typ 0:Impfung, typ1:Betaeubung, typ2: Corona, typ3: Mega-Schuss, typ4: BossAngriff, (typ5:Schlagstock)
     void Start()
    {
      
    }
    void OnTriggerEnter2D(Collider2D other) { 
       Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }
}

