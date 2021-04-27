using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInteraction : MonoBehaviour
{
    public int touched = 0;

    /*void touch() {
        touched++;
    }
    void leave() {
        touched--;
    }*/
    public bool checkMultiTouched() {
        if(touched > 1) {
            return true;
        }
        return false;
    }
}
