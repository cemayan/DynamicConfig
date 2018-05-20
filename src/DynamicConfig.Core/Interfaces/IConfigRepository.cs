using System;
using System.Collections.Generic;
using DynamicConfig.Core.Entities;

namespace DynamicConfig.Core.Interfaces
{
	public interface IConfigRepository
    {

 		IEnumerable<Config> GetValues(string key);
		void SetValue(string value);
		void DeleteValue(string id);
        void UpdateValue(Config model);
		String GetValue(string id);
    }
}
