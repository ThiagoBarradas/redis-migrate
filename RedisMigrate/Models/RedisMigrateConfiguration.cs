using System;

namespace RedisMigrate.Models
{
    public class RedisMigrateConfiguration
    {
        public RedisMigrateConfiguration()
        {

        }

        public int MaxThreads { get; set; }

        public string OriginConnectionString { get; set; }

        public int OriginDatabase { get; set; }

        public string OriginFilter { get; set; }

        public bool OriginPopulateEnabled { get; set; }

        public int OriginPopulateQuantity { get; set; }

        public string OriginPopulatePrefix { get; set; }

        public string DestinationConnectionString { get; set; }

        public int DestinationDatabase { get; set; }

        public string DestinationKeyPrefix { get; set; }

        public string DestinationKeyReplaceForEmpty { get; set; }

        public bool DestinationReplace { get; set; }

        public static RedisMigrateConfiguration Create()
        {
            var config = new RedisMigrateConfiguration
            {
                MaxThreads = int.Parse(Environment.GetEnvironmentVariable("MaxThreads")),
                OriginConnectionString = Environment.GetEnvironmentVariable("OriginConnectionString"),
                OriginDatabase = int.Parse(Environment.GetEnvironmentVariable("OriginDatabase")),
                OriginFilter = Environment.GetEnvironmentVariable("OriginFilter"),
                OriginPopulateEnabled = bool.Parse(Environment.GetEnvironmentVariable("OriginPopulateEnabled")),
                OriginPopulateQuantity = int.Parse(Environment.GetEnvironmentVariable("OriginPopulateQuantity")),
                OriginPopulatePrefix = Environment.GetEnvironmentVariable("OriginPopulatePrefix"),
                DestinationConnectionString = Environment.GetEnvironmentVariable("DestinationConnectionString"),
                DestinationDatabase = int.Parse(Environment.GetEnvironmentVariable("DestinationDatabase")),
                DestinationKeyPrefix = Environment.GetEnvironmentVariable("DestinationKeyPrefix"),
                DestinationKeyReplaceForEmpty = Environment.GetEnvironmentVariable("DestinationKeyReplaceForEmpty"),
                DestinationReplace = bool.Parse(Environment.GetEnvironmentVariable("DestinationReplace"))
            };

            if (config.MaxThreads <= 50)
            {
                config.MaxThreads = 50;
            }

            return config;
        }

        public static RedisMigrateConfiguration CreateForDebug()
        {
            return new RedisMigrateConfiguration
            {
                MaxThreads = 5000,
                OriginConnectionString = "localhost:6379,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false",
                OriginDatabase = 0,
                OriginFilter = "*",
                OriginPopulateEnabled = true,
                OriginPopulateQuantity = 1000000,
                OriginPopulatePrefix = "redlock:",
                DestinationConnectionString = "localhost:6377,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false",
                DestinationDatabase = 0,
                DestinationKeyPrefix = "migrated:",
                DestinationKeyReplaceForEmpty = "redlock:",
                DestinationReplace = true,
            };
        }
    }
}