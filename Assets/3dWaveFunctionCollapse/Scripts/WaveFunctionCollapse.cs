using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour
{
    [SerializeField] private Vector2 worldSize = new Vector2(2, 2);
    [SerializeField] private Tile[] tileOptions;
    [SerializeField] private Cell cellObject;
    [SerializeField] private List<Cell> grid;

    [SerializeField] private Tile backupTile;

    private int iteration = 0;

    private void Awake()
    {
        grid = new List<Cell>();
        InitalizeGrid();
    }

    private void InitalizeGrid()
    {
        for(int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                Cell newCell = Instantiate(cellObject, new Vector3(x, 0, y), Quaternion.identity, transform);
                newCell.CreateCell(false, tileOptions);
                grid.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }

    IEnumerator CheckEntropy()
    {
        //First we remove any collapsed tiles then we sort the list to have lowest entropy first and remove anything that is greater than the lowest entropy
        List<Cell> tempGrid = new List<Cell>(grid);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a,b) => a.tileOptions.Length - b.tileOptions.Length);
        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);

        yield return new WaitForSeconds(0.125f);

        //Then with the grid of cells that remains we collapse a cell
        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        //We pick one of the lowest entropy choices to collapse at random
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        Cell cellToCollapse = tempGrid[randIndex];
        cellToCollapse.collapsed = true;

        //then based on what tile options are available we pick one at random here
        Tile selectedTile = backupTile;
        if (cellToCollapse.tileOptions.Length > 0)
        {
            int randomTileIndex = UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length);
            selectedTile = cellToCollapse.tileOptions[randomTileIndex];
        }
        cellToCollapse.tileOptions = new Tile[] { selectedTile };

        //Then we create the tile
        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, cellToCollapse.transform.rotation, cellToCollapse.transform);

        //And finally update the next generation based on the result of the collapsing cell
        UpdateGeneration();
    }

    private void UpdateGeneration()
    {
        List<Cell> newGenerationCells = new List<Cell>(grid);

        int width = (int)worldSize.x;
        int height = (int)worldSize.y;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                int index = x + y * width;

                if (grid[index].collapsed)
                {
                    //If already collapsed we just set copy that over as is to the new generation cell grid
                    newGenerationCells[index] = grid[index];
                }
                else
                {
                    //Prepare options with all possible options, we will later check against neighbours to update the options for the current grid cell
                    List<Tile> options = new List<Tile>();
                    foreach(Tile t in tileOptions)
                    {
                        options.Add(t);
                    }

                    //First we check for any possible neighbours in the up direction
                    if(y > 0)
                    {
                        Cell up = grid[x + (y - 1) * width];
                        List<Tile> validOptions = new List<Tile>();

                        //We get all possible options for the tile above for any neighbours in the opposite direction from where we are looking at
                        foreach (Tile possibleTiles in up.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileOptions, obj => obj == possibleTiles);
                            var valid = tileOptions[validOption].downNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        //Here we check the validity of the valid options we have populated and if they aren't valid then we will remove them from the options list
                        CheckValidity(options, validOptions);
                    }

                    if (x < width - 1)
                    {
                        Cell left = grid[x + 1 + y * width];
                        List<Tile> validOptions = new List<Tile>();

                        //We get all possible options for the tile above for any neighbours in the opposite direction from where we are looking at
                        foreach (Tile possibleTiles in left.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileOptions, obj => obj == possibleTiles);
                            var valid = tileOptions[validOption].rightNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        //Here we check the validity of the valid options we have populated and if they aren't valid then we will remove them from the options list
                        CheckValidity(options, validOptions);
                    }

                    if (y < height - 1)
                    {
                        Cell down = grid[x + (y + 1) * width];
                        List<Tile> validOptions = new List<Tile>();

                        //We get all possible options for the tile above for any neighbours in the opposite direction from where we are looking at
                        foreach (Tile possibleTiles in down.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileOptions, obj => obj == possibleTiles);
                            var valid = tileOptions[validOption].upNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        //Here we check the validity of the valid options we have populated and if they aren't valid then we will remove them from the options list
                        CheckValidity(options, validOptions);
                    }

                    if (x > 0)
                    {
                        Cell right = grid[x - 1 + y * width];
                        List<Tile> validOptions = new List<Tile>();

                        //We get all possible options for the tile above for any neighbours in the opposite direction from where we are looking at
                        foreach (Tile possibleTiles in right.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileOptions, obj => obj == possibleTiles);
                            var valid = tileOptions[validOption].leftNeighbour;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        //Here we check the validity of the valid options we have populated and if they aren't valid then we will remove them from the options list
                        CheckValidity(options, validOptions);
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for(int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCells[index].RecreateCell(newTileList);
                }
            }
        }

        grid = newGenerationCells;
        ++iteration;

        if (iteration < worldSize.x * worldSize.y)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    private void CheckValidity(List<Tile> optionList, List<Tile> validOptions)
    {
        //Straight forward we loop the list backwards(becasue we removing elements) we check if the current option in the element is valid compared to the validOptions if it's not valid we remove the element.
        for (int i = optionList.Count - 1; i >= 0; i--)
        {
            var element = optionList[i];
            if(!validOptions.Contains(element))
            {
                optionList.RemoveAt(i);
            }
        }
    }
}
