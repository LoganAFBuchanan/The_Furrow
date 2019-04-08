using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using SpriteGlow;

public class HeroControl : Unit
{
    [FMODUnity.EventRef]
    public List<string> hurtSounds;

    public Color LeadingColor;
    public string UnitName;
    public int defenseStrength;
    public GameObject defenceEffect;

    private bool isGlow;

    public GameObject skillObject1;
    public GameObject skillObject2;
    public GameObject skillObject3;

    private List<Cell> cells;
    private List<Unit> units;
    private Skill selectedSkill;
    private GameObject selectedSkillObject;

    public CharacterCombatUI combatUI;

    [System.NonSerialized]
    public SpriteGlowEffect highlightEffect;

    [System.NonSerialized]
    public bool isDead;


    [System.NonSerialized]
    public Skill skill1;
    [System.NonSerialized]
    public Skill skill2;
    [System.NonSerialized]
    public Skill skill3;

    private int moveBonus = 0;

    private string combatteam;

    public override void Initialize()
    {
        base.Initialize();
        //transform.position += new Vector3(0, 0, 0);
        //GetComponent<Renderer>().material.color = LeadingColor;

        highlightEffect = GetComponent<SpriteGlowEffect>();
        isGlow = true;

        isDead = false;

        //highlightEffect.GlowBrightness = 0.0f;
        highlightEffect.OutlineWidth = 0;
        highlightEffect.GlowBrightness = 2.5f;

        AttachSkills();

        if (PlayerNumber == 0)
        {
            combatteam = "ALLY";
        }
        else
        {
            combatteam = "ENEMY";
        }

    }

    public override void OnUnitClicked()
    {
        base.OnUnitClicked();
    }
    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();

