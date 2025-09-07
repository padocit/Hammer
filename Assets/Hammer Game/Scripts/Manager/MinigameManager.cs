using UnityEngine;
using System;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("Main Canvas Elements")]
    [SerializeField] GameObject _miniGameWindow;  // MiniGameWindow 루트 패널
    private BarMinigame _bar;            // MiniGameWindow에 붙은 BarMinigame

    bool _running;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        _bar = _miniGameWindow.GetComponent<BarMinigame>();
        _miniGameWindow.SetActive(false);
        _running = false;
    }

    public void ShowBarMinigame(Action<int> onComplete)
    {
        if (_running || _bar == null) 
            return;

        _running = true;

        _miniGameWindow.SetActive(true);

        _bar.StartGame(score =>
        {
            onComplete?.Invoke(score);
            CloseMinigame();
        });
    }

    public void CloseMinigame()
    {
        if (!_running) return;
        _running = false;

        _miniGameWindow.SetActive(false);
    }
}
