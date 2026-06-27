using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One evolution card in the level-up screen. Purely presentational: it shows a node's data
/// and reports back which node was picked. Knows nothing about the tree or the manager.
/// </summary>
public class EvolutionCard : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private EvolutionNode _node;
    private Action<EvolutionNode> _onSelected;

    /// <summary>Binds the card to a node and the callback fired when the player clicks it.</summary>
    public void Setup(EvolutionNode node, Action<EvolutionNode> onSelected)
    {
        _node = node;
        _onSelected = onSelected;

        if (categoryText != null) categoryText.text = node.Category;
        if (nameText != null) nameText.text = node.DisplayName;
        if (descriptionText != null) descriptionText.text = node.Description;
        if (iconImage != null) iconImage.sprite = node.Icon;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick() => _onSelected?.Invoke(_node);
}
