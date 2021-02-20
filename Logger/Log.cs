/*
 * 
 * 项目：地下斗士
 * 
 * Title: 核心层，日志调试系统(Log日志)
 * 
 * Description:
 *      具体作用：方便软件开发人员调试程序
 *      实现原理：
 *          1、把开发人员在代码中定义的调试语句，写入本日志的缓存
 *          2、当缓存中数量超过定义的最大写入文件数值，则把缓存内容调试语句一次性写入文本文件*          
 *      
 * Version: 1.0
 *
 * Author:何柱洲
 * 
*/

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;

namespace Kernal
{
    public static class Log
    {
        /*核心字段*/
        private static List<string> _listArray;                     //日志缓存数据
        private static string _logPath;                             //Log日志文件路径
        private static e_State _LogState;                           //Log日志状态(部署模式)
        private static int _logMaxCapacity;                         //Log日志最大容量
        private static int _logBufferMaxNumber;                     //Log日志缓存最大容量

        /*系统常量*/
        //XML配置文件"标签常量"
        private const string XML_CONFIG_LOG_PATH = "LogPath";                      //LogPath
        private const string XML_CONFIG_LOG_STATE = "LogState";                    //LogPath
        private const string XML_CONFIG_LOG_MAX_CAPACITY = "LogMaxCapacity";       //LogPath
        private const string XML_CONFIG_LOG_BUFFER_NUMBER = "LogBufferNumber";     //LogPath
        //日志状态常量(部署模式)
        private const string XML_CONFIG_LOG_STATE_DEVELOP = "Develop";             //Develop
        private const string XML_CONFIG_LOG_STATE_SPECIAL = "Special";             //Special
        private const string XML_CONFIG_LOG_STATE_DEPLOY = "Deploy";               //Deploy
        private const string XML_CONFIG_LOG_STATE_STOP = "Stop";                   //Stop
        //日志默认路径
        private static string XML_CONFIG_LOG_DEFAULT_PATH = "DungeonFighterLog.txt";
        //日志最大容量
        private static int LOG_DEFAULT_MAX_CAPACITY_NUMBER = 2000;
        //日志缓存默认最大容量
        private static int LOG_DEFAULT_MAX_LOG_BUFFER_NUMBER = 1;
        private static string LOG_TIPS = "@@@ Important @@@";

        /*临时字段定义*/
        //日志状态(部署模式)
        private static string strLogState = null;
        //日志最大容量
        private static string strLogMaxCapacity = null;
        //日志缓存最大容量
        private static string strLogBufferNumber = null;
        static Log()
        {
            //日志缓存数据
            _listArray = new List<string>();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            //日志文件路径
            IConfigManager tmp_ConfigMgr = new ConfigManager(KernalParameter.GetLogPath(), KernalParameter.GetLogRootNodeName());
            _logPath = tmp_ConfigMgr.AppSetting[XML_CONFIG_LOG_PATH];
            //日志状态(部署模式)
            strLogState = tmp_ConfigMgr.AppSetting[XML_CONFIG_LOG_STATE];
            //日志最大容量
            strLogMaxCapacity = tmp_ConfigMgr.AppSetting[XML_CONFIG_LOG_MAX_CAPACITY];
            //日志缓存最大容量
            strLogBufferNumber = tmp_ConfigMgr.AppSetting[XML_CONFIG_LOG_BUFFER_NUMBER];
#endif

            //日志文件路径
            if (string.IsNullOrEmpty(_logPath))
            {
                _logPath = UnityEngine.Application.persistentDataPath + "//" + XML_CONFIG_LOG_DEFAULT_PATH;
            }

            //日志状态(部署模式)
            if (!string.IsNullOrEmpty(strLogState))
            {
                switch (strLogState)
                {
                    case XML_CONFIG_LOG_STATE_DEVELOP:
                        _LogState = e_State.Develop;
                        break;
                    case XML_CONFIG_LOG_STATE_SPECIAL:
                        _LogState = e_State.Special;
                        break;
                    case XML_CONFIG_LOG_STATE_DEPLOY:
                        _LogState = e_State.Deploy;
                        break;
                    case XML_CONFIG_LOG_STATE_STOP:
                        _LogState = e_State.Stop;
                        break;
                    default:
                        _LogState = e_State.Stop;
                        break;
                }
            }
            else
            {
                _LogState = e_State.Stop;
            }

            //日志最大容量
            if (!string.IsNullOrEmpty(strLogMaxCapacity))
            {
                _logMaxCapacity = Convert.ToInt32(strLogMaxCapacity);
            }
            else
            {
                _logMaxCapacity = LOG_DEFAULT_MAX_CAPACITY_NUMBER;
            }

            //日志缓存最大容量
            if (!string.IsNullOrEmpty(strLogBufferNumber))
            {
                _logBufferMaxNumber = Convert.ToInt32(strLogBufferNumber);
            }
            else
            {
                _logBufferMaxNumber = LOG_DEFAULT_MAX_LOG_BUFFER_NUMBER;
            }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            //创建文件
            if (!File.Exists(_logPath))
            {
                File.Create(_logPath);                      //创建文件

                Thread.CurrentThread.Abort();               //终止当前线程
            }
            //把日志文件中的数据同步到日志缓存中
            SyncFileDataToLogArray();
#endif
        }//Log() end

