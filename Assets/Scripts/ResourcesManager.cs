using System;

public class ResourcesManager : SingletonBase<ResourcesManager>
{
    public event Action<int, int> CoinAmountChanged;

    private int m_Coins;
    public int Coins
    {
        get => m_Coins;
        set
        {
            CoinAmountChanged?.Invoke(m_Coins, value);
            m_Coins = value;
        }
    }
}