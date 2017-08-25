using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace maskx.SystemWorkflow
{
    /// <summary>
    ///  
    /// </summary>
    public class WFJson : IList
    {

        internal JToken _JToken;


        #region Constructor
        public WFJson()
        {
            _JToken = new JObject();
        }
        public WFJson(JToken jToke)
        {
            _JToken = jToke;
        }
        public WFJson(object obj)
        {
            _JToken = JToken.FromObject(obj);
        }
        #endregion

        #region Method
        public static WFJson Parse(string json)
        {
            return new WFJson(JToken.Parse(json));
        }

        /// <summary>
        /// 获取对象的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="path">对象的路径</param>
        /// <returns>对象的值</returns>
        public T V<T>(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                return _JToken.ToObject<T>();
            var v = _JToken.SelectToken(path);
            if (v == null)
                throw new KeyNotFoundException(string.Format("cannot find {0} in {1}", path, _JToken));
            return v.ToObject<T>();
        }
        #endregion

        #region Indexer
        /// <summary>
        /// 获取Array的指定索引元素或Object的指定索引的属性
        /// </summary>
        /// <param name="index">要获取元素的索引</param>
        /// <returns>指定索引的元素</returns>
        public WFJson this[int index]
        {
            get
            {
                var a = _JToken as JArray;
                if (a == null)
                {
                    var b = _JToken as JObject;
                    var c = b.PropertyValues().ToArray()[index];
                    return new WFJson(c);
                }
                return new WFJson(a[index]);
            }
            set
            {
                var a = _JToken as JArray;
                if (a == null)
                {
                    var b = _JToken as JObject;
                    var c = b.Properties().ToArray()[index];
                    c.Value = value._JToken;
                }
                else
                {
                    a[index] = value._JToken;
                }

            }
        }
        /// <summary>
        /// 获取指定路径的对象
        /// </summary>
        /// <param name="path">对象路径</param>
        /// <returns>指定路径的对象</returns>
        public WFJson this[string path]
        {
            get
            {
                var v = _JToken.SelectToken(path);
                if (v == null)
                    throw new KeyNotFoundException(string.Format("cannot find {0} in {1}", path, _JToken));
                return new WFJson(v);
            }
            set
            {
                SetValue(path, value._JToken);
            }
        }
        #endregion

        #region IList
        public IEnumerator GetEnumerator()
        {
            if (_JToken is JContainer a)
            {
                foreach (var item in a.Children())
                {
                    yield return new WFJson(item);
                }
            }

        }
        public int Add(object value)
        {
            var a = _JToken as JArray;
            if (a == null)
                throw new Exception("Only collecttion can add child");
            if (value is WFJson c)
            {
                a.Add(c._JToken);
            }
            else
            {
                a.Add(JToken.FromObject(value));
            }
            return a.Count;
        }

        public bool Contains(object value)
        {
            var a = _JToken as JContainer;
            if (value is WFJson wj)
                return a.Contains(wj._JToken);
            if (value is JToken j)
                return a.Contains(j);
            return a.Contains(value);
        }
        public bool Contains<T>(T v)
        {
            var a = _JToken as JContainer;
            if (v is WFJson wj)
                return a.Contains(wj._JToken);
            if (v is JToken j)
                return a.Contains(j);
            foreach (WFJson item in this)
            {
                if (EqualityComparer<T>.Default.Equals(item.V<T>(), v))
                    return true;
            }
            return false;
        }
        public void Clear()
        {
            var a = _JToken as JContainer;
            a.RemoveAll();
        }
        public int IndexOf(object value)
        {
            JToken t = null;
            if (value is WFJson wj)
                t = wj._JToken;
            else if (value is JToken j)
                t = j;
            if (_JToken is JArray a)
                return a.IndexOf(t);
            var b = _JToken as JObject;
            int index = 0;
            foreach (var item in b.Properties())
            {
                if (item == t)
                    return index;
                index++;
            }
            return -1;
        }
        public int IndexOf<T>(T value)
        {
            JToken t = null;
            if (value is WFJson wj)
                t = wj._JToken;
            else if (value is JToken j)
                t = j;
            if (t == null)
            {
                int index = -1;
                foreach (WFJson item in this)
                {
                    index++;
                    if (EqualityComparer<T>.Default.Equals(item.V<T>(), value))
                        return index;
                }
            }
            else
            {
                if (_JToken is JArray a)
                    return a.IndexOf(t);
                var b = _JToken as JObject;
                int index = 0;
                foreach (var item in b.Properties())
                {
                    if (item == t)
                        return index;
                    index++;
                }
            }
            return -1;
        }
        public void Insert(int index, object value)
        {
            var t = (value as WFJson)._JToken;
            if (_JToken is JArray a)
            {
                a.Insert(index, t);
                return;
            }
            if (_JToken is JObject b)
            {
                int i = 0;
                foreach (var item in b.Properties())
                {
                    if (i == index)
                    {
                        item.AddBeforeSelf(t);
                        break;
                    }
                    i++;
                }
            }
        }
        public void Remove(object value)
        {
            var t = (value as WFJson)._JToken;
            if (_JToken is JArray a)
            {
                a.Remove(t);
                return;
            }
            if (_JToken is JObject b)
            {
                foreach (var item in b.Properties())
                {
                    if (item == t)
                    {
                        item.Remove();
                        break;
                    }
                }
            }
        }

        public void RemoveAt(int index)
        {
            if (_JToken is JArray a)
            {
                a.RemoveAt(index);
                return;
            }
            if (_JToken is JObject b)
            {
                int i = 0;
                foreach (var item in b.Properties())
                {
                    if (i == index)
                    {
                        item.Remove();
                        break;
                    }
                    i++;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (_JToken is JArray a)
            {
                Array.Copy(a.ToArray(), array, index);
                return;
            }
            if (_JToken is JObject b)
            {
                Array.Copy(b.Properties().ToArray(), array, index);
                return;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                if (_JToken is JArray a)
                    return a.Count;
                if (_JToken is JObject b)
                    return b.Properties().Count();
                return 0;

            }
        }
        object _SyncRoot = new object();
        public object SyncRoot
        {
            get
            {
                return _SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public ICollection Keys
        {
            get
            {
                if (_JToken is JObject a)//TODO: 代码未完成，需要继续处理
                    return a.Properties().ToArray();
                return new List<string>();
            }
        }
        public ICollection Values
        {
            get
            {
                var a = _JToken as JContainer;
                return a.Values().Cast<WFJson>().ToArray();
            }
        }
        public object this[object key]
        {
            get
            {
                if (key is string s)
                    return this[s];
                if (key is int i)
                    return this[i];
                return null;
            }
        }
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = new WFJson(new JValue(value));
            }
        }

        #endregion


        #region implicit operator
        public static implicit operator WFJson(JToken value)
        {
            return new WFJson(value);
        }
        public static implicit operator WFJson(bool value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(bool? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(DateTimeOffset value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(byte value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(byte? value)
        {
            return new WFJson(new JValue(value));
        }

        public static implicit operator WFJson(sbyte value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(sbyte? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(long value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(DateTime? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(DateTimeOffset? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(decimal? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(double? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(short value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(ushort value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(int value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(int? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(DateTime value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(long? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(float? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(decimal value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(short? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(ushort? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(uint? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(ulong? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(double value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(float value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(string value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(uint value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(ulong value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(byte[] value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(Uri value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(TimeSpan value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(TimeSpan? value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(Guid value)
        {
            return new WFJson(new JValue(value));
        }
        public static implicit operator WFJson(Guid? value)
        {
            return new WFJson(new JValue(value));
        }
        #endregion

        void SetValue(string path, JToken v)
        {
            var t = _JToken.SelectToken(path);
            var lastDot = path.LastIndexOf('.');
            var name = path.Substring(lastDot + 1);
            if (t == null)
            {
                //如果 name 以 ] 结尾应该是 Array
                if (name.EndsWith("]", StringComparison.InvariantCultureIgnoreCase))
                {
                    name = name.TrimEnd("[0]".ToCharArray());
                    t = new JArray(v);
                    t = new JProperty(name, t);
                    t = new JObject(t);

                }
                else
                {
                    t = new JProperty(name, v);
                    t = new JObject(t);
                }
                //_JToken 是 JValue 类型，没有Prpoerty，需要重新创建_JToken
                if (lastDot < 0)
                {
                    _JToken = t;
                }
                else
                {
                    var newPath = path.Remove(lastDot);
                    SetValue(newPath, t);
                }
            }
            else
            {
                t.Replace(v);
            }
        }
        public override string ToString()
        {
            return _JToken.ToString(Newtonsoft.Json.Formatting.None);
        }
    }
}
