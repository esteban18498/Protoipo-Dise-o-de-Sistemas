using UnityEngine;

[DefaultExecutionOrder(100)] // run after characters move
[RequireComponent(typeof(Camera))]
public class FightCamera : MonoBehaviour
{
    [Header("Targets (use the players' Anchor transforms)")]
    public Transform targetA;
    public Transform targetB;

    [Header("Follow")]
    public Vector2 frameOffset = Vector2.zero; // nudge framing if needed
    public float zOffset = -10f;
    public float followSmooth = 8f; // higher = snappier

    [Header("Zoom (FOV)")]
    public float minFOV = 60f;     // never smaller than this
    public float maxFOV = 100f;    // max zoom-out when far apart
    public float minDistance = 2f; // world units for minFOV (≈ 2 tiles if 1 empty tile between)
    public float maxDistance = 12f;// world units for maxFOV (adjust to your arena)
    public float zoomSmooth = 8f;

    [Header("Optional clamps")]
    public bool clampX;
    public Vector2 xLimits = new Vector2(-999, 999);
    public bool clampY;
    public Vector2 yLimits = new Vector2(-999, 999);

    Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    void LateUpdate()
    {
        if (!targetA && !targetB) return;

        // Fallback if one target is missing
        Vector3 a = targetA ? targetA.position : targetB.position;
        Vector3 b = targetB ? targetB.position : targetA.position;

        // --- Center between anchors ---
        Vector3 mid = (a + b) * 0.5f;
        Vector3 desired = new Vector3(
            mid.x + frameOffset.x,
            mid.y + frameOffset.y,
            zOffset
        );

        if (clampX) desired.x = Mathf.Clamp(desired.x, xLimits.x, xLimits.y);
        if (clampY) desired.y = Mathf.Clamp(desired.y, yLimits.x, yLimits.y);

        transform.position = Vector3.Lerp(
            transform.position, desired, Time.deltaTime * Mathf.Max(0.0001f, followSmooth));

        // --- Zoom by distance → FOV ---
        float dist = Vector3.Distance(a, b);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist); // 0 at min, 1 at max
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSmooth);
    }

    void OnDrawGizmosSelected()
    {
        if (targetA && targetB)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(targetA.position, targetB.position);
        }
    }
}
