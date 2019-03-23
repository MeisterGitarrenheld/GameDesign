using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityWall : ARBAbility {

    public int Duration = 8;

    private bool _previewActive = false;

    [SerializeField]
    private GameObject _previewPrefab;
    private GameObject _previewObject;

    private GameObject _playerObject;
    private bool _placeWall = false;

    protected override void OnTrigger()
    {
        if (_playerObject == null)
            _playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);

        if (!_previewActive)
        {
            PauseCooldown = _previewActive = true;
            _previewObject = Instantiate(_previewPrefab);
        }
        else
        {
            _placeWall = true;
            PauseCooldown = _previewActive = false;
        }
    }

    protected override void UpdateAbility()
    {
        if(_previewActive)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            foreach(var hit in Physics.RaycastAll(ray))
            {
                if(hit.transform.name == Terrain.activeTerrain.name)
                {
                    _previewObject.transform.position = hit.point;
                    _previewObject.transform.rotation = _playerObject.transform.rotation;
                    break;
                }
            }
        }
        if(_placeWall)
        {
            // spawn wall on server
            _playerObject.GetComponent<RBAbilityWallServer>().SpawnWall(_previewObject.transform.position, _previewObject.transform.rotation, Duration);
            Destroy(_previewObject);
            _placeWall = false;
        }
    }
}
