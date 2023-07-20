using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageIndicator : MonoBehaviour
{
    public float lifetime = 0.5f;
    float t;
    public AnimationCurve scaleCurve;

    public Transform target;
    Vector3 playerPos;
    Vector3 playerForward;

    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rect.localScale = Vector3.one * scaleCurve.Evaluate(t / lifetime);
        t += Time.deltaTime;

        if(target != null)
        {
            if (GameManager.Instance.Player != null)
            {
                playerPos = GameManager.Instance.PlayerCenter;
                playerForward = GameManager.Instance.Player.transform.forward;
            }
            playerPos.y = 0f;
            Vector3 targetPos = target.position;
            targetPos.y = 0f;
            Vector3 dir = (targetPos - playerPos).normalized;
            float angle = Vector3.SignedAngle(
                playerForward,
                dir,
                Vector3.down);

            rect.eulerAngles = Vector3.forward * angle;  
        }

        if(t >= lifetime)
            Destroy(gameObject);
    }
}
