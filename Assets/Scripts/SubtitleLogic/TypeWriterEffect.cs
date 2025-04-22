using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMP_Text))]
public class TypeWriterEffect : MonoBehaviour
{
    private TMP_Text _textBox;

    // Basic TypeWriter Functionality
    private int _currentVisibleCharacterIndex;
    private Coroutine _typewriteCoroutine;
    private bool _readyForNewText = true;

    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _interpunctualityDelay;

    [Header("TypeWriter Settings")]
    [SerializeField] private float characterPerSecond = 20;
    [SerializeField] private float interpunctuationDelay = 0.5f;

    // Skipping Functionality
    public bool CurrentlySkipping { get; private set; }
    private WaitForSeconds _skipDelay;

    [Header("Skip Options")]
    [SerializeField] private bool quickSkip;
    [SerializeField][Min(1)] private int skipSpeedup = 5;

    // Event Functionality
    private WaitForSeconds _textboxFullEventDelay;
    [SerializeField][Range(0.1f, 0.5f)] private float sendDoneDelay = 0.25f;

    public static event Action CompleteTextRevealed;
    public static event Action<char> CharacterRevealed;

    public bool allowskip = false;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
        _simpleDelay = new WaitForSeconds(1 / characterPerSecond);
        _interpunctualityDelay = new WaitForSeconds(interpunctuationDelay);
        _skipDelay = new WaitForSeconds(1 / (characterPerSecond * skipSpeedup));
        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);
    }

    public void StartTyping(string newText, float audioLength)
    {
        if (_typewriteCoroutine != null)
            StopCoroutine(_typewriteCoroutine);

        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;
        _textBox.text = newText;

        SetVoiceAudio(newText.Length, audioLength);
        _typewriteCoroutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;

            if (_currentVisibleCharacterIndex == lastCharacterIndex)
            {
                _textBox.maxVisibleCharacters++;
                yield return _textboxFullEventDelay;
                CompleteTextRevealed?.Invoke();
                _readyForNewText = true;
                yield break;
            }

            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;

            _textBox.maxVisibleCharacters++;

            if (CurrentlySkipping &&
                (character == '?' || character == '.' || character == ',' || character == ':'
                || character == ';' || character == '!' || character == '-'))
            {
                yield return _interpunctualityDelay;
            }
            else
            {
                yield return CurrentlySkipping ? _skipDelay : _simpleDelay;
            }

            CharacterRevealed?.Invoke(character);
            _currentVisibleCharacterIndex++;
        }
    }

    public void OnSkipingInteded()
    {
        if (_textBox.maxVisibleCharacters != _textBox.textInfo.characterCount - 1)
        {
            Skip();
        }
    }

    private void Skip()
    {
        if (CurrentlySkipping)
            return;

        CurrentlySkipping = true;

        if (!quickSkip)
        {
            StartCoroutine(SkipSpeedupReset());
            return;
        }

        StopCoroutine(_typewriteCoroutine);
        _textBox.maxVisibleCharacters = _textBox.textInfo.characterCount;
        _readyForNewText = true;
        CompleteTextRevealed?.Invoke();
    }

    private IEnumerator SkipSpeedupReset()
    {
        yield return new WaitUntil(() => _textBox.maxVisibleCharacters == _textBox.textInfo.characterCount - 1);
        CurrentlySkipping = false;
    }

    public void SetVoiceAudio(int dialogLength, float audioLength)
    {
        if (audioLength > 0)
        {
            characterPerSecond = dialogLength / audioLength; // Adjust type speed
            _simpleDelay = new WaitForSeconds(1 / characterPerSecond);
        }
        else
        {
            characterPerSecond = 20; // Default speed if no audio
            _simpleDelay = new WaitForSeconds(1 / characterPerSecond);
        }
    }
}
