using System;
using System.Collections.Generic;
using DynamicConfig.Core.Interfaces;
using StackExchange.Redis;

namespace DynamicConfig.Infrastructure.Utilities
{
	public class RedisConnectionUtility
	{

		static string url ="localhost";
		static int port = 6379;

		static ConfigurationOptions co = new ConfigurationOptions()
        {
            SyncTimeout = 500000,
            EndPoints =
            {
				{url,port }
            },
            AbortOnConnectFail = false // this prevents that error
        };

		static RedisConnectionUtility()
		{
       
			RedisConnectionUtility.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
			{
           
				if(ConnectionMultiplexer.Connect(co).IsConnected)
				{
					return ConnectionMultiplexer.Connect(co);
				}
				else
				{
					return null;
				}

            
			});
      
		}

		private static Lazy<ConnectionMultiplexer> lazyConnection;

		public static ConnectionMultiplexer Connection
		{
			get
			{

                if(lazyConnection !=null)
				{
					return lazyConnection.Value;
				}
				else
				{
					return null;
				}

			}
		}

	}
}
