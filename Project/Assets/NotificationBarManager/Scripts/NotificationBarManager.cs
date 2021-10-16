using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通知バー管理
/// </summary>
public class NotificationBarManager : MonoBehaviourSingleton<NotificationBarManager>
{
    [SerializeField] [Min(1)] private int viewMaxCount = 2;
    [SerializeField] [Min(2)] private int highSpeedCount = 10;
    [SerializeField] [Min(1)] private int queueMaxCount = 50;
    [SerializeField] private GameObject barObj = null;
    [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup = null;

    private Queue<NotificationBar> notificationQueue_ = new Queue<NotificationBar>();
    private List<NotificationBar> activeNotification_ = new List<NotificationBar>();

    // Update is called once per frame
    void Update()
    {
        // 終了した通知を削除する
        activeNotification_.Remove(null);
        NotificationBar[] notifications = activeNotification_.ToArray();
        foreach (NotificationBar notification in notifications)
        {
            if (!notification.IsOpen())
            {
                activeNotification_.Remove(notification);
                Destroy(notification.gameObject);
            }
        }

        // 次の通知を表示する
        while (activeNotification_.Count < viewMaxCount && notificationQueue_.Count > 0)
        {
            NotificationBar notification = notificationQueue_.Dequeue();
            if (notification != null)
            {
                notification.Open();
                activeNotification_.Add(notification);
            }
        }

        // スピード設定
        bool isHighSpeed = false;
        if ((notificationQueue_.Count + activeNotification_.Count) >= highSpeedCount) { isHighSpeed = true; }
        foreach (NotificationBar notification in activeNotification_)
        {
            notification.IsHighSpeed = isHighSpeed;
        }

        // 開閉アニメーション中の場合レイアウトを更新
        foreach (NotificationBar notification in activeNotification_)
        {
            if (notification.CurrentOpenState == OpenAndCloseBase.OpenState.OpenAnim || notification.CurrentOpenState == OpenAndCloseBase.OpenState.CloseAnim)
            {
                layoutGroup.CalculateLayoutInputHorizontal();
                layoutGroup.CalculateLayoutInputVertical();
                layoutGroup.SetLayoutHorizontal();
                layoutGroup.SetLayoutVertical();
                break;
            }
        }
    }

    /// <summary>
    /// 通知追加
    /// </summary>
    public void AddNotification(string title, string message)
    {
        if (notificationQueue_.Count == queueMaxCount) { return; }
        GameObject bar = GameObject.Instantiate(barObj, barObj.transform.parent);
        NotificationBar notification = bar.GetComponent<NotificationBar>();
        notification.SetTitle(title);
        notification.SetMessage(message);
        notificationQueue_.Enqueue(notification);
    }

    /// <summary>
    /// 全ての通知を削除する
    /// </summary>
    public void ClearAll()
    {
        // 表示中の通知を閉じる
        activeNotification_.Remove(null);
        foreach (NotificationBar notification in activeNotification_.ToArray())
        {
            notification.Close();
        }

        // キューに溜まっている通知を削除する
        foreach (NotificationBar notification in notificationQueue_.ToArray())
        {
            if (notification != null)
            {
                Destroy(notification.gameObject);
            }
        }
        notificationQueue_.Clear();
    }
}
