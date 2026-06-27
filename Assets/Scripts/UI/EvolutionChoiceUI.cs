using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows the level-up evolution screen. Listens to <see cref="EvolutionManager.OnChoicesAvailable"/>,
/// enables the level-up group and spawns one <see cref="EvolutionCard"/> per available node inside
/// the cards holder. Picking a card commits the choice and hides the screen.
/// </summary>
public class EvolutionChoiceUI : MonoBehaviour
{
    [SerializeField] private EvolutionManager evolution;

    [Tooltip("Container enabled while choosing (the level-up overlay).")]
    [SerializeField] private GameObject levelUpGroup;

    [Tooltip("Parent the cards are instantiated under (has the layout group).")]
    [SerializeField] private Transform cardsHolder;

    [SerializeField] private EvolutionCard cardPrefab;

    [Tooltip("Freeze gameplay while the player is choosing (per GDD).")]
    [SerializeField] private bool pauseWhileChoosing = true;

    private void OnEnable()
    {
        if (evolution != null)
            evolution.OnChoicesAvailable += Show;
    }

    private void OnDisable()
    {
        if (evolution != null)
            evolution.OnChoicesAvailable -= Show;
    }

    private void Start()
    {
        if (levelUpGroup != null)
            levelUpGroup.SetActive(false);
    }

    private void Show(IReadOnlyList<EvolutionNode> choices)
    {
        ClearCards();

        foreach (EvolutionNode node in choices)
        {
            EvolutionCard card = Instantiate(cardPrefab, cardsHolder);
            card.Setup(node, HandleChosen);
        }

        if (levelUpGroup != null)
            levelUpGroup.SetActive(true);

        if (pauseWhileChoosing)
            Time.timeScale = 0f;
    }

    private void HandleChosen(EvolutionNode node)
    {
        evolution.Choose(node);

        if (pauseWhileChoosing)
            Time.timeScale = 1f;

        if (levelUpGroup != null)
            levelUpGroup.SetActive(false);

        ClearCards();
    }

    private void ClearCards()
    {
        if (cardsHolder == null)
            return;

        for (int i = cardsHolder.childCount - 1; i >= 0; i--)
            Destroy(cardsHolder.GetChild(i).gameObject);
    }
}
