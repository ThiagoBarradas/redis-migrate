using Polly;
using RedisMigrate.Models;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static RedisMigrate.Program;

namespace RedisMigrate
{
    public class RedisMigrateProcessor : IDisposable
    {
        private RedisMigrateConfiguration Configuration { get; set; }

        private ConnectionMultiplexer RedisOrigin { get; set; }

        private ConnectionMultiplexer RedisDestination { get; set; }

        public RedisMigrateProcessor(RedisMigrateConfiguration configuration)
        {
            this.Configuration = configuration;
            this.RedisOrigin = ConnectionMultiplexer.Connect(configuration.OriginConnectionString);
            this.RedisDestination = ConnectionMultiplexer.Connect(configuration.DestinationConnectionString);
            ThreadPool.SetMinThreads(this.Configuration.MaxThreads, this.Configuration.MaxThreads);
        }

        public void Execute()
        {
            try
            {
                var originDb = this.RedisOrigin.GetDatabase(this.Configuration.OriginDatabase);
                var destinationDb = this.RedisDestination.GetDatabase(this.Configuration.DestinationDatabase);

                if (this.Configuration.OriginPopulateEnabled)
                {
                    Logger.LogLineWithLevel("INFO", "Populate {0} with Prefix {1}", this.Configuration.OriginPopulateQuantity, this.Configuration.OriginPopulatePrefix);
                    if (this.Configuration.OriginPopulatePrefix == null)
                    {
                        this.Configuration.OriginPopulatePrefix = "";
                    }

                    var ttl = TimeSpan.FromMinutes(30);

                    Parallel.For(0, this.Configuration.OriginPopulateQuantity, new ParallelOptions { MaxDegreeOfParallelism = this.Configuration.MaxThreads }, (i) =>
                    {
                        Logger.LogLineWithLevel("INFO", $"Writing test data {this.Configuration.OriginPopulatePrefix}key_x{i}");

                        Policy
                            .Handle<RedisTimeoutException>()
                            .RetryAsync(10, (err, i) => Task.Delay(50))
                            .ExecuteAsync(() =>
                            {
                                originDb.StringSet($"{this.Configuration.OriginPopulatePrefix}key_x{i}", $"test {i}", ttl);
                                return Task.CompletedTask;
                            });
                    });
                    
                    Logger.LogLineWithLevel("INFO", "Populate finished!");
                }

                var cursor = 0;
                var processed_success = 0;
                var processed_failed = 0;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    RedisResult result = null;

                    Policy.Handle<RedisTimeoutException>()
                          .RetryAsync(10)
                          .ExecuteAsync(() =>
                          {
                              result = originDb.Execute("scan", cursor, "MATCH", this.Configuration.OriginFilter, "COUNT", this.Configuration.MaxThreads);
                              return Task.CompletedTask;
                          });

                    var results = (RedisResult[])result;

                    cursor = (int)results[0];

                    var items = (string[])results[1];
                    Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = this.Configuration.MaxThreads }, (key) =>
                    {
                        var newKey = key;
                        if (!string.IsNullOrWhiteSpace(this.Configuration.DestinationKeyReplaceForEmpty))
                        {
                            newKey = newKey.Replace(this.Configuration.DestinationKeyReplaceForEmpty, "");
                        }

                        if (!string.IsNullOrWhiteSpace(this.Configuration.DestinationKeyPrefix))
                        {
                            newKey = $"{this.Configuration.DestinationKeyPrefix}{newKey}";
                        }

                        string value = null;
                        TimeSpan? ttl = null;
                        bool resultNew = false;

                        var flags = (this.Configuration.DestinationReplace)
                            ? When.Always
                            : When.NotExists;

                        Policy.Handle<RedisTimeoutException>()
                              .RetryAsync(10)
                              .ExecuteAsync(() =>
                              {
                                  value = originDb.StringGet(key);
                                  ttl = originDb.KeyTimeToLive(key);
                                  resultNew = destinationDb.StringSet(newKey, value, ttl, flags);
                                  return Task.CompletedTask;
                              });

                        Logger.LogLineWithLevel("INFO", "Origin Key: {0} \t Destination Key {1} \t Result: {2}", key, newKey, resultNew);

                        if (resultNew)
                        {
                            Interlocked.Increment(ref processed_success);
                        }
                        else
                        {
                            Interlocked.Increment(ref processed_failed);
                        }

                    });
                } 
                while (cursor != 0);

                stopwatch.Stop();

                Logger.LogLineWithLevel("INFO", "Total: {0} - Success: {1} - Failed {2}", processed_success + processed_failed, processed_success, processed_failed);
                Logger.LogLineWithLevel("INFO", "Duration is seconds: {0}", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception e)
            {
                Logger.LogLineWithLevel("ERROR", "Execute: An exception occurred");
                Logger.LogLineWithLevel("ERROR", "Message: {0}", e.Message);
            }
        }

        public void Dispose()
        {
            this.RedisOrigin.Dispose();
            this.RedisDestination.Dispose();
            Logger.LogLineWithLevel("INFO", "Redis Migrate Application Finish");
        }
    }
}

