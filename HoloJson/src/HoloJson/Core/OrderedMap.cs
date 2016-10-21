using HoloJson.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Core
{
    //// System.Collections.Specialized.OrderedDictionary is not supported on WinRT.
    //// Cf. https://msdn.microsoft.com/en-us/library/System.Collections.Specialized%28v=vs.110%29.aspx
    //// This is just a temporary substitute.
    //public class OrderedMap<K,V> : Dictionary<K,V>, IDictionary<K,V>    // , IList<K>
    //{
    //    // ????
    //    //private IList<KeyValuePair<K, V>> _map;

    //    // Ordered list of keys ???
    //    // private IList<K> _keyList;

    //    // ??? "Insertion time" keyed by the keys of the original dictionary.
    //    private IDictionary<K, long> _insertedTimeMap;
    //    // ....

    //    public OrderedMap()
    //    {
    //        // _keyList = new List<K>();
    //        _insertedTimeMap = new Dictionary<K, long>();
    //    }

    //    // tbd
    //    // There are a number of ways we can "fake" OrderedDictionary
    //    //   using Dictionary and List iemplementations
    //    //   (e.g., without fully implementing IOrderedDictionary from scratch)
    //    // The simplest solution seems to be just using Dictionary as a base class,
    //    //   and, we override all operations, for which ordering is significant,
    //    //   using an embedded list.
    //    // ...


    //    public override void Add(K key, V value)
    //    {
    //        base.Add(key, value);
    //        // _keyList.Add(key);
    //        // TBD:
    //        var now = DateTimeUtil.CurrentUnixEpochMillis();
    //        _insertedTimeMap.Add(key, now);
    //        // ....
    //    }
    //    public override bool Remove(K key)
    //    {
    //        var suc = base.Remove(key);
    //        if (suc) {
    //            // _keyList.Remove(key);
    //            _insertedTimeMap.Remove(key);
    //        }
    //        return suc;
    //    }
    //    public override void Clear()
    //    {
    //        base.Clear();
    //        // _keyList.Clear();
    //        _insertedTimeMap.Clear();
    //    }

    //    public override IEnumerator GetEnumerator()
    //    {
    //        // TBD:
    //        // This should be done based on the ordering of the _keyList.....
    //        return base.GetEnumerator();
    //    }

    //    public override KeyCollection Keys
    //    {
    //        get
    //        {
    //            // TBD:
    //            // This should be done based on the ordering of the _keyList.....
    //            // return base.Keys;

    //            var keyCollection = base.Keys;

    //            // ????
    //            // keyCollection = _keyList;

    //            // ???
    //            var sorted = keyCollection.OrderBy(o => _insertedTimeMap[o]);


    //            // ?????
    //            // return sorted;


    //            return keyCollection;
    //        }
    //    }



    //}

}
