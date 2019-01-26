using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class HeroControl : Unit
{
    public Color LeadingColor;

    private string combatteam;

    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 1, 0);
        GetComponent<Renderer>().material.color = LeadingColor;

        if(PlayerNumber == 0){
            combatteam = "ALLY";
        }else{
            combatteam = "ENEMY";
        }

    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        MovementPoints = TotalMovementPoints;
        DefenceFactor = 0;
    }

    public override bool IsCellMovableTo(Cell cell)
    {
        return base.IsCellMovableTo(cell) &&
        (cell as CombatTile).tileteam == combatteam;
        //Prohibits moving through cells that are Not allied with you
    }

    public override bool IsCellTraversable( Cell cell)
    {
        return base .IsCellTraversable(cell) &&
        (cell as CombatTile).tileteam == combatteam;
        //Prohibits moving through cells that are Not allied with you
    }

    public override void MarkAsAttacking(Unit other)
    {
    }

    public override void MarkAsDestroyed()
    {
    }

    public override void MarkAsFinished()
    {
    }

    public override void MarkAsDefending(Unit other)
    {
    }

    public override void MarkAsFriendly()
    {
        GetComponent<Renderer>().material.color = LeadingColor + new Color(0.8f, 1, 0.8f);
    }

    public override void MarkAsReachableEnemy()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.red;
    }

    public override void MarkAsSelected()
    {
        GetComponent<Renderer>().material.color = LeadingColor + Color.green;
    }

    public override void UnMark()
    {
        GetComponent<Renderer>().material.color = LeadingColor;
    }
}
