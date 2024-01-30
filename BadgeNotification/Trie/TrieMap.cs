using System;
using System.Collections.Generic;
using System.Text;

namespace Voidex.Trie
{
    /// <summary>
    /// TrieMap data structure.
    /// </summary>
    /// <typeparam name="TValue">Type of Value at each TrieNode.</typeparam>
    public class TrieMap<TValue> : ITrieMap<TValue>
    {
        #region data members

        /// <summary>
        /// Root TrieNode.
        /// </summary>
        private readonly TrieNode<TValue> rootTrieNode;

        #endregion

        #region ctors

        /// <summary>
        /// Create a new TrieMap instance.
        /// </summary>
        public TrieMap()
        {
            rootTrieNode = new TrieNode<TValue>(Const.ROOT);
        }

        #endregion

        #region ITrieMap<TValue>

        /// <summary>
        /// Gets TValue item for key from TrieMap.
        /// </summary>
        public TValue ValueBy(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var trieNode = GetTrieNode(key);
            return trieNode != null ? trieNode.Value : default(TValue);
        }

        /// <summary>
        /// Gets TValue items by key prefix from TrieMap.
        /// </summary>
        public IEnumerable<TValue> ValuesBy(string keyPrefix)
        {
            foreach (var value in
                     Traverse
                     (
                         GetTrieNode(keyPrefix),
                         new StringBuilder(keyPrefix),
                         (_, v) => v
                     ))
            {
                yield return value;
            }
        }

        /// <summary>
        /// Gets all TValue items from TrieMap.
        /// </summary>
        public IEnumerable<TValue> Values()
        {
            return ValuesBy(Const.ROOT);
        }

        /// <summary>
        /// Gets keys by key prefix from TrieMap.
        /// </summary>
        public IEnumerable<string> KeysBy(string keyPrefix)
        {
            var node = GetTrieNode(keyPrefix);
            var r = Traverse
            (
                node,
                new StringBuilder(keyPrefix),
                (kBuilder, v) => kBuilder.ToString()
            );

            foreach (var key in r)
            {
                yield return key;
            }
        }

        /// <summary>
        /// Gets all keys from TrieMap.
        /// </summary>
        public IEnumerable<string> Keys()
        {
            return KeysBy(Const.ROOT);
        }

        /// <summary>
        /// Gets string->TValue pairs by key prefix from TrieMap.
        /// </summary>
        public IEnumerable<KeyValuePair<string, TValue>> KeyValuePairsBy(string keyPrefix)
        {
            //if prefix is not Root
            foreach (var kvPair in
                     Traverse
                     (
                         GetTrieNode(keyPrefix),
                         new StringBuilder(keyPrefix),
                         (kBuilder, v) => new KeyValuePair<string, TValue>(kBuilder.ToString(), v)
                     ))
            {
                yield return kvPair;
            }
        }

        /// <summary>
        /// Gets all string->TValue pairs from TrieMap.
        /// </summary>
        public IEnumerable<KeyValuePair<string, TValue>> KeyValuePairs()
        {
            return KeyValuePairsBy("");
        }

        /// <summary>
        /// Adds TValue item for key to TrieMap.
        /// </summary>
        public void Add(string key, TValue value)
        {
            var trieNode = rootTrieNode;
            var words = key.Split(Const.SEPARATOR);
            foreach (var word in words)
            {
                var child = trieNode.GetChild(word);
                if (child == null)
                {
                    child = new TrieNode<TValue>(word);
                    trieNode.SetChild(child);
                }

                trieNode = child;
            }

            trieNode.Value = value;
        }

        /// <summary>
        /// Returns true if key present in TrieMap.
        /// </summary>
        public bool HasKey(string key)
        {
            return GetTrieNode(key)?.HasValue() ?? false;
        }

        /// <summary>
        /// Returns true if key prefix present in TrieMap.
        /// </summary>
        public bool HasKeyPrefix(string keyPrefix)
        {
            return GetTrieNode(keyPrefix) != null;
        }

        /// <summary>
        /// Gets the equivalent TrieNode in the TrieMap for given key prefix.
        /// If prefix not present, then returns null.
        /// </summary>
        public TrieNode<TValue> GetTrieNode(string keyPrefix)
        {
            return rootTrieNode.GetTrieNode(keyPrefix);
        }

        /// <summary>
        /// Removes key from TrieMap.
        /// </summary>
        public void Remove(string key)
        {
            var trieNode = GetTrieNode(key);
            if (!trieNode?.HasValue() ?? true)
            {
                throw new ArgumentOutOfRangeException($"{key} does not exist in trieMap.");
            }

            trieNode.Clear();
        }

