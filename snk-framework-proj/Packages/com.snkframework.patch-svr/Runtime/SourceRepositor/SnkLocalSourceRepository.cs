using System.IO;
using System.Threading.Tasks;
using SnkFramework.PatchService.Runtime.Core;
using UnityEngine;

namespace SnkFramework.PatchService.Runtime
{
    public class SnkLocalSourceRepository : ISnkLocalSourceRepository
    {
        public int Version { get; private set; } = 0;
        public Task Initialize()
        {
            string path = "persistentDataPath/.client";
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                Version = int.Parse(text.Trim());
            }

            return Task.CompletedTask;
        }

        public bool Exist(string sourceKey)
        {
            Debug.Log("Exist:" + sourceKey);
            return false;
        }

        public SnkSourceInfo GetSourceInfo(string key)
        {
            throw new System.NotImplementedException();
        }

        public string LocalPath { get; }
    }
}