/*
 * 
 * 项目：地下斗士
 * 
 * Title: 核心层，XML对话系统数据解析管理脚本
 * 
 * Description:
 *      具体作用：解析XML对话数据文件内容
 *      
 * Version: 1.0
 *
 * Author:何柱洲
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;

namespace Kernal
{
    public class XMLDialogsDataAnalysisManager : MonoBehaviour
    {
        private static XMLDialogsDataAnalysisManager _instance;

        private List<DialogDataFormat> _listDialogDataArray;                        //对话数据集合
        private string _strXMLPath;                                                 //XML文件路径
        private string _strXMLRootNodeName;                                         //XML根节点名称
        /*常量定义*/
        private const float TIME_DELAY = 0.1f;                                      //延迟时间
        private const string XML_ATTRIBUTE_1 = "DialogSecNum";                      //XNK 文件属性字符串
        private const string XML_ATTRIBUTE_2 = "DialogSecName";
        private const string XML_ATTRIBUTE_3 = "SectionIndex";
        private const string XML_ATTRIBUTE_4 = "DialogSide";
        private const string XML_ATTRIBUTE_5 = "DialogPerson";
        private const string XML_ATTRIBUTE_6 = "DialogContent";

        private XMLDialogsDataAnalysisManager()
        {
            _listDialogDataArray = new List<DialogDataFormat>();
        }

        /// <summary>
        /// 得到本类实例
        /// </summary>
        /// <returns></returns>
        public static XMLDialogsDataAnalysisManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("XMLDialogsDataAnalysisManager").AddComponent<XMLDialogsDataAnalysisManager>();
            }
            return _instance;
        }

        /// <summary>
        /// 设置XML路径与根节点的名称
        /// </summary>
        /// <param name="_xmlPath"></param>
        /// <param name="_xmlRootNodeName"></param>
        public void SetXMLPathAndRootNodeName(string _xmlPath, string _xmlRootNodeName)
        {
            if (!string.IsNullOrEmpty(_xmlPath) && !string.IsNullOrEmpty(_xmlRootNodeName))
            {
                _strXMLPath = _xmlPath;
                _strXMLRootNodeName = _xmlRootNodeName;
            }
        }

        /// <summary>
        /// 得到本脚本数据集合
        /// </summary>
        /// <returns></returns>
        public List<DialogDataFormat> GetAllXMLDataArray()
        {
            if (_listDialogDataArray != null && _listDialogDataArray.Count >= 1)
            {
                return _listDialogDataArray;
            }
            else
            {
                return null;
            }
        }
        IEnumerator Start()
        {
            //等待关于XML路径与XML根节点名称的赋值
            yield return new WaitForSeconds(TIME_DELAY);
            if (!string.IsNullOrEmpty(_strXMLPath) && !string.IsNullOrEmpty(_strXMLRootNodeName))
            {
                StartCoroutine("ReadXMLConfigByWWW");
            }
            else
            {
                Debug.LogError(GetType() + "/Start()/_strXMLPath or _strXMLRootNodeName is null");
            }
        }

        /// <summary>
        /// 读取XML配置文件
        /// </summary>
        /// <returns></returns>
        IEnumerator ReadXMLConfigByWWW()
        {
            WWW tmp_WWW = new WWW(_strXMLPath);
            while (!tmp_WWW.isDone)
            {
                yield return tmp_WWW;
                //初始化配置
                InitXMLConfig(tmp_WWW, _strXMLRootNodeName);
            }
        }

        /// <summary>
        /// 初始化XML配置
        /// </summary>
        /// <param name="_www"></param>
        /// <param name="_rootNodeName"></param>
        private void InitXMLConfig(WWW _www, string _rootNodeName)
        {
            if (_listDialogDataArray == null || string.IsNullOrEmpty(_www.text))
            {
                Debug.LogError(GetType() + "/Start()/_listDialogDataArray == null or _rootNodeName is null");
                return;
            }
            //XML解析
            XmlDocument tmp_XMLDoc = new XmlDocument();
            //tmp_XMLDoc.LoadXml(_www.text);    这种方法发布到Android手机端不能正确显示中文
            
            /* 以下4行代码代替上面注释内容，解决中文乱码问题 */
            System.IO.StringReader tmp_StrReader = new System.IO.StringReader(_www.text);
            tmp_StrReader.Read();
            System.Xml.XmlReader tmp_XMLReader = System.Xml.XmlReader.Create(tmp_StrReader);
            tmp_XMLDoc.LoadXml(tmp_StrReader.ReadToEnd());

            XmlNodeList tmp_Nodes = tmp_XMLDoc.SelectSingleNode(_rootNodeName).ChildNodes;
            foreach (XmlElement tmp_XMLElemtItem in tmp_Nodes)
            {
                DialogDataFormat tmp_Data = new DialogDataFormat();
                //段落编号
                tmp_Data.DialogSecNum = Convert.ToInt32(tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_1));
                //段落名称
                tmp_Data.DialogSecName = tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_2);
                //段落内序号
                tmp_Data.SectionIndex = Convert.ToInt32(tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_3));
                //段落双方
                tmp_Data.DialogSide = tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_4);
                //对话认命
                tmp_Data.DialogPerson = tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_5);
                //对话内容
                tmp_Data.DialogContent = tmp_XMLElemtItem.GetAttribute(XML_ATTRIBUTE_6);
                //加入集合
                _listDialogDataArray.Add(tmp_Data);
            }//foreach end
        }//InitXMLConfig() end
    }
}
