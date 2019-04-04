using EbookApp.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EbookApp.Data
{
    public class DAL
    { 
        readonly SQLiteAsyncConnection database;

        public DAL(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<QuestionItem>().Wait();
        }

        public Task<List<QuestionItem>> GetItemsAsync()
        {
            return database.Table<QuestionItem>().ToListAsync();
        }

        public Task<List<QuestionItem>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<QuestionItem>("SELECT * FROM [QuestionItem]");
        }

        public Task<QuestionItem> GetItemAsync(string genre, string title)
        {
            return database.Table<QuestionItem>().Where(i => i.Genre == genre && i.Title == title).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(QuestionItem item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(QuestionItem item)
        {
            return database.DeleteAsync(item);
        }
    }
}