        #region public,重要管理方法


        /// <summary>
        /// 写数据到文件，默认最低级别
        /// </summary>
        /// <param name="_writeFileDate"></param>
        public static void Write(string _writeFileDate)
        {
            Write(_writeFileDate, e_Level.Low);
        }

        /// <summary>
        /// 写数据到文件中
        /// </summary>
        /// <param name="_writeFileDate">写入的文件信息</param>
        /// <param name="_level">重要等级级别</param>
        public static void Write(string _writeFileDate, e_Level _level)
        {
            //参数检查
            if (_LogState == e_State.Stop)
            {
                return;
            }

            //如果日志缓存数量超过指定容量，则清空
            if (_listArray.Count >= _logMaxCapacity)
            {
                _listArray.Clear();
            }
            if (!string.IsNullOrEmpty(_writeFileDate))
            {
                //增加日期
                _writeFileDate = "Log State:" + _LogState.ToString() + " / " + DateTime.Now.ToString() + "/ " + _writeFileDate;

                //对于不同的"日志状态"，分特性情形写入文件
                if (_level == e_Level.High)
                {
                    _writeFileDate = LOG_TIPS + _writeFileDate;
                }
                switch (_LogState)
                {
                    case e_State.Develop:
                        //追加调试信息写入文件
                        AppendDateToFile(_writeFileDate);
                        break;
                    case e_State.Special:
                        if (_level == e_Level.High || _level == e_Level.Special)
                        {
                            AppendDateToFile(_writeFileDate);
                        }
                        break;
                    case e_State.Deploy:
                        if (_level == e_Level.High)
                        {
                            AppendDateToFile(_writeFileDate);
                        }
                        break;
                    case e_State.Stop:

                        break;
                    default:
                        break;
                }
            }
        }//Write() end

        /// <summary>
        /// 查询日志缓存中所有数据
        /// </summary>
        /// <returns></returns>
        public static List<string> QueryAllDateFromLogBuffer()
        {
            if (_listArray != null)
            {
                return _listArray;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清除实体日志文件与日志缓存中所有数据
        /// </summary>
        public static void ClearLogFileAndBufferDate()
        {
            if (_listArray != null)
            {
                //数据全部清空
                _listArray.Clear();
            }
            SyncLogArrayToFile();
        }

        /// <summary>
        /// 同步缓存数据信息到实体文件中
        /// </summary>
        public static void SyncLogArrayToFile()
        {
            if (!string.IsNullOrEmpty(_logPath))
            {
                StreamWriter tmp_StreamWrite = new StreamWriter(_logPath);
                foreach (string tmp_Item in _listArray)
                {
                    tmp_StreamWrite.WriteLine(tmp_Item);
                }
                tmp_StreamWrite.Close();
            }
        }
        #endregion
        /// <summary>
        /// 把日志文件中的数据同步到日志缓存中
        /// </summary>
        private static void SyncFileDataToLogArray()
        {
            if (!string.IsNullOrEmpty(_logPath))
            {
                StreamReader tmp_StreamReader = new StreamReader(_logPath);
                while (tmp_StreamReader.Peek() >= 0)
                {
                    _listArray.Add(tmp_StreamReader.ReadLine());
                }
                tmp_StreamReader.Close();
            }
        }

        /// <summary>
        /// 追加数据到文件
        /// </summary>
        /// <param name="_writeFileDate">调试信息</param>
        private static void AppendDateToFile(string _writeFileDate)
        {
            if (!string.IsNullOrEmpty(_writeFileDate))
            {
                //把调试信息添加到缓存中
                _listArray.Add(_writeFileDate);
            }

            //如果缓存集合超过超过指定数量(_logBufferMaxNumber)，则同步到实体文件中
            if ((_listArray.Count % _logBufferMaxNumber) == 0)
            {
                //同步缓存中的数据信息到实体文件中
                SyncLogArrayToFile();
            }
        }


        #region 本类的枚举类型
        /// <summary>
        /// 日志状态(部署模式)
        /// </summary>
        public enum e_State
        {
            Develop,                                                //开发模式(输出日主所有内容)
            Special,                                                //指定输出模式
            Deploy,                                                  //部署模式(只输出最核心日志信息，例如严重错误信息、用户登录账号等)
            Stop,                                                   //停止输出模式(不输出任何日志信息)
        }

        /// <summary>
        /// 调试信息的等级
        /// </summary>
        public enum e_Level
        {
            High,
            Special,
            Low,
        }
        #endregion
    }//class end
}