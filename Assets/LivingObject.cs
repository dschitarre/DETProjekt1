using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingObject : MonoBehaviour
{
    public int leben;

    public bool infiziert;

    public abstract bool immobile();
}
