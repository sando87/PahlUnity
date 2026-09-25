using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using PahlUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PahlUnity.Demo
{
    public class SceneUILoading : ScreenUIBase
    {
        [SerializeField] TextMeshProUGUI _LoadingText = null;
        [SerializeField] Image _FillImage = null;

        [SerializeField] LocalizationManager _ManagerA = null;

        // [SerializeField] ItemDatabase _ItemDB = null;

        public LocalizationManager ManagerA => _ManagerA;

        IEnumerator Start()
        {
            FadeIn(0.5f);

            _LoadingText.text = "Loading.";
            _FillImage.fillAmount = 0;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Loading..";
            _FillImage.fillAmount = 0.33f;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Loading...";
            _FillImage.fillAmount = 0.66f;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Done!!";
            _FillImage.fillAmount = 1;
            yield return newWaitForSeconds.Cache(0.2f);

            // InitializeGameSystem().Forget();

            FadeOut(0.5f);
            yield return newWaitForSeconds.Cache(0.5f);
            SceneSwitchManager.Instance.ChangeSceneAsync(SceneType.MainTitle).Forget();
        }

        async UniTask InitializeGameSystem()
        {
            ManagerA.gameObject.SetActive(true);
            (ManagerA as IInitializer).Initialize(null);

            IInitializer playerSaveDataManager = SaveManager<InGamePlayingData>.Instance as IInitializer;
            string filename = typeof(InGamePlayingData).Name + ".json";
            string fullPath = Path.Combine(Application.persistentDataPath, filename);
            InitializingState state = await playerSaveDataManager.InitializeAsync((new LocalFileIO(), fullPath), 10);
            if (state == InitializingState.InitializedSuccess)
            {
                EventManager.Instance.GlobalEvents.Register((SaveUserPlayData eventType) =>
                {
                    if (eventType.ImmediateSave)
                        SaveManager<InGamePlayingData>.Instance.SaveImmediate();
                    else
                        SaveManager<InGamePlayingData>.Instance.RequestSave();
                });
            }

            InitItemDatabase();

            // await LoadTableData<ItemResourceData>();
            // await LoadTableData<CharResourceData>();
            // await LoadTableData<SkillResourceData>();
            // await LoadTableData<EnemyResourceData>();
            // await LoadTableData<SpecOptionData>();
        }

        async UniTask LoadTableData<T>() where T : ITableRecord, new()
        {
            LoaderGoogleSheet googleSheetLoader = new LoaderGoogleSheet("1pRpEq-zAwYvoB5N_D5H--NKltHOscvOcBu8uOAA3ph8");
            string sheetname = typeof(T).Name;
            string sheetData = await googleSheetLoader.LoadAsync(sheetname);
            T[] resourceDatas = CSVParser<T>.Parse(sheetData);
            TableDataContainer<T>.Instance.InitDataList(resourceDatas);
        }


        void InitItemDatabase()
        {
            // LOG.errorif(_ItemDB == null);
            // TableDataContainer<ItemResourceData>.Instance.InitDataList(_ItemDB.ItemList.ToArray());
        }
    }
}

