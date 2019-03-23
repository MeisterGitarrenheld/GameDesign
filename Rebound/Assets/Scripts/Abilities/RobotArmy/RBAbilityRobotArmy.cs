using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityRobotArmy : ARBAbility
{
    private Transform _playerTransform;
    private RBAbilityRobotArmyServer _serverHandler;
    private Transform _robotSpawnPosition;
    private CharacterController _characterController;

    protected override void OnTrigger()
    {
        print("activated " + this.ToString());

        if (_playerTransform == null)
        {
            _playerTransform = gameObject.FindTagInParentsRecursively("Player").transform;
            _serverHandler = _playerTransform.GetComponent<RBAbilityRobotArmyServer>();
            _robotSpawnPosition = _playerTransform.Find("RobotArmySpawnPosition");
            _characterController = _playerTransform.GetComponent<CharacterController>();
        }

        StartCoroutine(SpawnRobots());
    }

    IEnumerator SpawnRobots()
    {
        for (int i = 0; i < 5; i++)
        {
            _serverHandler.SpawnRobot(_robotSpawnPosition.position, _playerTransform.rotation, _playerTransform.forward.normalized);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
