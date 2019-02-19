using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{

    public Skill skill;
    public BehaviourType behaviourType;
    private System.Random _rnd;

    private int enemyPlayerNumber = 0; //Number of the human player
   
    
    public enum BehaviourType
    {
        AGGRESSIVE_MOVE,
        DEFENSIVE_MOVE,
        AGGRESIVE_MOVE_SKILL,
        DEFENSIVE_MOVE_SKILL,
        SKILL
    }

    public AIBehaviour()
    {
        _rnd = new System.Random();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnactBehaviour(CellGrid cellGrid, HeroControl myUnit)
    {
        Debug.Log("Enacting behaviour of type: " + behaviourType);

        switch(behaviourType)
        {
            case BehaviourType.AGGRESSIVE_MOVE:
                StartCoroutine(MoveTowardsPlayer(cellGrid, myUnit));
                break;
            
            case BehaviourType.DEFENSIVE_MOVE:
                StartCoroutine(MoveAwayFromPlayer(cellGrid, myUnit));
                break;
        }
        
    }



    public IEnumerator MoveTowardsPlayer(CellGrid _cellGrid, HeroControl unit)
    {
        var enemyUnits = _cellGrid.Units.FindAll(u => u.PlayerNumber.Equals(enemyPlayerNumber)).ToList();
        List<Cell> potentialDestinations = new List<Cell>();
            
            foreach (var enemyUnit in enemyUnits)
            {
                potentialDestinations.AddRange(_cellGrid.Cells.FindAll(c=> unit.IsCellMovableTo(c) && unit.IsUnitAttackable(enemyUnit, c, skill))); 
            }//Making a list of cells that the unit can attack from.
      
            var notInRange = potentialDestinations.FindAll(c => c.GetDistance(unit.Cell) > unit.ActionPoints);
            potentialDestinations = potentialDestinations.Except(notInRange).ToList();

            if (potentialDestinations.Count == 0 && notInRange.Count !=0)
            {
                potentialDestinations.Add(notInRange.ElementAt(_rnd.Next(0,notInRange.Count-1)));
            }     

            if(potentialDestinations.Count == 0)
            {
                potentialDestinations.AddRange(_cellGrid.Cells.FindAll(c => (c as CombatTile).tiletype == CombatTile.TileType.CONTESTED && unit.IsCellMovableTo(c)));
            }

            potentialDestinations = potentialDestinations.OrderBy(h => _rnd.Next()).ToList();
            List<Cell> shortestPath = null;
            foreach (var potentialDestination in potentialDestinations)
            {
                var path = unit.FindPath(_cellGrid.Cells, potentialDestination);
                if ((shortestPath == null && path.Sum(h => h.MovementCost) > 0) || shortestPath != null && (path.Sum(h => h.MovementCost) < shortestPath.Sum(h => h.MovementCost) && path.Sum(h => h.MovementCost) > 0))
                    shortestPath = path;

                var pathCost = path.Sum(h => h.MovementCost);
                if (pathCost > 0 && pathCost <= unit.MovementPoints)
                {
                    unit.Move(potentialDestination, path);
                    while (unit.isMoving)
                        yield return 0;
                    shortestPath = null;
                    break;
                }
                yield return 0;
            }//If there is a path to any cell that the unit can attack from, move there.

            if (shortestPath != null)
            {      
                foreach (var potentialDestination in shortestPath.Intersect(unit.GetAvailableDestinations(_cellGrid.Cells)).OrderByDescending(h => h.GetDistance(unit.Cell)))
                {
                    var path = unit.FindPath(_cellGrid.Cells, potentialDestination);
                    var pathCost = path.Sum(h => h.MovementCost);
                    if (pathCost > 0 && pathCost <= unit.MovementPoints)
                    {
                        unit.Move(potentialDestination, path);
                        while (unit.isMoving)
                            yield return 0;
                        break;
                    }
                    yield return 0;
                }
            }//If the path cost is greater than unit movement points, move as far as possible.

    }

    public IEnumerator MoveAwayFromPlayer(CellGrid _cellGrid, HeroControl unit)
    {
        var enemyUnits = _cellGrid.Units.FindAll(u => u.PlayerNumber.Equals(enemyPlayerNumber)).ToList();
        List<Cell> potentialDestinations = new List<Cell>();
            
                 
            var notInRange = potentialDestinations.FindAll(c => c.GetDistance(unit.Cell) > unit.ActionPoints);

            potentialDestinations.AddRange(_cellGrid.Cells.FindAll(c => (c as CombatTile).transform.position.x >= 7 && unit.IsCellMovableTo(c)));
            

            potentialDestinations = potentialDestinations.OrderBy(h => _rnd.Next()).ToList();
            List<Cell> shortestPath = null;
            foreach (var potentialDestination in potentialDestinations)
            {
                var path = unit.FindPath(_cellGrid.Cells, potentialDestination);
                if ((shortestPath == null && path.Sum(h => h.MovementCost) > 0) || shortestPath != null && (path.Sum(h => h.MovementCost) < shortestPath.Sum(h => h.MovementCost) && path.Sum(h => h.MovementCost) > 0))
                    shortestPath = path;

                var pathCost = path.Sum(h => h.MovementCost);
                if (pathCost > 0 && pathCost <= unit.MovementPoints)
                {
                    unit.Move(potentialDestination, path);
                    while (unit.isMoving)
                        yield return 0;
                    shortestPath = null;
                    break;
                }
                yield return 0;
            }//If there is a path to any cell that the unit can attack from, move there.

            if (shortestPath != null)
            {      
                foreach (var potentialDestination in shortestPath.Intersect(unit.GetAvailableDestinations(_cellGrid.Cells)).OrderByDescending(h => h.GetDistance(unit.Cell)))
                {
                    var path = unit.FindPath(_cellGrid.Cells, potentialDestination);
                    var pathCost = path.Sum(h => h.MovementCost);
                    if (pathCost > 0 && pathCost <= unit.MovementPoints)
                    {
                        unit.Move(potentialDestination, path);
                        while (unit.isMoving)
                            yield return 0;
                        break;
                    }
                    yield return 0;
                }
            }//If the path cost is greater than unit movement points, move as far as possible.

    }
}
