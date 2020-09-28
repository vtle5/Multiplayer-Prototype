using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const int TICKS_PER_SEC = 60;    //in unity, go to project settings and change fixed timestamp to 1/TICKS_PER_SEC so 1/30 = .033333
    public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
}
