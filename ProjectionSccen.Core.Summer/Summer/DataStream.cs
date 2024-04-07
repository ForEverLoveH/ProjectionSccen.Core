namespace ProjectionSccen.Core.Summer ;

 public class DataStream : MemoryStream
 {
     private static Queue<DataStream> dataStreams = new Queue<DataStream>();
     private static int PoolMaxCount = 200;
     private DataStream() { }

     public static DataStream Allocate()
     {
         lock (dataStreams)
         {
             if (dataStreams.Count > 0)
             {
                 return dataStreams.Dequeue();
             }
         }
         return new DataStream();
     }
     public static DataStream Allocate(byte[] data)
     {
         DataStream dataStream = Allocate();
         dataStream.Write(data, 0, data.Length);
         dataStream.Position = 0;
         return dataStream;
     }/// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
     /*/ public ushort ReadUShort()
      {
          return (ushort)((ReadByte() << 8) | ReadByte());
      }*/

     public ushort ReadUShort()
     {
         byte[] bytes = new byte[2];

         this.Read(bytes, 0, bytes.Length);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(bytes);
         return BitConverter.ToUInt16(bytes, 0);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <returns></returns>
     public ushort ReadUInt()
     {
         byte[] bytes = new byte[4];

         this.Read(bytes, 0, bytes.Length);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(bytes);
         return BitConverter.ToUInt16(bytes, 0);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <returns></returns>
     public ushort ReadULong()
     {
         byte[] bytes = new byte[8];

         this.Read(bytes, 0, bytes.Length);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(bytes);
         return BitConverter.ToUInt16(bytes, 0);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="value"></param>
     public void WriteUShort(ushort value)
     {
         byte[] dats = BitConverter.GetBytes(value);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(dats);
         this.Write(dats, 0, 2);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="value"></param>
     public void WriteUInt(uint value)
     {
         byte[] dats = BitConverter.GetBytes(value);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(dats);
         this.Write(dats, 0, 4);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="value"></param>
     public void WriteULong(ulong value)
     {
         byte[] dats = BitConverter.GetBytes(value);
         if (BitConverter.IsLittleEndian)
             Array.Reverse(dats);
         this.Write(dats, 0, 8);
     }
     public void WriteInt(int value)
     {
         byte[] ds = BitConverter.GetBytes(value);
         if (BitConverter.IsLittleEndian) Array.Reverse(ds);
         this.Write(ds, 0, 4);
     }
     public void WriteLong(long value)
     {
         byte[] ds = BitConverter.GetBytes(value);
         if (BitConverter.IsLittleEndian)
         {
             Array.Reverse(ds);
         }
         this.Write(ds, 0, 8);
     }


     /// <summary>
     /// 
     /// </summary>
     /// <param name="disposing"></param>
     protected override void Dispose(bool disposing)
     {
         // Log.Information("dataStream 自动释放");
         lock (dataStreams)
         {
             if (dataStreams.Count < PoolMaxCount)
             {
                 this.Position = 0;
                 this.SetLength(0);
                 dataStreams.Enqueue(this);
             }
             else
             {
                 base.Dispose(disposing);
                 this.Close();
             }
         }
     }
 }