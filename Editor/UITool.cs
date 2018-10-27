using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class UITool
{
    public static int UIlayer = LayerMask.NameToLayer("UI");
    static int FontSize = 25;
    static Font fnt;
    static Font CacheFont()
    {
        if (fnt == null)
            fnt = AssetDatabase.LoadAssetAtPath<Font>("Assets/xxx.ttf");
        if (fnt == null)
            fnt = Font.CreateDynamicFontFromOSFont("Arial", FontSize);
        return fnt;
    }

    [MenuItem("Component/UI/Shortcut/Create Text #&L")]
    public static void CreateText()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject obj = Selection.gameObjects[0];
            GameObject text = new GameObject();
            RectTransform textRect = text.AddComponent<RectTransform>();
            Text textTx = text.AddComponent<Text>();
            text.transform.SetParent(obj.transform);
            text.name = "Text";
            text.layer = UIlayer;
            //设置字体
            textTx.font = CacheFont();
            textTx.fontSize = FontSize;
            textTx.text = "plateface";
            textTx.alignment = TextAnchor.MiddleCenter;
            textRect.localScale = Vector3.one;
            textRect.sizeDelta = new Vector2(textTx.preferredWidth, textTx.preferredHeight);
            RectTransformZero(textRect);
        }
    }

    [MenuItem("Plateface/CreateUGUI Button #&B")]
    public static void CreateButton()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject obj = Selection.gameObjects[0];
            if (obj == null) return;

            GameObject button = new GameObject();
            GameObject buttonTx = new GameObject();

            RectTransform buttonRect = button.AddComponent<RectTransform>();
            RectTransform buttonTxRect = buttonTx.AddComponent<RectTransform>();

            button.AddComponent<Image>();
            Text texBtn = buttonTx.AddComponent<Text>();
            texBtn.font = CacheFont();
            texBtn.fontSize = FontSize;
            texBtn.text = "Button";
            buttonTxRect.sizeDelta = new Vector2(texBtn.preferredWidth, texBtn.preferredHeight);
            button.transform.SetParent(obj.transform);
            buttonTx.transform.SetParent(button.transform);
            button.name = "Button";
            buttonTx.name = "Text";

            button.layer = UIlayer;
            buttonTx.layer = UIlayer;

            RectTransformZero(buttonRect);
            RectTransformZero(buttonTxRect);
        }
    }

    [MenuItem("Plateface/CreateUGUI Image #&S")]
    public static void CreateImage()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject obj = Selection.gameObjects[0];

            GameObject image = new GameObject();
            RectTransform imageRect = image.AddComponent<RectTransform>();
            image.AddComponent<Image>();
            image.transform.SetParent(obj.transform);
            image.name = "Image";
            image.layer = 5;

            RectTransformZero(imageRect);
        }

    }
    [MenuItem("Plateface/CreateUGUI Image #&T")]
    public static void CreateRawImage()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject obj = Selection.gameObjects[0];

            GameObject image = new GameObject();
            RectTransform imageRect = image.AddComponent<RectTransform>();
            image.AddComponent<RawImage>();
            image.transform.SetParent(obj.transform);
            image.name = "Raw Image";
            image.layer = 5;

            RectTransformZero(imageRect);
        }

    }

    [MenuItem("Plateface/CreateUGUI InputField #&I")]
    public static void CreateInputField()
    {
        if (Selection.gameObjects.Length == 1)
        {
            GameObject obj = Selection.gameObjects[0];

            GameObject inputField = new GameObject();
            RectTransform rectTransform = inputField.AddComponent<RectTransform>();
            Image image = inputField.AddComponent<Image>();
            //image.sprite = Resources.Load<Sprite>("UnityPlugins/UGUIShortcutKeyTexture/background1");
            inputField.AddComponent<InputField>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            inputField.layer = UIlayer;

            inputField.transform.SetParent(obj.transform);
            inputField.name = "InputField";

            GameObject placeholder = new GameObject();
            Text placeholderTx = placeholder.AddComponent<Text>();
            placeholderTx.font = CacheFont();
            placeholderTx.fontSize = FontSize;
            placeholderTx.text = "Enter text...";
            placeholder.transform.SetParent(inputField.transform);
            placeholder.name = "Placeholder";
            placeholder.layer = UIlayer;
            placeholderTx.color = Color.black;

            GameObject text = new GameObject();
            Text textTx = text.AddComponent<Text>();
            text.transform.SetParent(inputField.transform);
            text.name = "Text";
            text.layer = UIlayer;

            textTx.color = Color.black;

            RectTransformZero(rectTransform);
        }
    }

    public static void RectTransformZero(RectTransform rectTransform)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchoredPosition3D = Vector3.zero;
    }


    public enum ExportUINode
    {
        Btn = 0,
        Lb,
        Img,
        Go
    }
    public static string[] luaFuncName = {
        "Btn",
        "Text",
        "BigBg",
        "Go",
    };
    [MenuItem("GameObject/获取控件路径", priority = -2)]
    public static void GetWgtPath()
    {
        var trans = Selection.activeTransform;
        if (trans == null) return;
        var path = FindPath(trans);
        EditorGUIUtility.systemCopyBuffer = path;
        Debug.Log(path);
    }
    [MenuItem("GameObject/获取控件路径(包含界面名)", priority = -3)]
    public static void GetFullWgtPath()
    {
        var trans = Selection.activeTransform;
        if (trans == null) return;
        var path = FindPath(trans,true);
        EditorGUIUtility.systemCopyBuffer = path;
        Debug.Log(path);
    }
    [MenuItem("GameObject/获取控件lua代码段", priority = -1)]
    public static void GetWgtLuaScript()
    {
        var sels = Selection.gameObjects;
        if (sels.Length <= 0) return;
        var codes = new System.Text.StringBuilder();
        foreach (var one in sels)
        {
            var trans = one.transform;

            if (trans == null) return;
            var path = FindPath(trans);

            string template = "self.{0}{1} = self:get{2}(\"{3}\")";
            string name = string.Format("_{0}", trans.name);
            var type = GetNodeType(trans);
            string nameSuffix = type.ToString();
            string funcSuffix = luaFuncName[(int)type];
            string luaCode = string.Format(template, name, nameSuffix, funcSuffix, path);

            codes.AppendLine(luaCode);
            //Debug.Log(luaCode);
        }
        EditorGUIUtility.systemCopyBuffer = codes.ToString();
    }

    private static ExportUINode GetNodeType(Transform trans)
    {
        var btn = trans.GetComponent<Button>();
        var lb = trans.GetComponent<Text>();
        var img = trans.GetComponent<Image>();
        var type = ExportUINode.Go;
        if (btn != null) type = ExportUINode.Btn;
        else
        {
            if (img != null) type = ExportUINode.Img;
            else if (lb != null) type = ExportUINode.Lb;
        }
        return type;
    }

    static HashSet<string> validRoots;
    static string FindPath(Transform trans, bool includeUI = false)
    {
        if (validRoots == null)
        {
            validRoots = new HashSet<string>();
            validRoots.Add("HUD");
            validRoots.Add("HUDTOP");
            validRoots.Add("FULLSCREEN");
            validRoots.Add("POPUP");
            validRoots.Add("NOTIFY");
            validRoots.Add("NOTIFYTOP");
            validRoots.Add("TOP");
            validRoots.Add("TOPMOST");
            validRoots.Add("UIROOT");
        }
        var sb = new System.Text.StringBuilder();
        var nodes = new Stack<string>();

        while (trans != null &&
            !validRoots.Contains(trans.name))
        {
            nodes.Push(trans.name);
            trans = trans.parent;
        }
        //如果不包含根节点，则先去掉
        if (!includeUI && nodes.Count > 0) nodes.Pop();
        while (nodes.Count > 0)
        {
            sb.Append(nodes.Pop());
            if (nodes.Count > 0)
                sb.Append("/");
        }
        var path = sb.ToString();
        return path;
    }

    static Vector2 TargetResolution = new Vector2(1920, 1080);
    const string Str_TargetResolution = "1920*1080";
    static CanvasScaler scaler;

    [MenuItem("CONTEXT/RectTransform/根据当前分辨率适配" + Str_TargetResolution, priority = -1)]
    public static void AdjustForCurResolution()
    {
        var trans = Selection.activeTransform;
        AdjustForCurResolution(trans as RectTransform);
    }
    static void AdjustForCurResolution(RectTransform trans)
    {
        if (trans == null)
        {
            return;
        }
        var img = trans.GetComponent<Image>();
        if (img != null) img.SetNativeSize();

        if (scaler == null)
            scaler = GameObject.FindObjectOfType<CanvasScaler>();
        var refResolution = scaler.referenceResolution;

        var x = refResolution.x / TargetResolution.x;
        var y = refResolution.y / TargetResolution.y;

        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, trans.sizeDelta.x * x);
        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, trans.sizeDelta.y * y);
    }
    [MenuItem("GameObject/Ao/当前节点下所有ui根据当前分辨率适配" + Str_TargetResolution, priority = -3)]
    public static void AdjustUIForCurResolution()
    {
        var trans = Selection.activeTransform;
        var childs = trans.GetComponentInChildren<RectTransform>(true);
        foreach (RectTransform one in childs)
            AdjustForCurResolution(one);

    }

    #region 剧情
    [MenuItem("Assets/拷贝剧情资源路径")]
    public static void GetTaskRes()
    {
        var obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj);
        string copy = CopyUrl(path, "uitextures/", false);
        if (string.IsNullOrEmpty(copy))
            copy = CopyUrl(path, "prefabs/");
        Debug.Log(copy);
        EditorGUIUtility.systemCopyBuffer = copy;
    }

    static string CopyUrl(string url, string match, bool include = true)
    {
        var index = url.IndexOf(match);
        if (index < 0) return string.Empty;
        index += include ? 0 : match.Length;
        return url.Substring(index);
    }

    [MenuItem("Assets/拷贝剧情资源路径", true)]
    public static bool IsTaskRes()
    {
        var obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj);
        return path.Contains("uitextures") || path.Contains("prefabs");
    } 
    #endregion
}
