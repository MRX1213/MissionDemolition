using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject launchPoint;
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public int shotsTaken;
    
    // Camera follow settings
    [Header("Camera Follow Settings")]
    public Camera followCamera;
    public float followSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 5, -10);
    public bool followProjectile = false;
    
    // Camera toggle settings
    [Header("Camera Toggle Settings")]
    public bool cameraToggleMode = false; // false = follow ball, true = focus on slingshot
    public Vector3 slingshotCameraOffset = new Vector3(0, 5, -10);
    public float cameraTransitionSpeed = 3f;
    public KeyCode toggleKey = KeyCode.C; // Press C to toggle camera
    
    // Projectile cleanup settings
    [Header("Projectile Cleanup")]
    public float destroyYThreshold = -20f; // Destroy projectiles below this Y position
    // fields set dynamically
    [Header("Set Dynamically")]
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Vector3 originalCameraPosition;
    private GameObject projectileLine;
    
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
        
        // Set up camera reference if not assigned
        if (followCamera == null)
        {
            followCamera = Camera.main;
        }
        
        // Store original camera position
        if (followCamera != null)
        {
            originalCameraPosition = followCamera.transform.position;
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
        
        followProjectile = true;
        cameraToggleMode = false; // Switch to ball follow mode
        Debug.Log("Aiming started - camera now following ball");

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
            
            // Create projectile line when projectile is fired
            if (projLinePrefab != null)
            {
                projectileLine = Instantiate(projLinePrefab) as GameObject;
                projectileLine.transform.position = projectile.transform.position; // Start at projectile position
                
                // Set up the line to follow the projectile
                ProjectileLine lineScript = projectileLine.GetComponent<ProjectileLine>();
                if (lineScript != null)
                {
                    lineScript.SetProjectile(projectile); // Tell the line which projectile to follow
                    Debug.Log("Projectile line created and following projectile");
                }
            }
            else
            {
                Debug.LogWarning("ProjLinePrefab is not assigned - no line will be drawn");
            }
            
            // Camera is already following from OnMouseDown, just ensure it stays in ball follow mode
            cameraToggleMode = false; // Ensure we stay in ball follow mode
            Debug.Log("Shot fired - camera continues following ball");

            MissionDemolition.S.ShotFired();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handle camera toggle input
        if (Input.GetKeyDown(toggleKey))
        {
            cameraToggleMode = !cameraToggleMode;
            Debug.Log($"Camera mode: {(cameraToggleMode ? "Slingshot View" : "Ball Follow")}");
        }
        
        // Camera follows ball until it's destroyed or user toggles camera mode
        // No automatic stopping based on velocity
        
        // Camera follow logic
        if (followCamera != null)
        {
            Vector3 targetPosition;
            
            if (cameraToggleMode)
            {
                // Focus on slingshot
                targetPosition = transform.position + slingshotCameraOffset;
                if (Time.frameCount % 60 == 0) Debug.Log("Camera: Slingshot view");
            }
            else if (followProjectile && projectile != null)
            {
                // Follow the ball
                targetPosition = projectile.transform.position + cameraOffset;
                if (Time.frameCount % 60 == 0) Debug.Log($"Camera: Following ball at {projectile.transform.position}");
            }
            else
            {
                // Default position (original camera position)
                targetPosition = originalCameraPosition;
                if (Time.frameCount % 60 == 0) Debug.Log("Camera: Default position");
            }
            
            // Smoothly move camera towards target position
            followCamera.transform.position = Vector3.Lerp(
                followCamera.transform.position, 
                targetPosition, 
                cameraTransitionSpeed * Time.deltaTime
            );
            
            // Check if projectile has fallen below threshold and destroy it
            if (projectile != null && projectile.transform.position.y < destroyYThreshold)
            {
                Debug.Log($"Projectile destroyed at Y position: {projectile.transform.position.y}");
                Destroy(projectile);
                projectile = null;
                followProjectile = false; // Stop following when ball is destroyed
                
                // Also destroy the projectile line
                if (projectileLine != null)
                {
                    Destroy(projectileLine);
                    projectileLine = null;
                }
            }
        }
    }
    
    // Check if the projectile is sleeping (not moving)
    private bool IsProjectileSleeping()
    {
        if (projectile == null) return false;
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null) return false;
        
        // Check if the rigidbody is sleeping
        if (rb.IsSleeping())
        {
            return true;
        }
        
        // Alternative check: if velocity is very low (for cases where IsSleeping() might not work as expected)
        float velocityThreshold = 0.1f;
        if (rb.velocity.magnitude < velocityThreshold && rb.angularVelocity.magnitude < velocityThreshold)
        {
            return true;
        }
        
        return false;
    }
    
    // Public method to toggle camera (can be called from UI button)
    public void ToggleCamera()
    {
        cameraToggleMode = !cameraToggleMode;
        Debug.Log($"Camera mode: {(cameraToggleMode ? "Slingshot View" : "Ball Follow")}");
    }
    
    // Public method to set camera to slingshot view
    public void SetSlingshotView()
    {
        cameraToggleMode = true;
        Debug.Log("Camera set to Slingshot View");
    }
    
    // Public method to set camera to ball follow view
    public void SetBallFollowView()
    {
        cameraToggleMode = false;
        Debug.Log("Camera set to Ball Follow View");
    }
}
