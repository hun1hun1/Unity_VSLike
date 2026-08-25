using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public Player master;

    private Transform target;

    private Vector3 vStart;

    [Header("로켓 설정")]
    public float moveSpeed = 8f;
    public float maxDistance = 15f;

    private float explosionRadius = 2f;

    private bool exploded = false;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }

    void Start()
    {
        vStart = transform.position;
    }

    void Update()
    {
        if (exploded)
            return;

        // 타겟이 죽거나 삭제된 경우
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟 방향 계산
        Vector3 dir =
            (target.position - transform.position).normalized;

        // 타겟 방향으로 이동
        transform.position +=
            dir * moveSpeed * Time.deltaTime;

        // 로켓 방향 회전
        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.AngleAxis(
                angle,
                Vector3.forward
            );

        // 최대 거리
        float fDist =
            Vector3.Distance(
                vStart,
                transform.position
            );

        if (fDist >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (exploded)
            return;

        if (collision.gameObject.tag == "Monster")
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        Collider2D[] monsters =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius,
                1 << LayerMask.NameToLayer("Monster")
            );

        foreach (Collider2D monster in monsters)
        {
            Player targetPlayer =
                monster.GetComponent<Player>();

            if (targetPlayer == null)
                continue;

            master.Attack(targetPlayer);

            if (targetPlayer.Death())
            {
                GameManager.GetInstacne()
                    .monsterInventory
                    .AddMonster(targetPlayer.name);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}