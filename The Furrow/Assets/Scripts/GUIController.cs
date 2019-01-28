using System;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;
    public Button NextTurnButton;
    public Button skill1button;

    public Image UnitImage;
    public Text InfoText;
    public Text StatsText;

    private HeroControl currentunit;

    void Awake()
    {
        UnitImage.color = Color.gray;

        CellGrid.GameStarted += OnGameStarted;
        CellGrid.TurnStarted += OnTurnStarted;
        CellGrid.TurnEnded += OnTurnEnded;   
        CellGrid.GameEnded += OnGameEnded;
        CellGrid.UnitAdded += OnUnitAdded;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        foreach (Transform cell in CellGrid.transform)
        {
            cell.GetComponent<Cell>().CellHighlighted += OnCellHighlighted;
            cell.GetComponent<Cell>().CellDehighlighted += OnCellDehighlighted;
        }

        OnTurnEnded(sender,e);
    }

    private void OnGameEnded(object sender, EventArgs e)
    {
        InfoText.text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + " wins!";
    }

    public void OnTurnStarted(object sender, EventArgs e)
    {
        Debug.Log("Gui understands that the turn has started");
    }

    private void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);

        InfoText.text = "Player " + ((sender as CellGrid).CurrentPlayerNumber +1);
    }
    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        UnitImage.color = Color.gray;
        StatsText.text = "";
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        UnitImage.color = Color.gray;
        StatsText.text = "Movement Cost: " + (sender as Cell).MovementCost;
    }
    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        if (!(CellGrid.CurrentPlayer is HumanPlayer)) return;
        OnUnitDehighlighted(sender, new EventArgs());

        if ((sender as Unit).HitPoints <= 0) return;

        OnUnitHighlighted(sender, e);
    }
    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        StatsText.text = "";
        UnitImage.color = Color.gray;
    }
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        var unit = sender as HeroControl;
        currentunit = unit;
        StatsText.text = unit.UnitName + "\nHit Points: " + unit.HitPoints +"/"+unit.TotalHitPoints + "\nDefence: " + unit.DefenceFactor + "\nAction Points: " + unit.ActionPoints + "/" + unit.TotalActionPoints + "\nAttack: " + unit.AttackFactor + "\nDefence: " + unit.DefenceFactor + "\nRange: " + unit.AttackRange;
        UnitImage.color = unit.LeadingColor;

    }

    private void OnUnitSelected(object sender, EventArgs e)
    {
        var unit = sender as HeroControl;
        currentunit = unit;
        Debug.Log("Selected Unit is " + currentunit.UnitName);

       // skill1button.onClick.AddListener(currentunit.UseSkill(1));
        skill1button.onClick.AddListener(delegate { currentunit.UseSkill(1); });

    }

    private void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.unit);
    }

    private void RegisterUnit(Transform unit)
    {
        unit.GetComponent<Unit>().UnitHighlighted += OnUnitHighlighted;
        unit.GetComponent<Unit>().UnitDehighlighted += OnUnitDehighlighted;
        unit.GetComponent<Unit>().UnitAttacked += OnUnitAttacked;
        unit.GetComponent<Unit>().UnitSelected += OnUnitSelected;
    }
    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void DefendButtonClicked()
    {

        if(currentunit != null && CellGrid.CurrentPlayer is HumanPlayer && currentunit.PlayerNumber == CellGrid.CurrentPlayerNumber)
        {
            if(currentunit.ActionPoints > 0)
            {   
                currentunit.ActionPoints -= 1;
                currentunit.GainDefence();
            }
            
        }

    }
}
