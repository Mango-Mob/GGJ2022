using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BulletCount : MonoBehaviour
{
    public GameObject m_bulletPrefab;
    private List<Image> m_bullets = new List<Image>();

    private int m_ammoCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetAmmoCount(int _ammoCount)
    {
        if (m_ammoCount != _ammoCount)
        {
            m_ammoCount = _ammoCount;
            foreach (var bullet in m_bullets)
            {
                if (m_bullets.IndexOf(bullet) >= _ammoCount)
                {
                    bullet.enabled = false;
                }
            }
        }
    }
    public void SetupMaxAmmo(int _maxAmmo)
    {
        if (m_bullets.Count > 0)
        {
            foreach (var bullet in m_bullets)
            {
                Destroy(bullet.gameObject);
            }

            m_bullets.Clear();
        }

        for (int i = 0; i < _maxAmmo; i++)
        {
            GameObject newBullet = Instantiate(m_bulletPrefab, transform);
            m_bullets.Add(newBullet.GetComponent<Image>());
        }

        m_ammoCount = _maxAmmo;
    }
}
