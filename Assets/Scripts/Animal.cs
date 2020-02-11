using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Animal : MonoBehaviourPun
{
    private int health;
    private int damage;

    public void DealDamageToAnimal(Animal animal, int deal_damage)
    {
        animal.SetHealth(animal.GetHealth() - deal_damage);
        if (animal.GetHealth() <= 0)
        {
            animal.Die();
        }
    }

    public void DealDamageToPlayer(Hunter hunter, int deal_damage)
    {

    }

    public void Die()
    {
        // todo make the player go ghost mode
        Debug.Log("Dead");
    }

    public int GetHealth()
    {
        return this.health;
    }

    public int GetDamage()
    {
        return this.damage;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
