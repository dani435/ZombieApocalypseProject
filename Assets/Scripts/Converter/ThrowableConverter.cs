//Classe per gestire il lancio del converter

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableConverter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject objectToThrow;
    [SerializeField] private GameObject growingAreaPrefab;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwardForce;

    [Header("Growing Area")]
    [SerializeField] private float growthRate;
    [SerializeField] private float maxRadius;

    [Header("Trajectory")]
    [SerializeField] private Color trajectoryColor = Color.yellow;
    [SerializeField] private float trajectoryWidth = 0.1f;
    [SerializeField] private Material trajectoryMaterial;

    [Header("Particle Effect")]
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private float particleRadius = 3f;

    private LineRenderer trajectoryPreview;
    public bool readyToThrow;
    private ParticleSystem previewParticleEffect;

    [SerializeField] private Image converter;

    //Evento richiamato quando si raccoglie il random power up ed esce il converter
    public void OnEnable()
    {
        RandomUP.OnConverterCollected += ConverterCollected;
    }

    public void OnDisable()
    {
        RandomUP.OnConverterCollected -= ConverterCollected;
    }

    public void ConverterCollected()
    {
        readyToThrow = true;
    }

    void Start()
    {
        readyToThrow = false;

        // creazione della traiettoria
        trajectoryPreview = gameObject.AddComponent<LineRenderer>();
        trajectoryPreview.enabled = false;
        trajectoryPreview.startColor = trajectoryColor;
        trajectoryPreview.endColor = trajectoryColor;
        trajectoryPreview.startWidth = trajectoryWidth;
        trajectoryPreview.endWidth = .2f;
        trajectoryPreview.numCornerVertices = 32;
        trajectoryPreview.material = trajectoryMaterial;

        //creazione delle particelle
        previewParticleEffect = Instantiate(particleEffect, attackPoint.position, Quaternion.identity);
        previewParticleEffect.Stop();
    }

    void Update()
    {
        if (readyToThrow == true)
        {
            //Gestione del lancio del converter
            converter.enabled = true;
            if (Input.GetKeyDown(throwKey) && readyToThrow)
            {
                // Abilita la traiettoria
                trajectoryPreview.enabled = true;
                PreviewTrajectory();
                previewParticleEffect.Clear();
                previewParticleEffect.time = 0;
            }

            if (Input.GetKey(throwKey) && readyToThrow)
            {
                previewParticleEffect.Play();

                // aggiornamento della traiettoria al movimento della camera
                PreviewTrajectory();

            }

            if (Input.GetKeyUp(throwKey) && readyToThrow)
            {

                trajectoryPreview.enabled = false;
                previewParticleEffect.time = 0;
                previewParticleEffect.Stop();
                previewParticleEffect.Clear();
                // lancio del converter
                Throw();
                readyToThrow = false;
            }
        }
        else
        {
            converter.enabled = false;
        }
    }

    // gestione del lancio del converter dopo aver rilasciato il tasto
    private void Throw()
    {
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, Quaternion.identity);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.forward;
        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        // Torque force
        Vector3 torque;
        torque.x = Random.Range(-200, 200);
        torque.y = Random.Range(-200, 200);
        torque.z = 0;
        projectileRb.AddTorque(torque, ForceMode.Impulse);

        // creazione dell'area di esplosione
        StartCoroutine(GrowArea(projectile));

    }

    // creazione visiva della traiettoria
    private void PreviewTrajectory()
    {
        trajectoryPreview.positionCount = 0;

        // posizione iniziale
        trajectoryPreview.positionCount = 1;
        trajectoryPreview.SetPosition(0, attackPoint.position);

        Vector3 position = attackPoint.position;
        Vector3 velocity = (cam.forward * throwForce) + (Vector3.up * throwUpwardForce);
        float timeStep = 0.1f;
        float maxTime = 10f;

        int pointCount = Mathf.FloorToInt(maxTime / timeStep);
        trajectoryPreview.positionCount = pointCount + 1;

        for (int i = 1; i <= pointCount; i++)
        {
            float t = i * timeStep;
            position += velocity * timeStep;
            velocity += Physics.gravity * timeStep;
            trajectoryPreview.SetPosition(i, position);

            if (position.y <= 0.01f)
            {
                trajectoryPreview.positionCount = i + 1;
                position.y = -1f;
                trajectoryPreview.SetPosition(i, position);
                break;
            }
        }
        //Update della visione delle particelle

        previewParticleEffect.transform.position = position + new Vector3(0, 0f, 0);
        previewParticleEffect.transform.rotation = Quaternion.Euler(90, 0, 0);
        var shape = previewParticleEffect.shape;
        shape.radius = particleRadius;
    }

    //creazione area d'impatto
    private IEnumerator GrowArea(GameObject projectile)
    {
        // aspetta fino a che il converter non colpisca terra
        yield return new WaitUntil(() => projectile.transform.position.y - 0.2 <= 0f);

        // posizione del terreno colpita
        Vector3 hitPoint = new Vector3(projectile.transform.position.x, 0f, projectile.transform.position.z);

        // creazione dell'area
        GameObject growingArea = Instantiate(growingAreaPrefab, hitPoint, Quaternion.identity);
        FindObjectOfType<AudioManager>().Play("pickUpPowerUp");

        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            // incremento dell'area frame per frame
            currentRadius += growthRate * Time.deltaTime;
            growingArea.transform.localScale = Vector3.one * currentRadius * 2f;

            // controllo dei collider presenti all'interno dell'area
            Collider[] colliders = Physics.OverlapSphere(hitPoint, currentRadius);
            foreach (Collider collider in colliders)
            {
                // cambiamento del tag se ci sono nemici all'interno dell'area
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    collider.gameObject.tag = "Ally";

                }
            }

            yield return null;
        }


        // Destroy del visual effect dopo aver lanciato
        Destroy(projectile);
        Destroy(growingArea);
    }
}



