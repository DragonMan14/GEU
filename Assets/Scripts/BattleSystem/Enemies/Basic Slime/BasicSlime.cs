using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime : Enemy
{
    public GameObject Slimeball;

    public override void InitalizeAttackPool()
    {
        AddAttackToPool(new SlimeShotAttack(this.gameObject));
        AddAttackToPool(new Jump(this.gameObject));
       // AddAttackToPool(new Attack3());
    }

    #region Attacks
    private class SlimeShotAttack : EnemyAttack
    {
        private GameObject _basicSlime;
        private GameObject _slimeball;
        
        public SlimeShotAttack(GameObject _basicSlime)
        {
            this._basicSlime = _basicSlime;
            this._slimeball = _basicSlime.GetComponent<BasicSlime>().Slimeball;
            this.Damage = 5f;
        }
        public override void PerformAttack()
        {
            Vector3 spawnPosition = _basicSlime.transform.position + new Vector3(0.75f, 0, 0);
            GameObject shot = Instantiate(_slimeball, spawnPosition, Quaternion.identity);
            shot.GetComponent<SlimeShot>().Damage = this.Damage;
        }
    }

    private class Jump : EnemyAttack
    {
        private GameObject _basicSlime;
        private float _distance;

        public Jump(GameObject _basicSlime)
        {
            this._basicSlime = _basicSlime;
        }

        public override void PerformAttack()
        {
            // set animation to begin jump
            _distance = PlayerManager.Instance.PlayerCombat.transform.position.x - _basicSlime.transform.position.x; // fun line
            StartJump();
        }

        private void StartJump()
        {
            bool isLeft = _distance < 0;
            float absoluteDistance = Mathf.Min(Mathf.Abs(_distance), 6f); // Dist is player.x - slime.x, max is 6
            float jumpForce = Mathf.Max(Mathf.Pow(absoluteDistance, 0.85f), 1); // Min jump is 1, otherwise dist^0.85
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
            _basicSlime.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }

    public class Attack3 : EnemyAttack
    {
        public override void PerformAttack()
        {
            print("attack3");
        }
    }
    #endregion
}
