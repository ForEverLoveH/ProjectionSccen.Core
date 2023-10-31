using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectionSccen.Core
{
    public class Singleton<T> where T :  new()
    {
        private  static T _instance;
        /// <summary>
        /// 
        /// </summary>
        private static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            return Instance;
        }

    }
}
