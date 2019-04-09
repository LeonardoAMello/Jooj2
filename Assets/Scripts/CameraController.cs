using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = .125f;
    public Vector3 positionOffset, focusOffset;

    public float angleToView;
    public float distance = 2f; // The distance between the camera and the target
    [Range(2f, 3f)]
    public float height = 2.5f;

    public Vector2 sensibility;

    public PostProcessProfile intermediario, ultra;

    private void Start()
    {
        sensibility.x = PlayerPrefs.GetFloat("Sensibility X");
        sensibility.y = PlayerPrefs.GetFloat("Sensibility Y");

        if (sensibility.x == 0)
            sensibility.x = 75;
        if (sensibility.y == 0)
            sensibility.y = 20;

        AdjustGraphicSettings();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            angleToView += Input.GetAxis("Mouse X") * sensibility.x * Time.deltaTime;
            height -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensibility.y;

            if (height > 3)
                height = 3f;
            if (height < 2)
                height = 2f;

            Vector3 desiredPosition = LookAtDirection(angleToView);
            desiredPosition.y = height;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
            transform.LookAt(target.position + focusOffset);
        }
    }

    private Vector3 LookAtDirection(float angle)
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 50;

        if (distance > 5)
            distance = 5;

        if (distance < 2)
            distance = 2;
        // angle 0  -> front
        // angle 90 -> right

        while (angle > 360)
            angle -= 360;

        while (angle < 0)
            angle += 360;

        if (angle == 360)
            angle = 0;


        Vector3 cameraPosition = target.position + positionOffset;

        Vector3 rotationOffset = new Vector3();

        //if (angle < 180)
        //    rotationOffset.x = (angle / 180) * 4 - distance;
        //else
        //    rotationOffset.x = (Mathf.Abs(angle - 360) / 180) * 4 - distance;

        //if ((angle + 90) < 180)
        //    rotationOffset.z = ((angle + 90) / 180) * 4 - distance;
        //else
        //    rotationOffset.z = (Mathf.Abs((angle + 90) - 360) / 180) * 4 - distance;



        rotationOffset.x = -Mathf.Cos(Mathf.Abs(angle - 360) / (180 / Mathf.PI)) * distance;

        rotationOffset.z = -Mathf.Cos(Mathf.Abs(angle + 90 - 360) / (180 / Mathf.PI)) * distance;

        rotationOffset.y = (distance - 2) / 1.8f;

        return cameraPosition + rotationOffset;
    }

    public void AdjustGraphicSettings()
    {
        if (QualitySettings.GetQualityLevel() == 0)
            GetComponent<PostProcessLayer>().enabled = false;
        else if (QualitySettings.GetQualityLevel() == 1)
            GetComponent<PostProcessVolume>().profile = intermediario;
        else if (QualitySettings.GetQualityLevel() == 2)
            GetComponent<PostProcessVolume>().profile = ultra;

        AdjustParticles();
    }

    public static void AdjustParticles()
    {
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();

        for (int i = 0; i < particles.Length; i++)
            if (QualitySettings.GetQualityLevel() == 0)
            {
                if (particles[i].tag.Equals("OnlyUltra"))
                    Destroy(particles[i]);
                if (particles[i].tag.Equals("IntermediarioOrBetter"))
                    Destroy(particles[i]);
            }
            else if (QualitySettings.GetQualityLevel() == 1)
            {
                if (particles[i].tag.Equals("OnlyUltra"))
                    Destroy(particles[i]);
            }
    }
}
