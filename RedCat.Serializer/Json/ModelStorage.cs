using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedCat.Serializer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace RedCat.Serializer.Json
{
    public sealed class ModelStorage<T> : IUniqueStorage<T>, IModelStorage<T>
        where T : class, new()
    {
        #region Data
        private string Key
        {
            get;
            set;
        }
        private HashSet<T> Items
        {
            get;
            set;
        }

        private DateTime _lastChange;
        public DateTime LastChange
        {
            get { return _lastChange; }
            private set
            { _lastChange = value; }
        }

        #endregion
        #region Singleton

        private static ModelStorage<T> _instance = null;
        private static ModelStorage<T> CreateTable()
        {
            return _instance == null ? new ModelStorage<T>() : _instance;
        }

        private static async Task<ModelStorage<T>> GetTable()
        {
            if (TableExist<T>() == false)
            {
                return CreateTable();
            }
            else
            {
                ModelStorage<T> storage = new ModelStorage<T>();
                await storage.ReadModels();
                return storage;
            }
        }

        public static async Task<ModelStorage<T>> Instance()
        {
            return _instance == null ? await GetTable() : _instance;
        }

        public static bool TableExist<T>()
        {
            return ApplicationData.Current.LocalSettings.Values.Keys.Contains(typeof(T).Name);
        }


        public static async Task DropTable<T>()
        {
            if (TableExist<T>() == true)
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("RedCat", CreationCollisionOption.OpenIfExists);
                StorageFile file = await folder.GetFileAsync(ApplicationData.Current.LocalSettings.Values[typeof(T).Name].ToString());
                await file.DeleteAsync();
            }
        }

        #endregion

        private ModelStorage()
        {
            Key = Md5.CREATEUNIQUEHASH();
            Items = new HashSet<T>();
        }

        #region WorkDataMetod
        public void Insert(T item)
        {
            if (item != null)
                    Items.Add(item);
            else
                throw new NullReferenceException();
        }
        public void Insert(T item, ModelComparer<T> compare)
        {
            if (item != null && compare != null)
            {
                if (Items.Contains<T>(item, compare) == false)
                    Items.Add(item);
            }
            else
                throw new NullReferenceException();
        }
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }
        public T SelectOne(Func<T, bool> predicate)
        {
           return Items.Where(predicate).FirstOrDefault();
        }
        public List<T> SelectMany(Func<T, bool> predicate)
        {
            return Items.Where(predicate).ToList<T>();
        }
        public void RemoveOne(T item)
        {
            Items.Remove(item);
        }
        public int RemoveWhere(Predicate<T> predicate)
        {
            return Items.RemoveWhere(predicate);
        }
        public bool Contains(T item, ModelComparer<T> compare)
        {
            return Items.Contains<T>(item, compare);
        }
        public async Task SaveChanges()
        {
            StorageFolder folder = await GetRadCatFolder();
            StorageFile file = await folder.CreateFileAsync(GetFileName, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(new { key = Key, items = Items, LastChange = DateTime.Now }));
            ApplicationData.Current.LocalSettings.Values[typeof(T).Name] = GetFileName;
        }
        public List<T> SelectAll()
        {
            return Items.ToList();
        }
        #endregion

        private string GetFileName
        {
            get
            {
                return Key + ".json";
            }
        }

        private async Task<StorageFolder> GetRadCatFolder()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("RedCat", CreationCollisionOption.OpenIfExists);
        }

        private async Task ReadModels()
        {
            StorageFolder folder = await GetRadCatFolder();
            StorageFile file = await folder.GetFileAsync(ApplicationData.Current.LocalSettings.Values[typeof(T).Name].ToString());
            string read = await FileIO.ReadTextAsync(file);
            JObject json = JObject.Parse(read);
            Key = json["key"].ToString();
            Items = JsonConvert.DeserializeObject<HashSet<T>>(json["items"].ToString());
            LastChange = DateTime.Parse( json["LastChange"].ToString());
        }
    }
}
