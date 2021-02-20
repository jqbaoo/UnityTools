/*
 * 
 * 项目：地下斗士
 * 
 * Title: 核心层，对话数据管理类
 * 
 * Description:
 *      具体作用：根据"对话数据格式"(DialogDataFormat)定义，输入"段落编号，输出给定的对话内容"
 *      
 * Version: 1.0
 *
 * Author:何柱洲
 * 
*/

using System.Collections;
using System.Collections.Generic;

namespace Kernal
{
    public class DialogDataManager
    {
        private static DialogDataManager _instance;
        private static List<DialogDataFormat> _listAllDialogDataArray;                                  //所有的对话数组集合
        private static List<DialogDataFormat> _listCurrentDialogBufferArray;                            //当前对话缓存集合
        private static int _intIndexByDialogSection;                                                    //对话序号(某个段落)
        /*系统常量*/
        private const string XML_DEFINATION_HERO = "Hero";
        private const string XML_DEFINATION_NPC = "NPC";
        //原对话"对话编号"
        private static int _originalDialogSectionNum = 1;
        private DialogDataManager()
        {
            _listAllDialogDataArray = new List<DialogDataFormat>();
            _listCurrentDialogBufferArray = new List<DialogDataFormat>();
            _intIndexByDialogSection = 0;
        }

