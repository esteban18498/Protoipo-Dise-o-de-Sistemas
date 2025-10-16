using UnityEngine;
using UnityEngine.InputSystem;

public class AudioTest : MonoBehaviour
{
    public AudioCue mainMenu;
    public AudioCue quack;

    void Start()
    {
        AudioManager.Instance.PlayMusic(mainMenu, loop: true);
    }

    void Update()
    {
        // Space as a quick test
        if (Keyboard.current != null && Keyboard.current.numpad9Key.wasPressedThisFrame)
        {
            // play at listener/camera position to avoid 3D distance issues
            var pos = Camera.main ? Camera.main.transform.position : (Vector3?)null;
            AudioManager.Instance.PlaySfx(quack, pos);
        }
    }
}
