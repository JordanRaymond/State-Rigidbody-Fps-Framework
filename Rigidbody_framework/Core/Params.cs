using UnityEngine;

public abstract class Params<T> : ScriptableObject where T : Params<T>
{
    public abstract void UpdateValues(T newValues);
}