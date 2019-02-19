using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// Base class for all units in the game.
/// </summary>
public abstract class Unit : MonoBehaviour
{
    Dictionary<Cell, List<Cell>> cachedPaths = null;
    List<Cell> cachedTargets = null;
    /// <summary>
    /// UnitClicked event is invoked when user clicks the unit. 
    /// It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitClicked;
    /// <summary>
    /// UnitSelected event is invoked when user clicks on unit that belongs to him. 
    /// It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitSelected;
    /// <summary>
    /// UnitDeselected event is invoked when user click outside of currently selected unit's collider.
    /// It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitDeselected;
    /// <summary>
    /// UnitHighlighted event is invoked when user moves cursor over the unit. 
    /// It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitHighlighted;
    /// <summary>
    /// UnitDehighlighted event is invoked when cursor exits unit's collider. 
    /// It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitDehighlighted;
    /// <summary>
    /// UnitAttacked event is invoked when the unit is attacked.
    /// </summary>
    public event EventHandler<AttackEventArgs> UnitAttacked;
    /// <summary>
    /// UnitDestroyed event is invoked when unit's hitpoints drop below 0.
    /// </summary>
    public event EventHandler<AttackEventArgs> UnitDestroyed;
    /// <summary>
    /// UnitMoved event is invoked when unit moves from one cell to another.
    /// </summary>
    public event EventHandler<MovementEventArgs> UnitMoved;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.MakeTransition(state);
    }

    /// <summary>
    /// A list of buffs that are applied to the unit.
    /// </summary>
    public List<Buff> Buffs { get; private set; }

    public int TotalHitPoints { get; private set; }
    protected int TotalMovementPoints;
    [System.NonSerialized]
    public int TotalActionPoints;

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// </summary>
    public Cell Cell { get; set; }

    public int HitPoints;
    public int AttackRange;
    public int AttackFactor;
    public int DefenceFactor;
    /// <summary>
    /// Determines how far on the grid the unit can move.
    /// </summary>
    [System.NonSerialized]
    public int MovementPoints;
    /// <summary>
    /// Determines speed of movement animation.
    /// </summary>
    public float MovementSpeed;
    /// <summary>
    /// Determines how many actions unit can perform in one turn.
    /// </summary>
    public int ActionPoints;

    /// <summary>
    /// Indicates the player that the unit belongs to. 
    /// Should correspoond with PlayerNumber variable on Player script.
    /// </summary>
    public int PlayerNumber;

    /// <summary>
    /// Indicates if movement animation is playing.
    /// </summary>
    public bool isMoving { get; set; }

    private static DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
    private static IPathfinding _fallbackPathfinder = new AStarPathfinding();

    /// <summary>
    /// Method called after object instantiation to initialize fields etc. 
    /// </summary>
    public virtual void Initialize()
    {
        Buffs = new List<Buff>();

        UnitState = new UnitStateNormal(this);

        TotalHitPoints = HitPoints;
        TotalMovementPoints = ActionPoints;
        TotalActionPoints = Constants.STARTING_AP_MAX;
    }

    protected virtual void OnMouseDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
        MovementPoints = TotalMovementPoints;
        ActionPoints = TotalActionPoints;

        SetState(new UnitStateMarkedAsFriendly(this));
    }
    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        cachedPaths = null;
        Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        Buffs.RemoveAll(b => b.Duration == 0);
        Buffs.ForEach(b => { b.Duration--; });

        SetState(new UnitStateNormal(this));
    }
    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    protected virtual void OnDestroyed()
    {
        Cell.IsTaken = false;
        MarkAsDestroyed();
        Destroy(gameObject);
    }

    /// <summary>
    /// Method is called when unit is selected.
    /// </summary>
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    /// <summary>
    /// Method is called when unit is deselected.
    /// </summary>
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method indicates if it is possible to attack unit given as parameter, 
    /// from cell given as second parameter.
    /// </summary>
    public virtual bool IsUnitAttackable(Unit other, Cell sourceCell)
    {
        if (sourceCell.GetDistance(other.Cell) <= AttackRange)
            return true;

        return false;
    }

    //Overloaded method to determine if a skill is in range to hit a target enemy
    public virtual bool IsUnitAttackable(Unit other, Cell sourceCell, Skill skill)
    {
        
        for(int i = 0; i < skill.skillTargetX.Count(); i++)
        {
            
            if(other.Cell.transform.position == new Vector3(sourceCell.transform.position.x + skill.skillTargetX[i], 0, sourceCell.transform.position.z + skill.skillTargetY[i]))
            {
                Debug.Log("Other transform is: " + other.Cell.transform.position + " and the source is " + new Vector3(sourceCell.transform.position.x + skill.skillTargetX[i], 0, sourceCell.transform.position.z + skill.skillTargetY[i]));
                return true;
            }
                
        }
        
        return false;
       
    }

    /// <summary>
    /// Method deals damage to unit given as parameter.
    /// </summary>
    public virtual void DealDamage(Unit other)
    {
        if (isMoving)
            return;
        if (ActionPoints == 0)
            return;
        if (!IsUnitAttackable(other, Cell))
            return;

        MarkAsAttacking(other);
        ActionPoints--;
        other.Defend(this, AttackFactor);

        if (ActionPoints == 0)
        {
            SetState(new UnitStateMarkedAsFinished(this));
            MovementPoints = 0;
        }  
    }
    /// <summary>
    /// Attacking unit calls Defend method on defending unit. 
    /// </summary>
    // protected virtual void Defend(Unit other, int damage)
    // {
    //     MarkAsDefending(other);
    //     //Damage is calculated by subtracting attack factor of attacker and defence factor of defender. 
    //     //If result is below 1, it is set to 1. This behaviour can be overridden in derived classes.
    //     HitPoints -= Mathf.Clamp(damage - DefenceFactor, 0, damage);  
    //     if (UnitAttacked != null)
    //         UnitAttacked.Invoke(this, new AttackEventArgs(other, this, damage));

    //     if (HitPoints <= 0)
    //     {
    //         if (UnitDestroyed != null)
    //             UnitDestroyed.Invoke(this, new AttackEventArgs(other, this, damage));
    //         OnDestroyed();
    //     }
    // }

    public virtual void Defend(Unit other, int damage)
    {
        MarkAsDefending(other);
        //Damage is calculated by subtracting attack factor of attacker and defence factor of defender. 

        int healthdamage = damage;

        if(DefenceFactor > 0){
            healthdamage -= DefenceFactor;
            healthdamage = Mathf.Max(0, healthdamage); //Adjust to 0 if negative

            DefenceFactor -= damage;
            DefenceFactor = Mathf.Max(0, DefenceFactor); //Adjust to 0 if negative
        }

        HitPoints -= healthdamage;  
        if (UnitAttacked != null)
            UnitAttacked.Invoke(this, new AttackEventArgs(other, this, healthdamage));

        if (HitPoints <= 0)
        {
            if (UnitDestroyed != null)
                UnitDestroyed.Invoke(this, new AttackEventArgs(other, this, healthdamage));
            OnDestroyed();
        }
    }

    /// <summary>
    /// Moves the unit to destinationCell along the path.
    /// </summary>
    public virtual void Move(Cell destinationCell, List<Cell> path)
    {
        if (isMoving)
            return;
        
        var totalMovementCost = path.Sum(h => h.MovementCost);
        if (ActionPoints < totalMovementCost)
            return;

        ActionPoints -= totalMovementCost;

        Cell.IsTaken = false;
        Cell = destinationCell;
        destinationCell.IsTaken = true;

        if (MovementSpeed > 0)
            StartCoroutine(MovementAnimation(path));
        else
            transform.position = Cell.transform.position;

        if (UnitMoved != null)
            UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path));    
    }
    protected virtual IEnumerator MovementAnimation(List<Cell> path)
    {
        isMoving = true;
        path.Reverse();
        foreach (var cell in path)
        {
            Vector3 destination_pos = new Vector3(cell.transform.localPosition.x, transform.localPosition.y, cell.transform.localPosition.z);
            while (transform.localPosition != destination_pos)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination_pos, Time.deltaTime * MovementSpeed);
                yield return 0;
            }
        }
        isMoving = false;
        
    }

    ///<summary>
    /// Method indicates if unit is capable of moving to cell given as parameter.
    /// </summary>
    public virtual bool IsCellMovableTo(Cell cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method indicates if unit is capable of moving through cell given as parameter.
    /// </summary>
    public virtual bool IsCellTraversable(Cell cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method returns all cells that the unit is capable of moving to.
    /// </summary>
    public HashSet<Cell> GetAvailableDestinations(List<Cell> cells)
    {
        cachedPaths = new Dictionary<Cell, List<Cell>>();
        
        var paths = cachePaths(cells);
        foreach (var key in paths.Keys)
        {
            if (!IsCellMovableTo(key))
                continue;
            var path = paths[key];

            var pathCost = path.Sum(c => c.MovementCost);
            if (pathCost <= ActionPoints)
            {
                cachedPaths.Add(key, path);
            }
        }
        return new HashSet<Cell>(cachedPaths.Keys);
    }


    //Returns a list of cells that correspond to the target area of the chosen skill
    public List<Cell> GetAvailableTargets(List<Cell> cells, Skill skill)
    {
        
        cachedTargets = new List<Cell>();

        Vector2 targetCoord = new Vector2();
        
        for (int i = 0; i < cells.Count(); i++)
        {

            for(int j = 0; j < skill.skillTargetX.Length; j++){
                
                targetCoord = new Vector2(Cell.OffsetCoord.x + skill.skillTargetX[j], Cell.OffsetCoord.y + skill.skillTargetY[j]);

                Debug.Log("Target Coordinate " + j + ": " + targetCoord.x + ", " + targetCoord.y);

                if((int)cells[i].OffsetCoord.x == targetCoord.x && (int)cells[i].OffsetCoord.y == targetCoord.y)
                {
                    Debug.Log("Added cell to hover highlight at: " + cells[i].OffsetCoord.x + ", " + cells[i].OffsetCoord.y);
                    cachedTargets.Add(cells[i]);
                }
            }
            
            
        }

        return cachedTargets;
    }


    private Dictionary<Cell, List<Cell>> cachePaths(List<Cell> cells)
    {
        var edges = GetGraphEdges(cells);
        var paths = _pathfinder.findAllPaths(edges, Cell);
        return paths;
    }

    public List<Cell> FindPath(List<Cell> cells, Cell destination)
    {
        if(cachedPaths != null && cachedPaths.ContainsKey(destination))
        {
            return cachedPaths[destination];
        }
        else
        {
            return _fallbackPathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
        }
    }
    /// <summary>
    /// Method returns graph representation of cell grid for pathfinding.
    /// </summary>
    protected virtual Dictionary<Cell, Dictionary<Cell, int>> GetGraphEdges(List<Cell> cells)
    {
        Dictionary<Cell, Dictionary<Cell, int>> ret = new Dictionary<Cell, Dictionary<Cell, int>>();
        foreach (var cell in cells)
        {
            if (IsCellTraversable(cell) || cell.Equals(Cell))
            {
                ret[cell] = new Dictionary<Cell, int>();
                foreach (var neighbour in cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                {
                    ret[cell][neighbour] = neighbour.MovementCost;
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsDefending(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsAttacking(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    public abstract void MarkAsDestroyed();

    /// <summary>
    /// Method marks unit as current players unit.
    /// </summary>
    public abstract void MarkAsFriendly();
    /// <summary>
    /// Method mark units to indicate user that the unit is in range and can be attacked.
    /// </summary>
    public abstract void MarkAsReachableEnemy();
    /// <summary>
    /// Method marks unit as currently selected, to distinguish it from other units.
    /// </summary>
    public abstract void MarkAsSelected();
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    public abstract void MarkAsFinished();
    /// <summary>
    /// Method returns the unit to its base appearance
    /// </summary>
    public abstract void UnMark();
}

public class MovementEventArgs : EventArgs
{
    public Cell OriginCell;
    public Cell DestinationCell;
    public List<Cell> Path;

    public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}
public class AttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Unit Defender;

    public int Damage;

    public AttackEventArgs(Unit attacker, Unit defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;

        Damage = damage;
    }
}
public class UnitCreatedEventArgs : EventArgs
{
    public Transform unit;

    public UnitCreatedEventArgs(Transform unit)
    {
        this.unit = unit;
    }
}
