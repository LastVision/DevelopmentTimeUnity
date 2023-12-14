using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Tile[] tileOptions;

    public void CreateCell(bool collapsedState, Tile[] tileOptions)
    {
        this.collapsed = collapsedState;
        this.tileOptions = tileOptions;
    }

    public void RecreateCell(Tile[] tileOptions)
    {
        this.tileOptions = tileOptions;
    }
}