        highlightEffect.OutlineWidth += 1;
    }
    protected override void OnMouseExit()
    {
        base.OnMouseExit();
        highlightEffect.OutlineWidth -= 1;
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        MovementPoints = TotalMovementPoints;
        DefenceFactor = 0;
    }

    protected override void OnDestroyed()
    {
        Cell.IsTaken = false;
        animator.Play("Death",0,0);
        //Destroy(gameObject);
    }

    public void PlaySit()
    {
        animator.Play("sitting",0,0);
    }

    public void PlayIdle()
    {
        animator.Play("Idle",0,0);
    }

    public void JustDestroy()
    {
        StartCoroutine(FadeDeath());
        //Destroy(gameObject);
    }

    // Fades corpse before destroying
    // Adapted from: https://www.alanzucconi.com/2016/12/29/fading-sprites-unity-5/
    public IEnumerator FadeDeath()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        float duration = 1f;
        float start = Time.time;

        while (Time.time <= start + duration)
        {
            Color color = renderer.color;
            color.a = 1f - Mathf.Clamp01((Time.time - start) / duration);
            renderer.color = color;
            yield return new WaitForEndOfFrame();
        }
        //Destory non-player characters
        if(PlayerNumber != 0)
        {
            MarkAsDestroyed();
            Destroy(gameObject);
        }
        else
        {
            MarkAsDestroyed();
            SetCharacterDeath(true);
        }
        
    }

    public void SetCharacterDeath(bool setDead)
    {
        isDead = setDead;

        if(isDead)
        {
            if(GameObject.Find("GUIController") != null)
            {
                GameObject.Find("GUIController").GetComponent<GUIController>().RemoveListenersFromHero(this);
            }
            gameObject.SetActive(false);
        }
        else
        {
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 1f;
            GetComponent<SpriteRenderer>().color = color;
            gameObject.SetActive(true);
        }
    }

    

    public void GainDefence()
    {
        GameObject newEffect = GameObject.Instantiate(defenceEffect, this.transform);
        newEffect.transform.position += new Vector3(0.2f, 1.2f, 0);
        while (ActionPoints > 0)
        {
            DefenceFactor += defenseStrength;
            ActionPoints--;
        }

    }

    public void DisableGlow()
    {
        highlightEffect.OutlineWidth = 0;
        isGlow = false;
    }

    public void EnableGlow()
    {
        highlightEffect.OutlineWidth = 3;
        isGlow = true;
    }

    public virtual void Defend(Unit other, int damage)
    {
        base.Defend(other, damage);

        if(HitPoints > 0 && damage != 0) animator.Play("Hurt",0,0); 

        

    }

    public void AttachSkills()
    {
        if (skillObject1 != null) skill1 = skillObject1.GetComponent<Skill>();
        if (skillObject2 != null) skill2 = skillObject2.GetComponent<Skill>();
        if (skillObject3 != null) skill3 = skillObject3.GetComponent<Skill>();
    }

    public void PlayHurtSFX()
    {
        if(hurtSounds != null)
        {
            Rigidbody rigidBodytest = null;
            int rand = UnityEngine.Random.Range(0, hurtSounds.Count);
            FMOD.Studio.EventInstance soundEvent;
            string soundPath = hurtSounds[rand];
            soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), rigidBodytest);
            soundEvent.start();
        }
        
        
    }
    

    public void PlaySkillSFX()
    {
        FMOD.Studio.EventInstance soundEvent;
        string soundPath;
        Rigidbody rigidBodytest = null;

        if(selectedSkill.SFX != null)
        {
            soundPath = selectedSkill.SFX;
            soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), rigidBodytest);
            soundEvent.start();
        }

    }

    // <summary>
    // Basic Skill use function for player characters
    // </summary>
    public void UseSkill()
    {
        List<Cell> targetCells = new List<Cell>();
        List<HeroControl> hitTargets = new List<HeroControl>();

        Debug.Log(selectedSkill.skillname + " Used by " + UnitName);
        ActionPoints -= selectedSkill.actioncost;
        //if (animator != null) PlaySkillAnimation(selectedSkill);


        //If a skill moves the caster this section determines whether the damage is dealt before or after
        if (selectedSkill.moveCaster)
        {
            if (!selectedSkill.damageBeforeMove) StartCoroutine(SkillMove(selectedSkill, cells));
            if (selectedSkill.damageBeforeMove) StartCoroutine(UseMoveSkill(selectedSkill, cells, units));
        }


        selectedSkill.PassiveUse(this);


        //Figure out targets
        if (selectedSkillObject != null && !selectedSkill.moveCaster)
        {
            if (selectedSkill.targetAllAllies && selectedSkill.targetAllEnemies)
            {
                //Target all units on the map
                foreach (HeroControl unit in units)
                {
                    hitTargets.Add(unit);
                }
            }
            else if (selectedSkill.targetAllEnemies)
            {
                //Target all enemies
                foreach (HeroControl unit in units)
                {
                    if (unit.PlayerNumber != PlayerNumber) hitTargets.Add(unit);
                }
            }
            else if (selectedSkill.targetAllAllies)
            {
                //Target all allies
                foreach (HeroControl unit in units)
                {
                    if (unit.PlayerNumber == PlayerNumber) hitTargets.Add(unit);
                }
            }
            else
            {
                //Just target the skill targets
                targetCells = GetAvailableTargets(cells, selectedSkill);

                foreach (Cell targetCell in targetCells) //Check the cells that are in the target area
                {
                    if (targetCell.IsTaken) //If a cell is taken
                    {
                        foreach (HeroControl unit in units) //Check all units and use the skill on all those units
                        {
                            if (unit.Cell.OffsetCoord == targetCell.OffsetCoord)
                            {
                                Debug.Log(selectedSkill.skillname + " used on: " + (unit as HeroControl).UnitName);
                                hitTargets.Add(unit);

                            }
                        }
                    }
                }
            }




        }
        else if (selectedSkillObject != null && selectedSkill.moveCaster)
        {
            if (!selectedSkill.damageBeforeMove) StartCoroutine(UseMoveSkill(selectedSkill, cells, units));
            if (selectedSkill.damageBeforeMove) StartCoroutine(SkillMove(selectedSkill, cells));
        }
        else
        {
            Debug.LogError(UnitName + " Does not have a skill object");
        }

        //Apply ground effect if any
        if (selectedSkill.isGroundEffect)
        {
            if (targetCells.Count == 0) targetCells = GetAvailableTargets(cells, selectedSkill);
            selectedSkill.ApplyGroundEffect(targetCells);
        }

        //Spawn Tile VFX
        if(selectedSkill.tileVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                selectedSkill.SpawnTileEffect(effectCell);
            }
        }

        //Spawn spray vfx
        if(selectedSkill.slimeSprayVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                if(effectCell.OffsetCoord.x == (Cell.OffsetCoord.x - 1))
                    selectedSkill.SpawnSprayEffect(effectCell);
            }
        }

        int bonusDamage = 0;
        if (selectedSkill.bonusDamage != 0)
        {

            if (moveBonus != 0)
            {
                Debug.Log("Move Bonus: " + moveBonus);

                bonusDamage = selectedSkill.bonusDamage * moveBonus;
                if (selectedSkill.skillname == "Ignition Lance") selectedSkill.damage += bonusDamage;
                Debug.Log("Total Damage: " + selectedSkill.damage);
                moveBonus = 0;
            }

            if (selectedSkill.skillname == "Last Rites")
            {
                Defend(this, selectedSkill.bonusDamage);
                bonusDamage = (TotalHitPoints - HitPoints) * selectedSkill.bonusDamage;
                selectedSkill.damage += bonusDamage;
            }


        }

        //Apply skill based buffs
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).ApplySkillBuff(selectedSkill);
            }
        }

        //Use skill on all targets
        foreach (HeroControl target in hitTargets)
        {
            selectedSkill.UseSkill(this, target);
        }

        if (bonusDamage != 0) selectedSkill.damage -= bonusDamage;

        //Remove skill based buffs after use
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).UndoSkillBuff(selectedSkill);
            }
        }

    }


    //Overloaded version for AI use as they don't need a skill object
    public void UseSkillAI()
    {
        List<Cell> targetCells;
        List<HeroControl> hitTargets = new List<HeroControl>();

        Debug.Log(selectedSkill.skillname + " Used by " + UnitName);
        ActionPoints -= selectedSkill.actioncost;
        //if (animator != null) PlaySkillAnimation(selectedSkill);

        targetCells = GetAvailableTargets(cells, selectedSkill);

        

        if (selectedSkill.targetAllAllies && selectedSkill.targetAllEnemies)
        {
            //Target all units on the map
            foreach (HeroControl unit in units)
            {
                hitTargets.Add(unit);
            }
        }
        else if (selectedSkill.targetAllEnemies)
        {
            //Target all enemies
            foreach (HeroControl unit in units)
            {
                if (unit.PlayerNumber != PlayerNumber) hitTargets.Add(unit);
            }
        }
        else if (selectedSkill.targetAllAllies)
        {
            //Target all allies
            foreach (HeroControl unit in units)
            {
                if (unit.PlayerNumber == PlayerNumber) hitTargets.Add(unit);
            }
        }
        else
        {
            //Just target the skill targets
            targetCells = GetAvailableTargets(cells, selectedSkill);

            foreach (Cell targetCell in targetCells) //Check the cells that are in the target area
            {
                if (targetCell.IsTaken) //If a cell is taken
                {
                    foreach (HeroControl unit in units) //Check all units and use the skill on all those units
                    {
                        if (unit.Cell.OffsetCoord == targetCell.OffsetCoord)
                        {
                            Debug.Log(selectedSkill.skillname + " used on: " + (unit as HeroControl).UnitName);
                            hitTargets.Add(unit);

                        }
                    }
                }
            }
        }


        //Spawn Detection
        if (selectedSkill.isSpawner)
        {
            foreach (Cell targetCell in targetCells)
            {
                if (!targetCell.IsTaken)
                    selectedSkill.SpawnUnit(this, targetCell);
            }

        }

        Debug.Log("I am outside the AI ground effect skill section");
        //Apply ground effect if any
        if (selectedSkill.isGroundEffect)
        {
            Debug.Log("I am using a ground effect skill");
            if (targetCells.Count == 0) targetCells = GetAvailableTargets(cells, selectedSkill);
            selectedSkill.ApplyGroundEffect(targetCells);
        }

        //Spawn Tile VFX
        if(selectedSkill.tileVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                selectedSkill.SpawnTileEffect(effectCell);
            }
        }

        //Spawn spray vfx
        if(selectedSkill.slimeSprayVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                if(effectCell.OffsetCoord.x == (Cell.OffsetCoord.x - 1))
                    selectedSkill.SpawnSprayEffect(effectCell);
            }
        }

        int bonusDamage = 0;
        if (selectedSkill.bonusDamage != 0)
        {

            if (moveBonus != 0)
            {
                Debug.Log("Move Bonus: " + moveBonus);

                bonusDamage = selectedSkill.bonusDamage * moveBonus;
                if (selectedSkill.skillname == "Ignition Lance") selectedSkill.damage += bonusDamage;
                Debug.Log("Total Damage: " + selectedSkill.damage);
                moveBonus = 0;
            }

            if (selectedSkill.skillname == "Last Rites")
            {
                bonusDamage = (TotalHitPoints - HitPoints) * selectedSkill.bonusDamage;
                selectedSkill.damage += bonusDamage;
            }


        }

        
        //Apply skill based buffs
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).ApplySkillBuff(selectedSkill);
            }
        }

        //Use skill on all targets
        foreach (HeroControl target in hitTargets)
        {
            selectedSkill.UseSkill(this, target);
        }

        if (bonusDamage != 0) selectedSkill.damage -= bonusDamage;

        //Remove skill based buffs after use
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).UndoSkillBuff(selectedSkill);
            }
        }


    }

    //For Dashes
    public IEnumerator SkillMove(Skill skill, List<Cell> cells)
    {
        Debug.Log(UnitName + " is Dashing/Moving because of " + skill.skillname);
        float savedSpeed = MovementSpeed;
        MovementSpeed = Constants.PUSH_SPEED;
        for (int i = 0; i < skill.moveCasterX.Length; i++)
        {
            Debug.Log("Dash #: " + i);


            List<Cell> neighbourCells = Cell.GetNeighbours(cells);

            Vector2 destinationPosition = new Vector2(Cell.OffsetCoord.x + skill.moveCasterX[i], Cell.OffsetCoord.y + skill.moveCasterY[i]);

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
                        List<Cell> path = FindPath(cells, cell);
                        //Cell = cell;

                        moveBonus++;
                        //If target is moving just hang on a second
                        while (isMoving)
                            yield return 0;

                        PushMove(cell, path);

                    }
                }
            }
        }
        MovementSpeed = savedSpeed;
        yield return 0;
    }


    // For skill use after a dash
    public IEnumerator UseMoveSkill(Skill selectedSkill, List<Cell> cells, List<Unit> units)
    {
        while (isMoving)
            yield return 0;

        List<Cell> targetCells;
        List<HeroControl> hitTargets = new List<HeroControl>();

        Debug.Log(selectedSkill.skillname + " Used by " + UnitName);
        ActionPoints -= selectedSkill.actioncost;
        //if (animator != null) PlaySkillAnimation(selectedSkill);

        targetCells = GetAvailableTargets(cells, selectedSkill);

        if (selectedSkill.targetAllAllies && selectedSkill.targetAllEnemies)
        {
            //Target all units on the map
            foreach (HeroControl unit in units)
            {
                hitTargets.Add(unit);
            }
        }
        else if (selectedSkill.targetAllEnemies)
        {
            //Target all enemies
            foreach (HeroControl unit in units)
            {
                if (unit.PlayerNumber != PlayerNumber) hitTargets.Add(unit);
            }
        }
        else if (selectedSkill.targetAllAllies)
        {
            //Target all allies
            foreach (HeroControl unit in units)
            {
                if (unit.PlayerNumber == PlayerNumber) hitTargets.Add(unit);
            }
        }
        else
        {
            //Just target the skill targets
            targetCells = GetAvailableTargets(cells, selectedSkill);

            foreach (Cell targetCell in targetCells) //Check the cells that are in the target area
            {
                if (targetCell.IsTaken) //If a cell is taken
                {
                    foreach (HeroControl unit in units) //Check all units and use the skill on all those units
                    {
                        if (unit.Cell.OffsetCoord == targetCell.OffsetCoord)
                        {
                            Debug.Log(selectedSkill.skillname + " used on: " + (unit as HeroControl).UnitName);
                            hitTargets.Add(unit);

                        }
                    }
                }
            }
        }

        //Apply ground effect if any
        if (selectedSkill.isGroundEffect)
        {
            if (targetCells.Count == 0) targetCells = GetAvailableTargets(cells, selectedSkill);
            selectedSkill.ApplyGroundEffect(targetCells);
        }

        //Spawn Tile VFX
        if(selectedSkill.tileVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                selectedSkill.SpawnTileEffect(effectCell);
            }
        }

        //Spawn spray vfx
        if(selectedSkill.slimeSprayVFX != null)
        {
            foreach(Cell effectCell in targetCells)
            {
                if(effectCell.OffsetCoord.x == (Cell.OffsetCoord.x - 1))
                    selectedSkill.SpawnSprayEffect(effectCell);
            }
        }

        //Add bonus damage for movement, health loss, etc.  (ie: Ignition lance)

        int bonusDamage = 0;
        if (selectedSkill.bonusDamage != 0)
        {

            if (moveBonus != 0)
            {
                Debug.Log("Move Bonus: " + moveBonus);

                bonusDamage = selectedSkill.bonusDamage * moveBonus;
                if (selectedSkill.skillname == "Ignition Lance") selectedSkill.damage += bonusDamage;
                Debug.Log("Total Damage: " + selectedSkill.damage);
                moveBonus = 0;
            }

            if (selectedSkill.skillname == "Last Rites")
            {
                bonusDamage = (TotalHitPoints - HitPoints) * selectedSkill.bonusDamage;
                selectedSkill.damage += bonusDamage;
            }


        }
        

        //Apply skill based buffs
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).ApplySkillBuff(selectedSkill);
            }
        }

        //Use skill on all targets
        foreach (HeroControl target in hitTargets)
        {
            selectedSkill.UseSkill(this, target);
        }

        if (bonusDamage != 0) selectedSkill.damage -= bonusDamage;

        //Remove skill based buffs after use
        foreach(Buff buff in Buffs)
        {
            if(buff is DamageBuff)
            {
                (buff as DamageBuff).UndoSkillBuff(selectedSkill);
            }
        }

        yield return 0;
    }


    //Player Skill ANimation trigger
    public void PlaySkillAnimation(int _skillNum, List<Cell> _cells, List<Unit> _units)
    {

        cells = _cells;
        units = _units;

        selectedSkillObject = skillObject1;
        selectedSkill = skill1;

        switch (_skillNum)
        {
            case 1:
                selectedSkillObject = skillObject1;
                selectedSkill = skill1;
                break;

            case 2:
                selectedSkillObject = skillObject2;
                selectedSkill = skill2;
                break;

            case 3:
                selectedSkillObject = skillObject3;
                selectedSkill = skill3;
                break;

        }

        animator.Play(selectedSkill.skillname,0,0);

        bool hasSkill = false;
        foreach(AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
        // look at all the animation clips here!
            if(ac.name == selectedSkill.skillname)
            {
                hasSkill = true;
            }
        }

        if(!hasSkill)
        {
            UseSkill();
        }
    }



    //Player Skill ANimation trigger
    public void PlaySkillAnimationAI(Skill _selectedSkill, List<Cell> _cells, List<Unit> _units)
    {
        cells = _cells;
        units = _units;
        selectedSkill = _selectedSkill;
        Debug.Log("YOYOYO THIS BE THE AI SKILL ANIMATION!!!!!!!!!!! " + selectedSkill.skillname);
        if(selectedSkill.skillname == "AttackDeer")
            {
                Debug.Log("YOYOYO THIS BE THE DEER ATTACK!!!!!!!!!!!");
                bool hasted = false;
                foreach(Buff __buff in Buffs)
                {
                    if(__buff is DamageBuff)
                    {
                        hasted = true;
                    }
                }
                Debug.Log("Am I Hasted?: " + hasted);
                if(hasted) 
                {
                    animator.Play("HastedAttack",0,0);
                }
                else 
                {
                    Debug.Log("Because I am not hasted, I am going to play the animation for: " + selectedSkill.skillname);
                    animator.Play(selectedSkill.skillname,0,0);
                }
            }
            else
            {
                animator.Play(selectedSkill.skillname,0,0);
            }

        

        bool hasSkill = false;
        foreach(AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("AI Skill name: " + ac.name);
            // look at all the animation clips here!

            

            if(ac.name == selectedSkill.skillname)
            {
                hasSkill = true;
            }
        }

        if(!hasSkill)
        {
            UseSkillAI();
        }
    }

    public override bool IsCellMovableTo(Cell cell)
    {
        return base.IsCellMovableTo(cell) &&
        ((cell as CombatTile).tileteam == combatteam || (cell as CombatTile).tileteam == "CONTESTED");
        //Prohibits moving through cells that are Not allied with you
    }

    public override bool IsCellTraversable(Cell cell)
    {
        return base.IsCellTraversable(cell) &&
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
        GetComponent<BoxCollider>().enabled = true;
        highlightEffect.OutlineWidth = 0;
        if(combatUI != null)combatUI.ShowSkills(false);
    }

    public override void MarkAsDefending(Unit other)
    {
    }

    public override void MarkAsFriendly()
    {
        //GetComponent<Renderer>().material.color = LeadingColor + new Color(0.8f, 1, 0.8f);
        GetComponent<BoxCollider>().enabled = true;
        highlightEffect.OutlineWidth = 0;
        if(combatUI != null)combatUI.ShowSkills(false);
    }

    public override void MarkAsReachableEnemy()
    {
        //GetComponent<Renderer>().material.color = LeadingColor + Color.red;
    }

    public override void MarkAsSelected()
    {
        //GetComponent<Renderer>().material.color = LeadingColor + Color.green;
        GetComponent<BoxCollider>().enabled = false;
        highlightEffect.OutlineWidth = 3;
        if(combatUI != null)combatUI.ShowSkills(true);
        
    }

    

    public override void UnMark()
    {
        //GetComponent<Renderer>().material.color = LeadingColor;
        Debug.Log("UNMARKING UNIT");
        GetComponent<BoxCollider>().enabled = true;
        highlightEffect.OutlineWidth = 0;
        if(combatUI != null)combatUI.ShowSkills(false);
    }
}
