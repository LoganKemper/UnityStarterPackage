// Unity Starter Package - Version 2
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
        [System.Serializable]
        public class HealthStage
        {
            [Tooltip("Enter the health value at which the stage will be activated.")]
            public int healthThreshold;

            [Tooltip("Enable to prevent the health stage from being activated multiple times.")]
            public bool singleUse = true;

            [System.NonSerialized] public bool stageUsed;

            public UnityEvent onStageReached;
        }

        [SerializeField] private HealthStage[] healthStages;

        private EnemyHealth3D enemyHealth;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth3D>();
        }

        private void OnEnable()
        {
            enemyHealth.OnEnemyHealthChanged += EnemyHealthChanged;
        }

        private void OnDisable()
        {
            enemyHealth.OnEnemyHealthChanged -= EnemyHealthChanged;
        }

        private void EnemyHealthChanged(int oldHealth, int newHealth)
        {
            foreach (HealthStage stage in healthStages)
            {
                if (!stage.singleUse || !stage.stageUsed)
                {
                    if (oldHealth > stage.healthThreshold && newHealth <= stage.healthThreshold)
                    {
                        stage.stageUsed = true;
                        stage.onStageReached.Invoke();
                    }
                }
            }
        }
    }
}