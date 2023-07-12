using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor
{
    public ActorData Data { get; }
    public GameObject GameObject { get; }
}
