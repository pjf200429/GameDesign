using UnityEngine;

public class BossAnimationManager : MonoBehaviour
{
    public Animator animator;  // Animator component
    private AnimatorOverrideController overrideController;

    void Start()
    {
        // 初始化覆盖控制器并附加到 Animator
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    /// <summary>
    /// Replcae the animation claip
    /// </summary>
    /// <param name="newClip">aim clip</param>
    public void SetDefaultAttackAnimation(AnimationClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("[BossAnimationManager] Tried to set null attack animation.");
            return;
        }

        overrideController["ATTACK"] = newClip;
       // Debug.Log($"[BossAnimationManager] Replaced ATTACK animation with: {newClip.name}");
    }
}
