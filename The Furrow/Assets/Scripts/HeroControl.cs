using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class HeroControl : Unit
{
    public Color LeadingColor;
    public string UnitName;
    public int defenseStrength;

    public GameObject skillObject1;
    public GameObject skillObject2;
    public GameObject skillObject3;

    [System.NonSerialized]
    public Skill skill1;
    [System.NonSerialized]
    public Skill skill2;
    [System.NonSerialized]
    public Skill skill3;

    private string combatteam;

    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 1, 0);
        GetComponent<Renderer>().material.color = LeadingColor;

        if(skillObject1 != null) skill1 = skillObject1.GetComponent<Skill>();
        if(skillObject2 != null) skill2 = skillObject2.GetComponent<Skill>();
        if(skillObject3 != null) skill3 = skillObject3.GetComponent<Skill>();

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

    public void GainDefence()
    {

        while(ActionPoints > 0)
        {
            DefenceFactor += defenseStrength;
            ActionPoints--;
        }
        

    }

    public void UseSkill(int skillNum, List<Cell> cells, List<Unit> units)
    {   
        List<Cell> targetCells;
        List<HeroControl> hitTargets = new List<HeroControl>();

        switch (skillNum)
      {
          case 1:
            Debug.Log("Skill 1 Used by " + UnitName);
            ActionPoints -= skill1.actioncost;
            if(skillObject1 != null)
            {
                targetCells = GetAvailableTargets(cells, skill1);
                foreach(Cell cell in cells) //Check every cell on the game board
                {
                    foreach(Cell targetCell in targetCells) //Check the cells that are in the target area
                    {
                        if(cell.OffsetCoord == targetCell.OffsetCoord && cell.IsTaken) //If a cell is taken
                        {
                            foreach(HeroControl unit in units) //Check all units and use the skill on all those units
                            {
                                if(unit.Cell.OffsetCoord == cell.OffsetCoord)
                                {
                                    Debug.Log("Skill 1 used on: " + (unit as HeroControl).UnitName);
                                    hitTargets.Add(unit);
                                    
                                }
                            }
                        }
                    }
                }

            }else
            {
                Debug.LogError(UnitName + " Does not have a skill1");
            }

            foreach(HeroControl target in hitTargets)
            {
                skill1.UseSkill(this, target);
            }

            break;
          case 2:
            Debug.Log("Skill 2 Used by " + UnitName);
            ActionPoints -= skill2.actioncost;
            if(skillObject2 != null)
            {
                targetCells = GetAvailableTargets(cells, skill2);
                foreach(Cell cell in cells) //Check every cell on the game board
                {
                    foreach(Cell targetCell in targetCells) //Check the cells that are in the target area
                    {
                        if(cell.OffsetCoord == targetCell.OffsetCoord && cell.IsTaken) //If a cell is taken
                        {
                            foreach(HeroControl unit in units) //Check all units and use the skill on all those units
                            {
                                if(unit.Cell.OffsetCoord == cell.OffsetCoord)
                                {
                                    Debug.Log("Skill 2 used on: " + (unit as HeroControl).UnitName);
                                    hitTargets.Add(unit);
                                }
                            }
                        }
                    }
                }
            }else
            {
                Debug.LogError(UnitName + " Does not have a skill2");
            }

            foreach(HeroControl target in hitTargets)
            {
                skill2.UseSkill(this, target);
            }
            break;
        case 3:
            Debug.Log("Skill 3 Used by " + UnitName);
            ActionPoints -= skill3.actioncost;
            if(skillObject3 != null)
            {
                targetCells = GetAvailableTargets(cells, skill3);
                foreach(Cell cell in cells) //Check every cell on the game board
                {
                    foreach(Cell targetCell in targetCells) //Check the cells that are in the target area
                    {
                        if(cell.OffsetCoord == targetCell.OffsetCoord && cell.IsTaken) //If a cell is taken
                        {
                            foreach(HeroControl unit in units) //Check all units and use the skill on all those units
                            {
                                if(unit.Cell.OffsetCoord == cell.OffsetCoord)
                                {
                                    Debug.Log("Skill 3 used on: " + (unit as HeroControl).UnitName);
                                    hitTargets.Add(unit);
                                }
                            }
                        }
                    }
                }
            }else
            {
                Debug.LogError(UnitName + " Does not have a skill3");
            }
            foreach(HeroControl target in hitTargets)
            {
                skill3.UseSkill(this, target);
            }
            break;
          default:
            Debug.Log("Default case for use skill");
            break;
      }


    }

    public override bool IsCellMovableTo(Cell cell)
    {
        return base.IsCellMovableTo(cell) &&
        ((cell as CombatTile).tileteam == combatteam || (cell as CombatTile).tileteam == "CONTESTED");
        //Prohibits moving through cells that are Not allied with you
    }

    public override bool IsCellTraversable( Cell cell)
    {
        return base .IsCellTraversable(cell) &&
        ((cell as CombatTile).tileteam == combatteam || (cell as CombatTile).tileteam == "CONTESTED");
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
