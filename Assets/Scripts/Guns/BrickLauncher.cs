using UnityEngine;

public class BrickLauncher : MonoBehaviour
{
    [Header("Brick Prefab")]
    public GameObject prefabBrick;

    [Header("발사 설정")]
    public float fireInterval = 2f;

    // 벽돌을 위쪽으로 던지는 힘
    public float upwardPower = 8f;

    // 좌우 방향 힘
    public float horizontalPower = 3f;

    private float fireTimer = 0f;

    public Player master;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;

            Shot(master);
        }
    }

    // 기본 벽돌 발사
    public void Shot(Player master)
    {
        if (prefabBrick == null)
        {
            Debug.LogWarning(
                "[BrickLauncher] Brick Prefab이 설정되지 않았습니다."
            );

            return;
        }

        GameObject copyBrick =
            Instantiate(
                prefabBrick,
                transform.position,
                Quaternion.identity
            );

        Brick brick =
            copyBrick.GetComponent<Brick>();

        if (brick != null)
        {
            brick.master = master;
        }
        else
        {
            Debug.LogWarning(
                "[BrickLauncher] 생성된 벽돌 프리팹에 " +
                "'Brick' 컴포넌트가 없습니다."
            );
        }

        Rigidbody2D rigidbody =
            copyBrick.GetComponent<Rigidbody2D>();

        if (rigidbody != null)
        {
            // 왼쪽 또는 오른쪽으로 랜덤하게 던질 방향
            float randomX =
                Random.Range(-horizontalPower, horizontalPower);

            // 위쪽으로 던지는 힘
            Vector2 force =
                new Vector2(randomX, upwardPower);

            rigidbody.AddForce(
                force,
                ForceMode2D.Impulse
            );

            // 회전 효과
            float randomRotation =
                Random.Range(-360f, 360f);

            rigidbody.AddTorque(
                randomRotation,
                ForceMode2D.Impulse
            );
        }
        else
        {
            Debug.LogWarning(
                "[BrickLauncher] 생성된 벽돌 프리팹에 " +
                "'Rigidbody2D' 컴포넌트가 없습니다."
            );
        }
    }
}