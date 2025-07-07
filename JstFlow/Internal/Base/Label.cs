using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.Base
{
    public class Label : IEquatable<Label>
    {
        public string Code { get; }
        public string Name { get; }

        public Label(string code, string name = "")
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? "";
        }

        public override string ToString() => Name;

        public bool Equals(Label other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Code == other.Code;
        }

        public override bool Equals(object obj) =>
            obj is Label other && Equals(other);

        public override int GetHashCode() => Code?.GetHashCode() ?? 0;

        public static bool operator ==(Label left, Label right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Code == right.Code;
        }

        public static bool operator !=(Label left, Label right) => !(left == right);

        public static implicit operator Label(string code)
        {
            return new Label(code, "");
        }
    }


    public class Label<T> : IEquatable<Label<T>>
    {
        /// <summary>
        /// 唯一标识，用于相等判断和字典 key
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 中文名或友好显示名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 绑定的值
        /// </summary>
        public T Value { get; }

        public Label(string code, string name, T value)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? "";
            Value = value;
        }

        public override string ToString() => Name;

        public bool Equals(Label<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Code == other.Code;
        }

        public override bool Equals(object obj) =>
            obj is Label<T> other && Equals(other);

        public override int GetHashCode() => Code?.GetHashCode() ?? 0;

        public static bool operator ==(Label<T> left, Label<T> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Code == right.Code;
        }

        public static bool operator !=(Label<T> left, Label<T> right) => !(left == right);
        
        public static implicit operator Label<T>(string code)
        {
            return new Label<T>(code, "", default);
        }
    }


}
