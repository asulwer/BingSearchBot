using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

using SQLite;

using BingSearchBot.Model;

namespace BingSearchBot.DAO
{
    class SettingsDAO : IDisposable
    {
        public SettingsDAO()
        {
            FilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SettingsStorage.sqlite");
            dbConnection = new SQLiteAsyncConnection(FilePath);
        }
        public void Dispose()
        {
            FilePath = null;
            dbConnection = null;
        }

        public async Task CreateTable()
        {
            if (!CheckFileExists(FilePath))
                await dbConnection.CreateTableAsync<SettingsModel>();
        }
        public bool CheckFileExists(string fileName)
        {
            bool bFlag = true;

            try
            {
                bFlag = File.Exists(fileName);
            }
            catch
            {
                bFlag = false;
            }

            return bFlag;
        }
        public async Task<SettingsModel> Read(Guid id)
        {
            return (await dbConnection.QueryAsync<SettingsModel>("select * from SettingsModel where Id = '" + id + "'")).FirstOrDefault();
        }
        public async Task<ObservableCollection<SettingsModel>> ReadAll()
        {
            List<SettingsModel> myCollection = await dbConnection.Table<SettingsModel>().ToListAsync();
            return new ObservableCollection<SettingsModel>(myCollection);
        }
        public async Task Update(SettingsModel p)
        {
            if (p != null)
            {
                await dbConnection.RunInTransactionAsync((SQLiteConnection cn) =>
                {
                    cn.Update(p);
                });
            }
        }
        public async Task Insert(SettingsModel p)
        {
            await dbConnection.RunInTransactionAsync((SQLiteConnection cn) =>
            {
                cn.Insert(p);
            });
        }
        public async Task Delete(SettingsModel p)
        {
            var existingconact = (await dbConnection.QueryAsync<SettingsModel>("select * from SettingsModel where Id = '" + p.Id + "'")).FirstOrDefault();
            if (existingconact != null)
            {
                await dbConnection.RunInTransactionAsync((SQLiteConnection cn) =>
                {
                    cn.Delete(existingconact);
                });
            }
        }

        private SQLiteAsyncConnection dbConnection;
        private string FilePath;
    }
}