using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    // Bu sınıf bir tank üzerindeki çeşitli ayarları yönetmek için kullanılır.
    // GameManager sınıfı ile birlikte çalışarak tankların nasıl davranacağını 
    // ve oyuncuların oyunun farklı aşamalarında tanklarını kontrol edip edemeyeceklerini kontrol eder.

    // Bu tankın renklendirileceği renk.
    public Color m_PlayerColor;                             
    // Tankın doğacağı konum ve yön.
    public Transform m_SpawnPoint;                          
    // Bu yöneticinin hangi oyuncu için olduğunu belirler.
    [HideInInspector] public int m_PlayerNumber;            
    // Tankın rengine uyması için oyuncunun numarasını renklendiren bir metin dizesi.
    [HideInInspector] public string m_ColoredPlayerText;    
    // Tank oluşturulduğunda onun örneğine referans.
    [HideInInspector] public GameObject m_Instance;         
    // Bu oyuncunun şu ana kadar kaç kez kazandığını gösterir.
    [HideInInspector] public int m_Wins;                    

    // Tankın hareket scriptine referans, kontrolü etkinleştirmek ve devre dışı bırakmak için kullanılır.
    private TankMovement m_Movement;                        
    // Tankın ateş etme scriptine referans, kontrolü etkinleştirmek ve devre dışı bırakmak için kullanılır.
    private TankShooting m_Shooting;                        
    // Her raundun başlangıç ve bitiş aşamalarında dünya uzayındaki UI'yı devre dışı bırakmak için kullanılır.
    private GameObject m_CanvasGameObject;                  

    public void Setup ()
    {
        // Bileşenlere referanslar al.
        m_Movement = m_Instance.GetComponent<TankMovement> ();
        m_Shooting = m_Instance.GetComponent<TankShooting> ();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;

        // Oyuncu numaralarını scriptler arasında tutarlı olacak şekilde ayarla.
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        // Tankın rengine ve oyuncunun numarasına dayalı olarak 'OYUNCU 1' vb. yazan doğru rengi kullanarak bir dize oluştur.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // Tankın tüm render'larını al.
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer> ();

        // Tüm render'lar arasında dolaş...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... ve onların malzeme rengini bu tanka özgü renge ayarla.
            renderers[i].material.color = m_PlayerColor;
        }
    }

    // Oyuncunun tankını kontrol edememesi gereken oyun aşamalarında kullanılır.
    public void DisableControl ()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive (false);
    }

    // Oyuncunun tankını kontrol edebilmesi gereken oyun aşamalarında kullanılır.
    public void EnableControl ()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive (true);
    }

    // Her raundun başında tankı varsayılan durumuna getirmek için kullanılır.
    public void Reset ()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive (false);
        m_Instance.SetActive (true);
    }
}
