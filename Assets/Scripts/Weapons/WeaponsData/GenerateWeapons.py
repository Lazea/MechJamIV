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
        "projectileType": wpn[2],
        "projectileModifiers": list(wpn[3]),
        "fireMode": wpn[0],
        "fireModeModifier": wpn[1],
        "damageType": wpn[4]
    }

    # Get base accuracy
    for accuracy in weapon_props["BaseAccuracy"]:
        if accuracy["FireMode"].lower() == weapon_data["fireMode"].lower():
            weapon_data["baseAccuracy"] = accuracy["BaseAccuracy"]

            if weapon_data["projectileType"] == "Plasma":
                weapon_data["baseAccuracy"] = round(weapon_data["baseAccuracy"] * 0.25, 3)
            break

    # Get base accuracy
    for rate in weapon_props["BaseFireRate"]:
        if rate["FireMode"].lower() == weapon_data["fireMode"].lower():
            if rate["ProjectileType"].lower() == weapon_data["projectileType"].lower():
                weapon_data["baseFireRate"] = rate["BaseFireRate"]
                break
    if weapon_data["fireModeModifier"] == "Cluster":
        weapon_data["baseFireRate"] = weapon_data["baseFireRate"] * 4.25

    # Get base accuracy
    for damage in weapon_props["BaseDamage"]:
        if damage["FireMode"].lower() == weapon_data["fireMode"].lower():
            if damage["ProjectileType"].lower() == weapon_data["projectileType"].lower():
                weapon_data["baseDamage"] = damage["BaseDamage"]

                if weapon_data["damageType"] == "Normal":
                    weapon_data["baseDamage"] += 4
                break

    if weapon_data["projectileType"] == "Rocket" and weapon_data["fireModeModifier"]  in ["TrippleSplit", "Cluster"]:
        continue

    if weapon_data["damageType"] == "Normal" and weapon_data["projectileType"] in ["Plasma", "Laser"]:
        continue

    if weapon_data["damageType"] == "Energy" and weapon_data["projectileType"] in ["Ballistic", "Rocket"]:
        continue

    if weapon_data["damageType"] == "Shock" and weapon_data["projectileType"] in ["Ballistic", "Rocket"]:
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
