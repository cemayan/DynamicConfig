using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DynamicConfig.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using DynamicConfig.Core.Entities;

namespace DynamicConfig.Web.Controllers
{
    [Route("api/")]
    public class APIController : Controller
    {
          
		private readonly IConfigRepository _repository;

		public APIController(IConfigRepository repository)
		{
			_repository = repository;
		}


        [HttpGet]
        public IEnumerable<Config> Get()
        {
			return _repository.GetValues("CONFIGS")  ;
        }
        
        [HttpGet("{id}")]
        public string Get(string id)
        {
			return _repository.GetValue(id);
        }
        
        
        [HttpPost]
		public void Post([FromBody] Config model)
        {

			_repository.SetValue(model);

			//MemoryStream stream = new MemoryStream();
   //         DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Config));

   //         ser.WriteObject(stream, value);
   //         byte[] json = stream.ToArray();
   //         stream.Close();

			//_repository.SetValue(Encoding.UTF8.GetString(json, 0, json.Length));

        }
        
        [HttpPut("{id}")]
        public void Put([FromBody]Config model)
        {
			_repository.UpdateValue(model);
        }
        
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
			_repository.DeleteValue(id);
        }
    }
}
