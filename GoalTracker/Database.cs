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
            List<DisplayEntry> displayEntires = new List<DisplayEntry>();
            List<Category> Categories = await _database.Table<Category>().OrderBy(cat => cat.Units).ThenBy(cat => cat.Name).ToListAsync();
            foreach (Category category in Categories)
            {
                BasicEntry entry = await _database.Table<BasicEntry>().Where(curEntry => curEntry.Date == date && !curEntry.IsGoal && curEntry.CategoryName == category.Name).FirstOrDefaultAsync();
                if (entry == null)
                {
                    entry = new BasicEntry { Date = DateTime.Today, Quantity = 0, Id = 0 };
                }
                DisplayEntry displayEntry = new DisplayEntry
                {
                    CategoryName = category.Name,
                    Quantity = entry.Quantity,
                    Units = category.Units,
                    Id = entry.Id,
                    Goal = category.TargetQuantity
                };
                displayEntires.Add(displayEntry);
            }

            return displayEntires;
        }
        public async Task<int> SaveEntryAsync(BasicEntry entry)
        {
            BasicEntry existingEntry = await _database.Table<BasicEntry>()
                .Where(possibleMatch => (possibleMatch.IsGoal == entry.IsGoal && possibleMatch.Date == entry.Date && possibleMatch.CategoryName == entry.CategoryName))
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

        internal Task<List<BasicEntry>> getTrendEntries(string categoryName, bool isGoal, DateTime startDate, DateTime endDate)
        {
            return _database
                .Table<BasicEntry>()
                .Where(entry => entry.CategoryName == categoryName && entry.IsGoal == isGoal && entry.Date >= startDate && entry.Date <= endDate)
                .OrderBy(entry => entry.Date)
                .ToListAsync();
        }


        internal Task<BasicEntry> FetchPriorGoalEntry(string categoryName, DateTime curDate)
        {
            return _database.Table<BasicEntry>().Where(entry => entry.Date < curDate && entry.CategoryName == categoryName && entry.IsGoal == true).OrderByDescending(entry => entry.Date).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            Category existingCategory = await _database.Table<Category>().Where(cat => cat.Name == category.Name).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                return 0;
            }
            var temp = await _database.InsertAsync(category);
            await AddGoalEntry(category, DateTime.Today);
            return temp;
        }

        internal async Task<int> UpdateCategoryAsync(Category goal)
        {
            await AddGoalEntry(goal, DateTime.Today);
            return await _database.UpdateAsync(goal);
        }


        internal async Task<int> DeleteCategoryAsync(string goalName)
        {
            List<BasicEntry> matchingEntries = await _database.Table<BasicEntry>().Where(entry => entry.CategoryName == goalName).ToListAsync();
            List<Task> listOfTasks = new List<Task>();

            foreach (BasicEntry entry in matchingEntries)
            {
                listOfTasks.Add(_database.DeleteAsync(entry));
            }

            await Task.WhenAll(listOfTasks);
            return await _database.DeleteAsync<Category>(goalName);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().OrderBy(category => category.Name).ToListAsync();
        }

        private Task AddGoalEntry(Category category, DateTime entryDate)
        {
            BasicEntry newEntry = new BasicEntry { CategoryName = category.Name, Date = entryDate, IsGoal = true, Quantity = category.TargetQuantity };
            return SaveEntryAsync(newEntry);
        }

    }
}