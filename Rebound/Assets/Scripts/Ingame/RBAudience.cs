using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAudience : MonoBehaviour {

    public Sprite[] AudienceSprite;

    private Vector3 InitPosition;
    private Quaternion InitRotation;

    private float randomTimer;
    private float timer;

    private Rigidbody rb;

    [HideInInspector]
    public float MinJumpTime;
    public bool Jump;

	void Start ()
    {
        InitPosition = transform.position;
        InitRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        GetComponent<SpriteRenderer>().sprite = AudienceSprite[Random.Range(0, AudienceSprite.Length)];
        transform.localScale = new Vector3(Mathf.Max(0, (Random.Range(0, 2) * 2) - 1), 1, 1) * transform.localScale.x;
        MinJumpTime = 5f;
        randomTimer = Random.Range(MinJumpTime, MinJumpTime + 10f);
    }
	
	void Update ()
    {
		if(timer > randomTimer || Jump)
        {
            timer = 0;
            randomTimer = Random.Range(MinJumpTime, MinJumpTime + 10f);

            rb.AddForce(Vector3.up * Random.Range(50, 100), ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100)), ForceMode.Impulse);
            MinJumpTime = 5f;
            Jump = false;
        }
        else if( timer > randomTimer / 2f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, InitPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, InitRotation, 0.1f);
        }
        timer += Time.deltaTime;
	}




}
