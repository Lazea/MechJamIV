using UnityEngine;

namespace SOGameEventSystem.Events
{
    [CreateAssetMenu(
        fileName = "SOEvent_Damage_GameEvent",
        menuName = "Scriptable Objects/GameEvent System/Damage Game Event")]
    public class DamageGameEvent : BaseGameEvent<Damage> { }
}
