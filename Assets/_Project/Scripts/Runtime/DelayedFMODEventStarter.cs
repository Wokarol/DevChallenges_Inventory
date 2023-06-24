using Cysharp.Threading.Tasks;
using FMODUnity;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Scripts.Runtime
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class DelayedFMODEventStarter : MonoBehaviour
    {
        private void Start()
        {

            StartAsync().Forget();
        }

        private async UniTask StartAsync()
        {
            var emitter = GetComponent<StudioEventEmitter>();

            await UniTask.WaitUntil(() => FMODUnity.RuntimeManager.HaveMasterBanksLoaded);

            emitter.Play();
        }
    }
}