using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KillFeedManager : Singleton<KillFeedManager>
{
    public int MessageLimit = 5;

    public GameObject m_killFeedPrefab;
    public Sprite WolfIcon;
    public Sprite PlayerIcon;

    public void DisplayWolfKill(string killer, string victimName)
    {
        GameObject feed = GameObject.Instantiate(m_killFeedPrefab, transform as RectTransform);

        feed.GetComponent<KillFeed>().Killer.text = killer;
        feed.GetComponent<KillFeed>().Victim.text = victimName;
        feed.GetComponent<KillFeed>().WeaponIcon.sprite = WolfIcon;

        KillFeed[] feedList = GetComponentsInChildren<KillFeed>().ToArray();
        if(feedList.Length > MessageLimit)
        {
            Destroy(feedList[0].gameObject);
        }
    }

    public void DisplayPlayerKill(string victimName)
    {
        GameObject feed = GameObject.Instantiate(m_killFeedPrefab, transform as RectTransform);

        feed.GetComponent<KillFeed>().Killer.text = "Player";
        feed.GetComponent<KillFeed>().Victim.text = victimName;
        feed.GetComponent<KillFeed>().WeaponIcon.sprite = PlayerIcon;

        KillFeed[] feedList = GetComponentsInChildren<KillFeed>().ToArray();
        if (feedList.Length > MessageLimit)
        {
            Destroy(feedList[0].gameObject);
        }
    }
}
