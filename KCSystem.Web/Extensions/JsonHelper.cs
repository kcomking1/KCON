
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KCSystem.Web.Extensions
{
    public class JsonHelper
    {
        private static string ProviderName = string.Empty;
        public static string GetValue(string filePath, string City)
        {
            //添加 json 文件路径
            var builder = new ConfigurationBuilder().SetBasePath(filePath).AddJsonFile("cityPicker.json");
            //创建配置根对象
            var configurationRoot = builder.Build();
            var provider = configurationRoot.GetSection("ChineseDistricts").GetChildren().ToList();
            getSections(configurationRoot, provider, City);
            return ProviderName;
        }

        /// <summary>
        /// 根据省份查询相应市区编码
        /// </summary>
        /// <param name="root"></param>
        /// <param name="provider"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        private static void getSections(IConfigurationRoot root, List<IConfigurationSection> provider,string City)
        {
            foreach (var prov in provider)
            {
                var path = root.GetSection(prov.Path);
                if (string.IsNullOrEmpty(path.Value))
                {
                    getSections(root, path.GetChildren().ToList(), City);
                }
                else
                {
                    if (City == path.Value)
                    {
                        string CodeName = path.Key;
                        var CodePath = path.Path.Split(":");
                        var providerStr = root.GetSection("ChineseDistricts").GetSection("86").GetChildren().ToList();
                        string PriveName = GetProviders(root, providerStr, CodePath[1]);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 根据市区查询相应省份
        /// </summary>
        /// <param name="root"></param>
        /// <param name="provider"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        private static string GetProviders(IConfigurationRoot root, List<IConfigurationSection> provider, string City)
        {
            ProviderName= string.Empty;
            foreach (var prov in provider)
            {
                var path = root.GetSection(prov.Path);
                if (string.IsNullOrEmpty(path.Value))
                {
                    ProviderName = GetProviders(root, path.GetChildren().ToList(), City);
                    if(!string.IsNullOrEmpty(ProviderName))
                        break;
                }
                else
                {
                    if (path.Path.IndexOf("code") > -1 && City == path.Value)
                    {
                        ProviderName = root.GetSection(prov.Path.Replace("code", "address")).Value;
                        break;
                    }
                }
            }
            return ProviderName;
        }

      
    }

    /// <summary>
    /// Json文件读写
    /// 引用Newtonsoft.Json
    /// </summary>
    public class JsonFileHelper
    {
        //注意：section为根节点
        private string _jsonName;
        private string _path;
        private IConfiguration Configuration { get; set; }
        public JsonFileHelper(string jsonName)
        {
            _jsonName = jsonName;
            if (!jsonName.EndsWith(".json"))
                _path = $"{jsonName}.json";
            else
                _path = jsonName;
            //ReloadOnChange = true 当*.json文件被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = _path, ReloadOnChange = true, Optional = true })
            .Build();
        }

        /// <summary>
        /// 读取Json返回实体对象
        /// </summary>
        /// <returns></returns>
        public T Read<T>() => Read<T>("");

        /// <summary>
        /// 根据节点读取Json返回实体对象
        /// </summary>
        /// <returns></returns>
        public T Read<T>(string section)
        {
             
                using (var file = new StreamReader(_path))
                using (var reader = new JsonTextReader(file))
                {
                    var jObj = (JObject)JToken.ReadFrom(reader);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        var secJt = jObj[section];
                        if (secJt != null)
                        {
                            return JsonConvert.DeserializeObject<T>(secJt.ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<T>(jObj.ToString());
                    }
                }
            
            
            return default(T);
        }

        /// <summary>
        /// 读取Json返回集合
        /// </summary>
        /// <returns></returns>
        public List<T> ReadList<T>() => ReadList<T>("");

        /// <summary>
        /// 根据节点读取Json返回集合
        /// </summary>
        /// <returns></returns>
        public List<T> ReadList<T>(string section)
        {
             
            {
                using (var file = new StreamReader(_path))
                using (var reader = new JsonTextReader(file))
                {
                    var jObj = (JObject)JToken.ReadFrom(reader);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        var secJt = jObj[section];
                        if (secJt != null)
                        {
                            return JsonConvert.DeserializeObject<List<T>>(secJt.ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<List<T>>(jObj.ToString());
                    }
                }
            }
          
            return default(List<T>);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T">自定义对象</typeparam>
        /// <param name="t"></param>
        public void Write<T>(T t) => Write("", t);

        /// <summary>
        /// 写入指定section文件
        /// </summary>
        /// <typeparam name="T">自定义对象</typeparam>
        /// <param name="t"></param>
        public void Write<T>(string section, T t)
        {
             
            {
                JObject jObj;
                using (StreamReader file = new StreamReader(_path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObj = (JObject)JToken.ReadFrom(reader);
                    var json = JsonConvert.SerializeObject(t);
                    if (string.IsNullOrWhiteSpace(section))
                        jObj = JObject.Parse(json);
                    else
                        jObj[section] = JObject.Parse(json);
                }

                using (var writer = new StreamWriter(_path))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jObj.WriteTo(jsonWriter);
                }
            }
            
        }

        /// <summary>
        /// 删除指定section节点
        /// </summary>
        /// <param name="section"></param>
        public void Remove(string section)
        {
            
            JObject jObj;
            using (StreamReader file = new StreamReader(_path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jObj = (JObject)JToken.ReadFrom(reader);
                jObj.Remove(section);
            }

            using (var writer = new StreamWriter(_path))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jObj.WriteTo(jsonWriter);
            }
            
            
        }
    }
}
