using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillname;
    public int actioncost;
    

    public int[] skillTargetX;
    public int[] skillTargetY;

    public void Awake()
    {
        //skillTargetX = new int[3]{ 2, 2, 2 };
        //skillTargetY = new int[3]{ 1, 0, -1 };
    }

    public void UseSkill()
    {

    }

    // public HashSet<Cell> GetAvailableTargets(List<Cell> cells)
    // {
    //     cachedTargets = new Dictionary<Cell, List<Cell>>();
        
    //     var paths = cachePaths(cells);
    //     foreach (var key in paths.Keys)
    //     {
    //         if (!IsCellMovableTo(key))
    //             continue;
    //         var path = paths[key];

    //         var pathCost = path.Sum(c => c.MovementCost);
    //         if (pathCost <= ActionPoints)
    //         {
    //             cachedTargets.Add(key, path);
    //         }
    //     }
    //     return new HashSet<Cell>(cachedTargets.Keys);
    // }

}