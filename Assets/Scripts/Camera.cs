/*
 * Name: Wesley Ho
 * ID: 2382489
 * Email: weho@chapman.edu
 * CPSC 236-02
 * Assignment: Final Project - Hide and Seek (and Tag)
 * This is my own work, and I did not cheat on this assignment.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private const float defaultHeight = -10.0f;
    public GameObject FollowTarget;

    public float Zoom = 0.0f;

    public void Update() {
        Vector2 targetPos = FollowTarget.transform.position;
        transform.position = new Vector3(targetPos.x, targetPos.y, defaultHeight + Zoom);

    }
}
