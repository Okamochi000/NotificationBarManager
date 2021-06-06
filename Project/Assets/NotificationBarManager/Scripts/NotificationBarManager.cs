using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ʒm�o�[�Ǘ�
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
        // �I�������ʒm���폜����
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

        // ���̒ʒm��\������
        while (activeNotification_.Count < viewMaxCount && notificationQueue_.Count > 0)
        {
            NotificationBar notification = notificationQueue_.Dequeue();
            if (notification != null)
            {
                notification.Open();
                activeNotification_.Add(notification);
            }
        }

        // �X�s�[�h�ݒ�
        bool isHighSpeed = false;
        if ((notificationQueue_.Count + activeNotification_.Count) >= highSpeedCount) { isHighSpeed = true; }
        foreach (NotificationBar notification in activeNotification_)
        {
            notification.IsHighSpeed = isHighSpeed;
        }

        // �J�A�j���[�V�������̏ꍇ���C�A�E�g���X�V
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
    /// �ʒm�ǉ�
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
    /// �S�Ă̒ʒm���폜����
    /// </summary>
    public void ClearAll()
    {
        // �\�����̒ʒm�����
        activeNotification_.Remove(null);
        foreach (NotificationBar notification in activeNotification_.ToArray())
        {
            notification.Close();
        }

        // �L���[�ɗ��܂��Ă���ʒm���폜����
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
