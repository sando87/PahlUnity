#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity
{
    public class SaveManager<T> : Singleton<SaveManager<T>>, IInitializer where T : SaveDataBase, new()
    {
        private InitializingState mInitState = InitializingState.NotInitialized;
        InitializingState IInitializer.InitState => mInitState;

        private ITextDataProvider mLoader = null;
        private string mFullPath = "";

        private T mSaveData = null;
        public T SaveData => mSaveData;

        private float mSaveRequestedTime = 0;
        private float mLastSaveTime = 0;
        private bool mIsSaving = false;

        public void SaveImmediate()
        {
            SaveAsync().Forget();
        }

        // 바로 세이브 하지 않고 일정 시간 동안 다시 RequestSave() 호출이 없으면 그때 세이브 하는 방식
        // 골드나 경험치 같이 자주 변경되는 데이터를 위한 세이브 방식
        public void RequestSave()
        {
            bool isAlreadyRequested = mSaveRequestedTime != 0;
            mSaveRequestedTime = Time.time;
            if (!isAlreadyRequested)
            {
                DoSaveWhenIdle().Forget();
            }
        }

        public bool IsIdleFromRequested => Time.time - mSaveRequestedTime > 3.0f;

        async UniTask DoSaveWhenIdle()
        {
            while (true)
            {
                await UniTask.WaitUntil(() => IsIdleFromRequested);

                await SaveAsync();

                if (IsIdleFromRequested)
                {
                    mSaveRequestedTime = 0;
                    break;
                }
            }
        }

        private async UniTask SaveAsync()
        {
            if (mInitState != InitializingState.InitializedSuccess &&
                mInitState != InitializingState.Initializing)
                return;

            try
            {
                await UniTask.WaitUntil(() => !mIsSaving);
                mIsSaving = true;
                mSaveData.RandomValue = UnityEngine.Random.Range(0, int.MaxValue);
                mSaveData.SaveTimeUtcTicks = System.DateTime.UtcNow.Ticks;
                mSaveData.PlayTimeSeconds += (Time.time - mLastSaveTime);
                mLastSaveTime = Time.time;

                string saveStrData = JsonParser.ToJson(mSaveData);
#if !IS_DEBUG_MODE
                // 릴리즈 모드일때는 암호화
                saveStrData = MyDecoding.Encrypt(saveStrData);
#endif
                await mLoader.SaveAsync(mFullPath, saveStrData).Timeout(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                LOG.log(ex.Message);
            }
            finally
            {
                mIsSaving = false;
            }
        }

        async UniTask<InitializingState> IInitializer.InitializeAsync(object param, float timeout)
        {
            if (param is (ITextDataProvider provider, string fullPath))
            {
                mLoader = provider;
                mFullPath = fullPath;
            }
            else
            {
                mInitState = InitializingState.InitializedFailed;
                return mInitState;
            }

            mInitState = InitializingState.Initializing;

            try
            {
                if (mLoader.IsExist(mFullPath))
                {
                    string loadedData = await mLoader.LoadAsync(mFullPath).Timeout(TimeSpan.FromSeconds(timeout));
#if !IS_DEBUG_MODE
                    // 릴리즈 모드일때는 복호화
                    loadedData = MyDecoding.Decrypt(loadedData);
#endif
                    mSaveData = JsonParser.FromJson<T>(loadedData);

                    if (mSaveData.FileVersion != SaveDataBase.LastestFileVersion)
                    {
                        mSaveData.OnVersionMismatch();
                    }
                }
                else
                {
                    mSaveData = new T();
                }

                mSaveData.FileVersion = SaveDataBase.LastestFileVersion;
                mSaveData.LoginCount++;
                mLastSaveTime = Time.time;
                await SaveAsync();
            }
            catch (Exception ex)
            {
                LOG.log(ex.Message);
                mLoader = null;
                mFullPath = "";
                mInitState = InitializingState.InitializedFailed;
                return mInitState;
            }

            mInitState = InitializingState.InitializedSuccess;
            return mInitState;
        }
    }

    [System.Serializable]
    public class SaveDataBase
    {
        public const int LastestFileVersion = 1;

        public int FileVersion;
        public int RandomValue;
        public long SaveTimeUtcTicks;
        public float PlayTimeSeconds;
        public int LoginCount;

        public bool IsFirstPlay => LoginCount == 1;

        public virtual void OnVersionMismatch() { }

        public SaveDataBase()
        {
            FileVersion = LastestFileVersion;
            RandomValue = UnityEngine.Random.Range(0, int.MaxValue);
            SaveTimeUtcTicks = System.DateTime.UtcNow.Ticks;
            PlayTimeSeconds = 0;
            LoginCount = 0;
        }
    }
}