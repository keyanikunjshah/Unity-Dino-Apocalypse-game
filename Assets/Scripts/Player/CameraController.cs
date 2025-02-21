using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public float sensitivity = 2f;
    public float minXangle = -30f;
    public float maxXangle = 30f;

    public float minYangle = -360f;
    public float maxYangle = 360f;
    public float smoothspeed = 10f;

    private float rotationX = 0f;
    private float rotationY = 0f;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X");

        float mouseY = Input.GetAxis("Mouse Y");
        rotationX -= mouseY;
        rotationY += mouseX;

        rotationX = Mathf.Clamp(rotationX, minXangle, maxXangle);

        rotationY = Mathf.Clamp(rotationY, minYangle, maxYangle);

        Quaternion targetroation = Quaternion.Euler(rotationX, rotationY, 0);

        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetroation, smoothspeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetroation, smoothspeed * Time.deltaTime);



    }
}
