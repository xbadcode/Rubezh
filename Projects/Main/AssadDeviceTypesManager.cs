﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Main
{
    public static class AssadDeviceTypesManager
    {
        static List<x.modelInfoType> modelInfos;
        public static List<x.modelInfoType> ModelInfos
        {
            get { return modelInfos; }
        }

        static x.modelInfoType rootModelInfo;
        public static x.modelInfoType RootModelInfo
        {
            get { return rootModelInfo; }
        }

        public static x.modelInfoType GetModelInfo(string deviceType)
        {
            return ModelInfos.Find(a => a.type1 == deviceType);
        }

        public static void LoadTypes()
        {
            modelInfos = new List<x.modelInfoType>();

            XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
            FileStream fileStream = new FileStream("DeviceModel.xml", FileMode.Open);
            x.MessageType message = (x.MessageType)serializer.Deserialize(fileStream);
            fileStream.Close();
            x.CPconfirmationType confirmation = (x.CPconfirmationType)message.Item;
            rootModelInfo = (x.modelInfoType)confirmation.Items[0];

            //XmlSerializer deSerializer = new XmlSerializer(typeof(x.modelInfoType));
            //FileStream fileStreamOut = new FileStream("xxx.xml", FileMode.CreateNew);
            //deSerializer.Serialize(fileStreamOut, rootModelInfo);
            //fileStreamOut.Close();

            //XmlSerializer serializer = new XmlSerializer(typeof(x.modelInfoType));
            //FileStream fileStream = new FileStream("xxx.xml", FileMode.Open);
            //x.modelInfoType rootModelInfo = (x.modelInfoType)serializer.Deserialize(fileStream);
            //fileStream.Close();

            modelInfos.Add(rootModelInfo);
            AddChild(rootModelInfo);
        }

        static void AddChild(x.modelInfoType parent)
        {
            if (parent.type != null)
                foreach (x.modelInfoType child in parent.type)
                {
                    modelInfos.Add(child);
                    AddChild(child);
                }
        }
    }
}
