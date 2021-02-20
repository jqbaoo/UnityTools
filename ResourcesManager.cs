/*
 * 
 * 项目：地下斗士
 * 
 * Title: 核心层，资源动态加载管理器
 * 
 * Description:
 *      属于"脚本插件"
 *      目的：具备"对象缓冲"功能的资源加载脚本
 *      
 * Version: 1.0
 *
 * Author:何柱洲
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernal
{
    public class ResourcesManager : MonoBehaviour
    {
        private static ResourcesManager _instance;
        private Hashtable _ht = null;                                           //容器

        private ResourcesManager()
        {
            _ht = new Hashtable();
        }

        public static ResourcesManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("ResoucesManager").AddComponent<ResourcesManager>();
                return _instance;
            }
            return _instance;
        }

        /// <summary>
        /// 调用资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_path">路径</param>
        /// <param name="_isCache">是否需要加入缓存池</param>
        /// <returns></returns>
        public T LoadResource<T>(string _path, bool _isCache) where T : Object
        {
            if (_ht.Contains(_path))
            {
                return _ht[_path] as T;
            }
            else
            {
                T tmp_TRes = Resources.Load<T>(_path);
                if (tmp_TRes == null)
                {
                    Debug.LogWarning(GetType() + "/ResourcesManager()/tmp_TRes 提取的资源找不到！path = " + _path);
                }
                else if (_isCache)
                {
                    _ht.Add(_path, tmp_TRes);
                }
                return tmp_TRes;
            }
        }

        /// <summary>
        /// 调用资源
        /// </summary>
        /// <param name="_path">路径</param>
        /// <param name="_isCache">是否需要加入缓存池</param>
        /// <returns></returns>
        public GameObject LoadAsset(string _path, bool _isCache)
        {
            GameObject tmp_GoObj = LoadResource<GameObject>(_path, _isCache);
            GameObject tmp_GoObjClone = Instantiate<GameObject>(tmp_GoObj);
            if (tmp_GoObjClone == null)
            {
                Debug.LogWarning(GetType() + "/LoadAsset()/资源克隆失败！path = " + _path);
            }
            return tmp_GoObjClone;
        }
    }
}