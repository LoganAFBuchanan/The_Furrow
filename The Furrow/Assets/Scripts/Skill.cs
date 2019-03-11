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

    public GameObject hitVFX;
    public GameObject tileVFX;

    public bool isSplash;
    public int splashDamage;

    public bool targetAllAllies;
    public bool targetAllEnemies;

    public bool moveCaster;
    public bool damageBeforeMove;
    public bool moveTarget;

    public bool hitNeeded;
    public int defenceChange;

    public bool shieldTargets;
    public int shieldAmount;

    public bool isSpawner;
    public GameObject spawnedUnit;

    public bool allyImmune;

    public bool isBuff;
    public BuffType buffType;
    public int buffStrength;
    public int buffDuration;

    public bool isDebuff;
    public DebuffType debuffType;
    public int debuffStrength;
    public int debuffDuration;

    public bool isGroundEffect;
    public GroundEffectType groundEffectType;

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
        SLOW,
        DAMAGE,
        NONE
    }

    public enum DebuffType
    {
        SLOW,
        NONE
    }

    public enum GroundEffectType
    {
        SLIME,
        NONE
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

        if (isBuff)
        {
            switch (buffType)
            {
                case BuffType.HASTE:
                    buff = new HasteBuff(buffDuration, buffStrength);
                    break;

                case BuffType.DAMAGE:
                    buff = new DamageBuff(buffDuration, buffStrength);
                    break;

                default:
                    break;
            }
        }

        if (isDebuff)
        {
            switch (debuffType)
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


    public void ApplyGroundEffect(List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            switch (groundEffectType)
            {
                case GroundEffectType.SLIME:
                    (cell as CombatTile).ApplySlimed();
                    break;
            }
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

        if(shieldTargets) target.DefenceFactor += shieldAmount;

        SpawnHitEffect(target);

        if (!allyImmune)
        {
            if (!isSplash)
                target.Defend(user, damage);
            else
            {
                //If skill is splash damage then reduce damage if the target isn't in the first target coordinate then take half damage
                Vector2 primaryDamageCoord = new Vector2(user.Cell.OffsetCoord.x + skillTargetX[0], user.Cell.OffsetCoord.y + skillTargetY[0]);
                if (target.Cell.OffsetCoord == primaryDamageCoord) target.Defend(user, damage);
                else target.Defend(user, splashDamage);
            }
        }
        else
        {
            if (target.PlayerNumber != user.PlayerNumber)
            {

                if (!isSplash)
                    target.Defend(user, damage);
                else
                {
                    //If skill is splash damage then reduce damage if the target isn't in the first target coordinate then take half damage
                    Vector2 primaryDamageCoord = new Vector2(user.Cell.OffsetCoord.x + skillTargetX[0], user.Cell.OffsetCoord.y + skillTargetY[0]);
                    if (target.Cell.OffsetCoord == primaryDamageCoord) target.Defend(user, damage);
                    else target.Defend(user, splashDamage);
                }
            }
        }

        if (isBuff && target.PlayerNumber == user.PlayerNumber)
        {
            var _buff = buff.Clone();
            _buff.Apply(target);
            target.Buffs.Add(_buff);
        }

        if (isDebuff && target.PlayerNumber != user.PlayerNumber)
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

            Vector2 destinationPosition = new Vector3(currCell.OffsetCoord.x + moveTargetX[i], currCell.OffsetCoord.y + moveTargetY[i]);

            foreach (Cell cell in neighbourCells)
            {
                Debug.Log("CurrPos: " + cell.OffsetCoord + " to " + destinationPosition);
                if (cell.OffsetCoord == destinationPosition)
                {
                    if (cell.IsTaken)
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

    public void SpawnHitEffect(HeroControl target)
    {
        if(hitVFX != null)
        {
            GameObject spawnedVFX = Instantiate(hitVFX, target.Cell.transform);
            //spawnedVFX.transform.position = new Vector3(0, 0, 0);

        }
    }

    public void SpawnTileEffect(Cell target)
    {
        if(tileVFX != null)
        {
            GameObject spawnedVFX = Instantiate(tileVFX, target.transform);
            //spawnedVFX.transform.position = new Vector3(0, 0, 0);

        }
    }

}