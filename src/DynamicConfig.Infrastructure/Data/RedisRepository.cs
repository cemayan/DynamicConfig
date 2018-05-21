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
using System.Linq;

namespace DynamicConfig.Infrastructure.Data
{

	public class RedisRepository : IConfigRepository
    {
    
        private static IDatabase _database;

		public RedisRepository()
        {
            var connection = RedisConnectionUtility.Connection;

            if(connection != null)
			{
				_database = connection.GetDatabase();
			}

        }
            
        
        
		public IEnumerable<Config> GetValues(string key,string applicationName)
        {
	
			var arr = new List<Config>();



			//When there is no storage connection
            if(_database==null)
			{
                int counter = 0;  
                string line;  

            
                System.IO.StreamReader file =   
					new System.IO.StreamReader("redislog.txt");  
                while((line = file.ReadLine()) != null)  
                {  
					using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(line)))
                    {
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Config));
                        Config co = (Config)deserializer.ReadObject(ms);
                        if (co.IsActive && co.ApplicationName == applicationName)
                        {
                            
                            arr.Add(new Config { ApplicationName = co.ApplicationName, Name = co.Name, IsActive = co.IsActive, Type = co.Type, Value = co.Value, Id = co.Id });
                        }

                    }               

                   
                    counter++;  
                }  

                file.Close();  
			}

			else
			{
				var configs = _database.HashValues(key);


				//When there is a storage connection, if there is no data
				if (configs.Count() == 0)
				{
                               
					var allLines = File.ReadAllLines("redislog.txt");
                    var filteredLines = allLines.ToList();
                   foreach (var item in filteredLines)
					{

						using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(item)))
						{

							DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Config));
							Config co = (Config)deserializer.ReadObject(ms);
                            

                            MemoryStream stream = new MemoryStream();

							deserializer.WriteObject(stream, co);
                            byte[] json = stream.ToArray();
                     
						
							var hashList = new List<HashEntry>();
                            hashList.Add(new HashEntry(co.Id, Encoding.UTF8.GetString(json, 0, json.Length)));
                            _database.HashSet("CONFIGS", hashList.ToArray());


						}


					}


				}
				else

				{
					foreach (var item in configs)
                    {

                        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(item.ToString())))
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Config));
                            Config co = (Config)deserializer.ReadObject(ms);
                            if (co.IsActive && co.ApplicationName == applicationName)
                            {

                                arr.Add(new Config { ApplicationName = co.ApplicationName, Name = co.Name, IsActive = co.IsActive, Type = co.Type, Value = co.Value, Id = co.Id });
                            }

                        }


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
           

            using (StreamWriter writer = System.IO.File.AppendText("redislog.txt"))
            {

				writer.WriteLine(Encoding.UTF8.GetString(json, 0, json.Length));


            }
         
			//When there is no storage connection
            if (_database == null)
            {

            }
			else
			{
                 var hashList = new List<HashEntry>();
                hashList.Add(new HashEntry(random, Encoding.UTF8.GetString(json, 0, json.Length)));
                _database.HashSet("CONFIGS", hashList.ToArray());
			}
         
           
        }



		public void UpdateValue(Config model)
        {
			//When there is no storage connection
			if (_database == null)
			{
				MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

                ser.WriteObject(stream, model);
                byte[] json = stream.ToArray();


				StringBuilder newFile = new StringBuilder();

                string temp = "";

                
				var allLines = File.ReadAllLines("redislog.txt");

				foreach (string line in allLines)

                {

					if (allLines.Where(x => x.Contains(model.Id)).Any())
                        
                    {

                        
						temp = line.Replace(line,Encoding.UTF8.GetString(json, 0, json.Length));

                        newFile.Append(temp + "\r\n");

                        continue;

                    }

                    newFile.Append(line + "\r\n");

                }
            
               
                File.WriteAllText("redislog.txt",  newFile.ToString());

			}
			else
			{

            MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

                ser.WriteObject(stream, model);
                byte[] json = stream.ToArray();

                var hashList = new List<HashEntry>();
                hashList.Add(new HashEntry(model.Id, Encoding.UTF8.GetString(json, 0, json.Length)));
                _database.HashSet("CONFIGS", hashList.ToArray());
			}


        }


		public void DeleteValue(string id)
        {
			//When there is no storage connection
			if (_database == null)
            {
            
				var allLines = File.ReadAllLines("redislog.txt");
				var filteredLines = allLines.Where(x => !x.Contains(id));
				File.WriteAllLines("redislog.txt", filteredLines);
           

            }
			else
			{
				_database.HashDelete("CONFIGS", id);
			}


        }

		public string GetValue(string id)
		{   
			//When there is no storage connection
			if (_database == null)
            {

                var allLines = File.ReadAllLines("redislog.txt");
                var filteredLines = allLines.Where(x => x.Contains(id));
				return filteredLines.FirstOrDefault();
            }
			else
			{
				return _database.HashGet("CONFIGS", id); ;
			}

          
		}
	}

}
