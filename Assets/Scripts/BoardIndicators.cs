using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardIndicators : MonoBehaviour
{
    public static BoardIndicators Instance { set; get; }
    public GameObject indicatorPrefab;
    private List<GameObject> indicators;
    private void Start()
    {
        Instance = this;
        indicators = new List<GameObject>();
    }

    private GameObject GetIndicatorObject()
    {
        GameObject go = indicators.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(indicatorPrefab);
            indicators.Add(go);
        }

        return go;
    }

    public void IndicatorAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    GameObject go = GetIndicatorObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                    
                }
            }
        }
    }

    public void HideIndicators()
    {
        foreach (GameObject go in indicators)
        {
            go.SetActive(false);
        }
    }
}
