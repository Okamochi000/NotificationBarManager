using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 開閉ベース
/// </summary>
public class OpenAndCloseBase : MonoBehaviour
{
    /// <summary>
    /// 開閉状態
    /// </summary>
    public enum OpenState
    {
        OpenAnim,
        Opened,
        CloseAnim,
        Closed
    }

    /// <summary>
    /// 開閉アニメーションの種類
    /// </summary>
    protected enum OpenAnimType
    {
        None,
        Animator,
        Script
    }

    /// <summary>
    /// 開閉操作の種類
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
        // Openを使用しないときの自動開閉
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
        // Closeを使用しないときの自動開閉
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
    /// 開く
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
                    // アニメーション無し
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
    /// 閉じる
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
                    // アニメーション無し
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
    /// 開閉状態を取得する
    /// </summary>
    /// <returns></returns>
    public bool IsOpen()
    {
        return (CurrentOpenState != OpenState.Closed);
    }

    /// <summary>
    /// 開閉状態切替
    /// </summary>
    /// <param name="state"></param>
    /// <param name="isCallback"></param>
    protected void ChangeOpenState(OpenState state, bool isCallback = true)
    {
        if (CurrentOpenState != state)
        {
            // 開閉状態切替
            CurrentOpenState = state;

            // 表示状態切替
            if (isAutoActive)
            {
                if (CurrentOpenState == OpenState.OpenAnim) { this.gameObject.SetActive(true); }
                else if (CurrentOpenState == OpenState.Closed) { this.gameObject.SetActive(false); }
            }

            // コールバック呼び出し
            if (isCallback) { OnChangedOpenState(state); }

            // 次の操作呼び出し
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
    /// 開閉状態変更コールバック
    /// </summary>
    /// <param name="state"></param>
    protected virtual void OnChangedOpenState(OpenState state) { }

    /// <summary>
    /// アニメーショントリガー
    /// </summary>
    /// <param name="eventName"></param>
    private void OnAnimationTrigger(string eventName)
    {
        if (eventName == "Open") { ChangeOpenState(OpenState.Opened, true); }
        else if (eventName == "Close") { ChangeOpenState(OpenState.Closed, true); }
    }
}
