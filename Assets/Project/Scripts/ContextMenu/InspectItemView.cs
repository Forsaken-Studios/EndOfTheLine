
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ContextMenu
{
    public class InspectItemView : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemWeightText;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private Image itemIcon;

        public void SetUpInspectView(Item item)
        {
            this.itemNameText.text = item.itemName.ToString();
            this.itemWeightText.text = item.itemWeight.ToString();
            this.itemTypeText.text = item.ItemType.ToString();
            this.itemDescriptionText.text = item.itemDescription.ToString();

            this.itemIcon.sprite = item.itemIcon;
        }
    }
}
