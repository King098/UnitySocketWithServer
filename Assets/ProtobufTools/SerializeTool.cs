using System.IO;


namespace ProtoBuf
{
	public static class SerializeTool
	{
		/// <summary>  
		/// 序列化  
		/// </summary>  
		/// <typeparam name="T"></typeparam>  
		/// <param name="msg"></param>  
		/// <returns></returns>  
		static public byte[] Serialize<T>(T msg)  
		{  
			byte[] result = null;  
			if (msg != null)  
			{  
				using (var stream = new MemoryStream())  
				{  
					Serializer.Serialize<T>(stream, msg);  
					result = stream.ToArray();  
				}  
			}  
			return result;  
		}  

		/// <summary>  
		/// 反序列化  
		/// </summary>  
		/// <typeparam name="T"></typeparam>  
		/// <param name="message"></param>  
		/// <returns></returns>  
		static public T Deserialize<T>(byte[] message)  
		{  
			T result = default(T);  
			if (message != null)  
			{  
				using (var stream = new MemoryStream(message))  
				{  
					result = Serializer.Deserialize<T>(stream);  
				}  
			}  
			return result;  
		}  
	}
}