using System;
using System.Collections.Generic;
using DynamicConfig.Core.Interfaces;
using StackExchange.Redis;
using DynamicConfig.Infrastructure.Utilities;
using DynamicConfig.Core.Helpers;
using DynamicConfig.Core.Entities;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace DynamicConfig.Infrastructure.Data
{

	public class RedisRepository : IConfigRepository
    {
    
        private static IDatabase _database;

		public RedisRepository()
        {
            var connection = RedisConnectionUtility.Connection;

            _database = connection.GetDatabase();
        }
             
        
		public IEnumerable<Config> GetValues(string key)
        {
			var configs = _database.HashValues(key);

			var arr = new List<Config>();
            

            foreach (var item in configs)
            {
            
				using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(item.ToString())))  
                {
                  DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Config));  
                  Config co = (Config)deserializer.ReadObject(ms);
					if(co.IsActive && co.ApplicationName == System.AppDomain.CurrentDomain.FriendlyName)
					{
						arr.Add(new Config { ApplicationName = co.ApplicationName, Name = co.Name, IsActive = co.IsActive, Type = co.Type, Value = co.Value, Id = co.Id });
					}
	
                }
                         

            }
            return arr;
              
        }
        

		public void SetValue(Config model)
        {
			string random = new RandomStrings(8).generateString();


			model.Id = random;
			MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

            ser.WriteObject(stream, model);
            byte[] json = stream.ToArray();

            var hashList = new List<HashEntry>();
            hashList.Add(new HashEntry(random, Encoding.UTF8.GetString(json, 0, json.Length)));
            _database.HashSet("CONFIGS", hashList.ToArray());

           
        }



		public void UpdateValue(Config model)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

            ser.WriteObject(stream, model);
            byte[] json = stream.ToArray();

            var hashList = new List<HashEntry>();
            hashList.Add(new HashEntry(model.Id, Encoding.UTF8.GetString(json, 0, json.Length)));
            _database.HashSet("CONFIGS", hashList.ToArray());

        }


		public void DeleteValue(string id)
        {
            _database.HashDelete("CONFIGS", id);
        }

		public string GetValue(string id)
		{            
            return _database.HashGet("CONFIGS", id);;
		}
	}

}
