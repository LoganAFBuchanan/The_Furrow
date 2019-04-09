using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PatchPack : Artifact
{

    public float healBoost;
    public int cost { get; set; }
    public string name { get; set; }
    public string title { get; set; }
    public string desc { get; set; }
    public Image image { get; set; }
    public string lore { get; set; }

    public PatchPack()
    {
        name = "HealersHarp";
        title = "Healer’s Harp";
        desc = "Heal 50% when Resting";
        lore ="A gleaming harp, which tinkles with soothing music when asked nicely.";
        healBoost = 0.5f;
        cost = 15;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/HealersHarp"));
        image = artifactUI.GetComponent<Image>();
        UnityEngine.GameObject.Destroy(artifactUI);
    }

    public void Apply(OverworldPlayer player)
    {
        player.isHealBoosted = true;
        player.healBoost += healBoost;
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        player.isHealBoosted = false;
        player.healBoost -= healBoost;
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new PatchPack();
    }
}