import json
import random

# Load weapon properties from the JSON file
with open("weapon_properties.json", "r") as file:
    weapon_props = json.load(file)

json_file_path = "weapons_data.json"

# Load weapons from the JSON file
with open(json_file_path, "r") as file:
    weapons = json.load(file)
    
print(f"Loaded {len(weapons)} weapons")

def calculate_weapon_power(weapon_data):
    """
    Calculates the power ranking of a weapon combination based on its attributes.
    """
    power = 0

    # Score based on fire mode
    power += weapon_props["FireMode"].index(weapon_data["fireMode"])

    # Score based on fire mode modifier
    if weapon_data["fireModeModifier"] is not None:
        power += (weapon_props["FireModeModifier"].index(weapon_data["fireModeModifier"]) + 1) * 2

    # Score based on projectile
    power += weapon_props["ProjectileType"].index(weapon_data["projectileType"])

    # Score based on projectile modifiers
    for modifier in weapon_data["projectileModifiers"]:
        power += (weapon_props["ProjectileModifier"].index(modifier) + 1) * 6

    # Score based on damage type
    power += weapon_props["DamageType"].index(weapon_data["damageType"]) * 3

    return power

def categorize_weapon(score, max_score):
    s = score / max_score
    if s < 0.5:
        return "Common"
    elif s < 0.8:
        return "Uncommon"
    elif s < 0.92:
        return "Rare"
    else:
        return "Legendary"
    
def weighted_choice(choices, weights):
    return random.choices(choices, weights)[0]

power_scores = []
for wpn in weapons:
    wpn["power"] = calculate_weapon_power(wpn)
    power_scores.append(wpn["power"])

max_power_score = max(power_scores)
ranked_weapons = {}
for wpn in weapons:
    wpn["rarity"] = categorize_weapon(wpn["power"], max_power_score)
    if wpn["rarity"] not in ranked_weapons:
        ranked_weapons[wpn["rarity"]] = []
    
    if wpn["rarity"] == "Common":
        if len(wpn["projectileModifiers"]) > 0:
            if not weighted_choice([False, True], [0.9, 0.1]):
                continue
        if len(wpn["projectileModifiers"]) > 1:
            continue
        if wpn["fireModeModifier"] != None:
            if not weighted_choice([False, True], [0.8, 0.2]):
                continue

    if wpn["rarity"] == "Uncommon":
        if len(wpn["projectileModifiers"]) > 1:
            if not weighted_choice([False, True], [0.8, 0.2]):
                continue
        if wpn["fireModeModifier"] != None:
            if not weighted_choice([False, True], [0.7, 0.3]):
                continue

    if wpn["rarity"] == "Rare":
        if not weighted_choice([False, True], [0.2, 0.8]):
            continue

    if wpn["rarity"] == "Legendary":
        if not weighted_choice([False, True], [0.3, 0.7]):
            continue

    ranked_weapons[wpn["rarity"]].append(wpn)

for rarity in ranked_weapons:
    print(f"Rarity: {rarity}: {len(ranked_weapons[rarity])}")

# Write the weapons JSON data to the output file
json_data = json.dumps(ranked_weapons, indent=4)
output_file = "ranked_weapons_data.json"
with open(output_file, "w") as file:
    file.write(json_data)

print("Scored weapons data has been written to", output_file)
