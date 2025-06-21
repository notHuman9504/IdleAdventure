using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelChanger : MonoBehaviour
{
    [SerializeField]
    private LevelConnection _connection;

    [SerializeField]
    private string _targetSceneName;

    [SerializeField]
    private Transform _spawnPoint;

    private void Start()
    {
        if (_connection == LevelConnection.ActiveConnection)
        {
            FindObjectOfType<PlayerMovement>().transform.position = _spawnPoint.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            LevelConnection.ActiveConnection = _connection;
            //SceneManager.LoadScene(_targetSceneName);
        }
    }


}