using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager> {

    private Dictionary<string, GameObject> UIDict;
    private Dictionary<string, string> UIPathDict;

	public override void Init()
	{
        UIDict = new Dictionary<string, GameObject>();
        UIPathDict = new Dictionary<string, string>();
        UIPathDict.Add("Panel2", "Panel2");
    }

    public GameObject GetUI(string name)
    {
        if (UIPathDict.ContainsKey(name) == false)
        {
            return null;
        }

        if (UIDict.ContainsKey(name) == false)
        {
            GameObject go = GameObject.Instantiate(Resources.Load(UIPathDict[name], typeof(GameObject))) as GameObject;
            go.transform.SetParent(GameObject.Find("UICamera").transform);
            go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
            go.name = name;
            UIDict.Add(name, go);
            return go;            
        }
        return UIDict[name];
    }

    public void DestroyUI(string name)
    {
        if (UIDict.ContainsKey(name) == false)
        {
            return;
        }

        GameObject.Destroy(UIDict[name]);
        UIDict.Remove(name);
    }
}
