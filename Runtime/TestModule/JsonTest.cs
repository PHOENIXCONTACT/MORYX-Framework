using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Model;
using Marvin.Modules.ModulePlugins;
using Marvin.TestTools.Test.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marvin.TestModule
{
    [Plugin(LifeCycle.Singleton)]
    public class JsonTest : IPlugin
    {
        public IUnitOfWorkFactory TestFactory { get; set; }

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
                        Converters = new[] {new StringEnumConverter()}
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
                LogLevel = LogLevel.Info,
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
                Plugins = new List<TestPluginConfig>
                {
                    new TestPluginConfig2
                    {
                      PluginBoolValue  = true,
                      PluginDoubleValue = 10.5,
                      PluginStringValue = "owgegvuqwvqwe",
                      PluginEnumValue = ConfigEnumeration.Value2,
                      PluginIntegerValue = 7505,
                      PluginLongValue = 18364752513
                    },
                    new TestPluginConfig1
                    {
                      PluginBoolValue  = true,
                      PluginDoubleValue = 10.5,
                      PluginStringValue = "owgegvuqwvqwe",
                      PluginEnumValue = ConfigEnumeration.Value2,
                      PluginIntegerValue = 7505,
                      PluginLongValue = 18364752513
                    },
                }
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
            using (var uow = TestFactory.Create(ContextMode.Tracking))
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
            using (var uow = TestFactory.Create(ContextMode.AllOff))
            {
                stopWatch.Restart();
                foreach (var id in ids)
                {
                    var result = ReadLoop(uow, id, jsonSettings);
                }
                stopWatch.Stop();
            }
            var readTime = stopWatch.ElapsedMilliseconds;

            return new[] { writeTime, readTime };
        }

        private long WriteLoop(IUnitOfWork uow, ModuleConfig config, JsonSerializerSettings settings)
        {
            var json = JsonConvert.SerializeObject(config, typeof(IConfig), settings);
            var entity = uow.GetRepository<IJsonTesterRepository>().Create(json);
            uow.Save();
            return entity.Id;
        }

        private IConfig ReadLoop(IUnitOfWork uow, long id, JsonSerializerSettings settings)
        {
            var json = (from e in uow.GetRepository<IJsonTesterRepository>().Linq
                        where e.Id == id
                        select e.JsonData).FirstOrDefault();
            return JsonConvert.DeserializeObject<IConfig>(json, settings);
        }
    }
}