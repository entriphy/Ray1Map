﻿using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// An animation speed where the speed is the amount to increase the animation frame by each second
    /// </summary>
    public class AnimSpeed_SecondIncrease : AnimSpeedWithValue
    {
        public AnimSpeed_SecondIncrease() { }
        public AnimSpeed_SecondIncrease(float speed)
        {
            Speed = speed;
        }

        protected override float GetFrameChange() => Time.deltaTime * Speed;
    }
}