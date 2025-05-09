using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifeTime = 1.2f;
    public float growTime = 0.3f;
    public float maxScale = 1.5f;
    public float floatUpSpeed = 0.5f;

    private float elapsed = 0f;
    private DamageLayer owner;

    public void Init(float damage, DamageLayer owner)
    {
        this.owner = owner;
        text.text = damage.ToString();
        transform.localScale = Vector3.zero;
        elapsed = 0f;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;

        if (elapsed < growTime)
        {
            float t = elapsed / growTime;
            float scale = Mathf.Lerp(0f, maxScale, Mathf.Sin(t * Mathf.PI * 0.5f));
            transform.localScale = Vector3.one * scale;
        }
        else if (elapsed < lifeTime)
        {
            float t = (elapsed - growTime) / (lifeTime - growTime);
            float scale = Mathf.Lerp(maxScale, 0f, t * t);
            transform.localScale = Vector3.one * scale;
        }
        else
        {
            owner.ReturnToPool(this);
        }
    }
}
