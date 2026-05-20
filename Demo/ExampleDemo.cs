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

			LoadTableData();


		}

		async void LoadTableData()
		{
			LoaderGoogleSheet googleSheetLoader = new LoaderGoogleSheet("1pRpEq-zAwYvoB5N_D5H--NKltHOscvOcBu8uOAA3ph8");

			string sheetData = await googleSheetLoader.LoadAsync("ItemResourceData");
			LOG.trace(sheetData);
			ItemResourceData[] itemResourceDatas = CSVParser<ItemResourceData>.Parse(sheetData);
			LOG.trace(itemResourceDatas.Length);
			TableDataContainer<ItemResourceData>.Instance.InitDataList(itemResourceDatas);

			ItemResourceData item = TableDataContainer<ItemResourceData>.Instance.GetInfo("Item01");
			LOG.trace(item.Desc);
		}
	}
}
