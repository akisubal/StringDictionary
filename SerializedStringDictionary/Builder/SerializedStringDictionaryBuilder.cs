using System;
using System.Collections.Generic;
using SerializedStringDictionary.Internal;
using SerializedStringDictionary.Builder.Internal;
using System.Diagnostics;

namespace SerializedStringDictionary.Builder
{
    using BaseCheckArray = List<int>;

    /// <summary>
    /// 直列化可能な文字をキーとする辞書を作成する
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class SerializedStringDictionaryBuilder<T>
    {
        /// <summary>
        /// 作成する辞書と同じもの
        /// これを変換して直列化する
        /// </summary>
        private SortedDictionary<string, T> m_key_value = new SortedDictionary<string, T>();


        /// <summary>
        /// キーと値の登録
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public void Add(string key, T value)
        {
            m_key_value[key] = value;
        }

        /// <summary>
        /// 直列化した表を作成
        /// </summary>
        /// <returns></returns>
        public SerializedStringDictionary<T> Build()
        {
            var trie = new Trie();
            var codes_builder = new DoubleArrayCodesBuilder();
            foreach (var key_value in m_key_value)
            {
                trie.Add(key_value.Key);
                codes_builder.Add(key_value.Key);
            }


            var base_array = new BaseCheckArray() { 0, 1 };
            var check_array = new BaseCheckArray() { 0, 1 };
            var codes = codes_builder.Build();
            var state_dictionary = new Dictionary<Guid, int>();

            // 現在のステート
            int? current_state = null;

            foreach (var node in trie.GetEnumerator())
            {
                /// 一番最初なら初期値設定
                if (!current_state.HasValue)
                {
                    state_dictionary[node.Guid] = 1;
                }
                current_state = state_dictionary[node.Guid];



                /// baseの設定
                /// 終端なのでvalueのindexを反転して設定
                if ((node.Value == '\x0000') && node.Children.Count == 0)
                {
                    setToList(ref base_array, current_state.Value, -node.Index.Value);
                    continue;
                }



                /// 設定するbaseの検索 設定
                int current_base = getFromList(base_array, current_state.Value);
                bool is_colided = false;
                do
                {
                    is_colided = false;
                    foreach (var child in node.Children)
                    {
                        var code = codes[child.Value];
                        Debug.Assert(0 < code);
                        int next = current_base + code;
                        if (getFromList(check_array, next) != 0)
                        {
                            is_colided = true;
                            current_base += 1;
                            break;
                        }
                    }
                } while (is_colided);
                setToList(ref base_array, current_state.Value, current_base);

                /// 子の設定
                foreach (var child in node.Children)
                {
                    var code = codes[child.Value];
                    Debug.Assert(0 < code);
                    int next = current_base + code;

                    state_dictionary[child.Guid] = next;
                    if (getFromList(check_array, next) == 0)
                    {
                        // 未使用
                        setToList(ref check_array, next, current_state.Value);
                    }
                    else
                    {
                        // 衝突時処理
                        resolveCollision(current_state.Value, node, state_dictionary, codes, base_array, check_array);
                    }
                }
            }


            return new SerializedStringDictionary<T>(
                new DoubleArray(base_array, check_array, codes),
                m_key_value.Values); ;
        }


        /// <summary>
        /// double arrayの衝突の解決
        /// </summary>
        /// <param name="state"></param>
        /// <param name="node"></param>
        /// <param name="state_dictionary"></param>
        /// <param name="codes"></param>
        /// <param name="base_array"></param>
        /// <param name="check_array"></param>
        private void resolveCollision(int state, Trie.TrieNode node,
            Dictionary<Guid, int> state_dictionary, DoubleArrayCodes codes,
            BaseCheckArray base_array, BaseCheckArray check_array)
        {
            int old_base = base_array[state];
            int new_base = old_base;

            // 衝突しない新しいbaseを検索
            bool is_colided = false;
            do
            {
                if (is_colided) { ++new_base; }
                is_colided = false;
                foreach (var child in node.Children)
                {
                    int code = codes[child.Value];
                    Debug.Assert(0 < code);
                    int candidate = new_base + code;
                    if (check_array[candidate] != 0) { is_colided = true; break; }
                }
            } while (is_colided);



            // 新しいbaseに従って子の衝突の解消
            foreach (var child in node.Children)
            {
                var code = codes[child.Value];
                Debug.Assert(0 < code);
                int next = new_base + code;

                /// 新しい遷移先にcheckを設定
                state_dictionary[child.Guid] = next;
                setToList(ref check_array, next, state);

                int move_base = getFromList(base_array, old_base + code);
                setToList(ref base_array, next, move_base);

                if (move_base != 0)
                {
                    // 古い遷移先
                    int old_transit_pos = old_base + code;

                    // 古い遷移先から次に飛んだところ
                    for (int i = old_transit_pos; i < check_array.Count; ++i)
                    {
                        if (check_array[i] == old_transit_pos)
                        {
                            setToList(ref check_array, i, next);
                        }
                    }

                    /// 古い遷移先をリセット
                    setToList(ref base_array, old_transit_pos, 0);
                    setToList(ref check_array, old_transit_pos, 0);
                }
            }


            setToList(ref base_array, state, new_base);
        }


        /// <summary>
        /// リストから値を取得する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int getFromList(List<int> target, int index)
        {
            if (target == null) { return 0; }
            if (index < 0) { return 0; }
            if (target.Count <= index) { return 0; }

            return target[index];
        }


        /// <summary>
        /// リストから値を取得する
        /// リストサイズより添え数が大きい場合、リストをリサイズする
        /// </summary>
        /// <param name="target">編集対象リスト</param>
        /// <param name="index">編集先インデックス</param>
        /// <param name="value">設定する値</param>
        private static void setToList(ref List<int> target, int index, int value)
        {
            if (target == null) { return; }

            if (index <= 0) { return; }

            if (target.Count <= index)
            {
                for (int i = target.Count; i <= index; ++i)
                {
                    target.Add(0);
                }
            }

            target[index] = value;
        }
    }
}
