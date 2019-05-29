using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CS.Utils.CryptoTests.TestUtils
{
	public class JsonFormatter : IFormatter
	{
		private readonly JsonSerializer serializer;

		public SerializationBinder Binder { get; set; }
		public StreamingContext Context { get; set; }
		public ISurrogateSelector SurrogateSelector { get; set; }

		public JsonFormatter()
		{
			var settings = new JsonSerializerSettings()
			{
				ContractResolver = new DefaultContractResolver
				{
					SerializeCompilerGeneratedMembers = true
				},
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				TypeNameHandling = TypeNameHandling.All,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			serializer = JsonSerializer.Create(settings);
		}

		public object Deserialize(Stream serializationStream)
		{
			var reader = new JsonTextReader(new StreamReader(serializationStream));
			return serializer.Deserialize(reader);
		}

		public void Serialize(Stream serializationStream, object graph)
		{
			var writer = new JsonTextWriter(new StreamWriter(serializationStream));
			serializer.Serialize(writer, graph);
			writer.Flush();
		}
	}
}
