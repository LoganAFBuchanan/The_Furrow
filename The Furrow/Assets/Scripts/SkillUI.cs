using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{

    public GameObject skillPanel;

    public Image skillImage;
    public Image gridImage;

    public Text heroName;
    public Text skillName;
    public Text apCost;
    public Text skillDesc;

    public void FillSkillUI(Skill skill)
    {
        if(skill != null)
        {
            if(GetComponent<Button>() == null)GetComponent<Image>().sprite = skill.skillImage;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
            skillImage.sprite = skill.skillImage;
            gridImage.sprite = skill.gridImage;

            heroName.text = skill.character;
            skillName.text = skill.skillname;
            apCost.text = skill.actioncost.ToString() +" AP";
            skillDesc.text = skill.desc;
        }
        else
        {
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void OnMouseEnter()
    {
        if(heroName.text != "Hero")
        {
            skillPanel.SetActive(true);
        }
        
    }
    public void OnMouseExit()
    {
        skillPanel.SetActive(false);
    }

}
