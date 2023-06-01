using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicSlime : Enemy
{
    [SerializeField] private GameObject _slimeball;
    private readonly float minEndlag = 1f;
    private readonly float maxEndlag = 2f;

    private void Start()
    {
        StartCoroutine(EnemyAI());
    }

    public override void InitalizeAttackPool()
    {
        // 0 = Slimeball
        AddAttackToPool(new SlimeShotAttack(this.gameObject));
        // 1 = Jump
        AddAttackToPool(new Jump(this.gameObject));
       // AddAttackToPool(new Attack3());
    }

    public override IEnumerator EnemyAI()
    {
        while (true)
        {
            if (!CanAttack || CurrentlyAttacking)
            {
                yield return null;
                continue;
            }
            float endlag = UnityEngine.Random.Range(minEndlag, maxEndlag);
            yield return new WaitForSeconds(endlag);
            CurrentlyAttacking = true;
            yield return PerformRandomAttack();
        }
    }

    public override EnemyAttack GetRandomAttack()
    {
        // If in the air, slime can only do slimeshot
        if (!CurrentlyGrounded())
        {
            return _attackPool[0];
        }
        // Else randomly choose between slimeshot and jump
        int attackNum = UnityEngine.Random.Range(0, _attackPool.Count);
        return _attackPool[attackNum];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_groundCheck.position, _groundCheckDimensions);
    }

    #region Attacks
    private class SlimeShotAttack : EnemyAttack
    {
        private readonly GameObject _slimeObject;
        private readonly BasicSlime _slimeScript;
        private readonly GameObject _slimeball;
        
        public SlimeShotAttack(GameObject _basicSlime)
        {
            this._slimeObject = _basicSlime;
            this._slimeScript = _basicSlime.GetComponent<BasicSlime>();
            this._slimeball = _basicSlime.GetComponent<BasicSlime>()._slimeball;
            this.Damage = 5f;
        }
        public override IEnumerator PerformAttack()
        {
            yield return null;
            // Shooting the ball
            Vector3 spawnPosition = _slimeObject.transform.position + new Vector3(0.75f, 0, 0);
            GameObject shot = Instantiate(_slimeball, spawnPosition, Quaternion.identity);
            shot.GetComponent<SlimeShot>().Damage = this.Damage;
            // Attack is done
            _slimeScript.CurrentlyAttacking = false;
        }
    }

    private class Jump : EnemyAttack
    {
        private readonly GameObject _slimeObject;
        private readonly BasicSlime _slimeScript;

        public Jump(GameObject _basicSlime)
        {
            this._slimeObject = _basicSlime;
            this._slimeScript = _basicSlime.GetComponent<BasicSlime>();
        }

        public override IEnumerator PerformAttack()
        {
            // Get distance and jump
            float distance = PlayerManager.Instance.PlayerCombat.transform.position.x - _slimeObject.transform.position.x; // fun line
            yield return StartJump(distance);
            // Attack is done
            _slimeScript.CurrentlyAttacking = false;
            print("jump");
        }

        private IEnumerator StartJump(float distance)
        {
            bool isLeft = distance < 0;
            float absoluteDistance = Mathf.Min(Mathf.Abs(distance), 10f); // Dist is player.x - slime.x, max is 6
            float jumpForce;
            
            if (absoluteDistance < 2)
            {
                float jumpDistance = Mathf.Pow(absoluteDistance, -1f) + 2;
                jumpForce = Mathf.Clamp(jumpDistance, 1, 6f);
            }
            else
            {
                float jumpDistance = Mathf.Pow(absoluteDistance, 0.85f) - (Mathf.Pow(2, 0.85f) - 0.5f);
                jumpForce = Mathf.Clamp(jumpDistance, 1, 6);
            }
            jumpForce = 10;
            Vector2 force;
            if (isLeft)
            {
                force = new Vector2(-absoluteDistance, jumpForce);
            }
            else
            {
                force = new Vector2(absoluteDistance, jumpForce);
            }
            // idfk what impluse does lol :)
            _slimeObject.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            // Wait for jump to be done
            yield return null;
            while (_slimeObject.GetComponent<Rigidbody2D>().velocity.y != 0) 
            {
                print("in loop");
                yield return null;
            }
            // Reset the velocity
            _slimeObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public class Attack3 : EnemyAttack
    {
        public override IEnumerator PerformAttack()
        {
            print("attack3");
            yield return null;
        }
    }
    #endregion
}
