using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private GameObject drone;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float speed;

    [SerializeField] private int playerID = 0;
    [SerializeField] private Rewired.Player player;
    
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = player.GetAxis("Horizontal");
        float moveVertical = player.GetAxis("Vertical");

        Vector3 movement = new Vector3(-moveHorizontal, 0, moveVertical);

        drone.transform.Translate(movement * speed);
        cameraTarget.transform.position = drone.transform.position;
    }
}
