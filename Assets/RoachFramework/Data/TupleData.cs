using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 泛型二元组数据结构
    /// </summary>
    /// <typeparam name="T1">参数类型1</typeparam>
    /// <typeparam name="T2">参数类型2</typeparam>
    public class Tuple2<T1, T2> : IComparable<Tuple2<T1, T2>>
        where T1 : IComparable<T1> where T2 : IComparable<T2> {

        public T1 v1;
        public T2 v2;

        public Tuple2() {
            v1 = default;
            v2 = default;
        }

        public Tuple2(T1 v1, T2 v2) {
            this.v1 = v1;
            this.v2 = v2;
        }

        public int CompareTo(Tuple2<T1, T2> other) {
            var chk = v1.CompareTo(other.v1);
            return chk == 0 ? v2.CompareTo(other.v2) : chk;
        }
    }

    /// <summary>
    ///  字符串二元组
    /// </summary>
    public class Tuple2Str : Tuple2<string, string> { }
    
    /// <summary>
    /// 整数二元组
    /// </summary>
    public class Tuple2Int : Tuple2<int, int> { }
    
    /// <summary>
    /// 泛型三元组数据结构
    /// </summary>
    /// <typeparam name="T1">参数类型1</typeparam>
    /// <typeparam name="T2">参数类型2</typeparam>
    /// <typeparam name="T3">参数类型3</typeparam>
    public class Tuple3<T1, T2, T3> : IComparable<Tuple3<T1, T2, T3>>
        where T1 : IComparable<T1> where T2 : IComparable<T2> where T3 : IComparable<T3> {

        public T1 v1;
        public T2 v2;
        public T3 v3;

        public Tuple3() {
            v1 = default;
            v2 = default;
        }

        public Tuple3(T1 v1, T2 v2, T3 v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public int CompareTo(Tuple3<T1, T2, T3> other) {
            var chk = v1.CompareTo(other.v1);
            if (chk != 0) return chk;
            chk = v2.CompareTo(other.v2);
            return chk == 0 ? v3.CompareTo(other.v3) : chk;
        }
    }
    
    /// <summary>
    ///  字符串三元组
    /// </summary>
    public class Tuple3Str : Tuple3<string, string, string> { }
    
    /// <summary>
    /// 整数三元组
    /// </summary>
    public class Tuple3Int : Tuple3<int, int, int> { }
}
