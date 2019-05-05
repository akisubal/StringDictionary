using System;
using System.Collections.Generic;
using System.Text;

namespace StringDictionary.Internal
{
    /// <summary>
    /// 文字に対して配列をどれだけ進めるかの表
    /// 0は利用しない(ステートが進めないため)
    /// 1は終端文字として利用するため
    /// 2を足して利用する
    /// </summary>
    [Serializable]
    public struct DoubleArrayCodes
    {
        /// <summary>
        /// 終了文字に割り当てるコード
        /// </summary>
        public const int TERMINATE_CODE = 1;

        /// <summary>
        /// 終端文字以外に割り当てるコードのオフセット
        /// </summary>
        private const int CODE_OFFSET = 2;

        /// <summary>
        /// 常に1となるbit部分
        /// </summary>
        private char m_always_true_bit;

        /// <summary>
        /// 常に0となるbit部分
        /// </summary>
        private char m_always_false_bits;

        /// <summary>
        /// 生成
        /// Builderを用いて作る
        /// </summary>
        /// <param name="always_true_bits"></param>
        /// <param name="always_false_bits"></param>
        public DoubleArrayCodes(char always_true_bits, char always_false_bits)
        {
            m_always_true_bit = always_true_bits;
            m_always_false_bits = always_false_bits;

        }

        /// <summary>
        /// 文字から進める数を取得
        /// </summary>
        /// <param name="c">受け取る文字</param>
        /// <returns>進める数 対応する文字がなければ-1</returns>
        public int this[char c]
        {
            get { return Get(c); }
        }

        /// <summary>
        /// 文字から進める数を取得
        /// </summary>
        /// <param name="c">受け取る文字</param>
        /// <returns>進める数 対応する文字がなければ-1</returns>
        public int Get(char c)
        {
            if (c == '\x0000') { return TERMINATE_CODE; }

            if ((m_always_true_bit & c) != m_always_true_bit) { return -1; }
            if ((m_always_false_bits & c) != 0) { return -1; }

            char target_bits = (char)(~m_always_true_bit & ~m_always_false_bits);
            int result = c & target_bits;

            return result + CODE_OFFSET;
        }
    }
}
