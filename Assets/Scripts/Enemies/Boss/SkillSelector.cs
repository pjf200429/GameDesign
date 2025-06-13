using System;
using System.Collections.Generic;
using UnityEngine;

// 1. Blackboard: holds the input candidate list and the selected output
public class SkillBlackboard
{
    public List<BossBehaviorTree.BossSkillType> candidates; // input list
    public BossBehaviorTree.BossSkillType nextSkill;       // output selection
    public PlayerAttributes attrs;                         // optional player attributes
    public PlayerHealthController playerHealth;
    public EnemyHealthController enemyHealth;
    public float lastDamageTime;
}

// 2. BTNode base class
public abstract class BTNode
{
    public abstract bool Execute(SkillBlackboard bb);
}

// 3. Sequence node
public class SequenceNode : BTNode
{
    private readonly List<BTNode> _children;
    public SequenceNode(params BTNode[] children) => _children = new List<BTNode>(children);

    public override bool Execute(SkillBlackboard bb)
    {
        foreach (var child in _children)
            if (!child.Execute(bb))
                return false;
        return true;
    }
}

// 4. Selector node
public class SelectorNode : BTNode
{
    private readonly List<BTNode> _children;
    public SelectorNode(params BTNode[] children) => _children = new List<BTNode>(children);

    public override bool Execute(SkillBlackboard bb)
    {
        foreach (var child in _children)
            if (child.Execute(bb))
                return true;
        return false;
    }
}

// 5. Populate candidates node (¿ÉÑ¡)
public class PopulateCandidatesNode : BTNode
{
    public override bool Execute(SkillBlackboard bb)
    {
        if (bb.candidates == null || bb.candidates.Count == 0)
        {
            Debug.LogWarning("[SkillSelector] No candidates provided.");
            return false;
        }
        return true;
    }
}

// 6. Pick skill node
public class PickSkillNode : BTNode
{
    public override bool Execute(SkillBlackboard bb)
    {
        var list = bb.candidates;
        if (list == null || list.Count == 0)
        {
            bb.nextSkill = BossBehaviorTree.BossSkillType.None;
            return false;
        }
        int idx = UnityEngine.Random.Range(0, list.Count);
        bb.nextSkill = list[idx];
        return true;
    }
}

// 7. Custom condition nodes
public class PlayerHealthAboveNode : BTNode
{
    private readonly float _thresholdPct;
    public PlayerHealthAboveNode(float thresholdPct) => _thresholdPct = thresholdPct;

    public override bool Execute(SkillBlackboard bb)
    {
        if (bb.playerHealth == null)
        {
            Debug.LogWarning("[SkillSelector] PlayerHealthAboveNode: playerHealth is null!");
            return false;
        }
        float current = bb.playerHealth.getCurrentHealth;
        float max = bb.playerHealth.slider.maxValue;
        float ratio = current / max;
        bool result = ratio > _thresholdPct;


        return result;
    }
}

public class BossUnhurtNode : BTNode
{
    private readonly float _requiredSeconds;
    public BossUnhurtNode(float seconds) => _requiredSeconds = seconds;

    public override bool Execute(SkillBlackboard bb)
    {
        float timeSinceLastDamage = Time.time - bb.lastDamageTime;
        bool result = timeSinceLastDamage >= _requiredSeconds;

       // Debug.Log($"[SkillSelector] BossUnhurtNode: now={Time.time:F2}, lastDamageTime={bb.lastDamageTime:F2}, delta={timeSinceLastDamage:F2}, required={_requiredSeconds}, result={result}");
        return result;
    }
}


public class SetSkillNode : BTNode
{
    private readonly BossBehaviorTree.BossSkillType _skill;
    public SetSkillNode(BossBehaviorTree.BossSkillType skill) => _skill = skill;
 

    public override bool Execute(SkillBlackboard bb)
    {
        if (bb.candidates != null && bb.candidates.Contains(_skill))
        {
            bb.nextSkill = _skill;
            return true;
        }
        return false;
    }

}

// 8. The skill-selection tree wrapper
public class SkillSelector
{
    private readonly BTNode _root;
    private readonly BTNode meleeSlashSeq;
    private readonly BTNode jumpSmashSeq;
    private readonly BTNode fireballSeq;
    public SkillSelector()
    {
        // MeleeSlash: no condition, always available
        meleeSlashSeq = new SequenceNode(
            new SetSkillNode(BossBehaviorTree.BossSkillType.MeleeSweep)
        );

        // JumpSmash: Player HP > 50% AND boss unhurt for 10s
        jumpSmashSeq = new SequenceNode(
            new PlayerHealthAboveNode(0.5f),
            new BossUnhurtNode(10f),
            new SetSkillNode(BossBehaviorTree.BossSkillType.JumpSmash)
        );

        fireballSeq = new SequenceNode(
       new SetSkillNode(BossBehaviorTree.BossSkillType.Fireball)
   );

        // Selector: Try jumpSmash first, fallback to melee
        _root = new SelectorNode(
            jumpSmashSeq,
            fireballSeq,
            meleeSlashSeq
            
        );
    }


    public BossBehaviorTree.BossSkillType Select(
        List<BossBehaviorTree.BossSkillType> candidates,
        PlayerHealthController phc,
        EnemyHealthController ehc,
        float lastDamageTime
    )
    {
        var bb = new SkillBlackboard
        {
            candidates = candidates,
            playerHealth = phc,
            enemyHealth = ehc,
            lastDamageTime = lastDamageTime
        };

        bool ok = _root.Execute(bb);
        return ok ? bb.nextSkill : BossBehaviorTree.BossSkillType.None;
    }
}
