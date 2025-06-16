using UnityEngine;
using UnityEngine.AI;

public class MoveTEst : MonoBehaviour
{
    public Transform target; // 拖曳玩家物件到這

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}
