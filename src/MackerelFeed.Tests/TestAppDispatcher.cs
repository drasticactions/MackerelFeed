// <copyright file="TestAppDispatcher.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.UI.Services;

namespace MackerelFeed.Tests;

/// <summary>
/// Test App Dispatcher.
/// </summary>
public class TestAppDispatcher : IAppDispatcher
{
    /// <inheritdoc/>
    public bool Dispatch(Action action)
    {
        action();
        return true;
    }
}