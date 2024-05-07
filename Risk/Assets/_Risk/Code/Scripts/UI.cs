using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Camera thumbnailCamera;
    private readonly int width = 256;
    private readonly int height = 256;

    void Start()
    {
        // GameObject instance = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        // thumbnailCamera.gameObject.SetActive(true);
        // RenderTexture renderTexture = new RenderTexture(width, height, 24);
        // thumbnailCamera.targetTexture = renderTexture;
        // thumbnailCamera.Render();

        // RenderTexture.active = renderTexture;
        // Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        // texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        // texture2D.Apply();

        // byte[] imageBytes = texture2D.EncodeToPNG();
        // File.WriteAllBytes(Application.dataPath + "/Thumbnail.png", imageBytes);

        // DestroyImmediate(texture2D);
        // thumbnailCamera.gameObject.SetActive(false);
        // Destroy(instance);
    }
}
