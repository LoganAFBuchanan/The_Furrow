using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class CellGrid : MonoBehaviour
{
    /// <summary>
    /// LevelLoading event is invoked before Initialize method is run.
    /// </summary>
    public event EventHandler LevelLoading;
    /// <summary>
    /// LevelLoadingDone event is invoked after Initialize method has finished running.
    /// </summary>
    public event EventHandler LevelLoadingDone;
    /// <summary>
    /// GameStarted event is invoked at the beggining of StartGame method.
    /// </summary>
    public event EventHandler GameStarted;
    /// <summary>
    /// GameEnded event is invoked when there is a single player left in the game.
    /// </summary>
    public event EventHandler GameEnded;
    /// <summary>
    /// Turnstarted is invoked at the begining of the game and at the start of every turn
    /// </summary>
    public event EventHandler TurnStarted;
    /// <summary>
    /// Turn ended event is invoked at the end of each turn.
    /// </summary>
    public event EventHandler TurnEnded;
    
    [System.NonSerialized]
    public GameObject overWorldNode;
    [System.NonSerialized]
    public GameObject overWorldMap;

    /// <summary>
    /// UnitAdded event is invoked each time AddUnit method is called.
    /// </summary>
    public event EventHandler<UnitCreatedEventArgs> UnitAdded;
    
    private CellGridState _cellGridState; //The grid delegates some of its behaviours to cellGridState object.
    public CellGridState CellGridState
    {
        private get
        {
            return _cellGridState;
        }
        set
        {
            if(_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public Player CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }

    /// <summary>
    /// GameObject that holds player objects.
    /// </summary>
    public Transform PlayersParent;

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }

    public GUIController guiController;

    private void Start()
    {
        //_cellGridState = new CellGridState(this);
        PopulateUnits();

        if (LevelLoading != null)
            LevelLoading.Invoke(this, new EventArgs());

        Initialize();

        if (LevelLoadingDone != null)
            LevelLoadingDone.Invoke(this, new EventArgs());

        StartGame();
        (Players[1] as AIPlayer).InitializeAI(this);
        TurnStarted.Invoke(this, new EventArgs());
    }


    private void PopulateUnits()
    {
        GameObject unitList = GameObject.Find("Units");

        overWorldMap = GameObject.Find("MapControl");
        overWorldMap.SetActive(false);
        
        //Add Heroes
        if(GameObject.Find("Player"))
        {
            GameObject overWorldPlayer = GameObject.Find("Player");
            int listSize = overWorldPlayer.transform.childCount;

            for(int i = 0; i < listSize; i++)
            {
                
                if(overWorldPlayer.transform.GetChild(0).gameObject.GetComponent<HeroControl>().isDead)
                {
                    overWorldPlayer.transform.GetChild(0).gameObject.GetComponent<HeroControl>().SetCharacterDeath(true);
                }
                else
                {
                    overWorldPlayer.transform.GetChild(0).gameObject.GetComponent<HeroControl>().SetCharacterDeath(false);
                }
                //overWorldPlayer.transform.GetChild(0).gameObject.SetActive(true);
                overWorldPlayer.transform.GetChild(0).SetParent(unitList.transform);
                
            }
        }

        //Add Enemies
        if(GameObject.FindGameObjectsWithTag("MapNode").Length > 0)
        {
            overWorldNode = GameObject.FindGameObjectsWithTag("MapNode")[0];
            overWorldNode.GetComponent<MapNode>().image.SetActive(false);
            int listSize = overWorldNode.transform.childCount;

            for(int i = 0; i < listSize; i++)
            {
                    if(overWorldNode.transform.GetChild(0).gameObject.tag =="Enemy")
                    {
                        overWorldNode.transform.GetChild(0).gameObject.SetActive(true);
                        overWorldNode.transform.GetChild(0).SetParent(unitList.transform);
                    }

                
            }
            overWorldNode.SetActive(false);
        }
    }

    private void Initialize()
    {
        Players = new List<Player>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);
        guiController = GameObject.Find("GUIController").GetComponent<GUIController>();

        guiController.skill1hoverenter += OnSkill1HoverEnter;
        guiController.skill1hoverexit += OnSkill1HoverExit;
        guiController.skill2hoverenter += OnSkill2HoverEnter;
        guiController.skill2hoverexit += OnSkill2HoverExit;
        guiController.skill3hoverenter += OnSkill3HoverEnter;
        guiController.skill3hoverexit += OnSkill3HoverExit;

        Cells = new List<Cell>();
        Debug.Log("ChildCount of Cellgrid: " + transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
                Cells.Add(cell);
            else
                Debug.LogError("Invalid object in cells paretn game object");
        }

        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
            cell.GetComponent<Cell>().GetNeighbours(Cells);
            

            if((cell as CombatTile).tiletype != null)
            {
                if((cell as CombatTile).tiletype == CombatTile.TileType.ALLY)
                {
                    (cell as CombatTile).tileteam = "ALLY";
                    (cell as CombatTile).MovementCost = 1;
                    if((cell as CombatTile).isSlimed) (cell as CombatTile).MovementCost += 1;
                }else if((cell as CombatTile).tiletype == CombatTile.TileType.ENEMY)
                {
                    (cell as CombatTile).tileteam = "ENEMY";
                    (cell as CombatTile).MovementCost = 1;
                    if((cell as CombatTile).isSlimed) (cell as CombatTile).MovementCost += 1;
                }else if((cell as CombatTile).tiletype == CombatTile.TileType.CONTESTED)
                {
                    (cell as CombatTile).tileteam = "CONTESTED";
                    (cell as CombatTile).MovementCost = 2;
                    if((cell as CombatTile).isSlimed) (cell as CombatTile).MovementCost += 1;
                }
            }
            
            
        }

        
        

        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(Cells);
            foreach (var unit in Units)
            {
                AddUnit(unit.GetComponent<Transform>());
                
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");

        
        CheckContention();
    }

    private void OnUnitMoved(object sender, EventArgs e)
    {
        CellGridState.OnUnitMoved(sender as Unit);
    }

    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as Cell);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);

        //If AI then remove from ai controller list
        if((sender as Unit).PlayerNumber == 1)
        {
            Players[1].GetComponent<AIPlayer>().aiControllers.Remove((sender as Unit).GetComponent<AIControl>());
        }

        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
            {
                GameEndCleanup();
                GameEnded.Invoke(this, new EventArgs());
            }
                
        }
        CheckContention();
    }

    //Move the players out of units and back into the player object
    public void GameEndCleanup()
    {
        GameObject unitList = GameObject.Find("Units");

        GameObject overWorldPlayer = GameObject.Find("Player");
        int listSize = unitList.transform.childCount;

        int j = 0;
        for(int i = 0; i < listSize; i++)
        {
            if(unitList.transform.GetChild(j).gameObject.tag == "PlayerHero")
            {
                //Reset Position of heroes after fight
                if(unitList.transform.GetChild(j).GetComponent<HeroControl>().UnitName == "Aldric") unitList.transform.GetChild(j).transform.position = Constants.ALDRIC_START_POS;
                if(unitList.transform.GetChild(j).GetComponent<HeroControl>().UnitName == "Ide") unitList.transform.GetChild(j).transform.position = Constants.IDE_START_POS;

                unitList.transform.GetChild(j).gameObject.SetActive(false);
                unitList.transform.GetChild(j).SetParent(overWorldPlayer.transform);
            }
            else
            {
                j++;
            }
            
                
        }

        foreach (var cell in Cells)
        {
            cell.CellClicked -= OnCellClicked;
            cell.CellHighlighted -= OnCellHighlighted;
            cell.CellDehighlighted -= OnCellDehighlighted;
        }
        overWorldNode.GetComponent<MapNode>().image.SetActive(true);
        overWorldMap.SetActive(true);
        overWorldNode.SetActive(true);
        overWorldNode.GetComponent<MapNode>().GiveCombatRewards(overWorldPlayer.GetComponent<OverworldPlayer>());
        overWorldNode.transform.SetParent(overWorldMap.transform);
        overWorldNode.transform.position = overWorldNode.GetComponent<MapNode>().savedPosition;
    }

    public void CheckContention()
    {   
        List<CombatTile> contestedCells = GetContestedCells();
        List<Unit> contestedUnits = new List<Unit>();

        //Add Units that are within contested territory
        foreach(CombatTile cell in contestedCells)
        {
            foreach(Unit unit in Units)
            {
                if(unit.Cell.OffsetCoord == cell.OffsetCoord)
                {
                    contestedUnits.Add(unit);
                }
            }
        }

        if(contestedUnits.Count > 0)
        {   
            //Check that all contested units are on the same team
            int takingTeam = contestedUnits[0].PlayerNumber; 
            bool isContested = false;
            float captureColumn = (contestedUnits[0].Cell as CombatTile).OffsetCoord.x;
            foreach(Unit unit in contestedUnits)
            {
                if(unit.PlayerNumber == takingTeam)
                {

                }
                else
                {
                    //If there is a discrepancy then set contested to true and abort capture
                    isContested = true;
                }
            }

            if(isContested)
            {
                Debug.Log("Row still Contested, no capture needed");
            }
            else if(!isContested)
            {
                //If contested stays true and all units are on the same team, then initiate column capture of contested row
                Debug.Log("Capturing Column: " + captureColumn);
                SetContestedColumn((int)captureColumn, takingTeam);
            }
        }


        //Debug.Log(contestedUnits);
    }

    public List<CombatTile> GetContestedCells()
    {
        List<CombatTile> contestedCells = new List<CombatTile>();
        foreach(CombatTile cell in Cells)
        {
            if((cell as CombatTile).tileteam == "CONTESTED") contestedCells.Add((cell as CombatTile)); 
        }
        return contestedCells;
    }

    public void SetContestedColumn(int column, int team)
    {

        int captureDir = 0;
        

        if(team == 0)
        {
            captureDir = 1;
        }
        else if(team == 1)
        {
            captureDir = -1;
        }

        column += captureDir;

        Debug.Log("Contested column at " + column);

        foreach(CombatTile cell in Cells)
        {
            if(cell.tileteam == "CONTESTED")
            {
                if(team == 0)
                {
                    cell.tileteam = "ALLY";
                    cell.tiletype = CombatTile.TileType.ALLY;
                    cell.MovementCost = 1;
                }
                else if (team == 1)
                {
                    cell.tileteam = "ENEMY";
                    cell.tiletype = CombatTile.TileType.ENEMY;
                    cell.MovementCost = 1;
                }
            }

            Debug.Log("Cell x coord is: " + cell.OffsetCoord.x + ". And Column is: " + column);

            if(cell.OffsetCoord.x == column)
            {
                cell.tileteam = "CONTESTED";
                cell.tiletype = CombatTile.TileType.CONTESTED;
                cell.MovementCost = 2;
            }
            
        }


    }

    /// <summary>
    /// Adds unit to the game
    /// </summary>
    /// <param name="unit">Unit to add</param>
    public void AddUnit(Transform unit)
    {
        unit.GetComponent<Unit>().UnitClicked += OnUnitClicked;
        unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;
        unit.GetComponent<Unit>().UnitMoved += OnUnitMoved;

        if(UnitAdded != null)
            UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit)); 
    }

    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        if(GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);

        //Apply start of combat effect (ie: Artifacts)
        foreach(Unit unit in Units)
        {
            if(unit.startingDefence != null) unit.DefenceFactor += unit.startingDefence; //Steel egg implementation
            if(unit.startingAPBoost != null) unit.ActionPoints += unit.startingAPBoost; //Mercury Draught implementation
        }

        foreach(Cell cell in Cells)
        {
            cell.UnMark();
        }


    }
    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        if(CurrentPlayerNumber == 0)GetComponent<PlaySFX>().PlayFrom(Camera.main.gameObject.transform);
        CellGridState = new CellGridStateTurnChanging(this);

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        }//Skipping players that are defeated.

        foreach(Cell cell in Cells)
        {
            (cell as CombatTile).DecreaseEffectLifetimes();
        }

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);     
        TurnStarted.Invoke(this, new EventArgs());
    }

    private void OnSkill1HoverEnter(object sender, EventArgs e){
        Skill playerSkill = (sender as GUIController).currentunit.skill1;

        //CellGridState = new CellGridStateSkillHover(this, (sender as GUIController).currentunit, playerSkill);
        Debug.Log(playerSkill.skillname + " hovered");
        if(CellGridState is CellGridStateUnitSelected)
        {
            if (playerSkill.targetAllAllies && playerSkill.targetAllEnemies)
            {
                //Highlight all units on the map
                foreach(Unit unit in Units)
                {
                    unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllEnemies)
            {
                //Highlight all enemies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber != (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllAllies)
            {
                //Highlight all allies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber == (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else
            {
                List<Cell> targetCells = (sender as GUIController).currentunit.GetAvailableTargets(Cells, playerSkill);

                foreach(Cell cell in targetCells)
                {
                    cell.MarkAsTargetable();
                }
            }
        }

        
    }

    private void OnSkill2HoverEnter(object sender, EventArgs e){
        Skill playerSkill = (sender as GUIController).currentunit.skill2;

        //CellGridState = new CellGridStateSkillHover(this, (sender as GUIController).currentunit, playerSkill);
        Debug.Log(playerSkill.skillname + " hovered");
        if(CellGridState is CellGridStateUnitSelected)
        {
            if (playerSkill.targetAllAllies && playerSkill.targetAllEnemies)
            {
                //Highlight all units on the map
                foreach(Unit unit in Units)
                {
                    unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllEnemies)
            {
                //Highlight all enemies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber != (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllAllies)
            {
                //Highlight all allies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber == (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else
            {
                List<Cell> targetCells = (sender as GUIController).currentunit.GetAvailableTargets(Cells, playerSkill);

                foreach(Cell cell in targetCells)
                {
                    cell.MarkAsTargetable();
                }
            }
        }
    }

    private void OnSkill3HoverEnter(object sender, EventArgs e){
        Skill playerSkill = (sender as GUIController).currentunit.skill3;

        //CellGridState = new CellGridStateSkillHover(this, (sender as GUIController).currentunit, playerSkill);
        Debug.Log(playerSkill.skillname + " hovered");
        if(CellGridState is CellGridStateUnitSelected)
        {
            if (playerSkill.targetAllAllies && playerSkill.targetAllEnemies)
            {
                //Highlight all units on the map
                foreach(Unit unit in Units)
                {
                    unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllEnemies)
            {
                //Highlight all enemies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber != (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else if (playerSkill.targetAllAllies)
            {
                //Highlight all allies
                foreach(Unit unit in Units)
                {
                    if(unit.PlayerNumber == (sender as GUIController).currentunit.PlayerNumber) unit.Cell.MarkAsTargetable();
                }
            }
            else
            {
                List<Cell> targetCells = (sender as GUIController).currentunit.GetAvailableTargets(Cells, playerSkill);

                foreach(Cell cell in targetCells)
                {
                    cell.MarkAsTargetable();
                }
            }
        }
    }

    private void OnSkill1HoverExit(object sender, EventArgs e){
        Debug.Log("Skill 1 Exit");
        Unit __unit = (sender as GUIController).currentunit;
        CellGridState = new CellGridStateUnitSelected(this, __unit);
    }

    private void OnSkill2HoverExit(object sender, EventArgs e){
        Debug.Log("Skill 2 Exit");
        Unit __unit = (sender as GUIController).currentunit;
        CellGridState = new CellGridStateUnitSelected(this, __unit);
    }

    private void OnSkill3HoverExit(object sender, EventArgs e){
        Debug.Log("Skill 3 Exit");
        Unit __unit = (sender as GUIController).currentunit;
        CellGridState = new CellGridStateUnitSelected(this, __unit);
    }
}
