using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    //Default action list
    public List<AIBehaviour> actionList;

    //Special trigger condition
    public BehaviourTrigger triggeredBehaviour;
    private bool isTriggered = false;
    //Action list that begins once a special condition has been met
    public List<AIBehaviour> triggeredActionList;

    public enum BehaviourTrigger
    {
        NONE,
        LAST_ALLY_ALIVE
    }

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

    //Determines which behaviour list should be played through
    public void PlayNextBehaviour()
    {
        if(isTriggered)
        {
            triggeredActionList[step].EnactBehaviour(_cellGrid, myUnit);

            step++;
            if(step >= triggeredActionList.Count)
            {
                step = 0;
            }
        }
        else
        {
            actionList[step].EnactBehaviour(_cellGrid, myUnit);

            step++;
            if(step >= actionList.Count)
            {
                step = 0;
            }
        }
        
    }

    //Checks special conditions for the behaviour list to be changed
    private void CheckTriggers()
    {
        if(!isTriggered)
        {
            switch(triggeredBehaviour)
            {
                case BehaviourTrigger.NONE:
                    break;
                
                case BehaviourTrigger.LAST_ALIVE:
                    int allyCount = 0;
                    foreach(Unit unit in _cellGrid.Units)
                    {
                        if(unit.PlayerNumber == myUnit.PlayerNumber) allyCount++;
                    }
                    if(allyCount > 1) TriggerSpecialBehaviour();
                    break;
            }
        }
    }
    
    //Called whenever special trigger conditions are met
    private void TriggerSpecialBehaviour()
    {
        isTriggered = true;
        step = 0;
    }
}
