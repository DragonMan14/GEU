using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime : Enemy
{
    public GameObject Slimeball;

    public override void InitalizeAttackPool()
    {
        AddAttackToPool(new SlimeShotAttack(this.gameObject));
        AddAttackToPool(new Attack2());
        AddAttackToPool(new Attack3());
    }

    #region Attacks
    public class SlimeShotAttack : EnemyAttack
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

    public class Attack2 : EnemyAttack
    {
        public override void PerformAttack()
        {
            print("attack2");
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
