using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Target : MonoBehaviour
{
    [Tooltip("Drag here the target, which the current gameobject must follow.")]
    [SerializeField]
    private Transform targetToFollow;

    [Tooltip("If set to [0, 0, 0] the gameobject will precisely follow the target without any offset.")]
    [SerializeField]
    private Vector3 targetOffset;

    [Tooltip("If true, the gameobject will use the moveSpeed variable to smoothly transition towards the target - this will create a \"trying to catch up to the object\"effect. Set to false if the gameobject must immediately arrive at the location.")]
    [SerializeField]
    private bool lerpMovement = true;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private bool is_FollowEnabled = false;

    void Update()
    {
        if(is_FollowEnabled)
        {
            if (lerpMovement)
            {
                transform.position = Vector3.Lerp(transform.position, targetToFollow.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetToFollow.position + targetOffset;
            }
        }  
    }

    public void EnableFollow()
    {
        is_FollowEnabled = true;
    }
}
