using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// A UIController for the StatusEffects panel on the battle UI.
/// </summary>
public class UIStatusEffects : MonoBehaviour {
	public Image prefab;

	private List<Image> statusIcons = new List<Image>();
	private float iconStartPosX;
	private float iconSize;
	private float iconOffset;
	private float iconSpacing;

	//TODO: image scale bug - does not scale with container
	void Start () {
		RectTransform containerRect = gameObject.GetComponent<RectTransform>();
		iconStartPosX = -containerRect.rect.width / 2;
		iconSize = containerRect.rect.height;
		iconOffset = iconSize / 2;
		iconSpacing = iconSize / 6;
	}

	public void AddStatusIcon(Sprite icon) {
		// create the image
		Image newStatus = Instantiate(prefab) as Image;
		newStatus.name = "newItem";
		newStatus.transform.SetParent(gameObject.transform);
		newStatus.sprite = icon;
		
		// set the positions
		CalculateIconPosition(newStatus, statusIcons.Count);
		statusIcons.Add(newStatus);
	}

	public void RemoveStatusIconAt(int index) {
		RemoveStatus(index);

		// recalculate the positions of the icons
		if (index != statusIcons.Count-1) {
			for (int i = 0; i < statusIcons.Count; i++) {
				CalculateIconPosition(statusIcons[i], i);
			}
		}
	}

	public void RemoveAllStatusIcons() {
		for (int i = statusIcons.Count-1; i >= 0; i--) {
			RemoveStatus(i);
		}
	}

	private void RemoveStatus(int index) {
		Image img = statusIcons[index];
		Destroy(img.gameObject);
		statusIcons.RemoveAt(index);
	}

	private void CalculateIconPosition(Image newImage, int index) {
		RectTransform imageRect = newImage.GetComponent<RectTransform>();
		//imageRect.sizeDelta = new Vector2(iconSize, iconSize);
		
		float offset = (iconSize * index) + (iconSpacing * index-1) + iconOffset;
		float x = iconStartPosX + offset;
		imageRect.anchoredPosition = new Vector2(x, 0);
	}
}
