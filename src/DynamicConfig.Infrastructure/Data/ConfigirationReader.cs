using System;
using System.Threading;
using DynamicConfig.Core.Interfaces;
using DynamicConfig.Core.Interfaces.Shared;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DynamicConfig.Core;
using DynamicConfig.Core.Entities;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;


namespace DynamicConfig.Infrastructure.Data
{
	public class ConfigirationReader :IConfig
	{

		private readonly IConfigRepository _configRepository = new RedisRepository();
		private static Timer _timer;
		private readonly string _applicationName;
		private readonly string _connectionString;
		private ConcurrentDictionary<string, Config> _dict  = new ConcurrentDictionary<string, Config>();

       
        
		public ConfigirationReader(string applicationName, string connectionString, TimeSpan refreshTimerIntervalInMs)
		{
			_applicationName = applicationName;
			_connectionString = connectionString;
			_timer =   new Timer(_ => UpdateConfig() , null, 0,(int)refreshTimerIntervalInMs.TotalMilliseconds);
           
		}


		private ConcurrentDictionary<string,Config> getConfigFromRedis()
		{

			IEnumerable<Config> configs = _configRepository.GetValues("CONFIGS");

            foreach (Config item in configs)
			{
				_dict.TryAdd(item.Name, item);
			}
            
			return _dict;
		}
       

	    public void UpdateConfig()
		{
			getConfigFromRedis();

			foreach (var key in ConfigurationManager.AppSettings.AllKeys)
			{
				String value = ConfigurationManager.AppSettings[key];
                            
				if(!_dict.ContainsKey(key))
				{

					Config model = new Config();
                    model.Name = key;
					model.ApplicationName = _applicationName;
                    model.IsActive = true;
                    model.Type = "type1";
					model.Value = value;
                    
                    MemoryStream stream = new MemoryStream();  
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

                    ser.WriteObject(stream, model);
					byte[] json = stream.ToArray();  
					stream.Close();
                   
					_configRepository.SetValue(Encoding.UTF8.GetString(json, 0, json.Length));
				}
				else
				{

					Config _config;
					var _value = _dict.TryGetValue(key, out _config);




				}
		           		
			}
           
          string appName = System.IO.Directory.GetCurrentDirectory();
		  ExeConfigurationFileMap map = new ExeConfigurationFileMap();
          map.ExeConfigFilename = new DirectoryInfo(appName).GetFiles("*.config")[0].FullName;

          System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);



			foreach (KeyValuePair<string, Config> item in _dict)
			{

				String value = item.Value.Value;

				String value2 = ConfigurationManager.AppSettings[item.Key];

                               
				if(ConfigurationManager.AppSettings[item.Key] == value){}
				else if(ConfigurationManager.AppSettings[item.Key] == null)
				{

					String aa = ConfigurationManager.AppSettings[item.Key];
                   
					config.AppSettings.Settings.Add(item.Key,item.Value.Value);               
					config.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
				}

				else if(ConfigurationManager.AppSettings[item.Key] != value)
				{
					
					config.AppSettings.Settings[item.Key].Value =  item.Value.Value;
				    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
				}
               
			}


                         

		}

		public T GetValue<T>(string key)
		{
		     object setting_value = ConfigurationManager.AppSettings[key];
   	         return (T) Convert.ChangeType(setting_value, typeof(T));				
		}

		public IEnumerable<Config> aa()
		{


			return _configRepository.GetValues("CONFIGS");
		}

        
	}
}
