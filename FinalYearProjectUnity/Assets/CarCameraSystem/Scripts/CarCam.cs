//Special Thanks to Why485 for providing the Simple Car Camera Script
//https://www.youtube.com/watch?v=VyOGduSxyqk
//https://www.youtube.com/channel/UCzmQI4UxbxgXH-fTQc_UEjQ

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CarCam : MonoBehaviour
{
    Transform rootNode;
    Transform carCam;
    Transform car;
    Rigidbody carPhysics;
	Camera mainCamera;

	[Header("Cameras")]
	public List<CameraPosition> cameraPositions;

	[Header("General Settings")]
	[Tooltip("The Axis/Button which is defined for changing the camera perspective")]
	public string cameraAxisName = "Camera";

	[TagSelector]
	[Tooltip("The camera will shake if the car hits a Object with this tag")]
	public string cameraShakeTag = "";

	[Tooltip("At which speed is the maximum of shaking reached")]
	public float cameraShakeMaxSpeed = 30.0f;

	[HideInInspector]public int camIndex;
	private bool isShaking;

	void Start()
    {
		carCam = Camera.main.GetComponent<Transform>();
		mainCamera = Camera.main;
		rootNode = GetComponent<Transform>();
		car = rootNode.parent.GetComponent<Transform>();
		carPhysics = car.GetComponent<Rigidbody>();

		// Detach the camera so that it can move freely on its own.
        rootNode.parent = null;
    }

    void FixedUpdate()
    {
		if (!cameraPositions [camIndex].isStatic) {
			Quaternion look;
			Vector3 camOffset;

			camOffset = new Vector3 (cameraPositions [camIndex].xOffset, cameraPositions [camIndex].yOffset, cameraPositions [camIndex].zOffset);

			// Moves the camera to match the car's position.
			rootNode.position = Vector3.Lerp (rootNode.position, car.position, cameraPositions [camIndex].cameraStickiness * Time.fixedDeltaTime);

			Vector2 noise = new Vector2 ();

			float speedAmplitude = Mathf.Clamp(carPhysics.velocity.magnitude, 0.0f, cameraShakeMaxSpeed) / cameraShakeMaxSpeed;

			if (isShaking)
				noise = NoiseGen.Shake2D (cameraPositions [camIndex].amplitude * speedAmplitude, cameraPositions [camIndex].frequency, cameraPositions [camIndex].octaves, cameraPositions [camIndex].persistance, cameraPositions [camIndex].lacunarity, cameraPositions [camIndex].burstFrequency, cameraPositions [camIndex].burstContrast, Time.time);

			carCam.localPosition = camOffset + new Vector3(noise.x, noise.y, 0.0f);

			RaycastHit[] hits;
			hits = Physics.RaycastAll (car.position, (carCam.position - rootNode.position), (rootNode.position - carCam.position).magnitude);
			foreach(RaycastHit hit in hits) {
				Transform go = hit.collider.gameObject.transform.parent;
				Transform prevGo = go;
				while (go != null) {
					prevGo = go;
					go = go.transform.parent;
				}
				go = prevGo;
				if (go != car) {
					carCam.position = hit.point;
				}
			}

			// If the car isn't moving, default to looking forwards. Prevents camera from freaking out with a zero velocity getting put into a Quaternion.LookRotation
			if (carPhysics.velocity.magnitude < cameraPositions [camIndex].rotationThreshold)
				look = Quaternion.LookRotation (car.forward);
			else
				look = Quaternion.LookRotation (carPhysics.velocity.normalized);
        
			// Rotate the camera towards the velocity vector.
			look = Quaternion.Slerp(rootNode.rotation, look, cameraPositions[camIndex].cameraRotationSpeed * Time.fixedDeltaTime);                
			rootNode.rotation = look;
		}
    }

	void Update()
	{
		if (cameraPositions [camIndex].isStatic) {

			// Moves the camera to match the car's position.
			rootNode.position = car.position;
			rootNode.rotation = car.rotation;

			Vector2 noise = new Vector2 ();

			float speedAmplitude = Mathf.Clamp(carPhysics.velocity.magnitude, 0.0f, cameraShakeMaxSpeed) / cameraShakeMaxSpeed;

			if (isShaking)
				noise = NoiseGen.Shake2D (cameraPositions [camIndex].amplitude * speedAmplitude, cameraPositions [camIndex].frequency, cameraPositions [camIndex].octaves, cameraPositions [camIndex].persistance, cameraPositions [camIndex].lacunarity, cameraPositions [camIndex].burstFrequency, cameraPositions [camIndex].burstContrast, Time.time);

			carCam.localPosition = new Vector3 (cameraPositions [camIndex].xOffset, cameraPositions [camIndex].yOffset, cameraPositions [camIndex].zOffset) + new Vector3(noise.x, noise.y, 0.0f);
			carCam.rotation = rootNode.rotation;
		}

		//Check if Camere has to be changed
		if (Input.GetButtonDown (cameraAxisName)) {
			camIndex = (int)Mathf.Repeat (camIndex + 1, cameraPositions.Count);
		}

		if (cameraPositions [camIndex].canShake) {
			RaycastHit groundHit;
			if (Physics.Raycast (car.position, -car.up, out groundHit)) {
				if (groundHit.transform.tag == cameraShakeTag) {
					isShaking = true;
				} else {
					isShaking = false;
				}
			}
		} else {
			isShaking = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) {
			mainCamera = Camera.main;
			rootNode = GetComponent<Transform> ();
			car = rootNode.parent.GetComponent<Transform> ();
		}
		foreach (CameraPosition camera in cameraPositions) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawMesh (camMesh(), car.TransformPoint(camera.xOffset, camera.yOffset, camera.zOffset), Quaternion.LookRotation(car.forward), new Vector3(mainCamera.aspect * 0.5f, 0.5f, 0.5f));
		}
	}

	private Mesh camMesh() {
		Vector3 p0 = new Vector3(-0.5f,0.5f,1);
		Vector3 p1 = new Vector3(-0.5f,-0.5f,1);
		Vector3 p2 = new Vector3(0.5f,0.5f,1);
		Vector3 p3 = new Vector3(0.5f,-0.5f,1);
		Vector3 p4 = new Vector3(0,0,0);

		Mesh mesh = new Mesh ();
		mesh.Clear();
		mesh.vertices = new Vector3[]{p0,p1,p2,p3,p4};
		mesh.triangles = new int[] {
			3, 2, 0,
			0, 1, 3,
			1, 0, 4,
			0, 2, 4,
			2, 3, 4,
			3, 1, 4
		};

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}
}

