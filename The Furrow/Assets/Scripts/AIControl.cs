using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{

    public List<AIBehaviour> actionList;

    private int step;

    [System.NonSerialized]
    public CellGrid _cellGrid;

    private HeroControl myUnit;

    // Start is called before the first frame update
    void Awake()
    {
        step = 0;
        myUnit = this.gameObject.GetComponent<HeroControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayNextBehaviour()
    {
        actionList[step].EnactBehaviour(_cellGrid, myUnit);

        step++;
        if(step >= actionList.Count)
        {
            step = 0;
        }
    }
}
