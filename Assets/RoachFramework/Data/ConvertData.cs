using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// Vector2的Json转化过渡数据结构
    /// </summary>
    public struct CVector2 {
        
        public float x;
        public float y;
        public float magnitude => Mathf.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;

        public CVector2(float x, float y) {
            this.x = x;
            this.y = y;
        }
        
        public static implicit operator Vector2(CVector2 vec) {
            return new Vector2(vec.x, vec.y);
        }
		
        public static implicit operator CVector2(Vector2 vec) {
            return new CVector2(vec.x, vec.y);
        }
        
        /// <summary>
        /// 重载三轴坐标的加法操作
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>结果</returns>
        public static CVector2 operator +(CVector2 left, CVector2 right) {
        	return new CVector2() {
        		x = left.x + right.x,
        		y = left.y + right.y
        	};
        }

        /// <summary>
        /// 重载三轴坐标的减法操作
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>结果</returns>
        public static CVector2 operator -(CVector2 left, CVector2 right) {
        	return new CVector2() {
        		x = left.x - right.x,
        		y = left.y - right.y
        	};
        }

        /// <summary>
        /// 重载坐标乘法
        /// </summary>
        /// <param name="origin">原坐标</param>
        /// <param name="multi">乘数</param>
        /// <returns>新坐标</returns>
        public static CVector2 operator *(CVector2 origin, int multi) {
        	return new CVector2(origin.x * multi, origin.y * multi);
        }
        
        /// <summary>
        /// 重载坐标除法
        /// </summary>
        /// <param name="origin">原坐标</param>
        /// <param name="div">除数</param>
        /// <returns>新坐标</returns>
        public static CVector2 operator /(CVector2 origin, int div) {
        	return new CVector2(origin.x / div, origin.y / div);
        }

        /// <summary>
        /// 克隆一个相同坐标对象
        /// </summary>
        /// <returns></returns>
        public CVector2 Clone() {
        	return new CVector2(x, y);
        }
    }
    
    /// <summary>
    /// Vector3的Json转化过渡数据结构
    /// </summary>
    public struct CVector3 {
        
        public float x;
        public float y;
        public float z;
        public float magnitude => Mathf.Sqrt(x * x + y * y + z * z);
        public float sqrMagnitude => x * x + y * y + z * z;

        public CVector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public static implicit operator Vector3(CVector3 vec) {
            return new Vector3(vec.x, vec.y, vec.z);
        }
		
        public static implicit operator CVector3(Vector3 vec) {
            return new CVector3(vec.x, vec.y, vec.z);
        }
        
        /// <summary>
        /// 重载三轴坐标的加法操作
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>结果</returns>
        public static CVector3 operator +(CVector3 left, CVector3 right) {
        	return new CVector3() {
        		x = left.x + right.x,
        		y = left.y + right.y,
                z = left.z + right.z
        	};
        }

        /// <summary>
        /// 重载三轴坐标的减法操作
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>结果</returns>
        public static CVector3 operator -(CVector3 left, CVector3 right) {
        	return new CVector3() {
        		x = left.x - right.x,
        		y = left.y - right.y,
                z = left.z - right.z
        	};
        }

        /// <summary>
        /// 重载坐标乘法
        /// </summary>
        /// <param name="origin">原坐标</param>
        /// <param name="multi">乘数</param>
        /// <returns>新坐标</returns>
        public static CVector3 operator *(CVector3 origin, int multi) {
        	return new CVector3(origin.x * multi, origin.y * multi, origin.z * multi);
        }
        
        /// <summary>
        /// 重载坐标除法
        /// </summary>
        /// <param name="origin">原坐标</param>
        /// <param name="div">除数</param>
        /// <returns>新坐标</returns>
        public static CVector3 operator /(CVector3 origin, int div) {
        	return new CVector3(origin.x / div, origin.y / div, origin.z / div);
        }

        /// <summary>
        /// 克隆一个相同坐标对象
        /// </summary>
        /// <returns></returns>
        public CVector3 Clone() {
        	return new CVector3(x, y, z);
        }
    }
}
