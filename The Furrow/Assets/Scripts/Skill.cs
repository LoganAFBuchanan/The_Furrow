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

    public bool isSpawner;
    public GameObject spawnedUnit;

    public bool isBuff;
    public BuffType buffType;
    public int buffStrength;
    public int buffDuration;

    public bool isDebuff;
    public DebuffType debuffType;
    public int debuffStrength;
    public int debuffDuration;

    public int[] skillTargetX;
    public int[] skillTargetY;

    public int[] moveTargetX;
    public int[] moveTargetY;

    public int[] moveCasterX;
    public int[] moveCasterY;

    private Buff buff;
    private Buff debuff;

    public enum BuffType
    {
        HASTE,
        SLOW
    }

    public enum DebuffType
    {
        SLOW
    }

    public void Awake()
    {
        Debug.Log(this.gameObject.name);
        Debug.Log("HEY MY SPAWNABLE GAMEOBJECT IS " + spawnedUnit);
    }


    public void Initialize()
    {
        //skillTargetX = new int[3]{ 2, 2, 2 };
        //skillTargetY = new int[3]{ 1, 0, -1 };

        if(isBuff)
        {
            switch(buffType)
            {
                case BuffType.HASTE:
                    buff = new HasteBuff(buffDuration, buffStrength);
                    break;
                
                default:
                break;
            }
        }

        if(isDebuff)
        {
            switch(debuffType)
            {
                case DebuffType.SLOW:
                    debuff = new SlowDebuff(debuffDuration, debuffStrength);
                    break;
                
                default:
                break;
            }
        }
    }

    //Portion of the skill which will always fire
    public void PassiveUse(HeroControl user)
    {
        if (!hitNeeded) 
        {
            user.DefenceFactor += defenceChange;
            
        }
    }


    //If unit spawner then spawn a unit
    public void SpawnUnit(HeroControl user, Cell targetCell)
    {

        Debug.Log("The unit to be spawned is!: " + spawnedUnit);
        //Instantiate Unit
        GameObject newUnit = Instantiate(spawnedUnit);

        newUnit.transform.position = targetCell.transform.position;

        //Grab Script object
        var unitScript = newUnit.GetComponent<HeroControl>();

        unitScript.Initialize();

        //Place new Unit within the Units GameObject
        newUnit.transform.SetParent(GameObject.Find("Units").GetComponent<Transform>());

        //Attach to cell
        unitScript.Cell = targetCell;
        targetCell.IsTaken = true;

        //Add unit to the cellgrid
        CellGrid cellGrid = GameObject.Find("CellGrid").GetComponent<CellGrid>();
        cellGrid.Units.Add(unitScript);
        cellGrid.AddUnit(newUnit.transform);



    }

    public void UseSkill(HeroControl user, HeroControl target)
    {
        Initialize();

        if (defenceChange != 0 && hitNeeded) user.DefenceFactor += defenceChange; //Change defense if skill specifies

        
        target.Defend(user, damage);

        if(isBuff && target.PlayerNumber == 0)
        {
            var _buff = buff.Clone();
            _buff.Apply(target);
            target.Buffs.Add(_buff);
        } 

        if(isDebuff && target.PlayerNumber == 1)
        {
            var _debuff = debuff.Clone();
            _debuff.Apply(target);
            target.Buffs.Add(_debuff);
        } 
        

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