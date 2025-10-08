using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    static public bool goalMet = false;
    
    void OnTriggerEnter(Collider other)
    {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null)
        {
            goalMet = true;
            Material mat = GetComponent<Renderer>().material;
            Color color = mat.color;
            color.a = 0.80f;
            mat.color = color;
        }
    }


}