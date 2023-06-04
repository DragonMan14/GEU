using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BasicSlime : Enemy
{
    [SerializeField] private GameObject _slimeball;
    private readonly float minEndlag = 1f;
    private readonly float maxEndlag = 2f;

    public override void InitalizeAttackPool()
    {
        // 0 == Slimeball
        AddAttackToPool(new SlimeShotAttack(this.gameObject));
        // 1 == Jump
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
            SpawnSlimeBall(_slimeScript.Direction);
            // Attack is done
            _slimeScript.CurrentlyAttacking = false;
        }
        private void SpawnSlimeBall(Facing direction)
        {
            Vector3 spawnPosition;
            if (direction == Facing.right)
            {
                spawnPosition = _slimeObject.transform.position + new Vector3(0.75f, 0, 0);
            }
            else
            {
                spawnPosition = _slimeObject.transform.position + new Vector3(-0.75f, 0, 0);
            }
            SlimeShot shot = Instantiate(_slimeball, spawnPosition, Quaternion.identity).GetComponent<SlimeShot>();
            shot.Damage = this.Damage;
            shot.Pew(direction);
        }
    }

    private class Jump : EnemyAttack
    {
        private readonly GameObject _slimeObject;
        private readonly BasicSlime _slimeScript;
        private readonly float _minJumpDistance = 4f;
        private readonly float _maxJumpDistanceClose = 6f;
        private readonly float _maxJumpDistanceFar = 12f;

        public Jump(GameObject _basicSlime)
        {
            this._slimeObject = _basicSlime;
            this._slimeScript = _basicSlime.GetComponent<BasicSlime>();
        }

        public override IEnumerator PerformAttack()
        {
            // Get distance and jump
            Vector2 distance = PlayerManager.Instance.PlayerCombat.transform.position - _slimeObject.transform.position; // fun line
            yield return StartJump(distance);
            // Attack is done
            _slimeScript.CurrentlyAttacking = false;
        }

        private IEnumerator StartJump(Vector2 distance)
        {
            bool isLeft = distance.x < 0;
            float absoluteXDistance = Mathf.Min(Mathf.Abs(distance.x), 6f); // Dist is player.x - slime.x, max is 8
            float absoluteYDistance = Mathf.Min(Mathf.Abs(distance.y), 6f);
            float jumpForce;
            
            if (absoluteXDistance < 2)
            {
                float jumpDistance = Mathf.Pow(absoluteXDistance, -1f) + 2;
                jumpForce = Mathf.Clamp(jumpDistance, _minJumpDistance, _maxJumpDistanceClose);
            }
            else
            {
                float jumpDistance = Mathf.Pow(absoluteXDistance, 0.85f) - (Mathf.Pow(2, 0.85f) - 0.5f);
                jumpForce = Mathf.Clamp(jumpDistance, _minJumpDistance, _maxJumpDistanceFar);
            }

            // If there is a height difference between the player and slime, increase jump force proportional to that distance
            if (absoluteYDistance > 1)
            {
                jumpForce += absoluteYDistance * 1.25f;
            }

            absoluteXDistance = Mathf.Clamp(absoluteXDistance, 0f, 6f);
            Vector2 force;
            if (isLeft)
            {
                force = new Vector2(-absoluteXDistance, jumpForce);
            }
            else
            {
                force = new Vector2(absoluteXDistance, jumpForce);
            }

            // idfk what impluse does lol :)
            _slimeObject.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            // Wait for jump to be done
            yield return null;
            while (_slimeObject.GetComponent<Rigidbody2D>().velocity.y != 0) 
            {
                yield return null;
            }
            // Reset the velocity
            _slimeObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    #endregion
}
