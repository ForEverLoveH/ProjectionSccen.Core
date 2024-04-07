using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace ProjectionSccen.Core.Summer.Protos;

public class ProtoExtension
{
    private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();
    private static Dictionary<int, Type> mDict1 = new Dictionary<int, Type>();
    private static Dictionary<Type, int> mDict2 = new Dictionary<Type, int>();
    static ProtoExtension()
    {
        List<string> list = new List<string>();
        var q = from t in Assembly.GetExecutingAssembly().GetTypes() select t;
        q.ToList().ForEach(t =>
        {
            if (typeof(IMessage).IsAssignableFrom(t))
            {
                var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                _registry.Add(desc.FullName, t);
                list.Add(desc.FullName);
            }
        });

        list.Sort((x, y) =>
        {
            //按照字符串长度排序，
            if (x.Length != y.Length)
            {
                return x.Length - y.Length;
            }
            //如果长度相同
            return string.Compare(x, y, StringComparison.Ordinal);
        });

        for (int i = 0; i < list.Count; i++)
        {
            var fname = list[i];
            var t = _registry[fname];
          
            mDict1[i] = t;
            mDict2[t] = i;
        }
    }
    public static int SeqCode(Type type)
    {
        return mDict2[type];
    }

    public static Type SeqType(int code)
    {
        return mDict1[code];
    }
    /// <summary>
    /// 根据消息编码进行解析
    /// </summary>
    /// <param name="typeCode"></param>
    /// <param name="data"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static IMessage ParseFrom(int typeCode, byte[] data, int offset, int len)
    {
        Type t =  SeqType(typeCode);    
        var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
        var msg = desc.Parser.ParseFrom(data, offset, len);
        Console.WriteLine("解析消息：code={0} - {1}", typeCode, msg);
        return msg;
    }
}