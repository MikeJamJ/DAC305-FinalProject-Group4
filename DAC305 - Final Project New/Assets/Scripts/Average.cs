using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Average
{
    private float average;
    private float sum;
    private int count;

    public Average() {
        average = 0f;
        sum = 0f;
        count = 0;
    }

    public void update(float value) {
        sum += value;
        count++;
        average = sum / count;
    }

    public void update(int value) {
        sum += value;
        count++;
        average = sum / count;
    }

    public float getAverage() {
        return average;
    }
}
