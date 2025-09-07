// BarMinigame.cs
using UnityEngine;
using UnityEngine.UI;

public class BarMinigame : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] RectTransform barArea;      // 공통 부모(바 영역)
    [SerializeField] RectTransform cursor;       // 움직이는 커서(바Area 하위 권장)
    [SerializeField] RectTransform greenZone;
    [SerializeField] RectTransform yellowZone;
    [SerializeField] Button tapButton, closeButton;

    [Header("Tuning")]
    [SerializeField] float speed = 1.8f;    // 왕복 속도

    float _t;           // 0..1
    bool _running;
    System.Action<int> _onComplete;

    // x-구간 캐시
    (float min, float max) _greenX, _yellowX;

    void Awake()
    {
        tapButton.onClick.AddListener(OnTap);
        closeButton.onClick.AddListener(OnClose);
        gameObject.SetActive(false);
    }

    public void StartGame(System.Action<int> onComplete)
    {
        _onComplete = onComplete;
        _running = true;
        _t = Random.value;
        gameObject.SetActive(true);

        // 레이아웃 최신화 후 경계 캐싱 (한번만)
        CacheRanges();
    }
    void Update()
    {
        if (!_running) return;

        _t = Mathf.PingPong(Time.unscaledTime * speed, 1f);
        UpdateCursorX();
    }

    void UpdateCursorX()
    {
        // 바 영역 로컬 좌표로 커서 이동 (좌:-0.5w ~ 우:+0.5w)
        float w = barArea.rect.width;
        var pos = cursor.anchoredPosition;
        pos.x = Mathf.Lerp(-0.5f * w, 0.5f * w, _t);
        cursor.anchoredPosition = pos;
    }

    // === 핵심: x-구간만 비교 ===
    void CacheRanges()
    {
        _greenX = GetLocalXRange(greenZone, barArea);
        _yellowX = GetLocalXRange(yellowZone, barArea);
    }

    static (float min, float max) GetLocalXRange(RectTransform target, RectTransform relativeTo)
    {
        // relativeTo(=barArea) 기준 AABB 구간
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(relativeTo, target);
        return (b.min.x, b.max.x);
    }

    int EvaluateByX()
    {
        // 커서의 barArea-로컬 x
        float x = barArea.InverseTransformPoint(cursor.position).x;

        // 경계가 조금 겹쳐도 초록 > 노랑 순서로 우선 판정
        if (x >= _greenX.min && x <= _greenX.max) return 3;
        if ((x >= _yellowX.min && x <= _yellowX.max)) return 1;
        return 0; // 나머지 전부 빨강
    }

    void OnTap()
    {
        if (!_running) return;
        int score = EvaluateByX();
        _running = false;
        _onComplete?.Invoke(score);
    }

    void OnClose()
    {
        if (_running) { _running = false; _onComplete?.Invoke(0); }
    }
}
