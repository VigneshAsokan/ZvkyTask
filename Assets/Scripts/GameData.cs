using Spine.Unity;
using System;
using UnityEngine;
using static GameData;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SymbolsData", order = 1)]
public class GameData : ScriptableObject
{
    [SerializeField]
    private SymbolData[] _symbolData;

    [Serializable]
    private struct keyAudioPair
    {
        public string Name;
        public AudioClip Clip;
    }
    [SerializeField]
    private keyAudioPair[] _audioPairs;
    public SymbolData GetRandom()
    {
        return _symbolData[UnityEngine.Random.Range(0, _symbolData.Length)];
    }
    public bool TryGetAudioClip(string audioName, out AudioClip audioClip)
    {
        audioClip = null;   
        foreach (var audioPair in _audioPairs)
        {
            if(audioPair.Name == audioName)
            {
                audioClip = audioPair.Clip;
                return true; 
            }
        }
        return false;
    }
}
[Serializable]
public class SymbolData
{
    public SymbolType Type;
    public SlotSymbol Symbol;
}
public enum SymbolType { None =-1, L5, L4, L3, L2, L1, H4, H3, H2, H1, Wild, Bonus, Scatter }