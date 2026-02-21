using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraControl : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float height;

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 newTransform = playerTransform.position;
            newTransform.y = height;
            transform.position = newTransform;
        }
    }
}
