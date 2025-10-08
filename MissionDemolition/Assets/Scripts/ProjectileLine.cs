using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileLine : MonoBehaviour
{
    private LineRenderer _line;
    private bool _drawing = true;
    private GameObject _projectile; // Reference to the projectile GameObject
    private Vector3 _lastPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);
        _lastPosition = transform.position;
    }
    
    // Method to set the projectile reference
    public void SetProjectile(GameObject projectile)
    {
        _projectile = projectile;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_drawing && _projectile != null)
        {
            // Update the line position to follow the projectile
            transform.position = _projectile.transform.position;
            
            // Add a new point to the line if the projectile has moved significantly
            Vector3 currentPos = _projectile.transform.position;
            if (Vector3.Distance(currentPos, _lastPosition) > 0.1f)
            {
                _line.positionCount++;
                _line.SetPosition(_line.positionCount - 1, currentPos);
                _lastPosition = currentPos;
            }
            
            // Check if projectile is sleeping
            Rigidbody rb = _projectile.GetComponent<Rigidbody>();
            if (rb != null && rb.IsSleeping())
            {
                _drawing = false;
                _projectile = null;
            }
        }
    }
}
