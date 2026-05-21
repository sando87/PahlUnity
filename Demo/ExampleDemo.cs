using System.Collections;
using System.IO;
using UnityEngine;

namespace PahlUnity
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
			StartCoroutine(InitializeGameSystem());
		}

		IEnumerator InitializeGameSystem()
		{
			// ManagerA.SetActive(true);
			// (ManagerA as IInitializer).Initialize();
			yield return null;
			// ManagerB.SetActive(false);
			yield return null;
			// ManagerC.SetActive(false);

			string filename = typeof(SaveDataBase).Name + ".json";
			string fullPath = Path.Combine(Application.persistentDataPath, filename);
			(SaveManager<SaveDataBase>.Instance as IInitializer).Initialize((new LocalFileIO(), fullPath));

			LoadTableData<ItemResourceData>();
			LoadTableData<CharResourceData>();
			LoadTableData<SkillResourceData>();
			LoadTableData<EnemyResourceData>();
			LoadTableData<SpecOptionData>();
		}

		async void LoadTableData<T>() where T : ITableRecord, new()
		{
			LoaderGoogleSheet googleSheetLoader = new LoaderGoogleSheet("1pRpEq-zAwYvoB5N_D5H--NKltHOscvOcBu8uOAA3ph8");
			string sheetname = typeof(T).Name;
			string sheetData = await googleSheetLoader.LoadAsync(sheetname);
			T[] resourceDatas = CSVParser<T>.Parse(sheetData);
			TableDataContainer<T>.Instance.InitDataList(resourceDatas);
		}
	}
}
