using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    // Farklı oyuncuları tanımlamak için kullanılır
    public int m_PlayerNumber = 1;
    // Mermi prefabı
    public Rigidbody m_Shell;
    // Mermilerin fırlatıldığı tankın çocuğu
    public Transform m_FireTransform;
    // Mevcut fırlatma kuvvetini gösteren tankın çocuğu
    public Slider m_AimSlider;
    // Ateş etme sesini çalmak için kullanılan ses kaynağı. Hareket ses kaynağından farklı.
    public AudioSource m_ShootingAudio;
    // Her atış şarj olurken çalan ses
    public AudioClip m_ChargingClip;
    // Her atış yapıldığında çalan ses
    public AudioClip m_FireClip;
    // Ateş düğmesi basılmadığında mermiye verilen kuvvet
    public float m_MinLaunchForce = 15f;
    // Ateş düğmesi maksimum şarj süresi boyunca basılı tutulduğunda mermiye verilen kuvvet
    public float m_MaxLaunchForce = 30f;
    // Merminin maksimum kuvvetle fırlatılmadan önce ne kadar süre şarj olabileceği
    public float m_MaxChargeTime = 0.75f;

    // Mermileri fırlatmak için kullanılan giriş ekseni
    private string m_FireButton;
    // Ateş düğmesi bırakıldığında mermiye verilecek kuvvet
    private float m_CurrentLaunchForce;
    // Fırlatma kuvvetinin artış hızı, maksimum şarj süresine bağlı olarak
    private float m_ChargeSpeed;
    // Bu düğme basımıyla merminin fırlatılıp fırlatılmadığını belirten bayrak
    private bool m_Fired;

    // Tank etkinleştirildiğinde çağrılır
    private void OnEnable()
    {
        // Tank açıldığında fırlatma kuvvetini ve UI'ı sıfırla
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    // Oyun başladığında bir kez çağrılır
    private void Start()
    {
        // Ateş ekseni oyuncu numarasına göre belirlenir
        m_FireButton = "Fire" + m_PlayerNumber;

        // Fırlatma kuvvetinin artış hızı, maksimum şarj süresine bağlı olarak hesaplanır
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    // Her karede bir kez çağrılır
    private void Update()
    {
        // Slider, varsayılan olarak minimum fırlatma kuvvetine sahip olmalıdır
        m_AimSlider.value = m_MinLaunchForce;

        // Eğer maksimum kuvvet aşılmışsa ve mermi henüz fırlatılmamışsa...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... maksimum kuvveti kullan ve mermiyi fırlat
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // Aksi takdirde, ateş düğmesine yeni basılmışsa...
        else if (Input.GetButtonDown(m_FireButton))
        {
            // ... ateş bayrağını sıfırla ve fırlatma kuvvetini sıfırla
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // Sesi şarj sesine değiştir ve çalmaya başla
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // Aksi takdirde, ateş düğmesi basılı tutuluyorsa ve mermi henüz fırlatılmamışsa...
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Fırlatma kuvvetini artır ve slider'ı güncelle
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Aksi takdirde, ateş düğmesi bırakılmışsa ve mermi henüz fırlatılmamışsa...
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // ... mermiyi fırlat
            Fire();
        }
    }

    // Mermiyi fırlatan metod
    private void Fire()
    {
        // Ateş bayrağını ayarla, böylece Fire metodu sadece bir kez çağrılır
        m_Fired = true;

        // Mermi örneğini oluştur ve Rigidbody referansını sakla
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Merminin hızını, fırlatma pozisyonunun ileri yönünde fırlatma kuvvetiyle ayarla
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // Sesi ateş sesine değiştir ve çalmaya başla
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Fırlatma kuvvetini sıfırla. Bu, düğme olaylarının kaçırılması durumunda bir önlemdir
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}
