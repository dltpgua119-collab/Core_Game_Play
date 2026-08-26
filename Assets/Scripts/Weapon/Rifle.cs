// Rifle.cs
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.3f;
    public float bulletSpeed = 10f;  // 추가
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));

        Physics2D.IgnoreCollision(
            bullet.GetComponent<Collider2D>(),
            GetComponent<Collider2D>()
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;  // 수정
    }
}