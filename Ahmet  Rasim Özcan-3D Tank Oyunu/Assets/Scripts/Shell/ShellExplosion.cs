using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    // Patlamanın etkilediği katmanları filtrelemek için kullanılır, bu "Players" olarak ayarlanmalıdır.
    public LayerMask m_TankMask;
    // Patlamada oynayacak partiküllerin referansı.
    public ParticleSystem m_ExplosionParticles;
    // Patlamada çalacak sesin referansı.
    public AudioSource m_ExplosionAudio;
    // Patlama tankın merkezine denk geldiğinde verilen hasar miktarı.
    public float m_MaxDamage = 100f;
    // Patlamanın merkezine eklenen kuvvet miktarı.
    public float m_ExplosionForce = 1000f;
    // Mermi kaldırılmadan önce geçen süre (saniye cinsinden).
    public float m_MaxLifeTime = 2f;
    // Patlama tanklardan en fazla ne kadar uzaklıkta olabilir ve hala etkilenebilirler.
    public float m_ExplosionRadius = 5f;

    private void Start()
    {
        // Belirtilen ömür süresi dolduğunda mermiyi yok et.
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Merminin mevcut pozisyonundan patlama yarıçapına kadar olan bir küredeki tüm çarpışma nesnelerini topla.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        // Tüm çarpışma nesneleri arasında dolaş...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... ve onların rigidbody'lerini bul.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // Eğer bir rigidbody yoksa, bir sonraki çarpışma nesnesine geç.
            if (!targetRigidbody)
                continue;

            // Bir patlama kuvveti ekle.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Rigidbody ile ilişkilendirilmiş TankHealth script'ini bul.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // Eğer gameobject'e bağlı bir TankHealth script'i yoksa, bir sonraki çarpışma nesnesine geç.
            if (!targetHealth)
                continue;

            // Mermiden hedefe olan mesafeye göre hedefin alması gereken hasar miktarını hesapla.
            float damage = CalculateDamage(targetRigidbody.position);

            // Bu hasarı tanka ver.
            targetHealth.TakeDamage(damage);
        }

        // Partikülleri mermiden ayır.
        m_ExplosionParticles.transform.parent = null;

        // Partikül sistemini oynat.
        m_ExplosionParticles.Play();

        // Patlama ses efektini çal.
        m_ExplosionAudio.Play();

        // Partiküller bittikten sonra, bulundukları gameobject'i yok et.
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Mermiyi yok et.
        Destroy(gameObject);
    }

    // Hedef pozisyona göre hasarı hesaplayan metod.
    private float CalculateDamage(Vector3 targetPosition)
    {
        // Mermiden hedefe bir vektör oluştur.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Mermiden hedefe olan mesafeyi hesapla.
        float explosionDistance = explosionToTarget.magnitude;

        // Hedefin patlama yarıçapına göre maksimum mesafeye oranını hesapla.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Hasarı bu oranla maksimum olası hasarın bir kısmı olarak hesapla.
        float damage = relativeDistance * m_MaxDamage;

        // Minimum hasarın her zaman 0 olduğundan emin ol.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}
