using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Color_ExtensionMethods {

    public static bool isEqual(this Color col, Color otherCol, float threshold) {

        if (Mathf.Abs(col.r - otherCol.r) > threshold)
            return false;
        if (Mathf.Abs(col.g - otherCol.g) > threshold)
            return false;
        if (Mathf.Abs(col.b - otherCol.b) > threshold)
            return false;
        return true;
    }

}

