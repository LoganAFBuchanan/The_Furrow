﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTile : Square
{

    public TileType tiletype;

    public enum TileType
    {
        ALLY,
        ENEMY, 
        CONTESTED
    }

    [System.NonSerialized]
    public string tileteam;

    public bool isSlimed = false;
    public GameObject slimeObject;
    private int slimeLife;

    public void ApplySlimed()
    {
        if(!isSlimed)
        {
            isSlimed = true;
            MovementCost += 1;
            slimeLife = Constants.SLIME_LIFETIME;
            slimeObject.SetActive(true);
        }
    }

    public void DecreaseEffectLifetimes()
    {
        if(isSlimed)
        {
            slimeLife -= 1;
            if(slimeLife <= 0)
            {
                MovementCost -= 1;
                isSlimed = false;
                slimeObject.SetActive(false);
            }
        }
    }

    public void MarkAsContested()
    {
        Color newColor = Color.blue;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;
    }

    public override Vector3 GetCellDimensions()
    {
        return GetComponent<Renderer>().bounds.size;
    }

    public override void MarkAsTargetable()
    {
        
        Color newColor = Color.red;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;
    }

    public override void MarkAsHighlighted()
    {
        Color newColor = new Color(0.75f, 0.75f, 0.75f);
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;
    }

    public override void MarkAsPath()
    {
        Color newColor = Color.green;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;
        
    }

    public override void MarkAsReachable()
    {
        Color newColor = Color.yellow;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;
    }

    public override void UnMark()
    {
        Color newColor = Color.black;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;

        //if(tiletype == TileType.CONTESTED) MarkAsContested();
    }
}
