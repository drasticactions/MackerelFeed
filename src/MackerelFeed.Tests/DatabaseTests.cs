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