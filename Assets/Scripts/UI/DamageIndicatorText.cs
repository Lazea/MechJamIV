using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicatorText : MonoBehaviour
{
    public TextMesh textMesh;

    [Header("Colors")]
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color fireColor;
    [SerializeField]
    Color energyColor;
    [SerializeField]
    Color shockColor;
    [SerializeField]
    Color critColor;

    [Header("Animations")]
    [SerializeField]
    float lifetime;
    float t;
    [SerializeField]
    AnimationCurve scaleCurve;
    [SerializeField]
    float distance;
    float sideSway;
    Vector3 origin;
    [SerializeField]
    AnimationCurve translationCurve;

    // Start is called before the first frame update
    void Start()
    {
        sideSway = UnityEngine.Random.Range(-1f, 1f);
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float normT = t / lifetime;

        transform.localScale = Vector3.one * scaleCurve.Evaluate(normT);

        float delta = translationCurve.Evaluate(normT);
        Vector3 pos = origin + Vector3.up * delta * distance;
        pos += Vector3.right * delta * sideSway * 0.5f;
        transform.position = pos;
        
        transform.LookAt(
            transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);

        if(t >= lifetime)
        {
            Destroy(gameObject);
        }

        t += Time.deltaTime;
    }

    public void SetdamageValue(Damage damage)
    {
        if(damage.isCrit)
        {
            textMesh.color = critColor;
        }
        else
        {
            switch(damage.damageType)
            {
                case DamageType.Energy:
                    textMesh.color = energyColor;
                    break;
                case DamageType.Fire:
                    textMesh.color = fireColor;
                    break;
                case DamageType.Shock:
                    textMesh.color = shockColor;
                    break;
                default:
                    textMesh.color = normalColor;
                    break;
            }
        }

        textMesh.text = damage.amount.ToString();
    }
}
