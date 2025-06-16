using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public static event UnityAction<Character> OnCharacterDied;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private HpBar hpBar;
    public CharacterData CharacterData => characterData;
    protected List<ISkill> skills = new List<ISkill>();
    protected float moveSpeed = 1f;
    protected virtual float AttackRange => 1f;
    protected float atkTimer = 0f;
    protected bool canAttack => atkTimer <= 0f;
    [SerializeField] protected Transform moveTarget;
    protected NavMeshAgent navMeshAgent;
    public RenderTexture RenderTexture => characterCamera != null ? characterCamera.targetTexture : null;
    void Start()
    {
        SetupRenderTexture();
        SetupNavMeshAgent();
    }
    private void SetupNavMeshAgent()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.updateRotation = false;
                navMeshAgent.updateUpAxis = false;
                navMeshAgent.speed = moveSpeed;
                navMeshAgent.stoppingDistance = AttackRange;
                return;
            }
            Debug.LogWarning("NavMeshAgent is not found on " + gameObject.name);
        }
    }
    private void SetupRenderTexture()
    {
        if (characterCamera == null) return;
        var rt = new RenderTexture(200, 155, 1);
        characterCamera.targetTexture = rt;
    }

    void Update()
    {
        TickAI();
    }
    public void SetupCharacter(CharacterData characterData)
    {
        this.characterData = characterData;
        hpBar?.SetHpBar((float)characterData.Hp / characterData.MaxHp);
    }
    public virtual void TakeDamage(int damage)
    {
        characterData.TakeDamage(damage);
        hpBar?.SetHpBar((float)characterData.Hp / characterData.MaxHp);
        if (characterData.Hp <= 0)
        {
            Die();
        }
    }
    public void Heal(int amount)
    {
        characterData.Heal(amount);
        hpBar?.SetHpBar((float)characterData.Hp / characterData.MaxHp);
    }
    public virtual void LevelUp()
    {
        characterData.LevelUp();
    }
    protected virtual void TickAI()
    {
        TickAttack();
        TickSkill();
        if (moveTarget == null)
            SetMoveTarget();
        if (moveTarget == null) return;
        if (HasReachedDestination() && canAttack)
        {
            Attack();
            return;
        }
        if (!HasReachedDestination())
            MoveToTarget();
    }
    protected virtual void TickAttack()
    {
        atkTimer -= Time.deltaTime;
    }
    protected virtual void MoveToTarget()
    {
        navMeshAgent.SetDestination(moveTarget.position);
    }
    protected virtual void TickSkill()
    {
        foreach (var skill in skills.OfType<ActiveSkill>())
        {
            skill.TryUse(this);
        }
    }
    protected virtual void SetMoveTarget()
    {
        switch (characterData.Faction)
        {
            case CharacterData.FactionType.Ally:
                moveTarget = SpawnEnemyManager.Instance.GetClosestEnemy(transform.position)?.transform;
                break;
            case CharacterData.FactionType.Enemy:
                moveTarget = MercenaryManager.Instance.GetClosestMercenary(transform.position)?.transform;
                break;
        }
        if (moveTarget != null)
            navMeshAgent.SetDestination(moveTarget.position);
    }
    protected virtual void Attack()
    {
        atkTimer = 1f / characterData.AtkSpd;
        if (moveTarget.TryGetComponent(out Character target))
        {
            target.TakeDamage(characterData.Atk);
            if (target.TryGetComponent<ContributionTracker>(out ContributionTracker enemy))
            {
                enemy.RegisterDamage(this);
            }
            CombatTextSpawner.Instance.SpawnDamageText(moveTarget.position, transform.position, characterData.Atk, Color.red);
        }
    }
    protected bool IsTargetInRange()
    {
        if (moveTarget == null) return false;
        float distSqr = (moveTarget.position - transform.position).sqrMagnitude;
        return distSqr <= AttackRange * AttackRange;
    }
    protected bool HasReachedDestination()
    {
        return !navMeshAgent.pathPending &&
               navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
               (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }
    private void Die()
    {
        if (characterData.Faction == CharacterData.FactionType.Ally)
        {
            MercenaryManager.Instance.Dismiss(this);
        }
        else if (characterData.Faction == CharacterData.FactionType.Enemy)
        {
            SpawnEnemyManager.Instance.RemoveEnemy(this);
        }
        OnCharacterDied?.Invoke(this);
        DistributeExp();
        DistributeMoney();
        Destroy(gameObject);
    }
    private void DistributeMoney()
    {
        if (characterData.Faction == CharacterData.FactionType.Enemy)
        {
            PlayerDataManager.Instance.AddGold(characterData.DropMoney);
            Debug.Log($"Character {characterData.Type} dropped {characterData.DropMoney} gold.");
        }
    }
    private void DistributeExp()
    {
        if (characterData.Faction == CharacterData.FactionType.Ally) return;

        if (TryGetComponent<ContributionTracker>(out ContributionTracker tracker))
        {
            var contributors = tracker.GetContributors().ToList();
            if(contributors.Count == 0)
            {
                Debug.LogWarning("No contributors found for " + gameObject.name);
                return;
            }
            int exp = characterData.ExpRewardOnDeath / contributors.Count;
            if (exp <= 0) exp = 1;
            foreach (var contributor in contributors)
            {
                contributor.GainExp(exp);
            }
            return;
        }
        Debug.LogWarning("ContributionTracker not found on " + gameObject.name);

    }

    public void GainExp(float exp)
    {
        characterData.GainExp(exp);
        Debug.Log($"Character {characterData.Type} gained {exp} exp. Current exp: {characterData.Exp}/{characterData.ExpToNextLevel}");
    }
    public void LearnSkill(ISkill skill)
    {
        if (skills.Contains(skill)) return;
        skills.Add(skill);
    }
    public bool GetSkill<T>(out T result) where T : class, ISkill
    {
        result = skills.OfType<T>().FirstOrDefault();
        return result != null;
    }
    void OnDestroy()
    {
        if (characterCamera != null && characterCamera.targetTexture != null)
        {
            characterCamera.targetTexture.Release();
            characterCamera.targetTexture = null;
        }
    }
    #region Test Methods
    [ContextMenu("Die")]
    private void GoDie()
    {
        TakeDamage(999); // Trigger die logic
    }
    [ContextMenu("Level Up")]
    private void ForceLevelUp()
    {
        LevelUp();
        hpBar?.SetHpBar((float)characterData.Hp / characterData.MaxHp);
        Debug.Log($"Character {characterData.Type} leveled up to {characterData.Level}. Current HP: {characterData.Hp}/{characterData.MaxHp}");
    }
    #endregion
}
