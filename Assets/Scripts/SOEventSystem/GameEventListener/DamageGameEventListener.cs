using SOGameEventSystem.Events;
using UnityEngine;
using UnityEngine.Events;

namespace SOGameEventSystem.EventListeners
{
    public class DamageGameEventListener : BaseGameEventListener<Damage, DamageGameEvent, UnityEvent<Damage>> { }
}
