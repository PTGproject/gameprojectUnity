using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    public Camera mainCamera;

        private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(mainCamera.transform);
    }
}