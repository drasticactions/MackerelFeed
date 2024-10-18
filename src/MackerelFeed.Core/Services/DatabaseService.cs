// <copyright file="DatabaseService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.UI.Services;
using MackerelFeed.Events;
using MackerelFeed.Exceptions;
using MackerelFeed.Models;
using SQLite;

namespace MackerelFeed.Services;

/// <summary>
/// Database Service.
/// </summary>
public class DatabaseService : IDisposable
{
    private const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection database;
    private IErrorHandler errorHandler;

    private bool isInitialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="connectionString">Connection String.</param>
    /// <param name="errorHandler">Error Handler.</param>
    public DatabaseService(string connectionString, IErrorHandler errorHandler)
    {
        SQLitePCL.Batteries.Init();
        this.database = new SQLiteAsyncConnection(connectionString, Flags);
        this.errorHandler = errorHandler;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    ~DatabaseService()
    {
        this.ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Event for when a FeedListItem is updated.
    /// </summary>
    public event EventHandler<FeedListItemContentEventArgs>? OnFeedListItemUpdate;

    /// <summary>
    /// Event for when a FeedListItem is removed.
    /// </summary>
    public event EventHandler<FeedListItemContentEventArgs>? OnFeedListItemRemove;

    /// <summary>
    /// Event for when a FeedFolder is updated.
    /// </summary>
    public event EventHandler<FeedFolderContentEventArgs>? OnFeedFolderUpdate;

    /// <summary>
    /// Event for when a FeedFolder is removed.
    /// </summary>
    public event EventHandler<FeedFolderContentEventArgs>? OnFeedFolderRemove;

    /// <summary>
    /// Event for feed refresh.
    /// </summary>
    public event EventHandler? OnRefreshFeeds;

    /// <summary>
    /// Gets a value indicating whether the database is initialized.
    /// </summary>
    public bool IsInitialized => this.isInitialized;

    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <returns>Bool.</returns>
    public async Task<bool> InitializeAsync()
    {
        var createTablesResult = await this.database.CreateTablesAsync<AppSettings, FeedItem, FeedListItem, FeedFolder>(CreateFlags.None);
        this.isInitialized = createTablesResult.Results.Any(x => x.Value != CreateTableResult.Created && x.Value != CreateTableResult.Migrated) == false;
        return this.isInitialized;
    }

    /// <summary>
    /// Drop all tables.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task DropTablesAsync()
    {
        await this.database.DropTableAsync<AppSettings>();
        await this.database.DropTableAsync<FeedItem>();
        await this.database.DropTableAsync<FeedListItem>();
        await this.database.DropTableAsync<FeedFolder>();
        this.isInitialized = false;
    }

    public async Task<IEnumerable<FeedListItem>> GetFeedListItemsAsync()
    {
        if (!this.isInitialized)
        {
            return Array.Empty<FeedListItem>();
        }

        try
        {
            return await this.database.Table<FeedListItem>().ToListAsync();
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting FeedListItems.", ex));
            return Array.Empty<FeedListItem>();
        }
    }

    public async Task<IEnumerable<FeedListItem>> GetUnsortedFeedListItemsAsync()
    {
        if (!this.isInitialized)
        {
            return Array.Empty<FeedListItem>();
        }

        try
        {
            return await this.database.Table<FeedListItem>().Where(n => n.FolderId == null).ToListAsync();
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting FeedListItems.", ex));
            return Array.Empty<FeedListItem>();
        }
    }

    public async Task<FeedListItem?> GetFeedListItemAsync(string uriString)
    {
        if (!this.isInitialized)
        {
            return null;
        }

        if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
        {
            return null;
        }

        try
        {
            return await this.database.Table<FeedListItem>().FirstOrDefaultAsync(n => n.Uri == uri);
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting FeedListItem.", ex));
            return null;
        }
    }

    public async Task<FeedListItem?> GetFeedListItemAsync(int id)
    {
        if (!this.isInitialized)
        {
            return null;
        }

        try
        {
            return await this.database.Table<FeedListItem>().FirstOrDefaultAsync(n => n.Id == id);
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting FeedListItem.", ex));
            return null;
        }
    }

    public async Task<IEnumerable<FeedItem>> GetFeedItemsAsync(int feedListItemId)
    {
        if (!this.isInitialized)
        {
            return Array.Empty<FeedItem>();
        }

        try
        {
            return await this.database.Table<FeedItem>().Where(n => n.FeedListItemId == feedListItemId).ToListAsync();
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting FeedItems.", ex));
            return Array.Empty<FeedItem>();
        }
    }

    public async Task<int> UpsertFeedListItemAsync(FeedListItem item, IEnumerable<FeedItem>? feedItems = default)
    {
        if (!this.isInitialized)
        {
            return 0;
        }

        int rows = 0;

        try
        {
            var result = 0;
            if (item.Id <= 0)
            {
                result = await this.database.InsertAsync(item, typeof(FeedListItem));
            }
            else
            {
                result = await this.database.UpdateAsync(item, typeof(FeedListItem));
            }

            rows = result > 0 ? 1 : 0;
            if (feedItems is not null)
            {
                // Set the id of the FeedListItem on the FeedItems.
                foreach (var feedItem in feedItems)
                {
                    feedItem.FeedListItemId = item.Id;
                }

                var count = await this.UpsertFeedItemsAsync(feedItems);
                rows += count;
            }

            this.OnFeedListItemUpdate?.Invoke(this, new FeedListItemContentEventArgs(item));
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error upserting FeedListItem.", ex));
        }

        return rows;
    }

    public async Task<int> UpsertFeedItemsAsync(IEnumerable<FeedItem> items)
    {
        if (!this.isInitialized)
        {
            return 0;
        }

        int rows = 0;

        try
        {
            var missingFeedListItemIds = items.Any(x => x.FeedListItemId <= 0);
            if (missingFeedListItemIds)
            {
                throw new DatabaseException("FeedItem is missing a FeedListItemId.");
            }

            var itemsWithoutId = items.Where(x => x.Id <= 0);
            var itemsWithId = items.Where(x => x.Id > 0);

            var tasks = itemsWithoutId.Select(x => this.database.InsertAsync(x, typeof(FeedItem)));
            var tasks2 = itemsWithId.Select(x => this.database.UpdateAsync(x, typeof(FeedItem)));
            var totalTasks = tasks.Concat(tasks2);
            var results = await Task.WhenAll(totalTasks);
            rows = results.Count(x => x > 0);
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error upserting FeedItems.", ex));
        }

        return rows;
    }

    /// <summary>
    /// Get AppSettings.
    /// </summary>
    /// <returns><see cref="AppSettings"/>.</returns>
    public async Task<AppSettings?> GetAppSettingsAsync()
    {
        if (!this.isInitialized)
        {
            return null;
        }

        try
        {
            var settings = await this.database.Table<AppSettings>().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AppSettings();
                await this.database.InsertAsync(settings, typeof(AppSettings));
            }

            return settings;
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting AppSettings.", ex));
            return null;
        }
    }

    /// <summary>
    /// Update AppSettings.
    /// </summary>
    /// <param name="settings"><see cref="AppSettings"/>.</param>
    /// <returns>If the app settings were updated.</returns>
    public async Task<bool> UpdateAppSettingsAsync(AppSettings settings)
    {
        if (!this.isInitialized)
        {
            return false;
        }

        int rows = 0;

        try
        {
           rows = await this.database.UpdateAsync(settings, typeof(AppSettings));
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error updating AppSettings.", ex));
        }

        return rows > 0;
    }

    /// <summary>
    /// Dispose elements.
    /// </summary>
    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        this.database.CloseAsync();
    }
}