        /// <summary>
        /// Updates value for key in TrieMap.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(string key, TValue value)
        {
            var trieNode = GetTrieNode(key);
            if (!trieNode?.HasValue() ?? true)
            {
                return false;
            }

            trieNode.Value = value;
            return true;
        }

        /// <summary>
        /// Removes key prefix from TrieMap and return true else false.
        /// </summary>
        public bool RemoveKeyPrefix(string keyPrefix)
        {
            var trieNode = GetTrieNode(keyPrefix);
            trieNode?.Clear();
            return trieNode != null;
        }

        /// <summary>
        /// Gets string->TValue pairs for longest keys from the Trie.
        /// </summary>
        public IEnumerable<KeyValuePair<string, TValue>> GetLongestKeyValuePairs()
        {
            var longestKeyValuePairs = new List<KeyValuePair<string, TValue>>();
            var buffer = new StringBuilder();
            var length = new Wrapper<int>(0);
            GetLongestKeyValuePairs(rootTrieNode, longestKeyValuePairs, buffer, length);
            return longestKeyValuePairs;
        }

        /// <summary>
        /// Gets string->TValue pairs for shortest keys from the Trie.
        /// </summary>
        public IEnumerable<KeyValuePair<string, TValue>> GetShortestKeyValuePairs()
        {
            var shortestKeyValuePairs = new List<KeyValuePair<string, TValue>>();
            var buffer = new StringBuilder();
            var length = new Wrapper<int>(int.MaxValue);
            GetShortestKeyValuePairs(rootTrieNode, shortestKeyValuePairs, buffer, length);
            return shortestKeyValuePairs;
        }
        

        /// <summary>
        /// Clears all values from TrieMap.
        /// </summary>
        public void Clear()
        {
            rootTrieNode.Clear();
        }

        /// <summary>
        /// Gets the root TrieNode of the TrieMap.
        /// </summary>
        public TrieNode<TValue> GetRootTrieNode()
        {
            return rootTrieNode;
        }

        #endregion

        #region private methods

        /// <summary>
        /// DFS traversal starting from given TrieNode and yield.
        /// </summary>
        private IEnumerable<TResult> Traverse<TResult>(TrieNode<TValue> trieNode,
            StringBuilder buffer, Func<StringBuilder, TValue, TResult> transform)
        {
            if (trieNode == null)
            {
                yield break;
            }


            if (trieNode.HasValue())
            {
                yield return transform(buffer, trieNode.Value);
            }

            var children = trieNode.GetChildren();
            foreach (var child in children)
            {
                int lengthBeforeChild = buffer.Length;

                if (!string.IsNullOrEmpty(child.Word))
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(Const.SEPARATOR);
                    }

                    buffer.Append(child.Word);
                }

                foreach (var item in Traverse(child, buffer, transform))
                {
                    yield return item;
                }

                // Reset the buffer to the state before this child
                buffer.Length = lengthBeforeChild;
            }
        }


        private void GetLongestKeyValuePairs(TrieNode<TValue> trieNode,
            ICollection<KeyValuePair<string, TValue>> longestKeyValuePairs, StringBuilder buffer, Wrapper<int> length)
        {
            if (trieNode.HasValue())
            {
                if (buffer.Length > length.Value)
                {
                    longestKeyValuePairs.Clear();
                    length.Value = buffer.Length;
                }

                if (buffer.Length == length.Value)
                {
                    longestKeyValuePairs.Add(new KeyValuePair<string, TValue>(buffer.ToString(), trieNode.Value));
                }
            }

            foreach (var child in trieNode.GetChildren())
            {
                var bufferLengthBeforeAppend = buffer.Length;
                buffer.Append(child.Word);
                GetLongestKeyValuePairs(child, longestKeyValuePairs, buffer, length);
                buffer.Length = bufferLengthBeforeAppend;
            }
        }

        private void GetShortestKeyValuePairs(TrieNode<TValue> trieNode,
            ICollection<KeyValuePair<string, TValue>> shortestKeyValuePairs, StringBuilder buffer, Wrapper<int> length)
        {
            if (trieNode.HasValue())
            {
                if (buffer.Length < length.Value)
                {
                    shortestKeyValuePairs.Clear();
                    length.Value = buffer.Length;
                }

                if (buffer.Length <= length.Value)
                {
                    shortestKeyValuePairs.Add(new KeyValuePair<string, TValue>(buffer.ToString(), trieNode.Value));
                }
            }

            foreach (var child in trieNode.GetChildren())
            {
                int bufferLengthBeforeAppend = buffer.Length;
                buffer.Append(child.Word);
                GetShortestKeyValuePairs(child, shortestKeyValuePairs, buffer, length);
                buffer.Length = bufferLengthBeforeAppend;
            }
        }

        #endregion
    }
}