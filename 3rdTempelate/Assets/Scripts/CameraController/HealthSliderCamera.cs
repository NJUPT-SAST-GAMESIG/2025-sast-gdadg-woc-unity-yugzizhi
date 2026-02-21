using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSliderCamera : MonoBehaviour
{
    [SerializeField] private Transform healthSliderCamera; 
   
    void LateUpdate()
    {
        transform.LookAt(transform.position + healthSliderCamera.transform.forward);
    }
}
