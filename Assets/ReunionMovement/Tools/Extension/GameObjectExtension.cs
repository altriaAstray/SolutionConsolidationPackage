using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = System.Object;

/// <summary>
/// 游戏对象扩展
/// </summary>
public static class GameObjectExtension
{
    /// <summary>
    /// 设置宽
    /// </summary>
    /// <param name="rectTrans"></param>
    /// <param name="width"></param>
    public static void SetWidth(this RectTransform rectTrans, float width)
    {
        var size = rectTrans.sizeDelta;
        size.x = width;
        rectTrans.sizeDelta = size;
    }

    /// <summary>
    /// 设置高
    /// </summary>
    /// <param name="rectTrans"></param>
    /// <param name="height"></param>
    public static void SetHeight(this RectTransform rectTrans, float height)
    {
        var size = rectTrans.sizeDelta;
        size.y = height;
        rectTrans.sizeDelta = size;
    }
    /// <summary>
    /// 获取位置的X轴
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newX"></param>
    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }
    /// <summary>
    /// 获取位置的Y轴
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newY"></param>
    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }
    /// <summary>
    /// 获取位置的Z轴
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newZ"></param>
    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }
    /// <summary>
    /// 获取本地位置的X轴
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newX"></param>
    public static void SetLocalPositionX(this Transform t, float newX)
    {
        t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
    }
    /// <summary>
    /// 获取本地位置的Y轴
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newY"></param>
    public static void SetLocalPositionY(this Transform t, float newY)
    {
        t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newZ"></param>
    public static void SetLocalPositionZ(this Transform t, float newZ)
    {
        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
    }
    /// <summary>
    /// 设置缩放为0
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newScale"></param>
    public static void SetLocalScale(this Transform t, Vector3 newScale)
    {
        t.localScale = newScale;
    }
    /// <summary>
    /// 设置本地缩放为0
    /// </summary>
    /// <param name="t"></param>
    public static void SetLocalScaleZero(this Transform t)
    {
        t.localScale = Vector3.zero;
    }
    /// <summary>
    /// 获取位置的X轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }
    /// <summary>
    /// 获取位置的Y轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }
    /// <summary>
    /// 获取位置的Z轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }
    /// <summary>
    /// 获取本地位置的X轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetLocalPositionX(this Transform t)
    {
        return t.localPosition.x;
    }
    /// <summary>
    /// 获取本地位置的Y轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetLocalPositionY(this Transform t)
    {
        return t.localPosition.y;
    }
    /// <summary>
    /// 获取本地位置的Z轴
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetLocalPositionZ(this Transform t)
    {
        return t.localPosition.z;
    }
    /// <summary>
    /// 判断活动状态
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsActive(this Transform t)
    {
        if (t && t.gameObject)
            return t.gameObject.activeInHierarchy;
        return false;
    }
    /// <summary>
    /// 变换转矩阵变换
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static RectTransform rectTransform(this Transform t)
    {
        if (t && t.gameObject)
            return t.gameObject.GetComponent<RectTransform>();
        return null;
    }
    /// <summary>
    /// 判断刚体是否存在
    /// </summary>
    /// <param name="gobj"></param>
    /// <returns></returns>
    public static bool HasRigidbody(this GameObject gobj)
    {
        return (gobj.GetComponent<Rigidbody>() != null);
    }
    /// <summary>
    /// 判断动画是否存在
    /// </summary>
    /// <param name="gobj"></param>
    /// <returns></returns>
    public static bool HasAnimation(this GameObject gobj)
    {
        return (gobj.GetComponent<Animation>() != null);
    }
    /// <summary>
    /// 设置动画速度
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="newSpeed"></param>
    public static void SetSpeed(this Animation anim, float newSpeed)
    {
        anim[anim.clip.name].speed = newSpeed;
    }
    /// <summary>
    /// v3转v2
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    /// <summary>
    /// 设置活动状态
    /// </summary>
    /// <param name="com"></param>
    /// <param name="visible"></param>
    public static void SetActive(this Component com, bool visible)
    {
        if (com && com.gameObject && com.gameObject.activeSelf != visible) com.gameObject.SetActive(visible);
    }
    /// <summary>
    /// 设置活动状态（反向）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="visible"></param>
    public static void SetActiveX(this GameObject go, bool visible)
    {
        if (go && go.activeSelf != visible) go.SetActive(visible);
    }
    /// <summary>
    /// 设置名字
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    public static void SetName(this GameObject go, string name)
    {
        if (go && go.name != name) go.name = name;
    }
}

public static class EngineExtensions
{
    /// <summary>
    /// 字符串转byte
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static byte ToByte(this string val)
    {
        byte ret = 0;
        try
        {
            if (!String.IsNullOrEmpty(val))
            {
                ret = Convert.ToByte(val);
            }
        }
        catch (Exception)
        {
        }

        return ret;
    }
    /// <summary>
    /// 字符串转int64
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static long ToInt64(this string val)
    {
        long ret = 0;
        try
        {
            if (!String.IsNullOrEmpty(val))
            {
                ret = Convert.ToInt64(val);
            }
        }
        catch (Exception)
        {
        }

        return ret;
    }
    /// <summary>
    /// 字符串转float
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float ToFloat(this string val)
    {
        float ret = 0;
        try
        {
            if (!String.IsNullOrEmpty(val))
            {
                ret = Convert.ToSingle(val);
            }
        }
        catch (Exception)
        {
        }

        return ret;
    }
    /// <summary>
    /// 字符串转int32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public Int32 ToInt32(this string str)
    {
        Int32 ret = 0;
        try
        {
            if (!String.IsNullOrEmpty(str))
            {
                ret = Convert.ToInt32(str);
            }
        }
        catch (Exception)
        {
        }

        return ret;
    }
    /// <summary>
    /// 对象转int32
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Int32 ToInt32(this object obj)
    {
        Int32 ret = 0;
        try
        {
            if (obj != null)
            {
                ret = Convert.ToInt32(obj);
            }
        }
        catch (Exception)
        {
        }

        return ret;
    }

