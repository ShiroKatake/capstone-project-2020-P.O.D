using UnityEngine;

/// <summary>
/// This will be used to determine whether the object is an ally or an enemy to other objects.
/// </summary>
public class Actor  : MonoBehaviour
{
	[SerializeField] private int affiliation;
	public int Affiliation { get => affiliation; }
}
