// ----------------------------------------------------------------------------------------------------
// Copyright © Guo jin ming. All rights reserved.
// Homepage: https://kylin.app/
// E-Mail: kevin@kylin.app
// ----------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kit
{
    /// <summary>
    /// 集合或数组的助手类
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// 升序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        public static void OrderBy<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
           where TKey : IComparable<TKey>
        {
            for (int i = 0; i < array.Length - 1; i++)
                for (int j = i + 1; j < array.Length; j++)
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        var temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
        }

        /// <summary>
        /// 降序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        public static void OrderByDescending<T, TKey>(T[] array, SelectHandler<T, TKey> handler)
            where TKey : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
                for (int j = i + 1; j < array.Length; j++)
                    if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                    {
                        var temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
        }

        public delegate bool FindHandler<T>(T item);
        public delegate TKey SelectHandler<TSource, TKey>(TSource source);

        /// <summary>
        /// 查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T Find<T>(T[] array, FindHandler<T> handler)
        {
            foreach (var item in array)
            {
                //调用委托
                if (handler(item))
                    return item;
            }
            return default(T);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(T[] array, FindHandler<T> handler)
        {
            List<T> tempList = new List<T>();
            foreach (var item in array)
            {
                //调用委托
                if (handler(item))
                    tempList.Add(item);
            }
            return tempList.Count > 0 ? tempList.ToArray() : null;
        }

        public static TKey[] Select<T, TKey>(T[] array,
            SelectHandler<T, TKey> handler)
        {
            TKey[] tempArr = new TKey[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                tempArr[i] = handler(array[i]);
            }
            return tempArr;
        }

    }

}


