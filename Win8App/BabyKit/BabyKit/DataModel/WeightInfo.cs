﻿using BabyKit.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BabyKit.DataModel
{
    // Used for weight and height
    [DataContract]
    class Record
    {
        [DataMember]
        public DateTime Date{ get; set; }

        [DataMember]
        public double Value{ get; set; }

        public override string ToString()
        {
            return Date.ToString() + " " + Value.ToString("0.0");
        }
    }

    class WeightManager
    {
        private static readonly string WEIGHTINFO_PATH = "WeightInfo.json";

        internal static ObservableCollection<Record> _records = new ObservableCollection<Record>();

        public static async Task<ObservableCollection<Record>> Load()
        {
            var storageFolder = KnownFolders.DocumentsLibrary;

            try
            {
                _records = await FileHelper.LoadData<ObservableCollection<Record>>(WEIGHTINFO_PATH);
            }
            catch (FileNotFoundException)
            {
                //return null;
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return _records;
        }

        public static void Save()
        {
            FileHelper.SaveData(WEIGHTINFO_PATH, _records);
        }
    }

}
