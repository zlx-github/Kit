﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit.Runtime
{
    /// <summary>
    /// 第三方SDK  Android/Ios 源码 https://github.com/Kylin-Studio/Kit.ThirdPartySDK
    ///     注: 
    ///         1. 请在启动时 调用 ThirdPartySDK.CreateInstance()
    ///         2. 需挂载到 GameObject 名称为 "ThirdPartySDK" 对象上
    ///             如名称不一致 则更改 ThirdPartySDK 原生代码中   UnityPlayer.UnitySendMessage("name", "函数名称", "参数");
    /// </summary>
    public class ThirdPartySDK : MonoSingleton<ThirdPartySDK>
    {
        public bool isRegisterToWechat = false;

        protected override void OnCreate()
        { 
            base.OnCreate();

#if UNITY_EDITOR
#elif UNITY_ANDROID
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                instance.currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                instance.tool = new AndroidJavaClass(toolPackageName);
#endif
            Instance.RegisterAppWechat();
        }
         
        #region ... WeChat

        const string WXAppID = "微信APPID";

#if UNITY_EDITOR
#elif UNITY_ANDROID
    const string wechatToolPackageName = "com.my.kit.wechat.WechatTool";
    const string toolPackageName = "com.my.kit.Tool";
    AndroidJavaClass wechatTool;
#elif UNITY_IPHONE
    [DllImport("__Internal")]
    static extern bool IsWechatInstalled_iOS();
    [DllImport("__Internal")]
    static extern void RegisterApp_iOS(string appId);
    [DllImport("__Internal")]
    static extern bool OpenWechat_iOS(string state);
    [DllImport("__Internal")]
    static extern void WechatPay_iOS(string appId, string partnerId, string prepayId, string nonceStr, int timeStamp, string packageValue, string sign);
    [DllImport("__Internal")]
    static extern void CopyTextToClipboard_iOS(string input);
    [DllImport("__Internal")]
    static extern float GetBattery_iOS();
#endif

        /// <summary> 注册微信 </summary>
        public void RegisterAppWechat()
        {
            if (!isRegisterToWechat)
            {
#if UNITY_EDITOR
#elif UNITY_ANDROID
            wechatTool = new AndroidJavaClass(wechatToolPackageName);
            wechatTool.CallStatic("RegisterToWechat", currentActivity, WXAppID);
#elif UNITY_IPHONE
		    RegisterApp_iOS (WXAppID);
#endif
                isRegisterToWechat = true;
            }
        }

        /// <summary> 是否安装了微信 </summary>
        public bool IsWechatInstalled()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
        return wechatTool.CallStatic<bool>("IsWechatInstalled");
#elif UNITY_IPHONE
		return IsWechatInstalled_iOS();
#else
        return false;
#endif
        }

        #region ...登录

        /// <summary> 微信登录 </summary>
        public void WechatLogin()
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass loginC = new AndroidJavaClass(wechatToolPackageName);
        loginC.CallStatic("LoginWechat", "app_wechat");//后期改为随机数加session来校验
#elif UNITY_IPHONE
        OpenWechat_iOS("app_wechat");
#endif
        }

        /// <summary> 微信登录回调 </summary>
        /// <param name="code">授权成功后拿到的code</param>
        public void LoginCallBack(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                // 向服务器发送登录请求
            }
            else
            {
                // TODO: 登录失败, 请重试！
            } 
        }

        #endregion

        #region ...支付

        // <summary> 创建新的支付请求 </summary>
        public void CreateNewWechatPayOrder(int goodsId)
        {
            //向服务器发送购买商品请求
            //收到服务器返回的订单信息后，调用SendWechatPay发起支付
        }

        // <summary> 发送支付请求 </summary>
        void SendWechatPay(WxOrderResp ufo, string orderNo)
        {
#if UNITY_EDITOR
#elif UNITY_IPHONE
        WechatPay_iOS(ufo.partnerid, ufo.prepayid, ufo.noncestr, ufo.timestamp, ufo.package, ufo.sign);
#elif UNITY_ANDROID
        AndroidJavaClass utils = new AndroidJavaClass("com.my.kit.wechat.WechatTool");
        utils.CallStatic("SendPay", ufo.appid, ufo.partnerid, ufo.prepayid, ufo.noncestr, ufo.timestamp.ToString(), ufo.package, ufo.sign);
#endif
        }

        /// <summary> 支付回调 </summary>
        public void WechatPayCallback(string retCode)
        {
            switch (int.Parse(retCode))
            {
                case -2: // 支付取消 
                    break;
                case -1: // 支付失败 
                    break;
                case 0: // 支付成功 
                    break;
            }
        }

        /// <summary> 微信返回的json数据结构 </summary>
        public class WxOrderResp
        {
            public string appid;
            public string noncestr;
            public string package;
            public string partnerid;
            public string prepayid;
            public string sign;
            public int timestamp;
        }

        #endregion

        #endregion

        #region ... Ali Pay

#if UNITY_IPHONE

    [DllImport("__Internal")]
    static extern float AliPay_iOS(string info);

#endif

        public void CreateNewAliPayOrder(int goodsId)
        {
            //向服务器发送购买商品请求
            //收到服务器返回的订单信息后，调用SendWechatPay发起支付
        }

        public void SendAliPay(string info)
        {
#if UNITY_EDITOR
#elif UNITY_IPHONE
        AliPay_iOS(info);
#elif UNITY_ANDROID
        AndroidJavaObject utils = new AndroidJavaObject("com.my.kit.alipay.AliPay");
        utils.CallStatic("SendPay", info, currentActivity);
#endif
        }

        /// <summary> 支付回调 </summary>
        public void AliPayCallback(string paySuccess)
        {
            bool isSuccess = bool.Parse(paySuccess); 

        }

        #endregion
         
        #region 其他功能

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        public void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
            CopyTextToClipboard_iOS(input);  
#elif UNITY_ANDROID
            tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif
        }

        /// <summary>
        /// 获取电量
        /// </summary>
        public int GetBattery()
        {
#if UNITY_EDITOR
            return 50;
#elif UNITY_IPHONE
        return (int)(GetBattery_iOS() * 100);
#elif UNITY_ANDROID
        return tool.CallStatic<int>("GetBattery");
#else
        return 50;
#endif
        }

        #endregion
    }

}
