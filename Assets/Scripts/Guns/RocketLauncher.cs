using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject prefabRocket;

    public float ShotPower = 8f;

    public float searchRadius = 10f;

    [Header("발사 설정")]
    public float fireInterval = 2.0f;

    private float fireTimer = 0f;

    public Player master;

    [Header("폭발 설정")]
    public float explosionRadius = 2f;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;

            Shot(master);
        }
    }

    // 1. 특정 타겟을 지정해서 발사
    public void Shot(Transform target, Player master)
    {
        if (target == null)
        {
            Debug.LogWarning(
                "[RocketLauncher] Shot 실패: 타겟(Target)이 null입니다."
            );

            return;
        }

        Debug.Log(
            $"[RocketLauncher] 타겟 발견! " +
            $"타겟 이름: {target.name}"
        );

        Vector2 dir =
            (target.position - transform.position).normalized;

        GameObject copyRocket =
            Instantiate(
                prefabRocket,
                transform.position,
                Quaternion.identity
            );

        Rocket rocket =
            copyRocket.GetComponent<Rocket>();

        if (rocket != null)
        {
            rocket.master = master;
            rocket.moveSpeed = ShotPower;

            rocket.SetExplosionRadius(explosionRadius);
            rocket.SetTarget(target);
        }
        else
        {
            Debug.LogWarning(
                "[RocketLauncher] 생성된 로켓 프리팹에 " +
                "'Rocket' 컴포넌트가 없습니다."
            );
        }

        Rigidbody2D rigidbody =
            copyRocket.GetComponent<Rigidbody2D>();

        if (rigidbody != null)
        {
            rigidbody.AddForce(
                dir * ShotPower,
                ForceMode2D.Impulse
            );
        }
        else
        {
            Debug.LogWarning(
                "[RocketLauncher] 생성된 로켓 프리팹에 " +
                "'Rigidbody2D' 컴포넌트가 없습니다."
            );
        }

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        copyRocket.transform.rotation =
            Quaternion.AngleAxis(
                angle,
                Vector3.forward
            );
    }

    // 2. 가장 가까운 적을 찾아서 발사
    public void Shot(Player master)
    {
        Transform nearestEnemy =
            FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Shot(nearestEnemy, master);
        }
        else
        {
            Debug.Log(
                "[RocketLauncher] 주변에 적이 없습니다."
            );
        }
    }

    // 3. 방향을 지정해서 발사
    public void Shot(Vector3 dir, Player master)
    {
        GameObject copyRocket =
            Instantiate(
                prefabRocket,
                transform.position,
                Quaternion.identity
            );

        Rocket rocket =
            copyRocket.GetComponent<Rocket>();

        if (rocket != null)
        {
            rocket.master = master;
            rocket.moveSpeed = ShotPower;
            rocket.SetExplosionRadius(explosionRadius);
        }

        Rigidbody2D rigidbody =
            copyRocket.GetComponent<Rigidbody2D>();

        if (rigidbody != null)
        {
            rigidbody.AddForce(
                dir.normalized * ShotPower,
                ForceMode2D.Impulse
            );
        }

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        copyRocket.transform.rotation =
            Quaternion.AngleAxis(
                angle,
                Vector3.forward
            );
    }

    // 가장 가까운 Monster 찾기
    Transform FindNearestEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                searchRadius,
                1 << LayerMask.NameToLayer("Monster")
            );

        Transform nearest = null;

        float minDistance = Mathf.Infinity;

        Vector3 currentPos =
            transform.position;

        foreach (Collider2D enemy in enemies)
        {
            float dist =
                Vector3.Distance(
                    currentPos,
                    enemy.transform.position
                );

            if (dist < minDistance &&
                dist <= searchRadius)
            {
                minDistance = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            searchRadius
        );
    }
}