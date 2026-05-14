using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PahlUnity;
using UnityEngine;

/// <summary>
/// 현지화 언어로 번역 관련 처리.
/// 언어 테이블 파일 로딩 및 번역 수행
/// LanguageType의 Enum필드명과 테이블 파일의 헤더과 동일해야함.
/// </summary>

namespace PahlUnity
{
    public class LocalizationManager : SingletonMono<LocalizationManager>, IInitializer
    {
        [SerializeField] TextAsset _LanguageTableAsset = null;

        public event Action<LanguageType> EventChanged;

        public LanguageType CurrentLanguage { get; private set; } = LanguageType.None;

        Dictionary<LanguageType, LocaleSet> mLangTable = new Dictionary<LanguageType, LocaleSet>();

        InitializingState IInitializer.Initialize()
        {
            string languageText = _LanguageTableAsset.text;
            LocaleInfoRaw[] infos = CSVParser<LocaleInfoRaw>.Parse(languageText);

            mLangTable.Clear();

            foreach (LanguageType langType in Enum.GetValues(typeof(LanguageType)))
            {
                if (langType == LanguageType.None)
                {
                    continue;
                }

                LocaleSet localeSet = new LocaleSet();

                foreach (LocaleInfoRaw info in infos)
                {
                    localeSet.LocaledText.Add(info.ID, info.GetText(langType));
                }

                mLangTable.Add(langType, localeSet);
            }

            CurrentLanguage = GetSystemOSLanguage();

            return InitializingState.InitializedSuccess;
        }

        public string GetLocaledText(long id)
        {
            return mLangTable[CurrentLanguage].LocaledText[id];
        }

        public void ChangeLanguage(LanguageType language)
        {
            CurrentLanguage = language;
            EventChanged?.Invoke(language);
        }

        // 게임 초기 랭귀지 설정이 안되어 있는 경우 시스템 언어정보를 읽어온다.
        public static LanguageType GetSystemOSLanguage()
        {
            // 현재 시스템 언어를 읽어옴
            SystemLanguage sl = Application.systemLanguage;

            LanguageType langType = LanguageType.English;
            switch (sl)
            {
                case SystemLanguage.English: langType = LanguageType.English; break;
                case SystemLanguage.Japanese: langType = LanguageType.Japanese; break;
                case SystemLanguage.Korean: langType = LanguageType.Korean; break;
                case SystemLanguage.Portuguese: langType = LanguageType.Portuguese; break;
                case SystemLanguage.Russian: langType = LanguageType.Russian; break;
                case SystemLanguage.ChineseSimplified: langType = LanguageType.ChineseSimplified; break;
                case SystemLanguage.Spanish: langType = LanguageType.Spanish; break;
                case SystemLanguage.ChineseTraditional: langType = LanguageType.ChineseTraditional; break;
                case SystemLanguage.German: langType = LanguageType.German; break;
                case SystemLanguage.French: langType = LanguageType.French; break;
                case SystemLanguage.Italian: langType = LanguageType.Italian; break;
                case SystemLanguage.Indonesian: langType = LanguageType.Indonesian; break;
                default: langType = LanguageType.English; break;
            }
            return langType;
        }

    }

    public class LocaleSet
    {
        public Dictionary<long, string> LocaledText = new Dictionary<long, string>();

    }

    public class LocaleInfoRaw : ITableRecord
    {
        public int code = 0;
        public string division = "";
        public string note = "";
        public string English = "";
        public string Korean = "";
        public string Japanese = "";
        public string ChineseTraditional = "";
        public string ChineseSimplified = "";
        public string Spanish = "";
        public string Portuguese = "";
        public string Russian = "";
        public string German = "";
        public string French = "";
        public string Italian = "";
        public string Indonesian = "";

        public int RowIndex { get; set; }
        public long ID { get { return (long)code; } }

        public string GetText(LanguageType language)
        {
            switch (language)
            {
                case LanguageType.English: return English;
                case LanguageType.Korean: return Korean;
                case LanguageType.Japanese: return Japanese;
                case LanguageType.ChineseTraditional: return ChineseTraditional;
                case LanguageType.ChineseSimplified: return ChineseSimplified;
                case LanguageType.Spanish: return Spanish;
                case LanguageType.Portuguese: return Portuguese;
                case LanguageType.Russian: return Russian;
                case LanguageType.German: return German;
                case LanguageType.French: return French;
                case LanguageType.Italian: return Italian;
                case LanguageType.Indonesian: return Indonesian;
                default: return English;
            }
        }
    }

    // 지원하는 언어 종류
    public enum LanguageType
    {
        None,
        English,
        Korean,
        Japanese,
        ChineseSimplified,
        ChineseTraditional,
        Spanish,
        Portuguese,
        Russian,
        German,
        French,
        Italian,
        Indonesian
    }
}