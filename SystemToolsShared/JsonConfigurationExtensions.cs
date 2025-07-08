﻿using System;
using Microsoft.Extensions.Configuration;

namespace SystemToolsShared;

public static class JsonConfigurationExtensions
{
    public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path,
        bool optional, bool reloadOnChange, string key, string appSetEnKeysFileName)
    {
        if (builder is null) 
            throw new ArgumentNullException(nameof(builder));

        if (string.IsNullOrWhiteSpace(path)) 
            throw new ArgumentException("File path must be a non-empty string.");

        var source = new JsonConfigurationSource(null, path, optional, reloadOnChange, key, appSetEnKeysFileName);

        source.ResolveFileProvider();
        builder.Add(source);
        return builder;
    }
}