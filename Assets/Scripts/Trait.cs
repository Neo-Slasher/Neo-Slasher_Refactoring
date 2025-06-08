using System;
using System.Collections.Generic;


[Serializable]
public class TraitList
{
    public List<Trait> trait;
}


[Serializable]
public class Trait
{
    public int index;
    public string name;
    public int requireLv;
    public int rank;
    public int imageIndex;
    public string script;
    public int effectType1;
    public float effectValue1;
    public bool effectMulti1;
    public int effectType2;
    public float effectValue2;
    public bool effectMulti2;
    public int effectType3;
    public float effectValue3;
    public bool effectMulti3;
    public int effectType4;
    public float effectValue4;
    public bool effectMulti4;
}
