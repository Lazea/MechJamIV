import json
import random


# Load weapons from the JSON file
json_file_path = "weapons_data.json"
with open(json_file_path, "r") as file:
    weapons = json.load(file)

print(f"Loaded {len(weapons)} weapons")

# Grab common weapons
common = []
for weapon in weapons:
    if weapon["fireModeModifier"] == "None" and weapon["projectileType"] != "Rocket":
        _weapon = weapon.copy()
        _weapon["rarity"] = "Common"
        common.append(weapon.copy())

# Grab uncommon weapons
uncommon = []
for weapon in weapons:
    if weapon["fireModeModifier"] == "None" and weapon["projectileType"] != "Rocket":
        _weapon = weapon.copy()
        _weapon["baseDamage"] = int(_weapon["baseDamage"] * 1.25)
        _weapon["rarity"] = "Uncommon"
        uncommon.append(_weapon)
    elif weapon["fireModeModifier"] == "DualSplit":
        _weapon = weapon.copy()
        _weapon["rarity"] = "Uncommon"
        uncommon.append(_weapon)

# Grab rare weapons
rare = []
for weapon in weapons:
    _weapon = weapon.copy()
    _weapon["baseDamage"] = int(_weapon["baseDamage"] * 2.0)
    _weapon["baseFireRate"] = min(round(_weapon["baseFireRate"] / 1.15, 2), 0.9)
    _weapon["rarity"] = "Rare"
    rare.append(_weapon)

# Grab legendary weapons
legendary = []
for weapon in weapons:
    if weapon["fireModeModifier"] == "None":
        continue
    _weapon = weapon.copy()
    if _weapon["projectileType"] == "Rocket":
        _weapon["baseDamage"] = int(_weapon["baseDamage"] * 2.45)
    else:
        _weapon["baseDamage"] = int(_weapon["baseDamage"] * 3.0)
    _weapon["baseFireRate"] = min(round(_weapon["baseFireRate"] / 1.15, 2), 0.9)
    _weapon["rarity"] = "Legendary"
    legendary.append(_weapon)

# Create final dict
ranked_weapons = {
    "Common": common,
    "Uncommon": uncommon,
    "Rare": rare,
    "Legendary": legendary
}

for rarity in ranked_weapons:
    print(f"Rarity: {rarity}: {len(ranked_weapons[rarity])}")

# Write the weapons JSON data to the output file
json_data = json.dumps(ranked_weapons, indent=4)
output_file = "ranked_weapons_data.json"
with open(output_file, "w") as file:
    file.write(json_data)

print("Ranked weapons data has been written to", output_file)
    