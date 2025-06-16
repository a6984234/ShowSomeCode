using DG.Tweening;
using UnityEngine;

public class Archer : Character
{
    [SerializeField] private GameObject arrowPrefab;
    float arrowSpeed = 7.5f;
    [SerializeField] private float arcHeight = 0.2f;

    protected override float AttackRange => 3f;
    protected override void Attack()
    {
        atkTimer = 1f / CharacterData.AtkSpd;

        var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Vector3 startPos = transform.position;
        Vector3 endPos = moveTarget.position;
        float duration = Vector3.Distance(startPos, endPos) / arrowSpeed;
        float height = arcHeight;


        Character target = moveTarget?.GetComponent<Character>();

        DOTween.To(() => 0f, t =>
        {
                endPos = moveTarget != null ? moveTarget.position : endPos;
                var arcDir = startPos.x - endPos.x > 0 ? 1f : -1f;
                Vector3 direction = endPos - startPos;
                Vector3 perp = Vector3.Cross(direction.normalized, Vector3.forward);
                Vector3 linear = Vector3.Lerp(startPos, endPos, t);
                Vector3 arc = perp * Mathf.Sin(t * Mathf.PI) * height * arcDir;

                Vector3 current = linear + arc;
                Vector3 nextLinear = Vector3.Lerp(startPos, endPos, Mathf.Min(t + 0.01f, 1f));
                Vector3 nextArc = perp * Mathf.Sin(Mathf.Min(t + 0.01f, 1f) * Mathf.PI) * height * arcDir;
                Vector3 nextPos = nextLinear + nextArc;

                arrow.transform.position = current;
                arrow.transform.right = nextPos - current;            
        }, 1f, duration).SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            Destroy(arrow);
            if (target != null)
            {
                target.TakeDamage(CharacterData.Atk);
                if(target.TryGetComponent<ContributionTracker>(out ContributionTracker enemy))
                {
                    enemy.RegisterDamage(this);
                }
                CombatTextSpawner.Instance.SpawnDamageText(target.transform.position, transform.position, CharacterData.Atk, Color.red);
                
            }
        });
    }
}
