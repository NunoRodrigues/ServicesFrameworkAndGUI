using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Uqbar.Services.Framework
{
    public static class Configuration
    {
        public static string DefaultDirectory = "";

        public static DateTime Set(object instance)
        {
            return Set(instance, AppDomain.CurrentDomain.BaseDirectory);
        }

        public static DateTime Set(object instance, string baseDirectory)
        {
            // Insert code to set properties and fields of the object.
            XmlSerializer mySerializer = new
            XmlSerializer(instance.GetType());

            // To write to a file, create a StreamWriter object.
            string filename = Path.Combine(baseDirectory, getName(instance.GetType()));
            StreamWriter myWriter = new StreamWriter(filename);
            mySerializer.Serialize(myWriter, instance);
            myWriter.Close();

            return LastSet(instance.GetType());
        }

        public static DateTime LastSet(Type instanceType)
        {
            string filename = getName(instanceType);
            if (File.Exists(filename))
            {
                return File.GetLastWriteTimeUtc(filename);
            }

            return DateTime.MinValue;
        }

        public static DateTime LastSet<T>()
        {
            return LastSet(typeof(T));
        }

        public static T Get<T>()
        {
            return Get<T>(AppDomain.CurrentDomain.BaseDirectory);
        }

        public static T Get<T>(string baseDirectory)
        {
            string filename = Path.Combine(baseDirectory, getName<T>());
            
            if (System.IO.File.Exists(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                StreamReader reader = new StreamReader(filename);
                //reader.ReadToEnd();
                object instance = (T)serializer.Deserialize(reader);
                reader.Close();

                return (T)instance;
            }

            return default(T);
        }

        private static string getName<T>()
        {
            return getName(typeof(T));
        }

        private static string getName(Type instanceType)
        {
            return instanceType.ToString() + ".config";
        }
    }
}
