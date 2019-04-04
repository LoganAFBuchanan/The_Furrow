using System.Collections;
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
    public GameObject movementTile;
    public GameObject allyTile;
    public GameObject enemyTile;
    public GameObject targetTile;
    public GameObject pathTile;
    public GameObject highlightTile;

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

    public void ToggleOffTileEffects()
    {
        movementTile.SetActive(false);
        allyTile.SetActive(false);
        enemyTile.SetActive(false);
        targetTile.SetActive(false);
        pathTile.SetActive(false);
        highlightTile.SetActive(false);
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
        
        ToggleOffTileEffects();
        targetTile.SetActive(true);
    }

    public override void MarkAsHighlighted()
    {
        ToggleOffTileEffects();
        highlightTile.SetActive(true);
    }

    public override void MarkAsPath()
    {
        ToggleOffTileEffects();
        pathTile.SetActive(true);
        
    }

    public override void MarkAsReachable()
    {
        ToggleOffTileEffects();
        movementTile.SetActive(true);
    }

    public override void UnMark()
    {
        ToggleOffTileEffects();
        if(tiletype == TileType.ALLY || tiletype == TileType.CONTESTED) allyTile.SetActive(true);
        else enemyTile.SetActive(true);
        

        //if(tiletype == TileType.CONTESTED) MarkAsContested();
    }
}
