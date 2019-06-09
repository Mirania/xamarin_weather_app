using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Weather
{
    class Repository
    {
        string conn_string
        {
            get { return DependencyService.Get<IFileHelper>().GetLocalFilePath("Entries.db3"); }
        }

        public bool CreateDatabase()
        {
            try
            {
                var conexao = new SQLiteAsyncConnection(conn_string);
                conexao.CreateTableAsync<Entry>().Wait();
                return true;
            }
            catch (SQLiteException ex)
            {
                //Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public bool InsertEntry(Entry entry)
        {
            try
            {
                var conexao = new SQLiteAsyncConnection(conn_string);
                conexao.InsertAsync(entry).Wait();
                return true;
                
            }
            catch (SQLiteException ex)
            {
                //Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<Entry> GetEntries()
        {
            try
            {
                var conexao = new SQLiteAsyncConnection(conn_string);
                return conexao.Table<Entry>().ToListAsync().Result;
            }
            catch (SQLiteException ex)
            {
                //Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool DeleteEntries()
        {
            try
            {
                var conexao = new SQLiteAsyncConnection(conn_string);
                conexao.DeleteAllAsync<Entry>().Wait();
                return true;
            }
            catch (Exception ex)
            {
                //Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
    }
}
