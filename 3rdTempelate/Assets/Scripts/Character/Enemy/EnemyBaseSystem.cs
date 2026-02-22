using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBaseSystem : MonoBehaviour
{
    [Header("生命值")]
    [SerializeField] private int maxHealth = 100;      // 最大生命值
    [SerializeField] private int currentHealth;        // 当前生命值

    [Header("受击效果")]
    [SerializeField] private bool flashOnHit = true;  // 受击是否闪烁
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color hitColor = Color.red;

    [Header("UI设置")]
    [SerializeField] private Slider healthSlider;

    private Renderer enemyRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
            propertyBlock = new MaterialPropertyBlock();
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        OnHit();
    }

    private void OnHit()
    {
        if (flashOnHit && enemyRenderer != null)
        {
            StartCoroutine(HitFlash());
        }

    }
 
    private System.Collections.IEnumerator HitFlash()
    {
        propertyBlock.SetColor("_BaseColor", hitColor);
        enemyRenderer.SetPropertyBlock(propertyBlock);

        yield return new WaitForSeconds(flashDuration);

        propertyBlock.SetColor("_BaseColor", originalColor);
        enemyRenderer.SetPropertyBlock(propertyBlock);
    }
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = GetHealthPercentage();
        }
    }
}