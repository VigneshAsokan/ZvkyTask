using UnityEngine;

public class Slot : MonoBehaviour
{
    private SlotSymbol _currentSymbol;
    private SymbolType _type;
    public void Configure(SymbolData data, bool isStatic = false)
    {
        if(_currentSymbol != null)
        {
            Reset();
        }
        _currentSymbol = Instantiate(data.Symbol, transform);
        if (!isStatic)
        {
            _currentSymbol.PlayLandAnim();
            _type = data.Type;
        }
    }
    public void PlayWin()
    {
        _currentSymbol.PlayWinAnim();
    }
    public int GetReelValue()
    {
        return (int)_type;
    }
    public void Reset()
    {
        if (_currentSymbol != null)
        {
            Destroy(_currentSymbol.gameObject);
        }
    }
}
