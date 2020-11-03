using UnityEngine;

public class ShockwaveFX : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite levelOneShockwave;
	[SerializeField] private Sprite levelTwoShockwave;
	[SerializeField] private Sprite levelThreeShockwave;

	private Material shockwaveMaterial;
	private int shockwaveLevel;
	private Sprite[] shockwaveLevelSprites;

	public int ShockwaveLevel
	{
		set
		{
			shockwaveLevel = value;
			int level = value - 1 >= 0 ? value - 1 : 0;
			{
				spriteRenderer.sprite = shockwaveLevelSprites[level];
			}
			if (shockwaveLevel == 3)
			{
				shockwaveMaterial.SetInt("_Advanced", 1);
			}
			else
			{
				shockwaveMaterial.SetInt("_Advanced", 0);
			}
		}
	}

	private void Awake()
	{
		shockwaveMaterial = spriteRenderer.material;
		shockwaveLevelSprites = new Sprite[3] { levelOneShockwave, levelTwoShockwave, levelThreeShockwave };
	}
}
