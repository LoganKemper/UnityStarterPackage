// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Used to add UnityEvents to collectable count thresholds.
    /// </summary>
    public class CollectableEvents : MonoBehaviour
    {
        public enum Condition : byte
        {
            None,
            EqualTo,
            NotEqualTo,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo
        }

        [Header("Collectable Conditions")]
        [Tooltip("Enter the target collectable count for the condition.")]
        [SerializeField] private int count;

        [Tooltip("Choose the condition that will be evaluated.")]
        [SerializeField] private Condition condition = Condition.EqualTo;

        [Tooltip("If true, this condition can only be cleared once.")]
        [SerializeField] private bool singleUse = false;

        [Space(20)]
        [SerializeField] private UnityEvent onConditionCleared, onConditionFailed;

        private bool conditionUsed = false;

        // Call this method from the CollectableManager
        public void CheckCondition(int count)
        {
            // If this condition is set to single-use and has been used, cancel the check
            if (singleUse && conditionUsed)
            {
                return;
            }

            switch (condition)
            {
                default:
                case Condition.None:
                    // No condition set, cleared
                    ConditionCleared();
                    break;
                case Condition.EqualTo:
                    if (count == this.count)
                    {
                        // Count is equal to the target count, cleared
                        ConditionCleared();
                        return;
                    }
                    break;
                case Condition.NotEqualTo:
                    if (count != this.count)
                    {
                        // Count is not equal to the target count, cleared
                        ConditionCleared();
                        return;
                    }
                    break;
                case Condition.LessThanOrEqualTo:
                    if (count <= this.count)
                    {
                        // Count is less than or equal to the target count, cleared
                        ConditionCleared();
                        return;
                    }
                    break;
                case Condition.GreaterThanOrEqualTo:
                    if (count >= this.count)
                    {
                        // Count is greater than or equal to the target count, cleared
                        ConditionCleared();
                        return;
                    }
                    break;
            }

            // Condition has not been met, fail
            onConditionFailed.Invoke();
        }

        public void ConditionCleared()
        {
            conditionUsed = true;
            onConditionCleared.Invoke();
        }

        public void SetSingleUse(bool singleUse)
        {
            this.singleUse = singleUse;
        }
    }
}