using System;
using System.Collections.Generic;
using Orleans.Storage;
using System.Linq;

namespace Orleans.Runtime.Configuration
{
    /// <summary>
    /// Extension methods for configuration classes specific to OrleansProviders.dll 
    /// </summary>
    public static class BuiltInProvidersConfigurationExtensions
    {
        /// <summary>
        /// Adds a storage provider of type <see cref="MemoryStorage"/>
        /// </summary>
        /// <param name="config">The cluster configuration object to add provider to.</param>
        /// <param name="providerName">The provider name.</param>
        /// <param name="numStorageGrains">The number of storage grains to use.</param>
        public static void AddMemoryStorageProvider(
            this ClusterConfiguration config,
            string providerName = "MemoryStore",
            int numStorageGrains = MemoryStorage.NumStorageGrainsDefaultValue)
        {
            if (string.IsNullOrWhiteSpace(providerName)) throw new ArgumentNullException(nameof(providerName));

            var properties = new Dictionary<string, string>
            {
                { MemoryStorage.NumStorageGrainsPropertyName, numStorageGrains.ToString() },
            };

            config.Globals.RegisterStorageProvider<MemoryStorage>(providerName, properties);
        }


        /// <summary>
        /// Adds a storage provider of type <see cref="ShardedStorageProvider"/>
        /// </summary>
        /// <param name="config">The cluster configuration object to add provider to.</param>
        /// <param name="providerName">The provider name.</param>
        /// <param name="numStorageGrains">The number of storage grains to use.</param>
        /// <param name="collectionOfShardProviders">The names of the Providers in the shard collection</param>
        public static void AddShardedStorageProvider(
            this ClusterConfiguration config,
            string providerName,
            IEnumerable<string> collectionOfShardProviders)
        {
            if (string.IsNullOrWhiteSpace(providerName)) throw new ArgumentNullException(nameof(providerName));

            var properties = new Dictionary<string, string>();
            //{
            //    { MemoryStorage.NumStorageGrainsPropertyName, numStorageGrains.ToString() },
            //};


            IEnumerable<Orleans.Providers.IProviderConfiguration> provConfigsPre = config.Globals.GetAllProviderConfigurations();


            config.Globals.RegisterStorageProvider<ShardedStorageProvider>(providerName, properties);

            IEnumerable<Orleans.Providers.IProviderConfiguration> provConfigsPost = config.Globals.GetAllProviderConfigurations();

            Orleans.Providers.IProviderConfiguration shardedStorageConfig = null;
            if (config.Globals.TryGetProviderConfiguration(typeof(ShardedStorageProvider).FullName, providerName, out shardedStorageConfig))
            {
                //var config = new ProviderConfiguration();
                //config.Load(subElement, alreadyLoaded, nsManager);
                //add(config);

                var matchedProviderConfigurationList = provConfigsPre.Where(c => collectionOfShardProviders.Contains(c.Name)).Select(a => a);

                foreach (var providerConfig in matchedProviderConfigurationList)
                {
                    ((ProviderConfiguration) shardedStorageConfig).ChildConfigurations.Add(providerConfig);
                }
            }
            else
            {
                throw new Exception("this should not happen");
            }

        }


    }
}