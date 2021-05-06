using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoalTracker.Models;
using Microcharts;
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

            //_database.DeleteAllAsync<BasicEntry>();
            //_database.DeleteAllAsync<Category>();
        }

        public async Task<List<DisplayEntry>> GetDisplyEntriesForDateAsync(DateTime date)
        {
            List<BasicEntry> basicEntries = await _database.Table<BasicEntry>().Where(entry => entry.Date == date && !entry.IsGoal).ToListAsync();
            List<DisplayEntry> displayEntires = new List<DisplayEntry>();
            foreach (BasicEntry entry in basicEntries)
            {
                Category category = await _database.Table<Category>().Where(cat => cat.Id == entry.CategoryId).FirstOrDefaultAsync();
                DisplayEntry displayEntry = new DisplayEntry { CategoryName = category.Name, Date = entry.Date, Quantity = entry.Quantity, Units = category.Units, Id = entry.Id };
                displayEntires.Add(displayEntry);
            }

            return displayEntires;
        }
        public async Task<int> SaveEntryAsync(BasicEntry entry)
        {
            BasicEntry existingEntry = await _database.Table<BasicEntry>()
                .Where(possibleMatch => (!possibleMatch.IsGoal && possibleMatch.Date == entry.Date && possibleMatch.CategoryId == entry.CategoryId))
                .FirstOrDefaultAsync();
            if (existingEntry != null)
            {
                existingEntry.Quantity = existingEntry.Quantity + entry.Quantity;
                return await _database.UpdateAsync(existingEntry);
            }
            return await _database.InsertAsync(entry);
        }

        public Task<int> DeleteEntryAsync(int entryId)
        {
            return _database.DeleteAsync<BasicEntry>(entryId);
        }

        internal async Task<List<BasicEntry>> getTrendEntries(string categoryName, bool isGoal, DateTime startDate, DateTime endDate)
        {
            Category category = await _database.Table<Category>().Where(cat => cat.Name == categoryName).FirstOrDefaultAsync();
            if (category == null) return new List<BasicEntry>();
            return await _database
                .Table<BasicEntry>()
                .Where(entry => entry.CategoryId == category.Id && entry.IsGoal == isGoal && entry.Date >= startDate && entry.Date <= endDate)
                .OrderBy(entry => entry.Date)
                .ToListAsync();
        }


        internal async Task<BasicEntry> FetchPriorGoalEntry(string categoryName, DateTime curDate)
        {
            Category category = await _database.Table<Category>().Where(cat => cat.Name == categoryName).FirstOrDefaultAsync();
            if (category == null) return null;
            return await _database.Table<BasicEntry>().Where(entry => entry.Date < curDate && entry.CategoryId == category.Id && entry.IsGoal == true).OrderByDescending(entry => entry.Date).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            var temp = await _database.InsertAsync(category);
            await AddGoalEntry(category);
            return temp;
        }

        internal async Task<int> UpdateCategoryAsync(Category goal)
        {
            await AddGoalEntry(goal);
            return await _database.UpdateAsync(goal);
        }


        internal async Task<int> DeleteCategoryAsync(int goalId)
        {
            List<BasicEntry> matchingEntries = await _database.Table<BasicEntry>().Where(entry => entry.CategoryId == goalId).ToListAsync();
            List<Task> listOfTasks = new List<Task>();

            foreach (BasicEntry entry in matchingEntries)
            {
                listOfTasks.Add(_database.DeleteAsync(entry));
            }

            await Task.WhenAll(listOfTasks);
            return await _database.DeleteAsync<Category>(goalId);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }

        private Task AddGoalEntry(Category category)
        {
            BasicEntry newEntry = new BasicEntry { CategoryId = category.Id, Date = DateTime.Today, IsGoal = true, Quantity = category.TargetQuantity };
            return SaveEntryAsync(newEntry);
        }

    }
}