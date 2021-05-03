using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Models;
using SQLite;

namespace GoalTracker
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Category>().Wait();
            _database.CreateTableAsync<BasicEntry>().Wait();
        }

        public async Task<List<DisplayEntry>> GetDisplyEntriesForDateAsync()
        {
            // TODO make for date
            List<BasicEntry> basicEntries =  await _database.Table<BasicEntry>().ToListAsync();
            List<DisplayEntry> displayEntires = new List<DisplayEntry>();
            foreach(BasicEntry entry in basicEntries)
            {
                Category category = await _database.Table<Category>().Where(cat => cat.Id == entry.CategoryId).FirstOrDefaultAsync();
                DisplayEntry displayEntry = new DisplayEntry
                { CategoryName = category.Name, Date = entry.Date, Quantity = entry.Quantity, Units = category.Units, Id=entry.Id };
                displayEntires.Add(displayEntry);
            }

            return displayEntires;
        }

        public Task<int> SaveEntryAsync(BasicEntry entry)
        {
            return _database.InsertAsync(entry);
        }

        public async Task<int> DeleteEntryAsync(int entryId)
        {
            return await _database.DeleteAsync<BasicEntry>(entryId);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }
    }
}