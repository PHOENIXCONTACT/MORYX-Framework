// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Modules;
using Moryx.TestTools.Test.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Moryx.TestModule;

[Plugin(LifeCycle.Singleton)]
public class JsonTest : IPlugin
{
    public IUnitOfWorkFactory<TestModelContext> TestFactory { get; set; }

    private const int LoopCount = 100;

    /// <inheritdoc />
    public void Start()
    {
        Console.WriteLine();
        Console.WriteLine("Starting JSON benchmark");

        var config = CreateConfig();

        // As allways run the jit compiler
        JitRun(config);

        // Prepare json settings for comparison
        var settings = new[]
        {
            new
            {
                Quality = "Readable",
                Json = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented,
                    Converters = [new StringEnumConverter()]
                }
            },
            new
            {
                Quality = "Minimal",
                Json = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                }
            }
        };

        // Compare size of both settings
        Console.WriteLine("Size comparison:");
        foreach (var setting in settings)
        {
            var size = JsonConvert.SerializeObject(config, setting.Json).Length;
            Console.WriteLine("{0,10}: {1:D4} characters", setting.Quality, size);
        }
        Console.WriteLine();

        // Run test loops
        Console.WriteLine("Times:");
        foreach (var setting in settings)
        {
            var averages = new long[] { 0, 0 };
            for (int i = 0; i < 20; i++)
            {
                var times = TestLoop(config, setting.Json);
                averages[0] += times[0];
                averages[1] += times[1];
            }
            Console.WriteLine("{0,10}: {1:D4}ms write and {2:D4}ms read", setting.Quality, averages[0] / 20, averages[1] / 20);

        }
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void Stop()
    {
    }

    private ModuleConfig CreateConfig()
    {
        var config = new ModuleConfig
        {
            BoolValue = true,
            ConfigState = ConfigState.Valid,
            DoubleValue = 126.251,
            EnumValue = ConfigEnumeration.Value2,
            IntegerValue = 12035,
            SleepTime = 100,
            LogLevel = LogLevel.Information,
            LongValue = 1281259787,
            StringValue = "hasojgwbngäqwbvqwvb",
            TestPlugin = new TestPluginConfig1
            {
                PluginBoolValue = true,
                PluginDoubleValue = 10.5,
                PluginStringValue = "owgegvuqwvqwe",
                PluginEnumValue = ConfigEnumeration.Value2,
                PluginIntegerValue = 7505,
                PluginLongValue = 18364752513
            },
            Plugins =
            [
                new TestPluginConfig2
                {
                    PluginBoolValue = true,
                    PluginDoubleValue = 10.5,
                    PluginStringValue = "owgegvuqwvqwe",
                    PluginEnumValue = ConfigEnumeration.Value2,
                    PluginIntegerValue = 7505,
                    PluginLongValue = 18364752513
                },

                new TestPluginConfig1
                {
                    PluginBoolValue = true,
                    PluginDoubleValue = 10.5,
                    PluginStringValue = "owgegvuqwvqwe",
                    PluginEnumValue = ConfigEnumeration.Value2,
                    PluginIntegerValue = 7505,
                    PluginLongValue = 18364752513
                }

            ]
        };
        return config;
    }

    private void JitRun(ModuleConfig config)
    {
        var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        TestLoop(config, jsonSettings);
    }

    private long[] TestLoop(ModuleConfig config, JsonSerializerSettings jsonSettings)
    {
        var stopWatch = new Stopwatch();
        var ids = new List<long>();

        // Write time
        using (var uow = TestFactory.Create())
        {
            stopWatch.Start();
            for (var i = 0; i < LoopCount; i++)
            {
                ids.Add(WriteLoop(uow, config, jsonSettings));
            }
            stopWatch.Stop();
        }
        var writeTime = stopWatch.ElapsedMilliseconds;

        // Read time
        using (var uow = TestFactory.Create())
        {
            stopWatch.Restart();
            foreach (var id in ids)
            {
                var result = ReadLoop(uow, id, jsonSettings);
            }
            stopWatch.Stop();
        }
        var readTime = stopWatch.ElapsedMilliseconds;

        return [writeTime, readTime];
    }

    private long WriteLoop(IUnitOfWork uow, ModuleConfig config, JsonSerializerSettings settings)
    {
        var json = JsonConvert.SerializeObject(config, typeof(ConfigBase), settings);
        var entity = uow.GetRepository<IJsonEntityRepository>().Create(json);
        uow.SaveChanges();
        return entity.Id;
    }

    private ConfigBase ReadLoop(IUnitOfWork uow, long id, JsonSerializerSettings settings)
    {
        var json = (from e in uow.GetRepository<IJsonEntityRepository>().Linq
            where e.Id == id
            select e.JsonData).FirstOrDefault();
        return JsonConvert.DeserializeObject<ConfigBase>(json, settings);
    }
}