// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Attach to an enemy to add UnityEvents to stages of their health.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth3D))]
    public class EnemyHealthStages3D : MonoBehaviour
    {
        [SerializeField] private HealthStage[] healthStages;

        private EnemyHealth3D enemyHealth;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth3D>();
        }

        private void OnEnable()
        {
            enemyHealth.OnEnemyLostHealth += EnemyLostHealth;
        }

        private void OnDisable()
        {
            enemyHealth.OnEnemyLostHealth -= EnemyLostHealth;
        }

        private void EnemyLostHealth(int newHealth)
        {
            foreach (HealthStage stage in healthStages)
            {
                if (stage.healthThreshold == newHealth && (!stage.singleUse || !stage.stageUsed))
                {
                    stage.stageUsed = true;
                    stage.onStageReached.Invoke();
                }
            }
        }

        [System.Serializable]
        public class HealthStage
        {
            [Tooltip("Enter the health value at which the stage will be activated.")]
            public int healthThreshold;

            [Tooltip("Enable to prevent the health stage from being activated multiple times.")]
            public bool singleUse = true;

            [System.NonSerialized] public bool stageUsed = false;
            public UnityEvent onStageReached;
        }
    }
}
