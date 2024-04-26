using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 整型进度数据
    /// </summary>
    public class IntegerMeter {
		
        public int cusorValue;
        public int targetValue;

        public int DeltaValue => targetValue - cusorValue;

        public IntegerMeter(int val, bool startFull = false) {
            targetValue = val;
            cusorValue = startFull ? targetValue : 0;
        }

        public void ChangeCusor(int delta) {
            cusorValue += delta;
            if (cusorValue < 0) {
                cusorValue = 0;
            } else if (cusorValue > targetValue) {
                cusorValue = targetValue;
            }
        }

        public void Reset() {
            cusorValue = targetValue = 0;
        }

        public void Reset(int v) {
            cusorValue = targetValue = v;
        }

        public bool IsMax() {
            return cusorValue >= targetValue;
        }

        public float GetPercentage() {
            return cusorValue * 1f / targetValue;
        }
    }

    /// <summary>
    /// 浮点型进度数据
    /// </summary>
    public class FloatMeter {

        public float cusorValue;
        public float targetValue;
		
        public float DeltaValue => targetValue - cusorValue;

        public FloatMeter(float val, bool startFull = false) {
            targetValue = val;
            cusorValue = startFull ? targetValue : 0f;
        }
		
        public void ChangeCusor(float delta) {
            cusorValue += delta;
            if (cusorValue < 0f) {
                cusorValue = 0f;
            } else if (cusorValue > targetValue) {
                cusorValue = targetValue;
            }
        }

        public void Reset() {
            cusorValue = targetValue = 0;
        }

        public void Reset(float v) {
            cusorValue = targetValue = v;
        }

        public bool IsMax() {
            return cusorValue >= targetValue;
        }
		
        public float GetPercentage() {
            return cusorValue * 1f / targetValue;
        }
    }
}
