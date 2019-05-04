using System;
using System.Collections.Generic;
using System.Linq;
using SerializedStringDictionary.Internal;

namespace SerializedStringDictionary
{
    /// <summary>
    /// 直列化可能な文字をキーとする辞書
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializedStringDictionary<T>
    {
        /// <summary>
        /// キーからvalue配列のインデックスを検索するテーブル
        /// </summary>
        private DoubleArray m_double_array;

        /// <summary>
        /// valueの配列
        /// </summary>
        private T[] m_value;


        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="double_array">検索用double array</param>
        /// <param name="value">valueの配列</param>
        public SerializedStringDictionary(DoubleArray double_array, IEnumerable<T> value)
        {
            m_double_array = double_array;
            m_value = value.ToArray();
        }

        /// <summary>
        /// 値を取得
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>保存されたvalue キーに対応するものがなければInvalidOperationException</returns>
        public T this[string key]
        {
            get
            {
                int index = m_double_array.Find(key);
                if (index < 0) { throw new InvalidOperationException(); }
                if (m_value.Length <= index) { throw new InvalidOperationException(); }

                return m_value[index];
            }
        }

        /// <summary>
        /// キーに値が登録されていればそれを取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="result">値の返し先</param>
        /// <returns>キーに値が登録されてればtrue なければfalse</returns>
        public bool TryGet(string key, out T result)
        {
            int index = m_double_array.Find(key);
            if (index < 0) { result = default; return false; }
            result = m_value[index];
            return true;
        }

        /// <summary>
        /// キーが登録されていればtrue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return 0 <= m_double_array.Find(key);
        }
    }
}
