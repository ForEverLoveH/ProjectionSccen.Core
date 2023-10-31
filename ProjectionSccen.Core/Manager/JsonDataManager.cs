using Newtonsoft.Json;
namespace ProjectionSccen.Core.Manager;

public class JsonDataManager
{
     private static JsonDataManager instance;
     /// <summary>
     /// 
     /// </summary>
     public static JsonDataManager Instance
     {
          get
          {
               if (instance == null)
               {
                    instance = new JsonDataManager();
               }

               return instance;
          }
     }
     
     /// <summary>
     /// 
     /// </summary>
     /// <param name="t"></param>
     /// <typeparam name="T"></typeparam>
     /// <returns></returns>
     public string SerializeObject<T>(T t) where T : class
     {
          return t == null ? null : JsonConvert.SerializeObject(t);
     }
     /// <summary>
     /// 
     /// </summary>
     /// <param name="data"></param>
     /// <typeparam name="T"></typeparam>
     /// <returns></returns>
     public T DeserializeObject<T>(string data) where T : class
     {
          return string.IsNullOrEmpty(data) == true ? null : JsonConvert.DeserializeObject<T>(data);
     }
}