// BarMinigame.cs
using UnityEngine;
using UnityEngine.UI;

public class BarMinigame : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] RectTransform barArea;      // ���� �θ�(�� ����)
    [SerializeField] RectTransform cursor;       // �����̴� Ŀ��(��Area ���� ����)
    [SerializeField] RectTransform greenZone;
    [SerializeField] RectTransform yellowZone;
    [SerializeField] Button tapButton, closeButton;

    [Header("Tuning")]
    [SerializeField] float speed = 1.8f;    // �պ� �ӵ�

    float _t;           // 0..1
    bool _running;
    System.Action<int> _onComplete;

    // x-���� ĳ��
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

        // ���̾ƿ� �ֽ�ȭ �� ��� ĳ�� (�ѹ���)
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
        // �� ���� ���� ��ǥ�� Ŀ�� �̵� (��:-0.5w ~ ��:+0.5w)
        float w = barArea.rect.width;
        var pos = cursor.anchoredPosition;
        pos.x = Mathf.Lerp(-0.5f * w, 0.5f * w, _t);
        cursor.anchoredPosition = pos;
    }

    // === �ٽ�: x-������ �� ===
    void CacheRanges()
    {
        _greenX = GetLocalXRange(greenZone, barArea);
        _yellowX = GetLocalXRange(yellowZone, barArea);
    }

    static (float min, float max) GetLocalXRange(RectTransform target, RectTransform relativeTo)
    {
        // relativeTo(=barArea) ���� AABB ����
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(relativeTo, target);
        return (b.min.x, b.max.x);
    }

    int EvaluateByX()
    {
        // Ŀ���� barArea-���� x
        float x = barArea.InverseTransformPoint(cursor.position).x;

        // ��谡 ���� ���ĵ� �ʷ� > ��� ������ �켱 ����
        if (x >= _greenX.min && x <= _greenX.max) return 3;
        if ((x >= _yellowX.min && x <= _yellowX.max)) return 1;
        return 0; // ������ ���� ����
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
