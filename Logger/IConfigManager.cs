/*
 * 
 * 项目：地下斗士
 * 
 * Title: 接口，配置管理器
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

namespace Kernal
{
    public interface IConfigManager
    {
        /// <summary>
        /// 属性：应用设置
        /// </summary>
        Dictionary<string, string> AppSetting { get; }

        /// <summary>
        /// 得到AppSetting的最大数量
        /// </summary>
        int GetAppSettngMaxNumber();
    }
}