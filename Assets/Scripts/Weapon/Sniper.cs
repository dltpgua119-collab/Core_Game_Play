using UnityEngine;
using UnityEngine.UI;

public class Sniper : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float minBulletSpeed = 5f;
    public float maxBulletSpeed = 20f;

    [Header("Charge Settings")]
    public float maxChargeTime = 2f;

    [Header("UI")]
    public Image chargeGaugeImage;

    private bool isCharging = false;
    private float chargeStartTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            chargeStartTime = Time.time;
            if (chargeGaugeImage != null)
                chargeGaugeImage.transform.parent.gameObject.SetActive(true);
        }

        if (isCharging)
{
    float chargeRatio = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
    if (chargeGaugeImage != null)
    {
        chargeGaugeImage.fillAmount = chargeRatio;

        if (chargeRatio >= 1f)
            chargeGaugeImage.color = Color.white;
        else
            chargeGaugeImage.color = new Color(1f, 0.86f, 0f); // 노랑
    }
}

        if (Input.GetMouseButtonUp(0) && isCharging)
{
    float chargeRatio = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
    Shoot(chargeRatio);
    isCharging = false;
    if (chargeGaugeImage != null)
    {
        chargeGaugeImage.fillAmount = 0;
        chargeGaugeImage.color = new Color(1f, 0.86f, 0f); // 노랑으로 초기화
        chargeGaugeImage.transform.parent.gameObject.SetActive(false);
    }
}
    }

    void Shoot(float chargeRatio)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float speed = Mathf.Lerp(minBulletSpeed, maxBulletSpeed, chargeRatio);
            rb.linearVelocity = dir * speed;
        }
    }
}