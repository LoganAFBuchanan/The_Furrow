using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.SceneManagement;
using SpriteGlow;

public class MapNode : MonoBehaviour
{

    public string biome;
    public int encounterID;
    public int worldTier;
    public int tierPosition;
    public bool isUnique;

    //Has the node been touched by the shadow
    public bool isCorrupted;

    public int positionID;

    public GameObject image;

    public int combatGoldReward;
    public int combatBondReward;
    public int combatRationReward;
    public int combatArtifactReward;
    
    [System.NonSerialized]
    public bool isVisited;

    [System.NonSerialized]
    public bool isTaken;

    [System.NonSerialized]
    public float moveDistance;

    [System.NonSerialized]
    public Vector3 savedPosition;

    [System.NonSerialized]
    public float positionAdjust;

    public List<MapNode> accessNodes;
    public event System.EventHandler HoverEnter;
    public event System.EventHandler HoverExit;
    public event System.EventHandler NodeClicked;
    public event System.EventHandler PlayerEntered;

    public Flowchart flowchart;

    public List<GameObject> enemyList;

    private float takenSize = 2.0f;
    private float notTakenSize = 1.5f;
    private float animationSpeed = 1f;

    private float arrowTop = 1.8f;
    private float arrowBottom = 1.4f;

    private float fadeBottom = 0.5f;
    public bool isFading = false;

    [System.NonSerialized]
    public SpriteGlowEffect highlightEffect;

    private int glowWidth = 3;

    private bool floatSwitch = true;
    private bool fadeSwitch = false;
    private GameObject arrow;

    // Start is called before the first frame update
    public void Initialize()
    {
        isVisited = false;
        isTaken = false;
        isCorrupted = false;
        accessNodes = new List<MapNode>();
        flowchart = GetComponent<Flowchart>();
        enemyList = new List<GameObject>();

        highlightEffect = image.GetComponent<SpriteGlowEffect>();
        highlightEffect.OutlineWidth = 0;
        highlightEffect.GlowBrightness = glowWidth;

        arrow = image.transform.GetChild(0).gameObject;

        for(int i = 0; i < transform.childCount; i++)
        {
            enemyList.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if(isVisited)
        // {
        //     Color COLORTIME = new Color (128, 128, 128, 1);
        //     image.GetComponent<SpriteRenderer>().color = COLORTIME;
        // }
        TakenAnimation();
        ArrowAnimation();
        //FadeAnimation();
        GlowAnimation();
    }

    public void SetAccessNodes(List<MapNode> nodeList)
    {
        accessNodes = new List<MapNode>();
        float distance;
        foreach(MapNode node in nodeList)
        {
            distance = Vector3.Distance(transform.position, node.transform.position);
            Debug.Log("Node Distance is: " + distance);
            if(distance <= moveDistance && distance != 0)
            {
                accessNodes.Add(node);
            }
        }
    }

    private void ArrowAnimation()
    {
        if(isTaken)
        {
            arrow.SetActive(true);
            if(floatSwitch) arrow.transform.localPosition += new Vector3(0, Time.deltaTime * animationSpeed, 0);
            else arrow.transform.localPosition -= new Vector3(0, Time.deltaTime * animationSpeed, 0);
            
            if(arrow.transform.localPosition.y > arrowTop)
            {
                floatSwitch = false;
            }
            if(arrow.transform.localPosition.y < arrowBottom)
            {
                floatSwitch = true;
            }
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    private void TakenAnimation()
    {
        if(isTaken && image.transform.localScale.x < takenSize)
        {
            image.transform.localScale += new Vector3(Time.deltaTime * animationSpeed, Time.deltaTime * animationSpeed, 0);
        }
        if(!isTaken && image.transform.localScale.x > notTakenSize)
        {
            image.transform.localScale -= new Vector3(Time.deltaTime * animationSpeed, Time.deltaTime * animationSpeed, 0);
        }
    }

    private void FadeAnimation()
    {
        if(isFading)
        {
            Color tempColor = new Color();
            if(fadeSwitch) tempColor = new Color(255, 255, 255, image.GetComponent<SpriteRenderer>().color.a + (Time.deltaTime * animationSpeed));
            else tempColor = new Color(255, 255, 255, image.GetComponent<SpriteRenderer>().color.a - (Time.deltaTime * animationSpeed));
            
            image.GetComponent<SpriteRenderer>().color = tempColor;

            if(image.GetComponent<SpriteRenderer>().color.a >= 1.0f)
            {
                fadeSwitch = false;
            }
            if(image.GetComponent<SpriteRenderer>().color.a <= fadeBottom)
            {
                fadeSwitch = true;
            }
        }
        else
        {
           
        }
    }

    private void GlowAnimation()
    {
        if(isFading)
        {
            highlightEffect.OutlineWidth = glowWidth;
            Color tempColor = new Color();
            if(fadeSwitch) highlightEffect.GlowBrightness += Time.deltaTime * animationSpeed;
            else highlightEffect.GlowBrightness -= Time.deltaTime * animationSpeed;
            
            //image.GetComponent<SpriteRenderer>().color = tempColor;

            if(highlightEffect.GlowBrightness >= 2.0f)
            {
                fadeSwitch = false;
            }
            if(highlightEffect.GlowBrightness <= 1.0f)
            {
                fadeSwitch = true;
            }
        }
        else
        {
           highlightEffect.OutlineWidth = 0;
           highlightEffect.GlowBrightness = 1.0f;
        }
    }

    private void OnMouseEnter() 
    {
        
        //Debug.Log("MouseEntered!");
        HoverEnter.Invoke(this, new EventArgs());
    }

    private void OnMouseExit() 
    {
        HoverExit.Invoke(this, new EventArgs());
    }

    private void OnMouseUpAsButton() 
    {
        if(GameObject.Find("SayDialog") != null)
        {
            if(GameObject.Find("SayDialog").activeSelf)
            {
                return;
            }
        }
        else
        {
            Debug.Log("The Node Recognizes this as a button click!");
            NodeClicked.Invoke(this, new EventArgs());
        }
        
    }

    public void OnHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.red;
        isFading = true;
    }

    public void OnDeHighlightNode()
    {
        GetComponent<Renderer>().material.color = Color.white;
        isFading = false;
        image.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
    }

    public void OnPlayerHighlight()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void PlayerInNode()
    {
        PlayerEntered.Invoke(this, new EventArgs());
    }

    public void GiveCombatRewards(OverworldPlayer player)
    {
        //Add rewards to player and update GUI
        player.SetGoldCount(combatGoldReward);

        if(player.isBondBoosted)
        {
            Debug.Log(Mathf.RoundToInt((float)combatBondReward * (1f + player.bondBoost)));
            player.SetBondCount(Mathf.RoundToInt((float)combatBondReward * (1f + player.bondBoost)));
        }
        else
        {
            player.SetBondCount(combatBondReward);
        }
        player.SetRationCount(combatRationReward);

        for(int i = 0; i < combatArtifactReward; i++)
        {
            AddRandomArtifact();
        }
    }

    //Evenly spaces out nodes. Will be randomized later for more interesting layouts
    public void SetPosition()
    {
        float adjustVal = moveDistance/positionAdjust;

        Vector3 randomAdjustment = new Vector3(UnityEngine.Random.Range(-adjustVal, adjustVal), 0, UnityEngine.Random.Range(-adjustVal, adjustVal));

        transform.position = new Vector3((tierPosition * 10) + (worldTier * 5), 0, (worldTier * 10));
        transform.position += randomAdjustment;
    }

    public void UpdateVariables()
    {
        Debug.Log("EVENT IS OVER!!!");
        GameObject.Find("OverworldCamera").GetComponent<OverworldCamera>().inEncounter = false;
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        //player.GetEncounterVariables();
    }

    public void SetGoldCount(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetGoldCount(change);
    }

    public void SetRationCount(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetRationCount(change);
    }

    public void SetBondCount(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetBondCount(change);
    }

    public void SetChar1Health(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar1Health(change);
    }

    public void SetChar2Health(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar2Health(change);
    }

    public void SetChar1MaxHealth(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar1MaxHealth(change);
    }

    public void SetChar2MaxHealth(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar2MaxHealth(change);
    }

    public void SetChar1MaxAP(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar1MaxAP(change);
    }

    public void SetChar2MaxAP(int change)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.SetChar2MaxAP(change);
    }

    public void AddRandomArtifact()
    {
        Debug.Log("Player Gains an Artifact!");
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        player.AddRandomArtifact();

    }

    public void RemoveArtifact(string name)
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        Debug.Log("MapNode: Removing " + name);
        player.RemoveArtifact(name);
    }

