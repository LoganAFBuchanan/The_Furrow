using System.Linq;
using UnityEngine;

public abstract class CellGridState
{
    protected CellGrid _cellGrid;
    
    protected CellGridState(CellGrid cellGrid)
    {
        _cellGrid = cellGrid;
    }

    /// <summary>
    /// Method is called when a unit is clicked on.
    /// </summary>
    /// <param name="unit">Unit that was clicked.</param>
    public virtual void OnUnitClicked(Unit unit)
    {
        Debug.Log((unit as HeroControl).UnitName + "Has Been Clicked!" );
        foreach(Unit u in _cellGrid.Units)
        {
            u.UnMark();
        }
        unit.MarkAsSelected();
    }
    
    /// <summary>
    /// Method is called when mouse exits cell's collider.
    /// </summary>
    /// <param name="cell">Cell that was deselected.</param>
    public virtual void OnCellDeselected(Cell cell)
    {
        cell.UnMark();
    }

    /// <summary>
    /// Method is called when mouse enters cell's collider.
    /// </summary>
    /// <param name="cell">Cell that was selected.</param>
    public virtual void OnCellSelected(Cell cell)
    {
        cell.MarkAsHighlighted();
    }

    /// <summary>
    /// Method is called when a cell is clicked.
    /// </summary>
    /// <param name="cell">Cell that was clicked.</param>
    public virtual void OnCellClicked(Cell cell)
    {
        //_cellGrid.CheckContention();
    }

    public virtual void OnUnitMoved(Unit unit)
    {
        _cellGrid.CheckContention();
    }

    /// <summary>
    /// Method is called on transitioning into a state.
    /// </summary>
    public virtual void OnStateEnter()
    {
        if (_cellGrid.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
        {
            _cellGrid.CellGridState = new CellGridStateGameOver(_cellGrid);
        }
    }

    /// <summary>
    /// Method is called on transitioning out of a state.
    /// </summary>
    public virtual void OnStateExit()
    {
    }
}