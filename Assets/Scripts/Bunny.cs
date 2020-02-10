using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    male,
    female
}
public class Bunny : Animal
{
    private Gender gender;
    private string type;
    private void Start()
    {
        SetHealth(1);
        SetDamage(0);

        if(Random.Range(0, 1) == 0)
        {
            SetGender(Gender.male);
        }
        else
        {
            SetGender(Gender.female);
        }
    }

    public void SetGender(Gender gender)
    {
        this.gender = gender;
    }

    public Gender GetGender()
    {
        return this.gender;
    }
}
