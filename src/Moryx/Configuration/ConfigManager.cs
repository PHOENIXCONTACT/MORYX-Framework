// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Moryx.Serialization;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Moryx.Configuration
{
    /// <summary>
    /// Basic config manager to be used by environments to provide typed configuration
    /// </summary>
    public class ConfigManager : IConfigManager
    {
        /// <summary>
        /// Directory used to read and write config files
        /// </summary>
        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Array of processors used to scan the config for missing nodes
        /// </summary>
        protected virtual IValueProvider[] ValueProviders { get; } =
        {
            new DefaultValueProvider()
        };

        /// <inheritdoc />
        public T GetConfiguration<T>() where T : class, IConfig, new()
        {
            return GetConfiguration<T>(typeof(T).FullName);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(string name) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(false, name);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(bool getCopy) where T : class, IConfig, new()
        {
            return GetConfiguration<T>(getCopy, typeof(T).FullName);
        }

        /// <inheritdoc />
        public T GetConfiguration<T>(bool getCopy, string name) where T : class, IConfig, new()
        {
            return GetConfiguration(typeof(T), getCopy, name) as T;
        }

        /// <summary>
        /// Generic get configuration method
        /// </summary>
        protected virtual IConfig GetConfiguration(Type configType, bool getCopy, string name)
        {
            return TryGetFromDirectory(configType, name);
        }

        /// <inheritdoc />
        public void SaveConfiguration<T>(T configuration) where T : class, IConfig
        {
            SaveConfiguration(configuration, typeof(T).FullName);
        }

        /// <inheritdoc />
        public virtual void SaveConfiguration<T>(T configuration, string name) where T : class, IConfig
        {
            WriteToFile(configuration, name);
        }

        /// <summary>
        /// Try to read config from directory or create default replacement
        /// </summary>
        protected virtual IConfig TryGetFromDirectory(Type confType, string name)
        {
            // Get or create config object
            IConfig config;
            var configPath = GetConfigPath(name);

            if (File.Exists(configPath))
            {
                try
                {
                    var fileContent = File.ReadAllText(configPath);
                
                    config = (IConfig)JsonConvert.DeserializeObject(Decrypt(fileContent), confType, JsonSettings.ReadableReplace);
                    ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders(ValueProviders));
                }
                catch (Exception e)
                {
                    config = CreateConfig(confType, ConfigState.Error, e.Message);
                }
            }
            else
            {
                config = CreateConfig(confType, ConfigState.Generated, "Config file not found! Running on default values.");
            }

            return config;
        }

        /// <summary>
        /// Decrypt the file content of encrypted AES string 
        /// </summary>
        /// <param name="fileContent">The content which should be decrypted.</param>
        /// <returns>The decrypted file content.</returns>
        private string Decrypt(string fileContent)
        {
            try
            {
                // When the files are in the old non encrypted Format
                if (fileContent.StartsWith("{\r\n"))
                {
                    return fileContent;
                }
                using var aes = Aes.Create();
                var key = new Rfc2898DeriveBytes(CreateSaveKey("Mo"), Encoding.UTF8.GetBytes(CreateSaveKey("ryx")));
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                var buffer = Convert.FromBase64String(fileContent);
                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return fileContent;
            }
        }

        /// <summary>
        /// Encrypt the file content to an AES bases encryption.
        /// </summary>
        /// <param name="text">The content which should be encrypted</param>
        /// <returns>Base64 AES encrypted string</returns>
        private string Encrypt(string text)
        {
            try
            {
                // Encrypt the serialized json object with an AES encryption so that the config files are 
                // not readable 
                using var aes = Aes.Create();
                var key = new Rfc2898DeriveBytes(CreateSaveKey("Mo"), Encoding.UTF8.GetBytes(CreateSaveKey("ryx")));
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(text);
                }

                byte[] encryptedBytes = ms.ToArray();
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception e)
            {
                return text;
            }
            
        }

        private IConfig CreateConfig(Type confType, ConfigState state, string loadError)
        {
            var config = (IConfig)Activator.CreateInstance(confType);
            config.ConfigState = state;
            config.LoadError = loadError;

            // Initialize ConfigBase
            var configBase = config as ConfigBase;
            configBase?.Initialize();

            // Fill default values
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddProviders(ValueProviders));
            
            return config;
        }

        /// <summary>
        /// Write config object to mcf file
        /// </summary>
        protected void WriteToFile(object config, string name)
        {
            var text = JsonConvert.SerializeObject(config, JsonSettings.Readable);
            File.WriteAllText(GetConfigPath(name), Encrypt(text));
        }

        private string GetConfigPath(string name)
        {
            var configName = $"{name}{ConfigConstants.FileExtension}";
            return Path.Combine(ConfigDirectory, configName);
        }

        /// <summary>
        /// Checks whether the config exists
        /// </summary>
        /// <param name="name">Name of the configuration</param>
        /// <returns></returns>
        protected bool ConfigExists(string name)
        {
            return File.Exists(GetConfigPath(name));
        }

        /// <summary>
        /// Create a Save key
        /// </summary>
        /// <param name="addition">An addition to the key to make it unique</param>
        /// <returns>Encrypted string in Base64String Format</returns>
        private string CreateSaveKey(string addition)
        {
            var hashValue = new byte[]{ (byte)111, (byte)104,
                (byte)108, (byte)111, (byte)110,
                (byte)103, (byte)106, (byte)111,
                (byte)104, (byte)110, (byte)115,
                (byte)111, (byte)110 };
            var additionTextBytes = System.Text.Encoding.UTF8.GetBytes(addition);

            List<byte> byteList = new List<byte>();
            byteList.AddRange(additionTextBytes);
            byteList.AddRange(hashValue);

            using (HashAlgorithm algorithm = SHA1.Create())
            {
                var hash = algorithm.ComputeHash(byteList.ToArray());
                return Convert.ToBase64String(hash.ToArray());
            }
        }
    }
}
