using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSprintJoint;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay = 0.15f;
    [SerializeField] private float respawnDelay = 0.25f;

    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {   
        if (currentBallRigidbody == null) { return; }

        // check if finger is not on the screen
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        {   
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            //we want physics control on the ball now that player let go
            currentBallRigidbody.isKinematic = false;
            return;
        }

        isDragging = true;
        
        //this means that the ball is taken out of physics control and is kinematic means it moves iwth our finger
        currentBallRigidbody.isKinematic = true;

        //doesn't work as expected in unity simulator. this returns -infinity, infinity:
            
            // screen space - touch position in terms of pixels on your screen.
            //ReadValue will return a Vector2
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            // Debug.Log(touchPosition);
        
        
        // world space - position in terms of units inside the game world
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        // Debug.Log(worldPosition);
        currentBallRigidbody.position = worldPosition;

    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSprintJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        //make the ball react to physics again
        currentBallRigidbody.isKinematic = false;

        //clear the rigidbody so we can't reset the position by touching the screen again
        currentBallRigidbody = null;
        
        //call the DetachBall method after detachDelay time
        //by using nameof, the error will have a reference to the DetachBall method
        //nameof returns a string
        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        //turn off spring joint so it won't hold the ball anymore
        //this way the ball gets flung to the right of the screen instead of bouncing back because it's being held by the spring joint
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

}