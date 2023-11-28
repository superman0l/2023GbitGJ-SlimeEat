using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ע���̳д������Ҫȥ��awake����������ᵼ�´����awakeʧЧ��awake�еķ�����д��OnStart������
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool Global = false;//�������л�����ʱ�Ƿ���
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