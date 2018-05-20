using System;
using System.Collections.Generic;
using DynamicConfig.Core.Interfaces;
using StackExchange.Redis;

namespace DynamicConfig.Infrastructure.Utilities
{
	public class RedisConnectionUtility
	{

		static RedisConnectionUtility()
		{
			RedisConnectionUtility.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
			{
				return ConnectionMultiplexer.Connect("localhost");
			});
		}

		private static Lazy<ConnectionMultiplexer> lazyConnection;

		public static ConnectionMultiplexer Connection
		{
			get
			{
				return lazyConnection.Value;
			}
		}

	}
}
