using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

public static class AIUtilityFunctions
{
    public static bool IsNearDestination(this NavMeshAgent agent, float range = 1.0f)
    {
        return Vector3.Distance(agent.transform.position, agent.destination) < range;
    }

    public static void SetDestinationNear(this NavMeshAgent agent, Vector3 dest, float minDist = 0.0f, float maxDist = 0.0f)
    {
        Vector3 direction = dest - agent.transform.position;

        float distance = direction.magnitude;
        direction = direction.normalized;

        if(distance < minDist)
        {
            //Move away from target;
            dest += direction * (minDist - distance);
            agent.SetDestination(dest);
        }
        else if(distance > maxDist)
        {
            //Move to target;
            dest -= direction * maxDist;
            agent.SetDestination(dest);
        }
        else //Within Range
        {
            //Do nothing
        }
    }

    public static T GetClosest<T>(this MonoBehaviour mono, List<T> _list) where T : MonoBehaviour
    {
        T closest = null;

        float dist = float.MaxValue;
        foreach (var item in _list)
        {
            float newDist = Vector3.Distance(item.transform.position, mono.transform.position);
            if (newDist < dist)
            {
                closest = item;
                dist = newDist;
            }
        }

        return closest;
    }
}
