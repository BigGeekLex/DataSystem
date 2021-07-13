//#define JSON
#define XML

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if JSON
#endif
#if XML
using System.Xml.Serialization;
#endif

namespace MSFD
{
    public class SaveCore : MonoBehaviour
    {
        public static string defaultPath = "Default";
        public static string checkSumName = ".cs";
        public static void Save<T>(string path, T item)
        {
            if (item == null)
            {
                Debug.LogError("Nothing to save: " + path);
                return;
            }
            string str = Serialize<T>(item);

            path = GetDataPath() + path;

            string subPath = path.Substring(0, path.LastIndexOf('/'));

            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }

            string filePath = path + GetExtension();

            FileStream file = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(file);
            sw.Write(str);
            sw.Close();

            filePath = path + checkSumName + GetExtension();

            file = new FileStream(filePath, FileMode.Create);
            sw = new StreamWriter(file);
            sw.Write(CalculateChecksum(str));
            sw.Close();

        }
        public static T Load<T>(string path, T defaultValue = default(T))
        {
            path = GetDataPath() + path;
            string subPath = path.Substring(0, path.LastIndexOf('/'));
            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }
            string filePath = path + GetExtension();
            FileStream file = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(file);
            string str = sr.ReadToEnd();
            sr.Close();


            if (str.Length == 0)
            {
                //Debug.Log("DataNotFound, FirstStart");
                return defaultValue;
            }

            filePath = path + checkSumName + GetExtension(); 
            file = new FileStream(filePath, FileMode.OpenOrCreate);
            sr = new StreamReader(file);
            string checkSum = sr.ReadToEnd();
            sr.Close();
            
            if(checkSum == CalculateChecksum(str))
            {
                T item = Deserialize<T>(str);
                return item;
            }
            else
            {
                return defaultValue;
            }
        }
        public static T Deserialize<T>(string toDeserialize)
        {
#if JSON
            return JsonUtility.FromJson<T>(toDeserialize);
#endif

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }
        public static string Serialize<T>(T toSerialize)
        {
#if JSON
            if(toSerialize is string)
            {
                return toSerialize as string;
            }
            return JsonUtility.ToJson(toSerialize);
#endif

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
        static string GetExtension()
        {
            //CH
            return ".ch";
#if JSON
            return ".json";
#endif

            return ".xml";
        }
        static string GetDataPath()
        {
#if UNITY_EDITOR
            return Application.dataPath + "/Data" + "/";
#endif

#if UNITY_ANDROID
            return Application.persistentDataPath + "/Data" + "/";
#endif
#if UNITY_IOS
             return Application.persistentDataPath + "/Data" + "/";
#endif
        }
        static string CalculateChecksum(string data)
        {
            int hash = data.GetHashCode();
#if !UNITY_EDITOR
            hash = hash ^ SystemInfo.deviceUniqueIdentifier.GetHashCode();
#endif
            return hash.ToString();
        }
    }
}