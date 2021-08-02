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

public class Steerable : MonoBehaviour
{
    protected SteeringManager steering;
    protected Transform myTransform;
    protected GameManager manager;
    protected SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        steering = GetComponent<SteeringManager>();
        myTransform = transform;

        if(steering == null)
        {
            Debug.LogWarning("No steering manager found!");
        }
    }
    public void Start()
    {
        manager = GameSettings.GetManager();
        OnStart();
    }

    public void DoUpdate()
    {
        myTransform.position = manager.CheckGameBoundaries(myTransform.position);
        DoBehavior();
        steering.UpdateSteering();
    }

    public virtual void OnStart()
    {

    }

    public void SetRotation(float facing, float offset = -90.0f)
    {
        float angle = facing * Mathf.Rad2Deg + offset;
        myTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetOpacity(float opacity)
    {
        Color color = sprite.color;
        color.a = opacity;
        sprite.color = color;
    }

    public virtual void DoBehavior()
    {
        steering.Seek(new Vector2(0, 0), 0);
    }

    public Vector2 GetPosition()
    {
        return myTransform.position;
    }

    public Vector2 GetVelocity()
    {
        return steering.GetHostVelocity();
    }

    public Point GetPoint()
    {
        if(manager == null)
        {
            manager = GameSettings.GetManager();
        }
        return manager.WorldPosToPoint(GetPosition());
    }
}
