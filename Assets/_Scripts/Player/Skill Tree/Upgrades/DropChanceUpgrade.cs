using UnityEngine;

public class DropChanceUpgrade : Upgrade
{
    private float newDropChance;

    public DropChanceUpgrade(string name, int skillPointsCost, float newDropChance)
        : base(name, skillPointsCost)
    {
        this.newDropChance = newDropChance;
    }

    public override void ApplyUpgrade()
    {
        EnemyHealthController[] enemyHealthControllers = GameObject.FindObjectsOfType<EnemyHealthController>();
        foreach (var enemyHealthController in enemyHealthControllers)
        {
            enemyHealthController.SetDropChance(newDropChance);
        }
    }
}