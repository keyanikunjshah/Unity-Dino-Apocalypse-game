using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public Transform firepoint;
    public float fireRate = 0.1f;

    public float fireRange = 10f;

    public float nextFireTime = 0f;

    public bool isAuto = false;

    public int maxAmmo = 30;

    private int currentAmmo;

    public float reloadTime = 1.5f;

    private bool isReloading = false;

    public Animator animator;
    public ParticleSystem muzzleFlash;

    public ParticleSystem bloodEffect;

    public int damagePerShot = 10;
    public WFX_LightFlicker muzzleLightFlicker;

    [Header("Sound Effects")]

    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;

    public AudioClip ReloadSoundClip;

    void Start()
    {
        currentAmmo = maxAmmo;
    }
    void Update()
    {
        if (isReloading)
            return;
        if (isAuto == true)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time / fireRate;
                Shoot();

            }
            else
            {
                animator.SetTrigger("isshooting");
            }
        }

        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time / fireRate;
                Shoot();
            }
            else
            {
                animator.ResetTrigger("isshooting");
            }

        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }

    }

    public void Shoot()
    {
        if (currentAmmo > 0)
        {

            RaycastHit hit;
            if (Physics.Raycast(firepoint.position, firepoint.forward, out hit, fireRange))
            {
                Debug.Log("Hit: " + hit.transform.name);
                Debug.Log(hit.transform.name);
                //add damage to animal
                DinosaurAI dinosaurAI = hit.collider.GetComponent<DinosaurAI>();
                if (dinosaurAI != null)
                {
                    dinosaurAI.TakeDamage(damagePerShot);
                    ParticleSystem blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood.gameObject, blood.main.duration);
                    Debug.Log("Damage applied: " + damagePerShot);

                }
                WaypointDinosaurAI dinosaur = hit.transform.GetComponent<WaypointDinosaurAI>();
                if (dinosaur != null)
                {
                    Debug.Log("Dinosaur hit! Calling TakeDamage.");
                    dinosaur.TakeDamage(damagePerShot);
                }
                else
                {
                    Debug.Log("Hit object, but no WaypointDinosaurAI component found on it.");
                }


            }
            muzzleFlash.Play();
            muzzleLightFlicker.FlickerOnce();

            if (shootingSoundClip != null)
            {
                soundAudioSource.PlayOneShot(shootingSoundClip);
                Debug.Log("Shooting sound played.");
            }
            else
            {
                Debug.LogError("Shooting sound clip is not assigned!");
            }

            animator.SetTrigger("isshooting");
            currentAmmo--;


        }
        if (currentAmmo <= 0)
        {
            Reload();
        }



    }
    private void Reload()
    {
        if (!isReloading && currentAmmo <= 0)
        {
            animator.SetBool("reloading", true);
            isReloading = true;
            soundAudioSource.PlayOneShot(ReloadSoundClip);
            Invoke("FinishReloading", reloadTime);

        }
    }

    private void FinishReloading()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.SetBool("reloading", false);

        //reset reload anim

    }



}
