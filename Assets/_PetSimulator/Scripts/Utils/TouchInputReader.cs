using UnityEngine;

public class TouchInputReader : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float swipeSensitivity = 1.5f;   // Control how strong the swipe is
    public float maxSwipeDelta = 300f;      // Max swipe distance to normalize

    private Vector2 _startPos;
    private float _horizontal;

    public float Horizontal => _horizontal;

    private void Update()
    {
        _horizontal = 0f;

#if UNITY_EDITOR
        // Mouse input for testing in Game view
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            float delta = Input.mousePosition.x - _startPos.x;
            delta = Mathf.Clamp(delta, -maxSwipeDelta, maxSwipeDelta);
            _horizontal = Mathf.Clamp(delta / maxSwipeDelta * swipeSensitivity, -1f, 1f);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _horizontal = 0f;
        }
#else
        // Touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    float delta = touch.position.x - _startPos.x;
                    delta = Mathf.Clamp(delta, -maxSwipeDelta, maxSwipeDelta);
                    _horizontal = Mathf.Clamp(delta / maxSwipeDelta * swipeSensitivity, -1f, 1f);
                    break;

                case TouchPhase.Ended:
                    _horizontal = 0f;
                    break;
            }
        }
#endif
    }
}
