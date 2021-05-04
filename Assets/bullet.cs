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
        if(!other.gameObject.TryGetComponent<bullet>(out var a)&&!other.gameObject.TryGetComponent<Schlagstock>(out var b))
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if(!collision.gameObject.TryGetComponent<bullet>(out var a)&&!collision.gameObject.TryGetComponent<Schlagstock>(out var b))
        {
            Destroy(gameObject);
        }
    }
}

