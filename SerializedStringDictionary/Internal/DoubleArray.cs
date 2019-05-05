using System;
using System.Collections.Generic;
using System.Linq;

namespace StringDictionary.Internal
{
    [Serializable]
    public class DoubleArray
    {

        /// <summary>
        /// base_value:  stateの記録配列
        /// check_value: stateから次のstateに遷移できるかの配列
        /// ２つの配列を合成
        /// </summary>
        [Serializable]
        private struct BaseCheck
        {
            public int Base { get => m_base_value; set => m_base_value = value; }
            public int m_base_value;

            public int Check { get => m_check_value; set => m_check_value = value; }
            public int m_check_value;
        }
        private BaseCheck[] m_base_check_array;


        /// <summary>
        /// 文字を受け取った時に進める距離の表
        /// </summary>
        private DoubleArrayCodes m_codes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base_array"></param>
        /// <param name="check_array"></param>
        /// <param name="codes"></param>
        public DoubleArray(IEnumerable<int> base_array, IEnumerable<int> check_array, DoubleArrayCodes codes)
        {
            m_base_check_array = base_array.Select((b, i) => new { b, i })
                .Join(
                    check_array.Select((c, i) => new { c, i }),
                    b_i => b_i.i,
                    c_i => c_i.i,
                    (b_i, c_i) =>
                        new BaseCheck() { m_base_value = b_i.b, m_check_value = c_i.c }
                ).ToArray();
            m_codes = codes;
        }


        /// <summary>
        /// 見つかればそのインデックス
        /// 見つからなければ-1
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int Find(string s)
        {
            var state = 1;
            foreach (var c in s)
            {
                state = transit(state, c);
                if (state < 0) { return -1; }
            }

            /// 終端文字の遷移
            state = transit(state, '\x0000');
            if (state < 0) { return -1; }


            int goal = m_base_check_array[state].Base;
            return (goal <= 0) ? -goal : -1;
        }

        /// <summary>
        /// 次のステートに遷移する
        /// 遷移先がなければ-1
        /// </summary>
        /// <param name="current_state"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private int transit(int current_state, char path)
        {
            var code = m_codes[path];
            if (code < 0) { return -1; }

            int next = m_base_check_array[current_state].Base + code;
            if (current_state != m_base_check_array[next].Check) { return -1; }

            return next;
        }
    }
}
