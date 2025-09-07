using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private HashSet<UnlockableBase> _processedUnlockables = new HashSet<UnlockableBase>();
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("UnlockTrigger"))
        {
            UnlockableBase unlockableBase = other.GetComponentInParent<UnlockableBase>();
            if (unlockableBase != null)
            {
                // �̹� ó���� ���� �ƴϰ�, ���� ������� ���� ��쿡�� ó��
                if (!_processedUnlockables.Contains(unlockableBase) && !unlockableBase.IsUnlocked)
                {
                    Debug.Log($"[PlayerDetection] Attempting to unlock: {unlockableBase.gameObject.name}");
                    unlockableBase.TryUnlock();
                    
                    // �� �� ��� �õ��� ���� ��� (������ ����� ������ ��� �õ��Ϸ��� �� �� ����)
                    // _processedUnlockables.Add(unlockableBase);
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UnlockTrigger"))
        {
            UnlockableBase unlockableBase = other.GetComponentInParent<UnlockableBase>();
            if (unlockableBase != null)
            {
                _processedUnlockables.Remove(unlockableBase);
                Debug.Log($"[PlayerDetection] Exited unlock area: {unlockableBase.gameObject.name}");
            }
        }
    }
}
