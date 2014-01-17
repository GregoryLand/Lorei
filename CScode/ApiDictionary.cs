using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Lorei.CScode
{
    //Added to hold the apps and make it easier to pass them. 
    public class ApiDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public void Add(TKey key, TValue value)
        {
            m_logger.Info(value.GetType().Name + " Added to Dictionary");
            m_dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return m_dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return m_dictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            m_logger.Info(Values.GetType().Name + " Removed from Dictionary");
            return m_dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return m_dictionary.Values; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            m_logger.Info(item.GetType().Name + " Added to Dictionary");
            ((IDictionary<TKey, TValue>)m_dictionary).Add(item);
        }

        public void Clear()
        {
            m_dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)m_dictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>)m_dictionary).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)m_dictionary).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)m_dictionary).GetEnumerator();
        }

        public TValue this[TKey key] 
        {
            get
            {
                m_logger.Info( m_dictionary[key].GetType().Name + " Key was gotten from ApiDictionary");
                return m_dictionary[key];
            }
            set
            {
                m_logger.Info(value.GetType().Name + " was set in the collection");
                m_dictionary[key] = value;
            }
        }

        // Data
        private log4net.ILog m_logger = log4net.LogManager.GetLogger(typeof(ApiDictionary<TKey, TValue>));
        private Dictionary<TKey, TValue> m_dictionary = new Dictionary<TKey,TValue>();
    }
}
