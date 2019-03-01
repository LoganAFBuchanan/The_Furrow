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
        Color newColor = Color.white;
        newColor.a -= 1f - Constants.GRID_TRANSPARENCY;
        GetComponent<Renderer>().material.color = newColor;

        //if(tiletype == TileType.CONTESTED) MarkAsContested();
    }
}
