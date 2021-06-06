using System;
using UnityEngine;

public class AutoNotificationSample : MonoBehaviour
{
    [SerializeField] private float notificationInterval = 2.0f;

    private float playTime_ = 0.0f;

    // Update is called once per frame
    void Update()
    {
        int num = (int)(playTime_ / notificationInterval);
        playTime_ += Time.deltaTime;
        int nextNum = (int)(playTime_ / notificationInterval);

        if (num != nextNum)
        {
            string message = String.Format("���̃��b�Z�[�W��[{0}]�ɑ��M����܂���\n\n���b�Z�[�W�̓L���[�ɗ��܂�܂�", DateTime.Now);
            NotificationBarManager.Instance.AddNotification("�e�X�g", message);
        }
    }
}
