using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class VectorComparer : IComparer<Vector3> {
    public int Compare(Vector3 v1, Vector3 v2) {
        if (Mathf.Approximately(v1.x, v2.x))
            return 0;
        return v1.x > v2.x ? 1 : -1;
    }
}

