using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Awake()
    {

        Initialize();

        
    }

    public virtual void Initialize()
    {
        mapControlScript = GameObject.Find("MapControl").GetComponent<MapControl>();

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

    private void OnMapGenerated(object sender, EventArgs e)
    {
        Debug.Log("Oh Boy! The Map has been generated and now I'm gonna hook up the UI!");

        UpdateUIValues();
        
    }

    private void OnValuesChanged(object sender, EventArgs e)
    {
        UpdateUIValues();
    }

    private void UpdateUIValues()
    {
        char1HPText.text = "HP: " + mapControlScript.playerScript.characterList[0].HitPoints.ToString();
        char1BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        char2HPText.text = "HP: " + mapControlScript.playerScript.characterList[1].HitPoints.ToString();
        char2BondText.text = "Bond Lvl: " + mapControlScript.playerScript.bondLevel.ToString();

        goldText.text = "Gold: " + mapControlScript.playerScript.goldCount.ToString();
        rationText.text = "Rations: " + mapControlScript.playerScript.rationCount.ToString();
        bondPointText.text = "Bond Points: " + mapControlScript.playerScript.bondCount.ToString() + " / " +mapControlScript.playerScript.bondMax.ToString();
    }

    public void OnCampButtonClicked()
    {
        Debug.Log("Looks like Camping is back on the Menu! (Camp Button Clicked)");
        SceneManager.LoadScene(2);
    }
}
