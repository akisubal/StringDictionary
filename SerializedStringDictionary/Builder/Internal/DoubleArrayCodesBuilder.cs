using SerializedStringDictionary.Internal;

namespace SerializedStringDictionary.Builder.Internal
{
    /// <summary>
    /// 文字に対して配列をどれだけ進めるかの表を作成する
    /// </summary>
    internal class DoubleArrayCodesBuilder
    {
        /// <summary>
        /// 常に真のbit
        /// </summary>
        private char m_always_true_bit;

        /// <summary>
        /// 常に0のbit
        /// </summary>
        private char m_always_false_bits;


        /// <summary>
        /// 初期化
        /// </summary>
        public DoubleArrayCodesBuilder()
        {
            m_always_true_bit = '\xFFFF';
            m_always_false_bits = '\xFFFF';
        }

        /// <summary>
        /// 受け取りうる文字の登録
        /// </summary>
        /// <param name="str"></param>
        public void Add(string str)
        {
            foreach (var c in str)
            {
                m_always_true_bit &= c;
                m_always_false_bits &= (char)(~c);
            }
        }

        /// <summary>
        /// 表の作成
        /// </summary>
        /// <returns></returns>
        public DoubleArrayCodes Build()
        {
            return new DoubleArrayCodes(m_always_true_bit, m_always_false_bits);
        }
    }

}
