using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 // Kamera hareketinin yumuşaklığını ayarlar.
    public float m_ScreenEdgeBuffer = 4f;           // Hedeflerin ekrandaki tam sınırlarından ne kadar içeri doğru gideceğini belirler.
    public float m_MinSize = 6.5f;                  // Kamera'nın minimum boyutunu ayarlar.
    [HideInInspector] public Transform[] m_Targets; // Kameranın takip edeceği hedeflerin dizisi.

    private Camera m_Camera;                        // Kamera bileşeni.
    private float m_ZoomSpeed;                      // Yakınlaşma hızı.
    private Vector3 m_MoveVelocity;                 // Hareket vektörü.
    private Vector3 m_DesiredPosition;              // Kameranın ulaşmak istediği hedef pozisyon.

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>(); // Kamera bileşenini alır.
    }

    private void FixedUpdate()
    {
        Move(); // Kamerayı hareket ettirir.
        Zoom(); // Kameranın zoomunu ayarlar.
    }

    private void Move()
    {
        FindAveragePosition(); // Hedeflerin ortalama pozisyonunu bulur.

        // Kamerayı, istenilen pozisyona yumuşak bir şekilde hareket ettirir.
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3(); // Ortalama pozisyonu tutan bir vektör.
        int numTargets = 0; // Hedef sayısı.

        // Tüm hedefleri dolaşır.
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf) // Hedef etkin değilse bir sonrakine geçer.
                continue;

            averagePos += m_Targets[i].position; // Hedefin pozisyonunu ortalamaya ekler.
            numTargets++; // Hedef sayısını artırır.
        }

        if (numTargets > 0)
            averagePos /= numTargets; // Hedeflerin ortalama pozisyonunu bulur.

        averagePos.y = transform.position.y; // Kameranın yüksekliğini korur.

        m_DesiredPosition = averagePos; // İstenilen pozisyonu günceller.
    }

    private void Zoom()
    {
        float requiredSize = FindRequiredSize(); // Gerekli zoom büyüklüğünü bulur.
        // Yakınlaşmayı yumuşak bir şekilde ayarlar.
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // İstenilen pozisyonun yerel pozisyonunu bulur.

        float size = 0f; // Gerekli boyut.

        // Tüm hedefler için gerekli boyutu bulur.
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf) // Hedef etkin değilse bir sonrakine geçer.
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); // Hedefin yerel pozisyonunu bulur.

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; // Hedef ile istenilen pozisyon arasındaki farkı bulur.

            // Y eksenindeki farka göre boyutu ayarlar.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // X eksenindeki farka göre boyutu ayarlar.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer; // Ekran kenar tamponunu ekler.
        size = Mathf.Max(size, m_MinSize); // Minimum boyutu ayarlar.

        return size; // Hesaplanan boyutu döndürür.
    }

    // Kameranın başlangıç pozisyonunu ve boyutunu ayarlar.
    public void SetStartPositionAndSize()
    {
        FindAveragePosition(); // Ortalama pozisyonu bulur.

        transform.position = m_DesiredPosition; // Pozisyonu ayarlar.

        m_Camera.orthographicSize = FindRequiredSize(); // Yakınlaşmayı ayarlar.
    }
}
