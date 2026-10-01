using UnityEngine;

public class Brick : MonoBehaviour
{
    public Player master;

    [Header("벽돌 설정")]
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            Player target =
                collision.gameObject.GetComponent<Player>();

            Player attacker = master;

            if (target != null && attacker != null)
            {
                attacker.Attack(target);

                if (target.Death())
                {
                    GameManager.GetInstacne()
                        .monsterInventory
                        .AddMonster(target.name);
                }
            }

            // Destroy(gameObject)를 제거
        }
    }
}