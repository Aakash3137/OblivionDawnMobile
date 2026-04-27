using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class AddressableUnitTest : MonoBehaviour
{
    async void Start()
    {
        Debug.Log("Addressable start runs");

        var handle = Addressables.LoadAssetAsync<GameObject>("00 Fast Automobile Drones");
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Loaded successfully!");
            Instantiate(handle.Result, Vector3.zero, Quaternion.identity);
            Addressables.Release(handle);
        }
        else
        {
            Debug.LogError("Failed to load addressable!");
        }
    }
}