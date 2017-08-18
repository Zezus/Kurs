using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace KursavayaServer.Storage
{
    public class XmlStorage : IStorage
    {
        XmlSerializer _serializer;
        string _filePath;
        public XmlStorage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            _filePath = filePath;
            _serializer = new XmlSerializer(typeof(List<User>));
        }
        public List<User> Load()
        {

            using (var stream = File.OpenRead(_filePath))
            {
                var ddata = _serializer.Deserialize(stream);
                return (List<User>)ddata;
            }


        }

        public void Save(List<User> users)
        {
            using (var writer = new StreamWriter(_filePath))
            {
                _serializer.Serialize(writer, users);
            };

        }

        public void Delete(List<User> users)
        {
            using (var writer = new StreamWriter(_filePath))
            {
                _serializer.Serialize(writer, users);
            };
        }

    }
}
