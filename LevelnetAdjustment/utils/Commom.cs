﻿using LevelnetAdjustment.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LevelnetAdjustment.utils {
  public class Commom {

    /// <summary>
    /// 深拷贝
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T Clone<T>(T source) {
      using (Stream objectStream = new MemoryStream()) {
        //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
        IFormatter formatter = new BinaryFormatter();
        formatter.Serialize(objectStream, source);
        objectStream.Seek(0, SeekOrigin.Begin);
        return (T)formatter.Deserialize(objectStream);
      }
    }

    /// <summary>
    /// 合并两个集合的函数
    /// </summary>
    /// <param name="list1">第一个集合</param>
    /// <param name="list2">第二个集合</param>
    /// <returns>返回第union的合并结果</returns>
    public static List<T> Merge<T>(List<T> list1, List<T> list2) where T : class {
      List<T> listMerge1 = list1.Union(list2).ToList();//不允许有重复项
      List<T> listMerge2 = list1.Concat(list2).ToList();//允许出现重复项
      return listMerge2;
    }

    /// <summary>
    /// 判断是否为数字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsFloat(string str) {
      string regextext = "[+-]?[0-9]+(\\.[0-9]+)?";
      Regex regex = new Regex(regextext, RegexOptions.None);
      return regex.IsMatch(str);
    }

    /// <summary>
    /// 判断是否为整数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsInteger(string str) {
      try {
        int i = Convert.ToInt32(str);
        return true;
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// 生成指定长度的大小写字母和数字随机组合字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateRandomString(int length) {
      string word = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
      //随机类
      Random ra = new Random();
      string res = "";
      for (int j = 0; j < length; j++) {
        //拼接字符
        res += word[ra.Next(62)];
      }
      return res;
    }
  }
}
