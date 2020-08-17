using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory<T> : SerializableSingleton<T> where T : Factory<T>, new()
{
    
}
