using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SnkFramework.Network.FileStorage.Editor;
using SnkFramework.Network.FileStorage.Runtime.Base;
using UnityEngine;
using UnityEngine.TestTools;

namespace SnkFramework.Netwokr.FileStorage
{
    namespace Tests.Editor
    {
        public class TestSnkRemoteStoragesWithTake
        {
            [UnityTest] //腾讯云对象仓库测试
            public IEnumerator SnkRemoteCOSStorageWithEnumeratorPasses()
            {
                Debug.Log("开始测试：腾讯云对象仓库测试");
                yield return ExecuteRemoteStorage<SnkRemoteCOSStorage>();
            }

            [UnityTest] //阿里云对象仓库测试
            public IEnumerator SnkRemoteOSSStorageWithEnumeratorPasses()
            {
                Debug.Log("开始测试：阿里云对象仓库测试");
                yield return ExecuteRemoteStorage<SnkRemoteOSSStorage>();
            }

            [UnityTest] //华为云对象仓库测试
            public IEnumerator SnkRemoteOBSStorageWithEnumeratorPasses()
            {
                Debug.Log("开始测试：华为云对象仓库测试");
                yield return ExecuteRemoteStorage<SnkRemoteOBSStorage>();
            }

            private bool checkValid(ISnkStorage storage)
            {
                if (storage.mException != null)
                {
                    Debug.LogException(storage.mException);
                    return false;
                }
                return true;
            }

            protected IEnumerator ExecuteRemoteStorage<TRemoteStorage>()
                where TRemoteStorage : class, ISnkStorage, new()
            {
                ISnkStorage storage = default;
                try
                {
                    storage = new TRemoteStorage();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("创建远端仓库对象失败，请检查配置是否正确。type:" + typeof(TRemoteStorage).FullName);
                    Debug.LogError("请到菜单：Window/SnkFramework/File Storage 配置基础参数");
                }
                
                if(storage == null)
                    yield break;
                
                ExecuteRemoteStorageWithLoad(storage);
                if(checkValid(storage) == false)
                    yield break;
                
                ExecuteRemoteStorageWithPut(storage);
                if(checkValid(storage) == false)
                    yield break;
                
                ExecuteRemoteStorageWithLoad(storage);
                if(checkValid(storage) == false)
                    yield break;
                
                ExecuteRemoteStorageWithTake(storage, "Tmp" + typeof(TRemoteStorage).Name);
                if(checkValid(storage) == false)
                    yield break;
                
                ExecuteRemoteStorageWithDelete(storage);
                if(checkValid(storage) == false)
                    yield break;

                ExecuteRemoteStorageWithLoad(storage);
                if(checkValid(storage) == false)
                    yield break;
                Debug.Log("Test Completed============================");
            }

            private void Output(string tag, string[] keys)
            {
                Debug.Log("[" + tag + "-Begin]--------------------");
                string outputString = string.Empty;
                if (keys == null)
                {
                    outputString = "keys is null";
                }
                else if(keys.Length == 0)
                {
                    outputString = "keys.len = 0";
                }
                else
                {
                    foreach (var key in keys)
                        outputString += key + "\n";
                }
                Debug.Log(outputString.Trim());
                Debug.Log("[" + tag + "-End]");
            }

            private void ExecuteRemoteStorageWithLoad(ISnkStorage storage)
            {
                var infos = storage.LoadObjects();
                var keys = infos.Select(a => a.Item1).ToArray();
                Output("Load", keys);
            }

            private void ExecuteRemoteStorageWithPut(ISnkStorage storage)
            {
                string[] files = System.IO.Directory.GetFiles("ProjectSettings", "*.*", SearchOption.AllDirectories);
                var keys = storage.PutObjects(files.ToList());
                Output("Put", keys);
            }

            private void ExecuteRemoteStorageWithTake(ISnkStorage storage, string localDirPath)
            {
                var remoteInfos = storage.LoadObjects();
                var remoteKeys = remoteInfos.Select(a => a.Item1).ToArray();
                var keys = storage.TakeObjects(new List<string>(remoteKeys), localDirPath);
                Output("Take", keys);
            }

            private void ExecuteRemoteStorageWithDelete(ISnkStorage storage)
            {
                var remoteInfos = storage.LoadObjects();
                var remoteKeys = remoteInfos.Select(a => a.Item1).ToArray();
                var keys = storage.DeleteObjects(new List<string>(remoteKeys));
                Output("Delete", keys);
            }
        }
    }
}