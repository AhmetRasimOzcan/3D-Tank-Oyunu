using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    // Tankın başlangıç sağlığı
    public float m_StartingHealth = 100f;          
    // Sağlık göstergesi için slider
    public Slider m_Slider;                        
    // Sağlık göstergesi dolgu resmi
    public Image m_FillImage;                      
    // Tam sağlık rengini belirten renk
    public Color m_FullHealthColor = Color.green;  
    // Sıfır sağlık rengini belirten renk
    public Color m_ZeroHealthColor = Color.red;    
    // Patlama efekti prefabı
    public GameObject m_ExplosionPrefab;

    // Patlama sesi için AudioSource bileşeni
    private AudioSource m_ExplosionAudio;          
    // Patlama parçacık sistemi
    private ParticleSystem m_ExplosionParticles;   
    // Mevcut sağlık değeri
    private float m_CurrentHealth;  
    // Tankın ölü olup olmadığını belirten bayrak
    private bool m_Dead;            

    // Awake metodu, nesne oluşturulduğunda çağrılır
    private void Awake()
    {
        // Patlama prefabını oluştur ve parçacık sistemi bileşenini al
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // Patlama sesi bileşenini al
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Patlama efektini başlangıçta devre dışı bırak
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    // OnEnable metodu, nesne etkinleştiğinde çağrılır
    private void OnEnable()
    {
        // Sağlık ve ölü bayrağını başlangıç değerlerine ayarla
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Sağlık UI'ını güncelle
        SetHealthUI();
    }
    
    // Tankın hasar almasını sağlayan metod
    public void TakeDamage(float amount)
    {
        // Tankın mevcut sağlığını ayarla, UI'ı yeni sağlık durumuna göre güncelle ve tankın ölü olup olmadığını kontrol et
        m_CurrentHealth -= amount;
        SetHealthUI();

        // Eğer sağlık sıfır veya altına düşerse ve tank henüz ölmediyse, OnDeath metodunu çağır
        if(m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    // Sağlık UI'ını ayarlayan metod
    private void SetHealthUI()
    {
        // Slider değerini ve rengini ayarla
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    // Tankın ölümünü işleyen metod
    private void OnDeath()
    {
        // Tankın ölüm efektlerini oynat ve tankı devre dışı bırak
        m_Dead = true;
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}