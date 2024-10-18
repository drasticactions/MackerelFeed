// <copyright file="DatabaseTests.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using MackerelFeed.Models;
using MackerelFeed.Services;

namespace MackerelFeed.Tests;

/// <summary>
/// Tests the database.
/// </summary>
[TestClass]
public sealed class DatabaseTests
{
    private readonly TestAppDispatcher appDispatcher = new();
    private readonly TestErrorHandler errorHandler = new();

    /// <summary>
    /// Test the Database.
    /// </summary>
    /// <returns>Task.</returns>
    [TestMethod]
    public async Task FeedListItemTest()
    {
        var connectionString = "FeedListItemTest.db3";
        if (File.Exists(connectionString))
        {
            File.Delete(connectionString);
        }

        var database = new DatabaseService(connectionString, this.errorHandler);
        Assert.IsFalse(database.IsInitialized);
        var result = await database.InitializeAsync();
        Assert.IsTrue(result);
        using var client = new HttpClient();
        var rssFeedService = new RssFeedService(this.errorHandler, client);
        var rssFile = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "rss", "rss.xml"));
        var (feedItem, feedItemList) = await rssFeedService.ReadFeedAsync(rssFile, new FeedListItem() { Uri = new Uri("https://ascii.jp/rss.xml") });
        Assert.IsNotNull(feedItem);
        Assert.IsNotNull(feedItemList);
        Assert.IsTrue(feedItemList.Any());
        var rows = await database.UpsertFeedListItemAsync(feedItem, feedItemList);
        var total = feedItemList.Count + 1;
        Assert.IsTrue(rows == total);
        rows = await database.UpsertFeedListItemAsync(feedItem, feedItemList);
        Assert.IsTrue(rows == total);
        var items = await database.GetUnsortedFeedListItemsAsync();
        Assert.IsNotNull(items);
        Assert.IsTrue(items.Any());
        var item = items.FirstOrDefault();
        Assert.IsNotNull(item);
        var feedItems = await database.GetFeedItemsAsync(item.Id);
        Assert.IsNotNull(feedItems);
        Assert.IsTrue(feedItems.Any());
    }

    /// <summary>
    /// Test App Settings.
    /// </summary>
    /// <returns>Task.</returns>
    [TestMethod]
    public async Task AppSettingTest()
    {
        var connectionString = "AppSettingsTest.db3";
        if (File.Exists(connectionString))
        {
            File.Delete(connectionString);
        }

        var database = new DatabaseService(connectionString, this.errorHandler);
        Assert.IsFalse(database.IsInitialized);
        var result = await database.InitializeAsync();
        Assert.IsTrue(result);
        var appSettings = await database.GetAppSettingsAsync();
        Assert.IsNotNull(appSettings);
        Assert.IsTrue(appSettings.Id > 0);

        appSettings.AppTheme = AppTheme.Dark;
        appSettings.LanguageSetting = LanguageSetting.English;
        var updateResult = await database.UpdateAppSettingsAsync(appSettings);
        Assert.IsTrue(updateResult);
        var updatedAppSettings = await database.GetAppSettingsAsync();
        Assert.IsNotNull(updatedAppSettings);
        Assert.IsTrue(updatedAppSettings.Id > 0);
        Assert.AreEqual(appSettings.AppTheme, updatedAppSettings.AppTheme);
        Assert.AreEqual(appSettings.LanguageSetting, updatedAppSettings.LanguageSetting);
    }
}