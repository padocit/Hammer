using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFocus : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _playerCam;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _focusCam;
    [SerializeField] private float _holdTime = 1.5f;
    private int _prevFocusCamPriority;

    public void FocusOnTarget(Transform target)
    {
        _focusCam.Follow = target;
        _focusCam.LookAt = target;

        _prevFocusCamPriority = _focusCam.Priority;
        _focusCam.Priority = _playerCam.Priority + 1;
        StartCoroutine(ReturnToPlayer(_holdTime));
    }

    private IEnumerator ReturnToPlayer(float t = 2f)
    {
        yield return new WaitForSeconds(t);
        _focusCam.Priority = _prevFocusCamPriority;
    }
}
