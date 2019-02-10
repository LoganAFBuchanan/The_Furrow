using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;
    public Button defendButton;
    public Button NextTurnButton;
    public Button skill1button;
    public Button skill2button;
    public Button skill3button;

    public Image UnitImage;
    public Text InfoText;
    public Text StatsText;

    [System.NonSerialized]
    public HeroControl currentunit;

    //Events to cover highlighting when skill buttons are hovered upon
    public event EventHandler skill1hoverenter;
    public event EventHandler skill2hoverenter;
    public event EventHandler skill3hoverenter;

    public event EventHandler skill1hoverexit;
    public event EventHandler skill2hoverexit;
    public event EventHandler skill3hoverexit;

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
        InitializeButtons();

        OnTurnEnded(sender,e);
    }

    private void OnGameEnded(object sender, EventArgs e)
    {

        InfoText.text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + " wins!";

        CellGrid.overWorldMap.SetActive(true);
        CellGrid.overWorldNode.transform.SetParent(CellGrid.overWorldMap.transform);
        SceneManager.LoadScene(0);
    }

    public void OnTurnStarted(object sender, EventArgs e)
    {
        Debug.Log("Gui understands that the turn has started");
    }

    private void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);
        CellGrid.CheckContention();

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
        //currentunit = unit;
        StatsText.text = unit.UnitName + "\nHit Points: " + unit.HitPoints +"/"+unit.TotalHitPoints + "\nDefence: " + unit.DefenceFactor + "\nAction Points: " + unit.ActionPoints + "/" + unit.TotalActionPoints + "\nAttack: " + unit.AttackFactor + "\nDefence: " + unit.DefenceFactor + "\nRange: " + unit.AttackRange;
        UnitImage.color = unit.LeadingColor;

    }

    private void OnUnitSelected(object sender, EventArgs e)
    {
        var unit = sender as HeroControl;
        currentunit = unit;
        Debug.Log("Selected Unit is " + currentunit.UnitName);
        defendButton.gameObject.SetActive(true);
        if(currentunit.ActionPoints <= 0) defendButton.interactable = false;
        else defendButton.interactable = true;
       //Dynamically create buttons for each skill that a unit possesses. 
        if(currentunit.skillObject1 != null)
        {
            skill1button.gameObject.SetActive(true);
            skill1button.onClick.AddListener(delegate 
            { 
                currentunit.UseSkill(1, CellGrid.Cells, CellGrid.Units); 
                CellGrid.CellGridState = new CellGridStateUnitSelected(CellGrid, currentunit);
            });

            if(currentunit.ActionPoints < currentunit.skill1.actioncost) skill1button.interactable = false;
            else skill1button.interactable = true;

            skill1button.GetComponentInChildren<Text>().text = currentunit.skill1.skillname;
        }
        else skill1button.gameObject.SetActive(false);

        if(currentunit.skillObject2 != null)
        {
            skill2button.gameObject.SetActive(true);
            skill2button.onClick.AddListener(delegate 
            { 
                currentunit.UseSkill(2, CellGrid.Cells, CellGrid.Units); 
                CellGrid.CellGridState = new CellGridStateUnitSelected(CellGrid, currentunit);
            });

            if(currentunit.ActionPoints < currentunit.skill2.actioncost) skill2button.interactable = false;
            else skill2button.interactable = true;

            skill2button.GetComponentInChildren<Text>().text = currentunit.skill2.skillname;
        }
        else skill2button.gameObject.SetActive(false);

        if(currentunit.skillObject3 != null)
        {
            skill3button.gameObject.SetActive(true);
            skill3button.onClick.AddListener(delegate 
            { 
                currentunit.UseSkill(3, CellGrid.Cells, CellGrid.Units); 
                CellGrid.CellGridState = new CellGridStateUnitSelected(CellGrid, currentunit);
            });

            if(currentunit.ActionPoints < currentunit.skill3.actioncost) skill3button.interactable = false;
            else skill3button.interactable = true;
            
            skill3button.GetComponentInChildren<Text>().text = currentunit.skill3.skillname;
        }
        else skill3button.gameObject.SetActive(false);

    }


    private void OnUnitDeselected(object sender, EventArgs e)
    {
        //Avoid doubling listeners when selecting other units
        skill1button.onClick.RemoveAllListeners();
        skill2button.onClick.RemoveAllListeners();
        skill3button.onClick.RemoveAllListeners();
        InitializeButtons();
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
        unit.GetComponent<Unit>().UnitDeselected += OnUnitDeselected;
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
                currentunit.GainDefence();
            }
            
        }
        CellGrid.CellGridState = new CellGridStateUnitSelected(CellGrid, currentunit);

    }

    private void InitializeButtons()
    {
        defendButton.gameObject.SetActive(false);
        skill1button.gameObject.SetActive(false);
        skill2button.gameObject.SetActive(false);
        skill3button.gameObject.SetActive(false);
    }

    public void OnSkill1HoverEnter(){
        skill1hoverenter.Invoke(this, new EventArgs());
    }

    public void OnSkill2HoverEnter(){
        skill2hoverenter.Invoke(this, new EventArgs());
    }

    public void OnSkill3HoverEnter(){
        skill3hoverenter.Invoke(this, new EventArgs());
    }

    public void OnSkill1HoverExit(){
        skill1hoverexit.Invoke(this, new EventArgs());
    }

    public void OnSkill2HoverExit(){
        skill2hoverexit.Invoke(this, new EventArgs());
    }

    public void OnSkill3HoverExit(){
        skill3hoverexit.Invoke(this, new EventArgs());
    }
}
