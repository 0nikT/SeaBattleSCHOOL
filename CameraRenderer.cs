using UnityEngine;

namespace InfiniteVox
{
    public class CameraRenderer : MonoBehaviour
    {
        [SerializeField] private Vector2 _defaultResolution = new Vector2(1920, 1080);
        [SerializeField, Range(0f, 1f)] private float _widthOrHeight;

        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();

            if (GameSettings.Settings.GetData(false) == "tablet")
                _widthOrHeight = 0f;

            float targetAspect = _defaultResolution.x / _defaultResolution.y;

            float initialFov = _camera.fieldOfView;
            float horizontalFov = CalcVerticalFov(initialFov, 1f / targetAspect);

            float constantWidthFov = CalcVerticalFov(horizontalFov, _camera.aspect);
            _camera.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, _widthOrHeight);
        }

        private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2f) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}