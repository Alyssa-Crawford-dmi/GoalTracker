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

            //_database.InsertAsync(new Category { Name = "Fake 1", TargetQuantity = 10, Units = "Reps" });
            //_database.InsertAsync(new Category { Name = "Fake 2", TargetQuantity = 10, Units = "Reps" });
            //_database.InsertAsync(new Category { Name = "Fake 3", TargetQuantity = 10, Units = "Reps" });
            //_database.InsertAsync(new Category { Name = "Fake 4", TargetQuantity = 10, Units = "Reps" });
        }

        public async Task<List<DisplayEntry>> GetDisplyEntriesForDateAsync(DateTime date)
        {
            List<BasicEntry> basicEntries = await _database.Table<BasicEntry>().Where(entry => entry.Date == date).ToListAsync();
            List<DisplayEntry> displayEntires = new List<DisplayEntry>();
            foreach (BasicEntry entry in basicEntries)
            {
                Category category = await _database.Table<Category>().Where(cat => cat.Id == entry.CategoryId).FirstOrDefaultAsync();
                DisplayEntry displayEntry = new DisplayEntry { CategoryName = category.Name, Date = entry.Date, Quantity = entry.Quantity, Units = category.Units, Id = entry.Id };
                displayEntires.Add(displayEntry);
            }

            return displayEntires;
        }
        public Task<int> SaveEntryAsync(BasicEntry entry)
        {
            return _database.InsertAsync(entry);
        }

        public Task<int> DeleteEntryAsync(int entryId)
        {
            return _database.DeleteAsync<BasicEntry>(entryId);
        }


        internal Task UpdateCategoryAsync(Category goal)
        {
            return _database.UpdateAsync(goal);
        }

        internal Task DeleteCategoryAsync(int goalId)
        {
            return _database.DeleteAsync<Category>(goalId);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }
    }
}