using UnityEngine;

/// <summary>
/// Locks the camera's rendered area to a fixed aspect ratio (default 16:9),
/// adding black bars (letterbox/pillarbox) when the window doesn't match.
/// Put this on the camera that has the CinemachineBrain.
/// </summary>
[RequireComponent(typeof(Camera))]
public class AspectRatioEnforcer : MonoBehaviour
{
    [SerializeField] private float targetWidth = 16f;
    [SerializeField] private float targetHeight = 9f;

    private Camera _camera;
    private int _lastWidth;
    private int _lastHeight;

    private void Awake() => _camera = GetComponent<Camera>();

    private void Update()
    {
        if (Screen.width == _lastWidth && Screen.height == _lastHeight)
            return;

        Apply();
    }

    private void Apply()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = _camera.rect;

        if (scaleHeight < 1f)
        {
            // Janela mais alta que 16:9 → barras em cima/embaixo (letterbox).
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            // Janela mais larga que 16:9 → barras nas laterais (pillarbox).
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
        }

        _camera.rect = rect;
    }
}
