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
                // 이미 처리된 것이 아니고, 아직 언락되지 않은 경우에만 처리
                if (!_processedUnlockables.Contains(unlockableBase) && !unlockableBase.IsUnlocked)
                {
                    Debug.Log($"[PlayerDetection] Attempting to unlock: {unlockableBase.gameObject.name}");
                    unlockableBase.TryUnlock();
                    
                    // 한 번 언락 시도한 것은 기록 (완전히 언락될 때까지 계속 시도하려면 이 줄 제거)
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
