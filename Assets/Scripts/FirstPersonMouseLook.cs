using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMouseLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController pc;
    [SerializeField] private Transform firstPCam;
    [SerializeField] private SkinnedMeshRenderer frogMesh;


    [Header("Inputs")]
    //mouse sensitivity
    [SerializeField] private float mouseSens = 50f;


    //float for x rotation
    private float xRo = 0f;


    void Update()
    {
        if (!pc.thirdCamActive)
        {
            //set the cam as a child of the player
            transform.parent = player.transform;

            //float values for the mouse x and y
            float MouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float MouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

            //decrement x rotation with the value for mouse y (increment would flip the direction)
            xRo -= MouseY;
            //clamp value between -70 and 70 degrees to stop cam from rotating too far
            xRo = Mathf.Clamp(xRo, -70f, 70f);
            //rotate cam with x rotation value 
            firstPCam.localRotation = Quaternion.Euler(xRo, 0f, 0f);

            //rotate the player with the mouse's x rotation value around the y axis
            player.Rotate(Vector3.up * MouseX);

            //frog wont show up when in first person to stop camera clipping
            frogMesh.enabled = false;
        }
        else
        {
            frogMesh.enabled = true;
        }

    }
}
