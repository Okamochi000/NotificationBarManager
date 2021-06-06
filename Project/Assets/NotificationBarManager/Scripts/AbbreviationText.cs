using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ȗ��e�L�X�g
/// </summary>
public class AbbreviationText : UnityEngine.EventSystems.UIBehaviour
{
    /// <summary>
    /// �ȗ��
    /// </summary>
    public enum LimitType
    {
        ParentRect, // �e�T�C�Y�𒴂���Ƃ��X�N���[��
        WordCount   // �w�蕶�����𒴂���Ƃ��X�N���[��
    }

    public Text TargetText { get { return targetText; } }

    [SerializeField] private Text targetText = null;
    [SerializeField] private LimitType limitType = LimitType.ParentRect;
    [SerializeField] private string abbreviationLastText = "�c";
    [SerializeField] [Min(1)] private int characterLimit = 8;

    private string originalText_ = "";

    /// <summary>
    /// �e�L�X�g�ݒ�
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
            // �T�C�Y�𒴂���ꍇ�͏ȗ�����
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

                // ��Ԃɗ]�T������ꍇ�A���̈ꕶ��ǂݍ���
                if (targetText.preferredHeight + targetText.fontSize * 2.0f <= maxHeight)
                {
                    currentText += "\n";
                    continue;
                }

                // ���s�O�̕��̓r���ŏȗ�
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

                // ���s���O�̏�Ԃŏȗ�
                targetText.text = currentText + "\n" + abbreviationLastText;
                if (targetText.preferredHeight > maxHeight)
                {
                    targetText.text = (currentText + abbreviationLastText);
                    return;
                }

                // ���s��̏�Ԃŏȗ�
                if ((i + 1) == splits.Length)
                {
                    return;
                }

                // ���s��̕��̓r���ŏȗ�
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
            // �����������ȏ�Ȃ�ȗ�����
            if (originalText_.Length > characterLimit)
            {
                string abbreviationtext = originalText_.Substring(0, characterLimit);
                targetText.text = abbreviationtext + abbreviationLastText;
            }
        }
    }

    /// <summary>
    /// �e�L�X�g�擾
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
