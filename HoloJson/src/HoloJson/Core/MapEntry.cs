using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Core
{
    /// <summary>
    /// "Pair" class for Map.entry. </summary>
    /// <param> <K> </param>
    /// <param> <V> </param>
    public sealed class MapEntry<K, V> // : KeyValuePair<K, V>
    {
        private readonly K key;
        private V value;

        public MapEntry(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public K Key
        {
            get
            {
                return key;
            }
        }

        public V Value
        {
            get
            {
                return value;
            }
        }

        public V setValue(V value)
        {
            V old = this.value;
            this.value = value;
            return old;
        }

    }

}