﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Util
{ 
    public static T GetChildByName<T>(this GameObject go, string name) where T:Component
    {
        var child = GetChildByName(go, name);
        if (child == null)
            return default(T);

        return child.gameObject.GetComponent<T>();
    }

    public static GameObject GetChildByName(this GameObject go, string name) 
    {
        var child = GetChildByName(go.transform, name);
        return child == null ? null : child.gameObject;
    }

    public static Transform GetChildByName(Transform tr, string name)
    {
        // 广度优先
        foreach (Transform child in tr)
        {
            if (child.name == name)
                return child;
        }

        foreach (Transform child in tr)
        {
            Transform c = GetChildByName(child, name);
            if (c != null)
                return c;
        }

        return null;
    }

    public delegate bool TraversalCallback(Transform go);
    public static bool Traversal(this Transform transform, TraversalCallback callback)
    {
        // 深度优先
        foreach (Transform child in transform)
        {
            if (!callback(child))
                return false;

            if (!child.Traversal(callback))
                return false;
        }
        return true;
    }

    public static void ChangeLayer(GameObject go, int layer)
    {
        go.layer = layer;
        go.transform.Traversal(child=>{child.gameObject.layer = layer; return true; });
    }
    
    public static string UrlToIP(string url)
    {
        try
        {
            var host = System.Net.Dns.GetHostEntry(url);
            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return url;
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
            return url;
        }
    }

    public static string Md5File(string file)
    {
        using (var stream = System.IO.File.OpenRead(file))
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var data = md5.ComputeHash(stream);
            var sb = new System.Text.StringBuilder();

            foreach(var b in data)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
    }

    public static string GetFileSizeString(double size)
    {
        // http://en.wikipedia.org/wiki/File_size
        var units = new string[]{"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};
        foreach (var unit in units)
        {
            if (size < 1024d)
                return string.Format("{0:F}{1}", size, unit);
            size /= 1024;
        }
        return string.Format("{0:F}{1}", size, units[units.Length - 1]);
    }

    public static string GetFullName(this GameObject go, Transform root = null)
    {
        var sb = new System.Text.StringBuilder();
        var transform = go.transform;
        while (transform != null && transform != root)
        {
            sb.Insert(0, string.Format("/{0}", transform.name));
            transform = transform.parent;
        }
        return sb.ToString();
    }

	public static void RandomArray<T>(T[] array, int startIndex)
	{
		RandomArray(array, startIndex, array.Length - 1);
	}

	public static void RandomArray<T>(T[] array, int startIndex, int endIndex)
	{
		if (startIndex < 0 || endIndex >= array.Length || startIndex > endIndex)
			throw new UnityException(string.Format("failed to random array({0}), illegal random index: ({1}, {2})", array.Length, startIndex, endIndex));

		var randCount = endIndex - startIndex;
		while (randCount > 0)
		{
			var i = Random.Range(0, randCount);

			var temp = array[startIndex + randCount];
			array[startIndex + randCount] = array[startIndex + i];
			array[startIndex + i] = temp;

			randCount--;
		}
	}

//	static public T AddMissingComponent<T>(this GameObject go) where T : Component
//	{
//		T comp = go.GetComponent<T>();
//		if (comp == null)
//			comp = go.AddComponent<T>();
//		return comp;
//	}
}
