using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Terraria-style melee swing: pivot rotates a weapon sprite in an arc
// from behind the aim direction to in front of it, using an easing curve.
[DisallowMultipleComponent]
public class SwingWeapon : MonoBehaviour
{
    [Header("Targeting")]
    public LayerMask enemyLayer;
    public float range = 2f;
    public float attackInterval = 1f;

    [Header("Swing")]
    public Transform pivot;
    public float arcDegrees = 120f;
    public float swingDuration = 0.5f;
    public float hitAngleWidth = 40f;
    public AnimationCurve swingCurve = DefaultSwingCurve();

    static AnimationCurve DefaultSwingCurve()
    {
        // slow-in, fast-through-the-middle, slow-out — reads softer than a plain ease-in-out
        var curve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.5f, 0.5f, 0.9f, 0.9f),
            new Keyframe(1f, 1f, 0f, 0f)
        );
        for (int i = 0; i < curve.length; i++)
            curve.SmoothTangents(i, 0f);
        return curve;
    }

    [Header("Damage")]
    public int damage = 10;

    float cooldownTimer;
    bool isSwinging;
    readonly HashSet<Collider2D> hitThisSwing = new HashSet<Collider2D>();

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (isSwinging || cooldownTimer > 0f) return;

        Transform target = FindNearestEnemy();
        if (target == null) return;

        Vector2 dir = (target.position - pivot.position).normalized;
        StartCoroutine(Swing(dir));
        cooldownTimer = attackInterval;
    }

    Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pivot.position, range, enemyLayer);
        Transform nearest = null;
        float nearestSqrDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float sqrDist = ((Vector2)hit.transform.position - (Vector2)pivot.position).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = hit.transform;
            }
        }
        return nearest;
    }

    IEnumerator Swing(Vector2 aimDir)
    {
        isSwinging = true;
        hitThisSwing.Clear();

        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        float startAngle = aimAngle + arcDegrees * 0.5f;
        float endAngle = aimAngle - arcDegrees * 0.5f;

        float t = 0f;
        while (t < swingDuration)
        {
            t += Time.deltaTime;
            float progress = swingCurve.Evaluate(Mathf.Clamp01(t / swingDuration));
            float currentAngle = Mathf.LerpAngle(startAngle, endAngle, progress);
            pivot.rotation = Quaternion.Euler(0, 0, currentAngle);

            CheckHits(currentAngle);
            yield return null;
        }

        pivot.rotation = Quaternion.Euler(0, 0, endAngle);
        isSwinging = false;
    }

    void CheckHits(float bladeAngle)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pivot.position, range, enemyLayer);
        foreach (var hit in hits)
        {
            if (hitThisSwing.Contains(hit)) continue;

            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)pivot.position).normalized;
            float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            float delta = Mathf.Abs(Mathf.DeltaAngle(bladeAngle, targetAngle));

            if (delta <= hitAngleWidth * 0.5f)
            {
                hitThisSwing.Add(hit);
                hit.GetComponent<IDamageable>()?.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pivot == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot.position, range);
    }
}
