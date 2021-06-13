using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanHoldCrate
{
    bool CanHoldCrate();

    void PlaceCrate(Crate c);
}
