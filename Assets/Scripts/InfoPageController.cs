using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InfoPageController : MonoBehaviour
{
    public List<InfoPage> _pages;
    public Sprite IndicatorNonFilled;
    public Sprite IndicatorFilled;
    private int _currentPageShowing = 0;
    private void Awake()
    {
        foreach (var page in _pages)
        {
            page.PageObject.SetActive(false);
            page.PageIndicator.sprite = IndicatorNonFilled;
        }
    }
    public void OpenInfoPages()
    {
        this.gameObject.SetActive(true);
        _pages[0].PageObject.SetActive(true);
        _pages[0].PageIndicator.sprite = IndicatorFilled;
        _currentPageShowing = 0;
    }
    public void CloseInfoPages()
    {
        _pages[_currentPageShowing].PageObject.SetActive(false);
        _pages[_currentPageShowing].PageIndicator.sprite = IndicatorNonFilled;
        _currentPageShowing = 0;
        this.gameObject.SetActive(false);
    }
    public void NextPage()
    {
        ShowPage(_currentPageShowing + 1);
    }
    public void PrevPage()
    {
        ShowPage(_currentPageShowing - 1);
    }
    public void ShowPage(int pageNo)
    {
        if(pageNo < _pages.Count && pageNo >= 0)
        {
            _pages[pageNo].PageObject.SetActive(true);
            _pages[pageNo].PageIndicator.sprite = IndicatorFilled;
            _pages[_currentPageShowing].PageObject.SetActive(false);
            _pages[_currentPageShowing].PageIndicator.sprite = IndicatorNonFilled;
            _currentPageShowing = pageNo;
        }
    }
}
[Serializable]
public struct InfoPage
{
    public Image PageIndicator;
    public GameObject PageObject;
}
