using UnityEngine;

public class GoldBehavior : MonoBehaviour
{
    private Vector3 targetWorldPosition;
    private int goldAmount;
    private float moveSpeed = 5f;
    private bool movingToUI = false;

    private Vector3 startPosition;
    private float floatDuration = 1.5f; // Time spent floating
    private float moveDuration = 1.0f; // Time taken to move to UI
    private float timeElapsed = 0f;

    public void Initialize(Transform uiTarget, int amount)
    {
        // Convert UI position to world space
        targetWorldPosition = Camera.main.ScreenToWorldPoint(uiTarget.position);
        targetWorldPosition.z = 0f; // Ensure it's in 2D space
        goldAmount = amount;

        // Store starting position for floating
        startPosition = transform.position;

        // Start floating phase
        movingToUI = false;
        timeElapsed = 0f;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!movingToUI)
        {
            // Floating effect (up and down bobbing)
            float floatOffset = Mathf.Sin(timeElapsed * Mathf.PI * 2f) * 0.2f; // Sine wave for smooth bobbing
            transform.position = startPosition + new Vector3(0, floatOffset, 0);

            // After floating duration, start moving to UI
            if (timeElapsed >= floatDuration)
            {
                movingToUI = true;
                timeElapsed = 0f; // Reset time for movement phase
                startPosition = transform.position; // Capture current position before moving
            }
        }
        else
        {
            // Move towards UI with EaseOut effect
            float t = TweenUtils.NormalizeTime(timeElapsed, moveDuration);
            float easedT = TweenUtils.EaseOut(t);
            transform.position = Vector3.Lerp(startPosition, targetWorldPosition, easedT);

            // Check if reached UI target
            if (t >= 1f)
            {
                GameManager.Instance.AddGold(goldAmount);
                Destroy(gameObject);
            }
        }
    }
}