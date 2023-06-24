using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarUI : MonoBehaviour
{
    public RectTransform radarUIObject;
    float radius;
    public float radarRange = 30f;

    [Header("Markers")]
    public GameObject enemyMarker;

    [System.Serializable]
    public struct Marker
    {
        public RectTransform marker;
        public GameObject markedNPC;
    }
    public List<Marker> markers;

    [Header("Radar Effect")]
    public RectTransform radarEffect;
    public float duration = 1.5f;
    float t;
    public float delay = 2f;
    float _t;

    // Start is called before the first frame update
    void Start()
    {
        radius = (radarUIObject.sizeDelta.x * radarUIObject.localScale.x);
        radius *= 0.5f;

        radarEffect.localScale = Vector3.zero;
    }

    void LateUpdate()
    {
        UpdateMarkers();
        HandleRadarEffect();
    }

    void UpdateMarkers()
    {
        Transform playerT = GameManager.Instance.Player.transform;
        Vector3 forward = playerT.forward;
        forward.y = 0f;
        forward.Normalize();

        foreach (Marker m in markers)
        {
            Vector3 disp = m.markedNPC.transform.position - playerT.position;
            disp.y = 0f;

            float d = Mathf.Clamp01(disp.magnitude / radarRange);
            disp.Normalize();

            disp = playerT.InverseTransformDirection(disp);

            disp.y = disp.z;
            disp.z = 0f;

            m.marker.anchoredPosition3D = disp * radius * d;
        }
    }

    void HandleRadarEffect()
    {
        t += Time.deltaTime;
        radarEffect.localScale = Vector3.Lerp(
            Vector3.zero,
            Vector3.one,
            Mathf.Clamp01(t / duration));

        if (t >= duration)
            radarEffect.localScale = Vector3.zero;

        if (t >= duration + delay)
            t = 0f;
    }

    public void SpawnEnemyMarker(Transform npc)
    {
        Marker marker = new Marker();
        marker.marker = Instantiate(
            enemyMarker,
            radarUIObject.transform).GetComponent<RectTransform>();
        marker.markedNPC = npc.gameObject;

        markers.Add(marker);
    }

    public void RemoveEnemyMarker(Transform npc)
    {
        markers.Remove(markers.Single(m => m.markedNPC.Equals(npc.gameObject)));
    }
}
