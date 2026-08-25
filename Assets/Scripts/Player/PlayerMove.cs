using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // 이동
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.Translate(new Vector3(x, y, 0).normalized * speed * Time.deltaTime);

        // flip
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<SpriteRenderer>().flipX = mousePos.x < transform.position.x;
    }
}