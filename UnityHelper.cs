/*
 * 
 * Title: 核心层，帮助类
 * 
 * Description:
 *      具体作用：
 *      1、继承大量通用算法
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
    public class UnityHelper
    {

        private static UnityHelper _instance;
        private float _floDeltaTime;                    //累加时间

        public UnityHelper()
        {

        }

        /// <summary>
        /// 获取本类单例
        /// </summary>
        /// <returns></returns>
        public static UnityHelper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UnityHelper();
            }
            return _instance;
        }


        /// <summary>
        /// 指定间隔时间，时间到后返回true
        /// </summary>
        /// <param name="_smallIntervalTime"></param>
        /// <returns></returns>
        public bool GetSmallTime(float _smallIntervalTime)
        {
            _floDeltaTime += Time.deltaTime;
            if (_floDeltaTime >= _smallIntervalTime)
            {
                _floDeltaTime = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 把自身面向指定目标旋转
        /// </summary>
        /// <param name="_selfTran">自身</param>
        /// <param name="_goal">目标</param>
        /// <param name="_rotateSpeed">旋转速度[0,1]</param>
        public void FaceToGo(Transform _selfTran, Transform _goal, float _rotateSpeed)
        {
            //使用向量减法进行旋转
            Vector3 tmp_Target = new Vector3(_goal.transform.position.x, 0, _goal.transform.position.z);
            Vector3 tmp_Self = new Vector3(_selfTran.position.x, 0, _selfTran.position.z);
            _selfTran.rotation = Quaternion.Slerp(_selfTran.rotation, Quaternion.LookRotation(tmp_Target - tmp_Self), _rotateSpeed);
        }

        /// <summary>
        /// 得到指定范围的随机整数
        /// </summary>
        /// <returns></returns>
        public int GetRandomNum(int _minNum, int _maxNum)
        {
            int tmp_RandomNumResult = 0;
            Random.Range(_minNum, _maxNum);
            if (_minNum == _maxNum)
            {
                tmp_RandomNumResult = _minNum;
            }
            tmp_RandomNumResult = Random.Range(_minNum, _maxNum + 1);
            return tmp_RandomNumResult;
        }
    }
}