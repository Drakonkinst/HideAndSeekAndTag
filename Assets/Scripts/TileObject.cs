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

public class TileObject : MonoBehaviour
{
    private const int maxRotations = 4;
    private const float rotationAngle = 90.0f;

    public Color DefaultColor;
    public Color RevealedColor;
    public Sprite FootprintsSprite = null;
    public Sprite ConfusedFootprintsSprite = null;
    public Color PlayerColor;
    public Color HunterColor;

    private SpriteRenderer sprite;
    private SpriteRenderer footprintsSprite;
    private int rotation;
    private Transform footprintTransform;
    private float ageMultiplier = 1.0f;


    // Start is called before the first frame update
    // https://answers.unity.com/questions/676106/getcomponentinchildren-returning-a-component-in-pa.html
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if(FootprintsSprite != null)
        {
            footprintsSprite = GetComponentsInChildren<SpriteRenderer>()[1];
            footprintTransform = footprintsSprite.gameObject.transform;
        }
    }

    // https://stackoverflow.com/questions/41136892/set-sprite-of-image-to-default-none
    public void SetFootprints(Tile.FootprintDirection direction, bool isPlayer = false)
    {
        if(footprintsSprite == null)
        {
            return;
        }

        ResetFootprintRotation();
        if(direction == Tile.FootprintDirection.NONE)
        {
            footprintsSprite.sprite = null;
        } 
        else if(direction == Tile.FootprintDirection.CONFUSED)
        {
            footprintsSprite.sprite = ConfusedFootprintsSprite;
        }
        else if(direction == Tile.FootprintDirection.UP)
        {
            footprintsSprite.sprite = FootprintsSprite;
        }
        else if (direction == Tile.FootprintDirection.DOWN)
        {
            footprintsSprite.sprite = FootprintsSprite;
            RotateFootprints(2);
        }
        else if (direction == Tile.FootprintDirection.LEFT)
        {
            footprintsSprite.sprite = FootprintsSprite;
            RotateFootprints(3);
        }
        else if (direction == Tile.FootprintDirection.RIGHT)
        {
            footprintsSprite.sprite = FootprintsSprite;
            RotateFootprints(1);
        }

        if(isPlayer)
        {
            footprintsSprite.color = PlayerColor;
        } else
        {
            footprintsSprite.color = HunterColor;
        }
    }


    // https://answers.unity.com/questions/580001/trying-to-rotate-a-2d-sprite.html
    // Rotate clockwise
    private void RotateFootprints(int numTimes = 1)
    {
        for(int i = 0; i < numTimes; ++i)
        {
            rotation = (rotation + 1) % maxRotations;
            footprintTransform.Rotate(Vector3.back * rotationAngle);
        }
    }

    private void ResetFootprintRotation()
    {
        int numTimes = (maxRotations - rotation) % maxRotations;
        RotateFootprints(numTimes);
    }

    public void SetFootprintOpacity(float opacity)
    {
        if(footprintsSprite == null)
        {
            return;
        }

        Color color = footprintsSprite.color;
        color.a = opacity * ageMultiplier;
        footprintsSprite.color = color;
    }

    public void SetAgeMultiplier(float multiplier)
    {
        ageMultiplier = multiplier;
    }

    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    public void ResetColor()
    {
        sprite.color = DefaultColor;
    }

    public void Reveal()
    {
        sprite.color = RevealedColor;
    }
}
