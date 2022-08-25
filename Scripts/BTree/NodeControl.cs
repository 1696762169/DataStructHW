using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UISprite))]
public class NodeControl : MonoBehaviour
{
    private static GameObject number;
    private static int midWidth;
    private static int leftWidth;
    private static int rightWidth;
    public static int MidWidth => midWidth;
    public static int BorderWidth => leftWidth + rightWidth;

    /// <summary>
    /// ��ʼ��ȫ����Ϣ
    /// </summary>
    public static void Init()
    {
        GameObject node = Resources.Load<GameObject>("BTree/Node");
        UISprite sprite = node.GetComponent<UISprite>();
        UISpriteData data = sprite.atlas.GetSprite(sprite.spriteName);
        midWidth = data.width - data.borderRight - data.borderLeft;
        leftWidth = data.borderLeft;
        rightWidth = data.borderRight;

        number = Resources.Load<GameObject>("BTree/NumberLabel");
        UILabel label = number.GetComponent<UILabel>();
        label.width = midWidth;
        label.height = data.height;
        label.fontSize = midWidth / 4;
    }
    
    /// <summary>
    /// �����ڵ����
    /// </summary>
    /// <param name="node">�ڵ���Ϣ</param>
    public void CreateObject(BTree<int>.BTreeNode<int> node)
    {
        // ���������С
        UISprite sprite = GetComponent<UISprite>();
        int vc = node.values.Count;
        sprite.width = BorderWidth + midWidth * vc;

        // ���ɱ�ǩ�Ӷ���
        for (int i = 0; i < vc; ++i)
        {
            GameObject obj = Instantiate(number, transform);
            obj.transform.localPosition = new Vector3(midWidth * i - midWidth * vc / 2, 0, 0);
            obj.GetComponent<UILabel>().text = node.values[i].ToString();
        }
    }
}
