using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Slider _progressionBar;

    private void Start()
    {
        StartCoroutine(LoadGameScene("GamePlayScene"));
    }
    IEnumerator LoadGameScene(string LvlToLoad)
    {
        AsyncOperation loadAync = SceneManager.LoadSceneAsync(LvlToLoad);

        while (!loadAync.isDone)
        {
            float progressValue = Mathf.Clamp01(loadAync.progress / 0.9f);
            _progressionBar.value = progressValue;
            yield return null;
        }
    }
}
