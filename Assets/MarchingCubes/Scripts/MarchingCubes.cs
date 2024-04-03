using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private float m_Width = 10.0f;
    [SerializeField] private float m_Height = 10.0f;

    [SerializeField] private float m_NoiseResolution = 1.0f;

    [SerializeField] private bool m_VisualizeNoise = false;

    private float[,,] heights;

    private void Start()
    {
        StartCoroutine(UpdateAll());
    }

    private IEnumerator UpdateAll()
    {
        while (true)
        {
            SetHeights();
            yield return new WaitForSeconds(1);
        }
    }

    private void SetHeights()
    {
        heights = new float[(int)(m_Width + 1), (int)(m_Height + 1), (int)(m_Width + 1)];

        for(int x = 0; x < m_Width + 1; x++)
        {
            for(int y = 0 ; y < m_Height + 1; y++)
            {
                for(int z = 0; z < m_Width + 1; z++)
                {
                    float currentHeight = m_Height * Mathf.PerlinNoise(x * m_NoiseResolution, z * m_NoiseResolution);
                    float newHeight;

                    if(y > currentHeight)
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }


                    heights[x, y, z] = newHeight;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(!m_VisualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for(int x = 0; x < m_Width + 1; x++)
        {
            for(int y = 0; y < m_Height + 1; y++)
            {
                for(int z = 0; z < m_Width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z]);
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f);
                }
            }
        }
    }
}
