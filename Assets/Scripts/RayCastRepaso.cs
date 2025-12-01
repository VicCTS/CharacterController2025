using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCastRepaso : MonoBehaviour
{
    InputAction _clickAction;
    InputAction _positionAction;
    Vector2 _mousePostion;

    void Awake()
    {
        _clickAction = InputSystem.actions["Attack"];
        _positionAction = InputSystem.actions["MousePosition"];
    }

    void Update()
    {
        _mousePostion = _positionAction.ReadValue<Vector2>();

        if(_clickAction.WasPerformedThisFrame())
        {
            ShootRaycast();
        }
    }

    private void ShootRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePostion);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.transform.gameObject.layer == 3)
            {
                
            }

            if(hit.transform.tag == "sjhdfgudsgh")
            {
                
            }

            if(hit.transform.name == "shjfbs")
            {
                
            }
        }
    }
}