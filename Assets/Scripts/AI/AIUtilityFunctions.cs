using UnityEngine.AI;
using UnityEngine;

public static class AIUtilityFunctions
{
    public static bool IsNearDestination(this NavMeshAgent agent, float range = 1.0f)
    {
        return Vector3.Distance(agent.transform.position, agent.destination) < range;
    }
}
