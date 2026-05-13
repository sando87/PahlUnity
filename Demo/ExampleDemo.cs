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
			LOG.trace();
			// ManagerA.SetActive(true);
			(ManagerA as IInitializer).Initialize();
			yield return null;
			// ManagerB.SetActive(false);
			yield return null;
			// ManagerC.SetActive(false);
			LOG.trace();
		}
	}
}
