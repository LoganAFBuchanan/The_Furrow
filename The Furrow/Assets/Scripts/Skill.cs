using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillname;
    public string character;
    public int actioncost;
    public int damage;
    public int bonusDamage;

    public bool moveCaster;
    public bool moveTarget;

    public bool hitNeeded;
    public int defenceChange;

    public int[] skillTargetX;
    public int[] skillTargetY;

    public int[] moveTargetX;
    public int[] moveTargetY;

    public int[] moveCasterX;
    public int[] moveCasterY;


    public void Awake()
    {
        //skillTargetX = new int[3]{ 2, 2, 2 };
        //skillTargetY = new int[3]{ 1, 0, -1 };
    }

    //Portion of the skill which will always fire
    public void PassiveUse(HeroControl user)
    {
        if (!hitNeeded) user.DefenceFactor += defenceChange;
    }

    public void UseSkill(HeroControl user, HeroControl target)
    {

        if (defenceChange != 0 && hitNeeded) user.DefenceFactor += defenceChange; //Change defense if skill specifies
        target.Defend(user, damage);

        Debug.Log(target.UnitName + " has taken " + damage + " damage!");

        if (moveTarget)
        {
            StartCoroutine(PushTarget(user, target));

        }

    }


    public IEnumerator PushTarget(HeroControl user, HeroControl target)
    {
        CellGrid cellGrid = GameObject.Find("CellGrid").GetComponent<CellGrid>();
        

        Cell currCell = target.Cell;

        float savedSpeed = target.MovementSpeed;
        target.MovementSpeed = Constants.PUSH_SPEED;
        for (int i = 0; i < moveTargetX.Length; i++)
        {
            Debug.Log("Push #: " + i);
            
            
            List<Cell> neighbourCells = target.Cell.GetNeighbours(cellGrid.Cells);

            Vector3 destinationPosition = new Vector3(currCell.transform.position.x + moveTargetX[i], 0, currCell.transform.position.z + moveTargetY[i]);
            
            foreach(Cell cell in neighbourCells)
            {
                Debug.Log("CurrPos: " + cell.transform.position + " to " + destinationPosition);
                if(cell.transform.position == destinationPosition)
                {
                    if(cell.IsTaken)
                    {
                        //Grab other unit and damage both
                        Debug.Log("Other Cell is Taken");
                    }
                    else
                    {
                        //Move Unit to new Cell
                        List<Cell> path = target.FindPath(cellGrid.Cells, cell);
                        currCell = cell;

                        //If target is moving just hang on a second
                        while (target.isMoving)
                            yield return 0;
                        
                        target.PushMove(cell, path);

                    }
                }
            }
        }
        target.MovementSpeed = savedSpeed;
        yield return 0;
    }


}