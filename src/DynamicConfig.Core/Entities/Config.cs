using System;
using System.Runtime.Serialization;

namespace DynamicConfig.Core.Entities
{
	[DataContract] 
	public class Config 
    {
		[DataMember]
        public string Id { get; set; }
		[DataMember]  
		public string Name { get; set; }
		[DataMember]  
		public string Type { get; set; }
		[DataMember]  
		public string Value { get; set; }
		[DataMember]  
		public bool IsActive { get; set; }
		[DataMember]  
		public string ApplicationName { get; set; }    
    }
}
