import itertools
import json

# Load weapon properties from the JSON file
with open("weapon_properties.json", "r") as file:
    weapon_props = json.load(file)

print()

def generate_combinations(*args):
    """
    Generates all possible combinations of the provided lists.
    """
    combinations = list(itertools.product(*args))
    return combinations

# Generate projectile modifiers
projectile_modifiers = generate_combinations(
    weapon_props["ProjectileModifier"],
    weapon_props["ProjectileModifier"],
    weapon_props["ProjectileModifier"])
projectile_modifiers = [tuple(item for item in tpl if item != 'None') for tpl in projectile_modifiers]

# Clean projectile modifiers lsit to exclude duplicates and repeats
_projectile_modifiers = []
for tup in projectile_modifiers:
    unique_set = set(tup)
    if len(tup) == len(unique_set):
        _projectile_modifiers.append(tup)

# Convert the list of tuples to a set of frozensets
unique_set = {frozenset(pair) for pair in _projectile_modifiers}

# Convert the set of frozensets back to a list of tuples
unique_projectile_modifiers = [tuple(pair) for pair in unique_set]

# Generate all weapon combinations
weapons = generate_combinations(
    weapon_props["FireMode"],
    weapon_props["FireModeModifier"],
    weapon_props["ProjectileType"],
    unique_projectile_modifiers,
    weapon_props["DamageType"])

# Iterate over each weapon combination
dict = []
for wpn in weapons:
    # Create a dictionary to represent weapon data
    weapon_data = {
        "fireMode": wpn[0],
        "fireModeModifier": wpn[1],
        "projectileType": wpn[2],
        "projectileModifiers": list(wpn[3]),
        "damageType": wpn[4]
    }

    # Skip certain invalid combinations
    if weapon_data["fireMode"] == "SemiAuto":
        if weapon_data["fireModeModifier"] == "RampUpFire":
            continue

    if weapon_data["fireMode"] == "BurstFire":
        if weapon_data["projectileType"] == "Laser":
            continue
        if weapon_data["fireModeModifier"] == "RampUpFire":
            continue

    skip = False
    for mod in weapon_data["projectileModifiers"]:
        if mod == "Ricochet":
            if weapon_data["projectileType"] in ["EnergyBeam", "Rocket", "Laser"]:
                skip = True
                break
        if mod == "ExplodeOnHit":
            if weapon_data["projectileType"] == "Laser":
                skip = True
                break
        if mod == "ClusterOnHit":
            if weapon_data["projectileType"] in ["Ballistic", "Laser"]:
                skip = True
                break
        if mod == "Penetrate":
            if weapon_data["projectileType"] in ["Rocket", "Laser"]:
                skip = True
                break
    if skip:
        continue

    if weapon_data["damageType"] == "Normal" and weapon_data["projectileType"] in ["EnergyBeam", "Laser"]:
        continue

    if weapon_data["damageType"] == "Energy" and weapon_data["projectileType"] in ["Ballistic", "Rocket"]:
        continue

    # Append valid weapon data to the list
    dict.append(weapon_data)

print(f"Generated {len(dict)} weapons")

# Write the weapons JSON data to the output file
json_data = json.dumps(dict, indent=4)
output_file = "weapons_data.json"
with open(output_file, "w") as file:
    file.write(json_data)

print("Weapons data has been written to", output_file)
