﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TagsTree.Services;

public static class Serialization
{
    /// <summary>
    /// 将Json文件反序列化为某个类
    /// </summary>
    /// <typeparam name="T">带无参构造的类</typeparam>
    /// <param name="path">Json文件位置</param>
    /// <returns>返回文件中的数据，如果没有则返回新实例</returns>
    public static T Deserialize<T>(string path) where T : new()
    {
        try
        {
            return JsonSerializer.Deserialize<T>(File.OpenRead(path)) ?? new T();
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// 异步将Json文件反序列化为某个类
    /// </summary>
    /// <typeparam name="T">带无参构造的类</typeparam>
    /// <param name="path">Json文件位置</param>
    /// <returns>返回文件中的数据，如果没有则返回新实例</returns>
    public static async ValueTask<T> DeserializeAsync<T>(string path) where T : new()
    {
        try
        {
            return await JsonSerializer.DeserializeAsync<T>(File.OpenRead(path)) ?? new T();
        }
        catch (Exception)
        {
            return new();
        }
    }

    /// <summary>
    /// 将某个类序列化为Json文件
    /// </summary>
    /// <typeparam name="T">泛型类</typeparam>
    /// <param name="path">Json文件路径</param>
    /// <param name="obj">需要转化的对象</param>
    /// <returns></returns>
    public static void Serialize<T>(string path, T obj) => JsonSerializer.Serialize(File.Create(path), obj);

    /// <summary>
    /// 异步将某个类序列化为Json文件
    /// </summary>
    /// <typeparam name="T">泛型类</typeparam>
    /// <param name="path">Json文件路径</param>
    /// <param name="obj">需要转化的对象</param>
    /// <returns></returns>
    public static async ValueTask SerializeAsync<T>(string path, T obj) => await JsonSerializer.SerializeAsync(File.Create(path), obj);
}
