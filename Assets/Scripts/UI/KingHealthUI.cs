using UnityEngine;

public class KingHealthUI : MonoBehaviour
{
    [SerializeField]
    private PlayerUnitController kingController;
    
    [SerializeField]
    private RectTransform healthBarBackground;

    [SerializeField]
    private RectTransform healthBarFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.kingController.Health.HealthChanged += PlayerHealthChanged;
    }

    private void PlayerHealthChanged( object sender, UnitHealth.HealthChangedEventArgs e )
    {
        float healthPercentage = e.NewHealth / kingController.Health.MaxHealth;
        healthBarFill.sizeDelta = new Vector2( healthPercentage * healthBarBackground.sizeDelta.x, healthBarFill.sizeDelta.y );
    }
}
