using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FliesMovement : MonoBehaviour
{
    [SerializeField] float idleSpeed, turnSpeed, switchSeconds, idleRatio;
    [SerializeField] Vector2 animSpeedMinMax, moveSpeedMinMax, changeAnimEveryFromTo;
    [SerializeField] Vector2 changeTargetEveryFromTo;
    [SerializeField] Transform homeTarget, flyingTarget;
    [SerializeField] Vector2 radiusMinMax;
    [SerializeField] Vector2 yMinMax;

    //variation in base location so multiple flies don't return in the exact same spot
    [SerializeField] public bool returnToBase = false;
    [SerializeField] public float randomBaseOffset = 5, delayStart = 0f;

    private Animator anim;
    private Rigidbody rb;
    [System.NonSerialized] public float changeTarget = 0f, changeAnim = 0f, timeSinceTarget = 0f, timeSinceAnim = 0f, 
        prevAnim, currentAnim = 0f, prevSpeed, speed, zturn, prevz, turnSpeedBackup;
    
    private Vector3 rotateTarget, position, direction, velocity, randomizedBase;
    private Quaternion lookRotation;
    [System.NonSerialized] public float distanceFromBase, distanceFromTarget;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        turnSpeedBackup = turnSpeed;
        direction = Quaternion.Euler(transform.eulerAngles) * (Vector3.forward);
        if (delayStart < 0f) rb.velocity = idleSpeed * direction;
    }

    private void Update()
    {
        ChangeY();
    }


    void FixedUpdate()
    {
        //check for delay on start, wait for the delay 
        if(delayStart > 0f)
        {
            delayStart -= Time.fixedDeltaTime;
            return;
        }
        //calculate distances between flies and flying path waypoints
        distanceFromBase = Vector3.Magnitude(randomizedBase - rb.position);
        distanceFromTarget = Vector3.Magnitude(flyingTarget.position - rb.position);

        //allow sharp turns when close to target by increasing turning speed 
        if(returnToBase && distanceFromBase < 10f)
        {
            if(turnSpeed != 300f && rb.velocity.magnitude != 0f)
            {
                turnSpeedBackup = turnSpeed;
                turnSpeed = 300f;
            }
            //return original turning speed after target is reached
            else if(distanceFromBase <= 1f)
            {
                rb.velocity = Vector3.zero;
                turnSpeed = turnSpeedBackup;
                return;
            }
        }

        //change animation speed according to randmosied value 
        if(changeAnim < 0f)
        {
            prevAnim = currentAnim;
            currentAnim = ChangeAnim(currentAnim);
            //change animation at random times
            changeAnim = Random.Range(changeAnimEveryFromTo.x, changeAnimEveryFromTo.y);
            timeSinceAnim = 0f;
            //change speed of animation 
            prevSpeed = speed;
            //if active animation is idle, speed is idle
            if (currentAnim == 0) speed = idleSpeed;
            //else chose a value between minimum and maximum speed 
            else speed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, (currentAnim - animSpeedMinMax.x) / (animSpeedMinMax.y - animSpeedMinMax.x));
        }

        //change flying target
        if(changeTarget < 0f)
        {
            //pick new direction
            rotateTarget = ChangeDirection(rb.transform.position);
            //check if fly is returning to original waypoint
            if (returnToBase) changeTarget = 0.2f;
            //if it isn't, change to a new random target
            else changeTarget = Random.Range(changeTargetEveryFromTo.x, changeTargetEveryFromTo.y);
            timeSinceTarget = 0f;
        }

        //apply minimum and maximum flight height
        if(rb.transform.position.y < yMinMax.x + 10f ||
                rb.transform.position.y > yMinMax.y - 10f)
        {
            //tilt up or down when reaching extremes
            if (rb.transform.position.y < yMinMax.x + 10f) rotateTarget.y = 1f;
            else rotateTarget.y = -1f;
        }

        //calculate necessary tilting angle according to fly's rotation angle
        zturn = Mathf.Clamp(Vector3.SignedAngle(rotateTarget, direction, Vector3.up), -45f, 45f);

        changeAnim -= Time.fixedDeltaTime;
        changeTarget -= Time.fixedDeltaTime;
        timeSinceTarget += Time.fixedDeltaTime;
        timeSinceAnim += Time.fixedDeltaTime;

        //rotate fly towards flying waypoint
        if (rotateTarget != Vector3.zero) lookRotation = Quaternion.LookRotation(rotateTarget, Vector3.up);
        //increment turning for more natural rotation
        Vector3 rotation = Quaternion.RotateTowards(rb.transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime).eulerAngles;
        rb.transform.eulerAngles = rotation;

        //apply tilt (with smoothing)
        float temp = prevz;
        if (prevz < zturn) prevz += Mathf.Min(turnSpeed * Time.fixedDeltaTime, zturn - prevz);
             else if (prevz >= zturn) prevz -= Mathf.Min(turnSpeed * Time.fixedDeltaTime, prevz - zturn);
        prevz = Mathf.Clamp(prevz, -45f, 45f);

        rb.transform.Rotate(0f, 0f, prevz - temp, Space.Self);
        //check if landing (idle state) and gradually reduce speed 
        if(returnToBase && distanceFromBase < idleSpeed)
        {
            rb.velocity = Mathf.Min(idleSpeed, distanceFromBase) * direction;
        }

        //find flying direction acording to rotation angle of the fly
        direction = Quaternion.Euler(transform.eulerAngles) * Vector3.forward;
        //apply this direction as forward movement of the fly (with smoothing)
        rb.velocity = Mathf.Lerp(prevSpeed, speed, Mathf.Clamp(timeSinceAnim / switchSeconds, 0f, 1f)) * direction;

        if (rb.transform.position.y < yMinMax.x || rb.transform.position.y > yMinMax.y)
        {
            position = rb.transform.position;
            position.y = Mathf.Clamp(position.y, yMinMax.x, yMinMax.y);
            rb.transform.position = position;
        }
    }

    //randomise animation
    private float ChangeAnim(float currentAnim)
    {
        //pick random value for animation speed 
        float newState;
        if (Random.Range(0f, 1f) < idleRatio) newState = 0f;
        else
        {
            newState = Random.Range(animSpeedMinMax.x, animSpeedMinMax.y);
        }

        //change it in the animator as well adn return new animation value as current animation value
        if(newState != currentAnim)
        {
            anim.SetFloat("flySpeed", newState);
            if (newState == 0) anim.speed = 1f;
            else anim.speed = newState;
        }
        return newState;
    }

    //randomise direction of flight
    private Vector3 ChangeDirection(Vector3 currentPosition)
    {
        Vector3 newDir;

        //bring fly back to a random landing spot
        if (returnToBase)
        {
            randomizedBase = homeTarget.position;
            randomizedBase.y += Random.Range(-randomBaseOffset, randomBaseOffset);
            newDir = randomizedBase - currentPosition;
        }
        //check distance between fly and target  
        else if (distanceFromTarget > radiusMinMax.y)
        {
            //if larger than maximum flying radius, fly towards the target
            newDir = flyingTarget.position - currentPosition;
        }
        else if (distanceFromTarget < radiusMinMax.x)
        {
            //if lower than minimum, change flying drection to new target
            newDir = currentPosition - flyingTarget.position;
        }
        else
        {
            //unlimited options for new target on horisontal axis
            float angleXZ = Random.Range(-Mathf.PI, Mathf.PI);
            //limited options on vertical axis
            float angleY = Random.Range(-Mathf.PI / 40f, Mathf.PI / 40f);

            //calculate new target's direcion
            newDir = Mathf.Sin(angleXZ) * Vector3.forward + Mathf.Cos(angleXZ) * Vector3.right + Mathf.Sin(angleY) * Vector3.up;
        }

        return newDir.normalized;
    }

    void ChangeY()
    {
        if(Manager.tutorialComplete)
        {
            yMinMax = new Vector2(5, 70);
        }
        else
        {
            yMinMax = new Vector2(-7, 10);
        }
    }

    void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
