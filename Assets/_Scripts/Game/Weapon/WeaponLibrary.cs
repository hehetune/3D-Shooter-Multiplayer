
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WeaponLibrary
{
	public static readonly Dictionary<string, Prefab> Weapons;

	static WeaponLibrary()
	{
		Weapons = Resources.LoadAll<Prefab>("Prefabs/Weapon")
								.ToDictionary(prefab => prefab.name, prefab => prefab.GetComponent<Prefab>());
	}
}