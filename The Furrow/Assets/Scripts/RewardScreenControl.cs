using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreenControl : MonoBehaviour
{

    public Text rationReward;
    public Text goldReward;
    public Text bondReward;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRewardScreen(MapNode node)
    {
        this.gameObject.SetActive(true);
        rationReward.text = node.combatRationReward.ToString();
        goldReward.text = node.combatGoldReward.ToString();
        bondReward.text = node.combatBondReward.ToString();
    }

    public void CloseRewards()
    {
        GameObject.Find("GUIController").GetComponent<GUIController>().ReallyEndTheGame();
        this.gameObject.SetActive(false);   

    }




}
