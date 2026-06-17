using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Standard_Library
{
    public static class SaveManager
    {
        public static bool Save(List<DataSerializer> data, string project, string folder, string fileName)
        {
            SaveData saveData = new SaveData(data, project, folder, fileName);
            return Save(saveData);
        }
        
        private static bool Save(SaveData saveData)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(saveData.folder);

                // Configure Newtonsoft to handle inheritance and format it nicely
                var settings = new JsonSerializerSettings 
                {
                    TypeNameHandling = TypeNameHandling.Auto, // Injects $type for inherited classes
                    Formatting = Formatting.Indented,         // Makes it readable
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Prevents circular reference crashes
                };

                // Convert to JSON using Newtonsoft
                string json = JsonConvert.SerializeObject(saveData, settings);

                // Write to file
                File.WriteAllText(saveData.fullPath, json);

                Debug.Log($"✅ Saved to: {saveData.fullPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Save failed: {e.Message}");
                return false;
            }
        }
        [Serializable]
        private class SaveData
        {
            public List<DataSerializer> data;
            public readonly string fileName;
            public readonly string folder;
            public readonly string fullPath;
            public SaveData(List<DataSerializer> data,string project, string folder, string fileName)
            {
                this.data = data;
                this.fileName = fileName;
                this.folder = Path.Combine(Application.persistentDataPath, project, folder);
                fullPath = Path.Combine(this.folder, this.fileName+".json");
                int mainBlockCount = 1;
                foreach (DataSerializer dataSerializer in data)
                {
                    if (dataSerializer is not BlockData blockData) continue;
                    if (!blockData.blockName.ToLower().Contains("main")) continue;
                    blockData.blockName += $" {mainBlockCount}";
                    mainBlockCount++;
                }
            }
            //get save data
        }
    }
}