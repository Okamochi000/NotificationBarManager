using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ʒm�o�[
/// </summary>
public class NotificationBar : OpenAndCloseBase
{
    /// <summary>
    /// �J�A�j���[�V�������
    /// </summary>
    [System.Serializable]
    private struct AnimParam
    {
        [SerializeField] [Min(0.0f)] public float scaleTime;
        [SerializeField] [Min(0.0f)] public float fadeTime;

        public AnimParam(float scaleTime, float fadeTime)
        {
            this.scaleTime = scaleTime;
            this.fadeTime = fadeTime;
        }
    }

    public bool IsHighSpeed { get; set; } = false;

    [SerializeField] private AbbreviationText titleText = null;
    [SerializeField] private AbbreviationText messageText = null;
    [SerializeField] [Min(1.0f)] private float highSpeed = 2.0f;
    [SerializeField] [Min(0.1f)] private float aliveTime = 5.0f;
    [SerializeField] private AnimParam openParam = new AnimParam(0.5f, 0.2f);
    [SerializeField] private AnimParam closeParam = new AnimParam(0.5f, 0.2f);

    private RectTransform selfRectTransform_ = null;
    private CanvasGroup canvasGroup_ = null;
    private float playTime_ = 0.0f;
    private float tempHeight_ = 0.0f;
    private float speed_ = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (IsHighSpeed) { speed_ = highSpeed; }
        else { speed_ = 1.0f; }

        // ��莞�Ԍo�߂��������
        if (CurrentOpenState == OpenState.Opened)
        {
            playTime_ += (Time.deltaTime * speed_);
            if (playTime_ >= aliveTime)
            {
                Close();
                return;
            }
        }

        // �J�A�j���[�V����
        if (openAnimType != OpenAnimType.Script) { return; }
        if (CurrentOpenState == OpenState.OpenAnim)
        {
            // �J���A�j���[�V����

            // �T�C�Y�X�V
            float nextPlayTime = playTime_ + Time.deltaTime * speed_;
            if (playTime_ < openParam.scaleTime)
            {
                Vector2 sizeDelta = selfRectTransform_.sizeDelta;
                float easingIntarval = Mathf.Min(nextPlayTime, openParam.scaleTime) / openParam.scaleTime;
                sizeDelta.y = tempHeight_ * Easing(easingIntarval, 1.0f);
                selfRectTransform_.sizeDelta = sizeDelta;
                if (nextPlayTime >= openParam.scaleTime)
                {
                    if (titleText != null) { titleText.gameObject.SetActive(true); }
                    if (messageText != null) { messageText.gameObject.SetActive(true); }
                }
            }

            // �t�F�[�h�X�V
            playTime_ = nextPlayTime;
            if (nextPlayTime >= openParam.scaleTime)
            {
                if (openParam.fadeTime == 0.0f || nextPlayTime >= (openParam.scaleTime + openParam.fadeTime))
                {
                    // �t�F�[�h�I��
                    ChangeOpenState(OpenState.Opened);
                }
                else
                {
                    // �t�F�[�h�X�V
                    canvasGroup_.alpha = (nextPlayTime - openParam.scaleTime) / openParam.fadeTime;
                }
            }
        }
        else if (CurrentOpenState == OpenState.CloseAnim)
        {
            // ����A�j���[�V����

            // �t�F�[�h�X�V
            float nextPlayTime = playTime_ + Time.deltaTime * speed_;
            if (playTime_ < closeParam.fadeTime)
            {
                canvasGroup_.alpha = 1.0f - (Mathf.Min(nextPlayTime, closeParam.fadeTime) / closeParam.fadeTime);
                if (canvasGroup_.alpha == 0.0f)
                {
                    if (titleText != null) { titleText.gameObject.SetActive(false); }
                    if (messageText != null) { messageText.gameObject.SetActive(false); }
                }
            }

            // �T�C�Y�X�V
            playTime_ = nextPlayTime;
            if (nextPlayTime >= closeParam.fadeTime)
            {
                if (closeParam.scaleTime == 0.0f)
                {
                    // ����
                    ChangeOpenState(OpenState.Closed);
                }
                else
                {
                    // �T�C�Y�X�V
                    Vector2 sizeDelta = selfRectTransform_.sizeDelta;
                    float easingIntarval = 1.0f - Mathf.Min((nextPlayTime - closeParam.fadeTime), closeParam.scaleTime) / closeParam.scaleTime;
                    sizeDelta.y = tempHeight_ * Easing(easingIntarval, 1.0f);
                    selfRectTransform_.sizeDelta = sizeDelta;
                    if (nextPlayTime >= (closeParam.scaleTime + closeParam.fadeTime)) { ChangeOpenState(OpenState.Closed); }
                }
            }
        }
    }

    /// <summary>
    /// �^�C�g����ݒ肷��
    /// </summary>
    /// <param name="title"></param>
    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.SetText(title);
        }
    }

    /// <summary>
    /// ���b�Z�[�W��ݒ肷��
    /// </summary>
    /// <param name="message"></param>
    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.SetText(message);
        }
    }

    /// <summary>
    /// �J��ԕύX�R�[���o�b�N
    /// </summary>
    /// <param name="state"></param>
    protected override void OnChangedOpenState(OpenState state)
    {
        playTime_ = 0.0f;

        if (openAnimType != OpenAnimType.Script) { return; }

        if (canvasGroup_ == null) { canvasGroup_ = this.GetComponent<CanvasGroup>(); }
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }

        if (state == OpenState.OpenAnim)
        {
            canvasGroup_.alpha = 0.0f;
            if (titleText != null) { titleText.gameObject.SetActive(false); }
            if (messageText != null) { messageText.gameObject.SetActive(false); }
            if (tempHeight_ == 0.0f) { tempHeight_ = selfRectTransform_.sizeDelta.y; }
            Vector2 sizeDelta = selfRectTransform_.sizeDelta;
            if (openParam.scaleTime == 0.0f) { sizeDelta.y = tempHeight_; }
            else { sizeDelta.y = 0.0f; }
            selfRectTransform_.sizeDelta = sizeDelta;
        }
        else if (state == OpenState.Opened)
        {
            canvasGroup_.alpha = 1.0f;
            if (titleText != null) { titleText.gameObject.SetActive(true); }
            if (messageText != null) { messageText.gameObject.SetActive(true); }
            Vector2 sizeDelta = selfRectTransform_.sizeDelta;
            sizeDelta.y = tempHeight_;
            selfRectTransform_.sizeDelta = sizeDelta;
            tempHeight_ = 0.0f;
        }
        else if (state == OpenState.CloseAnim)
        {
            if (openParam.fadeTime == 0.0f)
            {
                canvasGroup_.alpha = 0.0f;
                if (titleText != null) { titleText.gameObject.SetActive(false); }
                if (messageText != null) { messageText.gameObject.SetActive(false); }
            }
            else
            {
                canvasGroup_.alpha = 1.0f;
                if (titleText != null) { titleText.gameObject.SetActive(true); }
                if (messageText != null) { messageText.gameObject.SetActive(true); }
            }
            if (tempHeight_ == 0.0f) { tempHeight_ = selfRectTransform_.sizeDelta.y; }
        }
        else if (state == OpenState.Closed)
        {
            canvasGroup_.alpha = 0.0f;
            if (titleText != null) { titleText.gameObject.SetActive(false); }
            if (messageText != null) { messageText.gameObject.SetActive(false); }
            Vector2 sizeDelta = selfRectTransform_.sizeDelta;
            sizeDelta.y = 0.0f;
            selfRectTransform_.sizeDelta = sizeDelta;
        }
    }

    /// <summary>
    /// �C�[�W���O��̒l���擾
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private float Easing(float t, float totaltime)
    {
        float min = 0.0f;
        float max = 1.0f;

        max -= min;
        t /= totaltime / 2;
        if (t < 1) return max / 2 * t * t + min;

        t = t - 1;
        return -max / 2 * (t * (t - 2) - 1) + min;

    }
}