using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    // Bu genel değişken, göreli rotasyonu kullanıp kullanmamayı seçmenizi sağlar
    public bool m_UseRelativeRotation = true;

    // Bu özel değişken, ebeveyn nesnenin başlangıçtaki göreli rotasyonunu saklayacak
    private Quaternion m_RelativeRotation;

    // Start metodu, herhangi bir Update metodundan önce ilk kez çağrılır
    private void Start()
    {
        // Ebeveyn nesnenin başlangıçtaki yerel rotasyonunu sakla
        m_RelativeRotation = transform.parent.localRotation;
    }

    // Update metodu her karede bir kez çağrılır
    private void Update()
    {
        // Eğer m_UseRelativeRotation doğruysa, bu transformun rotasyonunu saklanan göreli rotasyona ayarla
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}