[Serializable]
public class CameraPosition {

	public string name;

	[Tooltip("If checked: The camera will stay at a fixed Position outgoing from the car. The camera is also not avoiding walls etc...")]
	public bool isStatic = false;

	[Tooltip("X-Offset from the car to the camera")]
	public float xOffset = 0;

	[Tooltip("Y-Offset from the car to the camera")]
	public float yOffset = 0;

	[Tooltip("Z-Offset from the car to the camera")]
	public float zOffset = 0;

	[Tooltip("If car speed is below this value, then the camera will default to looking forwards.")]
	public float rotationThreshold = 1f;

	[Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
	public float cameraStickiness = 10.0f;

	[Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
	public float cameraRotationSpeed = 5.0f;

	[Header("Camera Shake Settings")]
	[Tooltip("If the camere should shake when the car hits a specific Surface")]
	public bool canShake;

	[Range(0,100)]
	public float amplitude = 1;
	[Range(0.00001f, 0.99999f)]
	public float frequency = 0.98f;

	[Range(1,4)]
	public int octaves = 2;

	[Range(0.00001f,5)]
	public float persistance = 0.2f;
	[Range(0.00001f,100)]
	public float lacunarity = 20;

	[Range(0.00001f, 0.99999f)]
	public float burstFrequency = 0.5f;

	[Range(0,5)]
	public int  burstContrast = 2;

}