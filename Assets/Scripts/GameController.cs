using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private int boardHeight = 3;

    private int boardWidth = 5;
    [SerializeField]
    private GameData _gameData;

    public GameData GameData => _gameData;
    [SerializeField]
    private GameObject _splashScreen;
    [SerializeField]
    private InfoPageController _infoPage;
    [SerializeField]
    private GameObject _spinScreen;
    [SerializeField]
    private GameObject _winScreen;
    [SerializeField]
    private GameObject[] _spinReelColoumns;
    [SerializeField]
    private Slot slot;

    private Slot[,] _reels;
    [SerializeField]
    private GameObject _board;
    private List<GameObject> _matchLines;
    private Vector3 _offset = new Vector3(0, 0.25f, 0);

    [SerializeField]
    private float _currentBalance = 100f;
    [SerializeField]
    private float _minBet;
    [SerializeField]
    private float _maxBet;
    [SerializeField]
    private float _currentBet;
    [SerializeField]
    private TextMeshProUGUI _betText;
    [SerializeField]
    private TextMeshProUGUI _balanceText;
    [SerializeField]
    private TextMeshProUGUI _totalWinText;
    [SerializeField]
    private TextMeshProUGUI _spinWinText;
    [SerializeField]
    private SkeletonGraphic _paylineAnim;

    private float _score = 0;

    private bool _isSpinning = false;

    private const string SPIN_AUDIO_NAME = "Spin";
    private const string SPIN_STOP_AUDIO_NAME = "SpinStop";
    private const string BUTTON_AUDIO = "Button";


    private int[][] PAY_LINES = new int[][]
    {
        new int[] {5, 6, 7, 8, 9},          // Payline 1
        new int[] {0, 1, 2, 3, 4},          // Payline 2
        new int[] {10, 11, 12, 13, 14},     // Payline 3
        new int[] {0, 6, 12, 8, 4},         // Payline 4
        new int[] {10, 6, 2, 8, 14},        // Payline 5
        new int[] {5, 1, 2, 3, 9},          // Payline 6
        new int[] {5, 11, 12, 13, 9},       // Payline 7
        new int[] {0, 1, 7, 13, 14},        // Payline 8
        new int[] {10, 11, 7, 3, 4},        // Payline 9
        new int[] {5, 11, 7, 3, 9},         // Payline 10
        new int[] {5, 1, 7, 13, 9},         // Payline 11
        new int[] {0, 6, 7, 8, 4},          // Payline 12
        new int[] {10, 6, 7, 8, 14},        // Payline 13
        new int[] {0, 6, 2, 8, 4},          // Payline 14
        new int[] {10, 6, 12, 8, 14},       // Payline 15
        new int[] {5, 6, 2, 8, 9},          // Payline 16
        new int[] {5, 6, 12, 8, 9},         // Payline 17
        new int[] {0, 1, 12, 3, 4},         // Payline 18
        new int[] {10, 11, 2, 13, 14},      // Payline 19
        new int[] {0, 11, 12, 13, 4}        // Payline 20
    };
    private Dictionary<SymbolType, float[]> PAY_TABLE = new Dictionary<SymbolType, float[]>()
    {
        {SymbolType.Wild, new float[]{1, 10, 20} },
        {SymbolType.H1, new float[]{1, 10, 20} },
        {SymbolType.H2, new float[]{0.8f, 6, 16} },
        {SymbolType.H3, new float[]{0.6f, 4, 12} },
        {SymbolType.H4, new float[]{0.4f, 2, 8} },
        {SymbolType.L5, new float[]{0.3f, 1.2f, 3} },
        {SymbolType.L4, new float[]{0.3f, 1.2f, 3} },
        {SymbolType.L3, new float[]{0.3f, 1.2f, 3} },
        {SymbolType.L2, new float[]{0.3f, 1.2f, 3} },
        {SymbolType.L1, new float[]{0.3f, 1.2f, 3} },
    };
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _infoPage.gameObject.SetActive(false);

        if (PlayerPrefs.GetInt("splashScreen", 0) == 0)
        {
            _splashScreen.SetActive(true);
            PlayerPrefs.SetInt("splashScreen", 1);
        }
        CreateSpinReelGrid();
        CreateGridSlots();
        UpdateBet(_minBet);
        _balanceText.text = "$" + _currentBalance.ToString("00.00");
        _totalWinText.text = "$" + _score.ToString("00.00");
    }
    private void Start()
    {
        PlayAudio("MainMusic", true);
    }

    private bool CreateGridSlots()
    {
        _reels = new Slot[boardHeight, boardWidth];
        _matchLines = new List<GameObject>();
        int heightLimit = boardHeight - 1;
        int widthLimit = boardWidth - 1;
        int heightIndex = 0;
        int widthIndex = 0;
        for (int i = heightLimit; i >= -heightLimit; i -= 2)
        {
            for (int j = -widthLimit; j <= widthLimit; j += 2)
            {
                Slot thisSlot = Instantiate(slot, _board.transform);
                thisSlot.transform.position = new Vector3(j * 1.1f, i * 1.05f, 0) + _offset;
                thisSlot.name = heightIndex + " " + widthIndex;
                _reels[heightIndex, widthIndex] = thisSlot;
                widthIndex++;
            }
            heightIndex++;
            widthIndex = 0;
        }
        return false;
    }
    private bool CreateSpinReelGrid()
    {
        int heightLimit = 20;
        int widthLimit = 4;
        int heightIndex = 0;
        int widthIndex = 0;
        for (int i = heightLimit; i >= -heightLimit; i -= 2)
        {
            for (int j = -widthLimit; j <= widthLimit; j += 2)
            {
                Slot thisSlot = Instantiate(slot, _spinReelColoumns[widthIndex].transform);
                thisSlot.transform.position = new Vector3(j * 1.1f, i * 1.05f, 0) + _offset;
                thisSlot.name = heightIndex + " " + widthIndex;
                thisSlot.Configure(_gameData.GetRandom(), true);
                thisSlot.AddComponent<Mask>();
                widthIndex++;
            }
            heightIndex++;
            widthIndex = 0;
        }
        for (int i = 0; i < 5; i++)
        {
            _spinReelColoumns[i].transform.localPosition = new Vector3(0,2050f, 0);
        }
        return true;
    }
    public void Spin()
    {
        if(!_isSpinning)
            StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        _isSpinning = true;
        _currentBalance -= _currentBet;
        _balanceText.text = "$" + _currentBalance.ToString("00.00");
        _paylineAnim.gameObject.SetActive(false);
        _spinScreen.SetActive(true);
        Vector3 initPos = new Vector3(0, 2050f, 0);
        PlayAudio(SPIN_AUDIO_NAME);
        for (int i = 0; i < boardWidth; i++)
        {
            GameObject column = _spinReelColoumns[i];
            column.SetActive(true);
            column.transform.localPosition = initPos;
            Slot[] slots = new Slot[boardHeight];
            for (int j = 0; j < boardHeight; j++)
            {
                slots[j] = _reels[j, i];
                slots[j].Reset();
            }
            LeanTween.moveLocalY(column, -2050f, 1f).setEaseInSine().setOnComplete(()=>
            {
                column.SetActive(false);
                foreach (Slot slot in slots)
                {
                    slot.Configure(_gameData.GetRandom());
                }
            });
            yield return new WaitForSeconds(0.5f);
        }
        PlayAudio(SPIN_STOP_AUDIO_NAME);

        yield return new WaitForSeconds(1.5f);
        //CheckReels();
        List<int> reels = new List<int>();
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                reels.Add(_reels[i, j].GetReelValue());
            }
        }
        BestScore bestScore = GetBestScore(reels.ToArray()); 
        Debug.Log("Best Match Score: " + bestScore.Score + " on Payline " + bestScore.PaylineIndex + "Symbol: " + (SymbolType)bestScore.Symbol);

        if (bestScore.PaylineIndex >= 0)
        {
            _score += bestScore.Score;
            _totalWinText.text = "$" + _score.ToString("00.00");
            _paylineAnim.AnimationState.SetAnimation (0, "payline_" + (bestScore.PaylineIndex + 1).ToString(), true);
            List<Vector2Int> indices = GetBestSymbolIndices(bestScore.PaylineIndex, bestScore.Symbol);
            foreach (Vector2Int index in indices)
            {
                _reels[index.x, index.y].PlayWin();
            }
            _paylineAnim.gameObject.SetActive(true);
            string audioName;
            switch ((SymbolType)bestScore.Symbol)
            {
                case SymbolType.H4:
                    audioName = "H4Pay";
                    break;
                case SymbolType.H3:
                    audioName = "H3Pay";
                    break;
                case SymbolType.H2:
                    audioName = "H2Pay";
                    break;
                case SymbolType.H1:
                    audioName = "H1Pay";
                    break;
                case SymbolType.Bonus:
                    audioName = "BonusWin";
                    break;
                default:
                    audioName = "LowPay";
                    break;
            }

            PlayAudio (audioName);
            yield return new WaitForSeconds(1f);
            _winScreen.GetComponentInChildren<SkeletonGraphic>().AnimationState.SetAnimation(0, "01_appear_bigwin", false);
            _winScreen.GetComponentInChildren<SkeletonGraphic>().AnimationState.AddAnimation(0, "01_idle_bigwin", false, 0);
            _spinWinText.text = "$" + bestScore.Score.ToString("00.00");
            _winScreen.gameObject.SetActive(true);

            PlayAudio("BigWin");
            yield return new WaitForSeconds(2f);
            _winScreen.gameObject.SetActive(false);
        }

        _isSpinning = false;

        yield return null;
    }
    public BestScore GetBestScore(int[] reels)
    {
        BestScore bestScore = new BestScore();
        for (int i = 0; i < PAY_LINES.Length; i++)
        {
            var (score, symbol) = GetPaylineScore(reels, PAY_LINES[i]);
            if (score > bestScore.Score)
            {
                bestScore.Score = score;
                bestScore.PaylineIndex = i;
                bestScore.Symbol = symbol;
            }
        }

        return bestScore;
    }

    private (float, int) GetPaylineScore(int[] reels, int[] payline)
{
    Dictionary<int, List<int>> symbolColumns = new Dictionary<int, List<int>>();

    // Collect columns for each symbol in the payline
    foreach (int index in payline)
    {
        int symbol = reels[index];
        int column = index % 5;

        if (!symbolColumns.ContainsKey(symbol))
        {
            symbolColumns[symbol] = new List<int>();
        }
        symbolColumns[symbol].Add(column);
    }

    float maxScore = 0;
    int bestSymbol = -1;

    // Calculate scores only for symbols that are in consecutive columns
    foreach (var kvp in symbolColumns)
    {
        int symbol = kvp.Key;
        List<int> columns = kvp.Value;

        // Exclude scatter and bonus symbols from the scoring process
        if (symbol == 10 || symbol == 11)
        {
            continue;
        }

        // Include wild symbols as part of the sequence
        if (columns.Count >= 3)
        {
            columns.Sort();

            bool areConsecutive = true;
            for (int i = 1; i < columns.Count; i++)
            {
                if (columns[i] != columns[i - 1] + 1)
                {
                    areConsecutive = false;
                    break;
                }
            }

            if (areConsecutive)
            {
                int sequenceLength = columns.Count;
                // Count wild symbols in the sequence
                if((SymbolType)symbol != SymbolType.Wild)
                {
                    int wildCount = 0;
                    foreach (var wildIndex in payline)
                    {
                        if (reels[wildIndex] == 9 && columns.Contains(wildIndex % 5))
                        {
                            wildCount++;
                        }
                    }
                    sequenceLength += wildCount;
                }

                if (TryGetSymbolScore((SymbolType)symbol, sequenceLength, out float symbolScore))
                {
                    float score = symbolScore * _currentBet;
                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestSymbol = symbol;
                    }
                }
            }
        }
    }

    return (maxScore, bestSymbol);
}

    private bool TryGetSymbolScore(SymbolType symbol, int length, out float score)
    {
        score = 1;
        if (PAY_TABLE.TryGetValue(symbol, out float[] value))
        {
            score = value[length - 3];
        }
        return true;
    }
    private List<Vector2Int> GetBestSymbolIndices(int paylineIndex, int symbol)
    {
        List<Vector2Int> symbolIndices = new List<Vector2Int>();

        int[] payline = PAY_LINES[paylineIndex];
        int idx = 0;
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                if(payline.Contains(idx) && symbol == _reels[i,j].GetReelValue())
                {
                    symbolIndices.Add(new Vector2Int(i, j));
                }
                idx++;
            }
        }
        return symbolIndices;
    }
    public void InfoButtonClicked()
    {
        if (!_isSpinning)
        {
            _infoPage.OpenInfoPages();
        }
    }
    public void IncreaseBet()
    {
        if (!_isSpinning)
        {
            PlayAudio(BUTTON_AUDIO);
            UpdateBet(_minBet);
        }
    }

    public void DecreaseBet()
    {
        if (!_isSpinning)
        {
            PlayAudio(BUTTON_AUDIO);
            UpdateBet(-_minBet);
        }
    }
    private void UpdateBet(float value)
    {
        _currentBet += value;
        _currentBet = Mathf.Clamp(_currentBet, _minBet, _maxBet);
        _betText.text = "$" + _currentBet.ToString("00.00");
    }
    private void UpdateBalance(float newBetValue)
    {
        _currentBet = newBetValue;
        _betText.text = "$" + _currentBet.ToString("00.00");
    }
    public void CloseSplashScreen()
    {
        PlayAudio(BUTTON_AUDIO);
        _splashScreen.SetActive(false);
    }
    public void PlayAudio(string audioName, bool loop = false)
    {
        if(_gameData.TryGetAudioClip(audioName, out AudioClip clip))
        {
            LeanAudio.play(clip).loop = loop;
        }
    }
}

public class BestScore
{
    public float Score = 0;
    public int PaylineIndex = -1;
    public int Symbol = -1;
}