        public static DialogDataManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DialogDataManager();
            }
            return _instance;
        }

        /// <summary>
        /// 外部加载数据集合
        /// </summary>
        /// <param name="_dialogDataArray">外部数据集合</param>
        /// <returns>true加载成功，false加载失败 </returns>
        public bool LoadAllDialogData(List<DialogDataFormat> _dialogDataArray)
        {
            //输入参数检查
            if (_dialogDataArray == null || _dialogDataArray.Count == 0)
            {
                return false;
            }

            if (_listAllDialogDataArray != null && _listAllDialogDataArray.Count == 0)
            {
                for (int i = 0; i < _dialogDataArray.Count; i++)
                {
                    _listAllDialogDataArray.Add(_dialogDataArray[i]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 得到下一条记录
        /// </summary>
        /// <param name="_diaSectionNum">输入：段落编号</param>
        /// <param name="_diaSide">输出：对话方</param>
        /// <param name="_strPersonName">输出：人名</param>
        /// <param name="_strDialogContent">输出：对话内容</param>
        /// <returns>true:输出合法数据  false：没有输出对话数据</returns>
        public bool GetNextDialogInfoRecoder(int _diaSectionNum, out e_DialogSide _diaSide, out string _strPersonName, out string _strDialogContent)
        {
            _diaSide = e_DialogSide.None;
            _strPersonName = "";
            _strDialogContent = "";
            //输入参数检查
            if (_diaSectionNum < 0)
            {
                return false;
            }
            
            //段落编号增大后，保留上一个"对话段落编号"，以方便后续逻辑处理
            if (_originalDialogSectionNum < _diaSectionNum)
            {
                //重制内部编号
                _intIndexByDialogSection = 0;
                //清空缓存
                _listCurrentDialogBufferArray.Clear();
                //把当前的"段落编号"记录下来
                _originalDialogSectionNum = _diaSectionNum;
            }

            if (_listCurrentDialogBufferArray != null && _listCurrentDialogBufferArray.Count >= 1)
            {
                if (_intIndexByDialogSection < _listCurrentDialogBufferArray.Count)
                {
                    //序号小于XML某个段落的总对话数
                    ++_intIndexByDialogSection;
                }
                else
                {
                    return false;
                }
            }
            //缓存为空
            else
            {
                ++_intIndexByDialogSection;
            }
            //得到对话信息
            GetDialogInfoRecoder(_diaSectionNum, out _diaSide, out _strPersonName, out _strDialogContent);

            return true;
        }

        /// <summary>
        /// 得到对话信息
        ///     对于输入的"段落编号"，首先在"当前对话数据集合"中查询
        ///     若找到直接换回结果，否则在"全部对话数据集合"中查询，且加入当前的缓存集合
        /// </summary>
        /// <param name="_diaSectionNum">输入：段落编号</param>
        /// <param name="_diaSide">输出：对话方</param>
        /// <param name="_strPersonName">输出：人名</param>
        /// <param name="_strDialogContent">输出：对话内容</param>
        /// <returns>true：数据有效  false数据无效</returns>
        private bool GetDialogInfoRecoder(int _diaSectionNum, out e_DialogSide _diaSide, out string _strPersonName, out string _strDialogContent)
        {
            _diaSide = e_DialogSide.None;
            _strPersonName = "空数据";
            _strDialogContent = "空数据";
            string tmp_StrDialogSide;


            if (_diaSectionNum <= 0)
            {
                return false;
            }

            //对于输入的"段落编号"，首先在"当前对话数据集合"中查询
            if (_listCurrentDialogBufferArray != null && _listCurrentDialogBufferArray.Count >= 1)
            {
                for (int i = 0; i < _listCurrentDialogBufferArray.Count; i++)
                {
                    //段落编号相同
                    if (_listCurrentDialogBufferArray[i].DialogSecNum == _diaSectionNum)
                    {
                        //段内序号相同
                        if (_listCurrentDialogBufferArray[i].SectionIndex == _intIndexByDialogSection)
                        {
                            tmp_StrDialogSide = _listCurrentDialogBufferArray[i].DialogSide;
                            if (tmp_StrDialogSide.Trim().Equals(XML_DEFINATION_HERO))
                            {
                                _diaSide = e_DialogSide.HersoSide;
                            }
                            else if (tmp_StrDialogSide.Trim().Equals(XML_DEFINATION_NPC))
                            {
                                _diaSide = e_DialogSide.NPCSide;
                            }
                            _strPersonName = _listCurrentDialogBufferArray[i].DialogPerson;
                            _strDialogContent = _listCurrentDialogBufferArray[i].DialogContent;

                            return true;
                        }
                    }
                }
            }

            //若找到直接换回结果，否则在"全部对话数据集合"中查询，且加入当前的缓存集合
            if (_listAllDialogDataArray != null && _listAllDialogDataArray.Count >= 1)
            {
                for (int i = 0; i < _listAllDialogDataArray.Count; i++)
                {
                    //段落编号相同
                    if (_listAllDialogDataArray[i].DialogSecNum == _diaSectionNum)
                    {
                        //段内序号相同
                        if (_listAllDialogDataArray[i].SectionIndex == _intIndexByDialogSection)
                        {
                            tmp_StrDialogSide = _listAllDialogDataArray[i].DialogSide;
                            if (tmp_StrDialogSide.Trim().Equals(XML_DEFINATION_HERO))
                            {
                                _diaSide = e_DialogSide.HersoSide;
                            }
                            else if (tmp_StrDialogSide.Trim().Equals(XML_DEFINATION_NPC))
                            {
                                _diaSide = e_DialogSide.NPCSide;
                            }
                            _strPersonName = _listAllDialogDataArray[i].DialogPerson;
                            _strDialogContent = _listAllDialogDataArray[i].DialogContent;

                            //把当前段落编号中的数据写入"当前段落缓存集合"
                            LoadToBufferArrayBySectionNum(_diaSectionNum);
                            return true;
                        }
                    }
                }
            }
            return false;
        }//GetDialogInfoRecoder() end

        /// <summary>
        /// 把当前段落编号中的数据写入"当前段落缓存集合"
        /// </summary>
        /// <param name="_diaSectionNum">输入：当前段落编号</param>
        /// <returns></returns>
        private bool LoadToBufferArrayBySectionNum(int _diaSectionNum)
        {
            //输入参数检查
            if (_diaSectionNum <= 0)
            {
                return false;
            }

            if (_listAllDialogDataArray != null && _listAllDialogDataArray.Count >= 1)
            {
                //清空当前集合以前的缓存
                _listCurrentDialogBufferArray.Clear();
                for (int i = 0; i < _listAllDialogDataArray.Count; i++)
                {
                    if (_listAllDialogDataArray[i].DialogSecNum == _diaSectionNum)
                    {
                        //把查询的数据添加到当前缓存数据
                        _listCurrentDialogBufferArray.Add(_listAllDialogDataArray[i]);
                    }
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 对话双方
    /// </summary>
    public enum e_DialogSide
    {
        None,                                               //无
        HersoSide,                                          //英雄
        NPCSide,                                            //NPC
    }
}