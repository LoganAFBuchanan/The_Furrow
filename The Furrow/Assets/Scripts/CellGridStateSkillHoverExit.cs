using System.Collections.Generic;
using System.Linq;

class CellGridStateSkillHoverExit : CellGridState
{

    //This state will have to clear all cells of their targetable highlights.

    private CellGrid cellGrid;

    public CellGridStateSkillHoverExit(CellGrid __cellGrid) : base(__cellGrid)
    {
        cellGrid = __cellGrid;
    }

    public override void OnStateEnter()
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid); 
    }
}