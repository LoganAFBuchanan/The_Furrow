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

    public PatchPack()
    {
        name = "PatchPack";
        title = "Patch Pack";
        desc = "Heal 50% when Resting";
        healBoost = 0.5f;
        cost = 15;
        UnityEngine.GameObject artifactUI;
        Debug.Log("Loading Artifact asset for " + name);
        artifactUI = UnityEngine.GameObject.Instantiate(UnityEngine.Resources.Load<UnityEngine.GameObject>("Artifacts/PatchPack"));
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