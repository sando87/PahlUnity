using System.Collections;
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
