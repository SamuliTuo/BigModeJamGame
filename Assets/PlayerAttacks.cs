using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private NormalAttackCollider attackCollider = null;

    private ThirdPersonController _controller;
    private Animator _animator;
    private CharacterController _charController;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private bool attacking = false;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();

        _animator = GetComponent<Animator>();
        _charController = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    
    public void UpdateAttacks()
    {
        if (attacking)
            return;

        if (_input.attack)
        {
            if (_controller.Mode == PlayerModes.NORMAL)
            {
                attackCollider.gameObject.SetActive(true);
                attackCollider.InitAttack();
                attacking = true;
                StartCoroutine(NormalAttack());
            }
            else if (_controller.Mode == PlayerModes.URBAN)
            {

            }
            else if (_controller.Mode == PlayerModes.ZONE)
            {

            }
            _input.attack = false;
        }
    }

    private float normalAttackColDuration = 0.2f;
    IEnumerator NormalAttack()
    {
        yield return new WaitForSeconds(normalAttackColDuration);
        attacking = false;
        attackCollider.StopAttack();
        attackCollider.gameObject.SetActive(false);
    }
    IEnumerator UrbanAttack()
    {
        yield return null;
    }
    IEnumerator ZoneAttack()
    {
        yield return null;
    }
}