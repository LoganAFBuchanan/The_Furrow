using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.EventSystems;

public class OverworldPlayer : MonoBehaviour
{

    public List<HeroControl> characterList;
    public int goldCount;
    public int rationCount;

    public int bondLevel;
    public int bondCount;
    public int bondMax;

    public MapNode currNode;

    public List<string> availableArtifacts;
    public List<Artifact> artifacts;

    [System.NonSerialized]
    public bool isBondBoosted;
    [System.NonSerialized]
    public float bondBoost;

    [System.NonSerialized]
    public bool isCampBondBoosted;
    [System.NonSerialized]
    public int campBondBoost;


    [System.NonSerialized]
    public bool isHuntBoosted;
    [System.NonSerialized]
    public int huntBoost;

    [System.NonSerialized]
    public bool isHealBoosted;
    [System.NonSerialized]
    public float healBoost;

    public event System.EventHandler NodeChanged;
    public event System.EventHandler StatsChanged;
    

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

    // Sets up the player
    public void Initialize()
    {
        availableArtifacts = new List<string>();
        artifacts = new List<Artifact>();

        isBondBoosted = false;
        bondBoost = 0;

        isCampBondBoosted = false;
        campBondBoost = 1;

        isHuntBoosted = false;
        huntBoost = 0;

        isHealBoosted = false;
        healBoost = 0;

        foreach (string name in Constants.allArtifacts)
        {
            availableArtifacts.Add(name);
        }

        characterList = new List<HeroControl>();
        foreach (Transform child in transform)
        {
            characterList.Add(child.gameObject.GetComponent<HeroControl>());

        }

        foreach (HeroControl hero in characterList)
        {
            Debug.Log(hero.UnitName);
            if (hero.gameObject.activeSelf)
            {
                hero.Initialize();
                hero.UnitAttacked += OnHeroAttacked;
                hero.combatUI.Initialize();
                hero.gameObject.SetActive(false);
            }
        }



        bondLevel = Constants.STARTING_BOND_LEVEL;
        goldCount = Constants.STARTING_GOLD;
        rationCount = Constants.STARTING_RATIONS;
        bondMax = Constants.BOND_MAX_LEVEL_1;

        DontDestroyOnLoad(this.gameObject);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MovePlayer(MapNode dest)
    {
        if (currNode == null)
        {

        }
        else
        {
            currNode.isTaken = false;
            currNode.OnDeHighlightNode();
        }

        //Ration reduction when moving
        if (rationCount > 0)
        {
            rationCount--;
        }
        else
        {
            foreach (HeroControl hero in characterList)
            {
                //Reduce character health if ration count is 0
                hero.HitPoints -= Constants.STARVING_COST;
            }
        }

        currNode = dest;
        currNode.isTaken = true;
        UpdateGUI();
        currNode.PlayerInNode();
    }

    public void ExecuteFlowchart(string startBlock)
    {

        if (!currNode.isVisited)
        {
            GetComponent<PlaySFX>().PlayFrom(Camera.main.gameObject.transform);
            SetEncounterVariables();
            GameObject.Find("OverworldCamera").GetComponent<OverworldCamera>().inEncounter = true;
            currNode.flowchart.ExecuteBlock(startBlock);
            currNode.isVisited = true;
        }
        else
        {
            //Move Darkness Forwards
        }
    }

    public void SetEncounterVariables()
    {
        //currNode.flowchart.SetStringVariable("biome", currNode.biome);
        currNode.flowchart.SetIntegerVariable("goldCount", goldCount);
        currNode.flowchart.SetIntegerVariable("rationCount", rationCount);

        currNode.flowchart.SetIntegerVariable("bondCount", bondCount);
        currNode.flowchart.SetIntegerVariable("bondMax", bondMax);
        currNode.flowchart.SetIntegerVariable("bondLevel", bondLevel);

        currNode.flowchart.SetIntegerVariable("char1Health", characterList[0].HitPoints);
        currNode.flowchart.SetIntegerVariable("char1MaxHealth", characterList[0].TotalHitPoints);

        currNode.flowchart.SetIntegerVariable("char2Health", characterList[1].HitPoints);
        currNode.flowchart.SetIntegerVariable("char2MaxHealth", characterList[1].TotalHitPoints);

        Debug.Log("CHARACTER 1 NAME IS BEFORE: " + currNode.flowchart.GetStringVariable("char1Name"));
        currNode.flowchart.SetStringVariable("char1Name", characterList[0].UnitName);
        currNode.flowchart.SetStringVariable("char2Name", characterList[1].UnitName);
        Debug.Log("CHARACTER 1 NAME IS NOW: " + currNode.flowchart.GetStringVariable("char1Name"));

        if(currNode.gameObject.name == "TitanPointEncounter")
        {
            bool hasStar = false;
            Debug.Log("It's the Lichen Encounter!");
            foreach(Artifact art in artifacts)
            {
                if(art.name == "StarShard")
                {
                    hasStar = true;
                }
            }

            if(hasStar)
            {
                currNode.flowchart.SetBooleanVariable("hasStar", true);
            }
        }

    }

    private void OnHeroAttacked(object sender, EventArgs e)
    {
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        StatsChanged.Invoke(this, new EventArgs());
    }


    public void SetGoldCount(int change)
    {
        goldCount += change;
        if (goldCount < 0) goldCount = 0;
        UpdateGUI();
    }

    public void SetRationCount(int change)
    {
        rationCount += change;
        if (rationCount < 0) rationCount = 0;
        UpdateGUI();
    }

    public void SetBondCount(int change)
    {
        bondCount += change;
        if (bondCount < 0) bondCount = 0;
        if (bondCount > bondMax) bondCount = bondMax;
        UpdateGUI();
    }

    public void SetChar1Health(int change)
    {
        characterList[0].HitPoints += change;
        if (characterList[0].HitPoints > characterList[0].TotalHitPoints) characterList[0].HitPoints = characterList[0].TotalHitPoints;
        if(characterList[0].HitPoints <= 0) characterList[0].HitPoints = 1;
        UpdateGUI();
    }

    public void SetChar2Health(int change)
    {
        characterList[1].HitPoints += change;
        if (characterList[1].HitPoints > characterList[1].TotalHitPoints) characterList[1].HitPoints = characterList[1].TotalHitPoints;
        if(characterList[1].HitPoints <= 0) characterList[1].HitPoints = 1;
        UpdateGUI();
    }

    public void SetChar1MaxHealth(int change)
    {
        characterList[0].TotalHitPoints += change;
        characterList[0].HitPoints += change;
        UpdateGUI();
    }

    public void SetChar2MaxHealth(int change)
    {
        characterList[1].TotalHitPoints += change;
        characterList[1].HitPoints += change;
        UpdateGUI();
    }

    public void SetChar1MaxAP(int change)
    {
        characterList[0].TotalActionPoints += change;
        characterList[0].ActionPoints += change;
        UpdateGUI();
    }

    public void SetChar2MaxAP(int change)
    {
        characterList[1].TotalActionPoints += change;
        characterList[1].ActionPoints += change;
        UpdateGUI();
    }

    //Pull a random artifact from available artifacts and apply its benefit to the player
    public void AddRandomArtifact()
    {
        int randomNumber = UnityEngine.Random.Range(0, availableArtifacts.Count);

        if (availableArtifacts.Count > 0)
        {
            switch (availableArtifacts[randomNumber])
            {
                case "FurtiveMushroom":
                    {
                        availableArtifacts.Remove("FurtiveMushroom");
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FurtiveMushroom");
                        break;
                    }

                case "StarShard":
                    {
                        availableArtifacts.Remove("StarShard");
                        StarShard newArtifact = new StarShard();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StarShard");
                        break;
                    }

                case "ScarredJournal":
                    {
                        availableArtifacts.Remove("ScarredJournal");
                        ScarredJournal newArtifact = new ScarredJournal();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added ScarredJournal");
                        break;
                    }

                case "SteelEgg":
                    {
                        availableArtifacts.Remove("SteelEgg");
                        SteelEgg newArtifact = new SteelEgg();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added SteelEgg");
                        break;
                    }

                case "FletcherKit":
                    {
                        availableArtifacts.Remove("FletcherKit");
                        FletcherKit newArtifact = new FletcherKit();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FletcherKit");
                        break;
                    }

                case "PatchPack":
                    {
                        availableArtifacts.Remove("PatchPack");
                        PatchPack newArtifact = new PatchPack();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added PatchPack");
                        break;
                    }

                case "MercuryDraught":
                    {
                        availableArtifacts.Remove("MercuryDraught");
                        MercuryDraught newArtifact = new MercuryDraught();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added MercuryDraught");
                        break;
                    }

                case "StoutCap":
                    {
                        availableArtifacts.Remove("StoutCap");
                        StoutCap newArtifact = new StoutCap();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StoutCap");
                        break;
                    }

                case "FurrowMorel":
                    {
                        availableArtifacts.Remove("FurrowMorel");
                        FurrowMorel newArtifact = new FurrowMorel();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FurrowMorel");
                        break;
                    }

                case "StrappedBundle":
                    {
                        availableArtifacts.Remove("StrappedBundle");
                        StrappedBundle newArtifact = new StrappedBundle();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StrappedBundle");
                        break;
                    }

                case "HarvestSack":
                    {
                        availableArtifacts.Remove("HarvestSack");
                        HarvestSack newArtifact = new HarvestSack();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added HarvestSack");
                        break;
                    }

                case "Cornucopia":
                    {
                        availableArtifacts.Remove("Cornucopia");
                        Cornucopia newArtifact = new Cornucopia();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added Cornucopia");
                        break;
                    }

                case "ConsortsScribe":
                    {
                        availableArtifacts.Remove("ConsortsScribe");
                        ConsortsScribe newArtifact = new ConsortsScribe();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added ConsortsScribe");
                        break;
                    }

                case "AegisCharm":
                    {
                        availableArtifacts.Remove("AegisCharm");
                        AegisCharm newArtifact = new AegisCharm();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added AegisCharm");
                        break;
                    }
                
                case "SanguineFang":
                    {
                        availableArtifacts.Remove("SanguineFang");
                        SanguineFang newArtifact = new SanguineFang();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added SanguineFang");
                        break;
                    }

                default:
                    {
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added Default Artifact");
                        break;
                    }
            }

        }
        else
        {
            FurtiveMushroom newArtifact = new FurtiveMushroom();
            artifacts.Add(newArtifact);
            newArtifact.Apply(this);
            GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
            UpdateGUI();
            Debug.Log("Added Default Artifact");

        }
    }

    public void AddArtifact(PointerEventData data, string name)
    {
        switch (name)
            {
                case "ShardedFlask":
                    {
                        availableArtifacts.Remove("FurtiveMushroom");
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FurtiveMushroom");
                        break;
                    }

                case "StarShard":
                    {
                        availableArtifacts.Remove("StarShard");
                        StarShard newArtifact = new StarShard();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StarShard");
                        break;
                    }

                case "CompatriotsLink":
                    {
                        availableArtifacts.Remove("ScarredJournal");
                        ScarredJournal newArtifact = new ScarredJournal();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added ScarredJournal");
                        break;
                    }

                case "TitanstoneShield":
                    {
                        availableArtifacts.Remove("SteelEgg");
                        SteelEgg newArtifact = new SteelEgg();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added SteelEgg");
                        break;
                    }

                case "TravelersTrap":
                    {
                        availableArtifacts.Remove("FletcherKit");
                        FletcherKit newArtifact = new FletcherKit();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FletcherKit");
                        break;
                    }

                case "HealersHarp":
                    {
                        availableArtifacts.Remove("PatchPack");
                        PatchPack newArtifact = new PatchPack();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added PatchPack");
                        break;
                    }

                case "DraughtofGoodHaste":
                    {
                        availableArtifacts.Remove("MercuryDraught");
                        MercuryDraught newArtifact = new MercuryDraught();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added MercuryDraught");
                        break;
                    }

                case "ShardedTankard":
                    {
                        availableArtifacts.Remove("StoutCap");
                        StoutCap newArtifact = new StoutCap();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StoutCap");
                        break;
                    }

                case "CurativeCuff":
                    {
                        availableArtifacts.Remove("FurrowMorel");
                        FurrowMorel newArtifact = new FurrowMorel();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added FurrowMorel");
                        break;
                    }

                case "SpiritdDish":
                    {
                        availableArtifacts.Remove("StrappedBundle");
                        StrappedBundle newArtifact = new StrappedBundle();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added StrappedBundle");
                        break;
                    }

                case "SpiritdPlatter":
                    {
                        availableArtifacts.Remove("HarvestSack");
                        HarvestSack newArtifact = new HarvestSack();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added HarvestSack");
                        break;
                    }

                case "SpiritdCornucopia":
                    {
                        availableArtifacts.Remove("Cornucopia");
                        Cornucopia newArtifact = new Cornucopia();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added Cornucopia");
                        break;
                    }

                case "FlamepaintingWand":
                    {
                        availableArtifacts.Remove("ConsortsScribe");
                        ConsortsScribe newArtifact = new ConsortsScribe();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added ConsortsScribe");
                        break;
                    }

                case "AegisCharm":
                    {
                        availableArtifacts.Remove("AegisCharm");
                        AegisCharm newArtifact = new AegisCharm();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added AegisCharm");
                        break;
                    }

                case "SanguineFang":
                    {
                        availableArtifacts.Remove("SanguineFang");
                        SanguineFang newArtifact = new SanguineFang();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added SanguineFang");
                        break;
                    }

                default:
                    {
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        artifacts.Add(newArtifact);
                        newArtifact.Apply(this);
                        GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().AddArtifact(newArtifact);
                        UpdateGUI();
                        Debug.Log("Added Default Artifact");
                        break;
                    }
            }
    }

    public void RemoveArtifact(string name)
    {
        foreach(Artifact art in artifacts)
        {
            if(art.name == name)
            {
                Debug.Log("Player: Removing " + name);
                art.Undo(this);
                GameObject.Find("OverworldUI").GetComponent<OverworldGUI>().RemoveArtifact(art);
            }
        }
    }

    //Return a random artifact from available artifacts
    public Artifact GetRandomArtifact()
    {
        int randomNumber = UnityEngine.Random.Range(0, availableArtifacts.Count);

        Debug.Log("THE SHOPS RANDOM NUMBER IS: " + randomNumber);

        if (availableArtifacts.Count > 0)
        {
            switch (availableArtifacts[randomNumber])
            {
                case "FurtiveMushroom":
                    {
                        availableArtifacts.Remove("FurtiveMushroom");
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        return newArtifact;
                        break;
                    }
                
                case "StarShard":
                    {
                        availableArtifacts.Remove("StarShard");
                        StarShard newArtifact = new StarShard();
                        return newArtifact;
                        break;
                    }

                case "ScarredJournal":
                    {
                        availableArtifacts.Remove("ScarredJournal");
                        ScarredJournal newArtifact = new ScarredJournal();
                        return newArtifact;
                        break;
                    }

                case "SteelEgg":
                    {
                        availableArtifacts.Remove("SteelEgg");
                        SteelEgg newArtifact = new SteelEgg();
                        return newArtifact;
                        break;
                    }

                case "FletcherKit":
                    {
                        availableArtifacts.Remove("FletcherKit");
                        FletcherKit newArtifact = new FletcherKit();
                        return newArtifact;
                        break;
                    }

                case "PatchPack":
                    {
                        availableArtifacts.Remove("PatchPack");
                        PatchPack newArtifact = new PatchPack();
                        return newArtifact;
                        break;
                    }

                case "MercuryDraught":
                    {
                        availableArtifacts.Remove("MercuryDraught");
                        MercuryDraught newArtifact = new MercuryDraught();
                        return newArtifact;
                        break;
                    }

                case "StoutCap":
                    {
                        availableArtifacts.Remove("StoutCap");
                        StoutCap newArtifact = new StoutCap();
                        return newArtifact;
                        break;
                    }

                case "FurrowMorel":
                    {
                        availableArtifacts.Remove("FurrowMorel");
                        FurrowMorel newArtifact = new FurrowMorel();
                        return newArtifact;
                        break;
                    }

                case "StrappedBundle":
                    {
                        availableArtifacts.Remove("StrappedBundle");
                        StrappedBundle newArtifact = new StrappedBundle();
                        return newArtifact;
                        break;
                    }

                case "HarvestSack":
                    {
                        availableArtifacts.Remove("HarvestSack");
                        HarvestSack newArtifact = new HarvestSack();
                        return newArtifact;
                        break;
                    }

                case "Cornucopia":
                    {
                        availableArtifacts.Remove("Cornucopia");
                        Cornucopia newArtifact = new Cornucopia();
                        return newArtifact;
                        break;
                    }

                case "ConsortsScribe":
                    {
                        availableArtifacts.Remove("ConsortsScribe");
                        ConsortsScribe newArtifact = new ConsortsScribe();
                        return newArtifact;
                        break;
                    }

                case "AegisCharm":
                    {
                        availableArtifacts.Remove("AegisCharm");
                        AegisCharm newArtifact = new AegisCharm();
                        return newArtifact;
                        break;
                    }

                case "SanguineFang":
                    {
                        availableArtifacts.Remove("SanguineFang");
                        SanguineFang newArtifact = new SanguineFang();
                        return newArtifact;
                        break;
                    }

                default:
                    {
                        FurtiveMushroom newArtifact = new FurtiveMushroom();
                        return newArtifact;
                        Debug.Log("Added Default Artifact");
                        break;
                    }
            }

        }
        else
        {
            FurtiveMushroom newArtifact = new FurtiveMushroom();
            return newArtifact;
            Debug.Log("Added Default Artifact");

        }
    }


    public void GetEncounterVariables()
    {
        //currNode.biome = currNode.flowchart.GetStringVariable("biome");
        goldCount = currNode.flowchart.GetIntegerVariable("goldCount");
        rationCount = currNode.flowchart.GetIntegerVariable("rationCount");
        //currNode.flowchart.GetStringVariable("char1Name");
        //currNode.flowchart.GetStringVariable("char2Name");
        UpdateGUI();
    }




}