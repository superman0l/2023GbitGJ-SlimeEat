using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Vector3> playerPos;
    [SerializeField] private List<Vector3> cameraPos;

    [SerializeField] private Transform camera;
    [SerializeField] private Transform player;

    private int levelPointer = 0;

    public void LevelChange(){
        levelPointer++;
        camera.position = cameraPos[levelPointer];
    }

    private void Start(){
        camera.position = cameraPos[levelPointer];
    }
    private void Update(){
        //level update
        if(levelPointer == 0 && player.position.x > 9.5f)
            LevelChange();

        //check player state
        if(levelPointer == 0 && player.position.y < -5.5f)
            player.position = playerPos[levelPointer];
    }
}
