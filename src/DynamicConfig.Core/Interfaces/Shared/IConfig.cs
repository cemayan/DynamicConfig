using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DynamicConfig.Core.Entities;

namespace DynamicConfig.Core.Interfaces.Shared
{
    public interface IConfig
    {
		T GetValue<T>(string key);
    }
}
