/*
 * 
 * 项目：地下斗士
 * 
 * Title: 核心层，配置管理器
 * 
 * Description:
 *      具体作用：读取系统核心XML配置信息
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
using System.Xml.Linq;                                                      //XDocument的命名空间
using System.IO;                                                            //文件输入输出流

namespace Kernal
{
    public class ConfigManager : IConfigManager
    {
        private static Dictionary<string, string> _appSetting;              //定义应用设置集合

        /// <summary>
        /// 配置管理器构造函数
        /// </summary>
        /// <param name="_logPath">日志路径</param>
        /// <param name="_xmlRootNodeName">XML根节点名称</param>
        public ConfigManager(string _logPath, string _xmlRootNodeName)
        {
            _appSetting = new Dictionary<string, string>();
            InitAndAnalysisXML(_logPath, _xmlRootNodeName);
        }

        /// <summary>
        /// 初始化与解析XML数据，到集合中_appSetting
        /// </summary>
        /// <param name="_logPath">日志路径</param>
        /// <param name="_xmlRootNodeName">XML根节点名称</param>
        private void InitAndAnalysisXML(string _logPath, string _xmlRootNodeName)
        {
            //参数检查
            if (string.IsNullOrEmpty(_logPath) || string.IsNullOrEmpty(_xmlRootNodeName))
            {
                return;
            }
            XDocument tmp_XMLDoc;                                                               //XML文档
            XmlReader tmp_XMLReader;                                                            //XML读写器
            try
            {
                tmp_XMLDoc = XDocument.Load(_logPath);                                              //加载日志路径
                tmp_XMLReader = XmlReader.Create(new StringReader(tmp_XMLDoc.ToString()));          //创建XML读写器
            }
            catch
            {
                throw new XMLAnalysisException(GetType() + "/InitAndAnalysisXML()/XML Analaysis Exception! Please check!!");
            }
            //循环解析XML
            while (tmp_XMLReader.Read())                            //读取逐个标签
            {
                //XML读写器从指定根节点开始读写
                if (tmp_XMLReader.IsStartElement() && tmp_XMLReader.LocalName == _xmlRootNodeName)
                {
                    using (XmlReader tmp_XMLReaderItem = tmp_XMLReader.ReadSubtree())
                    {
                        while (tmp_XMLReaderItem.Read())            //读标签的逐个内柔
                        {
                            //如果是节点的元素
                            if (tmp_XMLReaderItem.NodeType == XmlNodeType.Element)
                            {
                                string tmp_StrNode = tmp_XMLReaderItem.Name;
                                //读XML当前行的下一个内容
                                tmp_XMLReaderItem.Read();
                                //如果是节点内容
                                if (tmp_XMLReaderItem.NodeType == XmlNodeType.Text)
                                {
                                    //把标签的元素设置为键，内容设置为值
                                    _appSetting[tmp_StrNode] = tmp_XMLReaderItem.Value;
                                }
                            }
                        }
                    }
                }
            }
        }//InitAndAnalysisXML() end

        /// <summary>
        /// 属性：应用设置
        /// </summary>
        public Dictionary<string, string> AppSetting
        {
            get { return _appSetting; }
        }

        /// <summary>
        /// 得到AppSetting的最大数量
        /// </summary>
        public int GetAppSettngMaxNumber()
        {
            if (_appSetting != null && _appSetting.Count >= 1)
            {
                return _appSetting.Count;
            }
            else
            {
                return 0;
            }
        }
    }
}