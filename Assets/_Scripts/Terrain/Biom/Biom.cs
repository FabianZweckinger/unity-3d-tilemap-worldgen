using System;
using UnityEngine;

[Serializable]
public class Biom : IComparable {
    public string biomName = "unnamed biom";
    public int priority = 0;
    public bool overwrite = false;
    public TileObjectRandom[] tileObjects;
    public BiomOctave[] octaves = new BiomOctave[] { new BiomOctave(.1f, 5) };
    public bool notUseColor = false;
    public Color groundColor;

    public int CompareTo(object obj) {
        int i = ((Biom)obj).priority;

        if (priority > i) {
            return -1;
        } else if (priority < i) {
            return 1;
        } else {
            return 0;
        }
    }
}