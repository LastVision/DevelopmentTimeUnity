using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePreview : MonoBehaviour
{
    public Tile previewTile;

    private int tileNeighbourLookupIndex = 0;

    private void Start()
    {
        PreviewNeighbours();
    }
    public void PreviewNeighbours()
    {
        CleanupPreview();

        int maxNeighbourCount = 0;
        maxNeighbourCount = Math.Max(maxNeighbourCount, previewTile.upNeighbour.Length);
        maxNeighbourCount = Math.Max(maxNeighbourCount, previewTile.downNeighbour.Length);
        maxNeighbourCount = Math.Max(maxNeighbourCount, previewTile.leftNeighbour.Length);
        maxNeighbourCount = Math.Max(maxNeighbourCount, previewTile.rightNeighbour.Length);

        Vector3 neighbourOffset = Vector3.zero;
        for(int i = 0; i < maxNeighbourCount; i++)
        {
            CreateNeightbourPair(i, neighbourOffset);

            neighbourOffset.x += 4;
        }
    }

    private void CreateNeightbourPair(int neighbourPairIndex, Vector3 offsetPosition)
    {
        Tile tile = GameObject.Instantiate(previewTile, transform);
        tile.transform.position = transform.position + offsetPosition;

        CreateNeighbourIfThereIs(previewTile.upNeighbour.ToList(), neighbourPairIndex, offsetPosition + new Vector3(0, 0, -1));
        CreateNeighbourIfThereIs(previewTile.downNeighbour.ToList(), neighbourPairIndex, offsetPosition + new Vector3(0, 0, 1));
        CreateNeighbourIfThereIs(previewTile.leftNeighbour.ToList(), neighbourPairIndex, offsetPosition + new Vector3(1, 0, 0));
        CreateNeighbourIfThereIs(previewTile.rightNeighbour.ToList(), neighbourPairIndex, offsetPosition + new Vector3(-1, 0, 0));
    }

    private void CreateNeighbourIfThereIs(List<Tile> neighbourList, int neighbourPairIndex, Vector3 offsetPosition)
    {
        Debug.Log(neighbourList.Count + " " + neighbourPairIndex);
        if(neighbourList.Count > 0 && neighbourPairIndex < neighbourList.Count)
        {
            Debug.Log("Create tile");
            Tile obj = GameObject.Instantiate(neighbourList[neighbourPairIndex], transform);
            obj.transform.position = obj.transform.position + offsetPosition;
        }
    }

    private void CleanupPreview()
    {
        foreach(GameObject obj in this.gameObject.transform)
        {
            Destroy(obj);
        }

        tileNeighbourLookupIndex = 0;
    }
}
