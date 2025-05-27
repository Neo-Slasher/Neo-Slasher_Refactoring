using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExpList {
    public List<Exp> exp;
}

[Serializable]
public class Exp {
    public int level;
    public int reqExp;
    public int cExp;
}
