using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 注：继承此类后需要去掉awake函数，否则会导致此类的awake失效，awake中的方法请写在OnStart方法中
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool Global = false;//单例在切换场景时是否保留
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {

        if (instance == null)
        {
            instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (Global) DontDestroyOnLoad(gameObject);
        OnStart();
    }
    protected virtual void OnStart()
    {

    }
}