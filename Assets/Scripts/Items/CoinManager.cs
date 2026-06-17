using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private TMP_Text coinText;

    private int _coins;
    public int Coins => _coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Refresh();
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        _coins += amount;
        Refresh();
    }

    private void Refresh()
    {
        if (coinText != null)
            coinText.text = _coins.ToString();
    }
}
