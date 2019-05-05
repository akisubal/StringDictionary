using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StringDictionary.Builder.Internal
{
    /// <summary>
    /// 文字を値とするトライ木
    /// </summary>
    internal class Trie
    {
        /// <summary>
        /// トライ木のノード
        /// </summary>
        public class TrieNode
        {
            /// <summary>
            /// ノードごとguid
            /// </summary>
            /// 
            public Guid Guid
            {
                get => m_guid;
            }
            private Guid m_guid;

            /// <summary>
            /// 末端の場合、登録された文字列が何番目かのindex
            /// </summary>
            public int? Index
            {
                get => m_index;
                set => m_index = value;
            }
            private int? m_index;


            /// <summary>
            /// このノードに進むための文字
            /// </summary>
            public char Value
            {
                get => m_value;
                set => m_value = value;
            }
            private char m_value;


            /// <summary>
            /// 子ノード列
            /// </summary>
            public List<TrieNode> Children
            {
                get => m_children;
                set => m_children = value;
            }
            private List<TrieNode> m_children;



            /// <summary>
            /// 生成
            /// </summary>
            /// <param name="value"></param>
            public TrieNode(char value)
            {
                m_guid = Guid.NewGuid();
                m_index = null;
                m_value = value;
                m_children = new List<TrieNode>();
            }
        }

        /// <summary>
        /// ルートノード
        /// </summary>
        private TrieNode m_root;

        /// <summary>
        /// 登録された文字列数
        /// </summary>
        private int m_count;

        /// <summary>
        /// 構築
        /// </summary>
        public Trie()
        {
            m_root = new TrieNode('\x0');
            m_count = 0;
        }


        /// <summary>
        /// 文字列の追加
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            var value_listed = new List<char>(value.ToCharArray());
            value_listed.Add('\x0000');
            add(m_root, value_listed);
        }

        /// <summary>
        /// 文字列を検索
        /// </summary>
        /// <param name="value">検索する文字列</param>
        /// <returns>登録されていればそのindex なければnull</returns>
        public int? Find(string value)
        {
            if (m_root == null) { return null; }
            var value_listed = new List<char>(value.ToCharArray());
            value_listed.Add('\x0000');
            return find(m_root, value_listed);
        }

        /// <summary>
        /// 深さ優先探索
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TrieNode> GetEnumerator()
        {
            foreach (var n in getEnemerator(m_root)) { yield return n; }
        }

        /// <summary>
        /// 深さ優先探索
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<TrieNode> getEnemerator(TrieNode node)
        {
            yield return node;
            foreach (var child in node.Children)
            {
                foreach (var descendant in getEnemerator(child)) { yield return descendant; }
            }
        }


        /// <summary>
        /// ノードの追加
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        private void add(TrieNode node, List<char> value)
        {
            if (value.Count == 0)
            {
                node.Index = node.Index ?? m_count++;
                return;
            }
            var head = value[0];
            var next = node.Children.FirstOrDefault(x => x.Value == head);
            if (next == null)
            {
                next = new TrieNode(head);
                node.Children.Add(next);
            }

            value.RemoveAt(0);
            add(next, value);
        }


        /// <summary>
        /// 登録文字の検索
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int? find(TrieNode node, List<char> value)
        {
            if (value.Count == 0) { return null; }
            var head = value[0];
            var next = node.Children.FirstOrDefault(x => x.Value == head);
            if (next == null) { return null; }
            value.RemoveAt(0);
            if (value.Count == 0) { return next.Index; }


            return find(next, value);
        }
    }
}
