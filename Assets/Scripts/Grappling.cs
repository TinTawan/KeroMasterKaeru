using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Grappling : MonoBehaviour
{
    private PlayerController pc;
    public Transform cam;
    public Transform tongueTip;
    public LayerMask grappleable;
    public LineRenderer lr;

    public float maxGrappleDist;
    public float grappleDelay;

    private Vector3 grapplePoint;

    public float grapplingCd;
    private float grapplingCdTimer;

    public KeyCode grappleKey = KeyCode.Mouse0;

    private bool grappling;

    private bool hitGrappleObject = false;
    private bool caughtFly = false;

    //moving player with grapple
    [SerializeField] private float grapplingTime = 3f;  //time from hitting grapple object to being pulled to it
    private float lerpPos;  
    private Vector3 grapplePos;

    [SerializeField] private Image cursor;

    AudioManager am;

    


    private void Start()
    {
        pc = GetComponent<PlayerController>();
        am = FindObjectOfType<AudioManager>();

    }

    private void Update()
    {
        //initialize grapple on left mouse click
        if (Input.GetKeyDown(grappleKey) && !pc.thirdCamActive)
        {
            StartGrapple();
        }
        //start counting down timer if cooldown is above 0
        if(grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;


        //if the player has hit the grapple object, they will be lerped to it
        if (hitGrappleObject)
        {
            GrapplePlayer(grapplePos);

            pc.SetYVel(0f);
        }
        else //otherwise, lerpPos is reset
        {
            lerpPos = 0f;
        }


        GrappleInRange();
    }

     private void LateUpdate()
     {
        //set start position to where tongue tip is
        if(grappling)
            lr.SetPosition(0, tongueTip.position);
     }

    private void StartGrapple()
    {
        //check if timer is not 0 and return function
        if (grapplingCdTimer > 0) return;

        grappling = true;

        //use raycast to check if grapple hit 
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDist, grappleable))
        {
            grapplePoint = hit.point;


            //checks tag to see if fly or grappleable object has been hit
            if (hit.collider.CompareTag("Fly"))
            {
                //add a fly and 5 health if player doesn't have max health
                Manager.AddFlies(1);
                if(Manager.health < 100)
                {
                    Manager.AddHealth(2);
                }

                Destroy(hit.collider.gameObject);
                caughtFly = true;

                am.StartAudio(AudioManager.Sound.catchFly, transform.position, .07f);
            }
            else if (hit.collider.CompareTag("GrappleObject"))
            {
                //set the position to grapple to as the point the raycast hit + 1 on the y as this will be outside the collider and therefore no weird glitching inside
                grapplePos = new Vector3(hit.point.x, hit.transform.localPosition.y + 1f, hit.point.z);
                hitGrappleObject = true;

                am.StartAudio(AudioManager.Sound.grapple, transform.position, .06f);
            }
            else //start end grapple function if player hits something else
            {
                Invoke(nameof(EndGrapple), grappleDelay);
                
            }

            Invoke(nameof(ExecuteGrapple), grappleDelay);
            am.StartAudio(AudioManager.Sound.tongue, transform.position, .08f);
        }
        else
        {
          grapplePoint = cam.position + cam.forward * maxGrappleDist;

          Invoke(nameof(EndGrapple), grappleDelay);
          am.StartAudio(AudioManager.Sound.miss, transform.position, .08f);
        }
        //enable line renderer and set grapple point as end position
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);


      
    }

    private void ExecuteGrapple()
    {

        lr.enabled= false;

    }

    private void EndGrapple()
    {
        //disable grappling function and start cooldown timer
        grappling = false;

        grapplingCdTimer = grapplingCd;

        //disable line renderer
        lr.enabled = false;
    }

    //Move the player towards the grapple point they hit
    private void GrapplePlayer(Vector3 hitLocation)
    {

        if(lerpPos < 1)
        {
            pc.transform.position = Vector3.Lerp(pc.gameObject.transform.position, hitLocation, lerpPos);

            lerpPos += Time.deltaTime * grapplingTime;

        }
        else
        {
            pc.transform.position = hitLocation;
        }

    }

    void GrappleInRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            Color lightBlue = new Color(0f, 0.6392157f, 1f);

            //if object is within grapple range
            if (hit.distance <= maxGrappleDist)
            {
                //if aiming at fly
                if (hit.transform.gameObject.CompareTag("Fly"))
                {
                    //turn cursor green
                    cursor.color = Color.green;
                }
                //if aiming at grapple point
                else if (hit.transform.gameObject.CompareTag("GrappleObject"))
                {
                    cursor.color = lightBlue;
                }
                else
                {
                    //otherwise turn cursor white
                    cursor.color = Color.white;
                }


            }
            else
            {
                //otherwise turn cursor white
                cursor.color = Color.white;
            }

        }
        else
        {
            //if raycast hits nothing turn cursor white
            cursor.color = Color.white;
        }
    }


    public bool GetHitGrappleObject()
    {
        return hitGrappleObject;
    }
    public void SetHitGrappleObject(bool hitGrapple)
    {
        hitGrappleObject = hitGrapple;
    }

    public float GetGrappleCountDownTimer()
    {
        return grapplingCdTimer;
    }

    public bool GetGrapplingBool()
    {
        return grappling;
    }
    public bool GetFlyCaught()
    {
        return caughtFly;
    }
}
