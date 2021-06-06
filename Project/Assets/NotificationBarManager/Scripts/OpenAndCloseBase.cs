using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �J�x�[�X
/// </summary>
public class OpenAndCloseBase : MonoBehaviour
{
    /// <summary>
    /// �J���
    /// </summary>
    public enum OpenState
    {
        OpenAnim,
        Opened,
        CloseAnim,
        Closed
    }

    /// <summary>
    /// �J�A�j���[�V�����̎��
    /// </summary>
    protected enum OpenAnimType
    {
        None,
        Animator,
        Script
    }

    /// <summary>
    /// �J����̎��
    /// </summary>
    private enum OpenActionType
    {
        None,
        Open,
        Close
    }

    public OpenState CurrentOpenState { get; private set; } = OpenState.Closed;

    [SerializeField] protected OpenAnimType openAnimType = OpenAnimType.None;
    [SerializeField] private bool isAutoActive = true;

    private OpenActionType nextOpenAction_ = OpenActionType.None;

    public virtual void OnEnable()
    {
        // Open���g�p���Ȃ��Ƃ��̎����J��
        if (isAutoActive)
        {
            if (this.gameObject.activeSelf && CurrentOpenState == OpenState.Closed)
            {
                Open();
            }
        }
    }

    public virtual void OnDisable()
    {
        // Close���g�p���Ȃ��Ƃ��̎����J��
        if (isAutoActive)
        {
            if (!this.gameObject.activeSelf)
            {
                Close();
            }
        }

        if (CurrentOpenState != OpenState.Closed)
        {
            if (CurrentOpenState == OpenState.OpenAnim) { ChangeOpenState(OpenState.Opened); }
            if (CurrentOpenState == OpenState.Opened) { ChangeOpenState(OpenState.CloseAnim); }
            if (CurrentOpenState == OpenState.CloseAnim) { ChangeOpenState(OpenState.Closed); }
            nextOpenAction_ = OpenActionType.None;
        }
    }

    /// <summary>
    /// �J��
    /// </summary>
    public void Open()
    {
        if (CurrentOpenState == OpenState.Closed)
        {
            nextOpenAction_ = OpenActionType.None;
            ChangeOpenState(OpenState.OpenAnim, true);
            if (this.gameObject.activeInHierarchy)
            {
                if (openAnimType == OpenAnimType.None)
                {
                    // �A�j���[�V��������
                    ChangeOpenState(OpenState.Opened, true);
                }
                else if (openAnimType == OpenAnimType.Animator)
                {
                    // Animator
                    Animator animator = this.GetComponent<Animator>();
                    if (animator == null || animator.runtimeAnimatorController == null) { ChangeOpenState(OpenState.Opened, true); }
                }
            }
            else
            {
                ChangeOpenState(OpenState.Opened, true);
            }
        }
        else
        {
            nextOpenAction_ = OpenActionType.Open;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Close()
    {
        if (CurrentOpenState == OpenState.Opened)
        {
            nextOpenAction_ = OpenActionType.None;
            ChangeOpenState(OpenState.CloseAnim, true);
            if (this.gameObject.activeInHierarchy)
            {
                if (openAnimType == OpenAnimType.None)
                {
                    // �A�j���[�V��������
                    ChangeOpenState(OpenState.Closed, true);
                }
                else if (openAnimType == OpenAnimType.Animator)
                {
                    // Animator
                    Animator animator = this.GetComponent<Animator>();
                    if (animator == null || animator.runtimeAnimatorController == null) { ChangeOpenState(OpenState.Closed, true); }
                }
            }
            else
            {
                ChangeOpenState(OpenState.Opened, true);
            }
        }
        else
        {
            nextOpenAction_ = OpenActionType.Close;
        }
    }

    /// <summary>
    /// �J��Ԃ��擾����
    /// </summary>
    /// <returns></returns>
    public bool IsOpen()
    {
        return (CurrentOpenState != OpenState.Closed);
    }

    /// <summary>
    /// �J��Ԑؑ�
    /// </summary>
    /// <param name="state"></param>
    /// <param name="isCallback"></param>
    protected void ChangeOpenState(OpenState state, bool isCallback = true)
    {
        if (CurrentOpenState != state)
        {
            // �J��Ԑؑ�
            CurrentOpenState = state;

            // �\����Ԑؑ�
            if (isAutoActive)
            {
                if (CurrentOpenState == OpenState.OpenAnim) { this.gameObject.SetActive(true); }
                else if (CurrentOpenState == OpenState.Closed) { this.gameObject.SetActive(false); }
            }

            // �R�[���o�b�N�Ăяo��
            if (isCallback) { OnChangedOpenState(state); }

            // ���̑���Ăяo��
            if (CurrentOpenState == OpenState.Opened)
            {
                if (nextOpenAction_ == OpenActionType.Close) { Close(); }
                nextOpenAction_ = OpenActionType.None;
            }
            else if (CurrentOpenState == OpenState.Closed)
            {
                if (nextOpenAction_ == OpenActionType.Open) { Open(); }
                nextOpenAction_ = OpenActionType.None;
            }
        }
    }

    /// <summary>
    /// �J��ԕύX�R�[���o�b�N
    /// </summary>
    /// <param name="state"></param>
    protected virtual void OnChangedOpenState(OpenState state) { }

    /// <summary>
    /// �A�j���[�V�����g���K�[
    /// </summary>
    /// <param name="eventName"></param>
    private void OnAnimationTrigger(string eventName)
    {
        if (eventName == "Open") { ChangeOpenState(OpenState.Opened, true); }
        else if (eventName == "Close") { ChangeOpenState(OpenState.Closed, true); }
    }
}
