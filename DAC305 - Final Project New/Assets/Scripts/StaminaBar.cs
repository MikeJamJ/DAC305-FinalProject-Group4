using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{   
    [SerializeField] private float regenSpeed;  // Time between regeneration ticks
    [SerializeField] private float regenWait;   // Time before regen starts

    public Slider staminaBar;   // Stamina bar slider
    public static StaminaBar instance;  // create an instance of the stamina bar

    // Timer for controlling how fast stamina is regenerated
    private WaitForSeconds regenTick; 
    private Coroutine regen;

    private int maxStamina = 2000;  // max stamina of character
    private int currentStamina;     // current stamina of character    
    private int timesEmptied = 0;   // number of times the stamina bar was emptied

    private Average averageStamina = new Average(); // average stamina over level

    // Run once the first time the scene is loaded in
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {   
        // Set all variables to maximum stamina when the object is loaded in
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        // Set stamina regeneration timer
        regenTick = new WaitForSeconds(regenSpeed);
    }

    // Update runs every frame
    void Update() {
        averageStamina.update(currentStamina);
    }

    // Function for using stamina
    public void UseStamina(int amount)
    {   
        if(currentStamina - amount > 0)
        {   
            // If there is still stamina in the bar, use some
            currentStamina -= amount;
            staminaBar.value = currentStamina;

            if (regen != null)
                StopCoroutine(regen);

            regen = StartCoroutine(RegenStamina());
        }
        else
        {   
            // If there is no stamina, debug
            currentStamina = 0;
            staminaBar.value = currentStamina;
            timesEmptied++;
        }
    }

    // Function for increasing character stamina
    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(regenWait); // Time before regen starts
        
        while(currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100; // Fills stamina bar at standard rate (1%)
            staminaBar.value = currentStamina;
            yield return regenTick;
        }
        regen = null;
    }

    // Function for getting the current stamina of the bar
    public int getStamina() {
        return currentStamina;
    }

    // Fucntion for returning timesEmptied
    public int getTimesEmptied() {
        return timesEmptied;
    }

    // Function for returning the average stamina
    public float getAverage() {
        return averageStamina.getAverage() / maxStamina;
    }
}