    public void IncreaseDarkness()
    {
        Debug.Log("Darkness Doubles movement!");
    }

    public void FightEnemyGroup(int groupNum)
    {
        Debug.Log("Player fights enemy group " + groupNum + "!");
        DontDestroyOnLoad(this.gameObject);
        savedPosition = transform.position;
        transform.SetParent(null);
        transform.position = new Vector3(0, 0, 0);
        Debug.Log("Fade is found: " + GameObject.Find("Fade").GetComponent<SceneFader>());
        SceneFader scenetransition = GameObject.Find("Fade").GetComponent<SceneFader>();
        Debug.Log("scenetransition is " + scenetransition);
        switch(biome)
        {
            case "Forest":
                Debug.Log("WE GOING TO DA FOREST");
                StartCoroutine(scenetransition.FadeAndLoadScene( SceneFader.FadeDirection.In, 2));
                break;
            
            case "Swamp":
                StartCoroutine(scenetransition.FadeAndLoadScene( SceneFader.FadeDirection.In, 5));
                break;

            case "Ruins":
                StartCoroutine(scenetransition.FadeAndLoadScene( SceneFader.FadeDirection.In, 4));
                break;

            default:
                StartCoroutine(scenetransition.FadeAndLoadScene( SceneFader.FadeDirection.In, 2));
                break;
        }
        
    }

    public void OpenShop()
    
    {
        OverworldGUI overworldUI = GameObject.Find("OverworldUI").GetComponent<OverworldGUI>();

        Debug.Log("Sending Open shop call to the UI");
        overworldUI.ShowShop();
    }
}

