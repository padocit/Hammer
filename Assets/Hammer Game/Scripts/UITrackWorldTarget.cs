using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)] // ī�޶�/�ִϸ��̼� ���� �� ����
public class UITrackWorldTarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;                 // �Ӹ� �� or �� ������Ʈ(UIAnchor)
    [SerializeField] Vector3 worldOffset = new Vector3(0, 1.8f, 0);

    [Header("UI")]
    [SerializeField] Canvas canvas;                    // Main Canvas (Screen Space - Overlay/Camera)
    [SerializeField] RectTransform rect;               // �� UI�� RectTransform
    [SerializeField] Camera worldCamera;               // ���� Main Camera

    [Header("Options")]
    [SerializeField] bool hideWhenOffscreen = true;
    [SerializeField] Vector2 screenPadding = new Vector2(8f, 8f); // UI�� ȭ�� ������ ��¦ ������ �� ����
    [SerializeField] float smooth = 0f;                // 0�̸� ���, >0�̸� ��¦ �ε巴��

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

        // 1) ���� ��ġ �� ����Ʈ ��ǥ(0~1)
        Vector3 wp = target.position + worldOffset;
        Vector3 vp = worldCamera.WorldToViewportPoint(wp);

        // ī�޶� �ڸ� ����
        if (hideWhenOffscreen && vp.z < 0f)
        {
            if (rect.gameObject.activeSelf) rect.gameObject.SetActive(false);
            return;
        }

        if (!rect.gameObject.activeSelf) rect.gameObject.SetActive(true);

        // 2) ����Ʈ �� ĵ���� ���� ��ǥ
        RectTransform canvasRect = (RectTransform)canvas.transform;
        Vector2 canvasSize = canvasRect.rect.size;

        // �߾� ����(Anchor/Pivot�� 0.5,0.5��� ����; �ƴϸ� �׿� ���� ����)
        Vector2 targetLocalPos = new Vector2(
            (vp.x - 0.5f) * canvasSize.x,
            (vp.y - 0.5f) * canvasSize.y
        );

        // 3) �е����� ��¦ Ŭ���� (ȭ�� ������ �ʹ� �� ������)
        Vector2 half = canvasSize * 0.5f - screenPadding;
        targetLocalPos.x = Mathf.Clamp(targetLocalPos.x, -half.x, half.x);
        targetLocalPos.y = Mathf.Clamp(targetLocalPos.y, -half.y, half.y);

        // 4) ����(�ε巴�� �ɼ�)
        if (smooth > 0f)
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, targetLocalPos, ref _vel, smooth);
        else
            rect.anchoredPosition = targetLocalPos;
    }

    public void SetTarget(Transform t) => target = t;
}
