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
    private RectTransform char1BarMask;
    public GameObject char1SkillContainer;


    private Image char2Image;
    private Text char2HPText;
    private Text char2BondText;
    private RectTransform char2BarMask;
    public GameObject char2SkillContainer;

    private Image bondCircle;
    public Image dropDownBondCircle;

    public bool isCampGUI;
    
    private float barMaxWidth;

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
            isCampGUI = false;
            Initialize();
        }

        
        
    }

    public virtual void Initialize()
    {
        mapControlScript = GameObject.Find("MapControl").GetComponent<MapControl>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        if(!isCampGUI)
        {
            mapControlScript.MapGenerated += OnMapGenerated;
            mapControlScript.ValuesChanged += OnValuesChanged;
        
            char1Image = char1Stats.transform.GetChild(0).GetComponent<Image>();
            char1HPText = char1Stats.transform.GetChild(1).GetComponent<Text>();
            char1BondText = char1Stats.transform.GetChild(2).GetComponent<Text>();
            char1BarMask = char1Stats.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>();

            char2Image = char2Stats.transform.GetChild(0).GetComponent<Image>();
            char2HPText = char2Stats.transform.GetChild(1).GetComponent<Text>();
            char2BondText = char2Stats.transform.GetChild(2).GetComponent<Text>();
            char2BarMask = char2Stats.transform.GetChild(3).GetChild(0).GetComponent<RectTransform>();

            bondCircle = GameObject.Find("BondBackground").transform.GetChild(0).GetComponent<Image>();
            //dropDownBondCircle = GameObject.Find("DropDownBondBackground").transform.GetChild(0).GetComponent<Image>();

            barMaxWidth = char1BarMask.sizeDelta.x;
        
        
            Debug.Log("The Health bar mask max width is: " + barMaxWidth);

            UpdateHealthBars();
            UpdateSkills();
        }
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
        if(char1HPText != null)char1HPText.text = "HP: " + mapControlScript.playerScript.characterList[0].HitPoints.ToString() + " / " + mapControlScript.playerScript.characterList[0].TotalHitPoints.ToString();
        if(char1BondText != null)char1BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        if(char2HPText != null)char2HPText.text = "HP: " + mapControlScript.playerScript.characterList[1].HitPoints.ToString() + " / " + mapControlScript.playerScript.characterList[1].TotalHitPoints.ToString();
        if(char2BondText != null)char2BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        if(goldText != null)goldText.text = " " + mapControlScript.playerScript.goldCount.ToString();
        if(rationText != null)rationText.text = " " + mapControlScript.playerScript.rationCount.ToString();
        if(bondPointText != null)bondPointText.text = mapControlScript.playerScript.bondCount.ToString() + " / " + mapControlScript.playerScript.bondMax.ToString();

        if(mapControlScript.isFirstMove) campButton.interactable = false;
        else campButton.interactable = true;

        UpdateHealthBars();
        UpdateBondBar();
        UpdateSkills();
    }

    public void UpdateHealthBars()
    {   
        //Map a width value based on the characters current and total health values and then update the bar width values accordingly

        if(char1BarMask != null)char1BarMask.sizeDelta = new Vector2(Map(mapControlScript.playerScript.characterList[0].HitPoints, 0, mapControlScript.playerScript.characterList[0].TotalHitPoints, 0, barMaxWidth), 44);
        if(char2BarMask != null)char2BarMask.sizeDelta = new Vector2(Map(mapControlScript.playerScript.characterList[1].HitPoints, 0, mapControlScript.playerScript.characterList[1].TotalHitPoints, 0, barMaxWidth), 44);

    }

    public void UpdateBondBar()
    {
        if(bondCircle != null)bondCircle.fillAmount = Map(mapControlScript.playerScript.bondCount, 0, mapControlScript.playerScript.bondMax, 0, 1);
        if(dropDownBondCircle != null)dropDownBondCircle.fillAmount = Map(mapControlScript.playerScript.bondCount, 0, mapControlScript.playerScript.bondMax, 0, 1);
    }

    public void UpdateSkills()
    {
        int index = 1;
        foreach(Transform child in char1SkillContainer.transform)
        {
            if(index == 1) child.GetComponent<SkillUI>().FillSkillUI(mapControlScript.playerScript.characterList[0].skill2);
            if(index == 2) child.GetComponent<SkillUI>().FillSkillUI(mapControlScript.playerScript.characterList[0].skill3);
            if(index == 3) child.GetComponent<SkillUI>().FillSkillUI(null);

            index++;
        }

        index = 1;
        foreach(Transform child in char2SkillContainer.transform)
        {
            if(index == 1) child.GetComponent<SkillUI>().FillSkillUI(mapControlScript.playerScript.characterList[1].skill2);
            if(index == 2) child.GetComponent<SkillUI>().FillSkillUI(mapControlScript.playerScript.characterList[1].skill3);
            if(index == 3) child.GetComponent<SkillUI>().FillSkillUI(null);

            index++;
        }
        
    }

    public void OnDebugArtifactButtonClicked()
    {
        GameObject.Find("Player").GetComponent<OverworldPlayer>().AddRandomArtifact();
    }

    public void OnCampButtonClicked()
    {
        Debug.Log("Looks like Camping is back on the Menu! (Camp Button Clicked)");
        
        mapControlScript.darkness.MoveDarkness(Constants.CAMP_DARKNESS_MOVE);
        
        StartCoroutine(ChangeToCamp());
    }

    public IEnumerator ChangeToCamp()
    {
        while(mapControlScript.darkness.isMoving)
        {
            yield return 0;
        }
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
        foreach(Transform artifactSlot in shopPanel.transform.GetChild(0))
        {
            artifactSlot.gameObject.GetComponent<EventTrigger>().triggers.Clear();
           
        }
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

    //Adapted from several answers here: https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public static float Map(float from, float fromMin, float fromMax, float toMin,  float toMax) 
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }
}
