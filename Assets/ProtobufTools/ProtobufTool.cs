using System.IO;
using System;


namespace ProtoBuf
{
	public static class ProtobufTool 
	{

        public static int headLength = sizeof(ushort);


		public static byte[] CreateData(int typeId,IExtensible pbuf)
		{
			byte[] pbdata = SerializeTool.Serialize(pbuf);
            ByteBuffer buff = new ByteBuffer();
			buff.WriteInt(typeId);
			buff.WriteBytes(pbdata);
			return WriteMessage(buff.ToBytes());
		}

		public static byte[] WriteMessage(byte[] message)
		{
			MemoryStream ms = null;
			using (ms = new MemoryStream())
			{
				ms.Position = 0;
				BinaryWriter writer = new BinaryWriter(ms);
				ushort msglen = (ushort)message.Length;
				writer.Write(msglen);
				writer.Write(message);
				writer.Flush();
				return ms.ToArray();
			}
		}
	}
}