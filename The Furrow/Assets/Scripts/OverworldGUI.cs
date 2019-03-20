using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OverworldGUI : MonoBehaviour
{
    public MapControl mapControlScript;

    public Button campButton;

    public GameObject char1Stats;
    public GameObject char2Stats;

    public Text goldText;
    public Text rationText;
    public Text bondPointText;

    private Image char1Image;
    private Text char1HPText;
    private Text char1BondText;

    private Image char2Image;
    private Text char2HPText;
    private Text char2BondText;

    public ArtifactContainer artifactContainer;

    public GameObject shopPanel;

    // Start is called before the first frame update
    void Awake()
    {

        if(GameObject.FindGameObjectsWithTag("OverworldUI").Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Initialize();
        }

        
        
    }

    public virtual void Initialize()
    {
        mapControlScript = GameObject.Find("MapControl").GetComponent<MapControl>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        mapControlScript.MapGenerated += OnMapGenerated;
        mapControlScript.ValuesChanged += OnValuesChanged;
        
        char1Image = char1Stats.transform.GetChild(0).GetComponent<Image>();
        char1HPText = char1Stats.transform.GetChild(1).GetComponent<Text>();
        char1BondText = char1Stats.transform.GetChild(2).GetComponent<Text>();

        char2Image = char2Stats.transform.GetChild(0).GetComponent<Image>();
        char2HPText = char2Stats.transform.GetChild(1).GetComponent<Text>();
        char2BondText = char2Stats.transform.GetChild(2).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddArtifact(Artifact arti)
    {
        artifactContainer.LoadArtifactToGUI(arti);
    }

    public void RemoveArtifact(Artifact arti)
    {
        //Debug.Log("Overworld UI: Removing " + name);
        artifactContainer.RemoveArtifactFromGUI(arti);
    }

    private void OnMapGenerated(object sender, EventArgs e)
    {
        Debug.Log("Oh Boy! The Map has been generated and now I'm gonna hook up the UI!");

        UpdateUIValues();
        
    }

    private void OnValuesChanged(object sender, EventArgs e)
    {
        UpdateUIValues();
    }

    public virtual void UpdateUIValues()
    {
        char1HPText.text = "HP: " + mapControlScript.playerScript.characterList[0].HitPoints.ToString();
        char1BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        char2HPText.text = "HP: " + mapControlScript.playerScript.characterList[1].HitPoints.ToString();
        char2BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        goldText.text = "Gold: " + mapControlScript.playerScript.goldCount.ToString();
        rationText.text = "Rations: " + mapControlScript.playerScript.rationCount.ToString();
        bondPointText.text = "Bond Points: " + mapControlScript.playerScript.bondCount.ToString() + " / " +mapControlScript.playerScript.bondMax.ToString();

        if(mapControlScript.isFirstMove) campButton.interactable = false;
        else campButton.interactable = true;
    }

    public void OnDebugArtifactButtonClicked()
    {
        GameObject.Find("Player").GetComponent<OverworldPlayer>().AddRandomArtifact();
    }

    public void OnCampButtonClicked()
    {
        Debug.Log("Looks like Camping is back on the Menu! (Camp Button Clicked)");

        //Move Map Away
        mapControlScript.savedPosition = mapControlScript.transform.position;
        mapControlScript.transform.position = new Vector3(-100000, -100000, -100000);
        SceneManager.LoadScene(2);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        Debug.Log("ONSCENE LOADED IS CALLED IN THE OVERWORLD GUI");

        if(scene.name != "Overworld")
        {
            campButton.gameObject.SetActive(false);
        }
        else
        {
            campButton.gameObject.SetActive(true);
        }
        //if(scene.buildIndex != initialSceneIndex) CleanUpDelegates();

        
    }

    //Show shop and update available artifacts
    public void ShowShop()
    {
        Debug.Log("ShowShop Begun");
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        Debug.Log("Activating shop panel...");
        shopPanel.SetActive(true);
        campButton.interactable = false;
        mapControlScript.nodesEnabled = false;
        Debug.Log("Shop Panel Activated");
        Debug.Log("Starting loop through artifact slot transforms");
        foreach(Transform artifactSlot in shopPanel.transform.GetChild(0))
        {
            Debug.Log(artifactSlot.gameObject.name);
            artifactSlot.gameObject.SetActive(true);
            Image artifactImage = artifactSlot.Find("Image").GetComponent<Image>();;
            Text artifactName = artifactSlot.Find("Name").GetComponent<Text>();
            Text artifactDesc = artifactSlot.Find("Description").GetComponent<Text>();
            Text artifactCost = artifactSlot.Find("Cost").GetComponent<Text>();

            Artifact shopArtifact = player.GetRandomArtifact();
            Debug.Log("Generated " + shopArtifact.name);

            artifactImage.sprite = shopArtifact.image.sprite;
            Debug.Log("Grabbed sprite");
            artifactName.text = shopArtifact.title;
            artifactDesc.text = shopArtifact.desc;
            artifactCost.text = "Cost: " + shopArtifact.cost.ToString();

            EventTrigger trigger = artifactSlot.gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => 
            { 
                if(player.goldCount >= shopArtifact.cost)
                {
                    player.AddArtifact((PointerEventData)data, shopArtifact.name); 
                    player.SetGoldCount(-shopArtifact.cost);
                    artifactSlot.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Not enough gold!!");
                }
                
            });
            trigger.triggers.Add(entry);

        }

    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        campButton.interactable = true;
        mapControlScript.nodesEnabled = true;
    }

    public void BuyRation()
    {
        OverworldPlayer player = GameObject.Find("Player").GetComponent<OverworldPlayer>();
        if(player.goldCount >= Constants.SHOP_RATION_COST)
        {
            player.SetGoldCount(-Constants.SHOP_RATION_COST);
            player.SetRationCount(1);
        }
        else
        {
            Debug.Log("Not Enough Gold!");
        }
    }

    public void CleanUpDelegates()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        mapControlScript.MapGenerated -= OnMapGenerated;
        mapControlScript.ValuesChanged -= OnValuesChanged;
    }
}
