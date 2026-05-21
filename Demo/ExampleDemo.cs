using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
	public class ExampleDemo : SingletonMono<ExampleDemo>
	{
		[SerializeField] LocalizationManager _ManagerA = null;
		[SerializeField] GameObject _ManagerB = null;
		[SerializeField] GameObject _ManagerC = null;

		public LocalizationManager ManagerA => _ManagerA;
		public GameObject ManagerB => _ManagerB;
		public GameObject ManagerC => _ManagerC;

		void Start()
		{
			InitializeGameSystem().Forget();
		}

		async UniTask InitializeGameSystem()
		{
			// ManagerA.SetActive(true);
			// (ManagerA as IInitializer).Initialize();
			// yield return null;
			// ManagerB.SetActive(false);
			// yield return null;
			// ManagerC.SetActive(false);

			IInitializer playerSaveDataManager = SaveManager<PlayerSaveData>.Instance as IInitializer;
			string filename = typeof(PlayerSaveData).Name + ".json";
			string fullPath = Path.Combine(Application.persistentDataPath, filename);
			InitializingState state = await playerSaveDataManager.InitializeAsync((new LocalFileIO(), fullPath), 10);
			if (state == InitializingState.InitializedSuccess)
			{
				EventManager.Instance.GlobalEvents.Register((SaveUserPlayData eventType) =>
				{
					if (eventType.ImmediateSave)
						SaveManager<PlayerSaveData>.Instance.SaveImmediate();
					else
						SaveManager<PlayerSaveData>.Instance.RequestSave();
				});
			}

			await LoadTableData<ItemResourceData>();
			await LoadTableData<CharResourceData>();
			await LoadTableData<SkillResourceData>();
			await LoadTableData<EnemyResourceData>();
			await LoadTableData<SpecOptionData>();
		}

		async UniTask LoadTableData<T>() where T : ITableRecord, new()
		{
			LoaderGoogleSheet googleSheetLoader = new LoaderGoogleSheet("1pRpEq-zAwYvoB5N_D5H--NKltHOscvOcBu8uOAA3ph8");
			string sheetname = typeof(T).Name;
			string sheetData = await googleSheetLoader.LoadAsync(sheetname);
			T[] resourceDatas = CSVParser<T>.Parse(sheetData);
			TableDataContainer<T>.Instance.InitDataList(resourceDatas);
		}
	}
}
