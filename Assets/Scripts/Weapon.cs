using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _attackPower;
    [SerializeField] private bool clearHitList;
    private List<Agent> _agentsHit = new();
    private Agent _owner;

    private void OnValidate()
    {
        _owner = GetComponentInParent<Agent>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (clearHitList)
        {
            _agentsHit.Clear();
            clearHitList = false;
        }
        if (collision.gameObject.TryGetComponent<Agent>(out Agent agent))
        {
            if (agent.tag != tag && !_agentsHit.Contains(agent))
            {
                _agentsHit.Add(agent);

                //if not parrying or facing the wrong way
                if (!agent.IsParrying || !agent.IsFacing(_owner))
                {
                    //take damage
                    agent.TakeDamage(_attackPower);
                    return;
                }
                //else reduce stanima
                agent.ReduceStanima(_attackPower);
            }
        }
    }
}
