using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{

    public Skill skill;
    public BehaviourType behaviourType;

    private int enemyPlayerNumber = 0; //Number of the human player
   
    
    public enum BehaviourType
    {
        MOVE_SKILL,
        MOVE,
        SKILL
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
        myUnit = 

        if(behaviourType == BehaviourType.MOVE)
        {
            MoveTowardsPlayer(cellGrid, myUnit);
        }
    }



    public void MoveTowardsPlayer(CellGrid cellGrid, HeroControl myUnit)
    {
        var enemyUnits = cellGrid.Units.FindAll(u => u.PlayerNumber.Equals(enemyPlayerNumber)).ToList();

        List<Cell> potentialDestinations = new List<Cell>();
            
            foreach (var enemyUnit in enemyUnits)
            {
                potentialDestinations.AddRange(_cellGrid.Cells.FindAll(c=> unit.IsCellMovableTo(c) && unit.IsUnitAttackable(enemyUnit, c))); 
            }//Making a list of cells that the unit can attack from.
      
            var notInRange = potentialDestinations.FindAll(c => c.GetDistance(unit.Cell) > unit.MovementPoints);
            potentialDestinations = potentialDestinations.Except(notInRange).ToList();

            if (potentialDestinations.Count == 0 && notInRange.Count !=0)
            {
                potentialDestinations.Add(notInRange.ElementAt(_rnd.Next(0,notInRange.Count-1)));
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
}
