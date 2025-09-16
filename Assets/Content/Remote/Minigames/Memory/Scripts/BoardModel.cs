using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardModel : MonoBehaviour, IDisposable
{
    [SerializeField] private Vector2Int Size;
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private Grid Grid;
    
    private List<MemoryGameBlockModel> Blocks = new List<MemoryGameBlockModel>();

    private IMinigameGameObjectPool Pool;
    private MemoryGameConfigModel Config;

    private int GetCount()
    {
        return Size.x * Size.y;
    }

    public void Construct(IMinigameGameObjectPool pool, MemoryGameConfigModel config)
    {
        Pool = pool;
        Config = config;
    }

    public void Build()
    {
        Debug.Assert(GetCount() % 2 == 0);

        int totalPairs = GetCount() / 2;
        int rewardCount = Mathf.RoundToInt(totalPairs * 0.8f); // 80% rewards
        int penaltyCount = totalPairs - rewardCount; // 20% penalties

        var results = new List<MemoryGameResultData>();

        for (int i = 0; i < rewardCount; i++)
        {
            results.Add( new MemoryGameResultData
            {
                IsSuccess = true,
                Reward = Config.GetRewardData(i)
            }); // Use index directly for rewards
        }
        
        for (int i = 0; i < penaltyCount; i++)
        {
            results.Add( new MemoryGameResultData
            {
                IsSuccess = false,
                Penalty = Config.GetPenalty(i)
            });
        }

        // Create blocks and spawn icons
        List<int> availablePositions = new List<int>();
        for (int i = 0; i < GetCount(); i++)
        {
            availablePositions.Add(i);
        }

        // Calculate board center offset to position board at (0, 0, 0)
        Vector3 boardCenterOffset = new Vector3(
            -(Size.x - 1) * 0.5f,
            -(Size.y - 1) * 0.5f,
            0f
        );

        // Create blocks for each pair
        for (int pairIndex = 0; pairIndex < totalPairs; pairIndex++)
        {
            // Create 2 blocks for each pair
            for (int blockInPair = 0; blockInPair < 2; blockInPair++)
            {
                int positionIndex = pairIndex * 2 + blockInPair;
                int gridPosition = availablePositions[positionIndex];
                
                // Create block from prefab
                var blockGO = Instantiate(BlockPrefab, transform, false);
                var blockModel = blockGO.GetComponent<MemoryGameBlockModel>();

                // Get icon type based on result index
                var data = results[pairIndex];
                var icon = data.IsSuccess ? data.Reward.Icon : data.Penalty.Icon;

                // Spawn icon from pool
                var iconGO = Pool.Instantiate(icon);
                var placeholder = blockModel.GetPlaceholder();
                iconGO.transform.SetParent(placeholder, false);
                
                // Set position to grid with center offset
                Vector3Int gridPosition3D = new Vector3Int(gridPosition % Size.x, gridPosition / Size.x, 0);
                Vector3 worldPosition = Grid.CellToWorld(gridPosition3D) + boardCenterOffset;
                blockGO.transform.position = worldPosition;
                blockModel.SetTurnedHidden();
                blockModel.SetResultData(data);
                blockModel.SetIndex(positionIndex);
                Blocks.Add(blockModel);
            }
        }

        // Shuffle blocks and results
        for (int i = 0; i < Blocks.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, Blocks.Count);
            
            // Swap block positions
            Vector3 tempPosition = Blocks[i].transform.position;
            Blocks[i].transform.position = Blocks[randomIndex].transform.position;
            Blocks[randomIndex].transform.position = tempPosition;
        }
    }

    public MemoryGameBlockModel GetBlock(int index)
    {
        return Blocks[index];
    }

    public int GetBlocksCount()
    {
        return Blocks.Count;
    }

    public void SetAllClickable()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Blocks[i].SetInteractive();
        }
    }

    public void SetAllButtonsUnclickable()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Blocks[i].SetNonInteractive();
        }
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


