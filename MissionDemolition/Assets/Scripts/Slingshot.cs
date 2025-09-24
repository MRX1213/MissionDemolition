using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject launchPoint;
    public GameObject projectilePrefab;
    public float velocityMult = 10f;

    // fields set dynamically
    [Header("Set Dynamically")]
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    
    // Start is called before the first frame update
    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        if (launchPointTrans != null)
        {
            launchPoint = launchPointTrans.gameObject;
            launchPoint.SetActive(false);
            launchPos = launchPointTrans.position;
        }
        else
        {
            Debug.LogError("LaunchPoint child not found!");
        }
    }

    void OnMouseEnter()
    {
        print("Mouse Entered");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        print("Mouse Exited");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown() 
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectilePrefab is not assigned!");
            return;
        }

        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnMouseDrag()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
    }

    void OnMouseUp()
    {
        if (!aimingMode) return;

        aimingMode = false;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            
            // Calculate velocity based on how far the projectile was dragged
            Vector3 projectilePos = projectile.transform.position;
            Vector3 velocity = (launchPos - projectilePos) * velocityMult;
            rb.velocity = velocity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
