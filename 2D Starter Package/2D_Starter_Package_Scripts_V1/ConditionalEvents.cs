// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Used to add UnityEvents to specific count conditions.
    /// </summary>
    public class ConditionalEvents : MonoBehaviour
    {
        public enum Condition : byte
        {
            None,
            EqualTo,
            NotEqualTo,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo
        }

        [System.Serializable]
        public class CounterCondition
        {
            [Tooltip("Enter the value that will be evaluated.")]
            public int targetCount;

            [Tooltip("Choose the condition that will be used to evaluate the target count.")]
            public Condition condition = Condition.EqualTo;

            [Tooltip("Enable to prevent the condition from being activated multiple times.")]
            public bool singleUse = true;

            [System.NonSerialized] public bool conditionUsed = false;

            [Space(20)]
            public UnityEvent onConditionMet;
        }

        [Tooltip("The current value of the counter.")]
        [SerializeField] private int count;

        [Tooltip("One or more conditions to evaluate whenever the count changes.")]
        [SerializeField] private CounterCondition[] counterConditions;

        [Tooltip("If enabled, all conditions will be evaluated once on Start using the initial count.")]
        [SerializeField] private bool evaluateOnStart = true;

        [Tooltip("If disabled, the conditions will NOT be evaluated when this component is inactive.")]
        [SerializeField] private bool evaluateWhenInactive = true;

        private void Start()
        {
            if (evaluateOnStart)
            {
                EvaluateAll();
            }
        }

        public void Add(int addition)
        {
            count += addition;
            CountChanged();
        }

        public void Subtract(int subtraction)
        {
            count -= subtraction;
            CountChanged();
        }

        public void SetCount(int newCount)
        {
            count = newCount;
            CountChanged();
        }

        public void EvaluateAll()
        {
            CountChanged();
        }

        public void ResetSingleUseFlags()
        {
            if (counterConditions == null)
            {
                return;
            }

            foreach (var condition in counterConditions)
            {
                if (condition != null)
                {
                    condition.conditionUsed = false;
                }
            }
        }

        private void CountChanged()
        {
            if (counterConditions == null)
            {
                return;
            }

            if (!evaluateWhenInactive && !isActiveAndEnabled)
            {
                return;
            }

            for (int i = 0; i < counterConditions.Length; i++)
            {
                var condition = counterConditions[i];
                if (condition == null)
                {
                    continue;
                }

                // Skip if it already fired and is single-use
                if (condition.singleUse && condition.conditionUsed)
                {
                    continue;
                }

                if (IsConditionMet(count, condition.condition, condition.targetCount))
                {
                    if (condition.singleUse)
                    {
                        condition.conditionUsed = true;
                    }

                    condition.onConditionMet.Invoke();
                }
            }
        }

        private bool IsConditionMet(int value, Condition condition, int target)
        {
            return condition switch
            {
                Condition.EqualTo => value == target,
                Condition.NotEqualTo => value != target,
                Condition.LessThanOrEqualTo => value <= target,
                Condition.GreaterThanOrEqualTo => value >= target,
                _ => false,
            };
        }

        #region Context Menu Debug Helpers
        [ContextMenu("Add 1")]
        private void Debug_AddOne() => Add(1);

        [ContextMenu("Subtract 1")]
        private void Debug_SubtractOne() => Subtract(1);

        [ContextMenu("Evaluate All")]
        private void Debug_EvaluateAll() => EvaluateAll();

        [ContextMenu("Reset Single Use Flags")]
        private void Debug_ResetSingleUse() => ResetSingleUseFlags();
        #endregion

    }
}