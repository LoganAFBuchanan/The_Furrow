﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSquare : Square
{
    // Start is called before the first frame update
    public override Vector3 GetCellDimensions()
    {
        return GetComponent<Renderer>().bounds.size;
    }

    public override void MarkAsHighlighted()
    {
        GetComponent<Renderer>().material.color = new Color(0.75f, 0.75f, 0.75f);
    }

    public override void MarkAsPath()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public override void MarkAsReachable()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    public override void UnMark()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public override void MarkAsTargetable()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

}
