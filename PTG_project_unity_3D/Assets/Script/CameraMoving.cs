using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.dKey.isPressed)
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            transform.position += Vector3.forward * Time.deltaTime * speed;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            transform.position += Vector3.back * Time.deltaTime * speed;
        }
        if (Keyboard.current.qKey.isPressed)
        {
            transform.position += Vector3.up * Time.deltaTime * speed;
        }
        if (Keyboard.current.eKey.isPressed)
        {
            transform.position += Vector3.down * Time.deltaTime * speed;
        }
    }
}
