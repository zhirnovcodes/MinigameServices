using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel : MonoBehaviour, IDisposable
{
    [SerializeField] private Grid Grid; // UnityEngine.Grid for layout positioning
    [SerializeField] private List<MemoryGameBlockModel> Blocks = new List<MemoryGameBlockModel>();

    private IMinigameGameObjectPool Pool;

    public void Set(IMinigameGameObjectPool pool)
    {
        Pool = pool;
    }

    public MemoryGameBlockModel GetBlock(int index)
    {
        return Blocks[index];
    }

    public int GetBlocksCount()
    {
        return Blocks.Count;
    }

    public void AddBlock(BlockData data, GameObject prefab)
    {
        var go = Instantiate(prefab, transform);
        var model = go.GetComponent<MemoryGameBlockModel>();
        model.SetIndex(data.Index);
        Blocks.Add(model);
    }

    public void Dispose()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            if (Blocks[i] != null)
            {
                Destroy(Blocks[i].gameObject);
            }
        }
        Blocks.Clear();
    }
}


