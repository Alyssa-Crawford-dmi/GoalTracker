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
                .Where(possibleMatch => (possibleMatch.IsGoal == entry.IsGoal && possibleMatch.Date == entry.Date && possibleMatch.CategoryId == entry.CategoryId))
                .FirstOrDefaultAsync();
            if (existingEntry != null)
            {
                if (entry.IsGoal)
                {
                    existingEntry.Quantity = entry.Quantity;
                }
                else
                {
                    existingEntry.Quantity = existingEntry.Quantity + entry.Quantity;
                }
                return await _database.UpdateAsync(existingEntry);
            }
            return await _database.InsertAsync(entry);
        }

        public Task<int> DeleteEntryAsync(int entryId)
        {
            return _database.DeleteAsync<BasicEntry>(entryId);
        }

        internal Task<List<BasicEntry>> getTrendEntries(int categoryId, bool isGoal, DateTime startDate, DateTime endDate)
        {
            return _database
                .Table<BasicEntry>()
                .Where(entry => entry.CategoryId == categoryId && entry.IsGoal == isGoal && entry.Date >= startDate && entry.Date <= endDate)
                .OrderBy(entry => entry.Date)
                .ToListAsync();
        }


        internal Task<BasicEntry> FetchPriorGoalEntry(int categoryId, DateTime curDate)
        {
            return _database.Table<BasicEntry>().Where(entry => entry.Date < curDate && entry.CategoryId == categoryId && entry.IsGoal == true).OrderByDescending(entry => entry.Date).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            var temp = await _database.InsertAsync(category);
            await AddGoalEntry(category, DateTime.Today);
            return temp;
        }

        internal async Task<int> UpdateCategoryAsync(Category goal)
        {
            await AddGoalEntry(goal, DateTime.Today);
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

        private Task AddGoalEntry(Category category, DateTime entryDate)
        {
            BasicEntry newEntry = new BasicEntry { CategoryId = category.Id, Date = entryDate, IsGoal = true, Quantity = category.TargetQuantity };
            return SaveEntryAsync(newEntry);
        }

    }
}