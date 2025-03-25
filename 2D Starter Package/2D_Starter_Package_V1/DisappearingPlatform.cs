using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach to a platform and use the UnityEvents to disable its collider or whatever else needed to make the platform disappear.
/// </summary>
public class DisappearingPlatform : MonoBehaviour
{
    [Tooltip("Enter the tag name that should register collisions. Leave blank for any tag to be used.")]
    [SerializeField] private string tagName;

    [Tooltip("The time in seconds from when a collision is detected until onDisappear is invoked.")]
    [SerializeField] private float timeUntilDisappear = 2f;

    [Tooltip("The time in seconds from when onDisappear is invoked until onReappear is invoked. Leave at 0 for onReappear to never occur.")]
    [SerializeField] private float timeUntilReappear = 0f;

    [Space(20)]
    [SerializeField] private UnityEvent onDisappear, onReappear;

    private bool isDisappearing = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (string.IsNullOrEmpty(tagName) || collision.collider.CompareTag(tagName))
        {
            if (!isDisappearing)
            {
                StartCoroutine(DisappearCoroutine());
                isDisappearing = true;
            }
        }
    }

    private IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(timeUntilDisappear);
        onDisappear.Invoke();

        if (timeUntilReappear > 0)
        {
            yield return new WaitForSeconds(timeUntilReappear);
            onReappear.Invoke();
        }

        isDisappearing = false;
    }

    // Force timeUntilDisappear and timeUntilReappear to be 0 or greater
    private void OnValidate()
    {
        if (timeUntilDisappear < 0f)
        {
            timeUntilDisappear = 0;
        }

        if (timeUntilReappear < 0f)
        {
            timeUntilReappear = 0;
        }
    }
}
