using System.Collections.Generic;
using System.Linq;

class CellGridStateSkillHover : CellGridState
{

    private CellGrid cellGrid;
    private Skill hoveredSkill;
    private HeroControl unit;

    public CellGridStateSkillHover(CellGrid __cellGrid, HeroControl _unit, Skill _hoveredSkill) : base(__cellGrid)
    {
        cellGrid = __cellGrid;
        hoveredSkill = _hoveredSkill;
        unit = _unit;
    }

    public override void OnStateEnter()
    {

    }
}