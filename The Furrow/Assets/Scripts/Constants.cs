using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const int STARVING_COST = 2;
    
    public const int STARTING_GOLD = 25;
    public const int STARTING_RATIONS = 5;
    public const int STARTING_BOND_LEVEL = 1;

    public const int STARTING_AP_MAX = 5;

    public const int BOND_MAX_LEVEL_1 = 5;
    public const int BOND_MAX_LEVEL_2 = 10;
    public const int BOND_MAX_LEVEL_3 = 15;
    public const int BOND_MAX_LEVEL_4 = 20;
    public const int BOND_MAX_LEVEL_5 = 25;

    public const int LEVEL_UP_HP_INC = 2;

    public const int CAMP_HUNT_RATIONS = 3;
    public const float CAMP_REST_PCT = 0.33f;
    public const int CAMP_BOND_INC = 5;

    public const int SHOP_RATION_COST = 3;

    public const float PUSH_SPEED = 10f;

    public const float GRID_TRANSPARENCY = 0.5f;

    public static readonly Vector3 ALDRIC_START_POS = new Vector3(3f, 0f, 1.5f);
    public static readonly Vector3 IDE_START_POS = new Vector3(0f, 0f, 1.5f);


    public const int SLIME_LIFETIME = 2;

    public static readonly string[] allArtifacts = new string[] 
    {
        "FurtiveMushroom",
        "StarShard"
    };
    
    
}