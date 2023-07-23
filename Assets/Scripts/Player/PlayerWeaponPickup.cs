using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponPickup : MonoBehaviour
{
    [Header("Player Weapon Class")]
    public Weapon weapon;
    public Transform inHandWeapon;
    public Transform weaponBody;

    [Header("Pickup")]
    public float pickupRange = 4f;
    public float pickupHoldTime;
    float t;
    bool pickupHold;
    WeaponPickup pickup;

    [Header("Pickup Masks")]
    public LayerMask pickupMask;
    public LayerMask coverMask;

    [Header("Events")]
    public UnityEvent onPickup;

    // Start is called before the first frame update
    void Start()
    {
        if(weapon == null)
            weapon = GetComponent<Weapon>();
            
        pickup = null;

        var newWeapon = WeaponGenerator.Instance.GenerateWeapon(
            inHandWeapon.position,
            weapon.Data,
            inHandWeapon.gameObject,
            addPickup:false);
        weaponBody = newWeapon.Item1.transform;
        weaponBody.localScale = Vector3.one;
        weaponBody.rotation = Quaternion.LookRotation(inHandWeapon.forward);
        weapon.projectileSpawnPoint.transform.position = 
            newWeapon.Item2.transform.Find("ProjectileSpawnMount").position;
    }

    // Update is called once per frame
    void Update()
    {
        if(pickupHold)
        {
            t += Time.deltaTime;
            PickupMenuUI.Instance.SetPickupIndicatorFillBar(t / pickupHoldTime);

            if(t >= pickupHoldTime)
            {
                t = 0f;
                PickupWeapon();
                InteractRelease();
                PickupMenuUI.Instance.enabled = false;
            }
        }
        else
        {
            t = 0f;
        }
    }

    private void FixedUpdate()
    {
        WeaponPickup newPickup = GetWeaponPickup();
        if(newPickup != pickup)
        {
            if(newPickup == null)
            {
                Debug.LogFormat("No pickup");
                PickupMenuUI.Instance.enabled = false;
            }
            else
            {
                Debug.LogFormat("Found pickup {0}", newPickup.gameObject.name);

                PickupMenuUI.Instance.SetCarryWeapon(weapon.Data);
                PickupMenuUI.Instance.SetGroundWeapon(newPickup.Data);

                PickupMenuUI.Instance.enabled = true;
            }
        }

        pickup = newPickup;
    }

    public WeaponPickup GetWeaponPickup()
    {
        WeaponPickup _pickup = null;
        float distance = Mathf.Infinity;
        Vector3 origin = transform.position + Vector3.up;
        Collider[] colls = Physics.OverlapSphere(
            origin,
            pickupRange,
            pickupMask);
        foreach(var coll in colls)
        {
            WeaponPickup newPickup = coll.GetComponentInParent<WeaponPickup>();
            if(newPickup != null)
            {
                Vector3 disp = newPickup.transform.position - origin;
                float dist = disp.magnitude;
                Ray ray = new Ray(origin, disp);
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * dist);
                if(!Physics.Raycast(
                    ray,
                    dist,
                    coverMask))
                {
                    if(dist <= distance)
                    {
                        _pickup = newPickup;
                        distance = dist;
                    }
                }
            }
        }

        return _pickup;
    }

    [ContextMenu("Pickup Weapon")]
    public void PickupWeapon()
    {
        if(pickup == null)
        {
            Debug.LogWarning("No weapon in range for pickup.");
            return;
        }

        // Save current weapon data in hand
        var currData = weapon.Data;

        // Set ground weapon as current weapon and generate mesh in hand
        weapon.Data = pickup.Data;
        weapon.ResetWeaponData();
        Destroy(weaponBody.gameObject);
        var newWeapon = WeaponGenerator.Instance.GenerateWeapon(
            inHandWeapon.position,
            weapon.Data,
            inHandWeapon.gameObject,
            addPickup:false);
        weaponBody = newWeapon.Item1.transform;
        weaponBody.localScale = Vector3.one;
        weaponBody.rotation = Quaternion.LookRotation(inHandWeapon.forward);
        weapon.projectileSpawnPoint.transform.position = newWeapon.Item2.transform.Find("ProjectileSpawnMount").position;

        // Generate a new pickup weapon on the ground using the old in hand weapon data
        Vector3 position = pickup.transform.position;
        position += Vector3.up * 0.6f;
        Destroy(pickup.gameObject);
        WeaponGenerator.Instance.GenerateWeapon(position, currData);

        onPickup.Invoke();
    }

    public void InteractHold()
    {
        Debug.LogFormat("Interact Hold");
        pickupHold = true;
    }

    public void InteractRelease()
    {
        Debug.Log("Interact Hold released");
        pickupHold = false;
        PickupMenuUI.Instance.SetPickupIndicatorFillBar(0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, pickupRange);
    }
}
