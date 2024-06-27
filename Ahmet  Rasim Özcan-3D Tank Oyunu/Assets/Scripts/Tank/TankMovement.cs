using UnityEngine;

public class TankMovement : MonoBehaviour
{
    // Oyuncu numarası (kontroller için)
    public int m_PlayerNumber = 1;         
    // Tankın hareket hızı
    public float m_Speed = 12f;            
    // Tankın dönüş hızı
    public float m_TurnSpeed = 180f;       
    // Hareket sesi için AudioSource bileşeni
    public AudioSource m_MovementAudio;    
    // Motorun rölanti sesi
    public AudioClip m_EngineIdling;       
    // Motorun sürüş sesi
    public AudioClip m_EngineDriving;      
    // Sesin frekans aralığı
    public float m_PitchRange = 0.2f;

    // Hareket ekseni adı
    private string m_MovementAxisName;     
    // Dönüş ekseni adı
    private string m_TurnAxisName;         
    // Rigidbody bileşeni
    private Rigidbody m_Rigidbody;         
    // Hareket giriş değeri
    private float m_MovementInputValue;    
    // Dönüş giriş değeri
    private float m_TurnInputValue;        
    // Orijinal ses perdesi
    private float m_OriginalPitch;         

    // Awake metodu, nesne oluşturulduğunda çağrılır
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // OnEnable metodu, nesne etkinleştiğinde çağrılır
    private void OnEnable ()
    {
        // Rigidbody bileşenini aktif hale getir
        m_Rigidbody.isKinematic = false;
        // Hareket ve dönüş giriş değerlerini sıfırla
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    // OnDisable metodu, nesne devre dışı bırakıldığında çağrılır
    private void OnDisable ()
    {
        // Rigidbody bileşenini kinematik moda ayarla
        m_Rigidbody.isKinematic = true;
    }

    // Start metodu, oyun başladığında bir kez çağrılır
    private void Start()
    {
        // Hareket ve dönüş ekseni adlarını belirle
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        // Orijinal ses perdesini sakla
        m_OriginalPitch = m_MovementAudio.pitch;
    }
    
    // Update metodu her karede bir kez çağrılır
    private void Update()
    {
        // Oyuncu girişlerini sakla ve motor sesinin çaldığından emin ol
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        EngineAudio();
    }

    // Motor sesini kontrol eden metod
    private void EngineAudio()
    {
        // Eğer giriş yoksa (tank duruyorsa)...
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // ... ve eğer ses kaynağı şu anda sürüş klibini çalıyorsa...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... klibi rölantiye değiştir ve çal
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Aksi takdirde tank hareket ediyorsa ve şu anda rölanti klibi çalıyorsa...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... klibi sürüşe değiştir ve çal
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    // FixedUpdate metodu, belirli aralıklarla fizik hesaplamaları için çağrılır
    private void FixedUpdate()
    {
        // Tankı hareket ettir ve döndür
        Move();
        Turn();
    }

    // Tankı hareket ettiren metod
    private void Move()
    {
        // Oyuncu girişine göre tankın pozisyonunu ayarla
        // Tankın baktığı yönde, giriş, hız ve kareler arası zaman temelinde bir vektör oluştur
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Bu hareketi rigidbody'nin pozisyonuna uygula
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    // Tankı döndüren metod
    private void Turn()
    {
        // Oyuncu girişine göre tankın rotasyonunu ayarla
        // Giriş, hız ve kareler arası zamana göre dönecek dereceyi belirle
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Bu dönüşü y ekseninde bir rotasyona çevir
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Bu rotasyonu rigidbody'nin rotasyonuna uygula
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
