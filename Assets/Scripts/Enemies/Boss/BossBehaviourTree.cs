// --- BossBehaviorTree.cs ---
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviorTree
{
    private BossEnemy _boss;
    private BossSkillController _skills;
    private SkillSelector _selector = new SkillSelector();
    private PlayerHealthController _phc;
    private EnemyHealthController _ehc;

    private BossSkillType _nextSkill = BossSkillType.None;


    public BossSkillType NextSkill => _nextSkill; 

    [Header("Patrol Settings")]
    private float patrolSpeed = 2f;
    private float changeDirectionInterval = 3f;

    private Vector2 _moveDirection;
    private float _changeTimer;


    public BossBehaviorTree(BossEnemy boss, BossSkillController skills, PlayerHealthController phc, EnemyHealthController ehc)
    {
        _boss = boss;
        _skills = skills;
        _changeTimer = 0f;
        _phc = phc;
        _ehc = ehc;
    }
    public enum BossSkillType
    {
        None,
        MeleeSweep,
        JumpSmash,
        Fireball,
        ClusterBomb,
        Charge,
        RageCombo,
        DodgeCounter
    }

    public bool ShouldAttack(float dummy)
    {
        _nextSkill = BossSkillType.None;

        float hpPct = GetHP();
        float dist = _boss.PlayerDistance;

        // Collect candidate skills
        var candidates = new List<BossSkillType>();
       
        // Phase 1: only melee sweep and jump smash
        if (hpPct > _boss.phase2Threshold)
        {
            if (dist <= _skills.meleeSweepRange && _skills.IsSkillReady(BossSkillType.MeleeSweep))

            {
                candidates.Add(BossSkillType.MeleeSweep);
            }
            if (dist <= _skills.jumpSmashRange && _skills.IsSkillReady(BossSkillType.JumpSmash))
                candidates.Add(BossSkillType.JumpSmash);
        }
        // Phase 2: all other skills become available alongside Phase 1's
        else
        {
            
            // Summon (side-effect only)
            //if (_skills.IsSkillReady(BossSkillType.MeleeSweep))
            //    _skills.SummonMinions(_boss.MinionPrefab, _boss.SummonPoints);
            // Melee fallback
            if (dist <= _skills.meleeSweepRange && _skills.IsSkillReady(BossSkillType.MeleeSweep))
                candidates.Add(BossSkillType.MeleeSweep);
            // Jump Smash
            if (dist <= _skills.jumpSmashRange && _skills.IsSkillReady(BossSkillType.JumpSmash))
                candidates.Add(BossSkillType.JumpSmash);
            // Fireball
            if (dist > _skills.meleeSweepRange && dist <= _skills.fireballRange && _skills.IsSkillReady(BossSkillType.Fireball))
                candidates.Add(BossSkillType.Fireball);
            // Charge
            //if (dist > 2f && dist <= _skills.chargeRange && _skills.IsSkillReady(BossSkillType.Charge))
            //    candidates.Add(BossSkillType.Charge);
            // Cluster Bomb
            //if (dist > 4f && _skills.IsSkillReady(BossSkillType.ClusterBomb))
            //    candidates.Add(BossSkillType.ClusterBomb);
            //// Rage Combo
            //if (dist <= _skills.rageComboRange && _skills.IsSkillReady(BossSkillType.RageCombo))
            //    candidates.Add(BossSkillType.RageCombo);

            // Shield is a side-effect, not added to candidates
            if (Time.time - _skills.LastShieldTime >= _skills.shieldCooldown && _boss.PendingShieldRequest)
            {
                _skills.ActivateShield();
                _boss.SetShieldTime(Time.time);
                _boss.SetPeningRequest(false);
                _boss.SetClearRequest(true);
                _boss.SetInvulnerable(false);
            }
           // Debug.Log(_boss.ShieldTimer);
           if (!_boss.PendingShieldRequest && _boss.ClearShieldRequest && Time.time - _boss.ShieldTimer > 5f)
            {
                _skills.DeactivateShield();
                _boss.SetClearRequest(false);
                _boss.SetInvulnerable(true);
                
            }

        }

        // Pick one skill from candidates (e.g., first or random)
        if (candidates.Count > 0)
        {
            _nextSkill = _selector.Select(candidates,_phc,_ehc,_ehc.GetLastDamageTime());
            candidates.Clear();
        }


     
        return _nextSkill != BossSkillType.None;
    }


    


    public void Tick(float hpPct)
    {
        float dist = _boss.PlayerDistance;

        if (dist > 5f)
        {
            MoveTowardsPlayer();
        }
        else
        {
            IdleOrPatrol();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (_boss.Player == null) return;
        Vector2 direction = (_boss.Player.position - _boss.transform.position).normalized;
        _boss.transform.position += (Vector3)(direction * patrolSpeed * Time.deltaTime);

        if (direction.x > 0f)
        {
            _boss.spriteHolder.localScale = new Vector3(1, 1, 1);
            _boss.FacingRight = true;
        }
        else
        {
            _boss.spriteHolder.localScale = new Vector3(-1, 1, 1);
            _boss.FacingRight = false;
        }
    }

    private void IdleOrPatrol()
    {
        _changeTimer -= Time.deltaTime;
        if (_changeTimer <= 0f)
        {
            PickRandomDirection();
        }

        Vector2 pos = _boss.transform.position;
        Vector2 newPos = pos + _moveDirection * patrolSpeed * Time.deltaTime;
        _boss.transform.position = newPos;

        if (_moveDirection.x > 0f)
        {
            _boss.spriteHolder.localScale = new Vector3(1, 1, 1);
            _boss.FacingRight = true;
        }
        else if (_moveDirection.x < 0f)
        {
            _boss.spriteHolder.localScale = new Vector3(-1, 1, 1);
            _boss.FacingRight = false;
        }
    }

    private void PickRandomDirection()
    {
        _moveDirection = Random.insideUnitCircle.normalized;
        _changeTimer = changeDirectionInterval;
    }

    private float GetHP()
    {
        EnemyHealthController hc = _boss.GetComponent<EnemyHealthController>();
        return hc != null ? hc.CurrentHealth / (float)hc.MaxHealth : 1f;
    }
}