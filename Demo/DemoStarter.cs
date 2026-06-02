using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public class DemoStarter : SingletonMono<DemoStarter>
	{
		[SerializeField] LocalizationManager _ManagerA = null;
		[SerializeField] GameObject _ManagerB = null;
		[SerializeField] GameObject _ManagerC = null;

		// [SerializeField] ItemDatabase _ItemDB = null;

		public LocalizationManager ManagerA => _ManagerA;
		public GameObject ManagerB => _ManagerB;
		public GameObject ManagerC => _ManagerC;

		void Start()
		{
			InitializeGameSystem().Forget();
		}

		async UniTask InitializeGameSystem()
		{
			ManagerA.gameObject.SetActive(true);
			(ManagerA as IInitializer).Initialize(null);
			ManagerB.SetActive(false);
			ManagerC.SetActive(false);

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
