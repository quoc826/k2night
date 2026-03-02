using UnityEngine;

public interface IcanTakeDamage 
{
   public void TakeDamage(int damageAmount, Vector2 hitPoint, GameObject hitDirection);
}