    /// <summary>
    /// 从对象数组中获取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="openArgs"></param>
    /// <param name="offset"></param>
    /// <param name="isLog"></param>
    /// <returns></returns>
    public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
    {
        T ret;
        if ((openArgs.Length - 1) >= offset)
        {
            var arrElement = openArgs[offset];
            if (arrElement == null)
                ret = default(T);
            else
            {
                try
                {
                    ret = (T)Convert.ChangeType(arrElement, typeof(T));
                }
                catch (Exception)
                {
                    if (arrElement is string && string.IsNullOrEmpty(arrElement as string))
                        ret = default(T);
                    else
                    {
                        Debug.LogError(string.Format("[Error get from object[],  '{0}' change to type {1}", arrElement, typeof(T)));

                        ret = default(T);
                    }
                }
            }
        }
        else
        {
            ret = default(T);

            Debug.LogError(string.Format("[GetArg] {0} args - offset: {1}", openArgs, offset));
        }

        return ret;
    }
}

// C# 扩展, 扩充C#类的功能
public static class EngineToolExtensions
{
    // 扩展List/  

    /// <summary>
    /// 洗牌
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// 从序列中获取第一个元素或者默认值
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FirstOrDefault<T>(this IEnumerable<T> source)
    {
        if (source != null)
        {
            foreach (T item in source)
            {
                return item;
            }
        }

        return default(T);
    }
    /// <summary>
    /// 从序列中获取第一个元素
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> First<T>(this IEnumerable<T> source, int num)
    {
        var count = 0;
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (++count > num)
                {
                    break;
                }

                items.Add(item);
            }
        }

        return items;
    }

    public delegate bool FilterAction<T>(T t);

    /// <summary>
    /// 筛选(列表)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="testAction"></param>
    /// <returns></returns>
    public static List<T> Filter<T>(this IEnumerable<T> source, FilterAction<T> testAction)
    {
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (testAction(item))
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    public delegate bool FilterAction<T, K>(T t, K k);

    /// <summary>
    /// 筛选(字典)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="source"></param>
    /// <param name="testAction"></param>
    /// <returns></returns>
    public static Dictionary<T, K> Filter<T, K>(this IEnumerable<KeyValuePair<T, K>> source, FilterAction<T, K> testAction)
    {
        var items = new Dictionary<T, K>();
        if (source != null)
        {
            foreach (KeyValuePair<T, K> pair in source)
            {
                if (testAction(pair.Key, pair.Value))
                {
                    items.Add(pair.Key, pair.Value);
                }
            }
        }

        return items;
    }
    /// <summary>
    /// 从序列中获取最后一个元素或者默认值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T LastOrDefault<T>(this IEnumerable<T> source)
    {
        var result = default(T);
        foreach (T item in source)
        {
            result = item;
        }

        return result;
    }

    /// <summary>
    /// 从序列中获取最后一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static List<T> Last<T>(this IEnumerable<T> source, int num)
    {
        // 开始读取的位置
        var startIndex = Math.Max(0, source.ToList().Count - num);
        var index = 0;
        var items = new List<T>();
        if (source != null)
        {
            foreach (T item in source)
            {
                if (index < startIndex)
                {
                    continue;
                }

                items.Add(item);
            }
        }

        return items;
    }

    /// <summary>
    /// 给哈希集添加批量数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
    {
        bool allAdded = true;
        foreach (T item in items)
        {
            allAdded &= @this.Add(item);
        }

        return allAdded;
    }
    /// <summary>
    /// 转成数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T[] ToArray<T>(this IEnumerable<T> source)
    {
        var list = new List<T>();
        foreach (T item in source)
        {
            list.Add(item);
        }

        return list.ToArray();
    }
    /// <summary>
    /// 转成列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this IEnumerable<T> source)
    {
        var list = new List<T>();
        foreach (T item in source)
        {
            list.Add(item);
        }

        return list;
    }
    /// <summary>
    /// 联合体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<T> Union<T>(this List<T> first, List<T> second, IEqualityComparer<T> comparer)
    {
        var results = new List<T>();
        var list = first.ToList();
        list.AddRange(second);
        foreach (T item in list)
        {
            var include = false;
            foreach (T result in results)
            {
                if (comparer.Equals(result, item))
                {
                    include = true;
                    break;
                }
            }

            if (!include)
            {
                results.Add(item);
            }
        }

        return results;
    }
    /// <summary>
    /// 加入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sp"></param>
    /// <returns></returns>
    public static string Join<T>(this IEnumerable<T> source, string sp)
    {
        var result = new StringBuilder();
        foreach (T item in source)
        {
            if (result.Length == 0)
            {
                result.Append(item);
            }
            else
            {
                result.Append(sp).Append(item);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        foreach (TSource item in source)
        {
            if (Equals(item, value))
            {
                return true;
            }
        }

        return false;
    }
}