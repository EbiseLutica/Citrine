using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Citrine.Core.Api
{
    public class UserStorage
    {

        public UserStorage()
        {
            Reload();
        }

        /// <summary>
        /// 指定したユーザーのレコードを取得します。
        /// </summary>
        /// <value>指定したユーザーのレコード。存在しなければ新規作成したものを返します。</value>
		public UserRecord this[IUser user] 
        {
            get
            {
                lock (storage)
                {
                    return storage.ContainsKey(user.Id) ? storage[user.Id] : (storage[user.Id] = CreateRecord());
                }
            }
        }

        /// <summary>
        /// ストレージをファイルへ手動保存します。
        /// </summary>
		public void Save()
        {
            lock (fileLock)
                File.WriteAllText("./storage.json", SerializeStorage());
        }

        /// <summary>
        /// ストレージをファイルからリロードします。
        /// </summary>
		public void Reload()
        {
            if (File.Exists("./storage.json"))
                DeserializeStorage(File.ReadAllText("./storage.json"));
        }

        private string SerializeStorage()
        {
            lock (storage)
            {
                var obj = new Dictionary<string, Dictionary<string, object>>();
                foreach (var kv in storage)
                {
                    obj[kv.Key] = kv.Value.InternalRecord;
                }
                return JsonConvert.SerializeObject(obj);
            }
        }

        private void DeserializeStorage(string json)
        {
            var obj = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
            lock (storage)
            {
                foreach (var kv in obj)
                {
                    storage[kv.Key] = CreateRecord(kv.Value);
                }
            }
        }

        private UserRecord CreateRecord(Dictionary<string, object> r = null)
        {
            var rec = r == null ? new UserRecord() : new UserRecord(r);
            rec.Updated += () => Save();
            return rec;
        }

        private ConcurrentDictionary<string, UserRecord> storage = new ConcurrentDictionary<string, UserRecord>();

        private object fileLock = new object();

        public class UserRecord
        {
            public UserRecord() { }
            public UserRecord(Dictionary<string, object> r) => record = r;

            internal Dictionary<string, object> InternalRecord => record;

            public T Get<T>(string key, T defaultValue = default)
            {
                // キーに値がなければデフォルト値
                if (!Has(key))
                    return defaultValue;
                // 正しい型の値があれば返す　なければデフォルト
                return Is<T>(key) ? (T)record[key] : default;
            }

            public bool Has(string key) => record.ContainsKey(key);

            public bool Is<T>(string key) => Has(key) && record[key] is T;

            public void Set<T>(string key, T value)
            {
                // 同値が登録してあればreturn
                if (Has(key) && record[key] == (object)value)
                    return;
                record[key] = value;
                Updated?.Invoke();
            }

            public void Clear(string key)
            {
                if (!Has(key)) return;
                record.Remove(key);
                Updated?.Invoke();
            }

            public void ClearAll()
            {
                record.Clear();
                Updated?.Invoke();
            }

            public event Action Updated;

            private Dictionary<string, object> record = new Dictionary<string, object>();
        }
    }
}
