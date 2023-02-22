using GameLogic;
using GameLogic.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProcessCall : SingleToneMgr<ProcessCall>
{
    public string deeplinkURL;

    void Awake()
    {
        Instance = this;

        Application.deepLinkActivated += onDeepLinkActivated;

        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            //冷启动和应用。absoluteURL不为空，因此处理Deep Link
            onDeepLinkActivated(Application.absoluteURL);
            Debug.Log("AbsoluteURL: " + Application.absoluteURL);
        }
        // 初始化DeepLink Manager全局变量
        else
        {
            deeplinkURL = "[None]";
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            ApplicationChrome.SetStatusBar(true);
            int color = ApplicationChrome.ConvertColorToAndroidColor(GameLogic.Tools.HexToColor("00000000"));
            ApplicationChrome.SetStatusBarColor(color, true);
        }
    }

    private void onDeepLinkActivated(string url)
    {
        StartCoroutine(onDeepLinkActivated2(url));
    }

    IEnumerator onDeepLinkActivated2(string url)
    {
        yield return new WaitForSeconds(2f);

        // 更新 DeepLink Manager 全局变量，以便可以从任何位置访问 URL。
        deeplinkURL = url;
        Debug.Log(deeplinkURL);

        string analysis_1 = deeplinkURL.Split('?')[1];

        string[] analysis_2 = analysis_1.Split('&');

        string type = "";
        string itemid = "";
        string code = "";

        if (analysis_2.Length > 0)
        {
            type = analysis_2[0].Split('=')[1];
            itemid = analysis_2[1].Split('=')[1];
            code = analysis_2[2].Split('=')[1];
        }
    }
}
