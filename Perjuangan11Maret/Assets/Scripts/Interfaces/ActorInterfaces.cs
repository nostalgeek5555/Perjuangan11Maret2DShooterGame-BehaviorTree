using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillAble
{
    void Kill();
}

public interface IDamageAble<T>
{
    void TakeDamage(T damageTaken);
}
