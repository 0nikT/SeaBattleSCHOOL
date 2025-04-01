using UnityEngine;

namespace InfiniteVox
{
    public class SafeAreaFilter : MonoBehaviour
    {
        private void Awake()
        {
            if (GameSettings.Settings.GetData(false) == "desktop")
                return;

            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}