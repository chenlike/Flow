using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Common
{
    public class Res<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        // 构造函数
        public Res()
        {
        }

        public Res(bool success, string message = "", T data = default(T))
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // 成功结果的静态工厂方法
        public static Res<T> Ok(T data = default(T), string message = "操作成功")
        {
            return new Res<T>(true, message, data);
        }

        // 失败结果的静态工厂方法
        public static Res<T> Fail(string message = "操作失败", T data = default(T))
        {
            return new Res<T>(false, message, data);
        }
        // 检查是否成功
        public bool IsSuccess => Success;

        // 检查是否失败
        public bool IsFailure => !Success;


    }

    // 非泛型版本，用于不需要返回数据的操作
    public class Res
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public Res(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }

        public static Res Ok(string message = "操作成功")
        {
            return new Res(true, message);
        }

        public static Res Fail(string message = "操作失败")
        {
            return new Res(false, message);
        }

        public static Res Error(Exception ex)
        {
            return new Res(false, ex.Message);
        }

        public bool IsSuccess => Success;
        public bool IsFailure => !Success;
    }
}
