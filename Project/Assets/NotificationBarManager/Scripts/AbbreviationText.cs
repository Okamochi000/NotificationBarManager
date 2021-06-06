using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 省略テキスト
/// </summary>
public class AbbreviationText : UnityEngine.EventSystems.UIBehaviour
{
    /// <summary>
    /// 省略基準
    /// </summary>
    public enum LimitType
    {
        ParentRect, // 親サイズを超えるときスクロール
        WordCount   // 指定文字数を超えるときスクロール
    }

    public Text TargetText { get { return targetText; } }

    [SerializeField] private Text targetText = null;
    [SerializeField] private LimitType limitType = LimitType.ParentRect;
    [SerializeField] private string abbreviationLastText = "…";
    [SerializeField] [Min(1)] private int characterLimit = 8;

    private string originalText_ = "";

    /// <summary>
    /// テキスト設定
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        originalText_ = text;

        if (targetText == null) { return; }
        targetText.text = originalText_;

        if (!this.gameObject.activeInHierarchy) { return; }

        if (limitType == LimitType.ParentRect)
        {
            // サイズを超える場合は省略する
            float maxHeight = targetText.rectTransform.rect.size.y;
            if (targetText.preferredHeight <= maxHeight) { return; }

            string[] splits = originalText_.Replace("\r", "\n").Split('\n');
            string currentText = "";
            float maxWidth = targetText.rectTransform.rect.size.x;
            for (int i = 0; i < splits.Length; i++)
            {
                string split = splits[i];
                string prevText = currentText;
                currentText += split;
                targetText.text = currentText + abbreviationLastText;

                // 空間に余裕がある場合、次の一文を読み込む
                if (targetText.preferredHeight + targetText.fontSize * 2.0f <= maxHeight)
                {
                    currentText += "\n";
                    continue;
                }

                // 改行前の文の途中で省略
                if (targetText.preferredHeight > maxHeight)
                {
                    targetText.text = prevText;
                    float prevPreferredHeight = targetText.preferredHeight;
                    int lineCount = (int)((maxHeight - prevPreferredHeight) / targetText.fontSize) + 1;
                    maxWidth = maxWidth * lineCount;
                    targetText.text = currentText + abbreviationLastText;
                    float preferredWidth = targetText.preferredWidth;
                    int charaLength = (int)(maxWidth / (preferredWidth / (float)split.Length)) - abbreviationLastText.Length;
                    if (charaLength >= 0 && charaLength < split.Length)
                    {
                        targetText.text = prevText + split.Substring(0, charaLength) + abbreviationLastText;
                        if (targetText.preferredHeight > maxHeight)
                        {
                            while (targetText.preferredHeight > maxHeight && charaLength > 1)
                            {
                                charaLength--;
                                targetText.text = prevText + split.Substring(0, charaLength) + abbreviationLastText;
                            }
                        }
                        else if (targetText.preferredHeight < maxHeight)
                        {
                            while (targetText.preferredHeight < maxHeight && charaLength < (split.Length - 1))
                            {
                                charaLength++;
                                targetText.text = prevText + split.Substring(0, charaLength) + abbreviationLastText;
                            }
                            charaLength--;
                            targetText.text = prevText + split.Substring(0, charaLength) + abbreviationLastText;
                        }
                    }
                    return;
                }

                // 改行直前の状態で省略
                targetText.text = currentText + "\n" + abbreviationLastText;
                if (targetText.preferredHeight > maxHeight)
                {
                    targetText.text = (currentText + abbreviationLastText);
                    return;
                }

                // 改行後の状態で省略
                if ((i + 1) == splits.Length)
                {
                    return;
                }

                // 改行後の文の途中で省略
                {
                    currentText += "\n";
                    split = splits[(i + 1)];
                    targetText.text = split;
                    int charaLength = (int)(maxWidth / (targetText.preferredWidth / (float)split.Length)) - abbreviationLastText.Length;
                    if (charaLength >= 0 && charaLength < split.Length)
                    {
                        targetText.text = currentText + split.Substring(0, charaLength) + abbreviationLastText;
                        if (targetText.preferredHeight > maxHeight)
                        {
                            while (targetText.preferredHeight > maxHeight)
                            {
                                charaLength--;
                                targetText.text = currentText + split.Substring(0, charaLength) + abbreviationLastText;
                            }
                        }
                        else if (targetText.preferredHeight < maxHeight)
                        {
                            while (targetText.preferredHeight < maxHeight)
                            {
                                charaLength++;
                                targetText.text = currentText + split.Substring(0, charaLength) + abbreviationLastText;
                            }
                            charaLength--;
                            targetText.text = currentText + split.Substring(0, charaLength) + abbreviationLastText;
                        }
                    }
                    else
                    {
                        targetText.text = currentText + split + abbreviationLastText;
                    }
                }

                return;
            }
        }
        else if (limitType == LimitType.WordCount)
        {
            // 制限文字数以上なら省略する
            if (originalText_.Length > characterLimit)
            {
                string abbreviationtext = originalText_.Substring(0, characterLimit);
                targetText.text = abbreviationtext + abbreviationLastText;
            }
        }
    }

    /// <summary>
    /// テキスト取得
    /// </summary>
    /// <returns></returns>
    public string GetOriginalText()
    {
        if (originalText_ == "")
        {
            if (targetText != null)
            {
                return targetText.text;
            }
        }

        return originalText_;
    }

    protected override void Awake()
    {
        SetText(GetOriginalText());
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetText(GetOriginalText());
    }
}
