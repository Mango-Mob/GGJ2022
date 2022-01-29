using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeedManager : Singleton<KillFeedManager>
{
    public GameObject m_killFeedPrefab;
    public Sprite WolfIcon;
    public Sprite PlayerIcon;

    public void DisplayWolfKill(string killer, string victimName)
    {
        GameObject feed = GameObject.Instantiate(m_killFeedPrefab, transform as RectTransform);

        feed.GetComponent<KillFeed>().Killer.text = killer;
        feed.GetComponent<KillFeed>().Victim.text = victimName;
        feed.GetComponent<KillFeed>().WeaponIcon.sprite = WolfIcon;
    }

    public void DisplayPlayerKill(string victimName)
    {
        GameObject feed = GameObject.Instantiate(m_killFeedPrefab, transform as RectTransform);

        feed.GetComponent<KillFeed>().Killer.text = "Player";
        feed.GetComponent<KillFeed>().Victim.text = victimName;
        feed.GetComponent<KillFeed>().WeaponIcon.sprite = PlayerIcon;
    }
}
