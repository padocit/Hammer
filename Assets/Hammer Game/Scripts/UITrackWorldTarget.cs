using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)] // 카메라/애니메이션 적용 후 실행
public class UITrackWorldTarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;                 // 머리 본 or 빈 오브젝트(UIAnchor)
    [SerializeField] Vector3 worldOffset = new Vector3(0, 1.8f, 0);

    [Header("UI")]
    [SerializeField] Canvas canvas;                    // Main Canvas (Screen Space - Overlay/Camera)
    [SerializeField] RectTransform rect;               // 이 UI의 RectTransform
    [SerializeField] Camera worldCamera;               // 보통 Main Camera

    [Header("Options")]
    [SerializeField] bool hideWhenOffscreen = true;
    [SerializeField] Vector2 screenPadding = new Vector2(8f, 8f); // UI가 화면 밖으로 살짝 나가는 것 방지
    [SerializeField] float smooth = 0f;                // 0이면 즉시, >0이면 살짝 부드럽게

    Vector2 _vel;

    void Reset()
    {
        canvas = GetComponentInParent<Canvas>();
        rect = (RectTransform)transform;
        worldCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (!target || !canvas || !rect || !worldCamera) return;

        // 1) 월드 위치 → 뷰포트 좌표(0~1)
        Vector3 wp = target.position + worldOffset;
        Vector3 vp = worldCamera.WorldToViewportPoint(wp);

        // 카메라 뒤면 숨김
        if (hideWhenOffscreen && vp.z < 0f)
        {
            if (rect.gameObject.activeSelf) rect.gameObject.SetActive(false);
            return;
        }

        if (!rect.gameObject.activeSelf) rect.gameObject.SetActive(true);

        // 2) 뷰포트 → 캔버스 로컬 좌표
        RectTransform canvasRect = (RectTransform)canvas.transform;
        Vector2 canvasSize = canvasRect.rect.size;

        // 중앙 기준(Anchor/Pivot이 0.5,0.5라고 가정; 아니면 그에 맞춰 보정)
        Vector2 targetLocalPos = new Vector2(
            (vp.x - 0.5f) * canvasSize.x,
            (vp.y - 0.5f) * canvasSize.y
        );

        // 3) 패딩으로 살짝 클램프 (화면 밖으로 너무 안 나가게)
        Vector2 half = canvasSize * 0.5f - screenPadding;
        targetLocalPos.x = Mathf.Clamp(targetLocalPos.x, -half.x, half.x);
        targetLocalPos.y = Mathf.Clamp(targetLocalPos.y, -half.y, half.y);

        // 4) 적용(부드럽게 옵션)
        if (smooth > 0f)
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, targetLocalPos, ref _vel, smooth);
        else
            rect.anchoredPosition = targetLocalPos;
    }

    public void SetTarget(Transform t) => target = t;
}
