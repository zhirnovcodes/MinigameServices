using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTest : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Player Data")]
    public string PlayerName = "Ivan";
    public int PlayerCash = 10;
    public int PlayerDiamonds = 1;
    public List<CharacterCardsData> PlayerCards;
    
    private IMinigameModel Model;

    private void Start()
    {
        Model = GetComponent<IMinigameModel>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.enabled = false;
            StartGame().Forget();
        }
    }

    private async UniTaskVoid StartGame()
    {
        Debug.Log("Initing game");

        var input = new MinigameInputData
        {
            PlayerData = new List<MinigamePlayerData>
            {
                new MinigamePlayerData
                {
                    Name = PlayerName,
                    Cash = PlayerCash,
                    Diamonds = PlayerDiamonds,
                    CharacterCards = PlayerCards
                }
            }
        };

        var resultData = new MinigameResultData
        {
            Reward = new MinigameRewardData
            {
                CharacterCards = new List<CharacterCardsData>()
            },
            Penalties = new MinigamePenaltiesData
            {
                Penalties = new List<MinigamePenaltyData>()
            }
        };

        var services = new MinigameServices();

        Model.GameplayFinished += HandleGameplayFinished;

        Model.Init(input, resultData, services);
        
        Debug.Log("Starting game");

        await Model.StartGame();

        Debug.Log("Game process is over");
    }

    private void HandleGameplayFinished(MinigameResultData obj)
    {
        Debug.Log($"Game is finished with {obj.Status}. Rewards: {RewardToString(obj.Reward)}, Penalties: {PenaltiesToString(obj.Penalties)}");
    }

    private static string RewardToString(MinigameRewardData reward)
    {
        var parts = new List<string>();
        
        if (reward.Cash > 0)
            parts.Add($"Cash: {reward.Cash}");
            
        if (reward.Diamonds > 0)
            parts.Add($"Diamonds: {reward.Diamonds}");
            
        if (reward.CharacterCards != null && reward.CharacterCards.Count > 0)
        {
            var cardParts = new List<string>();
            foreach (var card in reward.CharacterCards)
            {
                cardParts.Add($"{card.ID} x{card.Count}");
            }
            parts.Add($"Cards: {string.Join(", ", cardParts)}");
        }
        
        return parts.Count > 0 ? string.Join(", ", parts) : "None";
    }

    private static string PenaltiesToString(MinigamePenaltiesData penalties)
    {
        if (penalties?.Penalties == null || penalties.Penalties.Count == 0)
            return "None";
            
        var parts = new List<string>();
        foreach (var penalty in penalties.Penalties)
        {
            var operation = penalty.Operation == PenaltyOperations.Remove ? "-" : "/";
            parts.Add($"{penalty.Penalty} {operation}{penalty.Amount}");
        }
        
        return string.Join(", ", parts);
    }
#endif
}
