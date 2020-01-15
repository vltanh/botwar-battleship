using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public float normalMoveSpeed = 10;
    public float smoothTime = 10f;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) { transform.position += Vector3.up * normalMoveSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.DownArrow)) { transform.position -= Vector3.up * normalMoveSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.LeftArrow)) { transform.Rotate(Vector3.right, normalMoveSpeed * Time.deltaTime); }
        if (Input.GetKey(KeyCode.RightArrow)) { transform.Rotate(Vector3.right, -normalMoveSpeed * Time.deltaTime); }

    }
}
