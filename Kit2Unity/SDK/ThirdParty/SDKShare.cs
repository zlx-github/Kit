using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit.Runtime
{
    /// <summary>
    /// 分享 SDK 管理器
    ///     注： 挂载到GameObject 名称为 "SDKShare" 对象上
    ///            如名称不一致 则更改 ThirdPartySDK 原生代码中   UnityPlayer.UnitySendMessage("name", "函数名称", "参数");
    /// </summary>
    public class SDKShare : MonoBehaviour
    {
         
        #region  ... WebChat

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="shareScene">分享的场景</param>
        /// <param name="shareImage">分享的图片</param>
        public void ShareImage(ShareScene shareScene, Texture2D shareImage)
        {
            byte[] data = shareImage.EncodeToJPG();
            byte[] dataThumb = UtilityMisc.SizeTextureBilinear(shareImage, Vector2.one * 150).EncodeToJPG(40);
#if UNITY_IPHONE
		IntPtr array = Marshal.AllocHGlobal (data.Length);
		Marshal.Copy (data, 0, array, data.Length);
		IntPtr arrayThumb = Marshal.AllocHGlobal (dataThumb.Length);
		Marshal.Copy (dataThumb, 0, arrayThumb, dataThumb.Length);
		ShareImage_iOS ((int)scene, array, data.Length, arrayThumb, dataThumb.Length);
#elif UNITY_ANDROID
            AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
            utils.CallStatic("ShareImage", (int)shareScene, data, dataThumb);
#endif
        }

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="shareScene">分享的场景</param>
        /// <param name="content">分享的文本内容</param>
        public void ShareText(ShareScene shareScene, string content)
        {
#if UNITY_IPHONE
            ShareText_iOS((int)scene, content);
#elif UNITY_ANDROID
            AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
            utils.CallStatic("ShareText", (int)shareScene, content);
#endif
        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="shareScene">分享的场景</param>
        /// <param name="url">分享的链接地址</param>
        /// <param name="title">分享链接的标题</param>
        /// <param name="content">分享链接的文本描述</param>
        /// <param name="thumb">缩略图</param>
        public void ShareUrl(ShareScene shareScene, string url, string title, string content, Texture2D shareImageThumb)
        {
            byte[] thumb = UtilityMisc.SizeTextureBilinear(shareImageThumb, Vector2.one * 150).EncodeToJPG(40);
#if UNITY_IPHONE
            IntPtr arrayThumb = Marshal.AllocHGlobal(thumb.Length);
            Marshal.Copy(thumb, 0, arrayThumb, thumb.Length);
            ShareUrl_iOS((int)scene, url, title, content, arrayThumb, thumb.Length);
#elif UNITY_ANDROID
            AndroidJavaClass utils = new AndroidJavaClass(WeChatShareUtils);
            utils.CallStatic("ShareWebPage", (int)shareScene, url, title, content, thumb);
#endif
        }

        #endregion

        #region ... Callback
        public void ShareCallBack(string errCode)
        {
           
        }
        #endregion

#if UNITY_IPHONE
    [DllImport("__Internal")]
    static extern void ShareImage_iOS(int scene, IntPtr ptr, int size, IntPtr ptrThumb, int sizeThumb);
    [DllImport("__Internal")]
    static extern void ShareText_iOS(int scene, string content);
    [DllImport("__Internal")]
    static extern void ShareUrl_iOS(int scene, string url, string title, string content, IntPtr ptrThumb, int sizeThumb);
#elif UNITY_ANDROID
    const string WeChatShareUtils = "com.my.kit.wechat.ShareUtils";
#endif

    }

    public enum ShareScene
    {
        /// <summary>
        /// 微信 好友
        /// </summary>
        WXContacts = 0,

        /// <summary>
        /// 微信 朋友圈
        /// </summary>
        WXMoments = 1,

        /// <summary>
        /// 微信收藏
        /// </summary>
        WXFavorite = 2,
    }
}
